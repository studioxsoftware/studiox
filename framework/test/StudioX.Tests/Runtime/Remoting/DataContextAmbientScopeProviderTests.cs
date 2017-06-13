using System.Threading.Tasks;
using StudioX.Runtime.Remoting;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Runtime.Remoting
{
    public class DataContextAmbientScopeProviderTests
    {
        private const string ContextKey = "StudioX.Tests.TestData";

        [Fact]
        public void TestSync()
        {
            var scopeAccessor = new DataContextAmbientScopeProvider<TestData>(
#if NET46
                new CallContextAmbientDataContext()
#else
                new AsyncLocalAmbientDataContext()
#endif
                );

            scopeAccessor.GetValue(ContextKey).ShouldBeNull();

            using (scopeAccessor.BeginScope(ContextKey, new TestData(42)))
            {
                scopeAccessor.GetValue(ContextKey).Number.ShouldBe(42);

                using (scopeAccessor.BeginScope(ContextKey, new TestData(24)))
                {
                    scopeAccessor.GetValue(ContextKey).Number.ShouldBe(24);
                }

                scopeAccessor.GetValue(ContextKey).Number.ShouldBe(42);
            }

            scopeAccessor.GetValue(ContextKey).ShouldBeNull();
        }

        [Fact]
        public async Task TestAsync()
        {
            var scopeAccessor = new DataContextAmbientScopeProvider<TestData>(
#if NET46
                new CallContextAmbientDataContext()
#else
                new AsyncLocalAmbientDataContext()
#endif
                );

            scopeAccessor.GetValue(ContextKey).ShouldBeNull();

            await Task.Delay(1);

            using (scopeAccessor.BeginScope(ContextKey, new TestData(42)))
            {
                await Task.Delay(1);

                scopeAccessor.GetValue(ContextKey).Number.ShouldBe(42);

                using (scopeAccessor.BeginScope(ContextKey, new TestData(24)))
                {
                    await Task.Delay(1);

                    scopeAccessor.GetValue(ContextKey).Number.ShouldBe(24);
                }

                await Task.Delay(1);

                scopeAccessor.GetValue(ContextKey).Number.ShouldBe(42);
            }

            await Task.Delay(1);

            scopeAccessor.GetValue(ContextKey).ShouldBeNull();
        }


        public class TestData
        {
            public TestData(int number)
            {
                Number = number;
            }

            public int Number { get; set; }
        }
    }
}
