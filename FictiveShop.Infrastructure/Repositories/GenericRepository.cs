using Ardalis.GuardClauses;
using FictiveShop.Core.Domain;
using FictiveShop.Core.Extensions;
using FictiveShop.Core.Interfeces;
using FictiveShop.Infrastructure.DataAccess;
using System;
using System.Collections.Generic;
using System.Linq;

namespace FictiveShop.Infrastructure.Repositories
{
    public class GenericRepository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly FictiveShopDbContext _db;

        public GenericRepository(FictiveShopDbContext db)
        {
            _db = db;
        }

        public void Create(TEntity entity)
        {
            if (entity is null)
                Guard.Against.Null(entity, nameof(entity), $"The {typeof(TEntity).Name} object was null when attempting to add it.");
            _db.Set<TEntity>().Add(entity);
        }

        public IReadOnlyCollection<TEntity> GetAll() => _db.Set<TEntity>().ToList();

        public IReadOnlyCollection<TEntity> GetAll(Func<TEntity, bool> filter) => _db.Set<TEntity>().Where(filter).ToList();

        public TEntity GetById(string id) => _db.Set<TEntity>().FirstOrDefault(x => x.Id == id);

        public void Remove(string id)
        {
            var entity = _db.Set<TEntity>().FirstOrDefault(x => x.Id == id);
            if (entity is null) Guard.Against.Null(entity, nameof(entity));

            _db.Set<TEntity>().Remove(entity);
        }

        public void Update(TEntity newEntity)
        {
            var oldEntity = _db.Set<TEntity>().FirstOrDefault(x => x.Id == newEntity.Id);
            if (oldEntity is null) Guard.Against.Null(oldEntity, nameof(oldEntity));

            _db.Set<TEntity>().ReplaceOne(newEntity, oldEntity);
        }
    }
}