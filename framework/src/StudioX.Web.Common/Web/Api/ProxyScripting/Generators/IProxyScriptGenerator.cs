using StudioX.Web.Api.Modeling;

namespace StudioX.Web.Api.ProxyScripting.Generators
{
    public interface IProxyScriptGenerator
    {
        string CreateScript(ApplicationApiDescriptionModel model);
    }
}