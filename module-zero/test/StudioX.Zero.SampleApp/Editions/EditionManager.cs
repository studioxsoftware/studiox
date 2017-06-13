using StudioX.Application.Editions;
using StudioX.Application.Features;
using StudioX.Domain.Repositories;

namespace StudioX.Zero.SampleApp.Editions
{
    public class EditionManager : StudioXEditionManager
    {
        public EditionManager(
            IRepository<Edition> editionRepository,
            IStudioXZeroFeatureValueStore featureValueStore)
            : base(
               editionRepository,
               featureValueStore)
        {
        }
    }
}