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
        private readonly IApiProxyScriptManager _proxyScriptManager;

        public StudioXServiceProxiesController(IApiProxyScriptManager proxyScriptManager)
        {
            _proxyScriptManager = proxyScriptManager;
        }

        [Produces("text/javascript", "text/plain")]
        public string GetAll(ApiProxyGenerationModel model)
        {
            return _proxyScriptManager.GetScript(model.CreateOptions());
        }
    }
}
