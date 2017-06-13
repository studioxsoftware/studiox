using StudioX.Dapper.Tests.Entities;

using DapperExtensions.Mapper;

namespace StudioX.Dapper.Tests.Mappings
{
    public sealed class ProductDetailMap : ClassMapper<ProductDetail>
    {
        public ProductDetailMap()
        {
            Table("ProductDetails");
            AutoMap();
        }
    }
}
