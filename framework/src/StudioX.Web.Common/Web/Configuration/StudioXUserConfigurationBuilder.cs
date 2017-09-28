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
        private readonly IMultiTenancyConfig _multiTenancyConfig;
        private readonly ILanguageManager _languageManager;
        private readonly ILocalizationManager _localizationManager;
        private readonly IFeatureManager _featureManager;
        private readonly IFeatureChecker _featureChecker;
        private readonly IPermissionManager _permissionManager;
        private readonly IUserNavigationManager _userNavigationManager;
        private readonly ISettingDefinitionManager _settingDefinitionManager;
        private readonly ISettingManager _settingManager;
        private readonly IStudioXAntiForgeryConfiguration _studioXAntiForgeryConfiguration;
        private readonly IStudioXSession _studioXSession;
        private readonly IPermissionChecker _permissionChecker;

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
            _multiTenancyConfig = multiTenancyConfig;
            _languageManager = languageManager;
            _localizationManager = localizationManager;
            _featureManager = featureManager;
            _featureChecker = featureChecker;
            _permissionManager = permissionManager;
            _userNavigationManager = userNavigationManager;
            _settingDefinitionManager = settingDefinitionManager;
            _settingManager = settingManager;
            _studioXAntiForgeryConfiguration = studioxAntiForgeryConfiguration;
            _studioXSession = studioxSession;
            _permissionChecker = permissionChecker;
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
                IsEnabled = _multiTenancyConfig.IsEnabled
            };
        }

        private StudioXUserSessionConfigDto GetUserSessionConfig()
        {
            return new StudioXUserSessionConfigDto
            {
                UserId = _studioXSession.UserId,
                TenantId = _studioXSession.TenantId,
                ImpersonatorUserId = _studioXSession.ImpersonatorUserId,
                ImpersonatorTenantId = _studioXSession.ImpersonatorTenantId,
                MultiTenancySide = _studioXSession.MultiTenancySide
            };
        }

        private StudioXUserLocalizationConfigDto GetUserLocalizationConfig()
        {
            var currentCulture = CultureInfo.CurrentUICulture;
            var languages = _languageManager.GetLanguages();

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
                config.CurrentLanguage = _languageManager.CurrentLanguage;
            }

            var sources = _localizationManager.GetAllSources().OrderBy(s => s.Name).ToArray();
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

            var allFeatures = _featureManager.GetAll().ToList();

            if (_studioXSession.TenantId.HasValue)
            {
                var currentTenantId = _studioXSession.GetTenantId();
                foreach (var feature in allFeatures)
                {
                    var value = await _featureChecker.GetValueAsync(currentTenantId, feature.Name);
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

            var allPermissionNames = _permissionManager.GetAllPermissions(false).Select(p => p.Name).ToList();
            var grantedPermissionNames = new List<string>();

            if (_studioXSession.UserId.HasValue)
            {
                foreach (var permissionName in allPermissionNames)
                {
                    if (await _permissionChecker.IsGrantedAsync(permissionName))
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
            var userMenus = await _userNavigationManager.GetMenusAsync(_studioXSession.ToUserIdentifier());
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

            var settingDefinitions = _settingDefinitionManager
                .GetAllSettingDefinitions()
                .Where(sd => sd.IsVisibleToClients);

            foreach (var settingDefinition in settingDefinitions)
            {
                var settingValue = await _settingManager.GetSettingValueAsync(settingDefinition.Name);
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
            var timezoneId = await _settingManager.GetSettingValueAsync(TimingSettingNames.TimeZone);
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
                    TokenCookieName = _studioXAntiForgeryConfiguration.TokenCookieName,
                    TokenHeaderName = _studioXAntiForgeryConfiguration.TokenHeaderName
                }
            };
        }
    }
}