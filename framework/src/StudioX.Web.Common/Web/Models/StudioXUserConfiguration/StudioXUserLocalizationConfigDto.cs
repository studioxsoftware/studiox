using System.Collections.Generic;
using StudioX.Localization;

namespace StudioX.Web.Models.StudioXUserConfiguration
{
    public class StudioXUserLocalizationConfigDto
    {
        public StudioXUserCurrentCultureConfigDto CurrentCulture { get; set; }

        public List<LanguageInfo> Languages { get; set; }

        public LanguageInfo CurrentLanguage { get; set; }

        public List<StudioXLocalizationSourceDto> Sources { get; set; }

        public Dictionary<string, Dictionary<string, string>> Values { get; set; }
    }
}