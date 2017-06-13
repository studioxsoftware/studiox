using System.Threading.Tasks;
using StudioX.Dependency;
using StudioX.Domain.Repositories;

namespace StudioX.Auditing
{
    /// <summary>
    /// Implements <see cref="IAuditingStore"/> to save auditing informations to database.
    /// </summary>
    public class AuditingStore : IAuditingStore, ITransientDependency
    {
        private readonly IRepository<AuditLog, long> auditLogRepository;

        /// <summary>
        /// Creates  a new <see cref="AuditingStore"/>.
        /// </summary>
        public AuditingStore(IRepository<AuditLog, long> auditLogRepository)
        {
            this.auditLogRepository = auditLogRepository;
        }

        public virtual Task SaveAsync(AuditInfo auditInfo)
        {
            return auditLogRepository.InsertAsync(AuditLog.CreateFromAuditInfo(auditInfo));
        }
    }
}