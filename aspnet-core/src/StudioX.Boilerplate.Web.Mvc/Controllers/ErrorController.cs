using System;
using StudioX.AspNetCore.Mvc.Controllers;
using StudioX.Web.Models;
using StudioX.Web.Mvc.Models;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc;

namespace StudioX.Boilerplate.Web.Controllers
{
    public class ErrorController : StudioXController
    {
        private readonly IErrorInfoBuilder errorInfoBuilder;

        public ErrorController(IErrorInfoBuilder errorInfoBuilder)
        {
            this.errorInfoBuilder = errorInfoBuilder;
        }

        public ActionResult Index()
        {
            var exHandlerFeature = HttpContext.Features.Get<IExceptionHandlerFeature>();

            var exception = exHandlerFeature != null
                                ? exHandlerFeature.Error
                                : new Exception("Unhandled exception!");

            return View(
                "Error",
                new ErrorViewModel(
                    errorInfoBuilder.BuildForException(exception),
                    exception
                )
            );
        }
    }
}