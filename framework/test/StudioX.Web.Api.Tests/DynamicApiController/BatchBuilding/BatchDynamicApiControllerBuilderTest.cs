using StudioX.TestBase;
using StudioX.WebApi.Controllers.Dynamic;
using Shouldly;
using Xunit;

namespace StudioX.Web.Api.Tests.DynamicApiController.BatchBuilding
{
    public class BatchDynamicApiControllerBuilderTest : StudioXIntegratedTestBase<StudioXWebApiTestModule>
    {
        [Fact]
        public void Test1()
        {
            lock (this)
            {
                var services = LocalIocManager.Resolve<DynamicApiControllerManager>().GetAll();
                services.Count.ShouldBe(1);
                services[0].ServiceName.ShouldBe("myapp/myFirst");
                services[0].Actions.Count.ShouldBe(1);
                services[0].Actions.ContainsKey("GetStr").ShouldBe(true);
                services[0].IsProxyScriptingEnabled.ShouldBeFalse();
            }
        }
    }
}