using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.MultiTenancy;
using Microsoft.EntityFrameworkCore;

namespace StudioX.Zero.EntityFrameworkCore
{
    [MultiTenancySide(MultiTenancySides.Host)]
    public abstract class StudioXZeroTenantDbContext<TRole, TUser,TSelf> : StudioXZeroCommonDbContext<TRole, TUser,TSelf>
        where TRole : StudioXRole<TUser>
        where TUser : StudioXUser<TUser>
        where TSelf: StudioXZeroTenantDbContext<TRole, TUser, TSelf>
    {

        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        protected StudioXZeroTenantDbContext(DbContextOptions<TSelf> options)
            :base(options)
        {

        }
    }
}