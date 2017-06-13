using System.Collections.Generic;
using System.Reflection;
using System.Threading.Tasks;
using System.Linq;
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
                    LocalizationManager.GetString(StudioXConsts.LocalizationSourceName, "CurrentUserDidNotLoginToTheApplication")
                    );
            }

            foreach (var authorizeAttribute in authorizeAttributes)
            {
                await PermissionChecker.AuthorizeAsync(authorizeAttribute.RequireAllPermissions, authorizeAttribute.Permissions);
            }
        }

        public async Task AuthorizeAsync(MethodInfo methodInfo)
        {
            await CheckFeatures(methodInfo);
            await CheckPermissions(methodInfo);
        }

        private async Task CheckFeatures(MethodInfo methodInfo)
        {
            var featureAttributes =
                ReflectionHelper.GetAttributesOfMemberAndDeclaringType<RequiresFeatureAttribute>(
                    methodInfo
                    );

            if (featureAttributes.Count <= 0)
            {
                return;
            }

            foreach (var featureAttribute in featureAttributes)
            {
                await featureChecker.CheckEnabledAsync(featureAttribute.RequiresAll, featureAttribute.Features);
            }
        }

        private async Task CheckPermissions(MethodInfo methodInfo)
        {
            if (!authConfiguration.IsEnabled)
            {
                return;
            }

            if (AllowAnonymous(methodInfo))
            {
                return;
            }

            var authorizeAttributes =
                ReflectionHelper.GetAttributesOfMemberAndDeclaringType(
                    methodInfo
                ).OfType<IStudioXAuthorizeAttribute>().ToArray();

            if (!authorizeAttributes.Any())
            {
                return;
            }

            await AuthorizeAsync(authorizeAttributes);
        }

        private static bool AllowAnonymous(MethodInfo methodInfo)
        {
            return ReflectionHelper.GetAttributesOfMemberAndDeclaringType(methodInfo)
                .OfType<IStudioXAllowAnonymousAttribute>().Any();
        }
    }
}