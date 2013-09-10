using Model.DailyInfo;

namespace NoSqlDataAccess.Entities.DailyInfo
{
    public class WeeklyInfoRepository : NoSqlRepositoryBase<DailyInfoSummaryData>
    {
        public WeeklyInfoRepository()
        {
        }

        public override string CollectionName
        {
            get
            {
                return "WeeklyInfos";
            }
        }
    }
}