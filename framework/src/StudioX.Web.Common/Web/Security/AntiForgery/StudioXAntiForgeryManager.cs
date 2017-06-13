using System;
using StudioX.Dependency;
using Castle.Core.Logging;

namespace StudioX.Web.Security.AntiForgery
{
    public class StudioXAntiForgeryManager : IStudioXAntiForgeryManager, IStudioXAntiForgeryValidator, ITransientDependency
    {
        public ILogger Logger { protected get; set; }

        public IStudioXAntiForgeryConfiguration Configuration { get; }

        public StudioXAntiForgeryManager(IStudioXAntiForgeryConfiguration configuration)
        {
            Configuration = configuration;
            Logger = NullLogger.Instance;
        }

        public virtual string GenerateToken()
        {
            return Guid.NewGuid().ToString("D");
        }

        public virtual bool IsValid(string cookieValue, string tokenValue)
        {
            return cookieValue == tokenValue;
        }
    }
}