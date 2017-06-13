using System.Collections.Generic;

namespace StudioX.Web.Models.StudioXUserConfiguration
{
    public class StudioXUserAuthConfigDto
    {
        public Dictionary<string,string> AllPermissions { get; set; }

        public Dictionary<string, string> GrantedPermissions { get; set; }
        
    }
}