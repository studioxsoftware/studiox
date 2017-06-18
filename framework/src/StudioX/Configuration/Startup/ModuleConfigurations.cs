namespace StudioX.Configuration.Startup
{
    internal class ModuleConfigurations : IModuleConfigurations
    {
        public IStudioXStartupConfiguration StudioXConfiguration { get; }

        public ModuleConfigurations(IStudioXStartupConfiguration startupConfiguration)
        {
            StudioXConfiguration = startupConfiguration;
        }
    }
}