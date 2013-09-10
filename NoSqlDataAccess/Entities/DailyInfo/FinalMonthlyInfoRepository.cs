using DataAccess.Interfaces;
using Model.DailyInfo;

namespace NoSqlDataAccess.Entities.DailyInfo
{
    public class FinalMonthlyInfoRepository : NoSqlRepositoryBase<DailyInfoSummaryData>,
                                              INoSqlFinalValuesRepositoryBase<DailyInfoSummaryData>
    {
        public override string CollectionName
        {
            get { return "FinalyMonthlyInfos"; }
        }
    }
}