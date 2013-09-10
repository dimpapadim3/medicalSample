using System;
using System.Collections.Generic;
using System.Linq;
using Business.EnergyLevelInfo.Helpers;
using Business.Helpers;
using Business.Interfaces;
using Common;
using DataAccess.Interfaces;
using ErrorClasses;
using Model.EnergyLevelInfo;
 using Controller = ServerDateTimeProvider.Controller;

namespace Business.EnergyLevelInfo
{
    internal class EnergyLevelService : IEnergyLevelInfoService
    {
        /// <summary>
        ///     Handles Monthly Questionnaire Summary Data
        /// </summary>
        public IDataAgreggationHelperTemplate<MonthlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>
            MontlyInfoHandlingTemplate { get; set; }

        /// <summary>
        ///     Handles Yearly Questionnaire Summary Data
        /// </summary>
        public IDataAgreggationHelperTemplate<YearlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>
            YearlyInfoHandlingTemplate { get; set; }
         

        #region Repositories

        public IRepositoryBase<DailyEnergyLevel> DailyEnergyLevelRepository { get; set; }
        public IEventsService EventsService { get; set; }
      
        #endregion

        public bool InsertDailyEnergyLevel(DailyEnergyLevel energyLevel)
        {
            try
            {
                DateTime now = Controller.GetServerDateTime();
                bool todayEnergyLevelExist =DailyEnergyLevelRepository.GetAsQueryable<DailyEnergyLevel>().ToList()
                                              .FirstOrDefault(e =>(e.UserId == energyLevel.UserId) && (now.Day == e.Date.Day) &&
                                                  (now.Month == e.Date.Month) && (now.Year == e.Date.Year)) != null;

                if (todayEnergyLevelExist)
                    return false;

                GenericError error;
                DailyEnergyLevelRepository.InsertEntity(out error, energyLevel);

                if (error == null)
                {
                    MontlyInfoHandlingTemplate.UpdateEntityRecords(energyLevel);

                    YearlyInfoHandlingTemplate.UpdateEntityRecords(energyLevel);

                    EventsService.UpdateUserDates(energyLevel.UserId, null, Controller.GetServerDateTime());
                }

                return error == null;
             }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        #region ViewInfo

        public IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetEnergyLevelsForTimePeriod(int userId, DateTime now,
                                                                                            int period)
        { 
            if (period == 0) return GetDailyInfoView(userId, now);
            if (period == 1) return GetWeeklyInfoView(userId, now);
            if (period == 2) return GetMonthlyInfoView(userId, now);
            if (period == 3) return GetYearlyInfoView(userId, now);
            throw new Exception("Wrong Period Number");
  
        }

        public bool SubmitEnergyLevel(int userId, byte energyLevel)
        {
            return InsertDailyEnergyLevel(CreateNewDailyEnergyLevel(userId, energyLevel));
        }

        public ServiceModel.EnergyLevel.EnergyLevelInfo GetSingleEnergyLevelForTimePeriod(int userId, DateTime now, int period)
        {
            if (period == 0) return GetDailyInfoView(userId, now, 1).FirstOrDefault();
            if (period == 1) return GetWeeklyInfoView(userId, now, 1).FirstOrDefault();
            if (period == 2) return GetMonthlyInfoView(userId, now, 1).FirstOrDefault();
            if (period == 3) return GetYearlyInfoView(userId, now, 1).FirstOrDefault();
            throw new Exception("Wrong Period Number");
        }

        public IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetDailyInfoView(long userId, DateTime now)
        {
            return GetDailyInfoView(userId, now, Constants.NUMBER_OF_DAY_RECORDS);
        }

        public IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetWeeklyInfoView(int userId, DateTime now)
        {
            return GetWeeklyInfoView(userId, now, Constants.NUMBER_OF_WEEK_RECORDS);
        }

        private IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetMonthlyInfoView(int userId, DateTime now)
        {
            return GetMonthlyInfoView(userId, now, Constants.NUMBER_OF_MONTH_RECORDS);
        }

        private IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetYearlyInfoView(int userId, DateTime now)
        {
            return GetYearlyInfoView(userId, now, Constants.NUMBER_OF_YEAR_RECORDS);
        }

        private IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetDailyInfoView(long userId, DateTime now,
                                                                                int numberOfDaysToDisplay)
        {
            var helper = new DailyyDataHelper { Repository = DailyEnergyLevelRepository };
            return helper.GetViewDataForTimePeriod((int)userId, now, numberOfDaysToDisplay).ToList();
        }

        private IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetWeeklyInfoView(int userId, DateTime now,
                                                                                 int numberOfWeeksToDisplay)
        {
            var helper = new WeeklyDataHelper { Repository = DailyEnergyLevelRepository };
            return helper.GetViewDataForTimePeriod(userId, now, numberOfWeeksToDisplay).ToList();
        }

        private IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetMonthlyInfoView(int userId, DateTime now,
                                                                                  int numberOfMonthsToDisplay)
        { 
            return MontlyInfoHandlingTemplate.GetInfoView(userId, now, numberOfMonthsToDisplay);
        }

        private IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetYearlyInfoView(int userId, DateTime now,
                                                                                 int numberOfYearsToDisplay)
        {
            return YearlyInfoHandlingTemplate.GetInfoView(userId, now, numberOfYearsToDisplay);
        }

        #endregion

        #region Daily

        private DailyEnergyLevel GetLattestDailyEnergyLevel(out GenericError error)
        {
            List<DailyEnergyLevel> lattestDailyEnergyLevel;

            try
            {
                lattestDailyEnergyLevel = DailyEnergyLevelRepository.GetEntities(out error, d => true).ToList();
                lattestDailyEnergyLevel.Sort((d1, d2) => DateTime.Compare(d1.Date, d2.Date));
            }
            catch (Exception ex)
            {
                Logger.Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                throw;
            }
            return lattestDailyEnergyLevel.Count > 0 ? lattestDailyEnergyLevel.Last() : new DailyEnergyLevel();
        }

        protected DailyEnergyLevel CreateNewDailyEnergyLevel(int userId, byte energyLevel)
        {
            DateTime maxDate;
            DateTime minDate;
            int count;

            if (!IsTheFirstEntry())
            {
                GenericError error;
                minDate = GetLattestDailyEnergyLevel(out error).MinDate;
                try
                {
                    GenericError error2;
                    maxDate = GetLattestDailyEnergyLevel(out error2).MaxDate;
                }
                catch (Exception ex)
                {
                    Logger.Controller.LogError(ex);
                    ErrorClasses.Controller.GetUnknownError();
                    throw;
                }
                GenericError error1;
                count = GetLattestDailyEnergyLevel(out error1).Count + 1;
            }
            else
            {
                maxDate = minDate = GetDateTimeNow();
                count = 1;
            }
            return new DailyEnergyLevel
                {
                    UserId = userId,
                    Date = GetDateTimeNow(),
                    MinDate = minDate,
                    MaxDate = maxDate,
                    EnergyLevel = energyLevel,
                    Count = count
                };
        }

        private bool IsTheFirstEntry()
        {
            GenericError error;
            List<DailyEnergyLevel> entries = DailyEnergyLevelRepository.GetEntities(out error, d => true);
            return entries == null || entries.Count == 0;
        }

        private DateTime GetDateTimeNow()
        {
            return Controller.GetServerDateTime();
        }

        #endregion
    }
}