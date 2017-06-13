using System.Threading.Tasks;
using StudioX.Application.Features;
using StudioX.Application.Navigation;
using StudioX.Authorization;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Localization;
using Castle.MicroKernel.Registration;
using NSubstitute;

namespace StudioX.Tests.Application.Navigation
{
    internal class NavigationTestCase
    {
        public NavigationManager NavigationManager { get; private set; }

        public UserNavigationManager UserNavigationManager { get; private set; }

        private readonly IIocManager iocManager;

        public NavigationTestCase()
            : this(new IocManager())
        {
        }

        public NavigationTestCase(IIocManager iocManager)
        {
            this.iocManager = iocManager;
            Initialize();
        }

        private void Initialize()
        {
            //Navigation providers should be registered
            iocManager.Register<MyNavigationProvider1>();
            iocManager.Register<MyNavigationProvider2>();

            //Preparing navigation configuration
            var configuration = new NavigationConfiguration();
            configuration.Providers.Add<MyNavigationProvider1>();
            configuration.Providers.Add<MyNavigationProvider2>();

            //Initializing navigation manager
            NavigationManager = new NavigationManager(iocManager, configuration);
            NavigationManager.Initialize();

            iocManager.IocContainer.Register(
                Component.For<IFeatureDependencyContext, FeatureDependencyContext>()
                    .UsingFactoryMethod(
                        () => new FeatureDependencyContext(iocManager, Substitute.For<IFeatureChecker>()))
                );

            //Create user navigation manager to test
            UserNavigationManager = new UserNavigationManager(NavigationManager, Substitute.For<ILocalizationContext>(), iocManager)
            {
                PermissionChecker = CreateMockPermissionChecker()
            };
        }

        private static IPermissionChecker CreateMockPermissionChecker()
        {
            var permissionChecker = Substitute.For<IPermissionChecker>();
            permissionChecker.IsGrantedAsync(new UserIdentifier(1, 1), "StudioX.Zero.UserManagement").Returns(Task.FromResult(true));
            permissionChecker.IsGrantedAsync(new UserIdentifier(1, 1), "StudioX.Zero.RoleManagement").Returns(Task.FromResult(false));
            return permissionChecker;
        }

        public class MyNavigationProvider1 : NavigationProvider
        {
            public override void SetNavigation(INavigationProviderContext context)
            {
                context.Manager.MainMenu.AddItem(
                    new MenuItemDefinition(
                        "StudioX.Zero.Administration",
                        new FixedLocalizableString("Administration"),
                        "fa fa-asterisk",
                        requiresAuthentication: true
                        ).AddItem(
                            new MenuItemDefinition(
                                "StudioX.Zero.Administration.User",
                                new FixedLocalizableString("User management"),
                                "fa fa-users",
                                "#/admin/users",
                                requiredPermissionName: "StudioX.Zero.UserManagement",
                                customData: "A simple test data"
                                )
                        ).AddItem(
                            new MenuItemDefinition(
                                "StudioX.Zero.Administration.Role",
                                new FixedLocalizableString("Role management"),
                                "fa fa-star-o",
                                "#/admin/roles",
                                requiredPermissionName: "StudioX.Zero.RoleManagement"
                                )
                        )
                    );
            }
        }

        public class MyNavigationProvider2 : NavigationProvider
        {
            public override void SetNavigation(INavigationProviderContext context)
            {
                var adminMenu = context.Manager.MainMenu.GetItemByName("StudioX.Zero.Administration");
                adminMenu.AddItem(
                    new MenuItemDefinition(
                        "StudioX.Zero.Administration.Setting",
                        new FixedLocalizableString("Setting management"),
                        icon: "fa fa-cog",
                        url: "#/admin/settings",
                        customData: new MyCustomDataClass { Data1 = 42, Data2 = "FortyTwo" }
                        )
                    );
            }
        }

        public class MyCustomDataClass
        {
            public int Data1 { get; set; }

            public string Data2 { get; set; }
        }
    }
}