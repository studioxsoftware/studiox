using StudioX.Application.Editions;
using StudioX.Application.Features;
using StudioX.Domain.Repositories;

namespace StudioX.Boilerplate.Editions
{
    public class EditionManager : StudioXEditionManager
    {
        public const string DefaultEditionName = "Standard";

        public EditionManager(
            IRepository<Edition> editionRepository, 
            IStudioXZeroFeatureValueStore featureValueStore)
            : base(
                editionRepository,
                featureValueStore
            )
        {
        }
    }
}
