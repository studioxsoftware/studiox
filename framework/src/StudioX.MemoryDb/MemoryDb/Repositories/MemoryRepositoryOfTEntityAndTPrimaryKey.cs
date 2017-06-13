using System.Collections.Generic;
using System.Linq;
using StudioX.Domain.Entities;
using StudioX.Domain.Repositories;

namespace StudioX.MemoryDb.Repositories
{
    //TODO: Implement thread-safety..?
    public class MemoryRepository<TEntity, TPrimaryKey> : StudioXRepositoryBase<TEntity, TPrimaryKey>
        where TEntity : class, IEntity<TPrimaryKey>
    {
        public virtual MemoryDatabase Database => databaseProvider.Database;

        public virtual List<TEntity> Table => Database.Set<TEntity>();

        private readonly IMemoryDatabaseProvider databaseProvider;
        private readonly MemoryPrimaryKeyGenerator<TPrimaryKey> primaryKeyGenerator;

        public MemoryRepository(IMemoryDatabaseProvider databaseProvider)
        {
            this.databaseProvider = databaseProvider;
            primaryKeyGenerator = new MemoryPrimaryKeyGenerator<TPrimaryKey>();
        }

        public override IQueryable<TEntity> GetAll()
        {
            return Table.AsQueryable();
        }

        public override TEntity Insert(TEntity entity)
        {
            if (entity.IsTransient())
            {
                entity.Id = primaryKeyGenerator.GetNext();
            }

            Table.Add(entity);
            return entity;
        }

        public override TEntity Update(TEntity entity)
        {
            var index = Table.FindIndex(e => EqualityComparer<TPrimaryKey>.Default.Equals(e.Id, entity.Id));
            if (index >= 0)
            {
                Table[index] = entity;
            }

            return entity;
        }

        public override void Delete(TEntity entity)
        {
            Delete(entity.Id);
        }

        public override void Delete(TPrimaryKey id)
        {
            var index = Table.FindIndex(e => EqualityComparer<TPrimaryKey>.Default.Equals(e.Id, id));
            if (index >= 0)
            {
                Table.RemoveAt(index);
            }
        }
    }
}