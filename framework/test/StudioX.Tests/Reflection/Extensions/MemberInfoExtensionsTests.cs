using System;
using StudioX.MultiTenancy;
using StudioX.Reflection.Extensions;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Reflection.Extensions
{
    public class MemberInfoExtensionsTests
    {
        [Theory]
        [InlineData(typeof(MyClass))]
        [InlineData(typeof(MyBaseClass))]
        public void GetSingleAttributeOfTypeOrBaseTypesOrNullTest(Type type)
        {
            var attr = type.GetSingleAttributeOfTypeOrBaseTypesOrNull<MultiTenancySideAttribute>();
            attr.ShouldNotBeNull();
            attr.Side.ShouldBe(MultiTenancySides.Host);
        }

        private class MyClass : MyBaseClass
        {
            
        }

        [MultiTenancySide(MultiTenancySides.Host)]
        private abstract class MyBaseClass
        {

        }
    }
}
