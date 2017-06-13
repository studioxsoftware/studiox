using System;

using StudioX.EntityFramework;

namespace StudioX.EntityFrameworkCore
{
    public class EfCoreBasedSecondaryOrmRegistrar : SecondaryOrmRegistrarBase
    {
        public EfCoreBasedSecondaryOrmRegistrar(Type dbContextType, IDbContextEntityFinder dbContextEntityFinder)
            : base(dbContextType, dbContextEntityFinder)
        {
        }

        public override string OrmContextKey { get; } = StudioXConsts.Orms.EntityFrameworkCore;
    }
}
