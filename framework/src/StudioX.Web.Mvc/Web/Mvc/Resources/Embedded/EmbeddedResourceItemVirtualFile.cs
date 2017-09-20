using System.IO;
using System.Web.Hosting;
using StudioX.Resources.Embedded;

namespace StudioX.Web.Mvc.Resources.Embedded
{
    public class EmbeddedResourceItemVirtualFile : VirtualFile
    {
        private readonly EmbeddedResourceItem _resource;

        public EmbeddedResourceItemVirtualFile(string virtualPath, EmbeddedResourceItem resource)
            : base(virtualPath)
        {
            _resource = resource;
        }

        public override Stream Open()
        {
            return new MemoryStream(_resource.Content);
        }
    }
}