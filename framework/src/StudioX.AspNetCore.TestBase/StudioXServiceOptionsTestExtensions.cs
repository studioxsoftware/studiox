using StudioX.Dependency;
using StudioX.Runtime.Session;
using StudioX.TestBase.Runtime.Session;

namespace StudioX.AspNetCore.TestBase
{
    public static class StudioXServiceOptionsTestExtensions
    {
        public static void SetupTest(this StudioXServiceOptions options)
        {
            options.IocManager = new IocManager();
            options.IocManager.RegisterIfNot<IStudioXSession, TestStudioXSession>();
        }
    }
}