using Model.EnergyLevelInfo;

namespace NoSqlDataAccess.Entities.EnergyLevelInfo
{
    public class MonthlyEnergyLevelRepository : NoSqlRepositoryBase<MonthlyEnergyLevel>
    {
        public MonthlyEnergyLevelRepository()
        {

        }

        public override string CollectionName { get { return "MonthlyEnergyLevels"; } }

    }


}
