using System;
using System.Collections.Generic;
using StudioX.Dependency;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Dependency
{
    public class ScopedIocResolverTests : TestBaseWithLocalIocManager
    {
        [Fact]
        public void UsingScopeTestShouldWork()
        {
            LocalIocManager.Register<SimpleDisposableObject>(DependencyLifeStyle.Transient);

            SimpleDisposableObject simpleObj = null;

            LocalIocManager.UsingScope(scope => { simpleObj = scope.Resolve<SimpleDisposableObject>(); });

            simpleObj.DisposeCount.ShouldBe(1);
        }

        [Fact]
        public void UsingScopeTestWithConstructorShouldWork()
        {
            LocalIocManager.Register<SimpleDisposableObject>(DependencyLifeStyle.Transient);

            SimpleDisposableObject simpleObj = null;

            LocalIocManager.UsingScope(scope => { simpleObj = scope.Resolve<SimpleDisposableObject>(new { myData = 40 }); });

            simpleObj.MyData.ShouldBe(40);
        }

        [Fact]
        public void IIocScopedResolverTestShouldWork()
        {
            LocalIocManager.Register<SimpleDisposableObject>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<SimpleDisposableObject2>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<SimpleDisposableObject3>(DependencyLifeStyle.Transient);

            SimpleDisposableObject simpleObj;
            SimpleDisposableObject2 simpleObj2;
            SimpleDisposableObject3 simpleObj3;

            using (var scope = LocalIocManager.CreateScope())
            {
                simpleObj = scope.Resolve<SimpleDisposableObject>();
                simpleObj2 = scope.Resolve<SimpleDisposableObject2>();
                simpleObj3 = scope.Resolve<SimpleDisposableObject3>();
            }

            simpleObj.DisposeCount.ShouldBe(1);
            simpleObj2.DisposeCount.ShouldBe(1);
            simpleObj3.DisposeCount.ShouldBe(1);
        }

        [Fact]
        public void IIocScopedResolverTestWithConstructorArgsShouldWork()
        {
            LocalIocManager.Register<SimpleDisposableObject>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<SimpleDisposableObject2>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<SimpleDisposableObject3>(DependencyLifeStyle.Transient);

            SimpleDisposableObject simpleObj;
            SimpleDisposableObject2 simpleObj2;
            SimpleDisposableObject3 simpleObj3;

            using (var scope = LocalIocManager.CreateScope())
            {
                simpleObj = scope.Resolve<SimpleDisposableObject>(new { myData = 40 });
                simpleObj2 = scope.Resolve<SimpleDisposableObject2>(new { myData = 4040 });
                simpleObj3 = scope.Resolve<SimpleDisposableObject3>(new { myData = 404040 });
            }

            simpleObj.MyData.ShouldBe(40);
            simpleObj2.MyData.ShouldBe(4040);
            simpleObj3.MyData.ShouldBe(404040);
        }

        [Fact]
        public void IIocScopedResolverTestResolveAllShouldDisposeAllRegistrants()
        {
            LocalIocManager.Register<ISimpleDependency, SimpleDependency>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<ISimpleDependency, SimpleDependency2>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<ISimpleDependency, SimpleDependency3>(DependencyLifeStyle.Transient);

            IEnumerable<ISimpleDependency> simpleDependendcies;

            using (var scope = LocalIocManager.CreateScope())
            {
                simpleDependendcies = scope.ResolveAll<ISimpleDependency>();
            }

            simpleDependendcies.ShouldAllBe(d => d.DisposeCount == 1);
        }

        [Fact]
        public void IIocScopedResolverTestResolveAllShouldWorkWithConstructor()
        {
            LocalIocManager.Register<ISimpleDependency, SimpleDependency>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<ISimpleDependency, SimpleDependency2>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<ISimpleDependency, SimpleDependency3>(DependencyLifeStyle.Transient);

            IEnumerable<ISimpleDependency> simpleDependendcies;

            using (var scope = LocalIocManager.CreateScope())
            {
                simpleDependendcies = scope.ResolveAll<ISimpleDependency>(new { myData = 40 });
            }

            simpleDependendcies.ShouldAllBe(x => x.MyData == 40);
        }

        [Fact]
        public void IIocScopedResolverTestResolveAllShouldWorkWithOtherResolvings()
        {
            LocalIocManager.Register<ISimpleDependency, SimpleDependency>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<ISimpleDependency, SimpleDependency2>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<ISimpleDependency, SimpleDependency3>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<SimpleDisposableObject>(DependencyLifeStyle.Transient);

            IEnumerable<ISimpleDependency> simpleDependendcies;
            SimpleDisposableObject simpleObject;

            using (var scope = LocalIocManager.CreateScope())
            {
                simpleDependendcies = scope.ResolveAll<ISimpleDependency>();
                simpleObject = scope.Resolve<SimpleDisposableObject>();
            }

            simpleDependendcies.ShouldAllBe(x => x.DisposeCount == 1);
            simpleObject.DisposeCount.ShouldBe(1);
        }

        [Fact]
        public void IIocScopedResolverTestResolveAllShouldWorkWithOtherResolvingsConstructorArguments()
        {
            LocalIocManager.Register<ISimpleDependency, SimpleDependency>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<ISimpleDependency, SimpleDependency2>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<ISimpleDependency, SimpleDependency3>(DependencyLifeStyle.Transient);
            LocalIocManager.Register<SimpleDisposableObject>(DependencyLifeStyle.Transient);

            IEnumerable<ISimpleDependency> simpleDependendcies;
            SimpleDisposableObject simpleObject;

            using (var scope = LocalIocManager.CreateScope())
            {
                simpleDependendcies = scope.ResolveAll<ISimpleDependency>(new { myData = 40 });
                simpleObject = scope.Resolve<SimpleDisposableObject>(new { myData = 40 });
            }

            simpleDependendcies.ShouldAllBe(x => x.MyData == 40);
            simpleObject.MyData.ShouldBe(40);
        }

        [Fact]
        public void IIocScopedResolverTestIsRegisteredShouldWork()
        {
            LocalIocManager.Register<ISimpleDependency, SimpleDependency>(DependencyLifeStyle.Transient);

            using (var scope = LocalIocManager.CreateScope())
            {
                scope.IsRegistered<ISimpleDependency>().ShouldBe(true);
                scope.IsRegistered(typeof(ISimpleDependency)).ShouldBe(true);
            }
        }

        [Fact]
        public void IIocScopedResolverTestCustomReleaseShouldWork()
        {
            LocalIocManager.Register<ISimpleDependency, SimpleDependency>(DependencyLifeStyle.Transient);

            ISimpleDependency simpleDependency;

            using (var scope = LocalIocManager.CreateScope())
            {
                simpleDependency = scope.Resolve<ISimpleDependency>();
                scope.Release(simpleDependency);
            }

            simpleDependency.DisposeCount.ShouldBe(1);
        }
    }

    public interface ISimpleDependency : IDisposable
    {
        int MyData { get; set; }
        int DisposeCount { get; set; }
    }

    public class SimpleDependency : ISimpleDependency
    {
        public int MyData { get; set; }

        public int DisposeCount { get; set; }

        public void Dispose()
        {
            DisposeCount++;
        }
    }

    public class SimpleDependency2 : ISimpleDependency
    {
        public int DisposeCount { get; set; }

        public int MyData { get; set; }

        public void Dispose()
        {
            DisposeCount++;
        }
    }

    public class SimpleDependency3 : ISimpleDependency
    {
        public int MyData { get; set; }

        public int DisposeCount { get; set; }

        public void Dispose()
        {
            DisposeCount++;
        }
    }
}
