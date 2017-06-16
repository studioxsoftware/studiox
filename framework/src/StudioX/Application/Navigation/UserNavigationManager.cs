using System.Collections.Generic;
using System.Threading.Tasks;
using StudioX.Application.Features;
using StudioX.Authorization;
using StudioX.Collections.Extensions;
using StudioX.Dependency;
using StudioX.Localization;
using StudioX.MultiTenancy;
using StudioX.Runtime.Session;

namespace StudioX.Application.Navigation
{
    internal class UserNavigationManager : IUserNavigationManager, ITransientDependency
    {
        public IPermissionChecker PermissionChecker { get; set; }

        public IStudioXSession StudioXSession { get; set; }

        private readonly INavigationManager navigationManager;
        private readonly ILocalizationContext localizationContext;
        private readonly IIocResolver iocResolver;

        public UserNavigationManager(
            INavigationManager navigationManager,
            ILocalizationContext localizationContext,
            IIocResolver iocResolver)
        {
            this.navigationManager = navigationManager;
            this.localizationContext = localizationContext;
            this.iocResolver = iocResolver;
            PermissionChecker = NullPermissionChecker.Instance;
            StudioXSession = NullStudioXSession.Instance;
        }

        public async Task<UserMenu> GetMenuAsync(string menuName, UserIdentifier user)
        {
            var menuDefinition = navigationManager.Menus.GetOrDefault(menuName);
            if (menuDefinition == null)
            {
                throw new StudioXException("There is no menu with given name: " + menuName);
            }

            var userMenu = new UserMenu(menuDefinition, localizationContext);
            await FillUserMenuItems(user, menuDefinition.Items, userMenu.Items);
            return userMenu;
        }

        public async Task<IReadOnlyList<UserMenu>> GetMenusAsync(UserIdentifier user)
        {
            var userMenus = new List<UserMenu>();

            foreach (var menu in navigationManager.Menus.Values)
            {
                userMenus.Add(await GetMenuAsync(menu.Name, user));
            }

            return userMenus;
        }

        private async Task<int> FillUserMenuItems(UserIdentifier user, IList<MenuItemDefinition> menuItemDefinitions,
            IList<UserMenuItem> userMenuItems)
        {
            //TODO: Can be optimized by re-using FeatureDependencyContext.

            var addedMenuItemCount = 0;

            using (var featureDependencyContext = iocResolver.ResolveAsDisposable<FeatureDependencyContext>())
            {
                featureDependencyContext.Object.TenantId = user == null ? null : user.TenantId;

                foreach (var menuItemDefinition in menuItemDefinitions)
                {
                    if (menuItemDefinition.RequiresAuthentication && user == null)
                    {
                        continue;
                    }

                    if (!string.IsNullOrEmpty(menuItemDefinition.RequiredPermissionName) &&
                        (user == null ||
                         !await PermissionChecker.IsGrantedAsync(user, menuItemDefinition.RequiredPermissionName)))
                    {
                        continue;
                    }

                    if (menuItemDefinition.FeatureDependency != null &&
                        (StudioXSession.MultiTenancySide == MultiTenancySides.Tenant ||
                         user != null && user.TenantId != null) &&
                        !await menuItemDefinition.FeatureDependency.IsSatisfiedAsync(featureDependencyContext.Object))
                    {
                        continue;
                    }

                    var userMenuItem = new UserMenuItem(menuItemDefinition, localizationContext);
                    if (menuItemDefinition.IsLeaf ||
                        await FillUserMenuItems(user, menuItemDefinition.Items, userMenuItem.Items) > 0)
                    {
                        userMenuItems.Add(userMenuItem);
                        ++addedMenuItemCount;
                    }
                }
            }

            return addedMenuItemCount;
        }
    }
}