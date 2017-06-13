using StudioX.Application.Services;

namespace StudioX.Web.Api.Tests.AppServices
{
    public interface IMyFirstAppService : IApplicationService
    {
        string GetStr(int i);

        [MyIgnoreApi]
        string GetStr2(int i);
    }
}