using System.Threading.Tasks;
using StudioX.Authorization.Roles;
using StudioX.Authorization.Users;
using StudioX.Dependency;
using StudioX.Domain.Uow;
using StudioX.Runtime.Session;
using Castle.Core.Logging;

namespace StudioX.Authorization
{
    /// <summary>
    /// Application should inherit this class to implement <see cref="IPermissionChecker"/>.
    /// </summary>
    /// <typeparam name="TRole"></typeparam>
    /// <typeparam name="TUser"></typeparam>
    public abstract class PermissionChecker<TRole, TUser> : IPermissionChecker, ITransientDependency, IIocManagerAccessor
        where TRole : StudioXRole<TUser>, new()
        where TUser : StudioXUser<TUser>
    {
        private readonly StudioXUserManager<TRole, TUser> userManager;

        public IIocManager IocManager { get; set; }

        public ILogger Logger { get; set; }

        public IStudioXSession StudioXSession { get; set; }

        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        /// <summary>
        /// Constructor.
        /// </summary>
        protected PermissionChecker(StudioXUserManager<TRole, TUser> userManager)
        {
            this.userManager = userManager;

            Logger = NullLogger.Instance;
            StudioXSession = NullStudioXSession.Instance;
        }

        public virtual async Task<bool> IsGrantedAsync(string permissionName)
        {
            return StudioXSession.UserId.HasValue && await userManager.IsGrantedAsync(StudioXSession.UserId.Value, permissionName);
        }

        public virtual async Task<bool> IsGrantedAsync(long userId, string permissionName)
        {
            return await userManager.IsGrantedAsync(userId, permissionName);
        }

        [UnitOfWork]
        public virtual async Task<bool> IsGrantedAsync(UserIdentifier user, string permissionName)
        {
            if (CurrentUnitOfWorkProvider?.Current == null)
            {
                return await IsGrantedAsync(user.UserId, permissionName);
            }

            using (CurrentUnitOfWorkProvider.Current.SetTenantId(user.TenantId))
            {
                return await userManager.IsGrantedAsync(user.UserId, permissionName);
            }
        }
    }
}
