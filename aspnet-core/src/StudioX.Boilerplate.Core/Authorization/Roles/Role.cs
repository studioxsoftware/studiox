using System.ComponentModel.DataAnnotations;
using StudioX.Authorization.Roles;
using StudioX.Boilerplate.Authorization.Users;

namespace StudioX.Boilerplate.Authorization.Roles
{
    public class Role : StudioXRole<User>
    {
        //Can add application specific role properties here

        public const int MaxDescriptionLength = 2000;

        [MaxLength(MaxDescriptionLength)]
        public string Description { get; set; }

        public bool IsActive { get; set; }

        public Role()
        {
        }

        public Role(int? tenantId, string displayName)
            : base(tenantId, displayName)
        {
        }

        public Role(int? tenantId, string name, string displayName)
            : base(tenantId, name, displayName)
        {
        }
    }
}