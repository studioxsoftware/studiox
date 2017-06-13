using System.Collections.Generic;
using StudioX.Threading.Extensions;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Threading
{
    public class LockExtensionsTests
    {
        private readonly List<int> numbers;

        public LockExtensionsTests()
        {
            numbers = new List<int> { 1 };
        }

        [Fact]
        public void TestLocking()
        {
            //Just sample usages:
            numbers.Locking(() => { });
            numbers.Locking(list => { });
            numbers.Locking(() => 42).ShouldBe(42);
            numbers.Locking(list => 42).ShouldBe(42);
        }
    }
}
