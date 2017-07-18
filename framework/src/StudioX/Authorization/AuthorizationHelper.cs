using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using StudioX.Application.Features;
using StudioX.Configuration.Startup;
using StudioX.Dependency;
using StudioX.Localization;
using StudioX.Reflection;
using StudioX.Runtime.Session;

namespace StudioX.Authorization
{
    internal class AuthorizationHelper : IAuthorizationHelper, ITransientDependency
    {
        public IStudioXSession StudioXSession { get; set; }
        public IPermissionChecker PermissionChecker { get; set; }
        public IFeatureChecker FeatureChecker { get; set; }
        public ILocalizationManager LocalizationManager { get; set; }

        private readonly IFeatureChecker featureChecker;
        private readonly IAuthorizationConfiguration authConfiguration;

        public AuthorizationHelper(IFeatureChecker featureChecker, IAuthorizationConfiguration authConfiguration)
        {
            this.featureChecker = featureChecker;
            this.authConfiguration = authConfiguration;
            StudioXSession = NullStudioXSession.Instance;
            PermissionChecker = NullPermissionChecker.Instance;
            LocalizationManager = NullLocalizationManager.Instance;
        }

        public async Task AuthorizeAsync(IEnumerable<IStudioXAuthorizeAttribute> authorizeAttributes)
        {
            if (!authConfiguration.IsEnabled)
            {
                return;
            }

            if (!StudioXSession.UserId.HasValue)
            {
                throw new StudioXAuthorizationException(
                    LocalizationManager.GetString(StudioXConsts.LocalizationSourceName,
                        "CurrentUserDidNotLoginToTheApplication")
                );
            }

            foreach (var authorizeAttribute in authorizeAttributes)
            {
                await PermissionChecker.AuthorizeAsync(authorizeAttribute.RequireAllPermissions,
                    authorizeAttribute.Permissions);
            }
        }

        public async Task AuthorizeAsync(MethodInfo methodInfo, Type type)
        {
            await CheckFeatures(methodInfo, type);
            await CheckPermissions(methodInfo, type);
        }

        private async Task CheckFeatures(MethodInfo methodInfo, Type type)
        {
            var featureAttributes = ReflectionHelper.GetAttributesOfMemberAndType<RequiresFeatureAttribute>(methodInfo, type);

            if (featureAttributes.Count <= 0)
            {
                return;
            }

            foreach (var featureAttribute in featureAttributes)
            {
                await featureChecker.CheckEnabledAsync(featureAttribute.RequiresAll, featureAttribute.Features);
            }
        }

        private async Task CheckPermissions(MethodInfo methodInfo, Type type)
        {
            if (!authConfiguration.IsEnabled)
            {
                return;
            }

            if (AllowAnonymous(methodInfo, type))
            {
                return;
            }

            var authorizeAttributes =
                ReflectionHelper
                    .GetAttributesOfMemberAndType(methodInfo, type)
                    .OfType<IStudioXAuthorizeAttribute>()
                    .ToArray();

            if (!authorizeAttributes.Any())
            {
                return;
            }

            await AuthorizeAsync(authorizeAttributes);
        }

        private static bool AllowAnonymous(MemberInfo methodInfo, Type type)
        {
            return ReflectionHelper
                .GetAttributesOfMemberAndType(methodInfo, type)
                .OfType<IStudioXAllowAnonymousAttribute>()
                .Any();
        }
    }
}