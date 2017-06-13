using System;
using StudioX.Application.Services.Dto;

namespace StudioX.Boilerplate.AuditLogs.Dto
{
    public class AuditLogListDto : EntityDto<long>
    {
        public virtual long? UserId { get; set; }

        public virtual string BranchName { get; set; }

        public virtual string UserName { get; set; }

        public virtual string ServiceName { get; set; }

        public virtual string MethodName { get; set; }

        public virtual DateTime ExecutionTime { get; set; }

        public virtual int ExecutionDuration { get; set; }

        public virtual string ClientIpAddress { get; set; }

        public virtual string ClientName { get; set; }

        public virtual string BrowserInfo { get; set; }

        public virtual string Exception { get; set; }
    }
}