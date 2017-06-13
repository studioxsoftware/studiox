using System;
using System.Threading.Tasks;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.EntityFrameworkCore.Tests.Domain;
using Shouldly;
using Xunit;

namespace StudioX.EntityFrameworkCore.Tests.Tests
{
    public class ExplicitLoadingTests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> blogRepository;
        private readonly IRepository<Post, Guid> postRepository;

        public ExplicitLoadingTests()
        {
            blogRepository = Resolve<IRepository<Blog>>();
            postRepository = Resolve<IRepository<Post, Guid>>();
        }

        [Fact]
        public async Task ShouldExplicitlyLoadCollections()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var blog = await blogRepository.FirstOrDefaultAsync(b => b.Name == "test-blog-1");
                blog.ShouldNotBeNull();
                blog.Posts.ShouldBeNull(); //Because EF core does not have lazy loading yet!

                await blogRepository.EnsureCollectionLoadedAsync(blog, b => b.Posts);
                blog.Posts.ShouldNotBeNull(); //Now loaded it!
                blog.Posts.Count.ShouldBeGreaterThan(0);

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task ShouldExplicitlyLoadProperties()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var post = await postRepository.FirstOrDefaultAsync(b => b.Title == "test-post-1-title");
                post.ShouldNotBeNull();
                post.Blog.ShouldBeNull(); //Because EF core does not have lazy loading yet!

                await postRepository.EnsurePropertyLoadedAsync(post, p => p.Blog);
                post.Blog.ShouldNotBeNull(); //Now loaded it!
                post.Blog.Name.ShouldBe("test-blog-1");

                await uow.CompleteAsync();
            }
        }
    }
}
