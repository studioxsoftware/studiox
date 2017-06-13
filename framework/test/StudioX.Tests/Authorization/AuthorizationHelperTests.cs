using System.Reflection;
using System.Threading.Tasks;
using StudioX.Application.Features;
using StudioX.Authorization;
using StudioX.Configuration.Startup;
using NSubstitute;
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
                typeof(MyNonAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyNonAuthorizedClass.TestNotAuthorized))
                );

            await authorizeHelper.AuthorizeAsync(
                typeof(MyAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyAuthorizedClass.TestNotAuthorized))
            );
        }

        [Fact]
        public async Task AuthorizedMethodsCanNotBeCalledAnonymously()
        {
            await Assert.ThrowsAsync<StudioXAuthorizationException>(async () =>
            {
                await authorizeHelper.AuthorizeAsync(
                    typeof(MyNonAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyNonAuthorizedClass.TestAuthorized))
                );
            });

            await Assert.ThrowsAsync<StudioXAuthorizationException>(async () =>
            {
                await authorizeHelper.AuthorizeAsync(
                    typeof(MyAuthorizedClass).GetTypeInfo().GetMethod(nameof(MyAuthorizedClass.TestAuthorized))
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
