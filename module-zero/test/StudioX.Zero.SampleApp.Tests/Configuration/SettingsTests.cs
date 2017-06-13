using System.Linq;
using StudioX.Configuration;
using StudioX.Domain.Repositories;
using Shouldly;
using Xunit;

namespace StudioX.Zero.SampleApp.Tests.Configuration
{
    public class SettingsTests : SampleAppTestBase
    {
        private readonly ISettingManager settingManager;
        private readonly IRepository<Setting, long> settingRepository;

        public SettingsTests()
        {
            settingManager = LocalIocManager.Resolve<ISettingManager>();
            settingRepository = LocalIocManager.Resolve<IRepository<Setting, long>>();
        }

        [Fact]
        public void ShouldGetAllSettings()
        {
            var allValues = settingManager.GetAllSettingValues();
            allValues.Any(v => v.Name == "Setting1").ShouldBe(true);
            allValues.Any(v => v.Name == "Setting2").ShouldBe(true);
        }

        [Fact]
        public void ShouldGetDefaultValueForSetting1()
        {
            settingManager.GetSettingValue<int>("Setting1").ShouldBe(1);
        }

        [Fact]
        public void ShouldChangeSetting2()
        {
            //Check default value
            settingManager.GetSettingValue("Setting2").ShouldBe("A");

            //Change it to a custom value
            settingManager.ChangeSettingForApplication("Setting2", "B");

            //Check value from manager
            settingManager.GetSettingValue("Setting2").ShouldBe("B");

            //Check value from repository
            var setting2 = settingRepository.FirstOrDefault(s => s.TenantId == null && s.UserId == null && s.Name == "Setting2");
            setting2.ShouldNotBe(null);
            setting2.Value.ShouldBe("B");

            //Set it again to default value
            settingManager.ChangeSettingForApplication("Setting2", "A");

            //Setting to default value cause the setting will be deleted from database
            settingRepository.FirstOrDefault(s => s.TenantId == null && s.UserId == null && s.Name == "Setting2").ShouldBe(null);

            //Check value again from manager
            settingManager.GetSettingValue("Setting2").ShouldBe("A");
        }
    }
}
