using System.Linq;
using System.Reflection;
using StudioX.Application.Features;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.Dependency;
using StudioX.Localization;
using StudioX.Localization.Dictionaries;
using StudioX.Localization.Dictionaries.Xml;
using StudioX.Modules;
using StudioX.MultiTenancy;
using StudioX.Reflection;
using StudioX.Zero.Configuration;
using StudioX.Configuration.Startup;
using StudioX.Reflection.Extensions;
using Castle.MicroKernel.Registration;

namespace StudioX.Zero
{
    /// <summary>
    /// StudioX zero core module.
    /// </summary>
    [DependsOn(typeof(StudioXKernelModule))]
    public class StudioXZeroCommonModule : StudioXModule
    {
        public override void PreInitialize()
        {
            IocManager.RegisterIfNot<IStudioXZeroEntityTypes, StudioXZeroEntityTypes>(); //Registered on services.AddStudioXIdentity() for StudioX.ZeroCore.

            IocManager.Register<IRoleManagementConfig, RoleManagementConfig>();
            IocManager.Register<IUserManagementConfig, UserManagementConfig>();
            IocManager.Register<ILanguageManagementConfig, LanguageManagementConfig>();
            IocManager.Register<IStudioXZeroConfig, StudioXZeroConfig>();

            Configuration.ReplaceService<ITenantStore, TenantStore>(DependencyLifeStyle.Transient);

            Configuration.Settings.Providers.Add<StudioXZeroSettingProvider>();

            Configuration.Localization.Sources.Add(
                new DictionaryBasedLocalizationSource(
                    StudioXZeroConsts.LocalizationSourceName,
                    new XmlEmbeddedFileLocalizationDictionaryProvider(
                        typeof(StudioXZeroCommonModule).GetAssembly(), "StudioX.Zero.Localization.Source"
                        )));

            IocManager.IocContainer.Kernel.ComponentRegistered += Kernel_ComponentRegistered;
        }

        public override void Initialize()
        {
            FillMissingEntityTypes();

            IocManager.Register<IMultiTenantLocalizationDictionary, MultiTenantLocalizationDictionary>(DependencyLifeStyle.Transient);
            IocManager.RegisterAssemblyByConvention(typeof(StudioXZeroCommonModule).GetAssembly());

            RegisterTenantCache();
        }

        private void Kernel_ComponentRegistered(string key, Castle.MicroKernel.IHandler handler)
        {
            if (typeof(IStudioXZeroFeatureValueStore).IsAssignableFrom(handler.ComponentModel.Implementation) && !IocManager.IsRegistered<IStudioXZeroFeatureValueStore>())
            {
                IocManager.IocContainer.Register(
                    Component.For<IStudioXZeroFeatureValueStore>().ImplementedBy(handler.ComponentModel.Implementation).Named("StudioXZeroFeatureValueStore").LifestyleTransient()
                    );
            }
        }

        private void FillMissingEntityTypes()
        {
            using (var entityTypes = IocManager.ResolveAsDisposable<IStudioXZeroEntityTypes>())
            {
                if (entityTypes.Object.User != null &&
                    entityTypes.Object.Role != null &&
                    entityTypes.Object.Tenant != null)
                {
                    return;
                }

                using (var typeFinder = IocManager.ResolveAsDisposable<ITypeFinder>())
                {
                    var types = typeFinder.Object.FindAll();
                    entityTypes.Object.Tenant = types.FirstOrDefault(t => typeof(StudioXTenantBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
                    entityTypes.Object.Role = types.FirstOrDefault(t => typeof(StudioXRoleBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
                    entityTypes.Object.User = types.FirstOrDefault(t => typeof(StudioXUserBase).IsAssignableFrom(t) && !t.GetTypeInfo().IsAbstract);
                }
            }
        }

        private void RegisterTenantCache()
        {
            if (IocManager.IsRegistered<ITenantCache>())
            {
                return;
            }

            using (var entityTypes = IocManager.ResolveAsDisposable<IStudioXZeroEntityTypes>())
            {
                var implType = typeof (TenantCache<,>)
                    .MakeGenericType(entityTypes.Object.Tenant, entityTypes.Object.User);

                IocManager.Register(typeof (ITenantCache), implType, DependencyLifeStyle.Transient);
            }
        }
    }
}
