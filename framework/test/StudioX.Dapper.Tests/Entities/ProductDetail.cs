using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

using StudioX.Domain.Entities;
using StudioX.Domain.Entities.Auditing;

namespace StudioX.Dapper.Tests.Entities
{
    [Table("ProductDetails")]
    public class ProductDetail : FullAuditedEntity, IMustHaveTenant
    {
        protected ProductDetail()
        {
        }

        public ProductDetail(string gender) : this()
        {
            Gender = gender;
        }

        [Required]
        public string Gender { get; set; }

        public int TenantId { get; set; }
    }
}
