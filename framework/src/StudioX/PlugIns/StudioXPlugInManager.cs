using System;
using System.Linq;

namespace StudioX.PlugIns
{
    public class StudioXPlugInManager : IStudioXPlugInManager
    {
        public PlugInSourceList PlugInSources { get; }

#if NET46
        private static readonly object SyncObj = new object();
        private static bool isRegisteredToAssemblyResolve;
#endif

        public StudioXPlugInManager()
        {
            PlugInSources = new PlugInSourceList();

            //TODO: Try to use AssemblyLoadContext.Default!
#if NET46
            RegisterToAssemblyResolve(PlugInSources);
#endif
        }

#if NET46
        private static void RegisterToAssemblyResolve(PlugInSourceList plugInSources)
        {
            if (isRegisteredToAssemblyResolve)
            {
                return;
            }

            lock (SyncObj)
            {
                if (isRegisteredToAssemblyResolve)
                {
                    return;
                }

                isRegisteredToAssemblyResolve = true;

                AppDomain.CurrentDomain.AssemblyResolve += (sender, args) =>
                {
                    return plugInSources.GetAllAssemblies().FirstOrDefault(a => a.FullName == args.Name);
                };
            }
        }

#endif
    }
}