using System;
using System.Collections.Generic;
using Business.Interfaces;
using Model;
using Model.DailyInfo;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.Helpers
{
    public class MonthlyTrainigDataSummary : PeriodTrainingDataStrategy
    {
        public override IList<TimeSpan> GetTimeSpans(DateTime now, int numberOfRecordsToDisplay)
        {
            var timespans = new List<TimeSpan>();
            for (int i = 0; i < numberOfRecordsToDisplay; i++)
            {
                var refDate = now.AddMonths(-i);
                timespans.Add(new MonthlyTimeSpan
                    {
                        MonthNumber = refDate.Month,
                        YearNumber = refDate.Year
                    });
            }
            return timespans;
        }

        public override bool TimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, IDateTracable d)
        {
            return DateTime.Compare(now.AddMonths(-numberOfMonthsToDisplay), d.Date) <= 0;
        }

        public override bool IsInLowerTimeBound(object d)
        {
            return true;
        }

        public override int NumberOfRecords
        {
            get { return Common.Constants.NUMBER_OF_MONTH_RECORDS; }
        }
    }
}