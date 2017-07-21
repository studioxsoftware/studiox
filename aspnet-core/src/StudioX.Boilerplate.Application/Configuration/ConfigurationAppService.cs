using System.Threading.Tasks;
using StudioX.Authorization;
using StudioX.Boilerplate.Configuration.Dto;
using StudioX.Runtime.Session;

namespace StudioX.Boilerplate.Configuration
{
    [StudioXAuthorize]
    public class ConfigurationAppService : BoilerplateAppServiceBase, IConfigurationAppService
    {
        public async Task ChangeUiTheme(ChangeUiThemeInput input)
        {
            await SettingManager.ChangeSettingForUserAsync(
                StudioXSession.ToUserIdentifier(),
                AppSettingNames.UiTheme,
                input.Theme
            );
        }
    }
}