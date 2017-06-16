using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.Boilerplate.AuditLogs.Dto;

namespace StudioX.Boilerplate.AuditLogs
{
    public interface IAuditLogAppService : IApplicationService
    {
        Task<ListResultDto<AuditLogListDto>> GetAll();

        PagedResultDto<AuditLogListDto> PagedResult(GetAuditLogsInput input);

        AuditLogDto Get(int id);
    }
}