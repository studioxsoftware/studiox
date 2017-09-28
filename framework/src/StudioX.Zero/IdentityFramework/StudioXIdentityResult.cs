using System.Collections.Generic;
using Microsoft.AspNet.Identity;

namespace StudioX.IdentityFramework
{
    public class StudioXIdentityResult : IdentityResult
    {
        public StudioXIdentityResult()
        {
            
        }

        public StudioXIdentityResult(IEnumerable<string> errors)
            : base(errors)
        {
            
        }

        public StudioXIdentityResult(params string[] errors)
            :base(errors)
        {
            
        }

        public static StudioXIdentityResult Failed(params string[] errors)
        {
            return new StudioXIdentityResult(errors);
        }
    }
}