using System;
using StudioX.Dependency;
using StudioX.MqMessages;
using StudioX.Threading.BackgroundWorkers;
using StudioX.Threading.Timers;
using Sample.MqMessages;

namespace Sample.BackgroundWorks
{
    public class TestWorker : PeriodicBackgroundWorkerBase, ISingletonDependency
    {
        private readonly IMqMessagePublisher publisher;

        public TestWorker(StudioXTimer timer, IMqMessagePublisher publisher)
            : base(timer)
        {
            this.publisher = publisher;
            Timer.Period = 3000;//3 seconds
            Timer.RunOnStart = true;
        }

        protected override void DoWork()
        {
            Logger.Info($"TestWork Done! Time:{DateTime.Now}");
            publisher.Publish(new TestMqMessage
            {
                Name = "TestWork",
                Value = "BlaBlaBlaBlaBlaBla",
                Time = DateTime.Now
            });
        }
    }
}
