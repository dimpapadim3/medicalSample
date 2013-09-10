using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using Business.Interfaces;
using Business.TrainningData;
using Business.TrainningData.Helpers;
using ErrorClasses;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Model.DailyInfo;
using Model.View;
using NoSqlDataAccess.Entities.DailyInfo;
using ServiceModel.DailyInfo;
using StructureMap;

namespace Business.Tests
{
    [TestClass]
    public class TrainingDataTests
    {
        public float Epsilon = (float)0.000001;
        private readonly DateTime _now = new DateTime(2013, 2, 13);

        private ITrainingDataService _trainingDataService;

        [TestInitialize]
        public void TestInitialize()
        {
            IocConfigurator.Configure();


            _trainingDataService = ObjectFactory.GetInstance<ITrainingDataService>();
        }



        [TestMethod]
        public void ShouldIncludeActivitiesThatAddUpMoreThanAnHourInYear()
        {
          //  GenericError error;
          //  TestHelper.DropTrainningCollections();
          ////  TestHelper.CreateEffortTrainingData();
          //  var data =
          //       _trainingDataService.GetActivityInfoSummary(75, _now, 5, new YearlyTrainigDataSummary(),
          //       new AverageyTrainigDataSummaryStrategy(), new List<Types.TrainingType>() { Types.TrainingType.Endurance });

          //  Assert.IsTrue(Math.Abs(data[Types.TrainingType.Endurance][0].Value) > 0);

        }

        [TestMethod]
        public void ShouldNotIncludeActivitiesLessThanAnHour()
        {
            //GenericError error;

            //var data = _trainingDataService.GetActivityInfoSummary(75, _now, 5, new DailyTrainingDataSummary(), new AverageyTrainigDataSummaryStrategy(),
            //                                                new List<Types.TrainingType>() { Types.TrainingType.Endurance });
            //Assert.IsTrue(Math.Abs(data[Types.TrainingType.Endurance][0].Value) == 0);

        }

        [TestMethod]
        public void ShouldIncludeActivitiesMoreThanAnHour()
        {
  
        }

        //[TestMethod]
        //public void TrainningDataTest()
        //{
        //    GenericError error;



        //    TrainingDataRepository.Drop();

        //    CreateTrainingData()
        //        .ToList()
        //        .ForEach(d => TrainingDataRepository.InsertEntity(out error, d));

        //    var PeriodTrainingDataStrategy = new DailyTrainingDataSummary();
        //    NumericCalculationTypeStrategy NumericCalculationTypeStrategy = new AverageyTrainigDataSummary();
        //    IList<TrainingDataTainingDetailsModel.TrainingDataValue> data = _trainingDataService.GetMeasurementInfoSummary(39, _now.AddMonths(1), PeriodTrainingDataStrategy, NumericCalculationTypeStrategy, 5,
        //                                                                      m => (decimal)m.BreathRate);
        //    Assert.IsTrue(Math.Abs(data[1].Value - 75) < Epsilon);

        //    NumericCalculationTypeStrategy = new MaxTrainigDataSummary();
        //    data = _trainingDataService.GetMeasurementInfoSummary(39, _now.AddMonths(1), PeriodTrainingDataStrategy, NumericCalculationTypeStrategy, 5, m => (decimal)m.BreathRate);
        //    Assert.IsTrue(Math.Abs(data[1].Value - 100) < Epsilon);


        //    NumericCalculationTypeStrategy = new MinTrainigDataSummary();
        //    data = _trainingDataService.GetMeasurementInfoSummary(39, _now.AddMonths(1), PeriodTrainingDataStrategy, NumericCalculationTypeStrategy, 5, m => (decimal)m.BreathRate);
        //    Assert.IsTrue(Math.Abs(data[1].Value - 50) < Epsilon);

        //    NumericCalculationTypeStrategy = new SumTrainigDataSummary();
        //    data = TrainingDataManager.GetMeasurementInfoSummary(39, now.AddMonths(1), 5, m => (decimal)m.BreathRate);
        //    Assert.IsTrue(data[1] == 150);
        //}

        //[TestMethod]
        //public void TrainningDataTestYear()
        //{
        //    GenericError error;

        //    var trainningDataRepository = new TrainingDataRepository();
        //    trainningDataRepository.Drop();

        //    CreateTrainingData()
        //        .ToList()
        //        .ForEach(d => TrainingDataRepository.InsertEntity(out error, d));

        //    var PeriodTrainingDataStrategy = new DailyTrainingDataSummary();
        //    NumericCalculationTypeStrategy NumericCalculationTypeStrategy = new AverageyTrainigDataSummary();
        //    IList<TrainingDataTainingDetailsModel.TrainingDataValue> data = _trainingDataService.GetMeasurementInfoSummary(39, _now.AddYears(1), PeriodTrainingDataStrategy, NumericCalculationTypeStrategy, 5,
        //                                                                      m => (decimal)m.BreathRate);
        //    Assert.IsTrue(Math.Abs(data[1].Value - 75) < Epsilon);

        //    NumericCalculationTypeStrategy = new MaxTrainigDataSummary();
        //    data = _trainingDataService.GetMeasurementInfoSummary(39, _now.AddYears(1), PeriodTrainingDataStrategy, NumericCalculationTypeStrategy, 5, m => (decimal)m.BreathRate);
        //    Assert.IsTrue(Math.Abs(data[1].Value - 100) < Epsilon);


        //    NumericCalculationTypeStrategy = new MinTrainigDataSummary();
        //    data = _trainingDataService.GetMeasurementInfoSummary(39, _now.AddYears(1), PeriodTrainingDataStrategy, NumericCalculationTypeStrategy, 5, m => (decimal)m.BreathRate);
        //    Assert.IsTrue(Math.Abs(data[1].Value - 50) < Epsilon);

        //    NumericCalculationTypeStrategy = new SumTrainigDataSummary();
        //    data = TrainingDataManager.GetMeasurementInfoSummary(39, now.AddMonths(1), 5, m => (decimal)m.BreathRate);
        //    Assert.IsTrue(data[1] == 150);
        //}

        //[TestMethod]
        //public void TrainningDataTestWeek()
        //{
        //    var now = new DateTime(2013, 2, 13);


        //    TrainingDataRepository.Drop();

        //    var data = new List<TrainingDataModel>();

        //    data.Add(new TrainingDataModel
        //        {
        //            UserId = 39,
        //            Sessions = new List<TrainingSession>
        //                {
        //                    On Monday
        //                    new TrainingSession
        //                        {
        //                            From = now.AddDays(-2),
        //                            To = now.AddDays(-2).AddHours(3),
        //                            ActivityTypeId = Types.TrainingType.Sport,
        //                            EffortTypeId = Types.EffortType.MaxPush,
        //                            Measurements =
        //                                new List<MeasurementInfo>
        //                                    {
        //                                        new MeasurementInfo {BodyTemperature = 36, BreathRate = 100}
        //                                    }
        //                        },
        //                    new TrainingSession
        //                        {
        //                            From = now,
        //                            To = now.AddHours(3),
        //                            ActivityTypeId = Types.TrainingType.Sport,
        //                            EffortTypeId = Types.EffortType.Hard,
        //                            Measurements =
        //                                new List<MeasurementInfo>
        //                                    {
        //                                        new MeasurementInfo {BodyTemperature = 36, BreathRate = 50}
        //                                    }
        //                        }
        //                }
        //        });

        //    var PeriodTrainingDataStrategy = new WeeklyTrainigDataSummary { FirstDayOfTheWeek = 1 };
        //    NumericCalculationTypeStrategy NumericCalculationTypeStrategy = new AverageyTrainigDataSummary();
        //    IList<TrainingDataTainingDetailsModel.TrainingDataValue> resultData = _trainingDataService.GetMeasurementInfoSummary(39, now, PeriodTrainingDataStrategy, NumericCalculationTypeStrategy, 5,
        //                                                                            m => (decimal)m.BreathRate);
        //     Assert.IsTrue(resultData[1]  == 75);

        //    NumericCalculationTypeStrategy = new MaxTrainigDataSummary();
        //    resultData = _trainingDataService.GetMeasurementInfoSummary(39, now.AddYears(1), PeriodTrainingDataStrategy, NumericCalculationTypeStrategy, 5,
        //                                                               m => (decimal)m.BreathRate);
        //     Assert.IsTrue(resultData[1] == 100);

        //    NumericCalculationTypeStrategy = new MinTrainigDataSummary();
        //    resultData = _trainingDataService.GetMeasurementInfoSummary(39, now.AddYears(1), PeriodTrainingDataStrategy, NumericCalculationTypeStrategy, 5,
        //                                                               m => (decimal)m.BreathRate);
        //     Assert.IsTrue(resultData[1] == 50);

        //    NumericCalculationTypeStrategy = new SumTrainigDataSummary();
        //    data = TrainingDataManager.GetMeasurementInfoSummary(39, now.AddMonths(1), 5, m => (decimal)m.BreathRate);
        //    Assert.IsTrue(data[1] == 150);
        //}

        //[TestMethod]
        //public void EfforTrainningDataTestMonth()
        //{
        //    GenericError error;

        //    TrainingDataRepository.Drop();

        //    CreateEffortTrainingData()
        //        .ToList()
        //        .ForEach(d => TrainingDataRepository.InsertEntity(out error, d));

        //    var PeriodTrainingDataStrategy = new MonthlyTrainigDataSummary();

        //    NumericCalculationTypeStrategy NumericCalculationTypeStrategy = new AverageyTrainigDataSummary();

        //    List<TrainingDataTainingDetailsModel.TrainingDataValue> data = _trainingDataService.GetEffortSummary(39, _now, PeriodTrainingDataStrategy, NumericCalculationTypeStrategy, 5);
        //    Assert.IsTrue(Math.Abs(data[0].Value - 1.75) < 0.00001);
        //}

        //[TestMethod]
        //public void EfforTrainningDataTestMonth_SecondValue()
        //{
        //    GenericError error;


        //    TrainingDataRepository.Drop();


        //    var data = new List<TrainingDataModel>();

        //    data.Add(new TrainingDataModel
        //        {
        //            UserId = 39,
        //            Sessions = new List<TrainingSession>
        //                {
        //                    new TrainingSession
        //                        {
        //                            From = _now.AddDays(-1),
        //                            To = _now.AddDays(-1).AddHours(3),
        //                            ActivityTypeId = Types.TrainingType.Sport,
        //                            EffortTypeId = Types.EffortType.MaxPush,
        //                            Measurements =
        //                                new List<MeasurementInfo>
        //                                    {
        //                                        new MeasurementInfo {BodyTemperature = 36, BreathRate = 100}
        //                                    }
        //                        },
        //                    new TrainingSession
        //                        {
        //                            From = _now,
        //                            To = _now.AddHours(3),
        //                            ActivityTypeId = Types.TrainingType.Sport,
        //                            EffortTypeId = Types.EffortType.Hard,
        //                            Measurements =
        //                                new List<MeasurementInfo>
        //                                    {
        //                                        new MeasurementInfo {BodyTemperature = 36, BreathRate = 50}
        //                                    }
        //                        }
        //                }
        //        });

        //    data.ToList().ForEach(d => TrainingDataRepository.InsertEntity(out error, d));

        //    var PeriodTrainingDataStrategy = new MonthlyTrainigDataSummary();
        //    var NumericCalculationTypeStrategy = new AverageyTrainigDataSummary();

        //    Dictionary<Types.TrainingType, List<TrainingDataTainingDetailsModel.TrainingDataValue>> resultData = _trainingDataService.GetActivityInfoSummary(39,
        //                                                                                                        _now
        //                                                                                                            .AddYears
        //                                                                                                            (1),
        //                                                                                                        5, PeriodTrainingDataStrategy, NumericCalculationTypeStrategy,
        //                                                                                                        new List
        //                                                                                                            <
        //                                                                                                            Types
        //                                                                                                            .
        //                                                                                                            TrainingType
        //                                                                                                            >
        //                                                                                                            {
        //                                                                                                                Types
        //                                                                                                            .TrainingType
        //                                                                                                            .Recovery,
        //                                                                                                                Types
        //                                                                                                            .TrainingType
        //                                                                                                            .Sport
        //                                                                                                            });
        //    Assert.IsTrue(Math.Abs(resultData[Types.TrainingType.Sport][0].Value - 1.25) < 0.00001);
        //}

        //[TestMethod]
        //public void TestDailyInfoFavouriteActivitiesUpdate()
        //{
        //    string locale = "en-US";

        //    var userId = 50;
        //    var now = new DateTime(2012, 2, 22);

        //    Add first Info the last month

        //    var currentFavourites = _trainingDataService.GetPopularActivities(userId, locale, 5);
        //    Assert.IsTrue(currentFavourites.Count == 5);


        //    TrainingDataModel trainingData = new TrainingDataModel() { Sessions = new List<TrainingSession> { new TrainingSession() { Sport = "Fitness" } } };

        //    _trainingDataService.SubmitTrainingData(userId, trainingData, now);


        //    currentFavourites = _trainingDataService.GetPopularActivities(userId, locale, 5);
        //    activity id=2 is now the most popular 
        //    Assert.IsTrue(currentFavourites[0].Id == 2);



        //}

        //#region Activities
        //[TestMethod]
        //public void EfforTrainningDataTestMonth_Activities()
        //{
        //    GenericError error;

        //    int userId = 39;

        //    TrainingDataRepository.Drop();

        //    CreateActivitiesTrainingData()
        //        .ToList()
        //        .ForEach(d => TrainingDataRepository.InsertEntity(out error, d));

        //    TrainingDataManager.PeriodTrainingDataStrategy = new MonthlyTrainigDataSummary();
        //    TrainingDataManager.NumericCalculationTypeStrategy = new AverageyTrainigDataSummary();

        //    ServiceModel.DailyInfo.TrainingDataSettings settings = new TrainingDataSettings()
        //        {
        //            MeasurmentType = MeasurementInfoTypes.BreathRate,
        //            MesurmentInfoCalculationType = 1,

        //            SelectedActivitiesTypes = new List<int> { 1, 2, 3, 4, 5 }
        //        };
        //    var trainingData = _trainingDataService.GetTrainingDataForPeriod(userId, 3, _now, settings);


        //    Assert.IsTrue(Math.Abs(trainingData.Activities[0][3].Value - 1.25) < 0.00001);
        //    Assert.IsTrue(Math.Abs(trainingData.Activities[2][4].Value - 1.25) < 0.00001);
        //}

        //[TestMethod]
        //public void EfforTrainningDataTestYear_Activities()
        //{
        //    GenericError error;

        //    int userId = 39;

        //    TrainingDataRepository.Drop();

        //    CreateActivitiesTrainingData()
        //        .ToList()
        //        .ForEach(d => TrainingDataRepository.InsertEntity(out error, d));


        //    ServiceModel.DailyInfo.TrainingDataSettings settings = new TrainingDataSettings()
        //    {
        //        MeasurmentType = Types.MeasurementInfoTypes.BreathRate,
        //        MesurmentInfoCalculationType = 1,

        //        SelectedActivitiesTypes = new List<int> { 1, 2, 3, 4, 5 }
        //    };
        //    var trainingData = _trainingDataService.GetTrainingDataForPeriod(userId, 4, _now, settings);


        //    Assert.IsTrue(Math.Abs(trainingData.Activities[0][3].Value - 1.25) < 0.00001);

        //}


        //#endregion

    }
}