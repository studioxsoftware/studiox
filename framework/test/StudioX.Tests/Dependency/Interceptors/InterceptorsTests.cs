using Castle.DynamicProxy;
using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency.Interceptors
{
    public class InterceptorsTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void InterceptorsShouldWork()
        {
            LocalIocManager.IocContainer.Register(
                Component.For<BracketInterceptor>().LifestyleTransient(),
                Component.For<MyGreetingClass>().Interceptors<BracketInterceptor>().LifestyleTransient()
                );

            var greetingObj = LocalIocManager.Resolve<MyGreetingClass>();

            greetingObj.SayHello("Long").ShouldBe("(Hello Long)");
        }

        public class MyGreetingClass
        {
            public virtual string SayHello(string name)
            {
                return "Hello " + name;
            }
        }

        public class BracketInterceptor : IInterceptor
        {
            public void Intercept(IInvocation invocation)
            {
                invocation.Proceed();
                invocation.ReturnValue = "(" + invocation.ReturnValue + ")";
            }
        }
    }
}
