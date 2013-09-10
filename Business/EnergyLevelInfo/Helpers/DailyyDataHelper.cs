using Business.Helpers;
using Business.Interfaces;
using Model.EnergyLevelInfo;

namespace Business.EnergyLevelInfo.Helpers
{
    public class DailyyDataHelper :
        DailyyDataHelper<ServiceModel.EnergyLevel.EnergyLevelInfo, DailyEnergyLevel>
    {
        public override ServiceModel.EnergyLevel.EnergyLevelInfo InitializeNewServiceType(
            DailyEnergyLevel energyLEvelInTimeSpan, TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo
                {
                Date = energyLEvelInTimeSpan.Date,
                Value = energyLEvelInTimeSpan.EnergyLevel,
                DateString = span.GetSpanDate()
            };
        }

        public override ServiceModel.EnergyLevel.EnergyLevelInfo CreateEmptyServiceType(TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo
                {
                    Value = -1,
                    DateString = span.GetSpanDate()
                };
        }
    }
}