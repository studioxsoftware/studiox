using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Reflection;

using StudioX.Domain.Entities;
using StudioX.Domain.Uow;
using StudioX.Reflection;
using StudioX.Runtime.Session;

namespace StudioX.Dapper.Filters.Action
{
    public abstract class DapperActionFilterBase
    {
        protected DapperActionFilterBase()
        {
            StudioXSession = NullStudioXSession.Instance;
            GuidGenerator = SequentialGuidGenerator.Instance;
        }

        public IStudioXSession StudioXSession { get; set; }

        public ICurrentUnitOfWorkProvider CurrentUnitOfWorkProvider { get; set; }

        public IGuidGenerator GuidGenerator { get; set; }

        protected virtual long? GetAuditUserId()
        {
            if (StudioXSession.UserId.HasValue && CurrentUnitOfWorkProvider?.Current != null)
            {
                return StudioXSession.UserId;
            }

            return null;
        }

        protected virtual void CheckAndSetId(object entityAsObj)
        {
            var entity = entityAsObj as IEntity<Guid>;
            if (entity != null && entity.Id == Guid.Empty)
            {
                Type entityType = entityAsObj.GetType();
                PropertyInfo idProperty = entityType.GetProperty("Id");
                var dbGeneratedAttr = ReflectionHelper.GetSingleAttributeOrDefault<DatabaseGeneratedAttribute>(idProperty);
                if (dbGeneratedAttr == null || dbGeneratedAttr.DatabaseGeneratedOption == DatabaseGeneratedOption.None)
                {
                    entity.Id = GuidGenerator.Create();
                }
            }
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
