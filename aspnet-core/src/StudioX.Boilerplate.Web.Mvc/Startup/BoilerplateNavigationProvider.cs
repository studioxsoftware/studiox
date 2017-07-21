using StudioX.Application.Navigation;
using StudioX.Localization;
using StudioX.Boilerplate.Authorization;

namespace StudioX.Boilerplate.Web.Startup
{
    /// <summary>
    /// This class defines menus for the application.
    /// </summary>
    public class BoilerplateNavigationProvider : NavigationProvider
    {
        public override void SetNavigation(INavigationProviderContext context)
        {
            context.Manager.MainMenu
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Home,
                        L("HomePage"),
                        url: "",
                        icon: "home",
                        requiresAuthentication: true
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Tenants,
                        L("Tenants"),
                        url: "Tenants",
                        icon: "business",
                        requiredPermissionName: PermissionNames.System.Administration.Tenants.MainMenu
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.Users,
                        L("Users"),
                        url: "Users",
                        icon: "people",
                        requiredPermissionName: PermissionNames.System.Administration.Users.MainMenu
                    )
                )
                .AddItem(
                    new MenuItemDefinition(
                        PageNames.Roles,
                        L("Roles"),
                        url: "Roles",
                        icon: "local_offer",
                        requiredPermissionName: PermissionNames.System.Administration.Roles.MainMenu
                    )
                ).AddItem(
                    new MenuItemDefinition(
                        PageNames.About,
                        L("About"),
                        url: "About",
                        icon: "info"
                    )
                ).AddItem( //Menu items below is just for demonstration!
                    new MenuItemDefinition(
                        "MultiLevelMenu",
                        L("MultiLevelMenu"),
                        icon: "menu"
                    ).AddItem(
                        new MenuItemDefinition(
                            "StudioXBoilerplate",
                            new FixedLocalizableString("StudioX Boilerplate")
                        ).AddItem(
                            new MenuItemDefinition(
                                "StudioXBoilerplateHome",
                                new FixedLocalizableString("Home"),
                                url: ""
                            )
                        )
                    ).AddItem(
                        new MenuItemDefinition(
                            "StudioXZero",
                            new FixedLocalizableString("StudioX Zero")
                        ).AddItem(
                            new MenuItemDefinition(
                                "StudioXZeroHome",
                                new FixedLocalizableString("Home"),
                                url: ""
                            )
                        )
                    )
                );
        }

        private static ILocalizableString L(string name)
        {
            return new LocalizableString(name, BoilerplateConsts.LocalizationSourceName);
        }
    }
}
