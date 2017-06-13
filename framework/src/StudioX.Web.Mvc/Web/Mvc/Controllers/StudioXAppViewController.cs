using System;
using System.Web.Mvc;
using StudioX.Auditing;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.Runtime.Validation;

namespace StudioX.Web.Mvc.Controllers
{
    public class StudioXAppViewController : StudioXController
    {
        [DisableAuditing]
        [DisableValidation]
        [UnitOfWork(IsDisabled = true)]
        public ActionResult Load(string viewUrl)
        {
            if (viewUrl.IsNullOrEmpty())
            {
                throw new ArgumentNullException(nameof(viewUrl));
            }

            return View(viewUrl.EnsureStartsWith('~'));
        }
    }
}
