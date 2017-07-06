using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Shouldly;
using StudioX.Dapper.Repositories;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.EntityFrameworkCore.Dapper.Tests.Domain;
using Xunit;

namespace StudioX.EntityFrameworkCore.Dapper.Tests.Tests
{
    public class RepositoryTests : StudioXEfCoreDapperTestApplicationBase
    {
        private readonly IDapperRepository<Blog> blogDapperRepository;
        private readonly IRepository<Blog> blogRepository;
        private readonly IDapperRepository<Post, Guid> postDapperRepository;
        private readonly IRepository<Post, Guid> postRepository;
        private readonly IRepository<Comment, long> commentRepository;
        private readonly IDapperRepository<Comment, long> commentDapperRepository;
        private readonly IUnitOfWorkManager uowManager;

        public RepositoryTests()
        {
            uowManager = Resolve<IUnitOfWorkManager>();
            blogRepository = Resolve<IRepository<Blog>>();
            postRepository = Resolve<IRepository<Post, Guid>>();
            blogDapperRepository = Resolve<IDapperRepository<Blog>>();
            postDapperRepository = Resolve<IDapperRepository<Post, Guid>>();
            commentRepository = Resolve<IRepository<Comment, long>>();
            commentDapperRepository = Resolve<IDapperRepository<Comment, long>>();
        }

        [Fact]
        public void ShouldGetInitialBlogs()
        {
            //Act
            var blogs = blogRepository.GetAllList();
            var blogsFromDapper = blogDapperRepository.GetAll();

            //Assert
            blogs.Count.ShouldBeGreaterThan(0);
            blogsFromDapper.Count().ShouldBeGreaterThan(0);
        }

        [Fact]
        public async Task ShouldAutomaticallySaveChangesOnUow()
        {
            int blog1Id;
            int blog2Id;

            //Act

            using (var uow = uowManager.Begin())
            {
                var blog1 = await blogRepository.SingleAsync(b => b.Name == "test-blog-1");
                blog1Id = blog1.Id;

                blog1.Name = "test-blog-1-updated";

                await blogDapperRepository.InsertAsync(new Blog("test-blog-2", "www"));

                var blog2 = blogRepository.Single(x => x.Name == "test-blog-2");
                blog2Id = blog2.Id;

                blog2.Name = "test-blog-2-updated";

                blogDapperRepository.Update(blog2);

                await uow.CompleteAsync();
            }

            //Assert

            await UsingDbContextAsync(async context =>
            {
                var blog1 = await context.Blogs.SingleAsync(b => b.Id == blog1Id);
                blog1.Name.ShouldBe("test-blog-1-updated");

                var blog2 = await context.Blogs.SingleAsync(b => b.Id == blog2Id);
                blog2.Name.ShouldBe("test-blog-2-updated");
            });
        }

        [Fact]
        public async Task ShouldAutomaticallySaveChangesOnUoWCompletedWithDapper()
        {
            int blog1Id;

            //Act
            using (var uow = uowManager.Begin())
            {
                var blog1 = await blogDapperRepository.SingleAsync(b => b.Name == "test-blog-1");
                blog1Id = blog1.Id;

                blog1.Name = "test-blog-1-updated";
                blogDapperRepository.Update(blog1);

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
        public async Task ShouldInsertNewEntitywithdapper()
        {
            using (var uow = uowManager.Begin())
            {
                var blog = new Blog("blog2", "http://myblog2.com");
                blog.IsTransient().ShouldBeTrue();
                await blogDapperRepository.InsertAsync(blog);
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

        [Fact]
        public async Task ShouldInsertNewEntityWithGuidIdwithdapper()
        {
            using (var uow = uowManager.Begin())
            {
                var blog1 = await blogRepository.GetAsync(1);
                var post = new Post(blog1.Id, "a test title", "a test body");
                post.IsTransient().ShouldBeTrue();
                await postDapperRepository.InsertAsync(post);
                await uow.CompleteAsync();
                post.IsTransient().ShouldBeFalse();
            }
        }

        [Fact]
        public void DapperandEfCoreshouldworkundersameunitofwork()
        {
            using (var uow = Resolve<IUnitOfWorkManager>().Begin())
            {
                var blogId = blogDapperRepository.InsertAndGetId(new Blog("LongSameUow", "www"));

                var blog = blogRepository.Get(blogId);

                blog.ShouldNotBeNull();

                uow.Complete();
            }
        }

        [Fact]
        public async Task ExecuteMethodForVoidSqlShouldWork()
        {
            var blogId = blogDapperRepository.InsertAndGetId(new Blog("LongBlog", "wwww.studiox.com"));

            await blogDapperRepository.ExecuteAsync("Update Blogs Set Name = @name where Id =@id",
                new {id = blogId, name = "LongNewBlog"});

            blogDapperRepository.Get(blogId).Name.ShouldBe("LongNewBlog");
            blogRepository.Get(blogId).Name.ShouldBe("LongNewBlog");
        }

        [Fact]
        public void QueryingWithTEntityTPrimaryKeyShouldWorkOnDapperRepositories()
        {
            commentRepository.Insert(new Comment("hey!"));

            var comments = commentDapperRepository.Query("select * from Comments").ToList();
            var comments2 = commentDapperRepository.Query<Comment>("select * from Comments").ToList();

            comments2.Count.ShouldBe(1);
            comments.Count.ShouldBe(1);
        }
    }
}