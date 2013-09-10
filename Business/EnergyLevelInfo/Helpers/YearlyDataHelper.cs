using System;
using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using Common.Classes;
using Model.DailyInfo;
using Model.EnergyLevelInfo;
using NoSqlDataAccess.Entities;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.EnergyLevelInfo.Helpers
{
    public class YearlyDataHelper : YearlyDataHelper<ServiceModel.EnergyLevel.EnergyLevelInfo, Model.EnergyLevelInfo.YearlyEnergyLevel>
    {
        public override ServiceModel.EnergyLevel.EnergyLevelInfo InitializeNewServiceType(Model.EnergyLevelInfo.YearlyEnergyLevel objectInTimeSpan, TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo()
            {
                Date = objectInTimeSpan.Date,
                Value = objectInTimeSpan.Average,
                DateString = span.GetSpanDate()
            };
        }

        public override ServiceModel.EnergyLevel.EnergyLevelInfo CreateEmptyServiceType(TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo() { Value = -1, DateString = span.GetSpanDate() };
        }
    }

    public class YearlyDataHelperDynamic : YearlyDynamicDataHelper<ServiceModel.EnergyLevel.EnergyLevelInfo, Model.EnergyLevelInfo.DailyEnergyLevel>
    {
        public override ServiceModel.EnergyLevel.EnergyLevelInfo InitializeNewServiceType(DailyEnergyLevel objectInTimeSpan, TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo()
            {
                Date = objectInTimeSpan.Date,
                Value = objectInTimeSpan.EnergyLevel,
                DateString = span.GetSpanDate()
            };
        }

        public override ServiceModel.EnergyLevel.EnergyLevelInfo CreateEmptyServiceType(TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo() { Value = -1, DateString = span.GetSpanDate() };
        }
    }
}