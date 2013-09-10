using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using Business.Interfaces;
using Business.TrainningData.Helpers;
using DataAccess.Interfaces;
using ErrorClasses;
using Model.DailyInfo;
using Model.View;
using ServiceModel.DailyInfo;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.TrainningData
{
    public class TrainingDataService : ITrainingDataService
    {


        #region Properties

        public IRepositoryBase<TrainingSession> TrainingSessionRepository { get; set; }
        public IRepositoryBase<TrainingSessionMeasurmentData> TrainingSessionDataRepository { get; set; }
        public IFavouriteActivityRepository FavouriteActivityRepository { get; set; }

        #endregion

        private readonly Dictionary<int, NumericCalculationTypeStrategy> _calculationStrategies =
            new Dictionary<int, NumericCalculationTypeStrategy>
                {
                    {1, new AverageyTrainigDataSummaryStrategy()},
                    {2, new MinTrainigDataSummaryStrategy()},
                    {3, new MaxTrainigDataSummaryStrategy()},
                    {4, new SumTrainigDataSummaryStrategy()}
                };



        public TrainingDataTainingDetailsModel GetTrainingDataForPeriod(int userId,
                                                                                                      int period,
                                                                                                      DateTime now, TrainingDataSettings
                                                                                                          settings)
        {
            var trainingDataTainingDetailsModel = new TrainingDataTainingDetailsModel();
            Types.MeasurementInfoTypes type;
            Enum.TryParse(settings.MeasurmentType.ToString(), out type);
            var periodTrainingDataStrategy = DataUtils.GetPeriodTrainingDataStrategy(userId, period);
            var numericCalculationTypeStrategy = _calculationStrategies[settings.MesurmentInfoCalculationType];

            var sessions = TrainingSessionRepository.GetAsQueryable<TrainingSession>().ToList()
                                                    .Where(d => d.UserId == userId && periodTrainingDataStrategy.IsInLowerTimeBound(d))
                                                    .ToList();

            var measurements = GetMeasurementInfoSummary(now, periodTrainingDataStrategy,numericCalculationTypeStrategy, periodTrainingDataStrategy.NumberOfRecords,
                                                         DataUtils.MeasurmentPropertySelectors[type], sessions);

            var activities = GetActivityInfoSummary(now, periodTrainingDataStrategy.NumberOfRecords,
                                                    periodTrainingDataStrategy,
                                                    numericCalculationTypeStrategy,
                                                    settings.SelectedActivitiesTypes.Cast<Types.TrainingType>().ToList(), sessions);

            var efforts = GetEffortSummary(now, periodTrainingDataStrategy,
                                           numericCalculationTypeStrategy,
                                           periodTrainingDataStrategy.NumberOfRecords, sessions);

            // NormalizeAllArrayValues(activities, measurements, efforts);
            if (measurements != null) trainingDataTainingDetailsModel.Measurments = measurements.ToList();
            if (activities != null)
            {

                trainingDataTainingDetailsModel.Activities = AddaptActivityDataFillOnlyOne(activities, periodTrainingDataStrategy.NumberOfRecords);
            }
            if (efforts != null) trainingDataTainingDetailsModel.Efort = efforts.ToList();

            trainingDataTainingDetailsModel.Activities.Reverse();
            trainingDataTainingDetailsModel.Efort.Reverse();
            trainingDataTainingDetailsModel.Measurments.Reverse();


            return trainingDataTainingDetailsModel;
        }


        private List<List<TrainingDataTainingDetailsModel.TrainingDataValue>> AddaptActivityDataFillOnlyOne(
            Dictionary<Types.TrainingType, List<TrainingDataTainingDetailsModel.TrainingDataValue>> activityInfo, int numberOfDays)
        {
            var activities = new List<List<TrainingDataTainingDetailsModel.TrainingDataValue>>();
            for (int day = 0; day < numberOfDays; day++)
            {
                activities.Add(GetActivityDataFillOnlyOne(activityInfo, day));
            }
            return activities;
        }

        //TODO:delete
        private List<TrainingDataTainingDetailsModel.TrainingDataValue> GetActivityDataFillOnlyOne(Dictionary<Types.TrainingType, List<TrainingDataTainingDetailsModel.TrainingDataValue>> activityInfo,
                                                              int day)
        {
            var list = new List<TrainingDataTainingDetailsModel.TrainingDataValue>();
            if (activityInfo.ContainsKey(Types.TrainingType.Endurance))
                list.Add(activityInfo[Types.TrainingType.Endurance][day]);

            if (activityInfo.ContainsKey(Types.TrainingType.Strength))
                list.Add(activityInfo[Types.TrainingType.Strength][day]);

            if (activityInfo.ContainsKey(Types.TrainingType.Speed))
                list.Add(activityInfo[Types.TrainingType.Speed][day]);

            if (activityInfo.ContainsKey(Types.TrainingType.Sport))
                list.Add(activityInfo[Types.TrainingType.Sport][day]);

            if (activityInfo.ContainsKey(Types.TrainingType.Competition))
                list.Add(activityInfo[Types.TrainingType.Competition][day]);

            return list;
        }

        #region  Measurment info

        public IList<TrainingDataTainingDetailsModel.TrainingDataValue> GetMeasurementInfoSummary(int userId, DateTime now,
            PeriodTrainingDataStrategy periodTrainingDataStrategy,
            NumericCalculationTypeStrategy numericCalculationTypeStrategy,
            int numberOfRecordsToDisplay, Func<TrainingSessionMeasurmentData,
            IList<DecimalData>> getter)
        {
            var sessions = TrainingSessionRepository.GetAsQueryable<TrainingSession>().ToList()
                                                                 .Where(d => d.UserId == userId && periodTrainingDataStrategy.IsInLowerTimeBound(d))
                                                                 .ToList();

            return GetMeasurementInfoSummary(now, periodTrainingDataStrategy, numericCalculationTypeStrategy, numberOfRecordsToDisplay, getter, sessions);
        }

        private IList<TrainingDataTainingDetailsModel.TrainingDataValue> GetMeasurementInfoSummary(DateTime now, PeriodTrainingDataStrategy periodTrainingDataStrategy,
                                                NumericCalculationTypeStrategy numericCalculationTypeStrategy,
                                                int numberOfRecordsToDisplay, Func<TrainingSessionMeasurmentData, IList<DecimalData>> getter, List<TrainingSession> sessions)
        {
            numericCalculationTypeStrategy.MeasurmentInfoSelector = getter;

            sessions = FillTrainingSessionData(sessions);

            var timeSpans = periodTrainingDataStrategy.GetTimeSpans(now, numberOfRecordsToDisplay);
            IList<TrainingDataTainingDetailsModel.TrainingDataValue> summaries = CollectMeasurmentDataFromSessions(timeSpans,
                                                                                                                   numericCalculationTypeStrategy,
                                                                                                                   sessions
                                                                                                                       .ToList());

            return summaries;
        }

        private List<TrainingSession> FillTrainingSessionData(List<TrainingSession> sessions)
        {

            sessions.ForEach(s =>
            {
                s.MeasurementInfo = TrainingSessionDataRepository.GetAsQueryable<TrainingSessionMeasurmentData>()
                    .FirstOrDefault(d => d.TrainingSessionId == s.TrainingSessionId);
            });

            return sessions;
        }

        private IList<TrainingDataTainingDetailsModel.TrainingDataValue> CollectMeasurmentDataFromSessions(IEnumerable<TimeSpan> timeSpans, NumericCalculationTypeStrategy numericCalculationTypeStrategy,
                                                                      List<TrainingSession> list)
        {
            var values = new List<TrainingDataTainingDetailsModel.TrainingDataValue>();

            foreach (var timespan in timeSpans)
            {
                var allSessionInTimespan = AllSessionInTimespan(list, timespan);

                var count = allSessionInTimespan.Count();

                values.Add(new TrainingDataTainingDetailsModel.TrainingDataValue { Value = count != 0 ? numericCalculationTypeStrategy.CalculateValue(allSessionInTimespan) : 0, Date = timespan.GetSpanDate() });
            }

            return values;
        }

        #endregion

        #region Activity

        public Dictionary<Types.TrainingType, List<TrainingDataTainingDetailsModel.TrainingDataValue>> GetActivityInfoSummary(int userId, DateTime now,
                                                                                         int numberOfRecordsToDisplay,
                                                                                         PeriodTrainingDataStrategy periodTrainingDataStrategy,
                                                                                         NumericCalculationTypeStrategy numericCalculationTypeStrategy,
                                                                                         IList<Types.TrainingType> selectedActivityTypes)
        {
            var sessions = TrainingSessionRepository.GetAsQueryable<TrainingSession>().ToList()
                                                                 .Where(
                                                                     d => d.UserId == userId && periodTrainingDataStrategy.IsInLowerTimeBound(d)).ToList();


            return GetActivityInfoSummary(now, numberOfRecordsToDisplay, periodTrainingDataStrategy, numericCalculationTypeStrategy, selectedActivityTypes, sessions);
        }

        private Dictionary<Types.TrainingType, List<TrainingDataTainingDetailsModel.TrainingDataValue>> GetActivityInfoSummary(DateTime now, int numberOfRecordsToDisplay,
                                                  PeriodTrainingDataStrategy periodTrainingDataStrategy,
                                                  NumericCalculationTypeStrategy numericCalculationTypeStrategy,
                                                  IEnumerable<Types.TrainingType> selectedActivityTypes, List<TrainingSession> sessions)
        {
            var activityTypesDic = new Dictionary<Types.TrainingType, List<TrainingDataTainingDetailsModel.TrainingDataValue>>();

            foreach (var type in selectedActivityTypes)
            {
                var timeSpans = periodTrainingDataStrategy.GetTimeSpans(now, numberOfRecordsToDisplay);

                Types.TrainingType type1 = type;
                var summaries = CollectDataFromromSessions(
                    timeSpans,
                    sessions.ToList(),
                    s =>
                    numericCalculationTypeStrategy.CalculateTotalEffortForActivity(
                        periodTrainingDataStrategy.FilterLessThanHourActivities(s), type1));
                //summaries.Reverse();
                activityTypesDic.Add(type, summaries);
            }

            return activityTypesDic;
        }


        private List<TrainingDataTainingDetailsModel.TrainingDataValue> CollectDataFromromSessions(IEnumerable<TimeSpan> timeSpans,
                                                              List<TrainingSession> list,
                                                              Func<List<TrainingSession>, float> valueSelector)
        {
            var values = new List<TrainingDataTainingDetailsModel.TrainingDataValue>();

            foreach (var timespan in timeSpans)
            {
                var allSessionInTimespan = AllSessionInTimespan(list, timespan);


                var count = allSessionInTimespan.Count();

                values.Add(new TrainingDataTainingDetailsModel.TrainingDataValue { Value = count != 0 ? valueSelector(allSessionInTimespan) : 0, Date = timespan.GetSpanDate() });
            }

            return values;
        }

        #endregion

        #region Effort

        public List<TrainingDataTainingDetailsModel.TrainingDataValue> GetEffortSummary(int userId, DateTime now,
                                                                       PeriodTrainingDataStrategy periodTrainingDataStrategy, NumericCalculationTypeStrategy numericCalculationTypeStrategy, int numberOfRecordsToDisplay)
        {
            var sessions = TrainingSessionRepository.GetAsQueryable<TrainingSession>().ToList()
                                                                 .Where(d => d.UserId == userId && periodTrainingDataStrategy.IsInLowerTimeBound(d))
                                                                 .ToList();

            return GetEffortSummary(now, periodTrainingDataStrategy, numericCalculationTypeStrategy, numberOfRecordsToDisplay, sessions);
        }

        private List<TrainingDataTainingDetailsModel.TrainingDataValue> GetEffortSummary(DateTime now, PeriodTrainingDataStrategy periodTrainingDataStrategy,
                                      NumericCalculationTypeStrategy numericCalculationTypeStrategy,
                                      int numberOfRecordsToDisplay, IEnumerable<TrainingSession> sessions)
        {
            var timeSpans = periodTrainingDataStrategy.GetTimeSpans(now, numberOfRecordsToDisplay);
            var summaries = CollectEffortsFromSessions(timeSpans, numericCalculationTypeStrategy, sessions.ToList());

            return summaries;
        }

        private List<TrainingDataTainingDetailsModel.TrainingDataValue> CollectEffortsFromSessions(IEnumerable<TimeSpan> timeSpans, NumericCalculationTypeStrategy numericCalculationTypeStrategy,
                                                              List<TrainingSession> list)
        {
            var values = new List<TrainingDataTainingDetailsModel.TrainingDataValue>();

            foreach (var timespan in timeSpans)
            {
                var allSessionInTimespan = AllSessionInTimespan(list, timespan);
                var count = allSessionInTimespan.Count();

                values.Add(new TrainingDataTainingDetailsModel.TrainingDataValue { Value = count != 0 ? numericCalculationTypeStrategy.CalculateTotalEffort(allSessionInTimespan) : 0, Date = timespan.GetSpanDate() });
            }

            return values;
        }

        #endregion

        #region Helpers

        private static List<TrainingSession> AllSessionInTimespan(
            IEnumerable<TrainingSession> list, TimeSpan timespan)
        {
            var allSessionInTimespan = new List<TrainingSession>();
            var includedSessions = list.Where(s => timespan.InTimeIncluded(s.DateTrainingStart) &&
                                                   timespan.InTimeIncluded(s.DateTrainingEnd));

            allSessionInTimespan.AddRange(includedSessions);
            return allSessionInTimespan;
        }

        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="trainingSession"></param>
        /// <returns></returns>
        public bool SubmitTrainingData(int userId, TrainingSession trainingSession)
        {
            if (TrainingSessionRepository == null)
            {
                TrainingSessionRepository = new SqlDataAccess.Entities.View.TrainingSessionsRepository();
            }

            if (TrainingSessionDataRepository == null)
            {
                TrainingSessionDataRepository = new NoSqlDataAccess.Entities.View.TrainingSessionMeasurmentDataRepository();
            }

            Guid uniqueId = Guid.NewGuid();

            GenericError error;

            trainingSession.UserId = userId;

            try
            {
                trainingSession.MeasurementInfo.TrainingSessionId = uniqueId;

                TrainingSessionDataRepository.InsertEntity(out error, trainingSession.MeasurementInfo);

            }
            catch (Exception e)
            {
                error = new GenericError() { ErrorDesc = e.Message };
            }

            if (error == null)
            {
                try
                {
                    // SAVE TO SQL SERVER

                    trainingSession.TrainingSessionId = uniqueId;

                    TrainingSessionRepository.InsertEntity(out error, trainingSession);
                }
                catch (Exception e)
                {
                    error = new GenericError() { ErrorDesc = e.Message };
                }

            }

            return error == null;
        }

        public List<float> GetActivityInfoSums(int userId, DateTime now,
                                                        PeriodTrainingDataStrategy periodStrategy,
                                                        List<Types.TrainingType> activitylist)
        {


            var list = TrainingSessionRepository.GetAsQueryable<TrainingSession>().ToList()
                                                                 .Where(
                                                                     d =>
                                                                     d.UserId == userId && periodStrategy.IsInLowerTimeBound(d))
                                                                 .ToList();

            var timeSpans = periodStrategy.GetTimeSpans(now, periodStrategy.NumberOfRecords);

            var values = new List<float>();

            foreach (var timespan in timeSpans)
            {
                var allSessionInTimespan = AllSessionInTimespan(list, timespan);

                var activityCount = allSessionInTimespan.Sum(s => activitylist.Contains(s.TrainingEnumTypeId) ? 1 : 0);

                values.Add(activityCount);
            }

            return values;

        }
    }
}