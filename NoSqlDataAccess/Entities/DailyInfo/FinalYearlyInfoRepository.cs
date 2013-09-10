using Common.Classes;
using DataAccess.Interfaces;
using Model.DailyInfo;

namespace NoSqlDataAccess.Entities.DailyInfo
{
    public class FinalYearlyInfoRepository : NoSqlRepositoryBase<DailyInfoSummaryData>, INoSqlFinalValuesRepositoryBase<DailyInfoSummaryData>
    {
        public FinalYearlyInfoRepository()
        {
        }

        public override string CollectionName
        {
            get
            {
                return "FinalyYearlyInfos";
            }
        }
    }
}