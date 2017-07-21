using System.Threading.Tasks;
using StudioX.Boilerplate.Authorization.Accounts.Dto;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Configuration;
using StudioX.Zero.Configuration;

namespace StudioX.Boilerplate.Authorization.Accounts
{
    public class AccountAppService : BoilerplateAppServiceBase, IAccountAppService
    {
        private readonly UserRegistrationManager userRegistrationManager;

        public AccountAppService(
            UserRegistrationManager userRegistrationManager)
        {
            this.userRegistrationManager = userRegistrationManager;
        }

        public async Task<IsTenantAvailableOutput> IsTenantAvailable(IsTenantAvailableInput input)
        {
            var tenant = await TenantManager.FindByTenancyNameAsync(input.TenancyName);
            if (tenant == null)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.NotFound);
            }

            if (!tenant.IsActive)
            {
                return new IsTenantAvailableOutput(TenantAvailabilityState.InActive);
            }

            return new IsTenantAvailableOutput(TenantAvailabilityState.Available, tenant.Id);
        }

        public async Task<RegisterOutput> Register(RegisterInput input)
        {
            var user = await userRegistrationManager.RegisterAsync(
                input.FirstName,
                input.LastName,
                input.EmailAddress,
                input.UserName,
                input.Password,
                false
            );

            var isEmailConfirmationRequiredForLogin =
                await SettingManager.GetSettingValueAsync<bool>(
                    StudioXZeroSettingNames.UserManagement.IsEmailConfirmationRequiredForLogin);

            return new RegisterOutput
            {
                CanLogin = user.IsActive && (user.IsEmailConfirmed || !isEmailConfirmationRequiredForLogin)
            };
        }
    }
}