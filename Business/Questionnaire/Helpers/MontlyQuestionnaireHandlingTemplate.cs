using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using DataAccess.Interfaces;
using ErrorClasses;
using Model.Questionnaire;

namespace Business.Questionnaire.Helpers
{
    public class MontlyQuestionnaireHandlingTemplate :
        DataAgreggationHelperTemplate<QuestionnaireAnswers, QuestionnaireAnswers, QuestionnaireSummaryData>
    {
        public IRepositoryBase<QuestionnaireAnswers> DailyQuestionnaireAnswersRepository { get; set; }
        public IRepositoryBase<QuestionnaireAnswers> MonthlyQuestionnaireAnswersRepository { get; set; }
        public INoSqlFinalValuesRepositoryBase<QuestionnaireAnswers> FinalMonthlyQuestionnaireAnswersRepository { get; set; }

        public override QuestionnaireSummaryData CreateEmptyServiceModel()
        {
            return new QuestionnaireSummaryData { Value = new List<double>() };
        }

        public override bool IsTheFirstEntryForUser(long userId)
        {
            GenericError error;
            List<QuestionnaireAnswers> entries = MonthlyQuestionnaireAnswersRepository.GetEntities(out error, d => d.UserId == userId);
            return entries == null || entries.Count == 0;
        }

        public override QuestionnaireAnswers CreateNewModelItem(QuestionnaireAnswers energyLevel)
        {
            return new QuestionnaireAnswers
                {
                    UserId = energyLevel.UserId,
                    Date = energyLevel.Date,
                    CoreScore = energyLevel.CoreScore,
                };
        }

        public override QuestionnaireAnswers GetLattestRecord(long userId)
        {
            GenericError error;
            var records = MonthlyQuestionnaireAnswersRepository.GetEntities(out error, d => d.UserId == userId);

            if (records.Any())
            {
                records.Sort((d1, d2) => DateTime.Compare(d1.Date, d2.Date));  
                return records.Last();
            }
            return new QuestionnaireAnswers();
        }

        public override void InitializeRunningTotals(QuestionnaireAnswers yearlyDailyInfo,
                                                     QuestionnaireAnswers energyLevel)
        {
            yearlyDailyInfo.QuestionnaireSummaryData = energyLevel.QuestionnaireSummaryData;
        }

        public override bool IsTheFirstRecordOfPeriod(QuestionnaireAnswers latestMonthlyRecord,
                                                      QuestionnaireAnswers energyLevel)
        {
            return latestMonthlyRecord.Date.Month < energyLevel.Date.Month;
        }

        public override void ComputeFromLastRecord(QuestionnaireAnswers answers, QuestionnaireAnswers yearlyEnergyLevel,
                                                   QuestionnaireAnswers latestMonthlyRecord)
        {
            yearlyEnergyLevel.QuestionnaireSummaryData =
                CalulateSummaryDataAverageValues(new[] { latestMonthlyRecord.QuestionnaireSummaryData, answers.QuestionnaireSummaryData });
        }

        private static QuestionnaireSummaryData CalulateSummaryDataAverageValues(
            IEnumerable<QuestionnaireSummaryData> lastQuestioannaires)
        {
            var questionnaireSummaryData = new QuestionnaireSummaryData { };
            for (int j = 0; j < 6; j++)
            {
                var average = lastQuestioannaires.Sum(q => { return q.Value[j]; }) / lastQuestioannaires.Count();
                questionnaireSummaryData.Value[j] = (int)Math.Round((double)average, MidpointRounding.AwayFromZero);
            }
            return questionnaireSummaryData;
        }


        public override bool IsTheLastRecordOfthePeriod(QuestionnaireAnswers latestMonthlyRecord,
                                                        QuestionnaireAnswers energyLevel)
        {
            return latestMonthlyRecord.Date.Month < energyLevel.Date.Month;
        }

        public override void InsertEntityToFinalRecords(out GenericError error, QuestionnaireAnswers latestMonthlyRecord)
        {
            error = null;
            FinalMonthlyQuestionnaireAnswersRepository.InsertEntity(out error, latestMonthlyRecord);
        }

        public override bool AddNewRecordToDataBase(QuestionnaireAnswers energyLevel)
        {
            GenericError error = null;
            try
            {
                MonthlyQuestionnaireAnswersRepository.InsertEntity(out error, energyLevel);
                if (error != null) return false;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public override IList<QuestionnaireSummaryData> GetInfoView(int userid, DateTime now,
                                                                    int numberOfMonthsToDisplay)
        {
            var dataHelper = new MonthlyDataHelper
                {
                    Repository = MonthlyQuestionnaireAnswersRepository,
                    FinalRepository = FinalMonthlyQuestionnaireAnswersRepository
                };

            IList<QuestionnaireSummaryData> data = dataHelper.GetViewDataForTimePeriod(userid, now,
                                                                                       numberOfMonthsToDisplay);

            return data;
 
        }
    }
}