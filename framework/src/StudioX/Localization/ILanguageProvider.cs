using System.Collections.Generic;

namespace StudioX.Localization
{
    public interface ILanguageProvider
    {
        IReadOnlyList<LanguageInfo> GetLanguages();
    }
}