namespace StudioX.Application.Navigation
{
    internal class NavigationProviderContext : INavigationProviderContext
    {
        public INavigationManager Manager { get; private set; }

        public NavigationProviderContext(INavigationManager manager)
        {
            Manager = manager;
        }
    }
}