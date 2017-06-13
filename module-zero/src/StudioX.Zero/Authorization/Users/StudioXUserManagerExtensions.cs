using System;
using StudioX.Authorization.Roles;
using StudioX.Threading;

namespace StudioX.Authorization.Users
{
    /// <summary>
    /// Extension methods for <see cref="StudioXUserManager{TRole,TUser}"/>.
    /// </summary>
    public static class StudioXUserManagerExtensions
    {
        /// <summary>
        /// Check whether a user is granted for a permission.
        /// </summary>
        /// <param name="manager">User manager</param>
        /// <param name="userId">User id</param>
        /// <param name="permissionName">Permission name</param>
        public static bool IsGranted<TRole, TUser>(StudioXUserManager<TRole, TUser> manager, long userId, string permissionName)
            where TRole : StudioXRole<TUser>, new()
            where TUser : StudioXUser<TUser>
        {
            if (manager == null)
            {
                throw new ArgumentNullException(nameof(manager));
            }

            return AsyncHelper.RunSync(() => manager.IsGrantedAsync(userId, permissionName));
        }

        //public static StudioXUserManager<TRole, TUser> Login<TRole, TUser>(StudioXUserManager<TRole, TUser> manager, string userNameOrEmailAddress, string plainPassword, string tenancyName = null)
        //    where TRole : StudioXRole<TUser>, new()
        //    where TUser : StudioXUser<TUser>
        //{
        //    if (manager == null)
        //    {
        //        throw new ArgumentNullException(nameof(manager));
        //    }

        //    return AsyncHelper.RunSync(() => manager.LoginAsync(userNameOrEmailAddress, plainPassword, tenancyName));
        //}
    }
}