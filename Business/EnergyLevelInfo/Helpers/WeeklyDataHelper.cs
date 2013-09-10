using System.Collections.Generic;
using System.Linq;
using Business.Helpers;
using Model.EnergyLevelInfo;
using TimeSpan = Business.Interfaces.TimeSpan;

namespace Business.EnergyLevelInfo.Helpers
{

    public class WeeklyDataHelper : WeeklyDataHelper<ServiceModel.EnergyLevel.EnergyLevelInfo, Model.EnergyLevelInfo.DailyEnergyLevel>
    {
        public override ServiceModel.EnergyLevel.EnergyLevelInfo InitializeNewServiceType(DailyEnergyLevel answers, TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo()
            {
                Value = answers.EnergyLevel,
                DateString = span.GetSpanDate(),
                Date = answers.Date
            };
        }

        public override ServiceModel.EnergyLevel.EnergyLevelInfo InitializeNewServiceType(List<DailyEnergyLevel> dailyInfoInTimeSpan, TimeSpan span)
        {
            double sum = dailyInfoInTimeSpan.Sum(e => e.EnergyLevel) / dailyInfoInTimeSpan.Count;
            return new ServiceModel.EnergyLevel.EnergyLevelInfo()
                {
                    Value = sum,
                    DateString = span.GetSpanDate(),
                    Date = dailyInfoInTimeSpan.First().Date
                };
        }

        public override ServiceModel.EnergyLevel.EnergyLevelInfo CreateEmptyServiceType(TimeSpan span)
        {
            return new ServiceModel.EnergyLevel.EnergyLevelInfo() { Value = -1, DateString = span.GetSpanDate() };
        }
    }




}