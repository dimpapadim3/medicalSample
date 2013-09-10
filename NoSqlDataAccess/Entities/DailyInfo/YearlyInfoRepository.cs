using System.Linq;
using Model.DailyInfo;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace NoSqlDataAccess.Entities.DailyInfo
{
    public class YearlyInfoRepository : NoSqlRepositoryBase<DailyInfoSummaryData>
    {
        public YearlyInfoRepository()
        {
        }

        public override string CollectionName
        {
            get
            {
                return "YearlyInfos";
            }
        }



        public bool Update(Model.DailyInfo.DailyInfoSummaryData todayDailyInfo)
        {
            MongoCollection<Model.DailyInfo.DailyInfoSummaryData> settingsCollection = Collection;

            var query = Query.And(
                Query.EQ("_id", todayDailyInfo.Id),
                Query.EQ("UserId", todayDailyInfo.UserId));

            var dailyInfo = Collection.Find(query).FirstOrDefault();
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