using StudioX.AspNetCore.Mvc.Proxying.Utils;
using Xunit;

namespace StudioX.AspNetCore.Tests.Utils
{
    public class ArrayMactherTests
    {
        [Theory]
        [InlineData(
            new[] { "p1", "p2.a1", "p2.a2.b1", "p2.a2.b2", "p3", "p4", "p5.c1", "p5.c2" },
            new[] { "p1", "p2", "p3", "p4", "p5" },
            new[] { "p1", "p2", "p2", "p2", "p3", "p4", "p5", "p5" })
            ]
        public void ShouldFindCorrectItems(string[] sourceArray, string[] destinationArray, string[] expectedArray)
        {
            var result = ArrayMatcher.Match(sourceArray, destinationArray);
            Assert.Equal(expectedArray, result);
        }
    }
}
