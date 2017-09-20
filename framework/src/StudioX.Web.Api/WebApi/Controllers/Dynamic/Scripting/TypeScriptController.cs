using System.Net.Http;
using System.Net.Http.Headers;
using StudioX.Auditing;
using StudioX.Web.Models;
using StudioX.Web.Security;
using StudioX.Web.Security.AntiForgery;
using StudioX.WebApi.Controllers.Dynamic.Formatters;
using StudioX.WebApi.Controllers.Dynamic.Scripting.TypeScript;

namespace StudioX.WebApi.Controllers.Dynamic.Scripting
{
    [DontWrapResult]
    [DisableAuditing]
    [DisableStudioXAntiForgeryTokenValidation]
    public class TypeScriptController : StudioXApiController
    {
        readonly TypeScriptDefinitionGenerator _typeScriptDefinitionGenerator;
        readonly TypeScriptServiceGenerator _typeScriptServiceGenerator;
        public TypeScriptController(TypeScriptDefinitionGenerator typeScriptDefinitionGenerator, TypeScriptServiceGenerator typeScriptServiceGenerator)
        {
            _typeScriptDefinitionGenerator = typeScriptDefinitionGenerator;
            _typeScriptServiceGenerator = typeScriptServiceGenerator;
        }
        
        public HttpResponseMessage Get(bool isCompleteService = false)
        {
            if (isCompleteService)
            {
                var script = _typeScriptServiceGenerator.GetScript();
                var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
                return response;
            }
            else
            {
                var script = _typeScriptDefinitionGenerator.GetScript();
                var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
                return response;
            }
        }
    }
}
