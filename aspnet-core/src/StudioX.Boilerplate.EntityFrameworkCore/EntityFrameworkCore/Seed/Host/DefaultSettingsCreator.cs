using System.Linq;
using StudioX.Configuration;
using StudioX.Localization;
using StudioX.Net.Mail;

namespace StudioX.Boilerplate.EntityFrameworkCore.Seed.Host
{
    public class DefaultSettingsCreator
    {
        private readonly BoilerplateDbContext context;

        public DefaultSettingsCreator(BoilerplateDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            //Emailing
            AddSettingIfNotExists(EmailSettingNames.DefaultFromAddress, "admin@mydomain.com");
            AddSettingIfNotExists(EmailSettingNames.DefaultFromDisplayName, "mydomain.com mailer");

            //Languages
            AddSettingIfNotExists(LocalizationSettingNames.DefaultLanguage, "en");
        }

        private void AddSettingIfNotExists(string name, string value, int? tenantId = null)
        {
            if (context.Settings.Any(s => s.Name == name && s.TenantId == tenantId && s.UserId == null))
            {
                return;
            }

            context.Settings.Add(new Setting(tenantId, null, name, value));
            context.SaveChanges();
        }
    }
}