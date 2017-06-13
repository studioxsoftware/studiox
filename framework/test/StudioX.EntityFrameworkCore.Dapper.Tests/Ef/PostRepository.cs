using System;

using StudioX.EntityFrameworkCore.Dapper.Tests.Domain;
using StudioX.EntityFrameworkCore.Repositories;

namespace StudioX.EntityFrameworkCore.Dapper.Tests.Ef
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
