namespace StudioX.Configuration.Startup
{
    internal class ModuleConfigurations : IModuleConfigurations
    {
        public IStudioXStartupConfiguration StudioXConfiguration { get; private set; }

        public ModuleConfigurations(IStudioXStartupConfiguration studioxConfiguration)
        {
            StudioXConfiguration = studioxConfiguration;
        }
    }
}