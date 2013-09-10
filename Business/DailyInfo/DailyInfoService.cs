using System;
using System.Collections.Generic;
using System.Linq;
using Business.DailyInfo.Helpers;
using Business.Helpers;
using Business.Interfaces;
using Business.TrainningData.Helpers;
using Common;
using DataAccess.Interfaces;
using ErrorClasses;
using Logger;
using Model;
using Model.DailyInfo;
 using ServiceModel.DailyInfo;
using StructureMap;
using Controller = ServerDateTimeProvider.Controller;

//using ServiceModel.DailyInfo;

namespace Business.DailyInfo
{
    internal class DailyInfoService : IDailyInfoService
    {
        private readonly ILogger _log = ObjectFactory.GetInstance<ILogger>();
        public ITrainingDataService TrainingDataService { get; set; }
         
        #region NoSql Data Acces Repositories


        /// <summary>
        ///     Handles Monthly Questionnaire Summary Data
        /// </summary>
        public IDataAgreggationHelperTemplate<DailyInfoSummaryData, Model.DailyInfo.DailyInfo, DailyInfoSummaryData>
            MontlyDailyInfoHandlingTemplate { get; set; }

        /// <summary>
        ///     Handles Yearly Questionnaire Summary Data
        /// </summary>
        public IDataAgreggationHelperTemplate<DailyInfoSummaryData, Model.DailyInfo.DailyInfo, DailyInfoSummaryData>
            YearlyInfoHandlingTemplate { get; set; }
         
         
        public IDailyInfoRepository DailyInfoSummaryDataRepository { get; set; }

        public IRepositoryBase<DailyInfoSummaryData> MonthlyInfoSummaryDataRepository { get; set; }

        public INoSqlFinalValuesRepositoryBase<DailyInfoSummaryData> FinalMonthlyInfoSummaryDataRepository { get; set; }

        public IRepositoryBase<DailyInfoSummaryData> YearlyInfoSummaryDataRepository { get; set; }

        public INoSqlFinalValuesRepositoryBase<DailyInfoSummaryData> FinalYearlyDailyInfoSummaryDataRepository { get; set; }

        #endregion NoSql Data Acces Repositories
 

        public List<DailyInfoSummaryData> GetDailyInfoViewData(int userId, int period, DateTime now)
        {
            if (period == 2)
                return GetWeekInfoView(userId, now);
            if (period == 3)
                return GetMonthlyInfoView(userId, now);
            if (period == 4)
                return GetYearlyInfoView(userId, now);
            throw new Exception("Time Period is out of Range");
        }

        public void InsertDailyInfo(int userId, Model.DailyInfo.DailyInfo info, DateTime? now = null)
        {
            _log.LogInfo("InsertDailyInfo : " + info);
            try
            {
                info.Date = now ?? Controller.GetServerDateTime();
                info.UserId = userId;

                //async
                GenericError error;
                var alreadyExisted = UpdateOrInsert(out error, userId, info, info.Date);

                if (error == null)
                {
                    if (alreadyExisted)
                    {
                       // UpdateMonthlyInfoRecord(userId, info, now);
                       //  UpdateYearlyInfoRecord(userId, info, now);
                    }
                    else
                    {
                    //    InsertMonthlyInfoRecord(info);
                    //   InsertYearlyInfoRecord(info);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                throw;
            }
        }

        //private void UpdateYearlyInfoRecord(int userId, Model.DailyInfo.DailyInfo info, DateTime? now)
        //{
        //    GenericError error;
        //    if (now == null) now = Controller.GetServerDateTime();
        //    List<DailyInfoSummaryData> todays =YearlyInfoSummaryDataRepository.GetEntities(out error,
        //                                                          t => t.UserId == userId &&
        //                                                          t.Date.Day == now.Value.Day &&
        //                                                          t.Date.Month == now.Value.Month &&
        //                                                          t.Date.Year == now.Value.Year);
        //    var last = todays.Count > 1 ? todays[2] : RecomputeRecord(out error, userId, t => t.Date.Year == now.Value.Year);
        //    var current = todays.Count == 0 ? new DailyInfoSummaryData() : todays[0];

        //    var monthlyInfoRepository = YearlyInfoSummaryDataRepository as YearlyInfoRepository;
        //    YearlyInfoHandlingTemplate.ComputeFromLastRecord(info, current, last);

        //    if (monthlyInfoRepository != null) monthlyInfoRepository.Update(current);
        //}

        //private void UpdateMonthlyInfoRecord(int userId, Model.DailyInfo.DailyInfo info, DateTime? now)
        //{
        //    GenericError error;
        //    if (now == null) now = Controller.GetServerDateTime();
        //    List<DailyInfoSummaryData> todays =
        //        MonthlyInfoSummaryDataRepository.GetEntities(out error,
        //                                                          t => t.UserId == userId &&
        //                                                          t.Date.Day == now.Value.Day &&
        //                                                          t.Date.Month == now.Value.Month &&
        //                                                          t.Date.Year == now.Value.Year);

        //    var last = todays.Count > 1 ? todays[2] : RecomputeRecord(out error, userId, t => t.Date.Month == now.Value.Month && t.Date.Day.CompareTo(now.Value.Day - 1) <= 0);
        //    var current = todays.Count == 0 ? new DailyInfoSummaryData() : todays[0];

        //    var monthlyInfoRepository = MonthlyInfoSummaryDataRepository as MonthlyInfoRepository;
        //    MontlyDailyInfoHandlingTemplate.ComputeFromLastRecord(info, current, last);

        //    if (monthlyInfoRepository != null) monthlyInfoRepository.Update(current);
        //}

        //private DailyInfoSummaryData RecomputeRecord(out GenericError error, long userId, Func<Model.DailyInfo.DailyInfo, bool> timeComparison)
        //{
        //    var sumInfo = new DailyInfoSummaryData();
        //    var allData = DailyInfoSummaryDataRepository.GetEntities(out error,
        //                                                    t => t.UserId == userId && timeComparison(t));

        //    if (allData.Count == 0) return new DailyInfoSummaryData();
        //    allData.ForEach(info =>
        //     {

        //         sumInfo.WeightSumTotal = sumInfo.WeightSumTotal + info.Weight;
        //         sumInfo.Count = sumInfo.Count + 1;
        //         sumInfo.WeightSumTotal = sumInfo.WeightSumTotal + info.Weight;

        //         sumInfo.AverageWeight = sumInfo.Count == 0 ? 0 : Math.Round(sumInfo.WeightSumTotal / sumInfo.Count, Constants.DAILY_INFO_WEIGHT_DIGITS_PERSIST);

        //         sumInfo.Hotel = sumInfo.Hotel + (info.Hotel ? 1 : 0);
        //         sumInfo.Rest = sumInfo.Rest + (info.Rest ? 1 : 0);
        //         sumInfo.Sick = sumInfo.Sick + (info.Sick ? 1 : 0);
        //         sumInfo.Travel = sumInfo.Travel + (info.Travel ? 1 : 0);

        //     });
        //    return sumInfo;
        //}

        public bool UpdateOrInsert(out GenericError error, int userId, Model.DailyInfo.DailyInfo todayDailyInfo,
                                   DateTime dateTime)
        {
            error = null;

            todayDailyInfo.Weight = Math.Round(todayDailyInfo.Weight, Constants.DAILY_INFO_WEIGHT_DIGITS_PERSIST);

            var dailyInfoRepository = DailyInfoSummaryDataRepository;
            if (dailyInfoRepository != null)
                return dailyInfoRepository.UpdateOrInsert(todayDailyInfo);
            throw new Exception("DailyInfo Repository does not support Update data");
        }

        public Model.DailyInfo.DailyInfo GetTodaysData(int userId, DateTime getServerDateTime)
        {
            GenericError error;
            DateTime now = Controller.GetServerDateTime();
            List<Model.DailyInfo.DailyInfo> todays =
                DailyInfoSummaryDataRepository.GetEntities(out error,
                                                           t => t.UserId == userId &&
                                                                t.Date.Day == now.Day &&
                                                                t.Date.Month == now.Month &&
                                                                t.Date.Year == now.Year);
            return todays.Count == 0 ? new Model.DailyInfo.DailyInfo() : todays.Last();
        }

        public static IList<Model.DailyInfo.DailyInfo> TranctuateDigits(IList<Model.DailyInfo.DailyInfo> data)
        {
            data.ToList().ForEach(d => d.Weight = Math.Round(d.Weight, Constants.DAILY_INFO_WEIGHT_DIGITS_DISPLAY));
            return data.ToList();
        }

        public static IList<DailyInfoSummaryData> TranctuateDigits(IList<DailyInfoSummaryData> data)
        {
            data.ToList().ForEach(d => d.AverageWeight = Math.Round(d.AverageWeight, Constants.DAILY_INFO_WEIGHT_DIGITS_DISPLAY));
            return data.ToList();
        }


        private List<DailyInfoSummaryData> GetYearlyInfoView(int userId, DateTime now)
        {
            //var results = YearlyInfoHandlingTemplate.GetInfoView(userId, now, Constants.NUMBER_OF_YEAR_RECORDS);

            DataHelper<DailyInfoSummaryData, Model.DailyInfo.DailyInfo> helper = new DailyInfoYearlyDataHelperDynamic { Repository = DailyInfoSummaryDataRepository };
            var results = helper.GetViewDataForTimePeriod(userId, now, Constants.NUMBER_OF_YEAR_RECORDS);
            if (results != null) return results.ToList();
            return new List<DailyInfoSummaryData>();
        }

        private List<DailyInfoSummaryData> GetMonthlyInfoView(int userId, DateTime now)
        {
            DataHelper<DailyInfoSummaryData, Model.DailyInfo.DailyInfo> helper = new DailyInfoMonthlyDataHelperDynamic { Repository = DailyInfoSummaryDataRepository };
            var results = helper.GetViewDataForTimePeriod(userId, now, Constants.NUMBER_OF_MONTH_RECORDS);
            //var results = MontlyDailyInfoHandlingTemplate.GetInfoView(userId, now, Constants.NUMBER_OF_MONTH_RECORDS);
            if (results != null) return results.ToList();
            return new List<DailyInfoSummaryData>();
        }

        protected IQueryable<T> GetFilteredDailyInfo<T>(int userId, IRepositoryBase<T> repository)
            where T : class, IUserQueryable
        {
            IEnumerable<T> lattestDailyInfos = repository.GetAsQueryable<T>().ToList().Where(q => q.UserId == userId);
            return lattestDailyInfos.AsQueryable();
        }

        private void InsertYearlyInfoRecord(Model.DailyInfo.DailyInfo info)
        {
            YearlyInfoHandlingTemplate.UpdateEntityRecords(info);
        }

        private void InsertMonthlyInfoRecord(Model.DailyInfo.DailyInfo info)
        {
            MontlyDailyInfoHandlingTemplate.UpdateEntityRecords(info);
        }

        #region Weekly

        private List<DailyInfoSummaryData> GetWeekInfoView(int userId, DateTime now)
        {
            var helper = new DailyInfoWeeklyDataHelper { Repository = DailyInfoSummaryDataRepository };
            return helper.GetViewDataForTimePeriod(userId, now, Constants.NUMBER_OF_WEEK_RECORDS).ToList();
        }
         
        #endregion Weekly

   
        #region Helpers

        private ServiceDailyInfo FillTrainingDetailsDayData(int userId, DateTime now, ServiceDailyInfo dailyInfo)
        {
            var periodTrainingDataStrategy = new DailyTrainingDataSummary();
            var numericCalculationTypeStrategy = new SumTrainigDataSummaryStrategy();

            Dictionary<Types.TrainingType, List<TrainingDataTainingDetailsModel.TrainingDataValue>> activityInfo = TrainingDataService.GetActivityInfoSummary(
                userId, now, periodTrainingDataStrategy.NumberOfRecords, periodTrainingDataStrategy, numericCalculationTypeStrategy,
                new List<Types.TrainingType>
                    {
                        Types.TrainingType.Endurance,
                        Types.TrainingType.Recovery,
                        Types.TrainingType.Speed,
                        Types.TrainingType.Competition,
                        Types.TrainingType.Sport,
                        Types.TrainingType.Strength
                    });



            dailyInfo.TrainingDataPerActivity = new TrainingDataPerActivity(activityInfo);

            return dailyInfo;
        }

      
        public ServiceDailyInfoSummaryData GetDailyInfoServiceViewData(int userId, int period, DateTime now)
        {
            var data = new ServiceDailyInfoSummaryData();

            if (period == 1)
                data = GetWeekInfoViewService(userId, now, Constants.NUMBER_OF_WEEK_RECORDS);
            if (period == 2)
                data = GetMonthlyInfoViewService(userId, now, Constants.NUMBER_OF_MONTH_RECORDS);
            if (period == 3)
                data = GetYearlyInfoViewService(userId, now, Constants.NUMBER_OF_YEAR_RECORDS);

            return ReverseSeries(data);
        }

        public ServiceDailyInfo GetDayInfoViewService(int userId, int numberOfDailyInfo, DateTime now)
        {
            var helper = new DailyInfoDailyDataHelper { Repository = DailyInfoSummaryDataRepository };
            IList<Model.DailyInfo.DailyInfo> data = helper.GetViewDataForTimePeriod(userId, now, numberOfDailyInfo);
            ServiceDailyInfo adapted = AdaptDayData(data.ToList());
            return FillTrainingDetailsDayData(userId, now, adapted);
        }

   

        private ServiceDailyInfo AdaptDayData(List<Model.DailyInfo.DailyInfo> lattestDailyInfos)
        {
            var serviceAdaptedData = new ServiceDailyInfo();
            foreach (Model.DailyInfo.DailyInfo info in lattestDailyInfos)
            {
                serviceAdaptedData.Date = info.Date;
                serviceAdaptedData.Rest.Add(info.Rest);
                serviceAdaptedData.Travel.Add(info.Travel);
                serviceAdaptedData.Hotel.Add(info.Hotel);
                serviceAdaptedData.Sick.Add(info.Sick);
                serviceAdaptedData.Weight.Add(info.Weight);
            }
            serviceAdaptedData.TodaysDailyInfo =
                lattestDailyInfos.FirstOrDefault(
                    d => d.Date.Day == Controller.GetServerDateTime().Day);
            return serviceAdaptedData;
        }
         
        private ServiceDailyInfoSummaryData GetWeekInfoViewService(int userId, DateTime now, int requestedRecords)
        {
            List<DailyInfoSummaryData> lattestDailyInfos = GetWeekInfoView(userId, now);

            var serviceAdaptedDataList = new List<ServiceDailyInfoSummaryData>();
            FIllEmptyRecordsServieDailyInfoSummaryData(serviceAdaptedDataList, requestedRecords);

            return FillTrainingDetailsDataSums(userId, now, AdaptData(lattestDailyInfos), DataUtils.GetPeriodTrainingDataStrategy(userId, Constants.WEEK_PERIOD));
        }

        private ServiceDailyInfoSummaryData ReverseSeries(ServiceDailyInfoSummaryData fillTrainingDetailsDataSums)
        {
            if (fillTrainingDetailsDataSums.TrainingData != null)
                fillTrainingDetailsDataSums.TrainingData.TrainningDataSummaries.Reverse();
            fillTrainingDetailsDataSums.Hotel.Reverse();
            fillTrainingDetailsDataSums.Rest.Reverse();
            fillTrainingDetailsDataSums.Travel.Reverse();
            fillTrainingDetailsDataSums.AverageWeight.Reverse();
            fillTrainingDetailsDataSums.Sick.Reverse();

            return fillTrainingDetailsDataSums;
        }

        /// <summary>
        ///     calculates the sum of activities
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="now"></param>
        /// <param name="serviceDailyInfoSummaryData"></param>
        /// <param name="periodStrategy"></param>
        /// <returns></returns>
        private ServiceDailyInfoSummaryData FillTrainingDetailsDataSums(int userId, DateTime now,
                                                                        ServiceDailyInfoSummaryData
                                                                            serviceDailyInfoSummaryData,
                                                                        PeriodTrainingDataStrategy periodStrategy)
        {
            serviceDailyInfoSummaryData.TrainingData = new TrainingData
                {
                    TrainningDataSummaries = TrainingDataService.GetActivityInfoSums(userId, now, periodStrategy,
                                                                                     new List<Types.TrainingType>
                                                                                         {
                                                                                             Types.TrainingType.Endurance,
                                                                                             Types.TrainingType.Strength,   
                                                                                             Types.TrainingType.Speed,                                                                               
                                                                                             Types.TrainingType.Sport,
                                                                                             Types.TrainingType.Competition,
                                                                                             Types.TrainingType.Recovery,
                                                                                             
                                                                                         })
                };


            return serviceDailyInfoSummaryData;
        }

        private ServiceDailyInfoSummaryData GetYearlyInfoViewService(int userId, DateTime now, int requestedRecords)
        {
            List<DailyInfoSummaryData> lattestDailyInfos = GetYearlyInfoView(userId, now);

            var serviceAdaptedDataList = new List<ServiceDailyInfoSummaryData>();
            FIllEmptyRecordsServieDailyInfoSummaryData(serviceAdaptedDataList, requestedRecords);
            return FillTrainingDetailsDataSums(userId, now, AdaptData(lattestDailyInfos), new YearlyTrainigDataSummary());
        }

        private void FIllEmptyRecordsServieDailyInfoSummaryData(List<ServiceDailyInfoSummaryData> intialList,
                                                                int requestedRecords)
        {
            int initialCount = intialList.Count();

            if (initialCount > requestedRecords)
                return;

            Enumerable.Range(initialCount, (requestedRecords - initialCount))
                      .ToList()
                      .ForEach(d => intialList.Add(new ServiceDailyInfoSummaryData()));
        }

        private ServiceDailyInfoSummaryData AdaptData(IEnumerable<DailyInfoSummaryData> lattestDailyInfos)
        {
            var serviceAdaptedData = new ServiceDailyInfoSummaryData();
            foreach (DailyInfoSummaryData info in lattestDailyInfos)
            {
                serviceAdaptedData.Date = info.Date;
                serviceAdaptedData.Rest.Add(info.Rest);
                serviceAdaptedData.Travel.Add(info.Travel);
                serviceAdaptedData.Hotel.Add(info.Hotel);
                serviceAdaptedData.Sick.Add(info.Sick);
                serviceAdaptedData.AverageWeight.Add(info.AverageWeight);
            }

            return serviceAdaptedData;
        }

        private ServiceDailyInfoSummaryData GetMonthlyInfoViewService(int userId, DateTime now, int requestedRecords)
        {
            //GenericError error = null;

            List<DailyInfoSummaryData> lattestDailyInfos = GetMonthlyInfoView(userId, now);

            var serviceAdaptedDataList = new List<ServiceDailyInfoSummaryData>();
            FIllEmptyRecordsServieDailyInfoSummaryData(serviceAdaptedDataList, requestedRecords);
            return FillTrainingDetailsDataSums(userId, now, AdaptData(lattestDailyInfos),
                                               new MonthlyTrainigDataSummary());
        }

        #endregion Web Service Data Adaptation
    }
}