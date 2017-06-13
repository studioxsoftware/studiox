using System.Reflection;
using StudioX.Domain.Uow;
using StudioX.EntityFramework;
using StudioX.Modules;
using StudioX.MultiTenancy;
using Castle.MicroKernel.Registration;

namespace StudioX.Zero.EntityFramework
{
    /// <summary>
    /// Entity framework integration module for ASP.NET Boilerplate Zero.
    /// </summary>
    [DependsOn(typeof(StudioXZeroCoreModule), typeof(StudioXEntityFrameworkModule))]
    public class StudioXZeroEntityFrameworkModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.ReplaceService(typeof(IConnectionStringResolver), () =>
            {
                IocManager.IocContainer.Register(
                    Component.For<IConnectionStringResolver, IDbPerTenantConnectionStringResolver>()
                        .ImplementedBy<DbPerTenantConnectionStringResolver>()
                        .LifestyleTransient()
                    );
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(Assembly.GetExecutingAssembly());
        }
    }
}
