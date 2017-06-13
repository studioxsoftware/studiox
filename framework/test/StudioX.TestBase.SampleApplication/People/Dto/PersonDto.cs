using StudioX.Application.Services.Dto;
using StudioX.AutoMapper;

namespace StudioX.TestBase.SampleApplication.People.Dto
{
    [AutoMap(typeof(Person))]
    public class PersonDto : EntityDto
    {
        public int ContactListId { get; set; }

        public string Name { get; set; }
    }
}