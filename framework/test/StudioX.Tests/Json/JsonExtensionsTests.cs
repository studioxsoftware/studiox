using StudioX.Json;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Json
{
    public class JsonExtensionsTests
    {
        [Fact]
        public void ToJsonStringTest()
        {
            42.ToJsonString().ShouldBe("42");
        }
    }
}
