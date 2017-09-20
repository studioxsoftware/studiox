using System;
using System.Web;
using StudioX.Dependency;
using StudioX.Modules;
using StudioX.Threading;
using StudioX.Web.Localization;

namespace StudioX.Web
{
    /// <summary>
    /// This class is used to simplify starting of StudioX system using <see cref="StudioXBootstrapper"/> class..
    /// Inherit from this class in global.asax instead of <see cref="HttpApplication"/> to be able to start StudioX system.
    /// </summary>
    /// <typeparam name="TStartupModule">Startup module of the application which depends on other used modules. Should be derived from <see cref="StudioXModule"/>.</typeparam>
    public abstract class StudioXWebApplication<TStartupModule> : HttpApplication
        where TStartupModule : StudioXModule
    {
        /// <summary>
        /// Gets a reference to the <see cref="StudioXBootstrapper"/> instance.
        /// </summary>
        public static StudioXBootstrapper StudioXBootstrapper { get; } = StudioXBootstrapper.Create<TStartupModule>();

        protected virtual void Application_Start(object sender, EventArgs e)
        {
            ThreadCultureSanitizer.Sanitize();
            StudioXBootstrapper.Initialize();
        }

        protected virtual void Application_End(object sender, EventArgs e)
        {
            StudioXBootstrapper.Dispose();
        }

        protected virtual void Session_Start(object sender, EventArgs e)
        {

        }

        protected virtual void Session_End(object sender, EventArgs e)
        {

        }

        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
            
        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_PostAuthenticateRequest(object sender, EventArgs e)
        {
            SetCurrentCulture();
        }

        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {

        }

        protected virtual void SetCurrentCulture()
        {
            StudioXBootstrapper.IocManager.Using<ICurrentCultureSetter>(cultureSetter => cultureSetter.SetCurrentCulture(Context));
        }
    }
}
