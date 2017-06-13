using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.SqlClient;
using System.IO;
using System.Reflection;

using StudioX.Configuration.Startup;
using StudioX.TestBase;

using Castle.MicroKernel.Registration;

using Dapper;

namespace StudioX.Dapper.Tests
{
    public abstract class DapperApplicationTestBase : StudioXIntegratedTestBase<StudioXDapperTestModule>
    {
        private readonly string connectionString;

        protected DapperApplicationTestBase()
        {
            Resolve<IMultiTenancyConfig>().IsEnabled = true;
            string executable = AppDomain.CurrentDomain.BaseDirectory;
            string path = Path.GetDirectoryName(Path.GetDirectoryName(Path.GetDirectoryName(executable))) + @"\Db\StudioXDapperTest.mdf";
            connectionString = $@"Data Source=(localdb)\MsSqlLocalDb;Integrated Security=SSPI;AttachDBFilename={path}";

            Resolve<IStudioXStartupConfiguration>().DefaultNameOrConnectionString = connectionString;

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
                             var connection = new SqlConnection(connectionString);

                             var files = new List<string>
                             {
                                 ReadScriptFile("CreateInitialTables")
                             };

                             foreach (string setupFile in files)
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
            string fileName = GetType().Namespace + ".Scripts" + "." + name + ".sql";
            using (Stream resource = Assembly.GetExecutingAssembly().GetManifestResourceStream(fileName))
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
