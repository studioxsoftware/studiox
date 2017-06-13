using System;
using StudioX.Domain.Entities;
using Shouldly;
using Xunit;

namespace StudioX.Tests.Domain.Entities
{
    public class EntityHelperTests
    {
        [Fact]
        public void GetPrimaryKeyTypeTests()
        {
            EntityHelper.GetPrimaryKeyType<Manager>().ShouldBe(typeof(int));
            EntityHelper.GetPrimaryKeyType(typeof(Manager)).ShouldBe(typeof(int));
            EntityHelper.GetPrimaryKeyType(typeof(TestEntityWithGuidPk)).ShouldBe(typeof(Guid));
        }

        private class TestEntityWithGuidPk : Entity<Guid>
        {
            
        }
    }
}
