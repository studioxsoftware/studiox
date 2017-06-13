using System.Collections.Generic;
using StudioX.Collections.Extensions;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Collections.Extensions
{
    public class ListExtensionsSortByDependenciesTests
    {
        [Fact]
        public void ShouldSortByDependencies()
        {
            var a = new DependedObject("A");
            var b = new DependedObject("B");
            var c = new DependedObject("C");
            var d = new DependedObject("D");

            b.Dependencies.Add(a);
            c.Dependencies.Add(a);
            c.Dependencies.Add(d);
            d.Dependencies.Add(b);

            ShouldSortedCorrectly(new List<DependedObject> { a, b, c, d });
            ShouldSortedCorrectly(new List<DependedObject> { d, c, b, a });
            ShouldSortedCorrectly(new List<DependedObject> { a, c, d, b });
            ShouldSortedCorrectly(new List<DependedObject> { c, a, d, b });
        }

        private static void ShouldSortedCorrectly(List<DependedObject> dependedObjects)
        {
            var sorted = dependedObjects.SortByDependencies(o => o.Dependencies);
            sorted[0].Name.ShouldBe("A");
            sorted[1].Name.ShouldBe("B");
            sorted[2].Name.ShouldBe("D");
            sorted[3].Name.ShouldBe("C");
        }

        private class DependedObject
        {
            public string Name { get; private set; }

            public List<DependedObject> Dependencies { get; private set; }

            public DependedObject(string name)
            {
                Name = name;
                Dependencies = new List<DependedObject>();
            }
        }
    }
}
