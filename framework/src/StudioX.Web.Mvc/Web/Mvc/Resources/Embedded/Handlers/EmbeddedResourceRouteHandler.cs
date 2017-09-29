using System;
using System.Web;
using System.Web.Routing;

namespace StudioX.Web.Mvc.Resources.Embedded.Handlers
{
    [Obsolete]
    internal class EmbeddedResourceRouteHandler : IRouteHandler
    {
        private readonly string rootPath;

        public EmbeddedResourceRouteHandler(string rootPath)
        {
            this.rootPath = rootPath;
        }

        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            return new EmbeddedResourceHttpHandler(rootPath, requestContext.RouteData);
        }
    }
}