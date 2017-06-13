using StudioX.AutoMapper;
using StudioX.Modules;
using StudioX.Reflection.Extensions;
using StudioX.Zero;
using IdentityServer4.Models;

namespace StudioX.IdentityServer4
{
    [DependsOn(typeof(StudioXZeroCoreModule), typeof(StudioXAutoMapperModule))]
    public class StudioXZeroCoreIdentityServerModule : StudioXModule
    {
        public override void PreInitialize()
        {
            Configuration.Modules.StudioXAutoMapper().Configurators.Add(config =>
            {
                //PersistedGrant -> PersistedGrantEntity
                config.CreateMap<PersistedGrant, PersistedGrantEntity>()
                    .ForMember(d => d.Id, c => c.MapFrom(s => s.Key));

                //PersistedGrantEntity -> PersistedGrant
                config.CreateMap<PersistedGrantEntity, PersistedGrant>()
                    .ForMember(d => d.Key, c => c.MapFrom(s => s.Id));
            });
        }

        public override void Initialize()
        {
            IocManager.RegisterAssemblyByConvention(typeof(StudioXZeroCoreIdentityServerModule).GetAssembly());
        }
    }
}
