using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Business.Interfaces;
using ErrorClasses;
using Model;
using Model.DailyInfo;
using NoSqlDataAccess.Entities;
using ServiceModel;
using ServiceModel.DailyInfo;
using StructureMap;

namespace Business.Plan
{
    public class PlanService : IPlanService
    {

        #region PlanDays Region
        private readonly PlanDayRepository _PlanDayRepository = new PlanDayRepository();

        public Model.PlanDay GetPastPlanDays(long userId, DateTime date)
        {
            var dailyInfoService = ObjectFactory.GetInstance<IDailyInfoService>();
            var viewDataService = ObjectFactory.GetInstance<ITrainingSessionService>();
            var userService = ObjectFactory.GetInstance<IUserService>();

            var trainingSession = viewDataService.GetTrainingSessionItems((int)userId)
                .Where(s => s.DateTrainingStart.Date.CompareTo(date.Date) == 0);// &&
                //            s.DateTrainingEnd.Date.CompareTo(date.Date) == 0);

            var dailyInfos = dailyInfoService.GetDayInfoViewService((int)userId, 1, date);

            var activities = new List<Activity>();

            if (dailyInfos.Hotel[0])
                activities.Add(new Hotel());
            if (dailyInfos.Rest[0])
                activities.Add(new Rest());
            if (dailyInfos.Travel[0])
                activities.Add(new Travel());
            if (dailyInfos.Weight[0] > 0)
                activities.Add(new Weight(dailyInfos.Weight[0]));
            if (dailyInfos.Sick[0] == true)
                activities.Add(new Sick());

            trainingSession.ToList().ForEach(session =>
            {
                var durationTimespan = session.DateTrainingEnd - session.DateTrainingStart;
                var duration = 0;
                if (durationTimespan.TotalMinutes >= 30 && durationTimespan.TotalMinutes < 45) duration = 30;
                if (durationTimespan.TotalMinutes >= 45 && durationTimespan.TotalMinutes < 60) duration = 30;
                if (durationTimespan.TotalMinutes >= 60 && durationTimespan.TotalMinutes < 90) duration = 90;
                if (durationTimespan.TotalMinutes >= 90 && durationTimespan.TotalMinutes <= 120) duration = 120;


                GenericError error;
                var sportTypes = userService.GetMasterUserSportsExtended(userId, out error);
                var selectedSportType = sportTypes.SportsList.SingleOrDefault(sport => sport.Id.Equals((long)session.SportId));
                int? s = ((selectedSportType != null) ? (int?)selectedSportType.Id : null);

               // activities.Add(new Training()
               // {
               //    DayPeriod = Convert.ToInt32(session.DayPeriod),
               //    Duration = durationTimespan.Minutes,
               //    EffortType = session.EffortId,
               //    FocusOn = session.TrainingTypeId,
               //    SportType = s
               //});
                var t = new Training();
                t.DayPeriod = Convert.ToInt32(session.DayPeriod);
                t.Duration = durationTimespan.Minutes;
                t.EffortType = session.EffortId;
                t.FocusOn = session.TrainingTypeId;
                t.SportType = s;
                t.Notes = session.Activity;
                //t.Notes = selectedSportType.LocalizedName;
                activities.Add(t);
                
            });

            var pastPlanDay = new PlanDay(date.Year, date.Month, date.Day, userId);
            pastPlanDay.Activities = activities;

            return pastPlanDay;
        }

        public bool Exists(long Id)
        {
            return _PlanDayRepository.Exists(Id);
        }

        public Model.PlanDay GetPlanDay(long Id, long masterUserId)
        {
            var planDay = _PlanDayRepository.GetPlanDay(Id);
            if (planDay == null || planDay.GetDate() < ServerDateTimeProvider.Controller.GetServerDateTime().Date)
            {
                return new Model.PlanDay(Id);
            }
            else
            {
                return planDay;
            }
        }

        public Model.PlanDay GetPlanDay(int year, int month, int day, long masterUserId, long userId)
        {
            var date = new DateTime(year, month, day);
            if (date < ServerDateTimeProvider.Controller.GetServerDateTime().Date)
            {
                return GetPastPlanDays(userId, date);
            }
            else
            {
                var planDay = _PlanDayRepository.GetPlanDay(year, month, day, masterUserId);
                if (planDay == null)
                {
                    return new Model.PlanDay(year, month, day, userId);
                }
                else
                {
                    return planDay;
                }
            }
        }

        public void Save(Model.PlanDay planDay)
        {
            if (planDay.GetDate() < DateTime.Now.Date)
            {
                throw new Exception("It's not allowed to add/change sessions in the past");
            }
            else
            {
                _PlanDayRepository.Save(planDay);
            }
        }

        public IEnumerable<Model.PlanDay> LoadPlanDays(int year, int month, long masterUserId)
        {
            return _PlanDayRepository.GetAsQueryable<Model.PlanDay>().Where(d => d.Year == year)
                                                       .Where(d => d.Month == month)
                                                       .Where(d => d.MasterUserID == masterUserId);
        }

        public void DeletePlanDays(long user_id, IEnumerable<Model.PlanDay> days_to_delete)
        {
            _PlanDayRepository.DeleteDays(days_to_delete);
        }

        public void DeletePlanDays(IEnumerable<long> ids_to_delete)
        {
            _PlanDayRepository.DeleteDays(ids_to_delete);
        }
        #endregion



        #region Plan Profile Region
        private PlanProfileRepository _RepoPlanProfileRepository = new PlanProfileRepository();

        public void Save(Model.PlanProfile profile)
        {
            _RepoPlanProfileRepository.Save(profile);
        }

        public Model.PlanProfile Load(MongoDB.Bson.ObjectId profileId)
        {
            return _RepoPlanProfileRepository.GetAsQueryable<Model.PlanProfile>().FirstOrDefault(p => p.Id == profileId.ToString());
        }

        public Model.PlanProfile Load(string profile_name, long user_id)
        {
            return _RepoPlanProfileRepository.GetAsQueryable<Model.PlanProfile>()
                                    .FirstOrDefault(p => p.Name.ToString() == profile_name && (long)p.UserID == user_id);
        }

        public Model.PlanProfile GetProfile(string profileId, long userId)
        {
            return _RepoPlanProfileRepository.GetAsQueryable<Model.PlanProfile>()
                                    .Where(p => (long)p.UserID == userId)
                                    .SingleOrDefault(p => p.Id == profileId);
        }

        public IEnumerable<Model.PlanProfile> GetProfilesByUser(long userId)
        {
            return _RepoPlanProfileRepository.GetAsQueryable<Model.PlanProfile>().Where(p => (long)p.UserID == userId);
        }

        public void DeleteProfile(string profileId)
        {
            _RepoPlanProfileRepository.Delete(profileId.ToString());
        }
        #endregion



        #region Waterfall User Preferences Region
        private WaterfallUserPreferencesRepository _waterfallRepo = new WaterfallUserPreferencesRepository();

        public IEnumerable<WaterfallUserPreferences> GetUserPreferences(long userId)
        {
            return _waterfallRepo.GetUserPreferences(userId);
        }

        public void Save(Model.WaterfallUserPreferences preferences)
        {
            _waterfallRepo.Save(preferences);
        }
        #endregion
    }
}
