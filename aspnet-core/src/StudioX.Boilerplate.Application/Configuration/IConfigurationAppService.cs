using System.Threading.Tasks;
using StudioX.Boilerplate.Configuration.Dto;

namespace StudioX.Boilerplate.Configuration
{
    public interface IConfigurationAppService
    {
        Task ChangeUiTheme(ChangeUiThemeInput input);
    }
}