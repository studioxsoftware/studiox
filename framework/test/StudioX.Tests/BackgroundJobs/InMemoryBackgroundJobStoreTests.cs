using System.Threading.Tasks;
using StudioX.BackgroundJobs;
using Shouldly;
using Xunit;

namespace StudioX.Tests.BackgroundJobs
{
    public class InMemoryBackgroundJobStoreTests
    {
        private readonly InMemoryBackgroundJobStore store;

        public InMemoryBackgroundJobStoreTests()
        {
            store = new InMemoryBackgroundJobStore();
        }

        [Fact]
        public async Task TestAll()
        {
            var jobInfo = new BackgroundJobInfo
            {
                JobType = "TestType",
                JobArgs = "{}"
            };
            
            await store.InsertAsync(jobInfo);
            (await store.GetWaitingJobsAsync(1000)).Count.ShouldBe(1);
            await store.DeleteAsync(jobInfo);
            (await store.GetWaitingJobsAsync(1000)).Count.ShouldBe(0);
        }
    }
}
