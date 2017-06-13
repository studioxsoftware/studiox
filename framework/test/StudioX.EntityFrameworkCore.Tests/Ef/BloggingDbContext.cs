using StudioX.EntityFrameworkCore.Tests.Domain;
using Microsoft.EntityFrameworkCore;

namespace StudioX.EntityFrameworkCore.Tests.Ef
{
    public class BloggingDbContext : StudioXDbContext
    {
        public DbSet<Blog> Blogs { get; set; }

        public DbSet<Post> Posts { get; set; }

        public BloggingDbContext(DbContextOptions<BloggingDbContext> options)
            : base(options)
        {
            
        }
    }
}
