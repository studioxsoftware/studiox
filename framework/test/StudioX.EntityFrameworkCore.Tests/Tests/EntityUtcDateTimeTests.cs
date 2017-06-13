using System;
using StudioX.Domain.Repositories;
using StudioX.EntityFrameworkCore.Tests.Domain;
using StudioX.Timing;
using Shouldly;
using Xunit;

namespace StudioX.EntityFrameworkCore.Tests.Tests
{
    public class EntityUtcDateTimeTests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> blogRepository;

        public EntityUtcDateTimeTests()
        {
            blogRepository = Resolve<IRepository<Blog>>();
        }

        [Fact]
        public void DateTimesShouldBeUTC()
        {
            Clock.Kind.ShouldBe(DateTimeKind.Utc);

            //Act

            var blogs = blogRepository.GetAllList();

            //Assert

            blogs.Count.ShouldBeGreaterThan(0);

            foreach (var blog in blogs)
            {
                blog.CreationTime.Kind.ShouldBe(DateTimeKind.Utc);
            }
        }
    }
}
