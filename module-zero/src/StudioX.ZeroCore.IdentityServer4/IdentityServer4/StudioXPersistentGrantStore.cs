using System.Collections.Generic;
using System.Threading.Tasks;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using IdentityServer4.Models;
using IdentityServer4.Stores;

namespace StudioX.IdentityServer4
{
    public class StudioXPersistedGrantStore : StudioXServiceBase, IPersistedGrantStore
    {
        private readonly IRepository<PersistedGrantEntity, string> persistedGrantRepository;

        public StudioXPersistedGrantStore(IRepository<PersistedGrantEntity, string> persistedGrantRepository)
        {
            this.persistedGrantRepository = persistedGrantRepository;
        }

        [UnitOfWork]
        public virtual async Task StoreAsync(PersistedGrant grant)
        {
            var entity = await persistedGrantRepository.FirstOrDefaultAsync(grant.Key);
            if (entity == null)
            {
                await persistedGrantRepository.InsertAsync(ObjectMapper.Map<PersistedGrantEntity>(grant));
            }
            else
            {
                ObjectMapper.Map(grant, entity);
            }
        }

        [UnitOfWork]
        public virtual async Task<PersistedGrant> GetAsync(string key)
        {
            var entity = await persistedGrantRepository.FirstOrDefaultAsync(key);
            if (entity == null)
            {
                return null;
            }

            return ObjectMapper.Map<PersistedGrant>(entity);
        }

        [UnitOfWork]
        public virtual async Task<IEnumerable<PersistedGrant>> GetAllAsync(string subjectId)
        {
            var entities = await persistedGrantRepository.GetAllListAsync(x => x.SubjectId == subjectId);
            return ObjectMapper.Map<List<PersistedGrant>>(entities);
        }

        [UnitOfWork]
        public virtual async Task RemoveAsync(string key)
        {
            await persistedGrantRepository.DeleteAsync(key);
        }

        [UnitOfWork]
        public virtual async Task RemoveAllAsync(string subjectId, string clientId)
        {
            await persistedGrantRepository.DeleteAsync(x => x.SubjectId == subjectId && x.ClientId == clientId);
        }

        [UnitOfWork]
        public virtual async Task RemoveAllAsync(string subjectId, string clientId, string type)
        {
            await persistedGrantRepository.DeleteAsync(x => x.SubjectId == subjectId && x.ClientId == clientId && x.Type == type);
        }
    }
}
