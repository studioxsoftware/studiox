using System;
using System.Collections.Generic;
using StudioX.Dependency;
using Hangfire;

namespace StudioX.Hangfire
{
    public class HangfireIocJobActivator : JobActivator
    {
        private readonly IIocResolver iocResolver;

        public HangfireIocJobActivator(IIocResolver iocResolver)
        {
            if (iocResolver == null)
            {
                throw new ArgumentNullException(nameof(iocResolver));
            }

            this.iocResolver = iocResolver;
        }

        public override object ActivateJob(Type jobType)
        {
            return iocResolver.Resolve(jobType);
        }

        public override JobActivatorScope BeginScope(JobActivatorContext context)
        {
            return new HangfireIocJobActivatorScope(this, iocResolver);
        }

        class HangfireIocJobActivatorScope : JobActivatorScope
        {
            private readonly JobActivator activator;
            private readonly IIocResolver iocResolver;

            private readonly List<object> resolvedObjects;

            public HangfireIocJobActivatorScope(JobActivator activator, IIocResolver iocResolver)
            {
                this.activator = activator;
                this.iocResolver = iocResolver;
                resolvedObjects = new List<object>();
            }

            public override object Resolve(Type type)
            {
                var instance = activator.ActivateJob(type);
                resolvedObjects.Add(instance);
                return instance;
            }

            public override void DisposeScope()
            {
                resolvedObjects.ForEach(iocResolver.Release);
            }
        }
    }
}