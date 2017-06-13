using System;
using StudioX;
using StudioX.Authorization;
using StudioX.Dependency;
using StudioX.UI;

namespace StudioX.Boilerplate.Authorization
{
    public class StudioXLoginResultTypeHelper : StudioXServiceBase, ITransientDependency
    {
        public StudioXLoginResultTypeHelper()
        {
            LocalizationSourceName = BoilerplateConsts.LocalizationSourceName;
        }

        public Exception CreateExceptionForFailedLoginAttempt(StudioXLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        {
            switch (result)
            {
                case StudioXLoginResultType.Success:
                    return new Exception("Don't call this method with a success result!");
                case StudioXLoginResultType.InvalidUserNameOrEmailAddress:
                case StudioXLoginResultType.InvalidPassword:
                    return new UserFriendlyException(L("LoginFailed"), L("InvalidUserNameOrPassword"));
                case StudioXLoginResultType.InvalidTenancyName:
                    return new UserFriendlyException(L("LoginFailed"), L("ThereIsNoTenantDefinedWithName{0}", tenancyName));
                case StudioXLoginResultType.TenantIsNotActive:
                    return new UserFriendlyException(L("LoginFailed"), L("TenantIsNotActive", tenancyName));
                case StudioXLoginResultType.UserIsNotActive:
                    return new UserFriendlyException(L("LoginFailed"), L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress));
                case StudioXLoginResultType.UserEmailIsNotConfirmed:
                    return new UserFriendlyException(L("LoginFailed"), L("UserEmailIsNotConfirmedAndCanNotLogin"));
                case StudioXLoginResultType.LockedOut:
                    return new UserFriendlyException(L("LoginFailed"), L("UserLockedOutMessage"));
                default: //Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn("Unhandled login fail reason: " + result);
                    return new UserFriendlyException(L("LoginFailed"));
            }
        }

        public string CreateLocalizedMessageForFailedLoginAttempt(StudioXLoginResultType result, string usernameOrEmailAddress, string tenancyName)
        {
            switch (result)
            {
                case StudioXLoginResultType.Success:
                    throw new Exception("Don't call this method with a success result!");
                case StudioXLoginResultType.InvalidUserNameOrEmailAddress:
                case StudioXLoginResultType.InvalidPassword:
                    return L("InvalidUserNameOrPassword");
                case StudioXLoginResultType.InvalidTenancyName:
                    return L("ThereIsNoTenantDefinedWithName{0}", tenancyName);
                case StudioXLoginResultType.TenantIsNotActive:
                    return L("TenantIsNotActive", tenancyName);
                case StudioXLoginResultType.UserIsNotActive:
                    return L("UserIsNotActiveAndCanNotLogin", usernameOrEmailAddress);
                case StudioXLoginResultType.UserEmailIsNotConfirmed:
                    return L("UserEmailIsNotConfirmedAndCanNotLogin");
                default: //Can not fall to default actually. But other result types can be added in the future and we may forget to handle it
                    Logger.Warn("Unhandled login fail reason: " + result);
                    return L("LoginFailed");
            }
        }
    }
}
