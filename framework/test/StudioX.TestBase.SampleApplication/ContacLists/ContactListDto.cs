using StudioX.Application.Services.Dto;
using StudioX.AutoMapper;

namespace StudioX.TestBase.SampleApplication.ContacLists
{
    [AutoMapFrom(typeof(ContactList))]
    public class ContactListDto : EntityDto
    {
        public virtual string Name { get; set; }
    }
}