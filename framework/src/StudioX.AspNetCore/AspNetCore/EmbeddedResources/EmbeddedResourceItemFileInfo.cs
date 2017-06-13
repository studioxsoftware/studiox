using System;
using System.IO;
using StudioX.Resources.Embedded;
using Microsoft.Extensions.FileProviders;

namespace StudioX.AspNetCore.EmbeddedResources
{
    public class EmbeddedResourceItemFileInfo : IFileInfo
    {
        public bool Exists => true;

        public long Length => resourceItem.Content.Length;

        public string PhysicalPath => null;

        public string Name => resourceItem.FileName;

        public DateTimeOffset LastModified => resourceItem.LastModifiedUtc;

        public bool IsDirectory => false;
        
        private readonly EmbeddedResourceItem resourceItem;

        public EmbeddedResourceItemFileInfo(EmbeddedResourceItem resourceItem)
        {
            this.resourceItem = resourceItem;
        }

        public Stream CreateReadStream()
        {
            return new MemoryStream(resourceItem.Content);
        }
    }
}