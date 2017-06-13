using System;
using System.Collections.Generic;
using StudioX.Domain.Entities;

namespace StudioX.EntityFramework
{
    public interface IDbContextEntityFinder
    {
        IEnumerable<EntityTypeInfo> GetEntityTypeInfos(Type dbContextType);
    }
}