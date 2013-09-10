using System;
using System.Linq.Expressions;
using ErrorClasses;

namespace DataAccess.Interfaces
{
    public interface IRepositoryBase<T> where T : class
    {
        void Remove(Expression<Func<T, bool>> filter, out GenericError error);
        void Drop();
        System.Linq.IQueryable<T> GetAsQueryable<T>();
        string CollectionName { get; }
        System.Collections.Generic.List<T> GetEntities(out ErrorClasses.GenericError error, Func<T, bool> filter = null);
        void InsertEntity(out ErrorClasses.GenericError error, System.Collections.Generic.IEnumerable<T> entity);
        void InsertEntity(out ErrorClasses.GenericError error, T entity);

    }
 
}