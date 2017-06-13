using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Application.Features;
using StudioX.Collections.Extensions;
using StudioX.Domain.Repositories;
using StudioX.Domain.Services;
using StudioX.Runtime.Caching;

namespace StudioX.Application.Editions
{
    public class StudioXEditionManager : IDomainService
    {
        private readonly IStudioXZeroFeatureValueStore featureValueStore;

        public IQueryable<Edition> Editions => EditionRepository.GetAll();

        public ICacheManager CacheManager { get; set; }

        public IFeatureManager FeatureManager { get; set; }

        protected IRepository<Edition> EditionRepository { get; set; }

        public StudioXEditionManager(
            IRepository<Edition> editionRepository,
            IStudioXZeroFeatureValueStore featureValueStore)
        {
            this.featureValueStore = featureValueStore;
            EditionRepository = editionRepository;
        }

        public virtual Task<string> GetFeatureValueOrNullAsync(int editionId, string featureName)
        {
            return featureValueStore.GetEditionValueOrNullAsync(editionId, featureName);
        }

        public virtual Task SetFeatureValueAsync(int editionId, string featureName, string value)
        {
            return featureValueStore.SetEditionFeatureValueAsync(editionId, featureName, value);
        }

        public virtual async Task<IReadOnlyList<NameValue>> GetFeatureValuesAsync(int editionId)
        {
            var values = new List<NameValue>();

            foreach (var feature in FeatureManager.GetAll())
            {
                values.Add(new NameValue(feature.Name, await GetFeatureValueOrNullAsync(editionId, feature.Name) ?? feature.DefaultValue));
            }

            return values;
        }

        public virtual async Task SetFeatureValuesAsync(int editionId, params NameValue[] values)
        {
            if (values.IsNullOrEmpty())
            {
                return;
            }

            foreach (var value in values)
            {
                await SetFeatureValueAsync(editionId, value.Name, value.Value);
            }
        }

        public virtual Task CreateAsync(Edition edition)
        {
            return EditionRepository.InsertAsync(edition);
        }

        public virtual Task<Edition> FindByNameAsync(string name)
        {
            return EditionRepository.FirstOrDefaultAsync(edition => edition.Name == name);
        }

        public virtual Task<Edition> FindByIdAsync(int id)
        {
            return EditionRepository.FirstOrDefaultAsync(id);
        }

        public virtual Task<Edition> GetByIdAsync(int id)
        {
            return EditionRepository.GetAsync(id);
        }

        public virtual Task DeleteAsync(Edition edition)
        {
            return EditionRepository.DeleteAsync(edition);
        }
    }
}
