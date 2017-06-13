using System;
using System.Collections.Generic;

namespace StudioX.Modules
{
    public interface IStudioXModuleManager
    {
        StudioXModuleInfo StartupModule { get; }

        IReadOnlyList<StudioXModuleInfo> Modules { get; }

        void Initialize(Type startupModule);

        void StartModules();

        void ShutdownModules();
    }
}