using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using Business.Interfaces;
using DataAccess.Interfaces;
using ErrorClasses;
using Logger;
using Model.DailyInfo;
using Model.StaticLookups;
using Model.View;
using MongoDB.Bson;
using ServiceModel;
using ServiceModel.View;
using SqlDataAccess.Entities.View;
using StructureMap;

namespace Business.View
{
    /// <summary>
    /// 
    /// </summary>
    public class TrainingSessionService : ITrainingSessionService
    {
        /// <summary>
        /// 
        /// </summary>
        protected static ILogger Log = ObjectFactory.GetInstance<ILogger>();

        #region SessionCommnets

        public List<TrainingSessionComment> GetSessionComments(Guid trainingSessionId)
        {
            List<TrainingSessionComment> trainingSessionCommentsList;

            try
            {
                GenericError error;
                trainingSessionCommentsList = TrainingSessionCommentsRepository.GetEntities(out error, c => c.TrainingSessionId == trainingSessionId);
            }
            catch (Exception ex)
            {
                DateTime dateTime = DateTime.UtcNow;
                Log.LogInfo(dateTime + ": " + "During the Execution of Get an error occured; Excreption Message: " + ex);
                throw;
            }

            return trainingSessionCommentsList;
        }

        public void SaveSessionComment(TrainingSessionComment trainingSessionCommentItem)
        {
            GenericError error;

            TrainingSessionCommentsRepository.InsertEntity(out error, trainingSessionCommentItem);
        }

        #endregion

        #region TrainingSession

        public List<TrainingSession> GetTrainingSessionItems(int userd)
        {
            List<TrainingSession> trainingSessions;
            try
            {
                GenericError error;
                trainingSessions = TrainingSessionsRepository.GetEntities(out error, s => s.UserId == userd);
            }
            catch (Exception ex)
            {
                var dateTime = DateTime.UtcNow;
                Log.LogInfo(dateTime + ": " + "During the Execution of GetList<T, TKey> an error occured; Excreption Message: " + ex);
                throw;
            }

            return trainingSessions;
        }

        public List<TrainingSession> GetTrainingSessionItems(Func<TrainingSession, bool> filter)
        {
            GenericError error;

            return TrainingSessionsRepository.GetEntities(out error, filter);
        }

        public IList<TrainingSessionMeasurmentData> GetSessionMeasurmentInfo(Guid trainingSessionId)
        {
            GenericError error;

            return TrainingSessionDataRepository.GetEntities(out error, m => m.TrainingSessionId == trainingSessionId);
        }

        public void DeleteComment(string id)
        {
            GenericError error;
            TrainingSessionCommentsRepository.Remove(c => c.Id == new ObjectId(id), out error);
        }

        #endregion

        #region Zones
        public IList<Zone> GetZonesForMeasurmentType(long userId, long masterUserId,
                                                 Types.MeasurementInfoTypes measurmentType)
        {
            return ZonesRepository.GetZonesForCoreTeamUser(userId, masterUserId).Where(z => z.MeasurmentTypeId == (int)measurmentType)
                                                        .ToList();
        }

        public void UpdateZone(Zone z)
        {
            var viewZonesRepository = ZonesRepository as ViewZonesRepository;
            if (viewZonesRepository != null) viewZonesRepository.UpdateZones(z);
        }

        public void AddNewZone(Zone zone)
        {
            GenericError error;

            ZonesRepository.InsertEntity(out error, zone);
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        public IRepositoryBase<TrainingSession> TrainingSessionsRepository { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public IRepositoryBase<TrainingSessionMeasurmentData> TrainingSessionDataRepository { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public IRepositoryBase<TrainingSessionComment> TrainingSessionCommentsRepository { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        public IZonesRepository ZonesRepository { get; set; }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="masterUserId"></param>
        /// <returns></returns>
        public Tuple<long, long> GetCoreTeamUserId(long userId, long masterUserId)
        {
            return ViewZonesRepository.GetCoreTeamUserId(userId, masterUserId);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<Effort> GetEffortTypes()
        {
            return new ViewUnitOfWork().Effort.ToList();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public IList<TrainingType> GetTrainingTypes()
        {
            return new ViewUnitOfWork().TrainingType.ToList();
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="masterUserId"></param>
        /// <returns></returns>
        public IList<SportExtended> GetSportTypes(int masterUserId)
        {
            ErrorClasses.GenericError error;
            if (masterUserId == -1)
                return new List<SportExtended>();
            return ObjectFactory.GetInstance<IUserService>().GetMasterUserSportsExtended(masterUserId, out error).SportsList;
            // return new SView().Sports.ToList();

        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="zoneId"></param>
        public void DeleteZone(int zoneId)
        {
            GenericError error;
            ZonesRepository.Remove(z => z.ZoneId == zoneId, out error);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessions"></param>
        public void InsertTrainingSession(List<TrainingSession> sessions)
        {
            GenericError error;

            TrainingSessionsRepository.InsertEntity(out error, sessions);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="trainingSession"></param>
        public void DeleteSession(TrainingSession trainingSession)
        {
            GenericError error;
            TrainingSessionsRepository.Remove(s => s.TrainingSessionId == trainingSession.TrainingSessionId, out error);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurmentData"></param>
        public void RemoveMeasurmentData(IList<TrainingSessionMeasurmentData> measurmentData)
        {
            GenericError error;
            measurmentData.ToList()
                          .ForEach(md => TrainingSessionDataRepository.Remove(m => m.TrainingSessionId == md.TrainingSessionId, out     error));
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="session1Data"></param>
        public void InsertMeasurmentData(TrainingSessionMeasurmentData session1Data)
        {
            GenericError error;
            TrainingSessionDataRepository.InsertEntity(out error, session1Data);
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="selectData"></param>
        /// <returns></returns>
        private static TrainingSessionMeasurmentData CopyPartialMeasurmentsData(TrainingSessionMeasurmentData data,
                                                                                Func<IList<DecimalData>, Types.MeasurementInfoTypes, List<DecimalData>> selectData)
        {
            var session1Data = new TrainingSessionMeasurmentData();

            foreach (Types.MeasurementInfoTypes type in Enum.GetValues(typeof(Types.MeasurementInfoTypes)))
            {
                var selectMeasurmentData = DataUtils.MeasurmentPropertySelectors[type];

                var mData = selectMeasurmentData(data);

                if (mData != null)
                {
                    var setData = DataUtils.MeasurmentPropertySetters[type];

                    var partialData = selectData(mData, type);
                    setData(session1Data, partialData);
                }
            }
            return session1Data;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="measuremntType"></param>
        /// <param name="seconds"></param>
        /// <returns></returns>
        //private static int GetIndexForTime(Types2.MeasurementInfoTypes measuremntType, float seconds)
        //{
        //    var frequency = TrainingSessionMeasurmentData.Frequecies[measuremntType];
        //    var index = seconds * frequency;

        //    return (int)Math.Round(index);
        //}

        /// <summary>
        /// 
        /// </summary>
        /// <param name="trainingSession"></param>
        /// <param name="splitingSeconds"></param>
        /// <param name="shouldKeepLeft"></param>
        public void CutSessionAtSpecificTime(TrainingSession trainingSession, DateTime splitingDateTime, bool shouldKeepLeft)
        {
            var measurmentData = GetSessionMeasurmentInfo(trainingSession.TrainingSessionId);

            var data = measurmentData.FirstOrDefault();

            TrainingSessionMeasurmentData partialMeasurmentData = null;

            if (shouldKeepLeft)
            {
                //CopyPartialMeasurmentsData(data, (m, t) => m.Take(GetIndexForTime(t, splitingSeconds)).ToList());
                CopyPartialMeasurmentsData(data, (m, t) => m.Where(x => x.Time >= splitingDateTime).ToList());
            }
            else
            {
                //CopyPartialMeasurmentsData(data, (m, t) => m.Skip(GetIndexForTime(t, splitingSeconds)).ToList());
                CopyPartialMeasurmentsData(data, (m, t) => m.Where(x => x.Time <= splitingDateTime).ToList());
            }

            if (partialMeasurmentData != null)
            {
                GenericError error = null;
                try
                {
                    RemoveMeasurmentData(measurmentData);
                }
                catch (Exception e)
                {
                    error = new GenericError { ErrorDesc = e.Message };
                }

                if (error == null)
                {
                    partialMeasurmentData.TrainingSessionId = trainingSession.TrainingSessionId;
                    partialMeasurmentData.UserId = trainingSession.UserId;
                    InsertMeasurmentData(partialMeasurmentData);


                }
            }
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="measurmentTypeId"></param>
        /// <param name="sessionId"></param>
        /// <returns></returns>
        public SessionMeasurmentInfo GetSessionMeasurmentInfo(int measurmentTypeId, string sessionId)
        {
            IList<TrainingSessionMeasurmentData> measurments =
                GetSessionMeasurmentInfo(Guid.Parse(sessionId));

            TrainingSession session =
                GetTrainingSessionItems(t => t.TrainingSessionId == Guid.Parse(sessionId))
                               .FirstOrDefault();
            var info = new SessionMeasurmentInfo
                {
                    MeasurmentData = new List<DecimalData>(),
                    Date = DateTime.Now.ToString("MMM ddd d HH:mm yyyy")
                };

            if (measurments != null && measurments.Count > 0)
            {
                Func<TrainingSessionMeasurmentData, IList<DecimalData>> measurmentSelector =
                    DataUtils.MeasurmentPropertySelectors[(Types.MeasurementInfoTypes)measurmentTypeId];

                IList<DecimalData> selectedataStream = measurmentSelector(measurments.FirstOrDefault());

                if (selectedataStream != null && selectedataStream.Count > 0)
                {
                    info = new SessionMeasurmentInfo
                        {
                            MeasurmentData = selectedataStream
                        };
                    if (session != null)
                    {
                        info.Date = session.DateTrainingStart.ToString("MMM ddd d HH:mm yyyy");
                        info.TrainingType = session.TrainingType.TrainingTypeName;
                    }
                }
            }
            return info; 
        }
        
        /// <summary>
        /// 
        /// </summary>
        /// <param name="sessions"></param>
        /// <param name="trainingSession"></param>
        /// <param name="splitingSeconds"></param>
        public void SplitSessionAtSpecificTime(List<TrainingSession> sessions, TrainingSession trainingSession, DateTime splitingDateTime)
        {

            var measurmentData = GetSessionMeasurmentInfo(trainingSession.TrainingSessionId);

            var data = measurmentData.FirstOrDefault();

            TrainingSessionMeasurmentData session2Data = null;
            TrainingSessionMeasurmentData session1Data = null;

            if (data != null)
            {
                session1Data = CopyPartialMeasurmentsData(data, (m, t) => m.Where(x => x.Time >= splitingDateTime).ToList());
                session2Data = CopyPartialMeasurmentsData(data, (m, t) => m.Where(x => x.Time <= splitingDateTime).ToList());
                //session1Data = CopyPartialMeasurmentsData(data, (m, t) => m.Take(GetIndexForTime(t, splitingSeconds)).ToList());
                //session2Data = CopyPartialMeasurmentsData(data, (m, t) => m.Skip(GetIndexForTime(t, splitingSeconds)).ToList());
            }

            if (session1Data != null &&
                session2Data != null)
            {
                session1Data.TrainingSessionId = sessions[0].TrainingSessionId;
                session2Data.TrainingSessionId = sessions[1].TrainingSessionId;

                session1Data.UserId = trainingSession.UserId;
                session2Data.UserId = trainingSession.UserId;

                GenericError error;

                try
                {
                    TrainingSessionDataRepository.InsertEntity(out error, session1Data);
                    TrainingSessionDataRepository.InsertEntity(out error, session2Data);
                }
                catch (Exception e)
                {
                    error = new GenericError { ErrorDesc = e.Message };
                }

                if (error == null)
                {
                    TrainingSessionsRepository.InsertEntity(out error, sessions);

                    measurmentData.ToList().ForEach(md => TrainingSessionDataRepository.Remove(m => m.TrainingSessionId == md.TrainingSessionId, out error));
                    TrainingSessionsRepository.Remove(s => s.TrainingSessionId == trainingSession.TrainingSessionId, out error);
                }

            }
        }
    }
}