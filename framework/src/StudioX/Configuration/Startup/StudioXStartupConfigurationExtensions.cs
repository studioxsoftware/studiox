using System;
using StudioX.Dependency;

namespace StudioX.Configuration.Startup
{
    /// <summary>
    ///     Extension methods for <see cref="IStudioXStartupConfiguration" />.
    /// </summary>
    public static class StudioXStartupConfigurationExtensions
    {
        /// <summary>
        ///     Used to replace a service type.
        /// </summary>
        /// <param name="configuration">The configuration.</param>
        /// <param name="type">Type.</param>
        /// <param name="impl">Implementation.</param>
        /// <param name="lifeStyle">Life style.</param>
        public static void ReplaceService(this IStudioXStartupConfiguration configuration, Type type, Type impl,
            DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
        {
            configuration.ReplaceService(type, () => { configuration.IocManager.Register(type, impl, lifeStyle); });
        }

        /// <summary>
        ///     Used to replace a service type.
        /// </summary>
        /// <typeparam name="TType">Type of the service.</typeparam>
        /// <typeparam name="TImpl">Type of the implementation.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="lifeStyle">Life style.</param>
        public static void ReplaceService<TType, TImpl>(this IStudioXStartupConfiguration configuration,
            DependencyLifeStyle lifeStyle = DependencyLifeStyle.Singleton)
            where TType : class
            where TImpl : class, TType
        {
            configuration.ReplaceService(typeof(TType),
                () => { configuration.IocManager.Register<TType, TImpl>(lifeStyle); });
        }


        /// <summary>
        ///     Used to replace a service type.
        /// </summary>
        /// <typeparam name="TType">Type of the service.</typeparam>
        /// <param name="configuration">The configuration.</param>
        /// <param name="replaceAction">Replace action.</param>
        public static void ReplaceService<TType>(this IStudioXStartupConfiguration configuration, Action replaceAction)
            where TType : class
        {
            configuration.ReplaceService(typeof(TType), replaceAction);
        }
    }
}