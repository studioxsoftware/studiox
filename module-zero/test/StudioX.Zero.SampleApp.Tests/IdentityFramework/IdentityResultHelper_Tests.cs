using StudioX.IdentityFramework;
using StudioX.Localization;
using Microsoft.AspNet.Identity;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.IdentityFramework
{
    public class IdentityResultHelper_Tests : SampleAppTestBase
    {
        [Fact]
        public void Should_Localize_IdentityFramework_Messages()
        {
            var localizationManager = Resolve<ILocalizationManager>();

            IdentityResultExtensions
                .LocalizeErrors(IdentityResult.Failed("Incorrect password."), localizationManager)
                .ShouldBe("Incorrect password.");

            IdentityResultExtensions
                .LocalizeErrors(IdentityResult.Failed("Passwords must be at least 6 characters."), localizationManager)
                .ShouldBe("Passwords must be at least 6 characters.");
        }
    }
}
