using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Dependency;
using Shouldly;
using Xunit;

namespace StudioX.TestBase.Tests.Application.Services
{
    public class ServiceWithDifferentInputsTests : StudioXIntegratedTestBase<StudioXKernelModule>
    {
        private readonly IMyAppService appService;

        public ServiceWithDifferentInputsTests()
        {
            LocalIocManager.Register<IMyAppService, MyAppService>(DependencyLifeStyle.Transient);
            appService = Resolve<IMyAppService>();
        }

        [Fact]
        public async Task GetsExpressionReturnsGenericAsyncTest()
        {
            var result = await appService.GetsExpressionReturnsGenericAsync<MyEmptyDto>(t => t != null);
            result.ShouldBeOfType(typeof(MyEmptyDto));
        }

        #region Application Service

        public interface IMyAppService : IApplicationService
        {
            Task<T> GetsExpressionReturnsGenericAsync<T>(Expression<Func<T, bool>> predicate)
                where T : class, new();
        }

        public class MyAppService : IMyAppService
        {
            public Task<T> GetsExpressionReturnsGenericAsync<T>(Expression<Func<T, bool>> predicate) where T : class, new()
            {
                return Task.FromResult(new T());
            }
        }

        public class MyEmptyDto
        {
            
        }

        #endregion
    }
}
