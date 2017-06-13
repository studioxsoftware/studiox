using Castle.MicroKernel.Registration;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency
{
    public class GenericInjectionTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void ShouldResolveGenericTypes()
        {
            LocalIocManager.IocContainer.Register(
                Component.For<MyClass>(),
                Component.For(typeof (IEmpty<>)).ImplementedBy(typeof (EmptyImplOne<>))
                );

            var genericObj = LocalIocManager.Resolve<IEmpty<MyClass>>();
            genericObj.GenericArg.GetType().ShouldBe(typeof(MyClass));
        }

        public interface IEmpty<T> where T : class 
        {
            T GenericArg { get; set; }
        }

        public class EmptyImplOne<T> : IEmpty<T> where T : class
        {
            public T GenericArg { get; set; }
        }

        public class MyClass
        {

        }
    }
}
