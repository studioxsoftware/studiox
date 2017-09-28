using System.Collections.Generic;
using System.Linq;
using StudioX.Localization;

namespace StudioX.ZeroCore.SampleApp.EntityFramework.Seed.Host
{
    public class DefaultLanguagesCreator
    {
        public static List<ApplicationLanguage> InitialLanguages => GetInitialLanguages();

        private readonly SampleAppDbContext context;

        private static List<ApplicationLanguage> GetInitialLanguages()
        {
            return new List<ApplicationLanguage>
            {
                new ApplicationLanguage(null, "en", "English", "famfamfam-flags gb"),
                new ApplicationLanguage(null, "tr", "Türkçe", "famfamfam-flags tr")
            };
        }

        public DefaultLanguagesCreator(SampleAppDbContext context)
        {
            this.context = context;
        }

        public void Create()
        {
            CreateLanguages();
        }

        private void CreateLanguages()
        {
            foreach (var language in InitialLanguages)
            {
                AddLanguageIfNotExists(language);
            }
        }

        private void AddLanguageIfNotExists(ApplicationLanguage language)
        {
            if (context.Languages.Any(l => l.TenantId == language.TenantId && l.Name == language.Name))
            {
                return;
            }

            context.Languages.Add(language);

            context.SaveChanges();
        }
    }
}