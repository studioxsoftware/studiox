using System;
using System.Collections.Generic;
using StudioX.Dependency;

namespace StudioX.Threading.BackgroundWorkers
{
    /// <summary>
    /// Implements <see cref="IBackgroundWorkerManager"/>.
    /// </summary>
    public class BackgroundWorkerManager : RunnableBase, IBackgroundWorkerManager, ISingletonDependency, IDisposable
    {
        private readonly IIocResolver iocResolver;
        private readonly List<IBackgroundWorker> backgroundJobs;

        /// <summary>
        /// Initializes a new instance of the <see cref="BackgroundWorkerManager"/> class.
        /// </summary>
        public BackgroundWorkerManager(IIocResolver iocResolver)
        {
            this.iocResolver = iocResolver;
            backgroundJobs = new List<IBackgroundWorker>();
        }

        public override void Start()
        {
            base.Start();

            backgroundJobs.ForEach(job => job.Start());
        }

        public override void Stop()
        {
            backgroundJobs.ForEach(job => job.Stop());

            base.Stop();
        }

        public override void WaitToStop()
        {
            backgroundJobs.ForEach(job => job.WaitToStop());

            base.WaitToStop();
        }

        public void Add(IBackgroundWorker worker)
        {
            backgroundJobs.Add(worker);

            if (IsRunning)
            {
                worker.Start();
            }
        }

        private bool isDisposed;

        public void Dispose()
        {
            if (isDisposed)
            {
                return;
            }

            isDisposed = true;

            backgroundJobs.ForEach(iocResolver.Release);
            backgroundJobs.Clear();
        }
    }
}
