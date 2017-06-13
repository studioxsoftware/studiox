using System;
using StudioX.Dependency;
using StudioX.Resources.Embedded;
using StudioX.Web.Configuration;
using Microsoft.Extensions.FileProviders;
using Microsoft.Extensions.Primitives;

namespace StudioX.AspNetCore.EmbeddedResources
{
    public class EmbeddedResourceFileProvider : IFileProvider
    {
        private readonly Lazy<IEmbeddedResourceManager> embeddedResourceManager;
        private readonly Lazy<IWebEmbeddedResourcesConfiguration> configuration;

        public EmbeddedResourceFileProvider(IIocResolver iocResolver)
        {
            embeddedResourceManager = new Lazy<IEmbeddedResourceManager>(
                () => iocResolver.Resolve<IEmbeddedResourceManager>(),
                true
            );

            configuration = new Lazy<IWebEmbeddedResourcesConfiguration>(
                () => iocResolver.Resolve<IWebEmbeddedResourcesConfiguration>(),
                true
            );
        }

        public IFileInfo GetFileInfo(string subpath)
        {
            var resource = embeddedResourceManager.Value.GetResource(subpath);

            if (resource == null || IsIgnoredFile(resource))
            {
                return new NotFoundFileInfo(subpath);
            }

            return new EmbeddedResourceItemFileInfo(resource);
        }

        public IDirectoryContents GetDirectoryContents(string subpath)
        {
            //TODO: Implement...?

            return new NotFoundDirectoryContents();
        }

        public IChangeToken Watch(string filter)
        {
            return NullChangeToken.Singleton;
        }

        protected virtual bool IsIgnoredFile(EmbeddedResourceItem resource)
        {
            return resource.FileExtension != null && configuration.Value.IgnoredFileExtensions.Contains(resource.FileExtension);
        }
    }
}