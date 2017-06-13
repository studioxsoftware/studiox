using System.Linq;
using System.Linq.Dynamic.Core;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using StudioX.Application.Services.Dto;
using StudioX.Auditing;
using StudioX.AutoMapper;
using StudioX.Configuration;
using StudioX.Domain.Repositories;
using StudioX.Extensions;
using StudioX.Linq.Extensions;
using StudioX.Boilerplate.AuditLogs.Dto;
using StudioX.Boilerplate.BaseModel;
using StudioX.Boilerplate.Configuration;

namespace StudioX.Boilerplate.AuditLogs
{
    [DisableAuditing]
    public class AuditLogAppService : BoilerplateAppServiceBase, IAuditLogAppService
    {
        private readonly IRepository<AuditLog, long> auditLogRepository;

        public AuditLogAppService(IRepository<AuditLog, long> auditLogRepository)
        {
            this.auditLogRepository = auditLogRepository;
        }

        public async Task<ListResultDto<AuditLogListDto>> GetAll()
        {
            var auditLogs = await QueryBuilder().ToListAsync();
            return new ListResultDto<AuditLogListDto>(auditLogs);
        }

        public PagedResultDto<AuditLogListDto> GetListPagedResult(GetAuditLogsInput input)
        {
            if (input.MaxResultCount <= 0)
                input.MaxResultCount = SettingManager.GetSettingValue<int>(BoilerplateSettingProvider.AuditLogsDefaultPageSize);

            if (input.Sorting.IsNullOrEmpty())
                input.Sorting = InputConstant.DefaultSorting;

            var query = QueryBuilder(input);
            var totalCount = query.Count();

            var auditLogs = query.OrderBy(input.Sorting)
                .PageBy(input)
                .ToList();

            return new PagedResultDto<AuditLogListDto>
            {
                TotalCount = totalCount,
                Items = auditLogs
            };
        }

        public AuditLogDto Get(int id)
        {
            var auditLog = auditLogRepository.Get(id);
            var auditLogDto = auditLog.MapTo<AuditLogDto>();
            auditLogDto.ExecutionTimeAgo = auditLogDto.ExecutionTime.ToTimeAgo();

            if (auditLog.UserId != null)
            {
                var user = UserManager.GetUserByIdAsync(auditLog.UserId.Value).Result;
                auditLogDto.UserName = user.UserName;
            }

            return auditLogDto;
        }

        #region Private methods

        private IQueryable<AuditLogListDto> QueryBuilder()
        {
            var query = auditLogRepository.GetAll()
                .GroupJoin(UserManager.Users, auditLog => auditLog.UserId, user => user.Id,
                    (auditLog, user) => new {AuditLog = auditLog, User = user})
                .SelectMany(t => t.User.DefaultIfEmpty(), (a, u) => new AuditLogListDto
                {
                    Id = a.AuditLog.Id,
                    UserName = u == null ? "" : u.UserName,
                    ServiceName = a.AuditLog.ServiceName,
                    MethodName = a.AuditLog.MethodName,
                    ExecutionTime = a.AuditLog.ExecutionTime,
                    ExecutionDuration = a.AuditLog.ExecutionDuration,
                    ClientIpAddress = a.AuditLog.ClientIpAddress,
                    BrowserInfo = a.AuditLog.BrowserInfo,
                    ClientName = a.AuditLog.ClientName,
                    Exception = a.AuditLog.Exception
                });

            return query;
        }

        private IQueryable<AuditLogListDto> QueryBuilder(GetAuditLogsInput input)
        {
            var query = QueryBuilder().Where(x => x.ExecutionTime >= input.StartDate 
                            && x.ExecutionTime <= input.EndDate);

            if (input.ExecutionDurationFrom.HasValue && input.ExecutionDurationTo.HasValue)
                query = query.Where(x => x.ExecutionDuration >= input.ExecutionDurationFrom
                            && x.ExecutionDuration <= input.ExecutionDurationTo);

            if (input.ExecutionDurationFrom.HasValue && !input.ExecutionDurationTo.HasValue)
                query = query.Where(x => x.ExecutionDuration >= input.ExecutionDurationFrom);

            if (!input.ExecutionDurationFrom.HasValue && input.ExecutionDurationTo.HasValue)
                query = query.Where(x => x.ExecutionDuration <= input.ExecutionDurationTo);

            if (!input.UserName.IsNullOrEmpty())
                query = query.Where(x => x.UserName.ToLower().Contains(input.UserName.Trim().ToLower()));

            if (!input.ServiceName.IsNullOrEmpty())
                query = query.Where(x => x.ServiceName.ToLower().Contains(input.ServiceName.Trim().ToLower()));

            if (!input.MethodName.IsNullOrEmpty())
                query = query.Where(x => x.MethodName.ToLower().Contains(input.MethodName.Trim().ToLower()));

            if (!input.BrowserInfo.IsNullOrEmpty())
                query = query.Where(x => x.BrowserInfo.ToLower().Contains(input.BrowserInfo.Trim().ToLower()));

            if (input.HasError.HasValue)
                query = query.Where(x => input.HasError.Value ? x.Exception != null : x.Exception == null);

            return query;
        }

        #endregion
    }
}