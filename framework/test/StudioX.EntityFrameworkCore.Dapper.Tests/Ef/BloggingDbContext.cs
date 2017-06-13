using StudioX.EntityFrameworkCore.Dapper.Tests.Domain;

using Microsoft.EntityFrameworkCore;

namespace StudioX.EntityFrameworkCore.Dapper.Tests.Ef
{
    public class BloggingDbContext : StudioXDbContext
    {
        public BloggingDbContext(DbContextOptions<BloggingDbContext> options)
            : base(options)
        {
        }

        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }
    }
}
