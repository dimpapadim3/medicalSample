using ErrorClasses;
using Model;
using MongoDB.Bson;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Linq;

namespace NoSqlDataAccess.Entities
{
    public class WaterfallUserPreferencesRepository : NoSqlRepositoryBase<WaterfallUserPreferences>
    {
        public WaterfallUserPreferencesRepository() { }

        public override string CollectionName { get { return "WaterfallUserPreferences"; } }

        public IEnumerable<WaterfallUserPreferences> GetUserPreferences(long Id)
        {
            return Collection.AsQueryable<WaterfallUserPreferences>().Where(up => up.UserID.Equals(Id));
        }

        public void Delete(string Id)
        {
            var entities = Query<WaterfallUserPreferences>.Where(p => p.Id == Id);
            Collection.Remove(entities);
        }

        public void DeleteAllPreferencesByUserId(long userId)
        {
            var entities = Query<WaterfallUserPreferences>.Where(d => d.UserID.Equals(userId));
            Collection.Remove(entities);
        }

        public void Save(WaterfallUserPreferences preferences)
        {
            var entities = Query<WaterfallUserPreferences>.Where(p => p.UserID.Equals(preferences.UserID)
                                                                  && p.PreviousValue.Equals(preferences.PreviousValue));
            Collection.Remove(entities);
            Collection.Save<WaterfallUserPreferences>(preferences);
        }

        //public void Save(WaterfallUserPreferences preferences)
        //{
        //    if (GetUserPreferences(preferences.UserID).SingleOrDefault(p => p.PreviousValue.Equals(preferences.PreviousValue)) == null)
        //    {
        //        Insert(preferences);
        //    }
        //    else
        //    {
        //        Update(preferences);
        //    }
        //}

        //private void Insert(WaterfallUserPreferences userPreferences)
        //{
        //    GenericError error;
        //    InsertEntity(out error, userPreferences);
        //    if (error != null)
        //    {
        //        throw new Exception(error.ErrorDesc);
        //    }
        //}

        //private void Update(WaterfallUserPreferences userPreferences)
        //{
        //    var entity = GetUserPreferences(userPreferences.UserID);
        //    Collection.Save<WaterfallUserPreferences>(userPreferences);
        //}
    }
}
