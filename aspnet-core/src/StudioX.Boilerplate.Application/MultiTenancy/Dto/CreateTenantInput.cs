using System.ComponentModel.DataAnnotations;
using StudioX.Authorization.Users;
using StudioX.AutoMapper;
using StudioX.MultiTenancy;

namespace StudioX.Boilerplate.MultiTenancy.Dto
{
    [AutoMapTo(typeof(Tenant))]
    public class CreateTenantInput
    {
        [Required]
        [StringLength(StudioXTenantBase.MaxTenancyNameLength)]
        [RegularExpression(Tenant.TenancyNameRegex)]
        public string TenancyName { get; set; }

        [Required]
        [StringLength(Tenant.MaxNameLength)]
        public string Name { get; set; }

        [Required]
        [StringLength(StudioXUserBase.MaxEmailAddressLength)]
        public string AdminEmailAddress { get; set; }

        [MaxLength(StudioXTenantBase.MaxConnectionStringLength)]
        public string ConnectionString { get; set; }
    }
}