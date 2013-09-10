using System;
using System.Collections.Generic;
using Business.Helpers;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.DailyInfo.Helpers
{
    internal class DailyInfoDailyDataHelper : DailyyDataHelper<Model.DailyInfo.DailyInfo, Model.DailyInfo.DailyInfo>
    {
        public override IList<Model.DailyInfo.DailyInfo> GetViewDataForTimePeriod(int userId, DateTime now,int numberOfMonthsToDisplay)
        {
            return DailyInfoService.TranctuateDigits(base.GetViewDataForTimePeriod(userId, now, numberOfMonthsToDisplay));
        }

        public override Model.DailyInfo.DailyInfo InitializeNewServiceType(Model.DailyInfo.DailyInfo dailyInfoInTimeSpan, TimeSpan span)
        {
            return dailyInfoInTimeSpan;
        }

        public override Model.DailyInfo.DailyInfo CreateEmptyServiceType(TimeSpan span)
        {
            return new Model.DailyInfo.DailyInfo();
        }
    }
}