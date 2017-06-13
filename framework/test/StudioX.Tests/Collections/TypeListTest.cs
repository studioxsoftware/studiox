using System;
using StudioX.Collections;
using Xunit;

namespace StudioX.Tests.Collections
{
    public class TypeListTest
    {
        [Fact]
        public void ShouldOnlyAddTrueTypes()
        {
            var list = new TypeList<IMyInterface>();
            list.Add<MyClass1>();
            list.Add(typeof(MyClass2));
            Assert.Throws<ArgumentException>(() => list.Add(typeof(MyClass3)));
        }

        public interface IMyInterface
        {
             
        }

        public class MyClass1 : IMyInterface
        {
            
        }

        public class MyClass2 : IMyInterface
        {

        }

        public class MyClass3
        {

        }
    }
}
