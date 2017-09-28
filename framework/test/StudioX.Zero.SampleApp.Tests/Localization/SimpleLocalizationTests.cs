using System.Globalization;
using System.Threading;
using StudioX.Localization;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Localization
{
    public class SimpleLocalizationTests : SampleAppTestBase
    {
        [Theory]
        [InlineData("en")]
        [InlineData("en-US")]
        [InlineData("en-GB")]
        public void Test1(string cultureName)
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.GetCultureInfo(cultureName);

            Resolve<ILocalizationManager>()
                .GetString(StudioXZeroConsts.LocalizationSourceName, "Identity.UserNotInRole")
                .ShouldBe("User is not in role.");
        }
    }
}
