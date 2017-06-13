using StudioX.Application.Services;
using StudioX.AspNetCore.App.Models;

namespace StudioX.AspNetCore.App.AppServices
{
    public class ParameterTestAppService : ApplicationService
    {
        public string GetComplexInput(SimpleViewModel model, bool testBool)
        {
            return "42";
        }
    }
}