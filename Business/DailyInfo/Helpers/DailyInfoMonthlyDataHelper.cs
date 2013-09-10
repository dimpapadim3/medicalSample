using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using Common;
using DataAccess.Interfaces;
using Model.DailyInfo;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.DailyInfo.Helpers
{
    internal class DailyInfoMonthlyDataHelper : MonthlyDataHelper<DailyInfoSummaryData, DailyInfoSummaryData>
    {
        public IRepositoryBase<DailyInfoSummaryData> MonthlyDailyInfoSummaryDataRepository { get; set; }

        public INoSqlFinalValuesRepositoryBase<DailyInfoSummaryData> FinalMonthlyDailyInfoSummaryDataRepository { get; set; }

        public override IList<DailyInfoSummaryData> GetViewDataForTimePeriod(int userId, DateTime now,
                                                                             int numberOfMonthsToDisplay)
        {
            IQueryable<DailyInfoSummaryData> infos =
                MonthlyDailyInfoSummaryDataRepository.GetAsQueryable<DailyInfoSummaryData>();

            if (infos.Any())
            {
                DailyInfoSummaryData currentMonthEnergyInfo = infos.Last();

                IEnumerable<DailyInfoSummaryData> lastMonthsEnergyLevelInfo = FinalMonthlyDailyInfoSummaryDataRepository
                    .GetAsQueryable<DailyInfoSummaryData>().ToList()
                    .Where(
                        d => d.UserId == userId && DoGetInTimePeriodFilterComparision(now, numberOfMonthsToDisplay, d));

                //aggregate lists
                var allMontlyEnergyInfo = new List<DailyInfoSummaryData> {currentMonthEnergyInfo};
                allMontlyEnergyInfo.AddRange(lastMonthsEnergyLevelInfo);
                InitializePeriodStrategy(userId);
                IList<TimeSpan> monthlyTimeSpans = PeriodSelectionStrategy.GetTimeSpans(now, numberOfMonthsToDisplay);
                List<DailyInfoSummaryData> energyLevelInfoViewData = FillRecordsOnEmptyTimeSpans(allMontlyEnergyInfo,
                                                                                                 monthlyTimeSpans);

                return DailyInfoService.TranctuateDigits(energyLevelInfoViewData);
            }
            return new List<DailyInfoSummaryData>();
        }


        public override DailyInfoSummaryData InitializeNewServiceType(DailyInfoSummaryData dailyInfoInTimeSpan,
                                                                      TimeSpan span)
        {
            return dailyInfoInTimeSpan;
        }

        public override DailyInfoSummaryData CreateEmptyServiceType(TimeSpan span)
        {
            return new DailyInfoSummaryData();
        }
    }

    internal class DailyInfoMonthlyDataHelperDynamic :MonthlyDynamicDataHelper<DailyInfoSummaryData, Model.DailyInfo.DailyInfo>
    {
        public override IList<DailyInfoSummaryData> GetViewDataForTimePeriod(int userId, DateTime now,
                                                                             int numberOfMonthsToDisplay)
        {
            return DailyInfoService.TranctuateDigits(base.GetViewDataForTimePeriod(userId, now, numberOfMonthsToDisplay));
        }

        protected override DailyInfoSummaryData DoGetServiceTypeInTimeSpan(
            List<Model.DailyInfo.DailyInfo> dailyLevelsInWeekSpan,
            TimeSpan dailyTimeSpan)
        {
            TimeSpan span = dailyTimeSpan;

            IEnumerable<Model.DailyInfo.DailyInfo> dailyInfoInTimeSpan =
                dailyLevelsInWeekSpan.Where(energyevel => span.InTimeIncluded(energyevel.Date));
            DailyInfoSummaryData serviceType = InitializeNewServiceType(dailyInfoInTimeSpan.ToList());
            return serviceType;
        }

        public static DailyInfoSummaryData InitializeNewServiceType(
            IList<Model.DailyInfo.DailyInfo> dailyLevelsInWeekSpan)
        {
            var w = new DailyInfoSummaryData
                {
                    AverageWeight = dailyLevelsInWeekSpan.Count == 0
                                        ? 0
                                        : Math.Round(
                                            dailyLevelsInWeekSpan.Sum(d => d.Weight)/
                                            (dailyLevelsInWeekSpan.Count(d => d.Weight > 0)),
                                            Constants.DAILY_INFO_WEIGHT_DIGITS_PERSIST),
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

        public override DailyInfoSummaryData InitializeNewServiceType(Model.DailyInfo.DailyInfo dailyInfoInTimeSpan,
                                                                      TimeSpan span)
        {
            throw new NotImplementedException();
        }
    }
}