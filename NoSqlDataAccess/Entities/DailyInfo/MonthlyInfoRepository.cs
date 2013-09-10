using System.Linq;
using DataAccess.Interfaces;
using Model.DailyInfo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace NoSqlDataAccess.Entities.DailyInfo
{
    public class MonthlyInfoRepository : NoSqlRepositoryBase<DailyInfoSummaryData>,
                                         IUpdatingRepository<DailyInfoSummaryData>
    {
        public override string CollectionName
        {
            get { return "MonthlyInfos"; }
        }

        public bool Update(DailyInfoSummaryData todayDailyInfo)
        {
            MongoCollection<DailyInfoSummaryData> settingsCollection = Collection;

            IMongoQuery query = Query.And(
                Query.EQ("_id", todayDailyInfo.Id),
                Query.EQ("UserId", todayDailyInfo.UserId));

            DailyInfoSummaryData dailyInfo = Collection.Find(query).FirstOrDefault();
            if (dailyInfo != null)
            {
                BsonDocument dilyInfoBsonDoc = todayDailyInfo.ToBsonDocument();
                settingsCollection.Save(dilyInfoBsonDoc);
                return true;
            }
            return false;
        }
    }
}