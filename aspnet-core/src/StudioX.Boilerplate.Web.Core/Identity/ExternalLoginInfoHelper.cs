using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using StudioX.Extensions;

namespace StudioX.Boilerplate.Identity
{
    public class ExternalLoginInfoHelper
    {
        public static (string firstName, string lastName) GetFirstNameAndLastNameFromClaims(List<Claim> claims)
        {
            string firstName = null;
            string lastName = null;

            var givennameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.GivenName);
            if (givennameClaim != null && !givennameClaim.Value.IsNullOrEmpty())
            {
                firstName = givennameClaim.Value;
            }

            var lastNameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Surname);
            if (lastNameClaim != null && !lastNameClaim.Value.IsNullOrEmpty())
            {
                lastName = lastNameClaim.Value;
            }

            if (firstName == null || lastName == null)
            {
                var nameClaim = claims.FirstOrDefault(c => c.Type == ClaimTypes.Name);
                if (nameClaim != null)
                {
                    var fullName = nameClaim.Value;
                    if (!fullName.IsNullOrEmpty())
                    {
                        var lastSpaceIndex = fullName.LastIndexOf(' ');
                        if (lastSpaceIndex < 1 || lastSpaceIndex > (fullName.Length - 2))
                        {
                            firstName = lastName = fullName;
                        }
                        else
                        {
                            firstName = fullName.Substring(0, lastSpaceIndex);
                            lastName = fullName.Substring(lastSpaceIndex);
                        }
                    }
                }
            }

            return (firstName, lastName);
        }
    }
}
