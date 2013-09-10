using Business.Helpers;
using Model.EnergyLevelInfo;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.EnergyLevelInfo.Helpers
{
    internal class MonthlyDataHelper : MonthlyDataHelper<ServiceModel.EnergyLevel.EnergyLevelInfo, MonthlyEnergyLevel>
    {
 

        public override ServiceModel.EnergyLevel.EnergyLevelInfo InitializeNewServiceType(MonthlyEnergyLevel dailyInfoInTimeSpan, TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo
                {
                    Date = dailyInfoInTimeSpan.Date,
                    Value = dailyInfoInTimeSpan.Average,
                    DateString = span.GetSpanDate()
                };
        }

        public override ServiceModel.EnergyLevel.EnergyLevelInfo CreateEmptyServiceType(TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo { Value = -1, DateString = span.GetSpanDate() };
        }
    }
     
    public class EnergyLevelMonthlyDataHelperDynamic :
        MonthlyDynamicDataHelper<ServiceModel.EnergyLevel.EnergyLevelInfo, DailyEnergyLevel>
    {
        public override ServiceModel.EnergyLevel.EnergyLevelInfo InitializeNewServiceType(DailyEnergyLevel dailyInfoInTimeSpan, TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo
                {
                Date = dailyInfoInTimeSpan.Date,
                Value = dailyInfoInTimeSpan.EnergyLevel,
                DateString = span.GetSpanDate()
            };
              
        }

        public override ServiceModel.EnergyLevel.EnergyLevelInfo CreateEmptyServiceType(TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo { Value = -1, DateString = span.GetSpanDate() };
        }
    }
 

}