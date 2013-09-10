using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using Business.Questionnaire.Helpers;
using Common.Classes;
using DataAccess.Interfaces;
using ErrorClasses;
using Model.Questionnaire;
using NoSqlDataAccess.Entities;

namespace Business.Questionnaire
{
    public class YearlyQuestionnaireHandlingTemplate :
        DataAgreggationHelperTemplate<QuestionnaireAnswers, QuestionnaireAnswers, QuestionnaireSummaryData>
    {
        public IRepositoryBase<QuestionnaireAnswers> DailyQuestionnaireAnswersRepository { get; set; }
        public IRepositoryBase<QuestionnaireAnswers> YearlyQuestionnaireAnswersRepository { get; set; }
        public INoSqlFinalValuesRepositoryBase<QuestionnaireAnswers> FinalYearlyQuestionnaireAnswersRepository { get; set; }


        public override QuestionnaireSummaryData CreateEmptyServiceModel()
        {
            return new QuestionnaireSummaryData { Value = new List<double>() };
        }

        public override bool IsTheFirstEntryForUser(long userId)
        {
            GenericError error;
            List<QuestionnaireAnswers> entries = YearlyQuestionnaireAnswersRepository.GetEntities(out error, d => d.UserId == userId);
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
            var records = YearlyQuestionnaireAnswersRepository.GetEntities(out error, d => d.UserId == userId);


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

        public override void ComputeFromLastRecord(QuestionnaireAnswers energyLevel, QuestionnaireAnswers yearlyEnergyLevel,
                                                   QuestionnaireAnswers latestMonthlyRecord)
        {
            yearlyEnergyLevel.QuestionnaireSummaryData =
                CalulateSummaryDataAverageValues(new[] { latestMonthlyRecord.QuestionnaireSummaryData, energyLevel.QuestionnaireSummaryData });
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
            FinalYearlyQuestionnaireAnswersRepository.InsertEntity(out error, latestMonthlyRecord);
        }

        public override bool AddNewRecordToDataBase(QuestionnaireAnswers energyLevel)
        {
            GenericError error = null;
            try
            {
                YearlyQuestionnaireAnswersRepository.InsertEntity(out error, energyLevel);
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
            var dataHelper = new YearlyDataHelper
                {
                    Repository = YearlyQuestionnaireAnswersRepository,
                    FinalRepository = FinalYearlyQuestionnaireAnswersRepository
                };

            IList<QuestionnaireSummaryData> data = dataHelper.GetViewDataForTimePeriod(userid, now,
                                                                                       numberOfMonthsToDisplay);

            return data;

            //var lastMonthlyEnergyInfo = YearlyQuestionnaireAnswersRepository.GetAsQueryable<QuestionnaireAnswers>().Where(q => q.UserId == userid).Last();
            //var lastMonthsEnergyLevelInfo = FinalYearlyQuestionnaireAnswersRepository
            //    .GetAsQueryable<QuestionnaireAnswers>().ToList()

            //    .Where(q => q.UserId == userid && DateTime.Compare(now.AddYears(-numberOfMonthsToDisplay), q.Date) <= 0);
            //// .Take(numberOfMonthsToDisplay - 1);
            //var energyLevelInfoViewData = new List<QuestionnaireSummaryData>();

            //energyLevelInfoViewData.Add(new QuestionnaireSummaryData { Value = lastMonthlyEnergyInfo.QuestionnaireSummaryData.Value });

            //lastMonthsEnergyLevelInfo.ToList().ForEach(d =>
            //{
            //    energyLevelInfoViewData.Add(new QuestionnaireSummaryData()
            //    {
            //        Value = d.QuestionnaireSummaryData.Value
            //    });

            //});

            //FIllEmptyRecords(energyLevelInfoViewData, numberOfMonthsToDisplay);

            //return energyLevelInfoViewData;
        }
    }
}