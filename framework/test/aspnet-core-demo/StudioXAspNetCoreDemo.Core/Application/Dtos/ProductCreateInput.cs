using System.ComponentModel.DataAnnotations;
using StudioX.AutoMapper;
using StudioXAspNetCoreDemo.Core.Domain;

namespace StudioXAspNetCoreDemo.Core.Application.Dtos
{
    [AutoMapTo(typeof(Product))]
    public class ProductCreateInput
    {
        [Required]
        [MaxLength(200)]
        public string Name { get; set; }

        public float? Price { get; set; }
    }
}