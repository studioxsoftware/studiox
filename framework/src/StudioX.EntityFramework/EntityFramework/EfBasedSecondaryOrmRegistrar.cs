using System;

namespace StudioX.EntityFramework
{
    public class EfBasedSecondaryOrmRegistrar : SecondaryOrmRegistrarBase
    {
        public EfBasedSecondaryOrmRegistrar(Type dbContextType, IDbContextEntityFinder dbContextEntityFinder)
            : base(dbContextType, dbContextEntityFinder)
        {
        }

        public override string OrmContextKey => StudioXConsts.Orms.EntityFramework;
    }
}
