using StudioX.Application.Services.Dto;

namespace StudioX.TestBase.SampleApplication.Messages
{
    public class GetMessagesWithFilterInput : PagedAndSortedResultRequestDto
    {
        public string Text { get; set; }
    }
}