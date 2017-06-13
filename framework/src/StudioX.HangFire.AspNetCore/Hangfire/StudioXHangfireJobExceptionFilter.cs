using StudioX.BackgroundJobs;
using StudioX.Dependency;
using StudioX.Events.Bus;
using StudioX.Events.Bus.Exceptions;
using Hangfire.Common;
using Hangfire.Server;

namespace StudioX.Hangfire
{
    public class StudioXHangfireJobExceptionFilter : JobFilterAttribute, IServerFilter, ITransientDependency
    {
        public IEventBus EventBus { get; set; }

        public StudioXHangfireJobExceptionFilter()
        {
            EventBus = NullEventBus.Instance;
        }

        public void OnPerforming(PerformingContext filterContext)
        {
        }

        public void OnPerformed(PerformedContext filterContext)
        {
            if (filterContext.Exception != null)
            {
                EventBus.Trigger(
                    this,
                    new StudioXHandledExceptionData(
                        new BackgroundJobException(
                            "A background job execution is failed on Hangfire. See inner exception for details. Use JobObject to get Hangfire background job object.",
                            filterContext.Exception
                        )
                        {
                            JobObject = filterContext.BackgroundJob
                        }
                    )
                );
            }
        }
    }
}
