using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using DataAccess.Interfaces;
using ErrorClasses;
using Model.DailyInfo;

namespace Business.DailyInfo.Helpers
{
    public class YearlyInfoAgreggationHelperTemplate :
        DataAgreggationHelperTemplate<DailyInfoSummaryData, Model.DailyInfo.DailyInfo, DailyInfoSummaryData>
    {
        public IRepositoryBase<Model.DailyInfo.DailyInfo> DailyInfoSummaryDataRepository { get; set; }

        public IRepositoryBase<DailyInfoSummaryData> YearlyDailyInfoSummaryDataRepository { get; set; }

        public INoSqlFinalValuesRepositoryBase<DailyInfoSummaryData> FinalYearlyDailyInfoSummaryDataRepository { get; set; }

        public override DailyInfoSummaryData CreateEmptyServiceModel()
        {
            return new DailyInfoSummaryData
                {
                    AverageWeight = 0,
                    Hotel = 0,
                    Rest = 0,
                    Sick = 0,
                    Travel = 0,
                };
        }

        public override bool IsTheFirstEntryForUser(long userId)
        {
            GenericError error;
            List<DailyInfoSummaryData> entries = YearlyDailyInfoSummaryDataRepository.GetEntities(out error, d => d.UserId == userId);
            return entries == null || entries.Count == 0;
        }

        public override DailyInfoSummaryData CreateNewModelItem(Model.DailyInfo.DailyInfo dailyInfo)
        {
            return new DailyInfoSummaryData
                {
                    UserId = dailyInfo.UserId,
                    Count = 1,
                    Date = dailyInfo.Date,
                    WeightSumTotal = dailyInfo.Weight,
                    AverageWeight = dailyInfo.Weight,
                    Hotel = dailyInfo.Hotel ? 1 : 0,
                    Rest = dailyInfo.Rest ? 1 : 0,
                    Sick = dailyInfo.Sick ? 1 : 0,
                    Travel = dailyInfo.Travel ? 1 : 0,
                };
        }

        public override DailyInfoSummaryData GetLattestRecord(long userId)
        {
            GenericError error;
            List<DailyInfoSummaryData> records = YearlyDailyInfoSummaryDataRepository.GetEntities(out error,  d => d.UserId== userId);
            if (records.Any())
            {
                records.Sort((d1, d2) => DateTime.Compare(d1.Date, d2.Date));
                return records.Last();
            }

            return new DailyInfoSummaryData();
        }

        public override void InitializeRunningTotals(DailyInfoSummaryData yearlyDailyInfo, Model.DailyInfo.DailyInfo dailyInfo)
        {
            yearlyDailyInfo.WeightSumTotal = dailyInfo.Weight;
        }

        public override bool IsTheFirstRecordOfPeriod(DailyInfoSummaryData latestYearlyRecord, Model.DailyInfo.DailyInfo energyLevel)
        {
            return latestYearlyRecord.Date.Year < energyLevel.Date.Year;
        }

        public override void ComputeFromLastRecord(Model.DailyInfo.DailyInfo dailyInfo, DailyInfoSummaryData yearlyEnergyLevel,
                                                   DailyInfoSummaryData latestYearlyRecord)
        {
            yearlyEnergyLevel.WeightSumTotal = latestYearlyRecord.WeightSumTotal + dailyInfo.Weight;
            yearlyEnergyLevel.Count = latestYearlyRecord.Count + 1;
            yearlyEnergyLevel.AverageWeight = yearlyEnergyLevel.Count == 0 ? 0 : Math.Round(yearlyEnergyLevel.WeightSumTotal / yearlyEnergyLevel.Count, Common.Constants.DAILY_INFO_WEIGHT_DIGITS_PERSIST);

            yearlyEnergyLevel.Hotel = latestYearlyRecord.Hotel + (dailyInfo.Hotel ? 1 : 0);
            yearlyEnergyLevel.Rest = latestYearlyRecord.Rest + (dailyInfo.Rest ? 1 : 0);
            yearlyEnergyLevel.Sick = latestYearlyRecord.Sick + (dailyInfo.Sick ? 1 : 0);
            yearlyEnergyLevel.Travel = latestYearlyRecord.Travel + (dailyInfo.Travel ? 1 : 0);
        }

        public override bool IsTheLastRecordOfthePeriod(DailyInfoSummaryData latestYearlyRecord, Model.DailyInfo.DailyInfo energyLevel)
        {
            return latestYearlyRecord.Date.Year < energyLevel.Date.Year;
        }

        public override void InsertEntityToFinalRecords(out GenericError error, DailyInfoSummaryData latestYearlyRecord)
        {
            FinalYearlyDailyInfoSummaryDataRepository.InsertEntity(out error, latestYearlyRecord);
        }

        public override bool AddNewRecordToDataBase(DailyInfoSummaryData energyLevel)
        {
            try
            {
                GenericError error;
                YearlyDailyInfoSummaryDataRepository.InsertEntity(out error, energyLevel);
                return error == null;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                Controller.GetUnknownError();
                throw;
            }
        }

        public override IList<DailyInfoSummaryData> GetInfoView(int userid, DateTime now, int numberOfYearsToDisplay)
        {
            var helper = new DailyInfoYearlyDataHelper
                {
                    YearlyDailyInfoSummaryDataRepository = YearlyDailyInfoSummaryDataRepository,
                    FinalYearlyDailyInfoSummaryDataRepository = FinalYearlyDailyInfoSummaryDataRepository
                };

            return helper.GetViewDataForTimePeriod(userid, now, numberOfYearsToDisplay);
        }
    }
}