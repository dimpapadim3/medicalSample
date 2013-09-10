using System.Collections.Generic;
using System.Linq;
using DataAccess.Interfaces;
using Model.DailyInfo;
using MongoDB.Driver;

namespace NoSqlDataAccess.Entities.DailyInfo
{
    public class FavouriteActivityRepository : NoSqlRepositoryBase<FavouriteActivities>, IFavouriteActivityRepository
    {
        public override string CollectionName
        {
            get { return "FavouriteActivities"; }
        }

        public void UpdateRecord(KeyValuePair<string, int> updateDictionairyEntry)
        {
            MongoCollection<FavouriteActivities> settingsCollection = Collection;

            IFavouriteActivities activities = Collection.FindOne();
            if (activities.ActivityClassCount != null)
            {
                KeyValuePair<string, int> entry =
                    activities.ActivityClassCount.FirstOrDefault(k => k.Key == updateDictionairyEntry.Key);
                if (!string.IsNullOrEmpty(entry.Key))
                {
                    activities.ActivityClassCount[entry.Key] = (updateDictionairyEntry.Value);
                }
                else
                    activities.ActivityClassCount[updateDictionairyEntry.Key] = (updateDictionairyEntry.Value);


                settingsCollection.Save(activities);
            }
        }
    }
}