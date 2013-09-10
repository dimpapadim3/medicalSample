using System;
using System.Collections.Generic;
using Business.Interfaces;
using Model;
using Model.DailyInfo;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.Helpers
{
    public class WeeklyTrainigDataSummary : PeriodTrainingDataStrategy
    {
        public int FirstDayOfTheWeek { get; set; }

        public override IList<TimeSpan> GetTimeSpans(DateTime now, int numberOfWeeksToDisplay)
        {
            var weeksTimespans = new List<TimeSpan>();
            var remainingWeeksEndDate = RemainingWeeksEndDate(now);

            if (ShouldDisplayCurrentWeek(now))
            {
                weeksTimespans.Add(GetCurrentWeeklyTimeSpan(now));
                numberOfWeeksToDisplay--;
            }
            else
            {
                remainingWeeksEndDate = remainingWeeksEndDate.AddDays(7);
            }

            for (int weekNumberShift = 0; weekNumberShift < numberOfWeeksToDisplay; weekNumberShift++)
            {
                weeksTimespans.Add(new WeeklyTimeSpan
                    {
                        StartDate = remainingWeeksEndDate.AddDays(-(weekNumberShift + 1) * 7 ),
                        EndDate = remainingWeeksEndDate.AddDays(-weekNumberShift * 7 - 1)
                    });
            }
            return weeksTimespans;
        }

        private bool ShouldDisplayCurrentWeek(DateTime now)
        {
            return (DataUtils.CompareDatesAfterStartDate(ServerDateTimeProvider.Controller.GetServerDateTime(), now));

        }

        private WeeklyTimeSpan GetCurrentWeeklyTimeSpan(DateTime now)
        {
            var timeSpan = new WeeklyTimeSpan();

            var curretDayOfWeek = (int)now.DayOfWeek;
            var daysDifference = Math.Abs(curretDayOfWeek - FirstDayOfTheWeek);

            if (curretDayOfWeek >= FirstDayOfTheWeek)
            {
                timeSpan.StartDate = now.AddDays(-daysDifference);
                timeSpan.EndDate = now.Date;
            }
            if (curretDayOfWeek < FirstDayOfTheWeek)
            {
                timeSpan.StartDate = now.AddDays(-(7 - daysDifference));
                timeSpan.EndDate = now.Date;
            }
            return timeSpan;
        }


        private DateTime RemainingWeeksEndDate(DateTime now)
        {
            DateTime tempDate = new DateTime();

            var curretDayOfWeek = (int)now.DayOfWeek;
            var daysDifference = Math.Abs(curretDayOfWeek - FirstDayOfTheWeek);

            if (curretDayOfWeek >= FirstDayOfTheWeek)
            {
                tempDate = now.AddDays(-daysDifference);
            }
            if (curretDayOfWeek < FirstDayOfTheWeek)
            {
                tempDate = now.AddDays(-(7 - daysDifference));
            }

            return tempDate;

        }

        public override bool TimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, IDateTracable d)
        {
            return DateTime.Compare(now.AddDays(-numberOfMonthsToDisplay * 7), d.Date) <= 0;
        }

        public override bool IsInLowerTimeBound(object d)
        {
            return true;
        }

        public override int NumberOfRecords
        {
            get { return Common.Constants.NUMBER_OF_WEEK_RECORDS; }
        }
    }
}