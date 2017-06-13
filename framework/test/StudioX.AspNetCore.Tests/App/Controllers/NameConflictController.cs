using System;
using StudioX.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.AspNetCore.App.Controllers
{
    public class NameConflictController : StudioXController
    {
        public string GetSelfActionUrl()
        {
            return Url.Action("GetSelfActionUrl", "NameConflict");
        }

        public string GetAppServiceActionUrlWithArea()
        {
            //Gets URL of NameConflictAppService.GetConstantString action
            return Url.Action("GetConstantString", "NameConflict", new { area = "app"});
        }
    }
}
