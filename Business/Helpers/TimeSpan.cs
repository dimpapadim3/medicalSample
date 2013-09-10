using System;
using System.Globalization;

namespace Business.Helpers
{
    public class TimeSpan
    {
        public virtual bool InTimeIncluded(DateTime now)
        {
            return true;
        }

        public virtual string GetSpanDate()
        {
            return ServerDateTimeProvider.Controller.GetServerDateTime().ToString(CultureInfo.InvariantCulture);
        }

        public virtual double TotalDuration
        {
            get { return 0; }
        }
    }

    #region TimeSpans

    public class DailyTimeSpan : TimeSpan
    {
        public DateTime ReferenceDay { get; set; }

        public override bool InTimeIncluded(DateTime day)
        {
            return day.Year == ReferenceDay.Year && day.Month == ReferenceDay.Month && day.Day == ReferenceDay.Day;
        }

        public override string GetSpanDate()
        {
            //return ReferenceDay.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture(Common.Constants.DEFAULT_LOCALE));
            return Utils.DateLocalizer.ToShortDate(Common.Constants.DEFAULT_LOCALE, ReferenceDay) + "\r\n" +
                Utils.DateLocalizer.ToDay(Common.Constants.DEFAULT_LOCALE, ReferenceDay);
        }
    }

    public class WeeklyTimeSpan : TimeSpan
    {
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }

        public override bool InTimeIncluded(DateTime day)
        {
            return DataUtils.CompareDatesInTimeSpan(StartDate, EndDate, day);

            //return (DateTime.Compare(day.Date.Date, StartDate.Date) > 0) && (DateTime.Compare(day.Date.Date, EndDate.Date) <= 0);
        }

        public override string GetSpanDate()
        {
            //return StartDate.ToString("d MMMM yyyy", CultureInfo.CreateSpecificCulture(Common.Constants.DEFAULT_LOCALE));
            return Utils.DateLocalizer.ToShortDate(Common.Constants.DEFAULT_LOCALE, StartDate);
        }


    }

    public class MonthlyTimeSpan : TimeSpan
    {
        public int MonthNumber { get; set; }

        public override bool InTimeIncluded(DateTime day)
        {
            return day.Month == MonthNumber && day.Year == YearNumber;
        }

        public int YearNumber { get; set; }

        public override string GetSpanDate()
        {
            //return MonthNumber + " - " + YearNumber;
            return Utils.DateLocalizer.ToMonthDate(Common.Constants.DEFAULT_LOCALE, new DateTime(YearNumber, MonthNumber, 1));
        }
    }

    public class YearlyTimeSpan : TimeSpan
    {
        public int YearNumber { get; set; }

        public override bool InTimeIncluded(DateTime day)
        {
            return day.Year == YearNumber;
        }

        public override string GetSpanDate()
        {
            return YearNumber.ToString(CultureInfo.InvariantCulture);
        }

    }

    #endregion

}