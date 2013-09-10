using System;
using System.Collections.Generic;
using Model;
using Model.DailyInfo;

namespace Business.Interfaces
{

    #region Time Period Strategies
    public abstract class PeriodTrainingDataStrategy
    {
        public static bool CompareDates(DateTime startDate, DateTime endDate, DateTime dateTime)
        {
            return ((startDate.Date.CompareTo(dateTime.Date) <= 0)) && ((dateTime.Date.CompareTo(endDate) <= 0));
        }

        public abstract int NumberOfRecords { get; }

        public void GetDataCalculate(DateTime from, DateTime to)
        {
        }

        public abstract bool IsInLowerTimeBound(object d);

        public abstract IList<TimeSpan> GetTimeSpans(DateTime now, int numberOfRecordsToDisplay);


        public abstract bool TimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, IDateTracable d);

        public virtual List<TrainingSession> FilterLessThanHourActivities(List<TrainingSession> s)
        {
            return s;
        }
    }


    #endregion

}