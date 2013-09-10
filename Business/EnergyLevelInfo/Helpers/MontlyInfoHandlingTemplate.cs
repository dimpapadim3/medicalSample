using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using DataAccess.Interfaces;
using ErrorClasses;
using Model.EnergyLevelInfo;
using Controller = Logger.Controller;

namespace Business.EnergyLevelInfo.Helpers
{
    public class MonthlyInfoHandlingTemplate :
        DataAgreggationHelperTemplate<MonthlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>
    {
        public IRepositoryBase<DailyEnergyLevel> DailyEnergyLevelRepository { get; set; }

        public IRepositoryBase<MonthlyEnergyLevel> MonthlyEnergyLevelRepository { get; set; }

        public INoSqlFinalValuesRepositoryBase<MonthlyEnergyLevel> FinalMonthlyDailyEnergyLevelRepository { get; set; }

        public override ServiceModel.EnergyLevel.EnergyLevelInfo CreateEmptyServiceModel()
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo();
        }

        public override bool IsTheFirstEntryForUser(long userId)
        {
            GenericError error;
            List<MonthlyEnergyLevel> entries = MonthlyEnergyLevelRepository.GetEntities(out error, d => d.UserId == userId);
            return entries == null || entries.Count == 0;
        }

        public override MonthlyEnergyLevel CreateNewModelItem(DailyEnergyLevel energyLevel)
        {
            return new MonthlyEnergyLevel
                {
                    UserId = energyLevel.UserId,
                    MonthNumber = energyLevel.Date.Month,
                    Date = energyLevel.Date,
                    Year = energyLevel.Date.Year
                };
        }

        public override MonthlyEnergyLevel GetLattestRecord(long userId)
        {
            GenericError error;
            List<MonthlyEnergyLevel> records = MonthlyEnergyLevelRepository.GetEntities(out error, d => d.UserId == userId);
            if (records.Any())
            {
                records.Sort((d1, d2) => DateTime.Compare(d1.Date, d2.Date));
                return records.Last();
            }

            return new MonthlyEnergyLevel();
        }

        public override void InitializeRunningTotals(MonthlyEnergyLevel monthlyEnergyLevel, DailyEnergyLevel energyLevel)
        {
            monthlyEnergyLevel.Count = 1;
            monthlyEnergyLevel.Average = energyLevel.EnergyLevel;
            monthlyEnergyLevel.SumTotal = energyLevel.EnergyLevel;
            // monthlyEnergyLevel.DailyEnergyLevel.Add(energyLevel);
        }

        public override bool IsTheFirstRecordOfPeriod(MonthlyEnergyLevel latestMonthlyRecord,
                                                      DailyEnergyLevel energyLevel)
        {
            return latestMonthlyRecord.Date.Month < energyLevel.Date.Month;
        }

        public override void ComputeFromLastRecord(DailyEnergyLevel energyLevel, MonthlyEnergyLevel yearlyEnergyLevel,
                                                   MonthlyEnergyLevel latestMonthlyRecord)
        {
            yearlyEnergyLevel.Count = latestMonthlyRecord.Count + 1;

            yearlyEnergyLevel.SumTotal = latestMonthlyRecord.SumTotal + energyLevel.EnergyLevel;

            var newAverage = yearlyEnergyLevel.SumTotal / yearlyEnergyLevel.Count;
            yearlyEnergyLevel.Average = Math.Round(newAverage, MidpointRounding.AwayFromZero);
        }

        public override bool IsTheLastRecordOfthePeriod(MonthlyEnergyLevel latestMonthlyRecord,
                                                        DailyEnergyLevel energyLevel)
        {
            return latestMonthlyRecord.Date.Month < energyLevel.Date.Month;
        }

        public override void InsertEntityToFinalRecords(out GenericError error, MonthlyEnergyLevel latestMonthlyRecord)
        {
            FinalMonthlyDailyEnergyLevelRepository.InsertEntity(out error, latestMonthlyRecord);
        }

        public override bool AddNewRecordToDataBase(MonthlyEnergyLevel energyLevel)
        {
            try
            {
                GenericError error;
                MonthlyEnergyLevelRepository.InsertEntity(out error, energyLevel);
                if (error != null) return false;
                return true;
            }
            catch (Exception ex)
            {
                Controller.LogError(ex);
                ErrorClasses.Controller.GetUnknownError();
                throw;
            }
        }

        public override IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetInfoView(int userId, DateTime now,
                                                                                    int numberOfMonthsToDisplay)
        {
            var helper = new MonthlyDataHelper
                {
                    Repository = MonthlyEnergyLevelRepository,
                    FinalRepository = FinalMonthlyDailyEnergyLevelRepository
                };
            return helper.GetViewDataForTimePeriod(userId, now, numberOfMonthsToDisplay).ToList();
        }
    }

    public class MonthlyEnergyHandlingTemplateDynamic :
      IDataAgreggationHelperTemplate<MonthlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>
    {

        public IRepositoryBase<DailyEnergyLevel> DailyEnergyLevelRepository { get; set; }

        public void UpdateEntityRecords(DailyEnergyLevel baseEntity)
        {
        }

        public IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetInfoView(int userId, DateTime now, int numberOfMonthsToDisplay)
        {
                   
            var helper = new EnergyLevelMonthlyDataHelperDynamic
                {
                    Repository = DailyEnergyLevelRepository
                };

            return helper.GetViewDataForTimePeriod(userId, now, numberOfMonthsToDisplay).ToList();

        }
    }

}