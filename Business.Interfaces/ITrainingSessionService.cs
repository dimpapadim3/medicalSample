using System;
using System.Collections.Generic;
using Model.DailyInfo;
using Model.View;
using ServiceModel;
using ServiceModel.View;
using Sport = Model.StaticLookups.Sport;

namespace Business.Interfaces
{
    public interface ITrainingSessionService
    {
        List<TrainingSessionComment> GetSessionComments(Guid trainingSessionId);
        void SaveSessionComment(TrainingSessionComment trainingSessionCommentItem);    
        void DeleteComment(string id);

        List<TrainingSession> GetTrainingSessionItems(int userd);
        List<TrainingSession> GetTrainingSessionItems(Func<TrainingSession, bool> filter);
        void InsertTrainingSession(List<TrainingSession> sessions);
        void DeleteSession(TrainingSession trainingSession);    

        IList<Zone> GetZonesForMeasurmentType(long userId, long masterUserId, Types.MeasurementInfoTypes measurmentType);    
        void UpdateZone(Zone zone);
        void AddNewZone(Zone zone );
        void DeleteZone(int zoneId);

        IList<Effort> GetEffortTypes();
        IList<TrainingType> GetTrainingTypes();
        IList<SportExtended> GetSportTypes(int masterUserId);

        IList<TrainingSessionMeasurmentData> GetSessionMeasurmentInfo(Guid newGuid);
        void RemoveMeasurmentData(IList<TrainingSessionMeasurmentData> measurmentData);
        void InsertMeasurmentData(TrainingSessionMeasurmentData session1Data);

        Tuple<long, long> GetCoreTeamUserId(long userId, long masterUserId);

        SessionMeasurmentInfo GetSessionMeasurmentInfo(int measurmentTypeId, string sessionId);
        void SplitSessionAtSpecificTime(List<TrainingSession> sessions, TrainingSession trainingSession, DateTime splitingDateTime);
        void CutSessionAtSpecificTime(TrainingSession trainingSession, DateTime splitingDateTime, bool b);
    }
}