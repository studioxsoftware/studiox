using System.Collections.Generic;
using StudioX.Application.Features;
using StudioX.Application.Services;
using StudioX.Domain.Repositories;

namespace StudioX.TestBase.SampleApplication.ContacLists
{
    public class ContactListAppService : ApplicationService, IContactListAppService
    {
        private readonly IRepository<ContactList> contactListRepository;

        public ContactListAppService(IRepository<ContactList> contactListRepository)
        {
            this.contactListRepository = contactListRepository;
        }

        [RequiresFeature(SampleFeatureProvider.Names.Contacts)]
        public void Test()
        {
            //This method is called only if SampleFeatureProvider.Names.Contacts feature is enabled
        }

        public List<ContactListDto> GetContactLists()
        {
            return ObjectMapper.Map<List<ContactListDto>>(contactListRepository.GetAllList());
        }
    }
}