using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using Business.Helpers;
using Business.Interfaces;
using Business.Questionnaire.QuestioannaireTemplates;
using Common;
using DataAccess.Interfaces;
using ErrorClasses;
using Model.Questionnaire;
using ServiceModel.Questionnaire;
using Controller = ServerDateTimeProvider.Controller;
using Question = ServiceModel.Questionnaire.Question;
using QuestionAnwer = ServiceModel.Questionnaire.QuestionAnwer;
using QuestionnaireTemplate = ServiceModel.Questionnaire.QuestionnaireTemplate;

namespace Business.Questionnaire
{
    internal class QuestionnaireServiceNew : IQuestionnaireService
    {
      
        public QuestionnaireServiceNew()
        {
            _questionnaireRegistry = new QuestioannaireTemplateRegistry();

          //   var calculatioAlgorithms = new QuestionnaireCalculationAlgorithm();
          //  _questionnaireCalculationAlgorithm = calculatioAlgorithms;
          //  _questionnaireCoreScoreCalculationAlgorithm = calculatioAlgorithms;

        }


        #region Repositories

        public IRepositoryBase<QuestionnaireAnswers> QuestionnaireAnswersRepository { get; set; }
        public IEventsService EventsService { get; set; }


        public CoreScoreAnswer GetAverageCoreScoreForPeriod(int userId, int period, DateTime currentTime, string locale)
        {
            IEnumerable<QuestionnaireAnswers> lattestSummaries = GetQuestionnaireAnswers(userId, period, currentTime);

            IList<QuestionnaireAnswers> questionnaireAnswerses = lattestSummaries as IList<QuestionnaireAnswers> ??
                                                                 lattestSummaries.ToList();
            if (questionnaireAnswerses.Any())
            {
                var averageCoreScores =(int)Math.Round(((float)questionnaireAnswerses.Sum(c => c.CoreScore) / questionnaireAnswerses.Count()));
                return QuestionnaireCalculationAlgorithmBase.GetLocalizedCoreScore(locale, (byte)averageCoreScores);
            }
            return QuestionnaireCalculationAlgorithmBase.GetNullCoreScore();
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


        #endregion

        #region Questionnaire View Data


        public List<QuestionnaireAverageSummaryData> GetAverageQuestionnaireViewData(int userId, int period,
                                                                                     DateTime now)
        {
            IEnumerable<QuestionnaireAnswers> answers = GetQuestionnaireAnswers(userId, period, now);

            if (answers != null && answers.Any())
                return  CalculateAvergesFromAnswersData(answers);
            return null;
        }



        public virtual List<QuestionnaireAverageSummaryData> CalculateAvergesFromAnswersData(IEnumerable<QuestionnaireAnswers> answers, int answersCount = 6)
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
                        average = questionnaireAnswerses.Aggregate(average, (current, answer) =>
                                                                   current + answer.CategoryQuestionsAnswers[i].Answer);
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

        protected virtual string GetSummaryCategoryScoreColor(double score)
        {
            if (score < 1.5)
                return Constants.Questioannaire.RED_COLOR;
            if (score < 2.5)
                return Constants.Questioannaire.ORANGE_COLOR;
            if (score <= 3.5)
                return Constants.Questioannaire.GREEN_COLOR;

            return Constants.Questioannaire.LIGHT_GRAY;
        }
        public virtual List<QuestionnaireTemplate> GetQuestionnairHistoricalViewData(int userId, string locale,
                                                                                     DateTime now, int numberToRecords)
        {
            var template = new List<QuestionnaireTemplate>();

            //List<QuestionnaireAnswers> lattestQuestioannairesAnswers = LattestQuestioannairesAnswers(userId, now,
            //                                                                                         numberToRecords);

           
            //lattestQuestioannairesAnswers.ForEach(q =>
            //    {
            //        GenericError error;
            //        QuestionnaireTemplate instanciatedTemplate =
            //            InstansiateQuestionnaireForSpecificAnswers(GetQuestionnaireTemplate(q.TemplateId, locale, out error, now, userId), q);
            //        instanciatedTemplate.Date = q.Date;
            //        template.Add(instanciatedTemplate);
            //    });


            return template;
        }

        public virtual List<QuestionnaireTemplate> GetQuestionnairHistoricalViewData(int userId, string locale,
                                                                                     int numberToRecords)
        {
            var template = new List<QuestionnaireTemplate>();

            List<QuestionnaireAnswers> lattestQuestioannairesAnswers =
                QuestionnaireAnswersRepository.GetAsQueryable<QuestionnaireAnswers>().ToList()
                                               .Where(q => q.UserId == userId)
                                               .OrderByDescending(q => q.Date).Skip(numberToRecords - 1).Take(1)
                                              .ToList();



            lattestQuestioannairesAnswers.ForEach(specificAnswer =>
                {
                    GenericError error;
                    var templateBuilder = _questionnaireRegistry.GetAveliableQuestioannaireTemplateBuilder(specificAnswer.TemplateId);
                    var templateProcessor = _questionnaireRegistry.GetAveliableQuestioannaireProccesor(specificAnswer.TemplateId);

                    QuestionnaireTemplate instanciatedTemplate = templateProcessor.
                        InstansiateQuestionnaireForSpecificAnswers(GetQuestionnaireTemplate(templateBuilder.TemplateId, templateBuilder.TemplateLocaleName, out error, specificAnswer.Date, userId), specificAnswer);
                  
                    instanciatedTemplate.Date = specificAnswer.Date;


                    templateBuilder.QuestionnaireCalculationAlgorithm.CalculateSingleQuestionnaireCategoryColors(instanciatedTemplate);
                    templateBuilder.QuestionnaireCoreScoreCalculationAlgorithm.CalculateSingleQuestionnaireCoreScoreColor(instanciatedTemplate);

                    template.Add(instanciatedTemplate);
                });

            return template;
        }

     
        public CoreScoreAnswer GetLattestCoreScore(long userId)
        {
            QuestionnaireAnswers lattestSummaries = QuestionnaireAnswersRepository.GetAsQueryable<QuestionnaireAnswers>().LastOrDefault(q => q.UserId == userId);

            //var calculationAlgorithm = new QuestionnaireCalculationAlgorithmWeek3();
            //if (lattestSummaries != null)
            //    return calculationAlgorithm.GetLocalizedCoreScore(Constants.DEFAULT_QUESTIONNAIRE_LOCALE, lattestSummaries.CoreScore);
            return QuestionnaireCalculationAlgorithmBase.GetNullCoreScore();
        }


        //private List<QuestionnaireAnswers> LattestQuestioannairesAnswers(int userId, DateTime now,
        //                                                                 int numberToRecords)
        //{
        //    List<QuestionnaireAnswers> lattestQuestioannairesAnswers =
        //        QuestionnaireAnswersRepository.GetAsQueryable<QuestionnaireAnswers>().ToList()
        //                                      .Where(q => q.UserId == userId &&
        //                                                  (DateTime.Compare(q.Date, now.AddDays(-1)) > 0) &&
        //                                                  (DateTime.Compare(q.Date, now) <= 0))
        //                                      .Take(numberToRecords)
        //                                      .ToList();
        //    return lattestQuestioannairesAnswers;
        //}


        #endregion

        #region Submit Questionnaire Template

        public QuestionnaireTemplate SubmitAnsweredTemplate(int userId, QuestionnaireTemplate answeredTemplate, DateTime? now)
        {
            var hasQuestioannairesAnswered = CheckIfQuestionnaireAlreadyAnswered(userId, now);

            if (!hasQuestioannairesAnswered)
            {
                var templateAnswersExtractor = _questionnaireRegistry.GetAveliableQuestioannaireProccesor(answeredTemplate.TemplateId);
                var templateBuilder = _questionnaireRegistry.GetAveliableQuestioannaireTemplateBuilder(answeredTemplate.TemplateId);

                QuestionnaireAnswers answers = templateAnswersExtractor.ExtractAnwsersFromAnsweredTemplate(answeredTemplate);

                try
                { 
                    //extract answers to persist and save them 
                    answers.CoreScore = templateBuilder.QuestionnaireCoreScoreCalculationAlgorithm.OveralCoreScoreType(answeredTemplate);
                    answers.QuestionnaireSummaryData = templateBuilder.QuestionnaireCalculationAlgorithm.CalculateSingleQuestionnaireCategoryScores(answeredTemplate);
                    UpadatePersistedValues(userId, answers, now);
                    
                    //Initialize null CategoryQuestions if any 
                    answeredTemplate.QuestionCategories.ForEach(qc=>
                    {
                        if (qc.CategoryQuestion.QuestionAnwer == null)qc.CategoryQuestion.QuestionAnwer = new QuestionAnwer();
                    });
                    //fill with the results 
                    templateBuilder.QuestionnaireCalculationAlgorithm.CalculateSingleQuestionnaireCategoryColors(answeredTemplate);
                    templateBuilder.QuestionnaireCoreScoreCalculationAlgorithm.CalculateSingleQuestionnaireCoreScoreColor(answeredTemplate);

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                }
            }


            return answeredTemplate;
        }

        protected virtual bool CheckIfQuestionnaireAlreadyAnswered(int userId, DateTime? now)
        {
            if (!now.HasValue) now = Controller.GetServerDateTime();

            QuestionnaireAnswers lattestQuestioannairesAnswers = QuestionnaireAnswersRepository
                .GetAsQueryable<QuestionnaireAnswers>().ToList()
                .FirstOrDefault(q => q.UserId == userId && (DateTime.Compare(q.Date, now.Value.Date) == 0));
            return lattestQuestioannairesAnswers != null;
        }

        private void UpadatePersistedValues(int userId, QuestionnaireAnswers questionnaireAnwsers, DateTime? now = null)
        {
            GenericError error;

            questionnaireAnwsers.Date = now ?? Controller.GetServerDateTime();
            questionnaireAnwsers.UserId = userId;

            QuestionnaireAnswersRepository.InsertEntity(out error, questionnaireAnwsers);

            if (error == null)
            {
                EventsService.UpdateUserDates(questionnaireAnwsers.UserId, Controller.GetServerDateTime(), null);
            }
        }

        #endregion

        #region Request Questionnaire Template

        private readonly QuestioannaireTemplateRegistry _questionnaireRegistry;

        public QuestionnaireTemplate GetQuestionnaireTemplate(int questionnaireTemplateId, string locale,
                                                              out GenericError error, DateTime now, int userId)
        {
            error = null;

            try
            {
                var templateBuilder = _questionnaireRegistry.GetAveliableQuestioannaireTemplateBuilder(questionnaireTemplateId);

                Model.Questionnaire.QuestionnaireTemplate questionTemplate = templateBuilder.BuildQuestionnaireTemplate();

                return templateBuilder.LocalizeTemplate(questionTemplate);

            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }

        }

        public QuestionnaireTemplate GetSubmitQuestionnaireTemplate(int questionnaireTemplateId, string defaultQuestionnaireLocale, out GenericError error,
            DateTime getServerDateTime, int userId)
        {

            error = null;

            try
            {
                var templateBuilder = _questionnaireRegistry.GetAveliableQuestioannaireTemplateBuilder(questionnaireTemplateId);

                Model.Questionnaire.QuestionnaireTemplate questionnaireTemplateTemplate = templateBuilder.BuildSubmitQuestionnaireTemplate();

                return templateBuilder.LocalizeTemplate(questionnaireTemplateTemplate);

            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }

        }



        #endregion

    }
}