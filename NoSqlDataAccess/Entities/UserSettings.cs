using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MongoDB.Driver.Builders;
using MongoDB.Bson;
using MongoDB.Driver;

namespace NoSqlDataAccess.Entities
{
    public class UserSettings : NoSqlRepositoryBase<Model.UserSettings>
    {
        public override string CollectionName
        {
            get { return "UserSettings"; }
        }

        public void UpdateLastAnsweredEnergyLevelDate(long userId, DateTime updatedTime)
        {
            UpdateRecord(userId,updatedTime, "LastAnsweredEnergyLevelDate");
        }

        public void UpdateLattestAnsweredQuestionaireDate(long userId, DateTime updatedTime)
        {
            UpdateRecord(userId,updatedTime, "LastAnsweredQuestionnaireDate");
        }


        public void UpdateRecord(long userId, DateTime updatedTime, string propertyName)
        {
            MongoCollection<Model.UserSettings> settingsCollection = Collection;
            var userIdQuery = new QueryDocument("UserId", userId);
            var setting = Collection.Find(userIdQuery).FirstOrDefault();
            if (setting != null)
            {
                var settingBsonDoc = setting.ToBsonDocument();
                settingBsonDoc[propertyName] = updatedTime;
                settingsCollection.Save(settingBsonDoc);
            }
            else
            {
                if (propertyName == "LastAnsweredQuestionnaireDate")
                    Collection.Insert(new Model.UserSettings()
                        {
                            UserId = userId,
                            LastAnsweredQuestionnaireDate = updatedTime
                        });
                if (propertyName == "LastAnsweredEnergyLevelDate")
                    Collection.Insert(new Model.UserSettings()
                    {
                        UserId = userId,
                        LastAnsweredEnergyLevelDate = updatedTime
                    });
            }
        }
    }
}
