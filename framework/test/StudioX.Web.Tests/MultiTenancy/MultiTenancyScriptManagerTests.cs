using StudioX.Configuration.Startup;
using StudioX.Web.MultiTenancy;
using Shouldly;
using Xunit;

namespace StudioX.Web.Tests.MultiTenancy
{
    public class MultiTenancyScriptManagerTests
    {
        [Fact]
        public void ShouldGetScript()
        {
            var scriptManager = new MultiTenancyScriptManager(new MultiTenancyConfig {IsEnabled = true});
            var script = scriptManager.GetScript();
            script.ShouldNotBe(null);
        }
    }
}
