using System;
using System.Collections.Generic;
using AutoMapper;

namespace StudioX.AutoMapper
{
    public class StudioXAutoMapperConfiguration : IStudioXAutoMapperConfiguration
    {
        public List<Action<IMapperConfigurationExpression>> Configurators { get; }

        public bool UseStaticMapper { get; set; }

        public StudioXAutoMapperConfiguration()
        {
            UseStaticMapper = true;
            Configurators = new List<Action<IMapperConfigurationExpression>>();
        }
    }
}