using StudioX.Application.Services;

namespace StudioXAspNetCoreDemo.Core.Application
{
    public class DemoAppServiceBase : ApplicationService
    {
        public DemoAppServiceBase()
        {
            LocalizationSourceName = "StudioXAspNetCoreDemoModule";
        }
    }
}
