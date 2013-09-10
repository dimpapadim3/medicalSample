using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Linq;
using Business.Helpers;
using Business.Interfaces;
using Common;
using DataAccess.Interfaces;
using ErrorClasses;
using Model.DailyInfo;
using Model.Questionnaire;
using ServiceModel.DailyInfo;
using ServiceModel.Questionnaire;
using StructureMap;
using Controller = ServerDateTimeProvider.Controller;
using Question = ServiceModel.Questionnaire.Question;
using QuestionAnwer = ServiceModel.Questionnaire.QuestionAnwer;
using QuestionCategory = Model.Questionnaire.QuestionCategory;
using QuestionnaireTemplate = ServiceModel.Questionnaire.QuestionnaireTemplate;

namespace Business.Questionnaire
{
    internal class QuestionnaireService : IQuestionnaireService
    {
        public QuestionnaireService()
        {
            _questionnaireBuilder = new QuestionnaireBuilder();
        }

        #region Repositories

        public IRepositoryBase<QuestionnaireAnswers> QuestionnaireAnswersRepository { get; set; }
        public IEventsService EventsService { get; set; }


        public CoreScoreAnswer GetAverageCoreScoreForPeriod(int userId, int period, DateTime currentTime,
                                                            string locale)
        {
            IEnumerable<QuestionnaireAnswers> lattestSummaries = GetQuestionnaireAnswers(userId, period, currentTime);

            IList<QuestionnaireAnswers> questionnaireAnswerses = lattestSummaries as IList<QuestionnaireAnswers> ??
                                                                 lattestSummaries.ToList();
            if (questionnaireAnswerses.Any())
            {
                var averageCoreScores =
                    (int)
                    Math.Round(((float)questionnaireAnswerses.Sum(c => c.CoreScore) / questionnaireAnswerses.Count()));
                return GetLocalizedCoreScore(locale, (byte)averageCoreScores);
            }
            return GetNullCoreScore();
        }

        private IEnumerable<QuestionnaireAnswers> GetQuestionnaireAnswers(int userId, int period,
                                                                          DateTime currentTime)
        {
            DateTime fromDateTime = currentTime;
            Func<DateTime, bool> isIncluded = d => { throw new Exception("Not Valid Period"); };

            if (period == 0)
                isIncluded =
                    d =>
                    DataUtils.CompareDatesAfterNotIncludingStartDate(
                        fromDateTime.AddDays(-Constants.QUESTIONNAIRE_NUMBER_OF_DAYS_RECORDS), d.Date) &&
                    DataUtils.CompareDatesBeforeEndDate(d.Date, currentTime); //take 2 weeks 
            if (period == 1)
                isIncluded =
                    d =>
                    DataUtils.CompareDatesInTimeSpan(
                        fromDateTime.AddDays(-Constants.QUESTIONNAIRE_NUMBER_OF_WEEKS_RECORDS), currentTime, d.Date);
            //take 5 weeks
            if (period == 2)
                isIncluded =
                    d =>
                    (fromDateTime.AddMonths(-Constants.NUMBER_OF_MONTH_RECORDS).Month <= d.Date.Month) &&
                    (currentTime.Month >= d.Date.Month);
            if (period == 3)
                isIncluded =
                    d =>
                    (fromDateTime.AddYears(-Constants.NUMBER_OF_YEAR_RECORDS).Year <= d.Date.Year) &&
                    (currentTime.Year >= d.Date.Year);

            IEnumerable<QuestionnaireAnswers> lattestSummaries =
                QuestionnaireAnswersRepository.GetAsQueryable<QuestionnaireAnswers>()
                                              .ToList()
                                              .Where(q => q.UserId == userId && isIncluded(q.Date));
            return lattestSummaries;
        }

        private CoreScoreAnswer GetLocalizedCoreScore(string locale, double questionnaireAnswer)
        {
            string scoreString = null;
            var coreScore = new CoreScoreAnswer();
            if (Math.Abs(questionnaireAnswer - 1) < 0.01)
                scoreString = "WEAKCORE";
            if (Math.Abs(questionnaireAnswer - 2) < 0.01)
                scoreString = "AVERAGECORE";
            if (Math.Abs(questionnaireAnswer - 3) < 0.01)
                scoreString = "STRONGCORE";
            if (scoreString != null)
            {
                coreScore.SetScoreAnswer(scoreString, locale, GetScoreColor(questionnaireAnswer));
            }
            else
            {
                return GetNullCoreScore();
            }

            return coreScore;
        }


        #endregion

        #region Questionnaire View Data

        protected double Epsilon
        {
            get { return 0.01; }
        }

        public QuestionnaireTemplate GetSubmitQuestionnaireTemplate(int i, string defaultQuestionnaireLocale, out GenericError error,
            DateTime getServerDateTime, int userId)
        {
            return GetQuestionnaireTemplate(i, defaultQuestionnaireLocale, out error, getServerDateTime, userId);
        }

        public List<QuestionnaireAverageSummaryData> GetAverageQuestionnaireViewData(int userId, int period,
                                                                                     DateTime now)
        {
            IEnumerable<QuestionnaireAnswers> answers = GetQuestionnaireAnswers(userId, period, now);

            QuestionnaireAnswers[] questionnaireAnswerses = answers as QuestionnaireAnswers[] ?? answers.ToArray();
            if (answers != null && questionnaireAnswerses.Any())
                return CalculateAvergesFromAnswersData(questionnaireAnswerses);
            return null;
        }

        public virtual List<QuestionnaireTemplate> GetQuestionnairHistoricalViewData(int userId, string locale,
                                                                                     DateTime now, int numberToRecords)
        {
            var template = new List<QuestionnaireTemplate>();

            List<QuestionnaireAnswers> lattestQuestioannairesAnswers = LattestQuestioannairesAnswers(userId, now,
                                                                                                     numberToRecords);

            lattestQuestioannairesAnswers.ForEach(q =>
                {
                    GenericError error;
                    QuestionnaireTemplate instanciatedTemplate =
                        InstansiateQuestionnaireForSpecificAnswers(
                            GetQuestionnaireTemplate(q.TemplateId, locale, out error, now, userId), q);
                    instanciatedTemplate.Date = q.Date;
                    FillDailyInfoSpecificDisplayQuestions(userId, instanciatedTemplate, now);
                    template.Add(instanciatedTemplate);
                });


            return template;
        }

        public virtual List<QuestionnaireTemplate> GetQuestionnairHistoricalViewData(int userId, string locale,
                                                                                     int numberToRecords)
        {
            var template = new List<QuestionnaireTemplate>();

            List<QuestionnaireAnswers> lattestQuestioannairesAnswers =
                QuestionnaireAnswersRepository.GetAsQueryable<QuestionnaireAnswers>().ToList()
                // ReSharper disable ImplicitlyCapturedClosure
                                              .Where(q => q.UserId == userId)
                // ReSharper restore ImplicitlyCapturedClosure
                                              .OrderByDescending(q => q.Date).Skip(numberToRecords - 1).Take(1)
                                              .ToList();


            lattestQuestioannairesAnswers.ForEach(q =>
                {
                    GenericError error;
                    QuestionnaireTemplate instanciatedTemplate =
                        InstansiateQuestionnaireForSpecificAnswers(
                            GetQuestionnaireTemplate(q.TemplateId, locale, out error, q.Date, userId), q);
                    instanciatedTemplate.Date = q.Date;
                    FillDailyInfoSpecificDisplayQuestions(userId, instanciatedTemplate, q.Date);
                    template.Add(instanciatedTemplate);
                });


            return template;
        }

        public QuestionnaireTemplate InstansiateQuestionnaireForSpecificAnswers(
            QuestionnaireTemplate questionnaireTemplate, QuestionnaireAnswers questionnaireAnswers)
        {
            //Fill category Questions
            questionnaireTemplate.QuestionCategories.ForEach(qc =>
                {
                    qc.CategoryQuestion.QuestionAnwer =
                        GetInsatnciatedAnswerForQuestion(questionnaireAnswers.CategoryQuestionsAnswers,
                                                         qc.CategoryQuestion,
                                                         () => qc.CategoryQuestion.QuestionId);

                    //Fill Questions Of Each Category 
                    qc.Questions.ForEach(
                        question =>
                        {
                            question.QuestionAnwer =
                                GetInsatnciatedAnswerForQuestion(questionnaireAnswers.QuestionsAnswers, question,
                                                                 () => question.QuestionId);
                        });
                });

            questionnaireTemplate.CoreScoreAnswer = GetLocalizedCoreScore(Constants.DEFAULT_QUESTIONNAIRE_LOCALE,
                                                                          questionnaireAnswers.CoreScore);
            return questionnaireTemplate;
        }

        public CoreScoreAnswer GetLattestCoreScore(long userId)
        {
            QuestionnaireAnswers lattestSummaries =
                QuestionnaireAnswersRepository.GetAsQueryable<QuestionnaireAnswers>()
                                              .LastOrDefault(q => q.UserId == userId);

            if (lattestSummaries != null)
                return GetLocalizedCoreScore(Constants.DEFAULT_QUESTIONNAIRE_LOCALE, lattestSummaries.CoreScore);
            return GetNullCoreScore();
        }

        private List<QuestionnaireAverageSummaryData> CalculateAvergesFromAnswersData(
            IEnumerable<QuestionnaireAnswers> answers, int answersCount = 6)
        {
            var averageSummaryDatas = new List<QuestionnaireAverageSummaryData>();

            for (int i = 1; i < answersCount + 1; i++)
            {
                double average = 0;
                if (answers != null)
                {
                    var questionnaireAnswerses = answers as QuestionnaireAnswers[];
                    if (questionnaireAnswerses != null)
                    {
                        average = questionnaireAnswerses.Aggregate(average,
                                                                   (current, answerse) =>
                                                                   current + answerse.CategoryQuestionsAnswers[i].Answer);
                        average = average / questionnaireAnswerses.Count();
                    }
                }

                averageSummaryDatas.Add(new QuestionnaireAverageSummaryData
                    {
                        Answer = average,
                        Color = GetSummaryCategoryScoreColor(average + 1)
                    });
            }
            return averageSummaryDatas;
        }

        private List<QuestionnaireAnswers> LattestQuestioannairesAnswers(int userId, DateTime now,
                                                                         int numberToRecords)
        {
            List<QuestionnaireAnswers> lattestQuestioannairesAnswers =
                QuestionnaireAnswersRepository.GetAsQueryable<QuestionnaireAnswers>().ToList()
                                              .Where(q => q.UserId == userId &&
                                                          (DateTime.Compare(q.Date, now.AddDays(-1)) > 0) &&
                                                          (DateTime.Compare(q.Date, now) <= 0))
                                              .Take(numberToRecords)
                                              .ToList();
            return lattestQuestioannairesAnswers;
        }

        private void FillDailyInfoSpecificDisplayQuestions(int userId, QuestionnaireTemplate instanciatedTemplate,
                                                           DateTime now)
        {
            try
            {
                ServiceDailyInfo weeksDailyInfos =
                    ObjectFactory.GetInstance<IDailyInfoService>().GetDayInfoViewService(userId, 7, now);


                if (weeksDailyInfos != null)
                {
                    DailyInfoSummaryData lastWeek = SumUpDailyInfos(weeksDailyInfos);
                    instanciatedTemplate.QuestionCategories[instanciatedTemplate.QuestionCategories.Count - 1]
                        .Questions[7].QuestionAnwer = new QuestionAnwer
                            {
                                Description = lastWeek.Rest.ToString(CultureInfo.InvariantCulture)
                            };

                    instanciatedTemplate.QuestionCategories[instanciatedTemplate.QuestionCategories.Count - 1]
                        .Questions[8].QuestionAnwer = new QuestionAnwer
                            {
                                Description = lastWeek.Travel.ToString(CultureInfo.InvariantCulture)
                            };

                    instanciatedTemplate.QuestionCategories[instanciatedTemplate.QuestionCategories.Count - 1]
                        .Questions[9].QuestionAnwer = new QuestionAnwer
                            {
                                Description = lastWeek.Hotel.ToString(CultureInfo.InvariantCulture)
                            };

                    instanciatedTemplate.QuestionCategories[instanciatedTemplate.QuestionCategories.Count - 1]
                        .Questions[10].QuestionAnwer = new QuestionAnwer
                            {
                                Description = "-"
                            };
                }
                else
                {
                    instanciatedTemplate.QuestionCategories[instanciatedTemplate.QuestionCategories.Count - 1]
                        .Questions[7].QuestionAnwer = new QuestionAnwer
                            {
                                Description = "-"
                            };

                    instanciatedTemplate.QuestionCategories[instanciatedTemplate.QuestionCategories.Count - 1]
                        .Questions[8].QuestionAnwer = new QuestionAnwer
                            {
                                Description = "-"
                            };

                    instanciatedTemplate.QuestionCategories[instanciatedTemplate.QuestionCategories.Count - 1]
                        .Questions[9].QuestionAnwer = new QuestionAnwer
                            {
                                Description = "-"
                            };

                    instanciatedTemplate.QuestionCategories[instanciatedTemplate.QuestionCategories.Count - 1]
                        .Questions[10].QuestionAnwer = new QuestionAnwer
                            {
                                Description = "-"
                            };
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        private DailyInfoSummaryData SumUpDailyInfos(ServiceDailyInfo weeksDailyInfos)
        {
            return new DailyInfoSummaryData
                {
                    AverageWeight = weeksDailyInfos.Weight.Sum() / weeksDailyInfos.Weight.Count,
                    Rest = weeksDailyInfos.Rest.Sum(d => d ? 1 : 0),
                    Travel = weeksDailyInfos.Travel.Sum(d => d ? 1 : 0),
                    Sick = weeksDailyInfos.Sick.Sum(d => d ? 1 : 0),
                    Hotel = weeksDailyInfos.Hotel.Sum(d => d ? 1 : 0),
                };
        }

        private static CoreScoreAnswer GetNullCoreScore()
        {
            return new CoreScoreAnswer
                {
                    CoreScore = "no data",
                    Color = Constants.Questioannaire.LIGHT_GRAY
                };
        }

        private QuestionAnwer GetInsatnciatedAnswerForQuestion(
            IEnumerable<Model.Questionnaire.QuestionAnwer> answers, Question qc, Func<int> questionIdSelector)
        {
            Model.Questionnaire.QuestionAnwer givenAnswer =
                answers.FirstOrDefault(a => a.QuestionId == questionIdSelector());
            PossibleAnswer templateAnswer =
                qc.PossivbleAnswers.FirstOrDefault(
                    pa => givenAnswer != null && Math.Abs((pa.Value + 1) - (givenAnswer.Answer + 1)) < 0.01);

            QuestionAnwer instanciatedAnswer;
            if (templateAnswer != null)
            {
                Debug.Assert(givenAnswer != null, "givenAnswer != null");
                instanciatedAnswer = new QuestionAnwer
                    {
                        Description = templateAnswer.Description,
                        Color = GetSingleCategoryScoreColor(givenAnswer.Answer)
                    };
            }
            else
            {
                instanciatedAnswer = new QuestionAnwer { Description = "-" };
            }
            return instanciatedAnswer;
        }

        private string GetScoreColor(double score)
        {
            if (Math.Abs(score - 1) < Epsilon)
                return Constants.Questioannaire.RED_COLOR;
            if (Math.Abs(score - 2) < Epsilon)
                return Constants.Questioannaire.ORANGE_COLOR;
            if (Math.Abs(score - 3) < Epsilon)
                return Constants.Questioannaire.GREEN_COLOR;
            return Constants.Questioannaire.LIGHT_GRAY;
        }

        /// <summary>
        ///     Gets The Color of the Questioannaire Categories answers as Sums
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        private static string GetSummaryCategoryScoreColor(double score)
        {
            if (score < 1.5)
                return Constants.Questioannaire.RED_COLOR;
            if (score < 2.5)
                return Constants.Questioannaire.ORANGE_COLOR;
            if (score <= 3.5)
                return Constants.Questioannaire.GREEN_COLOR;


            return Constants.Questioannaire.LIGHT_GRAY;
        }


        /// <summary>
        ///     Gets The Color of the Questioannaire Categories answers for a single questioannaire
        /// </summary>
        /// <param name="score"></param>
        /// <returns></returns>
        private string GetSingleCategoryScoreColor(double score)
        {
            if (Math.Abs(score - 0) < Epsilon)
                return Constants.Questioannaire.RED_COLOR;
            if (Math.Abs(score - 1) < Epsilon)
                return Constants.Questioannaire.ORANGE_COLOR;
            if (Math.Abs(score - 2) < Epsilon)
                return Constants.Questioannaire.GREEN_COLOR;
            if (Math.Abs(score - 2.5) < Epsilon)
                return Constants.Questioannaire.GREEN_COLOR;
            return Constants.Questioannaire.LIGHT_GRAY;
        }

        #endregion

        #region Submit Questionnaire Template

        public QuestionnaireTemplate SubmitAnsweredTemplate(int userId, QuestionnaireTemplate answeredTemplate,
                                                            DateTime? now)
        {
            if (!now.HasValue) now = Controller.GetServerDateTime();
            QuestionnaireAnswers lattestQuestioannairesAnswers = QuestionnaireAnswersRepository
                .GetAsQueryable<QuestionnaireAnswers>().ToList()
                .FirstOrDefault(
                    q =>
                    q.UserId == userId &&
                    (DateTime.Compare(q.Date, now.Value.Date) == 0));

            if (lattestQuestioannairesAnswers == null)
            {
                QuestionnaireAnswers answers = ExtractAnwsersFromAnsweredTemplate(answeredTemplate);

                try
                {
                    QuestionnaireAnswers processedAnswers = ProceessQuestionAnswers(userId, answers, now);
                    answeredTemplate.CoreScoreAnswer = GetLocalizedCoreScore(Constants.DEFAULT_QUESTIONNAIRE_LOCALE,
                                                                             processedAnswers.CoreScore);
                    UpadatePersistedValues(processedAnswers);
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }


            return answeredTemplate;
        }


        public QuestionnaireAnswers ExtractAnwsersFromAnsweredTemplate(QuestionnaireTemplate answeredTemplate)
        {
            var givenAnswersToPersist = new QuestionnaireAnswers
                {
                    CategoryQuestionsAnswers = new List<Model.Questionnaire.QuestionAnwer>(),
                    QuestionsAnswers = new List<Model.Questionnaire.QuestionAnwer>()
                };


            //Fill category Questions
            answeredTemplate.QuestionCategories.ForEach(qc =>
                {
                    if (qc.CategoryQuestion.QuestionAnwer != null)
                    {
                        givenAnswersToPersist.CategoryQuestionsAnswers.Add(new Model.Questionnaire.QuestionAnwer
                            {
                                QuestionId = qc.CategoryQuestion.QuestionId,
                                Answer = qc.CategoryQuestion.QuestionAnwer.Answer
                            });
                        qc.CategoryQuestion.QuestionAnwer = GetSpesificQuestionAnswer(qc.CategoryQuestion);
                        //Fill Questions Of Each Category 
                        qc.Questions.ForEach(question =>
                            {
                                if (question.QuestionAnwer != null)
                                {
                                    givenAnswersToPersist.QuestionsAnswers.Add(new Model.Questionnaire.QuestionAnwer
                                        {
                                            QuestionId = question.QuestionId,
                                            Answer = question.QuestionAnwer.Answer
                                        });
                                    question.QuestionAnwer = GetSpesificQuestionAnswer(question);
                                }
                                else
                                {
                                    givenAnswersToPersist.QuestionsAnswers.Add(new Model.Questionnaire.QuestionAnwer
                                        {
                                            QuestionId = question.QuestionId,
                                            Answer = -1
                                        });
                                    question.QuestionAnwer = GetSpesificQuestionAnswer(question);
                                }
                            });
                    }
                });

            return givenAnswersToPersist;
        }

        private QuestionnaireAnswers ProceessQuestionAnswers(int userId, QuestionnaireAnswers questionnaireAnwsers,
                                                             DateTime? now = null)
        {
            try
            {
                if (CalculateCoreScore(questionnaireAnwsers))
                {
                    FillSummaryData(questionnaireAnwsers);

                    questionnaireAnwsers.Date = now ?? Controller.GetServerDateTime();
                    questionnaireAnwsers.UserId = userId;

                    return questionnaireAnwsers;
                }
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                ErrorClasses.Controller.GetUnknownError();
                throw;
            }
            throw new Exception();
        }

        private void UpadatePersistedValues(QuestionnaireAnswers questionnaireAnwsers)
        {
            GenericError error;

            QuestionnaireAnswersRepository.InsertEntity(out error, questionnaireAnwsers);

            if (error == null)
            {
                EventsService.UpdateUserDates(questionnaireAnwsers.UserId, Controller.GetServerDateTime(), null);
            }
        }


        private QuestionAnwer GetSpesificQuestionAnswer(Question qc)
        {
            PossibleAnswer res =
                qc.PossivbleAnswers.FirstOrDefault(c => Math.Abs(c.Value - qc.QuestionAnwer.Answer) < 0.01);

            return new QuestionAnwer
                {
                    Answer = qc.QuestionAnwer.Answer,
                    Description = res == null ? "-" : res.Description,
                    Color = GetSingleCategoryScoreColor(qc.QuestionAnwer.Answer)
                };
        }

        //private void UpdateYearlyEnrergyLevel(QuestionnaireAnswers questionnaireAnwsers)
        //{
        //    YearlyQuestionnaireHandlingStrategy.UpdateEntityRecords(questionnaireAnwsers);
        //}

        //private void UpdateMonthlyEnrergyLevel(QuestionnaireAnswers questionnaireAnwsers)
        //{
        //    MontlyQuestionnaireHandlingStrategy.UpdateEntityRecords(questionnaireAnwsers);
        //}

        private void FillSummaryData(QuestionnaireAnswers questionnaireAnwsers)
        {
            questionnaireAnwsers.QuestionnaireSummaryData = new QuestionnaireSummaryData
                {
                    Value = questionnaireAnwsers.CategoryQuestionsAnswers.Select(c => c.Answer).ToList()
                };
        }

        private bool CalculateCoreScore(QuestionnaireAnswers questionnaireAnwsers)
        {
            try
            {
                double coreScore = CaluclateSum(questionnaireAnwsers);

                questionnaireAnwsers.CoreScore = CoreScoreCategoryType(coreScore);
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                return false;
            }
            return true;
        }

        private double CaluclateSum(QuestionnaireAnswers questionnaireAnwsers)
        {
            return questionnaireAnwsers.CategoryQuestionsAnswers.Sum(a => a.Answer + 1);
        }

        private byte CoreScoreCategoryType(double coreScore)
        {
            byte score = 0;

            if ((7 <= coreScore) && (12.83 > coreScore))
                score = 1;
            if ((12.84 <= coreScore) && (18.66 > coreScore))
                score = 2;
            if ((18.67 <= coreScore) && (24.5 >= coreScore))
                score = 3;

            return score;
        }

        #endregion

        #region Request Questionnaire Template

        private static Lazy<QuestionnaireTemplate> _serviceTemplate;
        private readonly QuestionnaireBuilder _questionnaireBuilder;

        public QuestionnaireTemplate GetQuestionnaireTemplate(int questionnaireTemplateId, string locale,
                                                              out GenericError error, DateTime now, int userId)
        {
            error = null;


            try
            {
                if (_serviceTemplate == null)
                    _serviceTemplate = new Lazy<QuestionnaireTemplate>(() =>
                        {
                            var serviceTemplate = new QuestionnaireTemplate();
                            Model.Questionnaire.QuestionnaireTemplate questionTemplate =
                                _questionnaireBuilder.BuildQuestionnaireTemplate();

                            var categoryRes = new QuestionCategories();

                            questionTemplate.QuestionCategories.ForEach(c => FillServiceCategory(locale, categoryRes, c));

                            serviceTemplate.QuestionCategories = categoryRes.QuestionCategoriesList;
                            return serviceTemplate;
                        });
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
            FillDailyInfoSpecificDisplayQuestions(userId, _serviceTemplate.Value, now);

            return FillCategoryGeneralDescriptions(_serviceTemplate.Value);
        }

        private QuestionnaireTemplate FillCategoryGeneralDescriptions(QuestionnaireTemplate serviceTemplate)
        {
            var generalDescriptions = new[]
                {
                    "Energy",
                    "General Health",
                    "Biomechanics",
                    "Training",
                    "Nutrition",
                    "Mental Energy",
                    "Recovery"
                };

            for (int i = 0; i < serviceTemplate.QuestionCategories.Count; i++)
            {
                serviceTemplate.QuestionCategories[i].CategoryQuestion.GeneralDescription = generalDescriptions[i];
            }

            return serviceTemplate;
        }

        private void FillServiceCategory(string locale, QuestionCategories categoryRes,
                                         QuestionCategory c)
        {
            var questionsRes = new Questions();

            c.Questions.ForEach(q => FillServiceQuestion(locale, questionsRes, q));

            ServiceModel.Questionnaire.QuestionCategory serviceCategory = categoryRes.AddItem(locale,
                                                                                              c.CategoryQuestion
                                                                                               .Description,
                                                                                              c.CategoryQuestion.Help);
            serviceCategory.CategoryQuestion = FillServiceCategoryQuestion(locale, c);
            serviceCategory.Questions = questionsRes.QuestionsList;
        }

        private Question FillServiceCategoryQuestion(string locale, QuestionCategory c)
        {
            var questionsRes = new Questions();
            var posibleAnswersRes = new PossibleAnswers();

            Question question = questionsRes.GetLocalizedQuestion(locale, c.CategoryQuestion.Description,
                                                                  c.CategoryQuestion.Help, c.CategoryQuestion.QuestionId);
            c.CategoryQuestion.PossivbleAnswers.ForEach(pa => FillPosibleAnswer(locale, posibleAnswersRes, pa));

            question.PossivbleAnswers = posibleAnswersRes.PossivbleAnswersList;

            return question;
        }

        private void FillServiceQuestion(string locale, Questions questionsRes, Model.Questionnaire.Question q)
        {
            var posibleAnswersRes = new PossibleAnswers();

            q.PossivbleAnswers.ForEach(pa => FillPosibleAnswer(locale, posibleAnswersRes, pa));

            Question serviceQuestion = questionsRes.AddItem(locale, q.Description, q.Help, q.QuestionId);

            serviceQuestion.PossivbleAnswers = posibleAnswersRes.PossivbleAnswersList;
        }

        private void FillPosibleAnswer(string locale, PossibleAnswers posibleAnswersRes, PossivbleAnswer pa)
        {
            posibleAnswersRes.AddItem(locale, pa.Description, pa.Value, pa.Id);
        }

        #endregion

        ///// <summary>
        /////     Handles Monthly Questionnaire Summary Data
        ///// </summary>
        //public MontlyQuestionnaireHandlingTemplate MontlyQuestionnaireHandlingStrategy { get; set; }

        ///// <summary>
        /////     Handles Yearly Questionnaire Summary Data
        ///// </summary>
        //public YearlyQuestionnaireHandlingTemplate YearlyQuestionnaireHandlingStrategy { get; set; }

        //public IRepositoryBase<QuestionnaireAnswers> MonthlyQuestionnaireAnswersRepository { get; set; }

        //public INoSqlFinalValuesRepositoryBase<QuestionnaireAnswers> FinalMonthlyQuestionnaireAnswersRepository { get; set; }

        //public IRepositoryBase<QuestionnaireAnswers> YearlyQuestionnaireAnswersRepository { get; set; }

        //public INoSqlFinalValuesRepositoryBase<QuestionnaireAnswers> FinalYearlyQuestionnaireAnswersRepository { get; set; }

        //move to resouress 
    }
}