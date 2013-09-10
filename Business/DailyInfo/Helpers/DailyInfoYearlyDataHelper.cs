using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using DataAccess.Interfaces;
using Model.DailyInfo;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.DailyInfo.Helpers
{
    public class DailyInfoYearlyDataHelper : YearlyDataHelper<DailyInfoSummaryData, DailyInfoSummaryData>
    {
        public IRepositoryBase<DailyInfoSummaryData> YearlyDailyInfoSummaryDataRepository { get; set; }

        public INoSqlFinalValuesRepositoryBase<DailyInfoSummaryData> FinalYearlyDailyInfoSummaryDataRepository { get; set; }

        public override IList<DailyInfoSummaryData> GetViewDataForTimePeriod(int userId, DateTime now,
                                                                             int numberOfMonthsToDisplay )
        {
            DailyInfoSummaryData currentMonthEnergyInfo =
                YearlyDailyInfoSummaryDataRepository.GetAsQueryable<DailyInfoSummaryData>().Last();
            IEnumerable<DailyInfoSummaryData> lastMonthsEnergyLevelInfo = FinalYearlyDailyInfoSummaryDataRepository
                .GetAsQueryable<DailyInfoSummaryData>().ToList()
                .Where(d => d.UserId == userId && DoGetInTimePeriodFilterComparision(now, numberOfMonthsToDisplay, d));

            //aggregate lists
            var allMontlyEnergyInfo = new List<DailyInfoSummaryData> {currentMonthEnergyInfo};
            allMontlyEnergyInfo.AddRange(lastMonthsEnergyLevelInfo);
            InitializePeriodStrategy(userId);
            IList<TimeSpan> monthlyTimeSpans = PeriodSelectionStrategy.GetTimeSpans(now, numberOfMonthsToDisplay);
            List<DailyInfoSummaryData> energyLevelInfoViewData = FillRecordsOnEmptyTimeSpans(allMontlyEnergyInfo,
                                                                                             monthlyTimeSpans);

            return DailyInfoService.TranctuateDigits(energyLevelInfoViewData);
        }

        public override DailyInfoSummaryData InitializeNewServiceType(DailyInfoSummaryData dailyInfoInTimeSpan, TimeSpan span)
        {
            return dailyInfoInTimeSpan;
        }

        public override DailyInfoSummaryData CreateEmptyServiceType(TimeSpan span)
        {
            return new DailyInfoSummaryData();
        }
    }

    public class DailyInfoYearlyDataHelperDynamic : YearlyDynamicDataHelper<DailyInfoSummaryData, Model.DailyInfo.DailyInfo>
    {

        public override IList<DailyInfoSummaryData> GetViewDataForTimePeriod(int userId, DateTime now, int numberOfMonthsToDisplay)
        {
            return DailyInfoService.TranctuateDigits(base.GetViewDataForTimePeriod(userId, now, numberOfMonthsToDisplay));

        }
        protected override DailyInfoSummaryData DoGetServiceTypeInTimeSpan(List<Model.DailyInfo.DailyInfo> dailyLevelsInWeekSpan,
                                                                           TimeSpan dailyTimeSpan)
        {
            TimeSpan span = dailyTimeSpan;

            IEnumerable<Model.DailyInfo.DailyInfo> dailyInfoInTimeSpan =
                dailyLevelsInWeekSpan.Where(energyevel => span.InTimeIncluded(energyevel.Date));
            DailyInfoSummaryData serviceType = InitializeNewServiceType(dailyInfoInTimeSpan.ToList());
            return serviceType;
        }

        public static DailyInfoSummaryData InitializeNewServiceType(IList<Model.DailyInfo.DailyInfo> dailyLevelsInWeekSpan)
        {
            var w = new DailyInfoSummaryData
                {
                    AverageWeight =
                        dailyLevelsInWeekSpan.Count == 0
                            ? 0
                            : Math.Round(
                                dailyLevelsInWeekSpan.Sum(d => d.Weight)/
                                dailyLevelsInWeekSpan.Count(d => d.Weight > 0),
                                Common.Constants.DAILY_INFO_WEIGHT_DIGITS_PERSIST),
                    Hotel = dailyLevelsInWeekSpan.Sum(d => d.Hotel ? 1 : 0),
                    Rest = dailyLevelsInWeekSpan.Sum(d => d.Rest ? 1 : 0),
                    Sick = dailyLevelsInWeekSpan.Sum(d => d.Sick ? 1 : 0),
                    Travel = dailyLevelsInWeekSpan.Sum(d => d.Travel ? 1 : 0)
                };

            return w;
        }

        public override DailyInfoSummaryData CreateEmptyServiceType(TimeSpan span)
        {
            return new DailyInfoSummaryData();
        }

        public override DailyInfoSummaryData InitializeNewServiceType(Model.DailyInfo.DailyInfo dailyInfoInTimeSpan, TimeSpan span)
        {
            throw new NotImplementedException();
        }
    }
}