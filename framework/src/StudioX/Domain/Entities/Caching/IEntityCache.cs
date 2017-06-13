using System.Threading.Tasks;
using StudioX.Runtime.Caching;

namespace StudioX.Domain.Entities.Caching
{
    public interface IEntityCache<TCacheItem> : IEntityCache<TCacheItem, int>
    {

    }

    public interface IEntityCache<TCacheItem, TPrimaryKey>
    {
        TCacheItem this[TPrimaryKey id] { get; }

        string CacheName { get; }

        ITypedCache<TPrimaryKey, TCacheItem> InternalCache { get; }

        TCacheItem Get(TPrimaryKey id);

        Task<TCacheItem> GetAsync(TPrimaryKey id);
    }
}