using Model.EnergyLevelInfo;

namespace NoSqlDataAccess.Entities.EnergyLevelInfo
{
    public class YearlyEnergyLevelRepository : NoSqlRepositoryBase<YearlyEnergyLevel>
    {
        public override string CollectionName
        {
            get
            {
                return "YearlyEnergyLevels";
            }
        }
    }

      
}
