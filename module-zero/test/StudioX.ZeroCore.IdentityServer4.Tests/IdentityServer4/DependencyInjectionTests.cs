using Xunit;

namespace StudioX.IdentityServer4
{
    public class DependencyInjectionTests: StudioXZeroIdentityServerTestBase
    {
        [Fact]
        public void ShouldInjectStudioXPersistedGrantStore()
        {
            Resolve<StudioXPersistedGrantStore>();
        }
    }
}
