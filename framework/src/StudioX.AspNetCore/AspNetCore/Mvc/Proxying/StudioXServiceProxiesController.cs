using StudioX.AspNetCore.Mvc.Controllers;
using StudioX.Auditing;
using StudioX.Web.Api.ProxyScripting;
using StudioX.Web.Models;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.AspNetCore.Mvc.Proxying
{
    [DontWrapResult]
    [DisableAuditing]
    public class StudioXServiceProxiesController : StudioXController
    {
        private readonly IApiProxyScriptManager proxyScriptManager;

        public StudioXServiceProxiesController(IApiProxyScriptManager proxyScriptManager)
        {
            this.proxyScriptManager = proxyScriptManager;
        }

        [Produces("text/javascript")]
        public string GetAll(ApiProxyGenerationModel model)
        {
            return proxyScriptManager.GetScript(model.CreateOptions());
        }
    }
}
