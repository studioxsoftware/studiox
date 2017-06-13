using System;
using System.Reflection;
using System.Threading.Tasks;
using StudioX.Threading;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Threading
{
    public class InternalAsyncHelperTests
    {
        private bool asyncMethod1Worked;
        private bool asyncMethod2Worked;

        [Fact]
        public void IsAsyncShouldWork()
        {
            AsyncHelper.IsAsyncMethod(GetType().GetMethod("MyMethod1Sync", BindingFlags.NonPublic | BindingFlags.Instance)).ShouldBe(false);
            AsyncHelper.IsAsyncMethod(GetType().GetMethod("MyMethod1Async", BindingFlags.NonPublic | BindingFlags.Instance)).ShouldBe(true);
            AsyncHelper.IsAsyncMethod(GetType().GetMethod("MyMethod2Sync", BindingFlags.NonPublic | BindingFlags.Instance)).ShouldBe(false);
            AsyncHelper.IsAsyncMethod(GetType().GetMethod("MyMethod2Async", BindingFlags.NonPublic | BindingFlags.Instance)).ShouldBe(true);
        }

        [Fact]
        public async Task ShouldCallAfterAction()
        {
            asyncMethod1Worked.ShouldBe(false);
            await InternalAsyncHelper.AwaitTaskWithPostActionAndFinally(
                MyMethod1Async(),
                async () =>
                {
                    asyncMethod1Worked.ShouldBe(true);
                    await Task.Delay(10);
                },
                (exception) => { }
                );

            asyncMethod2Worked.ShouldBe(false);
            var returnValue = await InternalAsyncHelper.AwaitTaskWithPostActionAndFinallyAndGetResult(
                MyMethod2Async(),
                async () =>
                {
                    asyncMethod2Worked.ShouldBe(true);
                    await Task.Delay(10);
                },
                (exception) => { }
                );

            returnValue.ShouldBe(42);
        }

        [Fact]
        public async Task ShouldCallFinallyOnSuccess()
        {
            var calledFinally = false;

            await InternalAsyncHelper.AwaitTaskWithFinally(
                MyMethod1Async(),
                exception =>
                {
                    calledFinally = true;
                    exception.ShouldBe(null);
                });

            calledFinally.ShouldBe(true);
        }
        
        [Fact]
        public async Task ShouldCallFinallyOnException()
        {
            var calledFinally = false;

            await Assert.ThrowsAsync<Exception>(async () =>
            {
                await InternalAsyncHelper.AwaitTaskWithFinally(
                    MyMethod1Async(true),
                    exception =>
                    {
                        calledFinally = true;
                        exception.ShouldNotBe(null);
                        exception.Message.ShouldBe("test exception");
                    });
            });

            calledFinally.ShouldBe(true);
        }

        private async Task MyMethod1Async(bool throwEx = false)
        {
            await Task.Delay(10);
            asyncMethod1Worked = true;
            if (throwEx)
            {
                throw new Exception("test exception");
            }
        }

        private async Task<int> MyMethod2Async()
        {
            await Task.Delay(10);
            asyncMethod2Worked = true;
            return 42;
        }

        private void MyMethod1Sync()
        {
            asyncMethod1Worked = true;
        }

        private void MyMethod2Sync()
        {

        }
    }
}
