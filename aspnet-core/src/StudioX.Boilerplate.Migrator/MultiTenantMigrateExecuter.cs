using System;
using System.Collections.Generic;
using StudioX.Data;
using StudioX.Dependency;
using StudioX.Domain.Repositories;
using StudioX.Domain.Uow;
using StudioX.Extensions;
using StudioX.MultiTenancy;
using StudioX.Runtime.Security;
using StudioX.Boilerplate.EntityFrameworkCore;
using StudioX.Boilerplate.EntityFrameworkCore.Seed;
using StudioX.Boilerplate.Migrator;
using StudioX.Boilerplate.MultiTenancy;

namespace StudioX.Boilerplate.Migrator
{
    public class MultiTenantMigrateExecuter : ITransientDependency
    {
        public Log Log { get; private set; }

        private readonly StudioXZeroDbMigrator migrator;
        private readonly IRepository<Tenant> tenantRepository;
        private readonly IDbPerTenantConnectionStringResolver connectionStringResolver;

        public MultiTenantMigrateExecuter(
            StudioXZeroDbMigrator migrator,
            IRepository<Tenant> tenantRepository,
            Log log,
            IDbPerTenantConnectionStringResolver connectionStringResolver)
        {
            Log = log;

            this.migrator = migrator;
            this.tenantRepository = tenantRepository;
            this.connectionStringResolver = connectionStringResolver;
        }

        public void Run(bool skipConnVerification)
        {
            var hostConnStr = connectionStringResolver.GetNameOrConnectionString(new ConnectionStringResolveArgs(MultiTenancySides.Host));
            if (hostConnStr.IsNullOrWhiteSpace())
            {
                Log.Write("Configuration file should contain a connection string named 'Default'");
                return;
            }

            Log.Write("Host database: " + ConnectionStringHelper.GetConnectionString(hostConnStr));
            if (!skipConnVerification)
            {
                Log.Write("Continue to migration for this host database and all tenants..? (Y/N): ");
                var command = Console.ReadLine();
                if (!command.IsIn("Y", "y"))
                {
                    Log.Write("Migration canceled.");
                    return;
                }
            }

            Log.Write("HOST database migration started...");

            try
            {
                migrator.CreateOrMigrateForHost(SeedHelper.SeedHostDb);
            }
            catch (Exception ex)
            {
                Log.Write("An error occured during migration of host database:");
                Log.Write(ex.ToString());
                Log.Write("Canceled migrations.");
                return;
            }

            Log.Write("HOST database migration completed.");
            Log.Write("--------------------------------------------------------");

            var migratedDatabases = new HashSet<string>();
            var tenants = tenantRepository.GetAllList(t => t.ConnectionString != null && t.ConnectionString != "");
            for (int i = 0; i < tenants.Count; i++)
            {
                var tenant = tenants[i];
                Log.Write(string.Format("Tenant database migration started... ({0} / {1})", (i + 1), tenants.Count));
                Log.Write("Name              : " + tenant.Name);
                Log.Write("TenancyName       : " + tenant.TenancyName);
                Log.Write("Tenant Id         : " + tenant.Id);
                Log.Write("Connection string : " + SimpleStringCipher.Instance.Decrypt(tenant.ConnectionString));

                if (!migratedDatabases.Contains(tenant.ConnectionString))
                {
                    try
                    {
                        migrator.CreateOrMigrateForTenant(tenant);
                    }
                    catch (Exception ex)
                    {
                        Log.Write("An error occured during migration of tenant database:");
                        Log.Write(ex.ToString());
                        Log.Write("Skipped this tenant and will continue for others...");
                    }

                    migratedDatabases.Add(tenant.ConnectionString);
                }
                else
                {
                    Log.Write("This database has already migrated before (you have more than one tenant in same database). Skipping it....");
                }

                Log.Write(string.Format("Tenant database migration completed. ({0} / {1})", (i + 1), tenants.Count));
                Log.Write("--------------------------------------------------------");
            }

            Log.Write("All databases have been migrated.");
        }
    }
}