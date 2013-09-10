using System;
using System.Collections.Generic;
using Model;
using Model.DailyInfo;

namespace Business.Helpers
{

    #region Time Period Strategies
    public abstract class PeriodTrainingDataStrategy
    {
        public static bool CompareDates(DateTime startDate, DateTime endDate, DateTime dateTime)
        {
            return DataUtils.CompareDatesAfterStartDate(startDate, dateTime) && DataUtils.CompareDatesBeforeEndDate(dateTime, endDate);
        }

        public abstract int NumberOfRecords { get; }

        public void GetDataCalculate(DateTime from, DateTime to)
        {
        }

        public abstract bool IsInLowerTimeBound(TrainingSession d);

        public abstract IList<TimeSpan> GetTimeSpans(DateTime now, int numberOfRecordsToDisplay);


        internal abstract bool TimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, IDateTracable d);

        internal virtual List<TrainingSession> FilterLessThanHourActivities(List<TrainingSession> s)
        {
            return s;
        }
    }
     

    #endregion

}