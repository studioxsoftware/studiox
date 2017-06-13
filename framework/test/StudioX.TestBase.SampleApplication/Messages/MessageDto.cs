using StudioX.Application.Services.Dto;
using StudioX.AutoMapper;

namespace StudioX.TestBase.SampleApplication.Messages
{
    [AutoMap(typeof(Message))]
    public class MessageDto : FullAuditedEntityDto
    {
        public int? TenantId { get; set; }

        public string Text { get; set; }
    }
}