using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.TestBase.SampleApplication.People.Dto;

namespace StudioX.TestBase.SampleApplication.People
{
    public interface IPersonAppService : IApplicationService
    {
        ListResultDto<PersonDto> GetPeople(GetPeopleInput input);

        Task CreatePersonAsync(CreatePersonInput input);

        Task DeletePerson(EntityDto input);

        string TestPrimitiveMethod(int a, string b, EntityDto c);
    }
}