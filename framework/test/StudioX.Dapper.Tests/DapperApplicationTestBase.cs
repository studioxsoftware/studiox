using System.Collections.Generic;
using System.Data.Common;
using System.Data.SQLite;
using System.IO;
using System.Reflection;
using Castle.MicroKernel.Registration;
using Dapper;
using StudioX.Configuration.Startup;
using StudioX.TestBase;

namespace StudioX.Dapper.Tests
{
    public abstract class DapperApplicationTestBase : StudioXIntegratedTestBase<StudioXDapperTestModule>
    {
        protected DapperApplicationTestBase()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;

            Resolve<IStudioXStartupConfiguration>().DefaultNameOrConnectionString = "Data Source=:memory:";

            StudioXSession.UserId = 1;
            StudioXSession.TenantId = 1;
        }

        protected override void PreInitialize()
        {
            base.PreInitialize();

            LocalIocManager.IocContainer.Register(
                Component.For<DbConnection>()
                    .UsingFactoryMethod(() =>
                    {
                        var connection =
                            new SQLiteConnection(Resolve<IStudioXStartupConfiguration>().DefaultNameOrConnectionString);
                        connection.Open();
                        var files = new List<string>
                        {
                            ReadScriptFile("CreateInitialTables")
                        };

                        foreach (var setupFile in files)
                        {
                            connection.Execute(setupFile);
                        }

                        return connection;
                    })
                    .LifestyleSingleton()
            );
        }

        private string ReadScriptFile(string name)
        {
            var fileName = GetType().Namespace + ".Scripts" + "." + name + ".sql";
            using (var resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
            {
                if (resource != null)
                {
                    using (var sr = new StreamReader(resource))
                    {
                        return sr.ReadToEnd();
                    }
                }
            }

            return string.Empty;
        }
    }
}