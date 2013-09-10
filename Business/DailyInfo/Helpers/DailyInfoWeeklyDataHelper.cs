using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using Model.DailyInfo;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.DailyInfo.Helpers
{
    public class DailyInfoWeeklyDataHelper : WeeklyDataHelper<DailyInfoSummaryData, Model.DailyInfo.DailyInfo>
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