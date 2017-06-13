using System.Collections.Generic;
using StudioX.Localization;
using Microsoft.AspNetCore.Http;

namespace StudioX.Boilerplate.Web.Views.Shared.Components.AccountLanguages
{
    public class LanguageSelectionViewModel
    {
        public LanguageInfo CurrentLanguage { get; set; }

        public IReadOnlyList<LanguageInfo> Languages { get; set; }

        public PathString CurrentUrl { get; set; }
    }
}