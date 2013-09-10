 
using Model.EnergyLevelInfo;

namespace NoSqlDataAccess.Entities.EnergyLevelInfo
{
    public class DailyEnergyLevelRepository : NoSqlRepositoryBase<DailyEnergyLevel>
    {
        public DailyEnergyLevelRepository()
        {
            //    BsonClassMap.RegisterClassMap<DailyEnergyLevelModel>(cm =>
            //    {
            //        cm.MapProperty<object>(m => m.Id).SetElementName("Id");
            //        cm.MapProperty<DateTime>(m => m.Date).SetElementName("ActivityDate");
            //    });
        }

        public override string CollectionName { get { return "DailyEnergyLevels"; } }

    }

    public class EnergyLevelInfoRepository : NoSqlRepositoryBase<Model.EnergyLevelInfo. EnergyLevelInfo>
    {
        public EnergyLevelInfoRepository()
        {
 
        }

        public override string CollectionName { get { return "EnergyLevelInfos"; } }

    }

}



