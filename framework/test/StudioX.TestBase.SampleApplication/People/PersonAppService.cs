using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using StudioX.Application.Services;
using StudioX.Application.Services.Dto;
using StudioX.Auditing;
using StudioX.Authorization;
using StudioX.Domain.Repositories;
using StudioX.Extensions;
using StudioX.TestBase.SampleApplication.People.Dto;

namespace StudioX.TestBase.SampleApplication.People
{
    public class PersonAppService : ApplicationService, IPersonAppService
    {
        private readonly IRepository<Person> personRepository;

        public PersonAppService(IRepository<Person> personRepository)
        {
            this.personRepository = personRepository;
        }

        [DisableAuditing]
        public ListResultDto<PersonDto> GetPeople(GetPeopleInput input)
        {
            var query = personRepository.GetAll();

            if (!input.NameFilter.IsNullOrEmpty())
            {
                query = query.Where(p => p.Name.Contains(input.NameFilter));
            }

            var people = query.ToList();

            return new ListResultDto<PersonDto>(ObjectMapper.Map<List<PersonDto>>(people));
        }

        public async Task CreatePersonAsync(CreatePersonInput input)
        {
            await personRepository.InsertAsync(ObjectMapper.Map<Person>(input));
        }

        [StudioXAuthorize("CanDeletePerson")]
        public async Task DeletePerson(EntityDto input)
        {
            await personRepository.DeleteAsync(input.Id);
        }

        public string TestPrimitiveMethod(int a, string b, EntityDto c)
        {
            return a + "#" + b + "#" + c.Id;
        }
    }
}
