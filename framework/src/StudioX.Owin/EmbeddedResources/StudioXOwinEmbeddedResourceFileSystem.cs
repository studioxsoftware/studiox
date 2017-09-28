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
        private readonly IEmbeddedResourceManager _embeddedResourceManager;
        private readonly IWebEmbeddedResourcesConfiguration _configuration;
        private readonly IFileSystem _physicalFileSystem;

        public StudioXOwinEmbeddedResourceFileSystem(
            IEmbeddedResourceManager embeddedResourceManager,
            IWebEmbeddedResourcesConfiguration configuration,
            string rootFolder)
        {
            _embeddedResourceManager = embeddedResourceManager;
            _configuration = configuration;
            _physicalFileSystem = new PhysicalFileSystem(rootFolder);
        }

        public bool TryGetFileInfo(string subpath, out IFileInfo fileInfo)
        {
            if (_physicalFileSystem.TryGetFileInfo(subpath, out fileInfo))
            {
                return true;
            }

            var resource = _embeddedResourceManager.GetResource(subpath);

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
            if (_physicalFileSystem.TryGetDirectoryContents(subpath, out contents))
            {
                return true;
            }

            //TODO: Implement..?

            contents = null;
            return false;
        }

        private bool IsIgnoredFile(EmbeddedResourceItem resource)
        {
            return resource.FileExtension != null && _configuration.IgnoredFileExtensions.Contains(resource.FileExtension);
        }
    }
}