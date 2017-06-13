using System;
using StudioX.Reflection;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Reflection
{
    public class TypeHelperTests
    {
        [Fact]
        public void TestIsFunc()
        {
            TypeHelper.IsFunc(new Func<object>(() => 42)).ShouldBe(true);
            TypeHelper.IsFunc(new Func<int>(() => 42)).ShouldBe(true);
            TypeHelper.IsFunc(new Func<string>(() => "42")).ShouldBe(true);

            TypeHelper.IsFunc("42").ShouldBe(false);
        }

        [Fact]
        public void TestIsFuncOfTReturn()
        {
            TypeHelper.IsFunc<object>(new Func<object>(() => 42)).ShouldBe(true);
            TypeHelper.IsFunc<object>(new Func<int>(() => 42)).ShouldBe(false);
            TypeHelper.IsFunc<string>(new Func<string>(() => "42")).ShouldBe(true);

            TypeHelper.IsFunc("42").ShouldBe(false);
        }

        [Fact]
        public void TestIsPrimitiveExtendedIncludingNullable()
        {
            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(int)).ShouldBe(true);
            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(int?)).ShouldBe(true);

            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(Guid)).ShouldBe(true);
            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(Guid?)).ShouldBe(true);

            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(string)).ShouldBe(true);

            TypeHelper.IsPrimitiveExtendedIncludingNullable(typeof(TypeHelperTests)).ShouldBe(false);
        }
    }
}
