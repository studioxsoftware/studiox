using System.Linq;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency
{
    public class IocManagerTests : TestBaseWithLocalIocManager
    {
        public IocManagerTests()
        {
            LocalIocManager.Register<IEmpty, EmptyImplOne>();
            LocalIocManager.Register<IEmpty, EmptyImplTwo>();
        }

        [Fact]
        public void ShouldGetFirstRegisteredClassIfRegisteredMultipleClassForSameInterface()
        {
            LocalIocManager.Resolve<IEmpty>().GetType().ShouldBe(typeof (EmptyImplOne));
        }

        [Fact]
        public void ResolveAllTest()
        {
            var instances = LocalIocManager.ResolveAll<IEmpty>();
            instances.Length.ShouldBe(2);
            instances.Any(i => i.GetType() == typeof(EmptyImplOne)).ShouldBeTrue();
            instances.Any(i => i.GetType() == typeof(EmptyImplTwo)).ShouldBeTrue();
        }

        public interface IEmpty
        {
            
        }

        public class EmptyImplOne : IEmpty
        {
            
        }

        public class EmptyImplTwo : IEmpty
        {

        }
    }
}