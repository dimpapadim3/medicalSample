using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model.View;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using MongoDB.Bson.Serialization.IdGenerators;

namespace NoSqlDataAccess
{
    internal static class DriverInitializer
    {
        static DriverInitializer()
        {
            MongoDB.Bson.Serialization.Options.DateTimeSerializationOptions options = 
                MongoDB.Bson.Serialization.Options.DateTimeSerializationOptions.LocalInstance;
            var serializer = new MongoDB.Bson.Serialization.Serializers.DateTimeSerializer(options);
            MongoDB.Bson.Serialization.BsonSerializer.RegisterSerializer(typeof(DateTime), serializer);

            BsonClassMap.RegisterClassMap<Model.Activity>();
            BsonClassMap.RegisterClassMap<Model.Training>();
            BsonClassMap.RegisterClassMap<Model.Travel>();
            BsonClassMap.RegisterClassMap<Model.Sleep>();
            BsonClassMap.RegisterClassMap<Model.Rest>();
            BsonClassMap.RegisterClassMap<Model.Hotel>();
            BsonClassMap.RegisterClassMap<Model.Weight>();
            BsonClassMap.RegisterClassMap<Model.Sick>();
            
            //BsonClassMap.RegisterClassMap<TrainingSessionMeasurmentData>(cm =>
            //{
            //    cm.MapIdProperty(c => c.Id)
            //        .SetIdGenerator(new ObjectIdGenerator())
            //        .SetRepresentation(BsonType.String);
            //    cm.MapProperty(c => c.ActivityClassification).SetElementName("ActivityClassification");
            //});
        }

        public static void InitializeDriver()
        {
            //dummy: just to cause invocation of the constructor
        }
    }
}
