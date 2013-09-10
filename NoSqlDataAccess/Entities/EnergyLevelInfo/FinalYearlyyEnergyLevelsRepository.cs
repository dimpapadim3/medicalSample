using Common.Classes;
using DataAccess.Interfaces;
using Model.EnergyLevelInfo;

namespace NoSqlDataAccess.Entities.EnergyLevelInfo
{
    public class FinalYearlyyEnergyLevelsRepository : NoSqlRepositoryBase<YearlyEnergyLevel>, INoSqlFinalValuesRepositoryBase<YearlyEnergyLevel>
    {
        public FinalYearlyyEnergyLevelsRepository() { }

        public override string CollectionName { get { return "FinalYearlyEnergyLevels"; } }
    }
}