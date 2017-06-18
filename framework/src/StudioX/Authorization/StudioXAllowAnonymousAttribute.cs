using System;

namespace StudioX.Authorization
{
    /// <summary>
    ///     Used to allow a method to be accessed by any user.
    ///     Suppress <see cref="StudioXAuthorizeAttribute" /> defined in the class containing that method.
    /// </summary>
    public class StudioXAllowAnonymousAttribute : Attribute, IStudioXAllowAnonymousAttribute
    {
    }
}