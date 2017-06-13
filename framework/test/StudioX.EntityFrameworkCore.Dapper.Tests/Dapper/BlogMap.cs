using StudioX.EntityFrameworkCore.Dapper.Tests.Domain;

using DapperExtensions.Mapper;

namespace StudioX.EntityFrameworkCore.Dapper.Tests.Dapper
{
    public sealed class BlogMap : ClassMapper<Blog>
    {
        public BlogMap()
        {
            Table("Blogs");
            Map(x => x.Id).Key(KeyType.Identity);
            Map(x => x.DomainEvents).Ignore();
            AutoMap();
        }
    }
}
