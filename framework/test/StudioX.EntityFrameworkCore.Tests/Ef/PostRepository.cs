using System;
using StudioX.EntityFrameworkCore.Repositories;
using StudioX.EntityFrameworkCore.Tests.Domain;

namespace StudioX.EntityFrameworkCore.Tests.Ef
{
    public class PostRepository : EfCoreRepositoryBase<BloggingDbContext, Post, Guid>, IPostRepository
    {
        public PostRepository(IDbContextProvider<BloggingDbContext> dbContextProvider)
            : base(dbContextProvider)
        {

        }

        public override int Count()
        {
            throw new Exception("can not get count of posts");
        }
    }
}
