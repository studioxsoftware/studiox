using AutoMapper;
using IObjectMapper = StudioX.ObjectMapping.IObjectMapper;

namespace StudioX.AutoMapper
{
    public class AutoMapperObjectMapper : IObjectMapper
    {
        private readonly IMapper mapper;

        public AutoMapperObjectMapper(IMapper mapper)
        {
            this.mapper = mapper;
        }

        public TDestination Map<TDestination>(object source)
        {
            return mapper.Map<TDestination>(source);
        }

        public TDestination Map<TSource, TDestination>(TSource source, TDestination destination)
        {
            return mapper.Map(source, destination);
        }
    }
}