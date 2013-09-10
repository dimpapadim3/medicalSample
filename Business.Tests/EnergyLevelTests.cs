using System;
using System.Collections.Generic;
using System.Linq;
using Business.EnergyLevelInfo.Helpers;
using Business.Helpers;
using Business.Interfaces;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.EnergyLevelInfo;
using StructureMap;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.Tests
{
    [TestClass]
    public class EnergyLevelTests
    {
        #region Day

        [TestInitialize]
        public void EnergyLevelInfoTestsInitialize()
        {
            IocConfigurator.Configure();
        }

        private long GetNewUserId()
        {
            return 75;
        }

        [TestMethod]
        public void ShouldIncludeTheCorrectDataInTimeSpans()
        {
            var now = new DateTime(2013, 4, 23);
            //Is Tuesday 
            Assert.IsTrue(now.DayOfWeek == DayOfWeek.Tuesday);

            var weeklyDataHelper = new WeeklyTrainigDataSummary();

            weeklyDataHelper.FirstDayOfTheWeek = DataUtils.GetDayNumber("Thursday");

            IList<TimeSpan> weeklyspans = weeklyDataHelper.GetTimeSpans(now, Constants.NUMBER_OF_WEEK_RECORDS);

            var energyevels = new List<DailyEnergyLevel>
                {
                    new DailyEnergyLevel {EnergyLevel = 0, Date = new DateTime(2013, 4, 19)},
                    new DailyEnergyLevel {EnergyLevel = 3, Date = new DateTime(2013, 4, 22)},
                    new DailyEnergyLevel {EnergyLevel = 3, Date = new DateTime(2013, 4, 23)},
                };

            var dataHelper = new WeeklyDataHelper();

            List<ServiceModel.EnergyLevel.EnergyLevelInfo> data = dataHelper.FillRecordsOnEmptyTimeSpans(energyevels,
                                                                                                         weeklyspans);

            Assert.IsTrue(data[0].Value == (energyevels.Sum(e => e.EnergyLevel)/energyevels.Count));
        }

        [TestMethod]
        public List<TimeSpan> WeeklyDataHelperGenerateCorrectTimeSpans()
        {
            var targetWeeklyTieSpans = new List<WeeklyTimeSpan>
                {
                    new WeeklyTimeSpan {StartDate = new DateTime(2013, 4, 22), EndDate = new DateTime(2013, 4, 23)},
                    new WeeklyTimeSpan {StartDate = new DateTime(2013, 4, 15), EndDate = new DateTime(2013, 4, 21)},
                    new WeeklyTimeSpan {StartDate = new DateTime(2013, 4, 8), EndDate = new DateTime(2013, 4, 14)},
                    new WeeklyTimeSpan {StartDate = new DateTime(2013, 4, 1), EndDate = new DateTime(2013, 4, 7)},
                };


            var now = new DateTime(2013, 4, 23);
            //Is Tuesday 
            Assert.IsTrue(now.DayOfWeek == DayOfWeek.Tuesday);

            var weeklyDataHelper = new WeeklyTrainigDataSummary();

            weeklyDataHelper.FirstDayOfTheWeek = DataUtils.GetDayNumber("Monday");

            IList<TimeSpan> weeklyspans = weeklyDataHelper.GetTimeSpans(now, Constants.NUMBER_OF_WEEK_RECORDS);

            for (int i = 0; i < targetWeeklyTieSpans.Count; i++)
            {
                //Every start date is monday 
                Assert.IsTrue(((WeeklyTimeSpan) weeklyspans[i]).StartDate.DayOfWeek == DayOfWeek.Monday);

                Assert.IsTrue(((WeeklyTimeSpan) weeklyspans[i]).StartDate.Day == targetWeeklyTieSpans[i].StartDate.Day);
                Assert.IsTrue(((WeeklyTimeSpan) weeklyspans[i]).EndDate.Day == targetWeeklyTieSpans[i].EndDate.Day);
            }
            return weeklyspans.ToList();
        }


        [TestMethod]
        public void GetLastDaysEnergyLevelTest_OnlyEvenDaysWereFilled()
        {
            TestHelper.DropEnergyLevels();

            var energyLevelInfoService = ObjectFactory.GetInstance<IEnergyLevelInfoService>();
            long userId = GetNewUserId();

            var dailyEnergyLevels = new List<DailyEnergyLevel>();

            for (int i = -50; i <= 0; i = i + 2)
            {
                var energyLevel = new DailyEnergyLevel
                    {
                        UserId = userId,
                        Date = DateTime.Now.AddDays(i),
                        EnergyLevel = 1,
                    };

                energyLevelInfoService.InsertDailyEnergyLevel(energyLevel);
                dailyEnergyLevels.Add(energyLevel);
            }

            IList<ServiceModel.EnergyLevel.EnergyLevelInfo> dayLevels =
                energyLevelInfoService.GetEnergyLevelsForTimePeriod((int) userId, DateTime.Now, 0);

            Assert.IsTrue(dayLevels.Count(e => e.Value == 1) == 4);
        }

        #endregion

        #region Month

        [TestMethod]
        public void MonthlyEnergyLevelsTest_ShouldIncludeTheCorrectDataInTimeSpans()
        {
            var initialTime = new DateTime(2013, 4, 23);

            TestHelper.DropEnergyLevels();

            var energyLevelInfoService = ObjectFactory.GetInstance<IEnergyLevelInfoService>();
            long userId = GetNewUserId();

            var dailyEnergyLevels = new List<DailyEnergyLevel>();

            const int daysBackWard = 50;

            for (int i = -daysBackWard; i <= 0; i = i + 1)
            {
                var energyLevel = new DailyEnergyLevel
                    {
                        UserId = userId,
                        Date = initialTime.AddDays(i),
                        EnergyLevel = 1,
                    };

                energyLevelInfoService.InsertDailyEnergyLevel(energyLevel);
                dailyEnergyLevels.Add(energyLevel);
            }

            IList<ServiceModel.EnergyLevel.EnergyLevelInfo> dayLevels =
                energyLevelInfoService.GetEnergyLevelsForTimePeriod((int) userId, initialTime, 2);

            Assert.IsTrue(dayLevels.Count(e => e.Value == 1) == 2);
        }

        [TestMethod]
        public void MonthlyEnergyLevelsTest_ExactlyOneMonth()
        {
            var initialTime = new DateTime(2013, 4, 23);

            TestHelper.DropEnergyLevels();

            var energyLevelInfoService = ObjectFactory.GetInstance<IEnergyLevelInfoService>();
            long userId = GetNewUserId();

            var dailyEnergyLevels = new List<DailyEnergyLevel>();
            const int daysBackWard = 22;

            for (int i = -daysBackWard; i <= 0; i = i + 1)
            {
                var energyLevel = new DailyEnergyLevel
                    {
                        UserId = userId,
                        Date = initialTime.AddDays(i),
                        EnergyLevel = 1,
                    };

                energyLevelInfoService.InsertDailyEnergyLevel(energyLevel);
                dailyEnergyLevels.Add(energyLevel);
            }

            IList<ServiceModel.EnergyLevel.EnergyLevelInfo> dayLevels =
                energyLevelInfoService.GetEnergyLevelsForTimePeriod((int) userId, initialTime, 2);

            Assert.IsTrue(dayLevels.Count(e => e.Value == 1) == 1);
        }

        [TestMethod]
        public void MonthlyEnergyLevelsTest_ExactlyTwoMonths()
        {
            var initialTime = new DateTime(2013, 4, 23);

            TestHelper.DropEnergyLevels();

            var energyLevelInfoService = ObjectFactory.GetInstance<IEnergyLevelInfoService>();
            long userId = GetNewUserId();

            var dailyEnergyLevels = new List<DailyEnergyLevel>();
            const int daysBackWard = 22 + 30;

            for (int i = -daysBackWard; i <= 0; i = i + 1)
            {
                var energyLevel = new DailyEnergyLevel
                    {
                        UserId = userId,
                        Date = initialTime.AddDays(i),
                        EnergyLevel = 1,
                    };

                energyLevelInfoService.InsertDailyEnergyLevel(energyLevel);
                dailyEnergyLevels.Add(energyLevel);
            }

            IList<ServiceModel.EnergyLevel.EnergyLevelInfo> dayLevels =
                energyLevelInfoService.GetEnergyLevelsForTimePeriod((int) userId, initialTime, 2);

            Assert.IsTrue(dayLevels.Count(e => e.Value == 1) == 2);
        }

        [TestMethod]
        public void MonthlyEnergyLevelsTest_FirstMonthContainingOnlyOneDay()
        {
            var initialTime = new DateTime(2013, 4, 23);

            TestHelper.DropEnergyLevels();

            var energyLevelInfoService = ObjectFactory.GetInstance<IEnergyLevelInfoService>();
            long userId = GetNewUserId();

            var dailyEnergyLevels = new List<DailyEnergyLevel>();

            const int daysBackWard = 23;

            for (int i = -daysBackWard; i <= 0; i = i + 1)
            {
                var energyLevel = new DailyEnergyLevel
                    {
                        UserId = userId,
                        Date = initialTime.AddDays(i),
                        EnergyLevel = 1,
                    };

                energyLevelInfoService.InsertDailyEnergyLevel(energyLevel);
                dailyEnergyLevels.Add(energyLevel);
            }

            IList<ServiceModel.EnergyLevel.EnergyLevelInfo> dayLevels =
                energyLevelInfoService.GetEnergyLevelsForTimePeriod((int) userId, initialTime, 2);

            Assert.IsTrue(dayLevels.Count(e => e.Value == 1) == 2);
        }

        #endregion

        #region Year

        [TestMethod]
        public void YearlyEnergyLevelsTest_ShouldIncludeTheCorrectDataInTimeSpans_ExactlyOneYear()
        {
            var initialTime = new DateTime(2013, 4, 23);

            TestHelper.DropEnergyLevels();

            var energyLevelInfoService = ObjectFactory.GetInstance<IEnergyLevelInfoService>();

            long userId = GetNewUserId();

            var dailyEnergyLevels = new List<DailyEnergyLevel>();

            int daysBackWard =
                DateTime.DaysInMonth(2013, 1) +
                DateTime.DaysInMonth(2013, 2) +
                DateTime.DaysInMonth(2013, 3) + 22;

            for (int i = -daysBackWard; i <= 0; i = i + 1)
            {
                var energyLevel = new DailyEnergyLevel
                    {
                        UserId = userId,
                        Date = initialTime.AddDays(i),
                        EnergyLevel = (byte) (-i%3),
                    };

                energyLevelInfoService.InsertDailyEnergyLevel(energyLevel);
                dailyEnergyLevels.Add(energyLevel);
            }

            IList<ServiceModel.EnergyLevel.EnergyLevelInfo> dayLevels =
                energyLevelInfoService.GetEnergyLevelsForTimePeriod((int) userId, initialTime, Constants.YEAR_PERIOD);

            Assert.IsTrue(dayLevels.Count(e => e.Value > 0) == 1);
            Assert.IsTrue(Math.Abs(dayLevels[0].Value - dailyEnergyLevels.Average(c => c.EnergyLevel)) < 0.1);
        }

        [TestMethod]
        public void YearlyEnergyLevelsTest_ExactlyTwoYears()
        {
            var initialTime = new DateTime(2013, 4, 23);

            TestHelper.DropEnergyLevels();

            var energyLevelInfoService = ObjectFactory.GetInstance<IEnergyLevelInfoService>();
            long userId = GetNewUserId();

            var dailyEnergyLevels = new List<DailyEnergyLevel>();
            int daysBackWard =
                DateTime.DaysInMonth(2013, 1) +
                DateTime.DaysInMonth(2013, 2) +
                DateTime.DaysInMonth(2013, 3) + 23;

            for (int i = -daysBackWard; i <= 0; i = i + 1)
            {
                var energyLevel = new DailyEnergyLevel
                    {
                        UserId = userId,
                        Date = initialTime.AddDays(i),
                        EnergyLevel = (byte) (-i%3),
                    };

                energyLevelInfoService.InsertDailyEnergyLevel(energyLevel);
                dailyEnergyLevels.Add(energyLevel);
            }

            IList<ServiceModel.EnergyLevel.EnergyLevelInfo> dayLevels =
                energyLevelInfoService.GetEnergyLevelsForTimePeriod((int) userId, initialTime, Constants.YEAR_PERIOD);

            Assert.IsTrue(dayLevels.Count(e => e.Value > 0) == 2);
            Assert.IsTrue(
                Math.Abs(dayLevels[1].Value -
                         dailyEnergyLevels.Where(d => d.Date.Year == 2012).Average(c => c.EnergyLevel)) < 0.1);
        }

        [TestMethod]
        public void YearlyEnergyLevelsTest_FirstYearContainingOnlyOneDay()
        {
            var initialTime = new DateTime(2013, 4, 23);

            TestHelper.DropEnergyLevels();

            var energyLevelInfoService = ObjectFactory.GetInstance<IEnergyLevelInfoService>();
            long userId = GetNewUserId();

            var dailyEnergyLevels = new List<DailyEnergyLevel>();

            int daysBackWard = DateTime.DaysInMonth(2013, 1) + DateTime.DaysInMonth(2013, 2) +
                               DateTime.DaysInMonth(2013, 3) + 23;

            for (int i = -daysBackWard; i <= 0; i = i + 1)
            {
                var energyLevel = new DailyEnergyLevel
                    {
                        UserId = userId,
                        Date = initialTime.AddDays(i),
                        EnergyLevel = (byte) (-i%3),
                    };

                energyLevelInfoService.InsertDailyEnergyLevel(energyLevel);
                dailyEnergyLevels.Add(energyLevel);
            }

            IList<ServiceModel.EnergyLevel.EnergyLevelInfo> dayLevels =
                energyLevelInfoService.GetEnergyLevelsForTimePeriod((int) userId, initialTime, Constants.YEAR_PERIOD);

            Assert.IsTrue(dayLevels.Count(e => e.Value > 0) == 2);
        }

        #endregion
    }
}