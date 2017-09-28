using System.Collections.Generic;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Localization;

namespace StudioX.Application.Navigation
{
    internal class NavigationManager : INavigationManager, ISingletonDependency
    {
        public IDictionary<string, MenuDefinition> Menus { get; private set; }

        public MenuDefinition MainMenu => Menus["MainMenu"];

        private readonly IIocResolver iocResolver;
        private readonly INavigationConfiguration configuration;

        public NavigationManager(IIocResolver iocResolver, INavigationConfiguration configuration)
        {
            this.iocResolver = iocResolver;
            this.configuration = configuration;

            Menus = new Dictionary<string, MenuDefinition>
                    {
                        {"MainMenu", new MenuDefinition("MainMenu", new LocalizableString("MainMenu", StudioXConsts.LocalizationSourceName))}
                    };
        }

        public void Initialize()
        {
            var context = new NavigationProviderContext(this);

            foreach (var providerType in configuration.Providers)
            {
                using (var provider = iocResolver.ResolveAsDisposable<NavigationProvider>(providerType))
                {
                    provider.Object.SetNavigation(context);
                }
            }
        }
    }
}