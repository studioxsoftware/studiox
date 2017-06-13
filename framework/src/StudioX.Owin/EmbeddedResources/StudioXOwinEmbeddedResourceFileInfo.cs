using System;
using System.IO;
using StudioX.Resources.Embedded;
using Microsoft.Owin.FileSystems;

namespace StudioX.Owin.EmbeddedResources
{
    public class StudioXOwinEmbeddedResourceFileInfo : IFileInfo
    {
        public long Length => resource.Content.Length;

        public string PhysicalPath => null;

        public string Name => resource.FileName;

        public DateTime LastModified => resource.LastModifiedUtc;

        public bool IsDirectory => false;

        private readonly EmbeddedResourceItem resource;

        public StudioXOwinEmbeddedResourceFileInfo(EmbeddedResourceItem resource)
        {
            this.resource = resource;
        }

        public Stream CreateReadStream()
        {
            return new MemoryStream(resource.Content);
        }
    }
}