using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Business.Interfaces;
using Common;
using ErrorClasses;
using Model.DailyInfo;
using Model.View;
using ServiceModel.DailyInfo;
using TimeSpan = Business.Interfaces.TimeSpan;
using StructureMap;

namespace Business.Helpers
{
    public class DataUtils
    {
        public static bool CompareDatesInTimeSpan(DateTime startDate, DateTime endDate, DateTime dateTime)
        {
            return CompareDatesAfterStartDate(startDate, dateTime) && CompareDatesBeforeEndDate(dateTime, endDate);
        }

        public static bool CompareDatesAfterStartDate(DateTime startDate, DateTime dateTime)
        {
            return (startDate.Date.CompareTo(dateTime.Date) <= 0);
        }

        public static bool CompareDatesAfterNotIncludingStartDate(DateTime startDate, DateTime dateTime)
        {
            return (startDate.Date.CompareTo(dateTime.Date) < 0);
        }
        public static bool CompareDatesBeforeEndDate(DateTime dateTime, DateTime endDate)
        {
            return (dateTime.Date.CompareTo(endDate) <= 0);
        }

        static IUserService _userService = ObjectFactory.GetInstance<IUserService>();

        public static int GetFirstDayOfTheWeekFromUserId(int userId)
        {
            GenericError error;
            var userName = SqlDataAccess.Entities.User.GetUserNameById(userId, out error);
            var user = _userService.GetUserInfo(userName, out error);
            int firstDayOfTheWeek = GetDayNumber(user.FirstDayOfTheWeek);
            return firstDayOfTheWeek;
        }

        public static PeriodTrainingDataStrategy GetPeriodTrainingDataStrategy(int userId, int period)
        {

            if (period == 0) return new DailyTrainingDataSummary();
            if (period == 1) return new WeeklyTrainigDataSummary { FirstDayOfTheWeek = GetFirstDayOfTheWeekFromUserId(userId) };
            if (period == 2) return new MonthlyTrainigDataSummary();
            if (period == 3) return new YearlyTrainigDataSummary();
            return new NullPeriodTrainingDataStrategy();

        }

        public static int GetDayNumber(string p)
        {

            int dayNum = 0;

            if (p == "Monday")
                dayNum = 1;
            if (p == "Tuesday")
                dayNum = 2;
            if (p == "Wednesday")
                dayNum = 3;
            if (p == "Thursday")
                dayNum = 4;
            if (p == "Friday")
                dayNum = 5;
            if (p == "Saturday")
                dayNum = 6;
            if (p == "Sunday")
                dayNum = 7;
            return dayNum;

        }

        public static void NormalizeData(IList<TrainingDataTainingDetailsModel.TrainingDataValue> data, float maxValue)
        {
            var dataMaxValue = data.Max(s => s.Value);
            if (Math.Abs((float)(dataMaxValue - 0)) < Constants.epsilon)
                return;
            var ratio = maxValue / dataMaxValue;
            for (int i = 0; i < data.Count; i++)
            {
                data[i].Value *= ratio;
            }
        }
        public static readonly Dictionary<Types.MeasurementInfoTypes, Func<TrainingSessionMeasurmentData, IList<DecimalData>>> MeasurmentPropertySelectors =
            new Dictionary<Types.MeasurementInfoTypes, Func<TrainingSessionMeasurmentData, IList<DecimalData>>>
                {
                    {Types.MeasurementInfoTypes.HeartRate, m =>  m.GetData(Types.MeasurementInfoTypes.HeartRate)},
                    {Types.MeasurementInfoTypes.RespirationRate, m =>   m.GetData(Types.MeasurementInfoTypes.RespirationRate)},
                    {Types.MeasurementInfoTypes.EPOC, m =>  m.GetData(Types.MeasurementInfoTypes.EPOC)},
                    {Types.MeasurementInfoTypes.Calories, m =>   m.GetData(Types.MeasurementInfoTypes.Calories)},
                    {Types.MeasurementInfoTypes.BodyTemp, m =>  m.GetData(Types.MeasurementInfoTypes.BodyTemp)},
                    {Types.MeasurementInfoTypes.Speed, m =>   m.GetData(Types.MeasurementInfoTypes.Speed)},
                    {Types.MeasurementInfoTypes.Distance, m =>  m.GetData(Types.MeasurementInfoTypes.Distance)},
                    {Types.MeasurementInfoTypes.LongitudinalAccel, m =>   m.GetData(Types.MeasurementInfoTypes.LongitudinalAccel)},
                    {Types.MeasurementInfoTypes.TotalSteps, m =>  m.GetData(Types.MeasurementInfoTypes.TotalSteps)},
                    {Types.MeasurementInfoTypes.Respiration, m =>    m.GetData(Types.MeasurementInfoTypes.Respiration)},
                    {Types.MeasurementInfoTypes.ECG, m =>   m.GetData(Types.MeasurementInfoTypes.ECG)},
                    {Types.MeasurementInfoTypes.ThreeDAcceleration,m=>m.GetData(Types.MeasurementInfoTypes.ThreeDAcceleration)},
                    {Types.MeasurementInfoTypes.ActivityClass,m=>m.GetData(Types.MeasurementInfoTypes.ActivityClass)},
                    {Types.MeasurementInfoTypes.Cadence, m =>  m.GetData(Types.MeasurementInfoTypes.Cadence)},
                };

        public static readonly Dictionary<Types.MeasurementInfoTypes, Action<TrainingSessionMeasurmentData, List<DecimalData>>> MeasurmentPropertySetters =
            new Dictionary<Types.MeasurementInfoTypes, Action<TrainingSessionMeasurmentData, List<DecimalData>>>
                {
                    {Types.MeasurementInfoTypes.HeartRate, (m,v) =>   m.SetData(Types.MeasurementInfoTypes.HeartRate,v) },
                    {Types.MeasurementInfoTypes.RespirationRate,  (m,v) =>   m.SetData(Types.MeasurementInfoTypes.RespirationRate,v)},
                    {Types.MeasurementInfoTypes.EPOC,  (m,v) => m.SetData(Types.MeasurementInfoTypes.EPOC,v)},
                    {Types.MeasurementInfoTypes.Calories,  (m,v) =>   m.SetData(Types.MeasurementInfoTypes.Calories,v)},
                    {Types.MeasurementInfoTypes.BodyTemp, (m,v) =>   m.SetData(Types.MeasurementInfoTypes.BodyTemp,v)},
                    {Types.MeasurementInfoTypes.Speed,  (m,v) =>   m.SetData(Types.MeasurementInfoTypes.Speed,v)},
                    {Types.MeasurementInfoTypes.Distance,  (m,v) => m.SetData(Types.MeasurementInfoTypes.Distance,v)},
                    {Types.MeasurementInfoTypes.LongitudinalAccel,  (m,v) =>   m.SetData(Types.MeasurementInfoTypes.LongitudinalAccel,v)},
                    {Types.MeasurementInfoTypes.TotalSteps,  (m,v) =>   m.SetData(Types.MeasurementInfoTypes.TotalSteps,v)},
                    {Types.MeasurementInfoTypes.Respiration,  (m,v) =>   m.SetData(Types.MeasurementInfoTypes.Respiration,v)},
                    {Types.MeasurementInfoTypes.ECG,  (m,v) =>  m.SetData(Types.MeasurementInfoTypes.ECG,v)},
                    {Types.MeasurementInfoTypes.ThreeDAcceleration, (m,v)=>m.SetData(Types.MeasurementInfoTypes.ThreeDAcceleration,v)},
                    {Types.MeasurementInfoTypes.ActivityClass, (m,v)=>m.SetData(Types.MeasurementInfoTypes.ActivityClass,v)},
                    {Types.MeasurementInfoTypes.Cadence,  (m,v) =>  m.SetData(Types.MeasurementInfoTypes.Cadence,v)},


                };
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