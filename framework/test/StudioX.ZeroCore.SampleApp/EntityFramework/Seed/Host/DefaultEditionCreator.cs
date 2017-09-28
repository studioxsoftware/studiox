using System.Linq;
using StudioX.Application.Editions;
using StudioX.Application.Features;
using StudioX.ZeroCore.SampleApp.Application;
using StudioX.ZeroCore.SampleApp.Core;

namespace StudioX.ZeroCore.SampleApp.EntityFramework.Seed.Host
{
    public class DefaultEditionCreator
    {
        private readonly SampleAppDbContext context;

        public DefaultEditionCreator(SampleAppDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            CreateEditions();
        }

        private void CreateEditions()
        {
            var defaultEdition = context.Editions.FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
            if (defaultEdition == null)
            {
                defaultEdition = new Edition { Name = EditionManager.DefaultEditionName, DisplayName = EditionManager.DefaultEditionName };
                context.Editions.Add(defaultEdition);
                context.SaveChanges();
            }

            if (defaultEdition.Id > 0)
            {
                CreateFeatureIfNotExists(defaultEdition.Id, AppFeatures.SimpleBooleanFeature, true);
            }
        }

        private void CreateFeatureIfNotExists(int editionId, string featureName, bool isEnabled)
        {
            var defaultEditionChatFeature = context.EditionFeatureSettings
                                                        .FirstOrDefault(ef => ef.EditionId == editionId && ef.Name == featureName);

            if (defaultEditionChatFeature == null)
            {
                context.EditionFeatureSettings.Add(new EditionFeatureSetting
                {
                    Name = featureName,
                    Value = isEnabled.ToString(),
                    EditionId = editionId
                });
            }
        }
    }
}