using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using Common.Classes;
using DataAccess.Interfaces;
using ErrorClasses;
using Model.EnergyLevelInfo;
using NoSqlDataAccess.Entities;
using Controller = Logger.Controller;

namespace Business.EnergyLevelInfo.Helpers
{
    public class YearyInfoHandlingTemplate :
        DataAgreggationHelperTemplate<YearlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>
    {
        public YearyInfoHandlingTemplate()
        {
        }

        public IRepositoryBase<Model.DailyInfo.DailyInfo> DailyEnergyLevelRepository { get; set; }

        public IRepositoryBase<YearlyEnergyLevel> YearlyEnergyLevelRepository { get; set; }

        public INoSqlFinalValuesRepositoryBase<YearlyEnergyLevel> FinalMonthlyDailyEnergyLevelRepository { get; set; }

        public override ServiceModel.EnergyLevel.EnergyLevelInfo CreateEmptyServiceModel()
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo();
        }

        public override bool IsTheFirstEntryForUser(long userId)
        {
            GenericError error;
            List<YearlyEnergyLevel> entries = YearlyEnergyLevelRepository.GetEntities(out error, d => d.UserId == userId);
            return entries == null || entries.Count == 0;
        }

        public override YearlyEnergyLevel CreateNewModelItem(DailyEnergyLevel energyLevel)
        {
            return new YearlyEnergyLevel
            {
                UserId = energyLevel.UserId,

                Date = energyLevel.Date,
                Year = energyLevel.Date.Year
            };
        }

        public override YearlyEnergyLevel GetLattestRecord(long userId)
        {
            GenericError error;
            List<YearlyEnergyLevel> records = YearlyEnergyLevelRepository.GetEntities(out error, d => d.UserId == userId);

            if (records.Any())
            {
                records.Sort((d1, d2) => DateTime.Compare(d1.Date, d2.Date));
                return records.Last();
            }


            return new YearlyEnergyLevel();
        }

        public override void InitializeRunningTotals(YearlyEnergyLevel YearlyEnergyLevel, DailyEnergyLevel energyLevel)
        {
            YearlyEnergyLevel.Count = 1;
            YearlyEnergyLevel.Average = energyLevel.EnergyLevel;
            YearlyEnergyLevel.SumTotal = energyLevel.EnergyLevel;
            // YearlyEnergyLevel.DailyEnergyLevel.Add(energyLevel);
        }

        public override bool IsTheFirstRecordOfPeriod(YearlyEnergyLevel latestMonthlyRecord,
                                                      DailyEnergyLevel energyLevel)
        {
            return latestMonthlyRecord.Date.Year < energyLevel.Date.Year;
        }

        public override void ComputeFromLastRecord(DailyEnergyLevel energyLevel, YearlyEnergyLevel yearlyEnergyLevel,
                                                   YearlyEnergyLevel latestMonthlyRecord)
        {
            yearlyEnergyLevel.Count = latestMonthlyRecord.Count + 1;

            yearlyEnergyLevel.SumTotal = latestMonthlyRecord.SumTotal + energyLevel.EnergyLevel;
            //YearlyEnergyLevel.DailyEnergyLevel = latestMonthlyRecord.DailyEnergyLevel;
            //  YearlyEnergyLevel.DailyEnergyLevel.Add(energyLevel);

            double newAverage = yearlyEnergyLevel.SumTotal / yearlyEnergyLevel.Count;
            yearlyEnergyLevel.Average = Math.Round(newAverage, MidpointRounding.AwayFromZero);
        }

        public override bool IsTheLastRecordOfthePeriod(YearlyEnergyLevel latestMonthlyRecord,
                                                        DailyEnergyLevel energyLevel)
        {
            return latestMonthlyRecord.Date.Year < energyLevel.Date.Year;
        }

        public override void InsertEntityToFinalRecords(out GenericError error, YearlyEnergyLevel latestMonthlyRecord)
        {
            FinalMonthlyDailyEnergyLevelRepository.InsertEntity(out error, latestMonthlyRecord);
        }

        public override bool AddNewRecordToDataBase(YearlyEnergyLevel energyLevel)
        {
            GenericError error;
            try
            {
                YearlyEnergyLevelRepository.InsertEntity(out error, energyLevel);
                if (error != null) return false;
                return true;
            }
            catch (Exception ex)
            {
                Controller.LogError(ex);
                error = ErrorClasses.Controller.GetUnknownError();
                return false;
            }
        }

        public override IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetInfoView(int userId, DateTime now,
                                                                                    int numberOfMonthsToDisplay)
        {
            var helper = new YearlyDataHelper()
            {
                Repository = YearlyEnergyLevelRepository,
                FinalRepository = FinalMonthlyDailyEnergyLevelRepository
            };
            return helper.GetViewDataForTimePeriod(userId, now, numberOfMonthsToDisplay).ToList();
        }
    }

    public class YearyInfoHandlingTemplateDynamic :
        IDataAgreggationHelperTemplate<YearlyEnergyLevel, DailyEnergyLevel, ServiceModel.EnergyLevel.EnergyLevelInfo>
    {
        public IRepositoryBase<Model.EnergyLevelInfo.DailyEnergyLevel> DailyEnergyLevelRepository { get; set; }

        public void UpdateEntityRecords(DailyEnergyLevel baseEntity)
        {

        }

        public IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetInfoView(int userid, DateTime now, int numberOfRecordsToDisplay)
        {
            return new YearlyDataHelperDynamic{Repository = DailyEnergyLevelRepository}
                                        .GetViewDataForTimePeriod(userid, now, numberOfRecordsToDisplay);
        }
    }
}