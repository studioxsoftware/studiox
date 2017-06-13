using System.Collections.Generic;

namespace StudioX.Resources.Embedded
{
    public interface IEmbeddedResourcesConfiguration
    {
        List<EmbeddedResourceSet> Sources { get; }
    }
}