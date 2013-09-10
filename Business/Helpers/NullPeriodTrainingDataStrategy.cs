using System;
using System.Collections.Generic;
using Business.Interfaces;
using Model;
using Model.DailyInfo;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.Helpers
{
    public class NullPeriodTrainingDataStrategy : PeriodTrainingDataStrategy
    {
       
        public override int NumberOfRecords
        {
            get { return 0; }
        }

        public override bool IsInLowerTimeBound(object d)
        {
            return false;
        }

        public override IList<TimeSpan> GetTimeSpans(DateTime now, int numberOfRecordsToDisplay)
        {
            return new List<TimeSpan>();
        }

        public override bool TimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, IDateTracable d)
        {
            return true; 
        }
    }
}
