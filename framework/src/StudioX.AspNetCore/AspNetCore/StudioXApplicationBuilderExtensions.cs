using System;
using System.Linq;
using StudioX.AspNetCore.EmbeddedResources;
using StudioX.AspNetCore.Localization;
using StudioX.Dependency;
using StudioX.Localization;
using Castle.LoggingFacility.MsLogging;
using JetBrains.Annotations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Localization;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace StudioX.AspNetCore
{
	public static class StudioXApplicationBuilderExtensions
    {
        public static void UseStudioX(this IApplicationBuilder app)
        {
            app.UseStudioX(null);
        }

	    public static void UseStudioX([NotNull] this IApplicationBuilder app, Action<StudioXApplicationBuilderOptions> optionsAction)
	    {
		    Check.NotNull(app, nameof(app));

	        var options = new StudioXApplicationBuilderOptions();
            optionsAction?.Invoke(options);

            if (options.UseCastleLoggerFactory)
		    {
			    app.UseCastleLoggerFactory();
			}

			InitializeStudioX(app);

		    if (options.UseStudioXRequestLocalization)
		    {
                //TODO: This should be added later than authorization middleware!
			    app.UseStudioXRequestLocalization();
		    }
	    }

		public static void UseEmbeddedFiles(this IApplicationBuilder app)
        {
            app.UseStaticFiles(
                new StaticFileOptions
                {
                    FileProvider = new EmbeddedResourceFileProvider(
                        app.ApplicationServices.GetRequiredService<IIocResolver>()
                    )
                }
            );
        }

        private static void InitializeStudioX(IApplicationBuilder app)
        {
            var bootstrapper = app.ApplicationServices.GetRequiredService<StudioXBootstrapper>();
            bootstrapper.Initialize();
        }

        public static void UseCastleLoggerFactory(this IApplicationBuilder app)
        {
            var castleLoggerFactory = app.ApplicationServices.GetService<Castle.Core.Logging.ILoggerFactory>();
            if (castleLoggerFactory == null)
            {
                return;
            }

            app.ApplicationServices
                .GetRequiredService<ILoggerFactory>()
                .AddCastleLogger(castleLoggerFactory);
        }

        public static void UseStudioXRequestLocalization(this IApplicationBuilder app, Action<RequestLocalizationOptions> optionsAction = null)
        {
            var iocResolver = app.ApplicationServices.GetRequiredService<IIocResolver>();
            using (var languageManager = iocResolver.ResolveAsDisposable<ILanguageManager>())
            {
                var supportedCultures = languageManager.Object
                    .GetLanguages()
                    .Select(l => CultureInfoHelper.Get(l.Name))
                    .ToArray();

                var options = new RequestLocalizationOptions
                {
                    SupportedCultures = supportedCultures,
                    SupportedUICultures = supportedCultures
                };

                var userProvider = new StudioXUserRequestCultureProvider();
                
                //0: QueryStringRequestCultureProvider
                options.RequestCultureProviders.Insert(1, userProvider);
                options.RequestCultureProviders.Insert(2, new StudioXLocalizationHeaderRequestCultureProvider());
                //3: CookieRequestCultureProvider
                options.RequestCultureProviders.Insert(4, new StudioXDefaultRequestCultureProvider());
                //5: AcceptLanguageHeaderRequestCultureProvider

                optionsAction?.Invoke(options);

                userProvider.CookieProvider = options.RequestCultureProviders.OfType<CookieRequestCultureProvider>().FirstOrDefault();
                userProvider.HeaderProvider = options.RequestCultureProviders.OfType<StudioXLocalizationHeaderRequestCultureProvider>().FirstOrDefault();

                app.UseRequestLocalization(options);
            }
        }
    }
}
