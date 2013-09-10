using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using DataAccess.Interfaces;
using ErrorClasses;
using Model.DailyInfo;

namespace Business.DailyInfo.Helpers
{
    public class MontlyInfoAgreggationHelperTemplate :
        DataAgreggationHelperTemplate<DailyInfoSummaryData, Model.DailyInfo.DailyInfo, DailyInfoSummaryData>
    {
        public IRepositoryBase<Model.DailyInfo.DailyInfo> DailyInfoSummaryDataRepository { get; set; }

        public IRepositoryBase<DailyInfoSummaryData> MonthlyDailyInfoSummaryDataRepository { get; set; }

        public INoSqlFinalValuesRepositoryBase<DailyInfoSummaryData> FinalMonthlyDailyInfoSummaryDataRepository { get; set; }

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
            List<DailyInfoSummaryData> entries = MonthlyDailyInfoSummaryDataRepository.GetEntities(out error ,d => d.UserId== userId);
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
            List<DailyInfoSummaryData> records = MonthlyDailyInfoSummaryDataRepository.GetEntities(out error, d => d.UserId== userId);
            if (records.Any())
                return records.Last();
            return new DailyInfoSummaryData();
        }

        public override void InitializeRunningTotals(DailyInfoSummaryData monthlyDailyInfo, Model.DailyInfo.DailyInfo dailyInfo)
        {
            monthlyDailyInfo.WeightSumTotal = dailyInfo.Weight;
        }

        public override bool IsTheFirstRecordOfPeriod(DailyInfoSummaryData latestMonthlyRecord, Model.DailyInfo.DailyInfo energyLevel)
        {
            return latestMonthlyRecord.Date.Month < energyLevel.Date.Month;
        }

        public override void ComputeFromLastRecord(Model.DailyInfo.DailyInfo dailyInfo, DailyInfoSummaryData monthlyEnergyLevel,
                                                   DailyInfoSummaryData latestMonthlyRecord)
        {
            monthlyEnergyLevel.WeightSumTotal = latestMonthlyRecord.WeightSumTotal + dailyInfo.Weight;
            monthlyEnergyLevel.Count = latestMonthlyRecord.Count + 1;
            monthlyEnergyLevel.AverageWeight = monthlyEnergyLevel.Count == 0 ? 0 : Math.Round(monthlyEnergyLevel.WeightSumTotal / monthlyEnergyLevel.Count, Common.Constants.DAILY_INFO_WEIGHT_DIGITS_PERSIST);

            monthlyEnergyLevel.Hotel = latestMonthlyRecord.Hotel + (dailyInfo.Hotel ? 1 : 0);
            monthlyEnergyLevel.Rest = latestMonthlyRecord.Rest + (dailyInfo.Rest ? 1 : 0);
            monthlyEnergyLevel.Sick = latestMonthlyRecord.Sick + (dailyInfo.Sick ? 1 : 0);
            monthlyEnergyLevel.Travel = latestMonthlyRecord.Travel + (dailyInfo.Travel ? 1 : 0);
        }

        public override bool IsTheLastRecordOfthePeriod(DailyInfoSummaryData latestMonthlyRecord, Model.DailyInfo.DailyInfo energyLevel)
        {
            return latestMonthlyRecord.Date.Month < energyLevel.Date.Month;
        }

        public override void InsertEntityToFinalRecords(out GenericError error, DailyInfoSummaryData latestMonthlyRecord)
        {
            FinalMonthlyDailyInfoSummaryDataRepository.InsertEntity(out error, latestMonthlyRecord);
        }

        public override bool AddNewRecordToDataBase(DailyInfoSummaryData energyLevel)
        {
            try
            {
                GenericError error;
                MonthlyDailyInfoSummaryDataRepository.InsertEntity(out error, energyLevel);
                if (error != null)
                    return false;
                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                Controller.GetUnknownError();
                throw;
            }
        }

        public override IList<DailyInfoSummaryData> GetInfoView(int userId, DateTime now, int numberOfMonthsToDisplay)
        {
            var helper = new DailyInfoMonthlyDataHelper
                {
                    MonthlyDailyInfoSummaryDataRepository = MonthlyDailyInfoSummaryDataRepository,
                    FinalMonthlyDailyInfoSummaryDataRepository = FinalMonthlyDailyInfoSummaryDataRepository
                };

            return helper.GetViewDataForTimePeriod(userId, now, numberOfMonthsToDisplay).ToList();
        }
    }
}