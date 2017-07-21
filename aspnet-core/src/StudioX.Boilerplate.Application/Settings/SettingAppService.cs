using System.Collections.Generic;
using System.Threading.Tasks;
using StudioX.Boilerplate.Settings.Dto;
using StudioX.Configuration;
using StudioX.Extensions;
using StudioX.Zero.Configuration;

namespace StudioX.Boilerplate.Settings
{
    public class SettingAppService : BoilerplateAppServiceBase, ISettingAppService
    {
        private readonly ISettingDefinitionManager settingDefinitionManager;

        public SettingAppService(ISettingDefinitionManager settingDefinitionManager)
        {
            this.settingDefinitionManager = settingDefinitionManager;
        }

        public async Task<IReadOnlyList<ISettingValue>> GetAll()
        {
            return StudioXSession.TenantId != null
                ? await SettingManager.GetAllSettingValuesForTenantAsync(StudioXSession.TenantId.Value)
                : await SettingManager.GetAllSettingValuesAsync();
        }

        public SettingDto Get()
        {
            var setting = new SettingDto
            {
                DefaultPasswordComplexity = GetDefaultPasswordComplexitySetting(),
                PasswordComplexity = GetPasswordComplexitySetting(),
                UserLockOut = GetUserLockOutSetting(),
                UserManagement = GetUserManagementSetting()
            };
            return setting;
        }

        public async Task Update(SettingDto input)
        {
            if (StudioXSession.TenantId != null)
            {
                var tenantId = StudioXSession.TenantId.Value;
                await UpdateTenantSetting(tenantId, input);
            }
            else
            {
                await UpdateApplicationSetting(input);
            }
        }

        #region Private methods

        private UserManagementDto GetUserManagementSetting()
        {
            var setting = new UserManagementDto
            {
                IsEmailConfirmationRequiredForLogin = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin).To<bool>(),
                IsNewRegisteredUserActiveByDefault = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.IsNewRegisteredUserActiveByDefault).To<bool>()
            };

            return setting;
        }

        private UserLockOutDto GetUserLockOutSetting()
        {
            var setting = new UserLockOutDto
            {
                IsEnabled = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.UserLockOut.IsEnabled).To<bool>(),
                MaxFailedAccessAttemptsBeforeLockout = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout).To<int>(),
                DefaultAccountLockoutSeconds = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds).To<int>()
            };

            return setting;
        }

        private PasswordComplexityDto GetPasswordComplexitySetting()
        {
            var setting = new PasswordComplexityDto
            {
                RequiredLength = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength).To<int>(),
                RequireDigit = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit).To<bool>(),
                RequireLowercase = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase).To<bool>(),
                RequireUppercase = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase).To<bool>(),
                RequireNonAlphanumeric = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric).To<bool>()
            };
            return setting;
        }

        private DefaultPasswordComplexityDto GetDefaultPasswordComplexitySetting()
        {
            var setting = new DefaultPasswordComplexityDto
            {
                RequiredLength = settingDefinitionManager.GetSettingDefinition(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength).DefaultValue.To<int>(),
                RequireDigit = settingDefinitionManager.GetSettingDefinition(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit).DefaultValue.To<bool>(),
                RequireLowercase = settingDefinitionManager.GetSettingDefinition(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase).DefaultValue.To<bool>(),
                RequireUppercase = settingDefinitionManager.GetSettingDefinition(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase).DefaultValue.To<bool>(),
                RequireNonAlphanumeric = settingDefinitionManager.GetSettingDefinition(
                        StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric)
                    .DefaultValue.To<bool>()
            };
            return setting;
        }

        private async Task UpdateTenantSetting(int tenantId, SettingDto input)
        {
            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                input.PasswordComplexity.RequiredLength.ToString());


            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                input.PasswordComplexity.RequireDigit.ToString().ToLower());

            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                input.PasswordComplexity.RequireLowercase.ToString().ToLower());

            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                input.PasswordComplexity.RequireUppercase.ToString().ToLower());

            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                input.PasswordComplexity.RequireNonAlphanumeric.ToString().ToLower());

            // UserManagementSetting
            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.IsNewRegisteredUserActiveByDefault,
                input.UserManagement.IsNewRegisteredUserActiveByDefault.ToString().ToLower());

            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,
                input.UserManagement.IsEmailConfirmationRequiredForLogin.ToString().ToLower());

            // UserLockOut
            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.UserLockOut.IsEnabled,
                input.UserLockOut.IsEnabled.ToString().ToLower());

            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds,
                input.UserLockOut.DefaultAccountLockoutSeconds.ToString());

            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout,
                input.UserLockOut.MaxFailedAccessAttemptsBeforeLockout.ToString());
        }

        private async Task UpdateApplicationSetting(SettingDto input)
        {
            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                input.PasswordComplexity.RequiredLength.ToString());


            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                input.PasswordComplexity.RequireDigit.ToString().ToLower());

            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                input.PasswordComplexity.RequireLowercase.ToString().ToLower());

            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                input.PasswordComplexity.RequireUppercase.ToString().ToLower());

            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                input.PasswordComplexity.RequireNonAlphanumeric.ToString().ToLower());

            // UserManagementSetting
            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.IsNewRegisteredUserActiveByDefault,
                input.UserManagement.IsNewRegisteredUserActiveByDefault.ToString().ToLower());

            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin,
                input.UserManagement.IsEmailConfirmationRequiredForLogin.ToString());

            // UserLockOut
            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.UserLockOut.IsEnabled,
                input.UserLockOut.IsEnabled.ToString().ToLower());

            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.UserLockOut.DefaultAccountLockoutSeconds,
                input.UserLockOut.DefaultAccountLockoutSeconds.ToString());

            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.UserLockOut.MaxFailedAccessAttemptsBeforeLockout,
                input.UserLockOut.MaxFailedAccessAttemptsBeforeLockout.ToString());
        }

        #endregion
    }
}