using StudioX.Application.Navigation;
using StudioX.Collections;

namespace StudioX.Configuration.Startup
{
    internal class NavigationConfiguration : INavigationConfiguration
    {
        public ITypeList<NavigationProvider> Providers { get; }

        public NavigationConfiguration()
        {
            Providers = new TypeList<NavigationProvider>();
        }
    }
}