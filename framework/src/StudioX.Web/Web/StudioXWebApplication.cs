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

        /// <summary>
        /// This method is called by ASP.NET system on web application's startup.
        /// </summary>
        protected virtual void Application_Start(object sender, EventArgs e)
        {
            ThreadCultureSanitizer.Sanitize();
            StudioXBootstrapper.Initialize();
        }

        /// <summary>
        /// This method is called by ASP.NET system on web application shutdown.
        /// </summary>
        protected virtual void Application_End(object sender, EventArgs e)
        {
            StudioXBootstrapper.Dispose();
        }

        /// <summary>
        /// This method is called by ASP.NET system when a session starts.
        /// </summary>
        protected virtual void Session_Start(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method is called by ASP.NET system when a session ends.
        /// </summary>
        protected virtual void Session_End(object sender, EventArgs e)
        {

        }

        /// <summary>
        /// This method is called by ASP.NET system when a request starts.
        /// </summary>
        protected virtual void Application_BeginRequest(object sender, EventArgs e)
        {
            SetCurrentCulture();
        }

        protected virtual void SetCurrentCulture()
        {
            StudioXBootstrapper.IocManager.Using<ICurrentCultureSetter>(cultureSetter => cultureSetter.SetCurrentCulture(Context));
        }

        /// <summary>
        /// This method is called by ASP.NET system when a request ends.
        /// </summary>
        protected virtual void Application_EndRequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected virtual void Application_Error(object sender, EventArgs e)
        {

        }
    }
}
