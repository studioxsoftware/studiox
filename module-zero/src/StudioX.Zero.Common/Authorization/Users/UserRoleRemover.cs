using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;

namespace StudioX.Authorization.Users
{
    /// <summary>
    /// Removes the user from all user roles when a user is deleted.
    /// </summary>
    public class UserRoleRemover :
        IEventHandler<EntityDeletedEventData<StudioXUserBase>>,
        ITransientDependency
    {
        private readonly IRepository<UserRole, long> userRoleRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public UserRoleRemover(
            IUnitOfWorkManager unitOfWorkManager, 
            IRepository<UserRole, long> userRoleRepository)
        {
            this.unitOfWorkManager = unitOfWorkManager;
            this.userRoleRepository = userRoleRepository;
        }

        [UnitOfWork]
        public virtual void HandleEvent(EntityDeletedEventData<StudioXUserBase> eventData)
        {
            using (unitOfWorkManager.Current.SetTenantId(eventData.Entity.TenantId))
            {
                userRoleRepository.Delete(
                    ur => ur.UserId == eventData.Entity.Id
                );
            }
        }
    }
}
