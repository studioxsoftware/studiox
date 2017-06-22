using System;
using System.Data.Common;
using System.Data.Entity.Infrastructure.Interception;
using System.Text.RegularExpressions;
using StudioX.Dependency;
using StudioX.Runtime;

namespace StudioX.EntityFramework.Interceptors
{
    public class WithNoLockInterceptor : DbCommandInterceptor, ITransientDependency
    {
        private const string InterceptionContextKey = "StudioX.EntityFramework.Interceptors.WithNolockInterceptor";

        private static readonly Regex TableAliasRegex = new Regex(
            @"(?<tableAlias>AS \[Extent\d+\](?! WITH \(NOLOCK\)))", RegexOptions.Multiline | RegexOptions.IgnoreCase);

        private readonly IAmbientScopeProvider<InterceptionContext> interceptionScopeProvider;

        public WithNoLockInterceptor(IAmbientScopeProvider<InterceptionContext> interceptionScopeProvider)
        {
            this.interceptionScopeProvider = interceptionScopeProvider;
        }

        public InterceptionContext NolockingContext => interceptionScopeProvider.GetValue(InterceptionContextKey);

        public override void ScalarExecuting(DbCommand command, DbCommandInterceptionContext<object> interceptionContext)
        {
            if (NolockingContext?.UseNolocking ?? false)
            {
                command.CommandText = TableAliasRegex.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");
                NolockingContext.CommandText = command.CommandText;
            }
        }

        public override void ReaderExecuting(DbCommand command,
            DbCommandInterceptionContext<DbDataReader> interceptionContext)
        {
            if (NolockingContext?.UseNolocking ?? false)
            {
                command.CommandText = TableAliasRegex.Replace(command.CommandText, "${tableAlias} WITH (NOLOCK)");
                NolockingContext.CommandText = command.CommandText;
            }
        }

        public IDisposable UseNolocking()
        {
            return interceptionScopeProvider.BeginScope(InterceptionContextKey,
                new InterceptionContext(string.Empty, true));
        }

        public class InterceptionContext
        {
            public InterceptionContext(string commandText, bool useNolocking)
            {
                CommandText = commandText;
                UseNolocking = useNolocking;
            }

            public string CommandText { get; set; }

            public bool UseNolocking { get; set; }
        }
    }
}