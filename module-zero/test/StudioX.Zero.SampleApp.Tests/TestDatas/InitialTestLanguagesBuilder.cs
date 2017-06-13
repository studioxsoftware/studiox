using System.Linq;
using StudioX.Localization;
using StudioX.Zero.SampleApp.EntityFramework;
using StudioX.Zero.SampleApp.MultiTenancy;

namespace StudioX.Zero.SampleApp.Tests.TestDatas
{
    public class InitialTestLanguagesBuilder
    {
        private readonly AppDbContext context;

        public InitialTestLanguagesBuilder(AppDbContext context)
        {
            this.context = context;
        }

        public void Build()
        {
            InitializeLanguagesOnDatabase();
        }

        private void InitializeLanguagesOnDatabase()
        {
            var defaultTenant = context.Tenants.Single(t => t.TenancyName == Tenant.DefaultTenantName);
            
            //Host languages
            context.Languages.Add(new ApplicationLanguage { Name = "en", DisplayName = "English" });
            context.Languages.Add(new ApplicationLanguage { Name = "tr", DisplayName = "Türkçe" });
            context.Languages.Add(new ApplicationLanguage { Name = "de", DisplayName = "German" });

            //Default tenant languages
            context.Languages.Add(new ApplicationLanguage { Name = "zh-CN", DisplayName = "简体中文", TenantId = defaultTenant.Id });
        }
    }
}