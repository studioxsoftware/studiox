using System;
using System.Threading.Tasks;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using Xunit;

namespace StudioX.EntityFrameworkCore.Tests.Tests
{
    public class RepositoryTests : EntityFrameworkCoreModuleTestBase
    {
        private readonly IRepository<Blog> blogRepository;
        private readonly IRepository<Post, Guid> postRepository;
        private readonly IUnitOfWorkManager uowManager;

        public RepositoryTests()
        {
            uowManager = Resolve<IUnitOfWorkManager>();
            blogRepository = Resolve<IRepository<Blog>>();
            postRepository = Resolve<IRepository<Post, Guid>>();
        }

        [Fact]
        public void ShouldGetInitialBlogs()
        {
            //Act

            var blogs = blogRepository.GetAllList();

            //Assert

            blogs.Count.ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task ShouldAutomaticallySaveChangesOnUow()
        {
            int blog1Id;

            //Act

            using (var uow = uowManager.Begin())
            {
                var blog1 = await blogRepository.SingleAsync(b => b.Name == "test-blog-1");
                blog1Id = blog1.Id;

                blog1.Name = "test-blog-1-updated";

                await uow.CompleteAsync();
            }

            //Assert

            await UsingDbContextAsync(async context =>
            {
                var blog1 = await context.Blogs.SingleAsync(b => b.Id == blog1Id);
                blog1.Name.ShouldBe("test-blog-1-updated");
            });
        }

        [Fact]
        public async Task ShouldNotIncludeNavigationPropertiesIfNotRequested()
        {
            //EF Core does not support lazy loading yet, so navigation properties will not be loaded if not included

            using (var uow = uowManager.Begin())
            {
                var post = await postRepository.GetAll().FirstAsync();

                post.Blog.ShouldBeNull();

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task ShouldIncludeNavigationPropertiesIfRequested()
        {
            using (var uow = uowManager.Begin())
            {
                var post = await postRepository.GetAllIncluding(p => p.Blog).FirstAsync();

                post.Blog.ShouldNotBeNull();
                post.Blog.Name.ShouldBe("test-blog-1");

                await uow.CompleteAsync();
            }
        }

        [Fact]
        public async Task ShouldInsertNewEntity()
        {
            using (var uow = uowManager.Begin())
            {
                var blog = new Blog("blog2", "http://myblog2.com");
                blog.IsTransient().ShouldBeTrue();
                await blogRepository.InsertAsync(blog);
                await uow.CompleteAsync();
                blog.IsTransient().ShouldBeFalse();
            }
        }

        [Fact]
        public async Task ShouldInsertNewEntityWithGuidId()
        {
            using (var uow = uowManager.Begin())
            {
                var blog1 = await blogRepository.GetAsync(1);
                var post = new Post(blog1, "a test title", "a test body");
                post.IsTransient().ShouldBeTrue();
                await postRepository.InsertAsync(post);
                await uow.CompleteAsync();
                post.IsTransient().ShouldBeFalse();
            }
        }
    }
}