using System;
using System.IO;
using StudioX.Resources.Embedded;
using Microsoft.Owin.FileSystems;

namespace StudioX.Owin.EmbeddedResources
{
    public class StudioXOwinEmbeddedResourceFileInfo : IFileInfo
    {
        public long Length => _resource.Content.Length;

        public string PhysicalPath => null;

        public string Name => _resource.FileName;

        public DateTime LastModified => _resource.LastModifiedUtc;

        public bool IsDirectory => false;

        private readonly EmbeddedResourceItem _resource;

        public StudioXOwinEmbeddedResourceFileInfo(EmbeddedResourceItem resource)
        {
            _resource = resource;
        }

        public Stream CreateReadStream()
        {
            return new MemoryStream(_resource.Content);
        }
    }
}