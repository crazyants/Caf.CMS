using MongoDB.Driver;
using MongoDB.Driver.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace CAF.Infrastructure.Core.Data
{
    /// <summary>
    /// Repository
    /// </summary>
    public partial interface IMongoDBRepository<T> where T : MongoDBBaseEntity
    {
        IMongoQueryable<T> Table { get; }

        T GetById(int id);

        T Insert(T entity);

        void InsertRange(IEnumerable<T> entities);

        T Update(T entity);

        void UpdateRange(IEnumerable<T> entities);


        void Delete(T entity);

        void DeleteRange(IEnumerable<T> entities);

        IMongoDatabase Context { get; }

    }
}
