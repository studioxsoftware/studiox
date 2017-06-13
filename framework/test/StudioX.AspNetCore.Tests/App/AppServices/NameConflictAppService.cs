using StudioX.Application.Services;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.AspNetCore.App.AppServices
{
    public class NameConflictAppService : ApplicationService
    {
        [HttpGet]
        public string GetConstantString()
        {
            return "return-value-from-app-service";
        }
    }
}