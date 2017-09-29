using System.Linq;
using Microsoft.EntityFrameworkCore;
using StudioX.Application.Editions;
using StudioX.Application.Features;
using StudioX.Boilerplate.Editions;

namespace StudioX.Boilerplate.EntityFrameworkCore.Seed.Host
{
    public class DefaultEditionCreator
    {
        private readonly BoilerplateDbContext context;

        public DefaultEditionCreator(BoilerplateDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            CreateEditions();
        }

        private void CreateEditions()
        {
            var defaultEdition = context.Editions.IgnoreQueryFilters()
                .FirstOrDefault(e => e.Name == EditionManager.DefaultEditionName);
            if (defaultEdition == null)
            {
                defaultEdition = new Edition
                {
                    Name = EditionManager.DefaultEditionName,
                    DisplayName = EditionManager.DefaultEditionName
                };
                context.Editions.Add(defaultEdition);
                context.SaveChanges();

                /* Add desired features to the standard edition, if wanted... */
            }
        }

        private void CreateFeatureIfNotExists(int editionId, string featureName, bool isEnabled)
        {
            var defaultEditionChatFeature = context.EditionFeatureSettings.IgnoreQueryFilters()
                .FirstOrDefault(ef => ef.EditionId == editionId && ef.Name == featureName);

            if (defaultEditionChatFeature == null)
                context.EditionFeatureSettings.Add(new EditionFeatureSetting
                {
                    Name = featureName,
                    Value = isEnabled.ToString(),
                    EditionId = editionId
                });
        }
    }
}