using System.Collections.Generic;
using System.Web;
using StudioX.Dependency;
using StudioX.Resources.Embedded;
using StudioX.Web.Configuration;
using Microsoft.Owin.FileSystems;

namespace StudioX.Owin.EmbeddedResources
{
    public class StudioXOwinEmbeddedResourceFileSystem : IFileSystem, ITransientDependency
    {
        private readonly IEmbeddedResourceManager embeddedResourceManager;
        private readonly IWebEmbeddedResourcesConfiguration configuration;
        private readonly IFileSystem physicalFileSystem;

        public StudioXOwinEmbeddedResourceFileSystem(
            IEmbeddedResourceManager embeddedResourceManager,
            IWebEmbeddedResourcesConfiguration configuration,
            string rootFolder)
        {
            this.embeddedResourceManager = embeddedResourceManager;
            this.configuration = configuration;
            physicalFileSystem = new PhysicalFileSystem(rootFolder);
        }

        public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
        {
            if (physicalFileSystem.TryGetFileInfo(subpath, out fileInfo))
            {
                return true;
            }

            var resource = embeddedResourceManager.GetResource(subpath);

            if (resource == null || IsIgnoredFile(resource))
            {
                fileInfo = null;
                return false;
            }

            fileInfo = new StudioXOwinEmbeddedResourceFileInfo(resource);
            return true;
        }

        public bool TryGetDirectoryContents(string subpath, out IEnumerable<IFileInfo> contents)
        {
            if (physicalFileSystem.TryGetDirectoryContents(subpath, out contents))
            {
                return true;
            }

            //TODO: Implement..?

            contents = null;
            return false;
        }

        private bool IsIgnoredFile(EmbeddedResourceItem resource)
        {
            return resource.FileExtension != null && configuration.IgnoredFileExtensions.Contains(resource.FileExtension);
        }
    }
}