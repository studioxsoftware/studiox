using System.Web.Caching;
using StudioX.Resources.Embedded;

namespace StudioX.Web.Mvc.Resources.Embedded
{
    public class EmbeddedResourceItemCacheDependency : CacheDependency
    {
        public EmbeddedResourceItemCacheDependency(EmbeddedResourceItem resource)
        {
            SetUtcLastModified(resource.LastModifiedUtc);
        }
    }
}