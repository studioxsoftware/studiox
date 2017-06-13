using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using StudioX.Configuration.Startup;

namespace StudioX.Runtime.Caching.Configuration
{
    internal class CachingConfiguration : ICachingConfiguration
    {
        public IStudioXStartupConfiguration StudioXConfiguration { get; private set; }

        public IReadOnlyList<ICacheConfigurator> Configurators => configurators.ToImmutableList();
        private readonly List<ICacheConfigurator> configurators;

        public CachingConfiguration(IStudioXStartupConfiguration startupConfiguration)
        {
            StudioXConfiguration = startupConfiguration;
            configurators = new List<ICacheConfigurator>();
        }

        public void ConfigureAll(Action<ICache> initAction)
        {
            configurators.Add(new CacheConfigurator(initAction));
        }

        public void Configure(string cacheName, Action<ICache> initAction)
        {
            configurators.Add(new CacheConfigurator(cacheName, initAction));
        }
    }
}