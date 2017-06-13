using System;
using System.Threading;
using StudioX.Dependency;

namespace StudioX.Threading.Timers
{
    /// <summary>
    /// A roboust timer implementation that ensures no overlapping occurs. It waits exactly specified <see cref="Period"/> between ticks.
    /// </summary>
    //TODO: Extract interface or make all members virtual to make testing easier.
    public class StudioXTimer : RunnableBase, ITransientDependency
    {
        /// <summary>
        /// This event is raised periodically according to Period of Timer.
        /// </summary>
        public event EventHandler Elapsed;

        /// <summary>
        /// Task period of timer (as milliseconds).
        /// </summary>
        public int Period { get; set; }

        /// <summary>
        /// Indicates whether timer raises Elapsed event on Start method of Timer for once.
        /// Default: False.
        /// </summary>
        public bool RunOnStart { get; set; }

        /// <summary>
        /// This timer is used to perfom the task at spesified intervals.
        /// </summary>
        private readonly Timer taskTimer;

        /// <summary>
        /// Indicates that whether timer is running or stopped.
        /// </summary>
        private volatile bool running;

        /// <summary>
        /// Indicates that whether performing the task or taskTimer is in sleep mode.
        /// This field is used to wait executing tasks when stopping Timer.
        /// </summary>
        private volatile bool performingTasks;

        /// <summary>
        /// Creates a new Timer.
        /// </summary>
        public StudioXTimer()
        {
            taskTimer = new Timer(TimerCallBack, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// Creates a new Timer.
        /// </summary>
        /// <param name="period">Task period of timer (as milliseconds)</param>
        /// <param name="runOnStart">Indicates whether timer raises Elapsed event on Start method of Timer for once</param>
        public StudioXTimer(int period, bool runOnStart = false)
            : this()
        {
            Period = period;
            RunOnStart = runOnStart;
        }

        /// <summary>
        /// Starts the timer.
        /// </summary>
        public override void Start()
        {
            if (Period <= 0)
            {
                throw new StudioXException("Period should be set before starting the timer!");
            }

            base.Start();

            running = true;
            taskTimer.Change(RunOnStart ? 0 : Period, Timeout.Infinite);
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public override void Stop()
        {
            lock (taskTimer)
            {
                running = false;
                taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }

            base.Stop();
        }

        /// <summary>
        /// Waits the service to stop.
        /// </summary>
        public override void WaitToStop()
        {
            lock (taskTimer)
            {
                while (performingTasks)
                {
                    Monitor.Wait(taskTimer);
                }
            }

            base.WaitToStop();
        }

        /// <summary>
        /// This method is called by taskTimer.
        /// </summary>
        /// <param name="state">Not used argument</param>
        private void TimerCallBack(object state)
        {
            lock (taskTimer)
            {
                if (!running || performingTasks)
                {
                    return;
                }

                taskTimer.Change(Timeout.Infinite, Timeout.Infinite);
                performingTasks = true;
            }

            try
            {
                if (Elapsed != null)
                {
                    Elapsed(this, new EventArgs());
                }
            }
            catch
            {

            }
            finally
            {
                lock (taskTimer)
                {
                    performingTasks = false;
                    if (running)
                    {
                        taskTimer.Change(Period, Timeout.Infinite);
                    }

                    Monitor.Pulse(taskTimer);
                }
            }
        }
    }
}