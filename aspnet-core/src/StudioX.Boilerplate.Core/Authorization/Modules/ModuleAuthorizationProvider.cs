using StudioX.Localization;

namespace StudioX.Boilerplate.Authorization.Modules
{
    public class ModuleAuthorizationProvider
    {
        // TODO: Will be change to localization
        public ILocalizableString L(string name)
        {
            return new FixedLocalizableString(name);
        }
    }
}
