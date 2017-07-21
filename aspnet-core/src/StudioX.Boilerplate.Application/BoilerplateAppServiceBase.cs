using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using StudioX.Application.Services;
using StudioX.Boilerplate.Authorization.Users;
using StudioX.Boilerplate.MultiTenancy;
using StudioX.IdentityFramework;
using StudioX.Runtime.Session;

namespace StudioX.Boilerplate
{
    /// <summary>
    ///     Derive your application services from this class.
    /// </summary>
    public abstract class BoilerplateAppServiceBase : ApplicationService
    {
        public TenantManager TenantManager { get; set; }

        public UserManager UserManager { get; set; }

        protected BoilerplateAppServiceBase()
        {
            LocalizationSourceName = BoilerplateConsts.LocalizationSourceName;
        }

        protected virtual Task<User> GetCurrentUserAsync()
        {
            var user = UserManager.FindByIdAsync(StudioXSession.GetUserId().ToString());
            if (user == null)
            {
                throw new Exception("There is no current user!");
            }

            return user;
        }

        protected virtual Task<Tenant> GetCurrentTenantAsync()
        {
            return TenantManager.GetByIdAsync(StudioXSession.GetTenantId());
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}