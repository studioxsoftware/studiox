using System;
using Shouldly;
using StudioX.Domain.Uow;
using Xunit;

namespace StudioX.Tests.Domain.Uow
{
    public class InnerUnitOfWorkCompleteHandleTest
    {
        [Fact]
        public void ShouldNotOverrideExceptionIfExceptionIsThrownByUser()
        {
            Assert.Throws<Exception>(
                new Action(() =>
                {
                    using (var uow = new InnerUnitOfWorkCompleteHandle())
                    {
                        throw new Exception("My inner exception!");
                    }
                })).Message.ShouldBe("My inner exception!");
        }

        [Fact]
        public void ShouldNotThrowExceptionIfCompleteCalled()
        {
            using (var uow = new InnerUnitOfWorkCompleteHandle())
            {
                uow.Complete();
            }
        }

        [Fact]
        public void ShouldThrowExceptionIfCompleteDidNotCalled()
        {
            Assert.Throws<StudioXException>(() =>
            {
                using (var uow = new InnerUnitOfWorkCompleteHandle())
                {
                }
            }).Message.ShouldBe(InnerUnitOfWorkCompleteHandle.DidNotCallCompleteMethodExceptionMessage);
        }
    }
}