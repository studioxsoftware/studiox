using Castle.MicroKernel.Registration;
using Castle.Windsor;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency
{
    public class UnicodeNamingTests
    {
        [Fact]
        public void CastleShouldSupportUnicodeClassNames()
        {
            var container = new WindsorContainer();

            container.Register(
                Component.For<Iお知らせAppService>().ImplementedBy<お知らせAppService>().LifestyleTransient()
            );

            container.Resolve<Iお知らせAppService>().ShouldBeOfType<お知らせAppService>();
        }

        //[Fact] This test is failing because Castle Windsor does not support it
        public void CastleShouldRegisterUnicodeNamesInConventions()
        {
            var container = new WindsorContainer();

            container.Register(
                Classes
                    .FromAssemblyContaining(typeof(UnicodeNamingTests))
                    .Where(c => c == typeof(お知らせAppService))
                    .WithServiceDefaultInterfaces()
                    .WithServiceSelf()
                    .LifestyleTransient()
            );

            container.Resolve<お知らせAppService>().ShouldBeOfType<お知らせAppService>();
            container.Resolve<Iお知らせAppService>().ShouldBeOfType<お知らせAppService>();
        }

        public interface Iお知らせAppService
        {
            
        }

        public class お知らせAppService : Iお知らせAppService
        {

        }
    }
}
