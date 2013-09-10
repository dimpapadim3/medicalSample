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
    public class PlanProfileRepository : NoSqlRepositoryBase<PlanProfile>
    {
        #region Public Methods
        public PlanProfileRepository() { }

        public override string CollectionName { get { return "PlanProfiles"; } }

        public bool Exists(string profileId)
        {
            return Collection.AsQueryable().Any(p => p.Id == profileId);
        }

        public bool NameExists(string profileName, long userId)
        {
            MongoCollection<PlanProfile> collection = Collection;
            return collection.AsQueryable().Any(p => p.Name == profileName && p.UserID.Equals(userId));
        }

        public void Delete(string profileID)
        {
            var profile = Query<PlanProfile>.Where(e => e.Id == profileID);

            Collection.Remove(profile);
        }

        public void Save(PlanProfile profile)
        {
            if (Exists(profile.Id))
            {
                Update(profile);
            }
            else
            {
                Insert(profile);
            }
        } 
        #endregion



        #region Private Methods
        private void Insert(PlanProfile profile)
        {
            MongoCollection<PlanProfile> collection = Collection;
            GenericError error;

            if (Exists(profile.Name))
            {
                throw new Exception("Profile with the given name already exists! Please choose a different profil name.");
            }
            else
            {
                InsertEntity(out error, profile);
            }
        }

        private void Update(PlanProfile profile)
        {
            var exists = Collection.AsQueryable<PlanProfile>()
                                   .Any(p => p.Id == profile.Id);

            if (exists)
            {
                Collection.Save(profile);
            }
        }
        #endregion

    }

}
