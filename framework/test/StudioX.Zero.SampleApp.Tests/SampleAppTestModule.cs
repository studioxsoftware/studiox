using System;
using StudioX.Modules;
using StudioX.TestBase;
using StudioX.Zero.Ldap;
using StudioX.Zero.SampleApp.EntityFramework;
using Castle.MicroKernel.Registration;
using Microsoft.Owin.Security;
using NSubstitute;

namespace StudioX.Zero.SampleApp.Tests
{
    [DependsOn(
        typeof(SampleAppEntityFrameworkModule),
        typeof(StudioXZeroLdapModule),
        typeof(StudioXTestBaseModule))]
    public class SampleAppTestModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.UnitOfWork.Timeout = TimeSpan.FromMinutes(2);
        }

        public override void Initialize()
        {
            IocManager.IocContainer.Register(
                Component.For<IAuthenticationManager>().Instance(Substitute.For<IAuthenticationManager>())
            );
        }
    }
}