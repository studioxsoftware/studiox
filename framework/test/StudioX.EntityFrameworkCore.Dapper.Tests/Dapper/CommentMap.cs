using DapperExtensions.Mapper;
using StudioX.EntityFrameworkCore.Dapper.Tests.Domain;

namespace StudioX.EntityFrameworkCore.Dapper.Tests.Dapper
{
    public sealed class CommentMap : ClassMapper<Comment>
    {
        public CommentMap()
        {
            Table("Comments");
            Map(x => x.Id).Key(KeyType.Identity);
            AutoMap();
        }
    }
}