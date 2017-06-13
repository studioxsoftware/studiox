using System.Collections.Generic;
using StudioX.Application.Services;

namespace StudioX.TestBase.SampleApplication.ContacLists
{
    public interface IContactListAppService : IApplicationService
    {
        void Test();

        List<ContactListDto> GetContactLists();
    }
}
