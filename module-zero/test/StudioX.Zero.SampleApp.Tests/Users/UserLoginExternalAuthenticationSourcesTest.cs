using System.Threading.Tasks;
using StudioX.Authorization;
using StudioX.Authorization.Users;
using StudioX.Dependency;
using StudioX.Modules;
using StudioX.Zero.Configuration;
using StudioX.Zero.SampleApp.Authorization;
using StudioX.Zero.SampleApp.MultiTenancy;
using StudioX.Zero.SampleApp.Users;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Users
{
    public class UserLoginExternalAuthenticationSourcesTest : SampleAppTestBase<UserLoginExternalAuthenticationSourcesTest.MyUserLoginTestModule>
    {
        private readonly AppLogInManager logInManager;

        public UserLoginExternalAuthenticationSourcesTest()
        {
            UsingDbContext(UserLoginHelper.CreateTestUsers);
            logInManager = LocalIocManager.Resolve<AppLogInManager>();
        }

        [Fact]
        public async Task ShouldLoginFromFakeAuthenticationSource()
        {
            var loginResult = await logInManager.LoginAsync("fakeuser@mydomain.com", "123qwe", Tenant.DefaultTenantName);
            loginResult.Result.ShouldBe(StudioXLoginResultType.Success);
            loginResult.User.AuthenticationSource.ShouldBe("FakeSource");
        }

        [Fact]
        public async Task ShouldFallbackToDefaultLoginUsers()
        {
            var loginResult = await logInManager.LoginAsync("owner@studioxsoftware.com", "123qwe");
            loginResult.Result.ShouldBe(StudioXLoginResultType.Success);
        }

        [DependsOn(typeof(SampleAppTestModule))]
        public class MyUserLoginTestModule : StudioXModule
        {
            public override void PreInitialize()
            {
                Configuration.Modules.Zero().UserManagement.ExternalAuthenticationSources.Add<FakeExternalAuthenticationSource>();
            }

            public override void Initialize()
            {
                IocManager.Register<FakeExternalAuthenticationSource>(DependencyLifeStyle.Transient);
            }
        }

        public class FakeExternalAuthenticationSource : DefaultExternalAuthenticationSource<Tenant, User>
        {
            public override string Name
            {
                get { return "FakeSource"; }
            }

            public override Task<bool> TryAuthenticateAsync(string userNameOrEmailAddress, string plainPassword, Tenant tenant)
            {
                return Task.FromResult(
                    userNameOrEmailAddress == "fakeuser@mydomain.com" &&
                    plainPassword == "123qwe" &&
                    tenant != null &&
                    tenant.TenancyName == Tenant.DefaultTenantName
                    );
            }

            public override Task<User> CreateUserAsync(string userNameOrEmailAddress, Tenant tenant)
            {
                return Task.FromResult(
                    new User
                    {
                        UserName = userNameOrEmailAddress,
                        FirstName = userNameOrEmailAddress,
                        LastName = userNameOrEmailAddress,
                        EmailAddress = userNameOrEmailAddress,
                        IsEmailConfirmed = true,
                        IsActive = true
                    });
            }
        }
    }
}