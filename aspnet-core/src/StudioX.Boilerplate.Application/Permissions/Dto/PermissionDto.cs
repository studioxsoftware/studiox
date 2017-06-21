using StudioX.Authorization;
using StudioX.AutoMapper;
using StudioX.Localization;

namespace StudioX.Boilerplate.Permissions.Dto
{
    [AutoMapFrom(typeof(Permission))]
    public class PermissionDto
    {
        public virtual string Name { get; set; }

        [AutoMapFrom(typeof(ILocalizableString))]
        public virtual string DisplayName { get; set; }
        
        public virtual string ParentName { get; set; }

        [AutoMapFrom(typeof(ILocalizableString))]
        public virtual string Description { get; set; }

        public virtual int Level => Name.Split('.').Length;
    }
}