using System;
using System.Collections.Generic;
using Model.EnergyLevelInfo;

namespace Business.Interfaces
{
    public interface IEnergyLevelInfoService
    {
        bool InsertDailyEnergyLevel(DailyEnergyLevel energyLevel);
        IList<ServiceModel.EnergyLevel.EnergyLevelInfo> GetEnergyLevelsForTimePeriod(int userId, DateTime now, int period);
        bool SubmitEnergyLevel(int userId, byte energyLevel);
        ServiceModel.EnergyLevel.EnergyLevelInfo GetSingleEnergyLevelForTimePeriod(int userId, DateTime currentDisplayTimeState, int period);
    }
}