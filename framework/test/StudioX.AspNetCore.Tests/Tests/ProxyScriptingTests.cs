using System.Threading.Tasks;
using StudioX.AspNetCore.Mvc.Proxying;
using StudioX.Web.Api.ProxyScripting.Generators.JQuery;
using Shouldly;
using Xunit;

namespace StudioX.AspNetCore.Tests
{
    public class ProxyScriptingTests : AppTestBase
    {
        [Fact]
        public async Task jQueryScriptingSimpleTest()
        {
            // Act
            var response = await GetResponseAsStringAsync(
                GetUrl<StudioXServiceProxiesController>(
                    nameof(StudioXServiceProxiesController.GetAll),
                    new { type = JQueryProxyScriptGenerator.Name }
                )
            );

            response.ShouldNotBeNullOrEmpty();
        }
    }
}
