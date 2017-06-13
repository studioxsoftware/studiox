using StudioX.Application.Services.Dto;
using StudioX.AutoMapper;
using StudioXAspNetCoreDemo.Core.Domain;

namespace StudioXAspNetCoreDemo.Core.Application.Dtos
{
    [AutoMap(typeof(Product))]
    public class ProductDto : EntityDto
    {
        public string Name { get; set; }

        public float Price { get; set; }
    }
}
