using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using StudioX.Application.Features;
using StudioX.Application.Navigation;
using StudioX.Authorization;
using StudioX.Configuration;
using StudioX.Configuration.Startup;
using StudioX.Localization;
using StudioX.Runtime.Session;
using StudioX.Timing;
using StudioX.Timing.Timezone;
using StudioX.Web.Models.StudioXUserConfiguration;
using StudioX.Web.Security.AntiForgery;
using System.Linq;
using StudioX.Dependency;
using StudioX.Extensions;
using System.Globalization;

namespace StudioX.Web.Configuration
{
    public class StudioXUserConfigurationBuilder : ITransientDependency
    {
        private readonly IMultiTenancyConfig multiTenancyConfig;
        private readonly ILanguageManager languageManager;
        private readonly ILocalizationManager localizationManager;
        private readonly IFeatureManager featureManager;
        private readonly IFeatureChecker featureChecker;
        private readonly IPermissionManager permissionManager;
        private readonly IUserNavigationManager userNavigationManager;
        private readonly ISettingDefinitionManager settingDefinitionManager;
        private readonly ISettingManager settingManager;
        private readonly IStudioXAntiForgeryConfiguration studioXAntiForgeryConfiguration;
        private readonly IStudioXSession studioXSession;
        private readonly IPermissionChecker permissionChecker;

        public IStudioXSession StudioXSession => studioXSession;

        public StudioXUserConfigurationBuilder(
            IMultiTenancyConfig multiTenancyConfig,
            ILanguageManager languageManager,
            ILocalizationManager localizationManager,
            IFeatureManager featureManager,
            IFeatureChecker featureChecker,
            IPermissionManager permissionManager,
            IUserNavigationManager userNavigationManager,
            ISettingDefinitionManager settingDefinitionManager,
            ISettingManager settingManager,
            IStudioXAntiForgeryConfiguration studioxAntiForgeryConfiguration,
            IStudioXSession studioxSession,
            IPermissionChecker permissionChecker)
        {
            this.multiTenancyConfig = multiTenancyConfig;
            this.languageManager = languageManager;
            this.localizationManager = localizationManager;
            this.featureManager = featureManager;
            this.featureChecker = featureChecker;
            this.permissionManager = permissionManager;
            this.userNavigationManager = userNavigationManager;
            this.settingDefinitionManager = settingDefinitionManager;
            this.settingManager = settingManager;
            studioXAntiForgeryConfiguration = studioxAntiForgeryConfiguration;
            studioXSession = studioxSession;
            this.permissionChecker = permissionChecker;
        }

        public async Task<StudioXUserConfigurationDto> GetAll()
        {
            return new StudioXUserConfigurationDto
            {
                MultiTenancy = GetUserMultiTenancyConfig(),
                Session = GetUserSessionConfig(),
                Localization = GetUserLocalizationConfig(),
                Features = await GetUserFeaturesConfig(),
                Auth = await GetUserAuthConfig(),
                Nav = await GetUserNavConfig(),
                Setting = await GetUserSettingConfig(),
                Clock = GetUserClockConfig(),
                Timing = await GetUserTimingConfig(),
                Security = GetUserSecurityConfig()
            };
        }

        private StudioXMultiTenancyConfigDto GetUserMultiTenancyConfig()
        {
            return new StudioXMultiTenancyConfigDto
            {
                IsEnabled = multiTenancyConfig.IsEnabled
            };
        }

        private StudioXUserSessionConfigDto GetUserSessionConfig()
        {
            return new StudioXUserSessionConfigDto
            {
                UserId = StudioXSession.UserId,
                TenantId = StudioXSession.TenantId,
                ImpersonatorUserId = StudioXSession.ImpersonatorUserId,
                ImpersonatorTenantId = StudioXSession.ImpersonatorTenantId,
                MultiTenancySide = StudioXSession.MultiTenancySide
            };
        }

        private StudioXUserLocalizationConfigDto GetUserLocalizationConfig()
        {
            var currentCulture = CultureInfo.CurrentUICulture;
            var languages = languageManager.GetLanguages();

            var config = new StudioXUserLocalizationConfigDto
            {
                CurrentCulture = new StudioXUserCurrentCultureConfigDto
                {
                    Name = currentCulture.Name,
                    DisplayName = currentCulture.DisplayName
                },
                Languages = languages.ToList()
            };

            if (languages.Count > 0)
            {
                config.CurrentLanguage = languageManager.CurrentLanguage;
            }

            var sources = localizationManager.GetAllSources().OrderBy(s => s.Name).ToArray();
            config.Sources = sources.Select(s => new StudioXLocalizationSourceDto
            {
                Name = s.Name,
                Type = s.GetType().Name
            }).ToList();

            config.Values = new Dictionary<string, Dictionary<string, string>>();
            foreach (var source in sources)
            {
                var stringValues = source.GetAllStrings(currentCulture).OrderBy(s => s.Name).ToList();
                var stringDictionary = stringValues
                    .ToDictionary(_ => _.Name, _ => _.Value);
                config.Values.Add(source.Name, stringDictionary);
            }

            return config;
        }

        private async Task<StudioXUserFeatureConfigDto> GetUserFeaturesConfig()
        {
            var config = new StudioXUserFeatureConfigDto()
            {
                AllFeatures = new Dictionary<string, StudioXStringValueDto>()
            };

            var allFeatures = featureManager.GetAll().ToList();

            if (StudioXSession.TenantId.HasValue)
            {
                var currentTenantId = StudioXSession.GetTenantId();
                foreach (var feature in allFeatures)
                {
                    var value = await featureChecker.GetValueAsync(currentTenantId, feature.Name);
                    config.AllFeatures.Add(feature.Name, new StudioXStringValueDto
                    {
                        Value = value
                    });
                }
            }
            else
            {
                foreach (var feature in allFeatures)
                {
                    config.AllFeatures.Add(feature.Name, new StudioXStringValueDto
                    {
                        Value = feature.DefaultValue
                    });
                }
            }

            return config;
        }

        private async Task<StudioXUserAuthConfigDto> GetUserAuthConfig()
        {
            var config = new StudioXUserAuthConfigDto();

            var allPermissionNames = permissionManager.GetAllPermissions(false).Select(p => p.Name).ToList();
            var grantedPermissionNames = new List<string>();

            if (StudioXSession.UserId.HasValue)
            {
                foreach (var permissionName in allPermissionNames)
                {
                    if (await permissionChecker.IsGrantedAsync(permissionName))
                    {
                        grantedPermissionNames.Add(permissionName);
                    }
                }
            }

            config.AllPermissions = allPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");
            config.GrantedPermissions = grantedPermissionNames.ToDictionary(permissionName => permissionName, permissionName => "true");

            return config;
        }

        private async Task<StudioXUserNavConfigDto> GetUserNavConfig()
        {
            var userMenus = await userNavigationManager.GetMenusAsync(StudioXSession.ToUserIdentifier());
            return new StudioXUserNavConfigDto
            {
                Menus = userMenus.ToDictionary(userMenu => userMenu.Name, userMenu => userMenu)
            };
        }

        private async Task<StudioXUserSettingConfigDto> GetUserSettingConfig()
        {
            var config = new StudioXUserSettingConfigDto
            {
                Values = new Dictionary<string, string>()
            };

            var settingDefinitions = settingDefinitionManager
                .GetAllSettingDefinitions()
                .Where(sd => sd.IsVisibleToClients);

            foreach (var settingDefinition in settingDefinitions)
            {
                var settingValue = await settingManager.GetSettingValueAsync(settingDefinition.Name);
                config.Values.Add(settingDefinition.Name, settingValue);
            }

            return config;
        }

        private StudioXUserClockConfigDto GetUserClockConfig()
        {
            return new StudioXUserClockConfigDto
            {
                Provider = Clock.Provider.GetType().Name.ToCamelCase()
            };
        }

        private async Task<StudioXUserTimingConfigDto> GetUserTimingConfig()
        {
            var timezoneId = await settingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);
            var timezone = TimeZoneInfo.FindSystemTimeZoneById(timezoneId);

            return new StudioXUserTimingConfigDto
            {
                TimeZoneInfo = new StudioXUserTimeZoneConfigDto
                {
                    Windows = new StudioXUserWindowsTimeZoneConfigDto
                    {
                        TimeZoneId = timezoneId,
                        BaseUtcOffsetInMilliseconds = timezone.BaseUtcOffset.TotalMilliseconds,
                        CurrentUtcOffsetInMilliseconds = timezone.GetUtcOffset(Clock.Now).TotalMilliseconds,
                        IsDaylightSavingTimeNow = timezone.IsDaylightSavingTime(Clock.Now)
                    },
                    Iana = new StudioXUserIanaTimeZoneConfigDto
                    {
                        TimeZoneId = TimezoneHelper.WindowsToIana(timezoneId)
                    }
                }
            };
        }

        private StudioXUserSecurityConfigDto GetUserSecurityConfig()
        {
            return new StudioXUserSecurityConfigDto()
            {
                AntiForgery = new StudioXUserAntiForgeryConfigDto
                {
                    TokenCookieName = studioXAntiForgeryConfiguration.TokenCookieName,
                    TokenHeaderName = studioXAntiForgeryConfiguration.TokenHeaderName
                }
            };
        }
    }
}