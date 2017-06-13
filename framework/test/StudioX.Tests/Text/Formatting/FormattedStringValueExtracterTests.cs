using StudioX.Text;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Text.Formatting
{
    public class FormattedStringValueExtracterTests
    {
        [Fact]
        public void TestMatched()
        {
            TestMatched(
                "My name is Neo.",
                "My name is {0}.",
                new NameValue("0", "Neo")
                );

            TestMatched(
                "User Long does not exist.",
                "User {0} does not exist.",
                new NameValue("0", "Long")
                );
        }

        [Fact]
        public void TestNotMatched()
        {
            TestNotMatched(
                "My name is Neo.",
                "My name is Marry."
                );

            TestNotMatched(
                "Role {0} does not exist.",
                "User name {0} is invalid, can only contain letters or digits."
                );

            TestNotMatched(
                "{0} cannot be null or empty.",
                "Incorrect password."
                );

            TestNotMatched(
                "Incorrect password.",
                "{0} cannot be null or empty."
                );
        }

        [Fact]
        public void IsMatchTest()
        {
            string[] values;
            FormattedStringValueExtracter.IsMatch("User Long does not exist.", "User {0} does not exist.", out values).ShouldBe(true);
            values[0].ShouldBe("Long");
        }

        private static void TestMatched(string str, string format, params NameValue[] expectedPairs)
        {
            var result = new FormattedStringValueExtracter().Extract(str, format);
            result.IsMatch.ShouldBe(true);

            if (expectedPairs == null)
            {
                result.Matches.Count.ShouldBe(0);
                return;
            }

            result.Matches.Count.ShouldBe(expectedPairs.Length);

            for (int i = 0; i < expectedPairs.Length; i++)
            {
                var actualMatch = result.Matches[i];
                var expectedPair = expectedPairs[i];

                actualMatch.Name.ShouldBe(expectedPair.Name);
                actualMatch.Value.ShouldBe(expectedPair.Value);
            }
        }

        private void TestNotMatched(string str, string format)
        {
            var result = new FormattedStringValueExtracter().Extract(str, format);
            result.IsMatch.ShouldBe(false);
        }
    }
}