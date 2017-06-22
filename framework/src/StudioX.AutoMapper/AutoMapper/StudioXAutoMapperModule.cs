using System;
using System.Reflection;
using AutoMapper;
using Castle.MicroKernel.Registration;
using StudioX.Configuration.Startup;
using StudioX.Localization;
using StudioX.Modules;
using StudioX.Reflection;
using IObjectMapper = StudioX.ObjectMapping.IObjectMapper;

namespace StudioX.AutoMapper
{
    [DependsOn(typeof(StudioXKernelModule))]
    public class StudioXAutoMapperModule : StudioXModule
    {
        private readonly ITypeFinder typeFinder;

        private static volatile bool createdMappingsBefore;
        private static readonly object SyncObj = new object();

        public StudioXAutoMapperModule(ITypeFinder typeFinder)
        {
            this.typeFinder = typeFinder;
        }

        public override void PreInitialize()
        {
            IocManager.Register<IStudioXAutoMapperConfiguration, StudioXAutoMapperConfiguration>();

            Configuration.ReplaceService<IObjectMapper, AutoMapperObjectMapper>();

            Configuration.Modules.StudioXAutoMapper().Configurators.Add(CreateCoreMappings);
        }

        public override void PostInitialize()
        {
            CreateMappings();
        }

        private void CreateMappings()
        {
            lock (SyncObj)
            {
                Action<IMapperConfigurationExpression> configurer = configuration =>
                {
                    FindAndAutoMapTypes(configuration);
                    foreach (var configurator in Configuration.Modules.StudioXAutoMapper().Configurators)
                    {
                        configurator(configuration);
                    }
                };

                if (Configuration.Modules.StudioXAutoMapper().UseStaticMapper)
                {
                    //We should prevent duplicate mapping in an application, since Mapper is static.
                    if (!createdMappingsBefore)
                    {
                        Mapper.Initialize(configurer);
                        createdMappingsBefore = true;
                    }

                    IocManager.IocContainer.Register(
                        Component.For<IMapper>().Instance(Mapper.Instance).LifestyleSingleton()
                    );
                }
                else
                {
                    var config = new MapperConfiguration(configurer);
                    IocManager.IocContainer.Register(
                        Component.For<IMapper>().Instance(config.CreateMapper()).LifestyleSingleton()
                    );
                }
            }
        }

        private void FindAndAutoMapTypes(IMapperConfigurationExpression configuration)
        {
            var types = typeFinder.Find(type =>
                {
                    var typeInfo = type.GetTypeInfo();
                    return typeInfo.IsDefined(typeof(AutoMapAttribute)) ||
                           typeInfo.IsDefined(typeof(AutoMapFromAttribute)) ||
                           typeInfo.IsDefined(typeof(AutoMapToAttribute));
                }
            );

            Logger.DebugFormat("Found {0} classes define auto mapping attributes", types.Length);

            foreach (var type in types)
            {
                Logger.Debug(type.FullName);
                configuration.CreateAutoAttributeMaps(type);
            }
        }

        private void CreateCoreMappings(IMapperConfigurationExpression configuration)
        {
            var localizationContext = IocManager.Resolve<ILocalizationContext>();

            configuration.CreateMap<ILocalizableString, string>().ConvertUsing(ls => ls?.Localize(localizationContext));
            configuration.CreateMap<LocalizableString, string>()
                .ConvertUsing(ls => ls == null ? null : localizationContext.LocalizationManager.GetString(ls));
        }
    }
}