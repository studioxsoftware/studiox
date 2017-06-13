using System;
using System.Threading;
using StudioX.Dependency;
using StudioX.Quartz.Configuration;
using StudioX.TestBase;
using Quartz;
using Shouldly;
using Xunit;

namespace StudioX.Quartz.Tests
{
    public class QuartzTests : StudioXIntegratedTestBase<StudioXQuartzTestModule>
    {
        private readonly IStudioXQuartzConfiguration studioXQuartzConfiguration;
        private readonly IQuartzScheduleJobManager quartzScheduleJobManager;

        public QuartzTests()
        {
            quartzScheduleJobManager = LocalIocManager.Resolve<IQuartzScheduleJobManager>();
            studioXQuartzConfiguration = LocalIocManager.Resolve<IStudioXQuartzConfiguration>();

            ScheduleJobs();
        }

        private void ScheduleJobs()
        {
            quartzScheduleJobManager.ScheduleAsync<HelloJob>(
                job =>
                {
                    job.WithDescription("HelloJobDescription")
                       .WithIdentity("HelloJobKey");
                },
                trigger =>
                {
                    trigger.WithIdentity("HelloJobTrigger")
                           .WithDescription("HelloJobTriggerDescription")
                           .WithSimpleSchedule(schedule => schedule.WithRepeatCount(5).WithInterval(TimeSpan.FromSeconds(1)).Build())
                           .StartNow();
                });

            quartzScheduleJobManager.ScheduleAsync<GoodByeJob>(
                job =>
                {
                    job.WithDescription("GoodByeJobDescription")
                       .WithIdentity("GoodByeJobKey");
                },
                trigger =>
                {
                    trigger.WithIdentity("GoodByeJobTrigger")
                           .WithDescription("GoodByeJobTriggerDescription")
                           .WithSimpleSchedule(schedule => schedule.WithRepeatCount(5).WithInterval(TimeSpan.FromSeconds(1)).Build())
                           .StartNow();
                });
        }

        [Fact]
        public void QuartzSchedulerJobsShouldBeRegistered()
        {
            studioXQuartzConfiguration.Scheduler.ShouldNotBeNull();
            studioXQuartzConfiguration.Scheduler.IsStarted.ShouldBe(true);
            studioXQuartzConfiguration.Scheduler.CheckExists(JobKey.Create("HelloJobKey")).ShouldBe(true);
            studioXQuartzConfiguration.Scheduler.CheckExists(JobKey.Create("GoodByeJobKey")).ShouldBe(true);
        }

        [Fact]
        public void QuartzSchedulerJobsShouldBeExecutedWithSingletonDependency()
        {
            var helloDependency = LocalIocManager.Resolve<IHelloDependency>();
            var goodByeDependency = LocalIocManager.Resolve<IGoodByeDependency>();

            //Wait for execution!
            Thread.Sleep(TimeSpan.FromSeconds(5));

            helloDependency.ExecutionCount.ShouldBeGreaterThan(0);
            goodByeDependency.ExecutionCount.ShouldBeGreaterThan(0);
        }
    }

    [DisallowConcurrentExecution]
    public class HelloJob : JobBase, ITransientDependency
    {
        private readonly IHelloDependency helloDependency;

        public HelloJob(IHelloDependency helloDependency)
        {
            this.helloDependency = helloDependency;
        }

        public override void Execute(IJobExecutionContext context)
        {
            helloDependency.ExecutionCount++;
        }
    }

    [DisallowConcurrentExecution]
    public class GoodByeJob : JobBase, ITransientDependency
    {
        private readonly IGoodByeDependency goodByeDependency;

        public GoodByeJob(IGoodByeDependency goodByeDependency)
        {
            this.goodByeDependency = goodByeDependency;
        }

        public override void Execute(IJobExecutionContext context)
        {
            goodByeDependency.ExecutionCount++;
        }
    }

    public interface IHelloDependency
    {
        int ExecutionCount { get; set; }
    }

    public interface IGoodByeDependency
    {
        int ExecutionCount { get; set; }
    }

    public class HelloDependency : IHelloDependency, ISingletonDependency
    {
        public int ExecutionCount { get; set; }
    }

    public class GoodByeDependency : IGoodByeDependency, ISingletonDependency
    {
        public int ExecutionCount { get; set; }
    }
}
