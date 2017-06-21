using System.Collections.Generic;
using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Configuration;
using StudioX.Boilerplate.Settings.Dto;

namespace StudioX.Boilerplate.Settings
{
    public interface ISettingAppService : IApplicationService
    {
        Task<IReadOnlyList<ISettingValue>> GetAll();

        SettingDto Get();

        Task Update(SettingDto input);
    }
}