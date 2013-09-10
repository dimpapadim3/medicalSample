using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using Common.Classes;
using DataAccess.Interfaces;
using ErrorClasses;
using MongoDB.Driver;
using MongoDB.Bson;
using MongoDB.Bson.Serialization;
using System.Security.Cryptography;
using MongoDB.Bson.Serialization.Attributes;
using NoSqlDataAccess.Entities;

namespace NoSqlDataAccess
{
    public class MockDataRepositoryBase<T> : IRepositoryBase<T> where T : class   ,new()
    {
        public List<T> DummyData = new List<T>();

        public void Remove(Expression<Func<T, bool>> filter, out GenericError error)
        {
            throw new NotImplementedException();
        }

        public void Drop()
        {
            DummyData.Clear();
        }

        public IQueryable<T> GetAsQueryable<T>()
        {
            return ((IEnumerable<T>)DummyData).Select(x => x).AsQueryable(); 
        }

        public MongoDB.Driver.MongoCollection<T> Collection
        {
            get { return null; }
        }

        public string CollectionName
        {
            get { return ""; }
        }

        public List<T> GetEntities(out ErrorClasses.GenericError error, Func<T, bool> filter = null)
        {
            error = null;
            if (filter != null)
                return DummyData.Where(filter).ToList();
            return DummyData;
        }

        public void InsertEntity(out ErrorClasses.GenericError error, IEnumerable<T> entity)
        {
            error = null;
            entity.ToList().ForEach(e => DummyData.Add(e));
        }

        public void InsertEntity(out ErrorClasses.GenericError error, T entity)
        {
            error = null;
            DummyData.Add(entity);
        }
    }

    public class MockFinalDataRepositoryBase<T> : MockDataRepositoryBase<T> ,INoSqlFinalValuesRepositoryBase<T> where T : class   ,new()
    {
   
    }
}
