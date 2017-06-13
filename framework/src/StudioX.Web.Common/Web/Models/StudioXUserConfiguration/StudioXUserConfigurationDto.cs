namespace StudioX.Web.Models.StudioXUserConfiguration
{
    public class StudioXUserConfigurationDto
    {
        public StudioXMultiTenancyConfigDto MultiTenancy { get; set; }

        public StudioXUserSessionConfigDto Session { get; set; }

        public StudioXUserLocalizationConfigDto Localization { get; set; }

        public StudioXUserFeatureConfigDto Features { get; set; }

        public StudioXUserAuthConfigDto Auth { get; set; }

        public StudioXUserNavConfigDto Nav { get; set; }

        public StudioXUserSettingConfigDto Setting { get; set; }

        public StudioXUserClockConfigDto Clock { get; set; }

        public StudioXUserTimingConfigDto Timing { get; set; }

        public StudioXUserSecurityConfigDto Security { get; set; }
    }
}