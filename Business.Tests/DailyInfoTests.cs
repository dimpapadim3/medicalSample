using System;
using System.Collections.Generic;
using System.Linq;
using Business.Interfaces;
using Common;
using ErrorClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.DailyInfo;
using ServiceModel.DailyInfo;
using StructureMap;

namespace Business.Tests
{
    /// <summary>
    ///     Summary description for DailyInfoTests
    /// </summary>
    [TestClass]
    public class DailyInfoTests
    {
        private const int UserId = 75;
        private readonly DateTime _now = new DateTime(2013, 2, 13);
        public float Epsilon = (float) 0.000001;

        public TestContext TestContext { get; set; }

        #region Test DailyInfos Periods

        [TestMethod]
        public void ShouldHaveOneDigitForAllPeriods()
        {
            Constants.DAILY_INFO_WEIGHT_DIGITS_PERSIST = 1;
            Constants.DAILY_INFO_WEIGHT_DIGITS_DISPLAY = 1;

            TestHelper.DropDailyInfoCollections();

            var service = ObjectFactory.GetInstance<IDailyInfoService>();


            var info = new Model.DailyInfo.DailyInfo
                {
                    UserId = UserId,
                    Hotel = true,
                    Weight = 50.23,
                };

            service.InsertDailyInfo(UserId, info, _now);
            ServiceDailyInfo data = service.GetDayInfoViewService(UserId, Constants.NUMBER_OF_DAY_RECORDS, _now);

            Assert.IsTrue(Math.Abs(data.Weight[0] - 50.2) < 0.1);
 
            ServiceDailyInfoSummaryData sumdata = service.GetDailyInfoServiceViewData(UserId, 2, _now);
            Assert.IsTrue(Math.Abs(sumdata.AverageWeight.Last() - 50.2) < 0.1);

            sumdata = service.GetDailyInfoServiceViewData(UserId, 3, _now);
            Assert.IsTrue(Math.Abs(sumdata.AverageWeight.Last() - 50.2) < 0.1);
        }

        [TestMethod]
        public void ShouldDislayOneDigitForAllPeriods()
        {
            Constants.DAILY_INFO_WEIGHT_DIGITS_PERSIST = 2;
            Constants.DAILY_INFO_WEIGHT_DIGITS_DISPLAY = 1;

            TestHelper.DropDailyInfoCollections();

            var service = ObjectFactory.GetInstance<IDailyInfoService>();


            var info = new Model.DailyInfo.DailyInfo
                {
                    UserId = UserId,
                    Hotel = true,
                    Weight = 50.23,
                };

            service.InsertDailyInfo(UserId, info, _now);
            ServiceDailyInfo data = service.GetDayInfoViewService(UserId, Constants.NUMBER_OF_DAY_RECORDS, _now);

            Assert.IsTrue(Math.Abs(data.Weight[0] - 50.2) < 0.1);


            ServiceDailyInfoSummaryData sumdata = service.GetDailyInfoServiceViewData(UserId, 2, _now);
            Assert.IsTrue(Math.Abs(sumdata.AverageWeight.Last() - 50.2) < 0.1);

            sumdata = service.GetDailyInfoServiceViewData(UserId, 3, _now);
            Assert.IsTrue(Math.Abs(sumdata.AverageWeight.Last() - 50.2) < 0.1);
        }

        [TestMethod]
        public void ShouldGetCorrectDailyInfo()
        {
            var _now = new DateTime(2013, 3, 13);
            const int userId = 50;
            var service = ObjectFactory.GetInstance<IDailyInfoService>();

            TestHelper.DropDailyInfoCollections();

            var info = new Model.DailyInfo.DailyInfo
                {
                    UserId = userId,
                    Hotel = true,
                    Weight = 50,
                };

            service.InsertDailyInfo(userId, info, _now.AddDays(-6));
            ServiceDailyInfo dailyInfo = service.GetDayInfoViewService(userId, Constants.NUMBER_OF_DAY_RECORDS, _now);

            Assert.IsTrue(Math.Abs(dailyInfo.Weight[6] - 50) < 0.001);
            Assert.IsTrue(dailyInfo.Hotel[6]);
            Assert.IsTrue(dailyInfo.Sick[6] == false);
            Assert.IsTrue(dailyInfo.Travel[6] == false);
            Assert.IsTrue(dailyInfo.Rest[6] == false);
        }

        [TestMethod]
        public void ShouldContainTrainingDataAverages()
        {
            DateTime now = DateTime.Now;
            int userId = 75;
            var service = ObjectFactory.GetInstance<IDailyInfoService>();

            TestHelper.DropDailyInfoCollections();

            TestHelper.SeedTrainingData(new List<int> {75}, now, 1, 1);

            var info = new Model.DailyInfo.DailyInfo
                {
                    UserId = userId,
                    Hotel = true,
                    Weight = 50
                };


            service.InsertDailyInfo(userId, info, now.AddDays(-7));
            ServiceDailyInfo dailyInfo = service.GetDayInfoViewService(userId, Constants.NUMBER_OF_DAY_RECORDS,
                                                                       now.AddDays(-1));

            Assert.IsTrue(Math.Abs(dailyInfo.Weight[6] - 50) < 0.001);
            Assert.IsTrue(dailyInfo.Hotel[6]);
            Assert.IsTrue(dailyInfo.Sick[6] == false);
            Assert.IsTrue(dailyInfo.Travel[6] == false);
            Assert.IsTrue(dailyInfo.Rest[6] == false);


            Assert.IsTrue(Math.Abs(dailyInfo.TrainingDataPerActivity.ActivitiesDic[Types.TrainingType.Endurance][0].Value - 0.25) < 0.001);
        }
         
        [TestMethod]
        public void ShouldUpdateDailyInfo()
        {
            DateTime _now = DateTime.Now;
            int userId = 75;
            var service = ObjectFactory.GetInstance<IDailyInfoService>();

            TestHelper.DropDailyInfoCollections();
            var info = new Model.DailyInfo.DailyInfo
                {
                    UserId = userId,
                    Hotel = true,
                    Weight = 50,
                };

            service.InsertDailyInfo(userId, info);


            info.Travel = true;
            service.InsertDailyInfo(userId, info);

            GenericError error;

            ServiceDailyInfo dayDailyInfo = service.GetDayInfoViewService(userId, Constants.NUMBER_OF_DAY_RECORDS, _now);

            Assert.IsTrue(dayDailyInfo.Weight.First() == 50);
            Assert.IsTrue(dayDailyInfo.Hotel.First());
            Assert.IsTrue(!dayDailyInfo.Sick.First());
            Assert.IsTrue(dayDailyInfo.Travel.First());
            Assert.IsTrue(!dayDailyInfo.Rest.First());
        }
         
        [TestMethod]
        public void ShouldGetMonthlyInfoSummaryData()
        {
            var _now = new DateTime(2013, 4, 23);
            int userId = 75;
            var service = ObjectFactory.GetInstance<IDailyInfoService>();

            TestHelper.DropDailyInfoCollections();
            var info = new Model.DailyInfo.DailyInfo
                {
                    UserId = userId,
                    Hotel = true,
                    Weight = 50,
                };

            service.InsertDailyInfo(userId, info, _now);
            ServiceDailyInfo data = service.GetDayInfoViewService(userId, 2, _now);


            Assert.IsTrue(data.Hotel[0]);
            Assert.IsTrue(data.Rest[0] == false);
            Assert.IsTrue(data.Sick[0] == false);

            //Add Second Info
            var info2 = new Model.DailyInfo.DailyInfo
                {
                    UserId = userId,
                    Hotel = true,
                    Weight = 100,
                };

            service.InsertDailyInfo(userId, info2, _now.AddDays(-1));

            List<DailyInfoSummaryData> dataMonth = service.GetDailyInfoViewData(userId, 3, _now);
            Assert.IsTrue(dataMonth[0].Hotel == 2);
            Assert.IsTrue(Math.Abs(dataMonth[0].AverageWeight - 75) < 0.01);
        }

        #endregion

        [TestInitialize]
        public void DailyInfoTestsTestInitialize()
        {
            IocConfigurator.Configure();
            ObjectFactory.GetInstance<ITrainingDataService>();
        }
    }
}