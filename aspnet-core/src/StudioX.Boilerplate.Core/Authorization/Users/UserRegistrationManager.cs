using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Authorization.Users;
using StudioX.Domain.Services;
using StudioX.IdentityFramework;
using StudioX.Runtime.Session;
using StudioX.UI;
using StudioX.Boilerplate.Authorization.Roles;
using StudioX.Boilerplate.MultiTenancy;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace StudioX.Boilerplate.Authorization.Users
{
    public class UserRegistrationManager : DomainService
    {
        public IStudioXSession StudioXSession { get; set; }

        private readonly TenantManager tenantManager;
        private readonly UserManager userManager;
        private readonly RoleManager roleManager;
        private readonly IPasswordHasher<User> passwordHasher;

        public UserRegistrationManager(
            TenantManager tenantManager,
            UserManager userManager,
            RoleManager roleManager,
            IPasswordHasher<User> passwordHasher)
        {
            this.tenantManager = tenantManager;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.passwordHasher = passwordHasher;

            StudioXSession = NullStudioXSession.Instance;
        }

        public async Task<User> RegisterAsync(string firstName, string lastName, 
            string emailAddress, string userName, string plainPassword, bool isEmailConfirmed)
        {
            CheckForTenant();

            var tenant = await GetActiveTenantAsync();

            var user = new User
            {
                TenantId = tenant.Id,
                FirstName = firstName,
                LastName = lastName,
                EmailAddress = emailAddress,
                IsActive = true,
                UserName = userName,
                IsEmailConfirmed = true,
                Roles = new List<UserRole>()
            };

            user.SetNormalizedNames();

            user.Password = passwordHasher.HashPassword(user, plainPassword);

            foreach (var defaultRole in await roleManager.Roles.Where(r => r.IsDefault).ToListAsync())
            {
                user.Roles.Add(new UserRole(tenant.Id, user.Id, defaultRole.Id));
            }

            CheckErrors(await userManager.CreateAsync(user));
            await CurrentUnitOfWork.SaveChangesAsync();

            return user;
        }

        private void CheckForTenant()
        {
            if (!StudioXSession.TenantId.HasValue)
            {
                throw new InvalidOperationException("Can not register host users!");
            }
        }

        private async Task<Tenant> GetActiveTenantAsync()
        {
            if (!StudioXSession.TenantId.HasValue)
            {
                return null;
            }

            return await GetActiveTenantAsync(StudioXSession.TenantId.Value);
        }

        private async Task<Tenant> GetActiveTenantAsync(int tenantId)
        {
            var tenant = await tenantManager.FindByIdAsync(tenantId);
            if (tenant == null)
            {
                throw new UserFriendlyException(L("UnknownTenantId{0}", tenantId));
            }

            if (!tenant.IsActive)
            {
                throw new UserFriendlyException(L("TenantIdIsNotActive{0}", tenantId));
            }

            return tenant;
        }

        protected virtual void CheckErrors(IdentityResult identityResult)
        {
            identityResult.CheckErrors(LocalizationManager);
        }
    }
}
