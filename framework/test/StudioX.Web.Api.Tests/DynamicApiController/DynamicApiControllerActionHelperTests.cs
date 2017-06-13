using StudioX.Application.Services;
using StudioX.WebApi.Controllers.Dynamic.Builders;
using Shouldly;
using Xunit;

namespace StudioX.Web.Api.Tests.DynamicApiController
{
    public class DynamicApiControllerActionHelperTests
    {
        [Fact]
        public void ShouldFindRightMethods()
        {
            var methods = DynamicApiControllerActionHelper.GetMethodsOfType(typeof(IMyApplicationService));
            methods.Count.ShouldBe(4);
            foreach (var method in methods)
            {
                DynamicApiControllerActionHelper.IsMethodOfType(method, typeof(IMyApplicationService)).ShouldBe(true);
            }
        }

        private interface IMyApplicationService : IMyBaseAppService
        {
            void MyMethod1();

            int MyMethod2();

            void MyMethod3(string arg1);
        }

        private interface IMyBaseAppService : IApplicationService
        {
            int MyBaseMethod();            
        }
    }
}