using System;
using System.Collections.Generic;
using System.Linq;
using Business.Interfaces;
using Model;
using Model.DailyInfo;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.Helpers
{
    public class DailyTrainingDataSummary : PeriodTrainingDataStrategy
    {
        public override IList<TimeSpan> GetTimeSpans(DateTime now, int numberOfRecordsToDisplay)
        {
            var timespans = new List<TimeSpan>();
            for (int i = 0; i < numberOfRecordsToDisplay; i++)
            {
                timespans.Add(new DailyTimeSpan { ReferenceDay = now.AddDays(-i) });
            }
            return timespans;
        }

        public override bool IsInLowerTimeBound(object d)
        {
            return true;
        }

        public override int NumberOfRecords
        {
            get { return Common.Constants.NUMBER_OF_DAY_RECORDS; }
        }

        public override bool TimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, IDateTracable d)
        {
            // return DateTime.Compare(now.AddDays(-numberOfMonthsToDisplay), d.Date) <= 0;
            return DataUtils.CompareDatesAfterStartDate(now.AddDays(-numberOfMonthsToDisplay), d.Date);
        }

        public override List<TrainingSession> FilterLessThanHourActivities(List<TrainingSession> sessions)
        {
            return
                sessions.Where(
                    s =>
                    new System.TimeSpan(s.DateTrainingEnd.Day - s.DateTrainingStart.Day, s.DateTrainingEnd.Hour - s.DateTrainingStart.Hour, s.DateTrainingEnd.Minute - s.DateTrainingStart.Minute, 0)
                        .TotalMinutes > 60).ToList();
        }
    }
}