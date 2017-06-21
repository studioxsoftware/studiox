using System.Collections.Generic;
using System.Threading.Tasks;
using StudioX.Configuration;
using StudioX.Extensions;
using StudioX.Boilerplate.Settings.Dto;
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
                //IsNewRegisteredUserActiveByDefault = SettingManager.GetSettingValue(
                //    StudioXZeroSettingNames.UserManagement.IsNewRegisteredUserActiveByDefault).To<bool>()
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
                MinLength = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength).To<int>(),
                UseNumbers = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit).To<bool>(),
                UseLowerCaseLetters = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase).To<bool>(),
                UseUpperCaseLetters = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase).To<bool>(),
                UsePunctuations = SettingManager.GetSettingValue(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric).To<bool>()
            };
            return setting;
        }

        private DefaultPasswordComplexityDto GetDefaultPasswordComplexitySetting()
        {
            var setting = new DefaultPasswordComplexityDto
            {
                MinLength = settingDefinitionManager.GetSettingDefinition(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength).DefaultValue.To<int>(),
                UseNumbers = settingDefinitionManager.GetSettingDefinition(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit).DefaultValue.To<bool>(),
                UseLowerCaseLetters = settingDefinitionManager.GetSettingDefinition(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase).DefaultValue.To<bool>(),
                UseUpperCaseLetters = settingDefinitionManager.GetSettingDefinition(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase).DefaultValue.To<bool>(),
                UsePunctuations = settingDefinitionManager.GetSettingDefinition(
                    StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric).DefaultValue.To<bool>()
            };
            return setting;
        }

        private async Task UpdateTenantSetting(int tenantId, SettingDto input)
        {
            // PasswordComplexitySetting
            //await SettingManager.ChangeSettingForTenantAsync(tenantId,
            //    StudioXZeroSettingNames.UserManagement.PasswordComplexity.UseDefaultPasswordComplexitySettings,
            //    input.PasswordComplexity.UseDefaultPasswordComplexitySettings.ToString().ToLower());

            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                input.PasswordComplexity.MinLength.ToString());
       

            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                input.PasswordComplexity.UseNumbers.ToString().ToLower());

            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                input.PasswordComplexity.UseLowerCaseLetters.ToString().ToLower());

            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                input.PasswordComplexity.UseUpperCaseLetters.ToString().ToLower());

            await SettingManager.ChangeSettingForTenantAsync(tenantId,
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                input.PasswordComplexity.UsePunctuations.ToString().ToLower());

            // UserManagementSetting
            //await SettingManager.ChangeSettingForTenantAsync(tenantId,
            //    StudioXZeroSettingNames.UserManagement.IsNewRegisteredUserActiveByDefault,
            //    input.UserManagement.IsNewRegisteredUserActiveByDefault.ToString().ToLower());

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
            // PasswordComplexitySetting
            //await SettingManager.ChangeSettingForApplicationAsync(
            //    StudioXZeroSettingNames.UserManagement.PasswordComplexity.UseDefaultPasswordComplexitySettings,
            //    input.PasswordComplexity.UseDefaultPasswordComplexitySettings.ToString().ToLower());

            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequiredLength,
                input.PasswordComplexity.MinLength.ToString());


            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireDigit,
                input.PasswordComplexity.UseNumbers.ToString().ToLower());

            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireLowercase,
                input.PasswordComplexity.UseLowerCaseLetters.ToString().ToLower());

            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireUppercase,
                input.PasswordComplexity.UseUpperCaseLetters.ToString().ToLower());

            await SettingManager.ChangeSettingForApplicationAsync(
                StudioXZeroSettingNames.UserManagement.PasswordComplexity.RequireNonAlphanumeric,
                input.PasswordComplexity.UsePunctuations.ToString().ToLower());

            // UserManagementSetting
            //await SettingManager.ChangeSettingForApplicationAsync(
            //    StudioXZeroSettingNames.UserManagement.IsNewRegisteredUserActiveByDefault,
            //    input.UserManagement.IsNewRegisteredUserActiveByDefault.ToString().ToLower());

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