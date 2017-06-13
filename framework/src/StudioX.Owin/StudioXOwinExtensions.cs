using System;
using System.Web;
using StudioX.Dependency;
using StudioX.Modules;
using StudioX.Owin.EmbeddedResources;
using StudioX.Resources.Embedded;
using StudioX.Threading;
using StudioX.Web.Configuration;
using JetBrains.Annotations;
using Microsoft.Owin.StaticFiles;
using Owin;

namespace StudioX.Owin
{
    /// <summary>
    /// OWIN extension methods for StudioX.
    /// </summary>
    public static class StudioXOwinExtensions
    {
        /// <summary>
        /// This should be called as the first line for OWIN based applications for StudioX framework.
        /// </summary>
        /// <param name="app">The application.</param>
        public static void UseStudioX(this IAppBuilder app)
        {
            app.UseStudioX(null);
        }

        public static void UseStudioX(this IAppBuilder app, [CanBeNull] Action<StudioXOwinOptions> optionsAction)
        {
            ThreadCultureSanitizer.Sanitize();

            var options = new StudioXOwinOptions
            {
                UseEmbeddedFiles = HttpContext.Current?.Server != null
            };

            optionsAction?.Invoke(options);

            if (options.UseEmbeddedFiles)
            {
                if (HttpContext.Current?.Server == null)
                {
                    throw new StudioXInitializationException(@"Can not enable UseEmbeddedFiles for OWIN since HttpContext.Current is null! 
                            If you are using ASP.NET Core, serve embedded resources through ASP.NET Core middleware instead of OWIN.");
                }

                app.UseStaticFiles(new StaticFileOptions
                {
                    FileSystem = new StudioXOwinEmbeddedResourceFileSystem(
                        IocManager.Instance.Resolve<IEmbeddedResourceManager>(),
                        IocManager.Instance.Resolve<IWebEmbeddedResourcesConfiguration>(),
                        HttpContext.Current.Server.MapPath("~/")
                    )
                });
            }
        }

        /// <summary>
        /// Use this extension method if you don't initialize StudioX in other way.
        /// Otherwise, use <see cref="UseStudioX(IAppBuilder)"/>.
        /// </summary>
        /// <param name="app">The application.</param>
        /// <typeparam name="TStartupModule">The type of the startup module.</typeparam>
        public static void UseStudioX<TStartupModule>(this IAppBuilder app)
            where TStartupModule : StudioXModule
        {
            app.UseStudioX<TStartupModule>(null, null);
        }

        /// <summary>
        /// Use this extension method if you don't initialize StudioX in other way.
        /// Otherwise, use <see cref="UseStudioX(IAppBuilder)"/>.
        /// </summary>
        /// <typeparam name="TStartupModule">The type of the startup module.</typeparam>
        public static void UseStudioX<TStartupModule>(this IAppBuilder app, [CanBeNull] Action<StudioXBootstrapper> configureAction, [CanBeNull] Action<StudioXOwinOptions> optionsAction = null)
            where TStartupModule : StudioXModule
        {
            app.UseStudioX(optionsAction);

            if (!app.Properties.ContainsKey("_StudioXBootstrapper.Instance"))
            {
                var bootstrapper = StudioXBootstrapper.Create<TStartupModule>();
                app.Properties["_StudioXBootstrapper.Instance"] = bootstrapper;
                configureAction?.Invoke(bootstrapper);
                bootstrapper.Initialize();
            }
        }
    }
}