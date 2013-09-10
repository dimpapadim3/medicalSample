using Business.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ErrorClasses;
using ServiceModel;

namespace Business
{
    public  class Lookups :ILookUpsService
    {

        public  ServiceModel.CoreTeamUserRoles GetCoreTeamUserRoles(string locale, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                List<Model.StaticLookups.CoreTeamUserRole> modelRes =
                    SqlDataAccess.Entities.CoreTeamUserRole.GetCoreTeamUserRolesFromCache(out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                ServiceModel.CoreTeamUserRoles res = new ServiceModel.CoreTeamUserRoles();
                res.Locale = locale;
                res.CoreTeamUserRolesList = new List<ServiceModel.CoreTeamUserRole>();
                foreach (Model.StaticLookups.CoreTeamUserRole item in modelRes)
                {
                    res.AddItem(item.Name);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        Sports ILookUpsService.GetSports(string p, out GenericError error)
        {
            return GetSports(p, out error);
        }

        public  ServiceModel.Countries GetCountries(string locale, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                List<Model.StaticLookups.Country> modelRes =
                    SqlDataAccess.Entities.Country.GetCountriesFromCache(out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                ServiceModel.Countries res = new ServiceModel.Countries();
                res.Locale = locale;
                res.CountriesList = new List<ServiceModel.Country>();
                foreach (Model.StaticLookups.Country item in modelRes)
                {
                    res.AddItem(item.Name);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public  ServiceModel.Genders GetGenders(string locale, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                List<Model.StaticLookups.Gender> modelRes =
                    SqlDataAccess.Entities.Gender.GetGendersFromCache(out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                ServiceModel.Genders res = new ServiceModel.Genders();
                res.Locale = locale;
                res.GendersList = new List<ServiceModel.Gender>();
                foreach (Model.StaticLookups.Gender item in modelRes)
                {
                    res.AddItem(item.Name);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public  ServiceModel.HeightTypes GetHeightTypes(string locale, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                List<Model.StaticLookups.HeightType> modelRes =
                    SqlDataAccess.Entities.HeightType.GetHeightTypesFromCache(out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                ServiceModel.HeightTypes res = new ServiceModel.HeightTypes();
                res.Locale = locale;
                res.HeightTypesList = new List<ServiceModel.HeightType>();
                foreach (Model.StaticLookups.HeightType item in modelRes)
                {
                    res.AddItem(item.Name);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public  ServiceModel.Languages GetLanguages(string locale, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                List<Model.StaticLookups.Language> modelRes =
                    SqlDataAccess.Entities.Language.GetLanguagesFromCache(out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                ServiceModel.Languages res = new ServiceModel.Languages();
                res.Locale = locale;
                res.LanguagesList = new List<ServiceModel.Language>();
                foreach (Model.StaticLookups.Language item in modelRes)
                {
                    res.AddItem(item.Name, item.Locale);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public  ServiceModel.MetricTypes GetMetricTypes(string locale, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                List<Model.StaticLookups.MetricType> modelRes =
                    SqlDataAccess.Entities.MetricType.GetMetricTypesFromCache(out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                ServiceModel.MetricTypes res = new ServiceModel.MetricTypes();
                res.Locale = locale;
                res.MetricTypesList = new List<ServiceModel.MetricType>();
                foreach (Model.StaticLookups.MetricType item in modelRes)
                {
                    res.AddItem(item.Name);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public static ServiceModel.Sports GetSports(string locale, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                List<Model.StaticLookups.Sport> modelRes =
                    SqlDataAccess.Entities.Sport.GetSportsFromCache(out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                ServiceModel.Sports res = new ServiceModel.Sports();
                res.Locale = locale;
                res.SportsList = new List<ServiceModel.Sport>();
                foreach (Model.StaticLookups.Sport item in modelRes)
                {
                    res.AddItem(item.Id,item.Name);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public  ServiceModel.TemperatureTypes GetTemperatureTypes(string locale, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                List<Model.StaticLookups.TemperatureType> modelRes =
                    SqlDataAccess.Entities.TemperatureType.GetTemperatureTypesFromCache(out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                ServiceModel.TemperatureTypes res = new ServiceModel.TemperatureTypes();
                res.Locale = locale;
                res.TemperatureTypesList = new List<ServiceModel.TemperatureType>();
                foreach (Model.StaticLookups.TemperatureType item in modelRes)
                {
                    res.AddItem(item.Name);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public  ServiceModel.TimeZones GetTimeZones(string locale, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                List<Model.StaticLookups.TimeZone> modelRes =
                    SqlDataAccess.Entities.TimeZone.GetTimeZonesFromCache(out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                ServiceModel.TimeZones res = new ServiceModel.TimeZones();
                res.Locale = locale;
                res.TimeZonesList = new List<ServiceModel.TimeZone>();
                foreach (Model.StaticLookups.TimeZone item in modelRes)
                {
                    res.AddItem(item.Name, item.HoursDiff);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public  ServiceModel.WeightTypes GetWeightTypes(string locale, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                List<Model.StaticLookups.WeightType> modelRes =
                    SqlDataAccess.Entities.WeightType.GetWeightTypesFromCache(out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                ServiceModel.WeightTypes res = new ServiceModel.WeightTypes();
                res.Locale = locale;
                res.WeightTypesList = new List<ServiceModel.WeightType>();
                foreach (Model.StaticLookups.WeightType item in modelRes)
                {
                    res.AddItem(item.Name);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

        public  ServiceModel.DaysOfTheWeek GetDaysOfTheWeek(string locale, out ErrorClasses.GenericError error)
        {
            error = null;

            try
            {
                List<Model.StaticLookups.DayOfTheWeek> modelRes =
                    SqlDataAccess.Entities.DayOfTheWeek.GetDaysOfTheWeekFromCache(out error);

                if (error != null)
                    return null;
                if (modelRes == null)
                    return null;

                ServiceModel.DaysOfTheWeek res = new ServiceModel.DaysOfTheWeek();
                res.Locale = locale;
                res.DaysOfTheWeekList = new List<ServiceModel.DayOfTheWeek>();
                foreach (Model.StaticLookups.DayOfTheWeek item in modelRes)
                {
                    res.AddItem(item.Name);
                }

                return res;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return null;
            }
        }

    }
}
