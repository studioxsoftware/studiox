using System.ComponentModel;

namespace StudioX.Localization
{
    public class StudioXDisplayNameAttribute : DisplayNameAttribute
    {
        public override string DisplayName => LocalizationHelper.GetString(SourceName, Key);

        public string SourceName { get; set; }
        public string Key { get; set; }

        public StudioXDisplayNameAttribute(string sourceName, string key)
        {
            SourceName = sourceName;
            Key = key;
        }
    }
}