using System;
using System.Linq;
using Business.Interfaces;
using Common;
using ErrorClasses;
using Model.StaticLookups;
using TimeZone = Model.StaticLookups.TimeZone;

namespace Business.Events
{
    public class EventsService : IEventsService
    {
        public bool GetPopupEvent(long id, DateTime? userCreationDate,
                                  out bool energyEvent, out bool questionnaireEvent, out bool weightEvent,
                                  out GenericError error)
        {
            error = null;
            energyEvent = false;
            questionnaireEvent = false;
            weightEvent = false;

            if (id <= 0)
                return false;

            try
            {
                DayOfTheWeek dayOfTheWeek;
                TimeZone timeZone;
                DateTime? createdOn;
                bool result = SqlDataAccess.Entities.User.GeMasterUserInfoForEvents(id,
                                                                                    out dayOfTheWeek, out timeZone,
                                                                                    out createdOn, out error);

                if (error != null)
                    return false;

                if (!result)
                {
                    error = Controller.GetUnknownError();
                    return false;
                }

                if ((dayOfTheWeek == null) || (timeZone == null) || (createdOn == null))
                {
                    error = Controller.GetUnknownError();
                    return false;
                }

                DateTime? latestQuestionnaireDate;
                DateTime? latestEnergyLevelDate;
                DateTime? latestIncludedDate;
                GetUserDates(id, out latestIncludedDate, out latestQuestionnaireDate, out latestEnergyLevelDate);

                DateTime localizedServerDateTime =
                    ServerDateTimeProvider.Controller.GetLocalizedServerDateTime(timeZone.HoursDiff);
                DateTime localizedlatestQuestionnaireDate = latestQuestionnaireDate == null
                                                                ? DateTime.MinValue
                                                                : latestQuestionnaireDate.Value.AddHours(
                                                                    -Constants.SERVER_TIME_ZONE)
                                                                                         .AddHours(timeZone.HoursDiff);
                DateTime localizedlatestEnergyLevelDate = latestEnergyLevelDate == null
                                                              ? DateTime.MinValue
                                                              : latestEnergyLevelDate.Value.AddHours(
                                                                  -Constants.SERVER_TIME_ZONE)
                                                                                     .AddHours(timeZone.HoursDiff);
                DateTime localizedlatestIncludedDate = latestIncludedDate == null
                                                           ? DateTime.MinValue
                                                           : latestIncludedDate.Value.AddHours(
                                                               -Constants.SERVER_TIME_ZONE).AddHours(timeZone.HoursDiff);


                DateTime qDate = GetNextQuestionnaireDate(dayOfTheWeek.Day, localizedServerDateTime,
                                                          localizedlatestQuestionnaireDate,
                                                          localizedlatestIncludedDate);
                if (qDate <= localizedServerDateTime)
                    questionnaireEvent = true;

                if //((!questionnaireEvent) &&
                    (!DatePartIsEqual(localizedServerDateTime, localizedlatestEnergyLevelDate)) //)
                    energyEvent = true;

                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = Controller.GetUnknownError();
                return false;
            }
        }

        bool IEventsService.UpdateUserDates(long userId, DateTime? lastQ, DateTime? lastEn)
        {
            return UpdateUserDates(userId, lastQ, lastEn);
        }

        public bool GetUserDates(long id, out DateTime? lastIncl,
                                 out DateTime? lastQ, out DateTime? lastEn)
        {
            lastEn = null;
            lastQ = null;
            lastIncl = null;

            try
            {
                return SqlDataAccess.Entities.User.GetUserDates(id, out lastIncl, out lastQ, out lastEn);
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                return false;
            }
        }

        public void ClearUserDates(int userId)
        {
            var db = new SqlDataAccess.Entities.View.ViewUnitOfWork();
            var m = db.User.FirstOrDefault(u => u.Id == userId);
            var info = db.MasterUserInfo.FirstOrDefault(u => u.Id == m.MasterUserInfoId);

            if (info != null)
            {
                info.LastAnsweredEnergyDate = null;
                info.LastAnsweredQuestionnaireDate = null;
                info.LastIncludedInQuestionnaireDate = null;
                db.SaveChanges();
             }
         }

        private static DateTime GetNextQuestionnaireDate(DayOfWeek dayOfWeek, DateTime currentDate,
                                                         DateTime latestQuestionnaireDate,
                                                         DateTime latestIncludedDate)
        {
            DateTime res = currentDate;

            if (latestQuestionnaireDate == DateTime.MinValue)
            {
                while (res.DayOfWeek != dayOfWeek)
                    res = res.AddDays(1);
            }
            else
            {
                if (latestIncludedDate == DateTime.MinValue)
                    throw new Exception("Latest included date is undefined!");

                res = latestQuestionnaireDate.AddDays(1);

                while (res.DayOfWeek != dayOfWeek)
                    res = res.AddDays(1);

                if (((int) ((new DateTime(res.Year, res.Month, res.Day, 0, 0, 0) -
                             new DateTime(latestIncludedDate.Year, latestIncludedDate.Month, latestIncludedDate.Day))
                               .TotalDays)) <= 7)
                    res = res.AddDays(7);
            }

            return new DateTime(res.Year, res.Month, res.Day, 0, 0, 0);
        }

        private bool DatePartIsEqual(DateTime dt1, DateTime dt2)
        {
            return
                (dt1.Year == dt2.Year) &&
                (dt1.Month == dt2.Month) &&
                (dt1.Day == dt2.Day);
        }


        private static bool UpdateUserDates(long id, DateTime? lastQ, DateTime? lastEn)
        {
            try
            {
                DateTime? lastIncl = null;
                if (lastQ != null)
                {
                    DayOfTheWeek dayOfTheWeek;
                    TimeZone timeZone;
                    GenericError error;
                    DateTime? createdOn;
                    bool result = SqlDataAccess.Entities.User.GeMasterUserInfoForEvents(id, out dayOfTheWeek,
                                                                                        out timeZone, out createdOn,
                                                                                        out error);

                    if (!result)
                        return false;

                    if (dayOfTheWeek == null)
                        return false;

                    DayOfWeek dw = dayOfTheWeek.Day;
                    DateTime tempDt = lastQ.Value;

                    if (lastQ.Value.DayOfWeek == dw)
                    {
                        lastIncl = lastQ.Value.AddDays(-1);
                    }
                    else
                    {
                        while (tempDt.DayOfWeek != dw)
                        {
                            tempDt = tempDt.AddDays(-1);
                        }
                        lastIncl = tempDt.AddDays(-1);
                    }
                }

                bool res = SqlDataAccess.Entities.User.UpdateUserDates(id, lastIncl, lastQ, lastEn);

                if (!res)
                    return false;

                return true;
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                return false;
            }
        }
    }
}