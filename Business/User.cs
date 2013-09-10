using System;
using System.Collections.Generic;
using System.Linq;
using Business.Interfaces;
using ErrorClasses;
using Model.DailyInfo;
using NoSqlDataAccess.Entities.DailyInfo;
using ServiceModel;
using Controller = Security.Controller;

namespace Business
{
    public class User : IUserService, IPopularActivitiesTrackingService
    {

        #region Popular activities

        public void UpdateFavouriteActivities(string selectdActivityId, long userId)
        {
            GenericError error;
            var favouriteActivityRepository = new FavouriteActivityRepository();
            var entities = favouriteActivityRepository.GetEntities(out error, t => t.UserId == userId);
            if (entities.Count > 0)
            {
                var currentFavouriteCount = entities.First();
                var currentCountForSelectedActivity =
                    currentFavouriteCount.ActivityClassCount.FirstOrDefault(
                        activity => activity.Key.ToLower() == selectdActivityId);

                if (currentCountForSelectedActivity.Key != null)
                    favouriteActivityRepository.UpdateRecord(
                        new KeyValuePair<string, int>(currentCountForSelectedActivity.Key,
                                                      currentCountForSelectedActivity.Value + 1));
                else
                {
                    favouriteActivityRepository.UpdateRecord(
                      new KeyValuePair<string, int>(selectdActivityId, 1));
                }
            }
            else
            {
                favouriteActivityRepository.InsertEntity(out error, new FavouriteActivities()
                {
                    ActivityClassCount = new Dictionary<string, int> { { selectdActivityId, 1 } },
                    UserId = (int)userId,

                });

            }
        }

        public IComparer<ISport> GetPopularActivitiesComparer(int userId)
        {
            return new PopularActivitiesComparer(userId);
        }

        internal class PopularActivitiesComparer : Comparer<ISport>
        {
            //private readonly IUserService _service;
            private readonly long _userId;
            private readonly Dictionary<string, int> _favouriteActivities;

            public PopularActivitiesComparer(
                //   IUserService service,
                int userId)
            {
                GenericError error = null;
                var favouriteActivityRepository = new FavouriteActivityRepository();

                var favouriteActivities = favouriteActivityRepository.GetAsQueryable<FavouriteActivities>().FirstOrDefault(w => w.UserId == userId);

                if (favouriteActivities != null)
                    _favouriteActivities = favouriteActivities.ActivityClassCount;

                //_service = service;
                _userId = userId;
            }

            public override int Compare(ISport x, ISport y)
            {
                if (_favouriteActivities == null)
                    return x.Id.CompareTo(y.Id);

                if (_favouriteActivities.ContainsKey(x.Id.ToString())
                    && _favouriteActivities.ContainsKey(y.Id.ToString()))
                    return _favouriteActivities[x.Id.ToString()].CompareTo(_favouriteActivities[x.Id.ToString()]);

                if (_favouriteActivities.ContainsKey(x.Id.ToString()) &&
                    !_favouriteActivities.ContainsKey(y.Id.ToString()))
                    return -1;

                if (!_favouriteActivities.ContainsKey(x.Id.ToString()) &&
                     _favouriteActivities.ContainsKey(y.Id.ToString()))
                    return 1;

                return x.Id.CompareTo(y.Id);
            }
        }

        #endregion

        public ServiceModel.AuthenticationInfo Authenticate(string username, string password,
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                Model.User.CredentialsInfo crInfo = SqlDataAccess.Entities.User.GetCredentialsInfo(username, out error);

                if (error != null)
                    return null;

                if (crInfo == null)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return null;
                }


                ServiceModel.AuthenticationInfo authInfo = new ServiceModel.AuthenticationInfo();

                authInfo.Exists = crInfo.UserFound;
                authInfo.Username = username;
                authInfo.IsEnabled = crInfo.IsEnabled;
                authInfo.Id = crInfo.Id;
                if (authInfo.IsEnabled)
                    authInfo.Authenticated = Controller.Authenticate(password, crInfo.PasswordSalt, crInfo.PasswordHash);
                else
                    authInfo.Authenticated = false;

                return authInfo;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }


        public bool AuthenticateAndChangePassword(string username,
            string oldPassword, string newPassword,
                   out ErrorClasses.GenericError error)
        {
            error = null;
            ServiceModel.Result res = new ServiceModel.Result();

            ServiceModel.AuthenticationInfo authInfo = Authenticate(username, oldPassword,
                out error);

            if (error != null)
                return false;

            if (authInfo == null)
            {
                res.Status = false;
                res.Error = ErrorClasses.Controller.GetInvalidCredentialsError();

                return false;
            }

            if (!authInfo.Authenticated)
            {
                res.Status = false;
                res.Error = ErrorClasses.Controller.GetInvalidCredentialsError();

                return false;
            }


            bool changed = ChangePassword(username,
                null, oldPassword, newPassword, out error);

            if (error != null)
                return false;

            return changed;
        }

        public bool UsernameExists(string username, out ErrorClasses.GenericError error)
        {

            try
            {
                bool exists = SqlDataAccess.Entities.User.UsernameExists(username, out error);

                if (error != null)
                    return false;

                return exists;


            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public string GetUserNameById(int userId)
        {
            ErrorClasses.GenericError error = null;
            try
            {
                return SqlDataAccess.Entities.User.GetUserNameById(userId, out error);
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return "";
            }
        }

        public bool EmailExists(string email, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                bool exists = SqlDataAccess.Entities.User.EmailExists(email, out error);

                if (error != null)
                    return false;

                return exists;


            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public string GetUserFullName(long userId, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                return SqlDataAccess.Entities.User.GetUserFullName(userId, out error);
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public bool InviteCoreTeamUser(
            string masterUserUsername,
            string coreTeamUserEmail, string coreTeamUserRole,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                string masterUserEmail;
                long masterUserId = SqlDataAccess.Entities.User.GetUserIdAndEmail(masterUserUsername, out masterUserEmail, out error);
                if (error != null)
                    return false;
                if (masterUserId <= 0)
                {
                    error = ErrorClasses.Controller.GetUserNotFoundError();
                    return false;
                }

                if (masterUserEmail.ToUpper() == coreTeamUserEmail.ToUpper())
                {
                    error = ErrorClasses.Controller.GetCannotInviteYourselfError();
                    return false;
                }

                string urlLink = Utils.RandomGenerator.GetUrlLink();
                if (string.IsNullOrEmpty(urlLink))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("URL Link");
                    return false;
                }

                bool result = SqlDataAccess.Entities.User.InviteCoreTeamUser(masterUserId,
                    coreTeamUserEmail, coreTeamUserRole, urlLink,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public bool RemoveCoreTeamUser(
            string masterUserUsername, string masterUserEmail,
            string coreTeamUserEmail,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                long masterUserId = SqlDataAccess.Entities.User.GetUserId(masterUserUsername, masterUserEmail, out error);
                if (error != null)
                    return false;
                if (masterUserId <= 0)
                {
                    error = ErrorClasses.Controller.GetUserNotFoundError();
                    return false;
                }

                long coreTeamUserId = SqlDataAccess.Entities.User.GetUserId(null, coreTeamUserEmail, out error);
                if (error != null)
                    return false;
                if (coreTeamUserId <= 0)
                {
                    error = ErrorClasses.Controller.GetUserNotFoundError();
                    return false;
                }

                bool result = SqlDataAccess.Entities.User.RemoveCoreTeamUser(masterUserId, coreTeamUserId,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }


        public bool CancelCoreTeamUserinvitation(
            string masterUserUsername, string coreTeamUserEmail,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                long masterUserId = SqlDataAccess.Entities.User.GetUserId(masterUserUsername, null, out error);
                if (error != null)
                    return false;
                if (masterUserId <= 0)
                {
                    error = ErrorClasses.Controller.GetUserNotFoundError();
                    return false;
                }


                bool result = SqlDataAccess.Entities.User.CancelCoreTeamUserinvitation(masterUserId,
                    coreTeamUserEmail,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public bool CreateMasterUser(
            //main user data
            string invitationUrlLink,

            string firstName, string familyName, string email, string username, string password,
            string securityQuestion, string securityAnswer,
            string country, string language, string timeZone, string metricType,
            string temperatureType, string weightType, string heightType,
            //master user data
            string dateOfBirth, string weightKg, string heightCm,
            string sport, string otherSport, string gender, string firstDayOfTheWeek,
            //sensors
            string sensorsSerialNumbersDelimited,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                string passwordSalt;
                string passwordHash = Security.Controller.CreateHashAndSalt(password, out passwordSalt);

                if (string.IsNullOrEmpty(passwordSalt))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Password Salt");
                    return false;
                }
                if (string.IsNullOrEmpty(passwordHash))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Password Hash");
                    return false;
                }
                if (string.IsNullOrEmpty(invitationUrlLink))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Link");
                    return false;
                }

                if (string.IsNullOrEmpty(sensorsSerialNumbersDelimited))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Sensor Serial Numbers");
                    return false;
                }
                string[] serials = sensorsSerialNumbersDelimited.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (serials == null)
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Sensor Serial Numbers");
                    return false;
                }
                if (serials.Length <= 0)
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Sensor Serial Numbers");
                    return false;
                }
                List<string> sensorsSerialNumbers = new List<string>();
                foreach (string serial in serials)
                {
                    if (!string.IsNullOrEmpty(serial))
                        sensorsSerialNumbers.Add(serial.Trim());
                }


                bool result = SqlDataAccess.Entities.User.CreateMasterUser(
                    invitationUrlLink,
                    firstName, familyName, email, username, passwordHash, passwordSalt,
                    securityQuestion, securityAnswer, country, language, timeZone, metricType,
                    temperatureType, weightType, heightType,
                    dateOfBirth, weightKg, heightCm,
                    sport, otherSport, gender, firstDayOfTheWeek,
                    sensorsSerialNumbers,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        IDictionary<long, string> IUserService.GetCoreTeamUsers(long getCurrentMasterUserId, long p)
        {
            return GetCoreTeamUsers(getCurrentMasterUserId, p);
        }

        public bool UpdateUser(
            //main user data
            string username,
            string firstName, string familyName,
                  string securityQuestion, string securityAnswer,
                  string country, string language, string timeZone, string metricType,
                  string temperatureType, string weightType, string heightType,
            //master user data
                  string dateOfBirth, string weightKg, string heightCm,
                  string sport, string otherSport, string gender, string firstDayOfTheWeek,
                  out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                bool result = SqlDataAccess.Entities.User.UpdateUser(
                    username, firstName, familyName,
                    securityQuestion, securityAnswer, country, language, timeZone, metricType,
                    temperatureType, weightType, heightType,
                    dateOfBirth, weightKg, heightCm,
                    sport, otherSport, gender, firstDayOfTheWeek,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public bool UpdateUserWeight(
                long id, double weightKg)
        {
            ErrorClasses.GenericError error = null;

            try
            {

                bool result = SqlDataAccess.Entities.User.UpdateUserWeight(id, weightKg, out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

     
        public Model.User.User GetUserInfo(
            string username,
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                if (string.IsNullOrEmpty(username))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Username");
                    return null;
                }


                Model.User.User user = SqlDataAccess.Entities.User.GetUserInfo(username,
                    out error);

                if (error != null)
                    return null;

                if (user == null)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return null;
                }


                return user;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public DateTime? GetUserCreationDate(
            long id,
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                if (id <= 0)
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Id");
                    return null;
                }


                DateTime? dt = SqlDataAccess.Entities.User.GetUserCreationDate(id,
                    out error);

                if (error != null)
                    return null;

                if (dt == null)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return null;
                }


                return dt;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public string GetMasterUserCustomSport(
            long id,
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                if (id <= 0)
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Id");
                    return null;
                }

                string sport = SqlDataAccess.Entities.User.GetMasterUserCustomSport(id);


                return sport;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }


        public bool GetUserIsMaster(
            string username,
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                if (string.IsNullOrEmpty(username))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Username");
                    return false;
                }

                bool isMaster = SqlDataAccess.Entities.User.GetUserIsMaster(username,
                    out error);

                if (error != null)
                    return false;

                return isMaster;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public List<string> GetSerials(
            string username,
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                if (string.IsNullOrEmpty(username))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Username");
                    return null;
                }

                List<string> serials = SqlDataAccess.Entities.User.GetSerials(username,
                    out error);

                if (error != null)
                    return null;

                return serials;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public static IDictionary<long, string> GetCoreTeamUsers(long masterUserId, long currentUserId)
        {
            try
            {
                return SqlDataAccess.Entities.User.GetCoreTeamUsers(masterUserId, currentUserId);
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                return null;
            }
        }

        public ServiceModel.CoreTeamMembers GetCoreTeamUsers
            (string locale, string username, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                if (string.IsNullOrEmpty(locale))
                {
                    error = ErrorClasses.Controller.GetNoLocaleSetError();
                    return null;
                }
                if (string.IsNullOrEmpty(username))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Username");
                    return null;
                }
                if (string.IsNullOrEmpty(locale))
                {
                    error = ErrorClasses.Controller.GetNoLocaleSetError();
                    return null;
                }

                ServiceModel.CoreTeamMembers modelRes = SqlDataAccess.Entities.User.GetCoreTeamUsers
                    (locale, username, out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                return modelRes;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public ServiceModel.Teams GetTeams
            (long userId, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                ServiceModel.Teams modelRes = SqlDataAccess.Entities.User.GetTeams
                    (userId, out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                return modelRes;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }


        public ServiceModel.SportsExtended GetMasterUserSportsExtended
          (long userId, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                ServiceModel.SportsExtended modelRes = SqlDataAccess.Entities.User.GetMasterUserSportsExtended
                    (userId, out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                return modelRes;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public ServiceModel.UserMemberInfoItems GetUserMemberInfo(
            string username, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                if (string.IsNullOrEmpty(username))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Username");
                    return null;
                }

                ServiceModel.UserMemberInfoItems modelRes = SqlDataAccess.Entities.User.GetUserMemberInfo
                    (username, out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                return modelRes;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public bool CreateCoreTeamUser(
            int mode,
            //master user info
            string invitationUrlLink,
            //core team user data
            string firstName, string familyName, string email, string username, string password,
            string securityQuestion, string securityAnswer,
            string country, string language, string timeZone, string metricType,
            string temperatureType, string weightType, string heightType,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                string passwordSalt = null;
                string passwordHash = null;

                if (mode == 0)
                {
                    passwordHash = Security.Controller.CreateHashAndSalt(password, out passwordSalt);

                    if (string.IsNullOrEmpty(passwordSalt))
                    {
                        error = ErrorClasses.Controller.GetMissingParameterError("Password Salt");
                        return false;
                    }
                    if (string.IsNullOrEmpty(passwordHash))
                    {
                        error = ErrorClasses.Controller.GetMissingParameterError("Password Hash");
                        return false;
                    }
                }


                bool result = SqlDataAccess.Entities.User.CreateCoreTeamUser(mode,
                    invitationUrlLink,
                    firstName, familyName, email, username, passwordHash, passwordSalt,
                    securityQuestion, securityAnswer, country, language, timeZone, metricType,
                    temperatureType, weightType, heightType,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

      

        public bool PairSensors(
            //main user data
            string masterUserUsername, string masterUserEmail,
            //sensors
            string sensorsSerialNumbersDelimited,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                long masterUserId = SqlDataAccess.Entities.User.GetUserId(masterUserUsername, masterUserEmail, out error);
                if (error != null)
                    return false;
                if (masterUserId <= 0)
                {
                    error = ErrorClasses.Controller.GetUserNotFoundError();
                    return false;
                }

                if (string.IsNullOrEmpty(sensorsSerialNumbersDelimited))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Sensor Serial Numbers");
                    return false;
                }
                string[] serials = sensorsSerialNumbersDelimited.Split(new string[1] { "," }, StringSplitOptions.RemoveEmptyEntries);
                if (serials == null)
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Sensor Serial Numbers");
                    return false;
                }
                if (serials.Length <= 0)
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Sensor Serial Numbers");
                    return false;
                }
                List<string> sensorsSerialNumbers = new List<string>();
                foreach (string serial in serials)
                {
                    if (!string.IsNullOrEmpty(serial))
                        sensorsSerialNumbers.Add(serial);
                }


                bool result = SqlDataAccess.Entities.User.PairSensors(masterUserId,
                    sensorsSerialNumbers,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public bool ChangePassword(
            string masterUserUsername, string masterUserEmail,
            string oldPassword, string newPassword,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                long masterUserId = SqlDataAccess.Entities.User.GetUserId(masterUserUsername, masterUserEmail, out error);
                if (error != null)
                    return false;
                if (masterUserId <= 0)
                {
                    error = ErrorClasses.Controller.GetUserNotFoundError();
                    return false;
                }

                if (string.IsNullOrEmpty(oldPassword))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Old Password");
                    return false;
                }
                if (string.IsNullOrEmpty(newPassword))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("New Password");
                    return false;
                }

                string passwordHash;
                string passwordSalt;
                bool result = SqlDataAccess.Entities.User.GetPasswordInfo(masterUserId,
                    out passwordHash, out passwordSalt, out error);

                if (error != null)
                    return false;

                if (!result || string.IsNullOrEmpty(passwordSalt) || string.IsNullOrEmpty(passwordHash))
                {
                    error = ErrorClasses.Controller.GetNoResultError("Password info not retrieved");
                    return false;
                }


                bool authenticated = Controller.Authenticate(oldPassword, passwordSalt, passwordHash);
                if (!authenticated)
                {
                    error = ErrorClasses.Controller.GetInvalidCredentialsError();
                    return false;
                }


                passwordHash = Security.Controller.CreateHashAndSalt(newPassword, out passwordSalt);
                if (string.IsNullOrEmpty(passwordSalt))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Password Salt");
                    return false;
                }
                if (string.IsNullOrEmpty(passwordHash))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Password Hash");
                    return false;
                }


                result = SqlDataAccess.Entities.User.ChangePassword(masterUserId,
                    passwordHash, passwordSalt,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }


        public bool ChangePasswordByLink(
                string masterUserUsername,
                string urlLink, string newPassword,
            //
                out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                long masterUserId = SqlDataAccess.Entities.User.GetUserId(masterUserUsername, null, out error);
                if (error != null)
                    return false;
                if (masterUserId <= 0)
                {
                    error = ErrorClasses.Controller.GetUserNotFoundError();
                    return false;
                }

                if (string.IsNullOrEmpty(urlLink))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("URL link");
                    return false;
                }
                if (string.IsNullOrEmpty(newPassword))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("New Password");
                    return false;
                }

                string passwordHash;
                string passwordSalt;
                bool result = SqlDataAccess.Entities.User.GetPasswordInfo(masterUserId,
                    out passwordHash, out passwordSalt, out error);

                if (error != null)
                    return false;

                if (!result || string.IsNullOrEmpty(passwordSalt) || string.IsNullOrEmpty(passwordHash))
                {
                    error = ErrorClasses.Controller.GetNoResultError("Password info not retrieved");
                    return false;
                }

                passwordHash = Security.Controller.CreateHashAndSalt(newPassword, out passwordSalt);
                if (string.IsNullOrEmpty(passwordSalt))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Password Salt");
                    return false;
                }
                if (string.IsNullOrEmpty(passwordHash))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Password Hash");
                    return false;
                }


                result = SqlDataAccess.Entities.User.ChangePasswordAndRemoveRequest(masterUserId,
                    passwordHash, passwordSalt, urlLink,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }


        public bool RequestResetPassword(string userEmail,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                long userId = SqlDataAccess.Entities.User.GetUserId(null, userEmail, out error);
                if (error != null)
                    return false;
                if (userId <= 0)
                {
                    error = ErrorClasses.Controller.GetUserNotFoundError();
                    return false;
                }


                string urlLink = Utils.RandomGenerator.GetUrlLink();
                if (string.IsNullOrEmpty(urlLink))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("URL Link");
                    return false;
                }

                string newPassword = Utils.RandomGenerator.GetPassword();
                if (string.IsNullOrEmpty(urlLink))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("New Password");
                    return false;
                }

                bool result = SqlDataAccess.Entities.User.RequestResetPassword(userId, urlLink, newPassword,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }




        public bool CreateCustomSport(long masterUserId, string sportName,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {


                bool result = SqlDataAccess.Entities.User.CreateCustomSport(masterUserId, sportName,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public bool RequestRegister(string userEmail,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                string urlLink = Utils.RandomGenerator.GetUrlLink();
                if (string.IsNullOrEmpty(urlLink))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("URL Link");
                    return false;
                }


                bool result = SqlDataAccess.Entities.User.RequestRegister(urlLink, userEmail,
                    out error);


                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public bool GetResetPasswordInfo(string urlLink, out string username,
            out string password, out ErrorClasses.GenericError error)
        {
            error = null;
            username = null;
            password = null;


            try
            {
                if (string.IsNullOrEmpty(urlLink))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Url Link");
                    return false;
                }

                long userId = -1;
                bool hasExpired = false;
                password = SqlDataAccess.Entities.User.GetPasswordReset(urlLink, out userId, out username, out hasExpired,
                    out error);

                if (error != null)
                    return false;

                if (string.IsNullOrEmpty(password))
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }

        }


        public bool ResetPasswordAfterRequest(
            string urlLink, out string username, out string newPassword,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;
            username = null;
            newPassword = null;

            try
            {
                if (string.IsNullOrEmpty(urlLink))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Url Link");
                    return false;
                }

                long userId = -1;
                bool hasExpired = false;
                newPassword = SqlDataAccess.Entities.User.GetPasswordReset(urlLink, out userId, out username, out hasExpired,
                    out error);

                if (error != null)
                    return false;

                if (string.IsNullOrEmpty(newPassword))
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                string passwordHash;
                string passwordSalt;
                passwordHash = Security.Controller.CreateHashAndSalt(newPassword, out passwordSalt);
                if (string.IsNullOrEmpty(passwordSalt))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Password Salt");
                    return false;
                }
                if (string.IsNullOrEmpty(passwordHash))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Password Hash");
                    return false;
                }

                bool result = SqlDataAccess.Entities.User.ResetPasswordAfterRequest(userId,
                    passwordHash, passwordSalt,
                    out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = ErrorClasses.Controller.GetUnknownError();
                    return false;
                }


                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }


        public bool ValidateInvitationFromMasterUser(
            string email, string urlLink,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                if (string.IsNullOrEmpty(email))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Email");
                    return false;
                }
                if (string.IsNullOrEmpty(urlLink))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Url Link");
                    return false;
                }


                bool result = SqlDataAccess.Entities.User.ValidateInvitationFromMasterUser(email, urlLink,
                    out error);

                if (error != null)
                    return false;


                return result;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }


        public DateTime GetLocalizedServerTime(long userId)
        {
            try
            {

                return ServerDateTimeProvider.Controller.GetLocalizedServerDateTime(
                    SqlDataAccess.Entities.User.GeUserGMT0HoursDiff(userId));
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                return ServerDateTimeProvider.Controller.GetServerDateTime();
            }
        }

        public string GetEmailInvitationUrl(string urlLink,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                if (string.IsNullOrEmpty(urlLink))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Url Link");
                    return null;
                }


                string email = SqlDataAccess.Entities.User.GetEmailFromInvitationLink(urlLink,
                    out error);

                if (error != null)
                    return null;


                return email;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public string GetEmailRegistrationUrl(string urlLink,
            //
            out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                if (string.IsNullOrEmpty(urlLink))
                {
                    error = ErrorClasses.Controller.GetMissingParameterError("Url Link");
                    return null;
                }


                string email = SqlDataAccess.Entities.User.GetEmailFromRegistrationLink(urlLink,
                    out error);

                if (error != null)
                    return null;


                return email;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public Model.User.SerialAndBasicUserInfo GetUserInfoBySerial(string serial) { 
            ErrorClasses.GenericError error;
            return SqlDataAccess.Entities.User.GetUserInfoBySerial(serial, out error);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public System.Nullable<float> GetCalibrationFactorById(long userId, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                float? calibrationFactory =  SqlDataAccess.Entities.User.GetCalibrationFactorById(userId, out error);

                if (error != null)
                    return null;

                return calibrationFactory;

                
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);

                error = ErrorClasses.Controller.GetUnknownError();

                return null;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="CalibrationFactor"></param>
        /// <param name="error"></param>
        /// <returns></returns>
        public bool SetCalibrationFactorById(long userId, float CalibrationFactor, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {

                SqlDataAccess.Entities.User.SetCalibrationFactorById(userId, CalibrationFactor, out error);

                if (error != null)
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);

                error = ErrorClasses.Controller.GetUnknownError();

                return false;
            }
            return true;
        }
    }
}
