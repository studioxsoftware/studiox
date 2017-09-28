using System;
using System.Threading.Tasks;
using StudioX.Authorization;
using StudioX.Configuration;
using StudioX.Dependency;
using StudioX.Modules;
using StudioX.Zero.Ldap;
using StudioX.Zero.Ldap.Authentication;
using StudioX.Zero.Ldap.Configuration;
using StudioX.Zero.SampleApp.Authorization;
using StudioX.Zero.SampleApp.MultiTenancy;
using StudioX.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Ldap
{
    public class LdapAuthenticationSourceTests : SampleAppTestBase<LdapAuthenticationSourceTests.MyUserLoginTestModule>
    {
        private readonly AppLogInManager logInManager;

        public LdapAuthenticationSourceTests()
        {
            logInManager = Resolve<AppLogInManager>();
        }

        //[Fact]
        public async Task ShouldLoginFromLdapWithoutAnyConfiguration()
        {
            var result = await logInManager.LoginAsync("-","-", Tenant.DefaultTenantName);
            result.Result.ShouldBe(StudioXLoginResultType.Success);
        }

        //[Fact]
        public async Task ShouldNotLoginFromLdapIfDisabled()
        {
            var settingManager = Resolve<ISettingManager>();
            var defaultTenant = GetDefaultTenant();

            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.IsEnabled, "false");

            var result = await logInManager.LoginAsync("-", "-", Tenant.DefaultTenantName);
            result.Result.ShouldBe(StudioXLoginResultType.InvalidUserNameOrEmailAddress);
        }

        //[Fact]
        public async Task ShouldLoginFromLdapWithProperlyConfigured()
        {
            var settingManager = Resolve<ISettingManager>();
            var defaultTenant = GetDefaultTenant();

            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.Domain, "-");
            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.UserName, "-");
            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.Password, "-");

            var result = await logInManager.LoginAsync("-", "-", Tenant.DefaultTenantName);
            result.Result.ShouldBe(StudioXLoginResultType.Success);
        }

        //[Fact]
        public async Task ShouldNotLoginFromLdapWithWrongConfiguration()
        {
            var settingManager = Resolve<ISettingManager>();
            var defaultTenant = GetDefaultTenant();

            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.Domain, "InvalidDomain");
            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.UserName, "NoUserName");
            await settingManager.ChangeSettingForTenantAsync(defaultTenant.Id, LdapSettingNames.Password, "123123123123");

            await Assert.ThrowsAnyAsync<Exception>(() => logInManager.LoginAsync("testuser", "testpass", Tenant.DefaultTenantName));
        }

        [DependsOn(typeof(StudioXZeroLdapModule), typeof(SampleAppTestModule))]
        public class MyUserLoginTestModule : StudioXModule
        {
            public override void PreInitialize()
            {
                Configuration.Modules.ZeroLdap().Enable(typeof (MyLdapAuthenticationSource));
            }

            public override void Initialize()
            {
                //This is needed just for this test, not for real apps
                IocManager.Register<MyLdapAuthenticationSource>(DependencyLifeStyle.Transient);
            }
        }

        public class MyLdapAuthenticationSource : LdapAuthenticationSource<Tenant, User>
        {
            public MyLdapAuthenticationSource(ILdapSettings settings, IStudioXZeroLdapModuleConfig ldapModuleConfig)
                : base(settings, ldapModuleConfig)
            {

            }
        }
    }
}
