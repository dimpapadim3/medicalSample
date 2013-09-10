using Common.Classes;
using DataAccess.Interfaces;
using Model.EnergyLevelInfo;

namespace NoSqlDataAccess.Entities.EnergyLevelInfo
{
    public class FinalMonthlyEnergyLevelsRepository : NoSqlRepositoryBase<MonthlyEnergyLevel>, INoSqlFinalValuesRepositoryBase<MonthlyEnergyLevel>
    {
        public FinalMonthlyEnergyLevelsRepository()
        {
        }

        public override string CollectionName { get { return "FinalMonthlyEnergyLevels"; } }
    }
}