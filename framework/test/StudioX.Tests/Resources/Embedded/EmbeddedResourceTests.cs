using StudioX.Reflection.Extensions;
using StudioX.Resources.Embedded;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Resources.Embedded
{
    public class EmbeddedResourceTests
    {
        private readonly IEmbeddedResourceManager embeddedResourceManager;

        public EmbeddedResourceTests()
        {
            var configuration = new EmbeddedResourcesConfiguration();

            configuration.Sources.Add(
                new EmbeddedResourceSet(
                    "/MyApp/MyResources/", GetType().GetAssembly(), "StudioX.Tests.Resources.Embedded.MyResources"
                )
            );

            embeddedResourceManager = new EmbeddedResourceManager(configuration);
        }

        [Fact]
        public void ShouldDefineAndGetEmbeddedResources()
        {
            var resource = embeddedResourceManager.GetResource("/MyApp/MyResources/js/MyScriptFile1.js");

            resource.ShouldNotBeNull();
            Assert.True(resource.Assembly == GetType().GetAssembly());
            Assert.True(resource.Content.Length > 0);
        }
    }
}
