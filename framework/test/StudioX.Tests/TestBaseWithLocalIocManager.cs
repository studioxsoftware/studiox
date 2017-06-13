using System;
using StudioX.Dependency;

namespace StudioX.Tests
{
    public abstract class TestBaseWithLocalIocManager : IDisposable
    {
        protected IIocManager LocalIocManager;

        protected TestBaseWithLocalIocManager()
        {
            LocalIocManager = new IocManager();
        }

        public virtual void Dispose()
        {
            LocalIocManager.Dispose();
        }
    }
}