using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Common;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Data.Entity.Infrastructure;
using System.Data.Entity.Validation;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Castle.Core.Logging;
using EntityFramework.DynamicFilters;
using StudioX.Collections.Extensions;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Domain.Entities;
using StudioX.Domain.Entities.Auditing;
using StudioX.Domain.Uow;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Entities;
using StudioX.Extensions;
using StudioX.Reflection;
using StudioX.Runtime.Session;
using StudioX.Timing;

namespace StudioX.EntityFramework
{
    /// <summary>
    ///     Base class for all DbContext classes in the application.
    /// </summary>
    public abstract class StudioXDbContext : DbContext, ITransientDependency, IShouldInitialize
    {
        /// <summary>
        ///     Used to get current session values.
        /// </summary>
        public IStudioXSession StudioXSession { get; set; }

        /// <summary>
        ///     Used to trigger entity change events.
        /// </summary>
        public IEntityChangeEventHelper EntityChangeEventHelper { get; set; }

        /// <summary>
        ///     Reference to the logger.
        /// </summary>
        public ILogger Logger { get; set; }

        /// <summary>
        ///     Reference to the event bus.
        /// </summary>
        public IEventBus EventBus { get; set; }

        /// <summary>
        ///     Reference to GUID generator.
        /// </summary>
        public IGuidGenerator GuidGenerator { get; set; }

        /// <summary>
        ///     Reference to the current UOW provider.
        /// </summary>
        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        /// <summary>
        ///     Reference to multi tenancy configuration.
        /// </summary>
        public IMultiTenancyConfig MultiTenancyConfig { get; set; }

        /// <summary>
        ///     Can be used to suppress automatically setting TenantId on SaveChanges.
        ///     Default: false.
        /// </summary>
        public bool SuppressAutoSetTenantId { get; set; }

        /// <summary>
        ///     Constructor.
        ///     Uses <see cref="IStudioXStartupConfiguration.DefaultNameOrConnectionString" /> as connection string.
        /// </summary>
        protected StudioXDbContext()
        {
            InitializeDbContext();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected StudioXDbContext(string nameOrConnectionString)
            : base(nameOrConnectionString)
        {
            InitializeDbContext();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected StudioXDbContext(DbCompiledModel model)
            : base(model)
        {
            InitializeDbContext();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected StudioXDbContext(DbConnection existingConnection, bool contextOwnsConnection)
            : base(existingConnection, contextOwnsConnection)
        {
            InitializeDbContext();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected StudioXDbContext(string nameOrConnectionString, DbCompiledModel model)
            : base(nameOrConnectionString, model)
        {
            InitializeDbContext();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected StudioXDbContext(ObjectContext objectContext, bool dbContextOwnsObjectContext)
            : base(objectContext, dbContextOwnsObjectContext)
        {
            InitializeDbContext();
        }

        /// <summary>
        ///     Constructor.
        /// </summary>
        protected StudioXDbContext(DbConnection existingConnection, DbCompiledModel model, bool contextOwnsConnection)
            : base(existingConnection, model, contextOwnsConnection)
        {
            InitializeDbContext();
        }

        private void InitializeDbContext()
        {
            SetNullsForInjectedProperties();
            RegisterToChanges();
        }

        private void RegisterToChanges()
        {
            ((IObjectContextAdapter) this)
                .ObjectContext
                .ObjectStateManager
                .ObjectStateManagerChanged += ObjectStateManager_ObjectStateManagerChanged;
        }

        protected virtual void ObjectStateManager_ObjectStateManagerChanged(object sender, CollectionChangeEventArgs e)
        {
            var contextAdapter = (IObjectContextAdapter) this;
            if (e.Action != CollectionChangeAction.Add)
            {
                return;
            }

            var entry = contextAdapter.ObjectContext.ObjectStateManager.GetObjectStateEntry(e.Element);
            switch (entry.State)
            {
                case EntityState.Added:
                    CheckAndSetId(entry.Entity);
                    CheckAndSetMustHaveTenantIdProperty(entry.Entity);
                    SetCreationAuditProperties(entry.Entity, GetAuditUserId());
                    break;
                //case EntityState.Deleted: //It's not going here at all
                //    SetDeletionAuditProperties(entry.Entity, GetAuditUserId());
                //    break;
            }
        }

        private void SetNullsForInjectedProperties()
        {
            Logger = NullLogger.Instance;
            StudioXSession = NullStudioXSession.Instance;
            EntityChangeEventHelper = NullEntityChangeEventHelper.Instance;
            GuidGenerator = SequentialGuidGenerator.Instance;
            EventBus = NullEventBus.Instance;
        }

        public virtual void Initialize()
        {
            Database.Initialize(false);
            this.SetFilterScopedParameterValue(StudioXDataFilters.MustHaveTenant, StudioXDataFilters.Parameters.TenantId,
                StudioXSession.TenantId ?? 0);
            this.SetFilterScopedParameterValue(StudioXDataFilters.MayHaveTenant, StudioXDataFilters.Parameters.TenantId,
                StudioXSession.TenantId);
        }

        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Filter(StudioXDataFilters.SoftDelete, (ISoftDelete d) => d.IsDeleted, false);
            modelBuilder.Filter(StudioXDataFilters.MustHaveTenant,
                    (IMustHaveTenant t, int tenantId) => t.TenantId == tenantId || (int?) t.TenantId == null, 0);
                //While "(int?)t.TenantId == null" seems wrong, it's needed. See https://github.com/jcachat/EntityFramework.DynamicFilters/issues/62#issuecomment-208198058
            modelBuilder.Filter(StudioXDataFilters.MayHaveTenant,
                (IMayHaveTenant t, int? tenantId) => t.TenantId == tenantId, 0);
        }

        public override int SaveChanges()
        {
            try
            {
                var changedEntities = ApplyStudioXConcepts();
                var result = base.SaveChanges();
                EntityChangeEventHelper.TriggerEvents(changedEntities);
                return result;
            }
            catch (DbEntityValidationException ex)
            {
                LogDbEntityValidationException(ex);
                throw;
            }
        }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken)
        {
            try
            {
                var changeReport = ApplyStudioXConcepts();
                var result = await base.SaveChangesAsync(cancellationToken);
                await EntityChangeEventHelper.TriggerEventsAsync(changeReport);
                return result;
            }
            catch (DbEntityValidationException ex)
            {
                LogDbEntityValidationException(ex);
                throw;
            }
        }

        protected virtual EntityChangeReport ApplyStudioXConcepts()
        {
            var changeReport = new EntityChangeReport();

            var userId = GetAuditUserId();

            var entries = ChangeTracker.Entries().ToList();
            foreach (var entry in entries)
            {
                switch (entry.State)
                {
                    case EntityState.Added:
                        CheckAndSetId(entry.Entity);
                        CheckAndSetMustHaveTenantIdProperty(entry.Entity);
                        CheckAndSetMayHaveTenantIdProperty(entry.Entity);
                        SetCreationAuditProperties(entry.Entity, userId);
                        changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Created));
                        break;
                    case EntityState.Modified:
                        SetModificationAuditProperties(entry.Entity, userId);
                        if (entry.Entity is ISoftDelete && entry.Entity.As<ISoftDelete>().IsDeleted)
                        {
                            SetDeletionAuditProperties(entry.Entity, userId);
                            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity,
                                EntityChangeType.Deleted));
                        }
                        else
                        {
                            changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity,
                                EntityChangeType.Updated));
                        }

                        break;
                    case EntityState.Deleted:
                        CancelDeletionForSoftDelete(entry);
                        SetDeletionAuditProperties(entry.Entity, userId);
                        changeReport.ChangedEntities.Add(new EntityChangeEntry(entry.Entity, EntityChangeType.Deleted));
                        break;
                }

                AddDomainEvents(changeReport.DomainEvents, entry.Entity);
            }

            return changeReport;
        }

        protected virtual void AddDomainEvents(List<DomainEventEntry> domainEvents, object entityAsObj)
        {
            var generatesDomainEventsEntity = entityAsObj as IGeneratesDomainEvents;
            if (generatesDomainEventsEntity == null)
            {
                return;
            }

            if (generatesDomainEventsEntity.DomainEvents.IsNullOrEmpty())
            {
                return;
            }

            domainEvents.AddRange(
                generatesDomainEventsEntity.DomainEvents.Select(
                    eventData => new DomainEventEntry(entityAsObj, eventData)));
            generatesDomainEventsEntity.DomainEvents.Clear();
        }

        protected virtual void CheckAndSetId(object entityAsObj)
        {
            //Set GUID Ids
            var entity = entityAsObj as IEntity<Guid>;
            if (entity != null && entity.Id == Guid.Empty)
            {
                var entityType = ObjectContext.GetObjectType(entityAsObj.GetType());
                var idProperty = entityType.GetProperty("Id");
                var dbGeneratedAttr =
                    ReflectionHelper.GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(idProperty);
                if (dbGeneratedAttr == null || dbGeneratedAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.None)
                {
                    entity.Id = GuidGenerator.Create();
                }
            }
        }

        protected virtual void CheckAndSetMustHaveTenantIdProperty(object entityAsObj)
        {
            if (SuppressAutoSetTenantId)
            {
                return;
            }

            //Only set IMustHaveTenant entities
            if (!(entityAsObj is IMustHaveTenant))
            {
                return;
            }

            var entity = entityAsObj.As<IMustHaveTenant>();

            //Don't set if it's already set
            if (entity.TenantId != 0)
            {
                return;
            }

            var currentTenantId = GetCurrentTenantIdOrNull();

            if (currentTenantId != null)
            {
                entity.TenantId = currentTenantId.Value;
            }
            else
            {
                throw new StudioXException("Can not set TenantId to 0 for IMustHaveTenant entities!");
            }
        }

        protected virtual void CheckAndSetMayHaveTenantIdProperty(object entityAsObj)
        {
            if (SuppressAutoSetTenantId)
            {
                return;
            }

            //Only set IMayHaveTenant entities
            if (!(entityAsObj is IMayHaveTenant))
            {
                return;
            }

            var entity = entityAsObj.As<IMayHaveTenant>();

            //Don't set if it's already set
            if (entity.TenantId != null)
            {
                return;
            }

            //Only works for single tenant applications
            if (MultiTenancyConfig?.IsEnabled ?? false)
            {
                return;
            }

            //Don't set if MayHaveTenant filter is disabled
            if (!this.IsFilterEnabled(StudioXDataFilters.MayHaveTenant))
            {
                return;
            }

            entity.TenantId = GetCurrentTenantIdOrNull();
        }

        protected virtual void SetCreationAuditProperties(object entityAsObj, long? userId)
        {
            EntityAuditingHelper.SetCreationAuditProperties(MultiTenancyConfig, entityAsObj, StudioXSession.TenantId,
                userId);
        }

        protected virtual void SetModificationAuditProperties(object entityAsObj, long? userId)
        {
            EntityAuditingHelper.SetModificationAuditProperties(MultiTenancyConfig, entityAsObj, StudioXSession.TenantId,
                userId);
        }

        protected virtual void CancelDeletionForSoftDelete(DbEntityEntry entry)
        {
            if (!(entry.Entity is ISoftDelete))
            {
                return;
            }

            var softDeleteEntry = entry.Cast<ISoftDelete>();
            softDeleteEntry.Reload();
            softDeleteEntry.State = EntityState.Modified;
            softDeleteEntry.Entity.IsDeleted = true;
        }

        protected virtual void SetDeletionAuditProperties(object entityAsObj, long? userId)
        {
            if (entityAsObj is IHasDeletionTime)
            {
                var entity = entityAsObj.As<IHasDeletionTime>();

                if (entity.DeletionTime == null)
                {
                    entity.DeletionTime = Clock.Now;
                }
            }

            if (entityAsObj is IDeletionAudited)
            {
                var entity = entityAsObj.As<IDeletionAudited>();

                if (entity.DeleterUserId != null)
                {
                    return;
                }

                if (userId == null)
                {
                    entity.DeleterUserId = null;
                    return;
                }

                //Special check for multi-tenant entities
                if (entity is IMayHaveTenant || entity is IMustHaveTenant)
                {
                    //Sets LastModifierUserId only if current user is in same tenant/host with the given entity
                    if (entity is IMayHaveTenant && entity.As<IMayHaveTenant>().TenantId == StudioXSession.TenantId ||
                        entity is IMustHaveTenant && entity.As<IMustHaveTenant>().TenantId == StudioXSession.TenantId)
                    {
                        entity.DeleterUserId = userId;
                    }
                    else
                    {
                        entity.DeleterUserId = null;
                    }
                }
                else
                {
                    entity.DeleterUserId = userId;
                }
            }
        }

        protected virtual void LogDbEntityValidationException(DbEntityValidationException exception)
        {
            Logger.Error("There are some validation errors while saving changes in EntityFramework:");
            foreach (var ve in exception.EntityValidationErrors.SelectMany(eve => eve.ValidationErrors))
            {
                Logger.Error(" - " + ve.PropertyName + ": " + ve.ErrorMessage);
            }
        }

        protected virtual long? GetAuditUserId()
        {
            if (StudioXSession.UserId.HasValue &&
                CurrentUnitOfWorkProvider != null &&
                CurrentUnitOfWorkProvider.Current != null &&
                CurrentUnitOfWorkProvider.Current.GetTenantId() == StudioXSession.TenantId)
            {
                return StudioXSession.UserId;
            }

            return null;
        }

        protected virtual int? GetCurrentTenantIdOrNull()
        {
            if (CurrentUnitOfWorkProvider?.Current != null)
            {
                return CurrentUnitOfWorkProvider.Current.GetTenantId();
            }

            return StudioXSession.TenantId;
        }
    }
}