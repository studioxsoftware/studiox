namespace StudioX.Threading
{
    /// <summary>
    /// Base implementation of <see cref="IRunnable"/>.
    /// </summary>
    public abstract class RunnableBase : IRunnable
    {
        /// <summary>
        /// A boolean value to control the running.
        /// </summary>
        public bool IsRunning => isRunning;

        private volatile bool isRunning;

        public virtual void Start()
        {
            isRunning = true;
        }

        public virtual void Stop()
        {
            isRunning = false;
        }

        public virtual void WaitToStop()
        {

        }
    }
}