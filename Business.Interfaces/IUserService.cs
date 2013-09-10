using System;
using ErrorClasses;
using Model.StaticLookups;
using ServiceModel;

namespace Business.Interfaces
{
    public interface IUserService   
    {
        string GetUserFullName(long p, out ErrorClasses.GenericError error);
        bool ChangePassword(string username, string empty, string oldPass, string pass, out GenericError err);
        bool InviteCoreTeamUser(string username, string email, string role, out GenericError err);
        bool CancelCoreTeamUserinvitation(string username, string parameter, out GenericError err);

        Model.User.User GetUserInfo(string Username, out GenericError error);

        bool EmailExists(string p, out GenericError error2);

        bool UsernameExists(string p, out GenericError error1);

        System.Collections.Generic.List<string> GetSerials(string Username, out GenericError err);

        bool PairSensors(string Username, string p1, string p2, out GenericError err);

        bool GetUserIsMaster(string TicketUsername, out GenericError err2);

        bool UpdateUser(string TicketUsername, string FirstName, string LastName, string SecurityQuestion, string SecurityAnswer, string Country, string Language, string TimeZone, string MetricType, string TemperatureType, string WeightType, string HeightType, string date, string Weight, string Height, string Sports, string OtherSport, string gender, string FirstWeek, out GenericError err);

        ServiceModel.CoreTeamMembers GetCoreTeamUsers(string p, string Username, out GenericError err);

        bool RemoveCoreTeamUser(string Username, string p1, string p2, out GenericError err);
        Teams GetTeams(long userId, out GenericError error);

        SportsExtended GetMasterUserSportsExtended(long masterUserId, out ErrorClasses.GenericError error);

        DateTime? GetUserCreationDate(long userId, out GenericError err);
        bool  UpdateUserWeight(long userId, double weight);

        AuthenticationInfo Authenticate(string p1, string p2, out GenericError error);

        bool RequestResetPassword(string p, out GenericError error);

        bool CreateCustomSport(long masterUserId, string sportName, out ErrorClasses.GenericError error);

        bool RequestRegister(string p, out GenericError error);

        bool GetResetPasswordInfo(string id, out string username, out string newpass, out GenericError err);

        bool ChangePasswordByLink(string username, string id, string newPassword, out GenericError err);

        string GetEmailInvitationUrl(string p, out GenericError err);

        bool CreateCoreTeamUser(int mode, string p, string FirstName, string LastName, string Email, string Username, string Password, string SecurityQuestion, string SecurityAnswer, string Country, string Language, string TimeZone, string MetricType, string TemperatureType, string WeightType, string HeightType, out GenericError err);


        string GetEmailRegistrationUrl(string p, out GenericError err);

        bool CreateMasterUser(string p, string FirstName, string LastName, string Email, string Username, string Password, string SecurityQuestion, string SecurityAnswer, string Country, string Language, string TimeZone, string MetricType, string TemperatureType, string WeightType, string HeightType, string date, string Weight, string Height, string Sports, string OtherSport, string gender, string FirstWeek, string Serials, out GenericError err);

        System.Collections.Generic.IDictionary<long, string> GetCoreTeamUsers(long getCurrentMasterUserId, long p);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        System.Nullable<float> GetCalibrationFactorById(long userId, out ErrorClasses.GenericError error);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="CalibrationFactor"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        bool SetCalibrationFactorById(long userId, float CalibrationFactor, out ErrorClasses.GenericError error);
    }
}