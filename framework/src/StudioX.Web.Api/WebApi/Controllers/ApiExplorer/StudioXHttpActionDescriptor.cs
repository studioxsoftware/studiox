using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Http.Controllers;

namespace StudioX.WebApi.Controllers.ApiExplorer
{
    public class StudioXHttpActionDescriptor : HttpActionDescriptor
    {
        public override string ActionName
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Type ReturnType
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public override Task<object> ExecuteAsync(HttpControllerContext controllerContext, IDictionary<string, object> arguments, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }

        public override Collection<HttpParameterDescriptor> GetParameters()
        {
            throw new NotImplementedException();
        }
    }
}
