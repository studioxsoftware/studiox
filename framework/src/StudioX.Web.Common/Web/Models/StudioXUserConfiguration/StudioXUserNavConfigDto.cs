using System.Collections.Generic;
using StudioX.Application.Navigation;

namespace StudioX.Web.Models.StudioXUserConfiguration
{
    public class StudioXUserNavConfigDto
    {
        public Dictionary<string, UserMenu> Menus { get; set; }
    }
}