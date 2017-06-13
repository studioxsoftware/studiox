using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Events.Bus.Entities;
using StudioX.Events.Bus.Handlers;

namespace StudioX.Authorization.Users
{
    /// <summary>
    /// Synchronizes a user's information to user account.
    /// </summary>
    public class UserAccountSynchronizer :
        IEventHandler<EntityCreatedEventData<StudioXUserBase>>,
        IEventHandler<EntityDeletedEventData<StudioXUserBase>>,
        IEventHandler<EntityUpdatedEventData<StudioXUserBase>>,
        ITransientDependency
    {
        private readonly IRepository<UserAccount, long> userAccountRepository;
        private readonly IUnitOfWorkManager unitOfWorkManager;

        /// <summary>
        /// Constructor
        /// </summary>
        public UserAccountSynchronizer(
            IRepository<UserAccount, long> userAccountRepository,
            IUnitOfWorkManager unitOfWorkManager)
        {
            this.userAccountRepository = userAccountRepository;
            this.unitOfWorkManager = unitOfWorkManager;
        }

        /// <summary>
        /// Handles creation event of user
        /// </summary>
        [UnitOfWork]
        public virtual void HandleEvent(EntityCreatedEventData<StudioXUserBase> eventData)
        {
            using (unitOfWorkManager.Current.SetTenantId(null))
            {
                userAccountRepository.Insert(new UserAccount
                {
                    TenantId = eventData.Entity.TenantId,
                    UserName = eventData.Entity.UserName,
                    UserId = eventData.Entity.Id,
                    EmailAddress = eventData.Entity.EmailAddress,
                    LastLoginTime = eventData.Entity.LastLoginTime
                });
            }
        }

        /// <summary>
        /// Handles deletion event of user
        /// </summary>
        /// <param name="eventData"></param>
        [UnitOfWork]
        public virtual void HandleEvent(EntityDeletedEventData<StudioXUserBase> eventData)
        {
            using (unitOfWorkManager.Current.SetTenantId(null))
            {
                var userAccount =
                    userAccountRepository.FirstOrDefault(
                        ua => ua.TenantId == eventData.Entity.TenantId && ua.UserId == eventData.Entity.Id);
                if (userAccount != null)
                {
                    userAccountRepository.Delete(userAccount);
                }
            }
        }

        /// <summary>
        /// Handles update event of user
        /// </summary>
        /// <param name="eventData"></param>
        [UnitOfWork]
        public virtual void HandleEvent(EntityUpdatedEventData<StudioXUserBase> eventData)
        {
            using (unitOfWorkManager.Current.SetTenantId(null))
            {
                var userAccount = userAccountRepository.FirstOrDefault(ua => ua.TenantId == eventData.Entity.TenantId && ua.UserId == eventData.Entity.Id);
                if (userAccount != null)
                {
                    userAccount.UserName = eventData.Entity.UserName;
                    userAccount.EmailAddress = eventData.Entity.EmailAddress;
                    userAccount.LastLoginTime = eventData.Entity.LastLoginTime;
                    userAccountRepository.Update(userAccount);
                }
            }
        }
    }
}