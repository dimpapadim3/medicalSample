using System;
using System.Collections.Generic;
using Business.Interfaces;
using Model;
using Model.DailyInfo;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.Helpers
{
    public class YearlyTrainigDataSummary : PeriodTrainingDataStrategy
    {
        public override IList<TimeSpan> GetTimeSpans(DateTime now, int numberOfRecordsToDisplay)
        {
            var timespans = new List<TimeSpan>();
            for (int i = 0; i < numberOfRecordsToDisplay; i++)
            {
                timespans.Add(new YearlyTimeSpan { YearNumber = now.AddYears(-i).Year });
            }
            return timespans;
        }

        public override bool TimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, IDateTracable d)
        {
            return DateTime.Compare(now.AddYears(-numberOfMonthsToDisplay), d.Date) <= 0;
        }

        public override bool IsInLowerTimeBound(object d)
        {
            return true;
        }

        public override int NumberOfRecords
        {
            get { return Common.Constants.NUMBER_OF_YEAR_RECORDS; }
        }
    }
}