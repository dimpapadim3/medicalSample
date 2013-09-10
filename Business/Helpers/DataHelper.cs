using System;
using System.Collections.Generic;
using System.Linq;
using Business.Interfaces;
using DataAccess.Interfaces;
using Model;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.Helpers
{
    /// <summary>
    /// provides data that Are persisted in specific time intervals witch are determined by PeriodSelectionStrategy
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public abstract class DataHelper<TService, TModel> where TModel : class, IDateTracable, IUserQueryable
    {
        public PeriodTrainingDataStrategy PeriodSelectionStrategy { get; set; }
        //Dependency injection
        public IRepositoryBase<TModel> Repository { get; set; }
        public INoSqlFinalValuesRepositoryBase<TModel> FinalRepository { get; set; }

        public virtual IList<TService> GetViewDataForTimePeriod(int userId, DateTime now, int numberOfMonthsToDisplay)
        {
            var allMontlyEnergyInfo = DoGetFinalDataToDisplay(userId, now, numberOfMonthsToDisplay);
            InitializePeriodStrategy(userId);
            var monthlyTimeSpans = PeriodSelectionStrategy.GetTimeSpans(now, numberOfMonthsToDisplay);
            var energyLevelInfoViewData = FillRecordsOnEmptyTimeSpans(allMontlyEnergyInfo, monthlyTimeSpans);

            return energyLevelInfoViewData;
        }

        protected virtual List<TModel> DoGetFinalDataToDisplay(int userId, DateTime now, int numberOfMonthsToDisplay)
        {
            //the current month 
            var currentMonthEnergyInfo = GetCurrentModelType(userId, now, numberOfMonthsToDisplay);

            var lastMonthsEnergyLevelInfo = GetRemainingTimePeriodModelTypes(userId, now, numberOfMonthsToDisplay);

            //aggregate lists
            var allMontlyEnergyInfo = new List<TModel>();
            allMontlyEnergyInfo.Add(currentMonthEnergyInfo);
            allMontlyEnergyInfo.AddRange(lastMonthsEnergyLevelInfo);
            return allMontlyEnergyInfo;
        }

        protected virtual IEnumerable<TModel> GetRemainingTimePeriodModelTypes(int userId, DateTime now, int numberOfMonthsToDisplay)
        {
            IRepositoryBase<TModel> repository = FinalRepository ?? Repository;
            return repository.GetAsQueryable<TModel>().ToList()
           .Where(d => d.UserId == userId && DoGetInTimePeriodFilterComparision(now, numberOfMonthsToDisplay, d));
        }

        protected virtual bool DoGetInTimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, TModel d)
        {
            return PeriodSelectionStrategy.TimePeriodFilterComparision(now, numberOfMonthsToDisplay, d);
        }

        protected virtual TModel GetCurrentModelType(int userId, DateTime now, int numberOfMonthsToDisplay)
        {
            var entities = Repository.GetAsQueryable<TModel>().Where(m => m.UserId == userId).ToList();

            if (entities.Any())
            {
                entities.Sort((d1, d2) => DateTime.Compare(d1.Date, d2.Date));
                return entities.Last();
            }
            return null;
        }

        protected abstract void InitializePeriodStrategy(int userId);

        public List<TService> FillRecordsOnEmptyTimeSpans(List<TModel> intialList, IEnumerable<TimeSpan> timeSpans)
        {
// ReSharper disable CompareNonConstrainedGenericWithNull
            return timeSpans.Select(dailyTimeSpan => DoGetServiceTypeInTimeSpan(intialList, dailyTimeSpan)).Where(serviceType => serviceType != null).ToList();
// ReSharper restore CompareNonConstrainedGenericWithNull
        }

        protected virtual TService DoGetServiceTypeInTimeSpan(List<TModel> intialList, TimeSpan dailyTimeSpan)
        {
            TimeSpan span = dailyTimeSpan;

            if (intialList != null && intialList.All(s => s != null))
            {
                var dailyInfoInTimeSpan = intialList.FindAll(energyevel => { return span.InTimeIncluded(energyevel.Date); });
                var serviceType = dailyInfoInTimeSpan.Count > 0
                                      ? InitializeNewServiceType(dailyInfoInTimeSpan, span)
                                      : CreateEmptyServiceType(span);
                return serviceType;
            }
            return CreateEmptyServiceType(span);
        }


        public virtual TService InitializeNewServiceType(List<TModel> entityInTimeSpan, TimeSpan span)
        {
            return InitializeNewServiceType(entityInTimeSpan.FirstOrDefault(), span);
        }
        public abstract TService InitializeNewServiceType(TModel dailyInfoInTimeSpan, TimeSpan span);
        public abstract TService CreateEmptyServiceType(TimeSpan span);
    }
    /// <summary>
    /// this provides data that are not persisted in specific time intervals 
    /// </summary>
    /// <typeparam name="TService"></typeparam>
    /// <typeparam name="TModel"></typeparam>
    public abstract class DynamicDataHelper<TService, TModel> : DataHelper<TService, TModel> where TModel : class, IDateTracable, IUserQueryable
    {
        protected override List<TModel> DoGetFinalDataToDisplay(int userId, DateTime now, int numberOfMonthsToDisplay)
        {
            var lastMonthsEnergyLevelInfo = GetRemainingTimePeriodModelTypes(userId, now, numberOfMonthsToDisplay);

            InitializePeriodStrategy(userId);

            return lastMonthsEnergyLevelInfo.ToList();

        }

    }

    public abstract class DailyyDataHelper<TService, TModel> : DynamicDataHelper<TService, TModel> where TModel : class, IDateTracable, IUserQueryable
    {
        protected override bool DoGetInTimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, TModel d)
        {
            return DataUtils.CompareDatesAfterStartDate(now.AddDays(-numberOfMonthsToDisplay), d.Date);

        }

        protected override void InitializePeriodStrategy(int userid)
        {
            PeriodSelectionStrategy = DataUtils.GetPeriodTrainingDataStrategy(userid, Common.Constants.DAY_PERIOD);

        }


    }

    public abstract class WeeklyDataHelper<TService, TModel> : DynamicDataHelper<TService, TModel> where TModel : class, IDateTracable, IUserQueryable
    {
        protected override bool DoGetInTimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, TModel d)
        {
            return DataUtils.CompareDatesAfterStartDate(now.AddDays(-numberOfMonthsToDisplay * 7), d.Date);
        }

        protected override void InitializePeriodStrategy(int userid)
        {
            PeriodSelectionStrategy = new WeeklyTrainigDataSummary { FirstDayOfTheWeek = DataUtils.GetFirstDayOfTheWeekFromUserId(userid) };

        }

    }

    public abstract class MonthlyDynamicDataHelper<TService, TModel> : DynamicDataHelper<TService, TModel> where TModel : class, IDateTracable, IUserQueryable
    {
        protected override bool DoGetInTimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, TModel d)
        {
            return DataUtils.CompareDatesAfterStartDate(now.AddMonths(-numberOfMonthsToDisplay), d.Date.Date);
        }

        protected override void InitializePeriodStrategy(int userid)
        {
            PeriodSelectionStrategy = new MonthlyTrainigDataSummary();

        }

    }
    public abstract class MonthlyDataHelper<TService, TModel> : DataHelper<TService, TModel> where TModel : class, IUserQueryable, IDateTracable
    {
        protected override bool DoGetInTimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, TModel d)
        {
            return DataUtils.CompareDatesAfterStartDate(now.AddMonths(-numberOfMonthsToDisplay), d.Date.Date);
        }

        protected override void InitializePeriodStrategy(int userid)
        {
            PeriodSelectionStrategy = new MonthlyTrainigDataSummary();

        }

    }

    public abstract class YearlyDynamicDataHelper<TService, TModel> : DynamicDataHelper<TService, TModel> where TModel : class, IDateTracable, IUserQueryable
    {
        protected override bool DoGetInTimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, TModel d)
        {
            return DataUtils.CompareDatesAfterStartDate(now.AddYears(-numberOfMonthsToDisplay), d.Date);
        }

        protected override void InitializePeriodStrategy(int userid)
        {
            PeriodSelectionStrategy = new YearlyTrainigDataSummary();
        }


    }
    public abstract class YearlyDataHelper<TService, TModel> : DataHelper<TService, TModel> where TModel : class, IDateTracable, IUserQueryable
    {
        protected override bool DoGetInTimePeriodFilterComparision(DateTime now, int numberOfMonthsToDisplay, TModel d)
        {
            return DataUtils.CompareDatesAfterStartDate(now.AddYears(-numberOfMonthsToDisplay), d.Date);
        }

        protected override void InitializePeriodStrategy(int userid)
        {
            PeriodSelectionStrategy = new YearlyTrainigDataSummary();
        }


    }
}
