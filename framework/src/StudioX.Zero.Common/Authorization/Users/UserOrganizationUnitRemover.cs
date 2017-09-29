using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;

namespace StudioX.Authorization.Users
{
    /// <summary>
    /// Removes the user from all organization units when a user is deleted.
    /// </summary>
    public class UserOrganizationUnitRemover : 
        IEventHandler<EntityDeletedEventData<StudioXUserBase>>, 
        ITransientDependency
    {
        private readonly IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        public UserOrganizationUnitRemover(
            IRepository<UserOrganizationUnit, long> userOrganizationUnitRepository, 
            IUnitOfWorkManager unitOfWorkManager)
        {
            this.userOrganizationUnitRepository = userOrganizationUnitRepository;
            this.unitOfWorkManager = unitOfWorkManager;
        }

        [UnitOfWork]
        public virtual void HandleEvent(EntityDeletedEventData<StudioXUserBase> eventData)
        {
            using (unitOfWorkManager.Current.SetTenantId(eventData.Entity.TenantId))
            {
                userOrganizationUnitRepository.Delete(
                    uou => uou.UserId == eventData.Entity.Id
                );
            }
        }
    }
}