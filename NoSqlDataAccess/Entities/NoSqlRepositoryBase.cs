using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Common;
using DataAccess.Interfaces;
using ErrorClasses;
using MongoDB.Driver;
using MongoDB.Driver.Builders;
using MongoDB.Driver.Linq;

namespace NoSqlDataAccess.Entities
{
 
 
    public abstract class NoSqlRepositoryBase<T> : IRepositoryBase<T> where T : class
    {
        private readonly MongoCollection<T> _collection;

        protected NoSqlRepositoryBase()
        {
            GenericError error = null;
            _collection = GetCollection(out error);

            DriverInitializer.InitializeDriver();
        }

        public IConfiguration Configuration { get { return StructureMap.ObjectFactory.GetInstance<IConfiguration>(); } }

        protected MongoCollection<T> Collection
        {
            get { return _collection; }
        }

        public abstract string CollectionName { get; }

        public IQueryable<T> GetAsQueryable<T>()
        {
            return Collection.AsQueryable<T>();
        }

        public List<T> GetEntities(out GenericError error,
                                   Func<T, bool> filter = null)
        {
            error = null;

            var activityClasses = new List<T>();

            try
            {
                if (Collection != null)
                    activityClasses = Collection.AsQueryable()
                                                .Where(filter).ToList();
            }
            catch (Exception e)
            {
                error = new GenericError {ErrorDesc = e.Message};
                throw e;
            }

            return activityClasses;
        }

        public virtual void InsertEntity(out GenericError error, IEnumerable<T> entity)
        {
            error = null;
            IEnumerable<WriteConcernResult> results = new LinkedList<WriteConcernResult>();

            try
            {
                results = Collection.InsertBatch(entity, WriteConcern.Acknowledged);
            }
            catch (Exception)
            {
                string message = "Error inserting document in collection " + CollectionName + " :";
                results.ToList().ForEach(w => message += w.ErrorMessage);
                error = new GenericError { ErrorDesc = message };
                throw;
            }
        }

        public virtual void InsertEntity(out GenericError error, T entity)
        {
            InsertEntity(out error, new List<T> { entity });
        }


        public virtual void Remove(Expression<Func<T, bool>> filter, out GenericError error)
        {
            error = null;
            IEnumerable<WriteConcernResult> results = new LinkedList<WriteConcernResult>();
           
            try
            {
                IMongoQuery query = Query<T>.Where(filter);
       
                _collection.Remove(query, WriteConcern.Acknowledged);
            }
            catch (Exception)
            {
                string message = "Error inserting document in collection " + CollectionName + " :";
                results.ToList().ForEach(w => message += w.ErrorMessage);
                error = new GenericError { ErrorDesc = message };
                throw;
            }
        }

        public void Drop()
        {
            Collection.Drop();
        }

        protected virtual MongoCollection<T> GetCollection(out GenericError error)
        {
            string connString = GetConnection(out error);
            MongoDatabase database = MongoDatabase.Create(connString);
            return database.GetCollection<T>(CollectionName);
        }

        private string GetConnection(out GenericError error)
        {
            error = null;

            if (Configuration != null)
            {
                return ConfigurationConnString(ref error);
            }
            return Constants.MONGO_CONNECTION_STRING;
        }

        private string ConfigurationConnString(ref GenericError error)
        {
            string connString = Configuration.NoSqlDataConnectionString;

            if (string.IsNullOrEmpty(connString))
            {
                error = Controller.GetUnknownError();
                throw new Exception("Empty Connection String");
            }

            return connString;
        }

        public virtual void Update(out GenericError error)
        {
            Update(out error);
        }
    }
}