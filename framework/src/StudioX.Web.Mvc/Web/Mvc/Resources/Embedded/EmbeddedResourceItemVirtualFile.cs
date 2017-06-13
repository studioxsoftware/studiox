using System.IO;
using System.Web.Hosting;
using StudioX.Resources.Embedded;

namespace StudioX.Web.Mvc.Resources.Embedded
{
    public class EmbeddedResourceItemVirtualFile : VirtualFile
    {
        private readonly EmbeddedResourceItem resource;

        public EmbeddedResourceItemVirtualFile(string virtualPath, EmbeddedResourceItem resource)
            : base(virtualPath)
        {
            this.resource = resource;
        }

        public override Stream Open()
        {
            return new MemoryStream(resource.Content);
        }
    }
}