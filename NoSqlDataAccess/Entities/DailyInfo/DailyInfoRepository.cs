using System.Linq;
using DataAccess.Interfaces;
using ErrorClasses;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;

namespace NoSqlDataAccess.Entities.DailyInfo
{
    public class DailyInfoRepository : NoSqlRepositoryBase<Model.DailyInfo.DailyInfo>, IDailyInfoRepository
    {

        public override string CollectionName { get { return "DailyInfos"; } }

        public bool UpdateOrInsert(Model.DailyInfo.DailyInfo todayDailyInfo)
        {
            MongoCollection<Model.DailyInfo.DailyInfo> settingsCollection = Collection;

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
            else
            {
                GenericError error;
                InsertEntity(out error, todayDailyInfo);
                return false;
            }
        }
    }

}
