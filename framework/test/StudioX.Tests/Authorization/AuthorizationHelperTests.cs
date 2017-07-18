using System.Reflection;
using System.Threading.Tasks;
using NSubstitute;
using StudioX.Application.Features;
using StudioX.Authorization;
using StudioX.Configuration.Startup;
using Xunit;

namespace StudioX.Tests.Authorization
{
    public class AuthorizationHelperTests
    {
        private readonly AuthorizationHelper authorizeHelper;

        public AuthorizationHelperTests()
        {
            var featureChecker = Substitute.For<IFeatureChecker>();
            featureChecker.GetValueAsync(Arg.Any<string>()).Returns("false");

            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync(Arg.Any<string>()).Returns(false);

            var configuration = Substitute.For<IAuthorizationConfiguration>();
            configuration.IsEnabled.Returns(true);

            authorizeHelper = new AuthorizationHelper(featureChecker, configuration)
            {
                PermissionChecker = permissionChecker
            };
        }

        [Fact]
        public async Task NotAuthorizedMethodsCanBeCalledAnonymously()
        {
            await authorizeHelper.AuthorizeAsync(
                typeof(MyNonAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyNonAuthorizedClass.TestNotAuthorized)),
                typeof(MyNonAuthorizedClass)
            );

            await authorizeHelper.AuthorizeAsync(
                typeof(MyAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyAuthorizedClass.TestNotAuthorized)),
                typeof(MyAuthorizedClass)
            );
        }

        [Fact]
        public async Task AuthorizedMethodsCanNotBeCalledAnonymously()
        {
            await Assert.ThrowsAsync<StudioXAuthorizationException>(async () =>
            {
                await authorizeHelper.AuthorizeAsync(
                    typeof(MyNonAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyNonAuthorizedClass.TestAuthorized)),
                    typeof(MyNonAuthorizedClass)
                );
            });

            await Assert.ThrowsAsync<StudioXAuthorizationException>(async () =>
            {
                await authorizeHelper.AuthorizeAsync(
                    typeof(MyAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyAuthorizedClass.TestAuthorized)),
                    typeof(MyAuthorizedClass)
                );
            });
        }

        public class MyNonAuthorizedClass
        {
            public void TestNotAuthorized()
            {
            }

            [StudioXAuthorize]
            public void TestAuthorized()
            {
            }
        }

        [StudioXAuthorize]
        public class MyAuthorizedClass
        {
            [StudioXAllowAnonymous]
            public void TestNotAuthorized()
            {
            }

            public void TestAuthorized()
            {
            }
        }
    }
}