using System;
using System.Collections.Generic;
using AutoMapper;

namespace StudioX.AutoMapper
{
    public interface IStudioXAutoMapperConfiguration
    {
        List<Action<IMapperConfigurationExpression>> Configurators { get; }

        /// <summary>
        /// Use static <see cref="Mapper.Instance"/>.
        /// Default: true.
        /// </summary>
        bool UseStaticMapper { get; set; }
    }
}