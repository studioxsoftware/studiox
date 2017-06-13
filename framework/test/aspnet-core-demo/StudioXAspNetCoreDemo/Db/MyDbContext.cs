using StudioX.EntityFrameworkCore;
using StudioXAspNetCoreDemo.Core.Domain;
using Microsoft.EntityFrameworkCore;

namespace StudioXAspNetCoreDemo.Db
{
    public class MyDbContext : StudioXDbContext
    {
        public DbSet<Product> Products { get; set; }

        public MyDbContext(DbContextOptions<MyDbContext> options)
            : base(options)
        {
        }
    }
}