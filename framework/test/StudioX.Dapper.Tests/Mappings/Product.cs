using StudioX.Dapper.Tests.Entities;

using DapperExtensions.Mapper;

namespace StudioX.Dapper.Tests.Mappings
{
    public sealed class ProductMap : ClassMapper<Product>
    {
        public ProductMap()
        {
            Table("Products");
            Map(x => x.Id).Key(KeyType.Identity);
            AutoMap();
        }
    }
}
