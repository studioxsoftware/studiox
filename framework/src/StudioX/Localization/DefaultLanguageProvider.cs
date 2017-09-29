using System.Collections.Generic;
using System.Collections.Immutable;
using StudioX.Configuration.Startup;
using StudioX.Dependency;

namespace StudioX.Localization
{
    public class DefaultLanguageProvider : ILanguageProvider, ITransientDependency
    {
        private readonly ILocalizationConfiguration configuration;

        public DefaultLanguageProvider(ILocalizationConfiguration configuration)
        {
            this.configuration = configuration;
        }

        public IReadOnlyList<LanguageInfo> GetLanguages()
        {
            return configuration.Languages.ToImmutableList();
        }
    }
}