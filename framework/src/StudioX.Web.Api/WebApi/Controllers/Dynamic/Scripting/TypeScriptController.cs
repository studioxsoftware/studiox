using System.Net.Http;
using System.Net.Http.Headers;
using StudioX.Auditing;
using StudioX.Web.Models;
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
        readonly TypeScriptDefinitionGenerator typeScriptDefinitionGenerator;
        readonly TypeScriptServiceGenerator typeScriptServiceGenerator;
        public TypeScriptController(TypeScriptDefinitionGenerator typeScriptDefinitionGenerator, TypeScriptServiceGenerator typeScriptServiceGenerator)
        {
            this.typeScriptDefinitionGenerator = typeScriptDefinitionGenerator;
            this.typeScriptServiceGenerator = typeScriptServiceGenerator;
        }
        
        public HttpResponseMessage Get(bool isCompleteService = false)
        {
            if (isCompleteService)
            {
                var script = typeScriptServiceGenerator.GetScript();
                var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
                return response;
            }
            else
            {
                var script = typeScriptDefinitionGenerator.GetScript();
                var response = Request.CreateResponse(System.Net.HttpStatusCode.OK, script, new PlainTextFormatter());
                response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/x-javascript");
                return response;
            }
        }
    }
}
