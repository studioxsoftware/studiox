namespace StudioX.Application.Navigation
{
    internal class NavigationProviderContext : INavigationProviderContext
    {
        public INavigationManager Manager { get; }

        public NavigationProviderContext(INavigationManager manager)
        {
            Manager = manager;
        }
    }
}