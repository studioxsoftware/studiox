using System;
using StudioX.Application.Services.Dto;
using StudioX.Auditing;
using StudioX.AutoMapper;

namespace StudioX.Boilerplate.AuditLogs.Dto
{
    [AutoMapFrom(typeof(AuditLog))]
    public class AuditLogDto : EntityDto
    {
        public virtual string UserName { get; set; }

        public virtual string ServiceName { get; set; }

        public virtual string MethodName { get; set; }

        public virtual string Parameters { get; set; }

        public virtual DateTime ExecutionTime { get; set; }

        public virtual string ExecutionTimeAgo { get; set; }

        public virtual int ExecutionDuration { get; set; }

        public virtual string ClientIpAddress { get; set; }

        public virtual string ClientName { get; set; }

        public virtual string BrowserInfo { get; set; }

        public virtual string Exception { get; set; }
    }
}