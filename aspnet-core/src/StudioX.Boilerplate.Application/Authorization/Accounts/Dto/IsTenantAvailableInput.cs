using System.ComponentModel.DataAnnotations;
using StudioX.MultiTenancy;

namespace StudioX.Boilerplate.Authorization.Accounts.Dto
{
    public class IsTenantAvailableInput
    {
        [Required]
        [MaxLength(StudioXTenantBase.MaxTenancyNameLength)]
        public string TenancyName { get; set; }
    }
}

