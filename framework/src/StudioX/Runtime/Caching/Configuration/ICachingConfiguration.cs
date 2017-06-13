using System;
using System.Collections.Generic;
using StudioX.Configuration.Startup;

namespace StudioX.Runtime.Caching.Configuration
{
    /// <summary>
    /// Used to configure caching system.
    /// </summary>
    public interface ICachingConfiguration
    {
        /// <summary>
        /// Gets the StudioX configuration object.
        /// </summary>
        IStudioXStartupConfiguration StudioXConfiguration { get; }

        /// <summary>
        /// List of all registered configurators.
        /// </summary>
        IReadOnlyList<ICacheConfigurator> Configurators { get; }

        /// <summary>
        /// Used to configure all caches.
        /// </summary>
        /// <param name="initAction">
        /// An action to configure caches
        /// This action is called for each cache just after created.
        /// </param>
        void ConfigureAll(Action<ICache> initAction);

        /// <summary>
        /// Used to configure a specific cache. 
        /// </summary>
        /// <param name="cacheName">Cache name</param>
        /// <param name="initAction">
        /// An action to configure the cache.
        /// This action is called just after the cache is created.
        /// </param>
        void Configure(string cacheName, Action<ICache> initAction);
    }
}
