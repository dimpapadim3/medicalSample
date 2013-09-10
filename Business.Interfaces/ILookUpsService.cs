using System;
using ErrorClasses;
using ServiceModel;

namespace Business.Interfaces
{
    public interface ILookUpsService
    {
        Countries GetCountries(string defaultLocale, out GenericError error);
        TimeZones GetTimeZones(string defaultLocale, out GenericError error);
        Languages GetLanguages(string defaultLocale, out GenericError error);
        MetricTypes GetMetricTypes(string defaultLocale, out GenericError error);


        //Model.User.User GetUserInfo(string Username, out GenericError err);

        ///bool GetUserIsMaster(string Username, out GenericError err);

        CoreTeamUserRoles GetCoreTeamUserRoles(string p, out GenericError err);

        Sports GetSports(string p, out GenericError error);

        DaysOfTheWeek GetDaysOfTheWeek(string p, out GenericError error);
    }
}