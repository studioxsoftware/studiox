using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using StudioX.Timing;

namespace StudioX.BackgroundJobs
{
    /// <summary>
    ///     In memory implementation of <see cref="IBackgroundJobStore" />.
    ///     It's used if <see cref="IBackgroundJobStore" /> is not implemented by actual persistent store
    ///     and job execution is enabled (<see cref="IBackgroundJobConfiguration.IsJobExecutionEnabled" />) for the
    ///     application.
    /// </summary>
    public class InMemoryBackgroundJobStore : IBackgroundJobStore
    {
        private readonly Dictionary<long, BackgroundJobInfo> jobs;
        private long lastId;

        /// <summary>
        ///     Initializes a new instance of the <see cref="InMemoryBackgroundJobStore" /> class.
        /// </summary>
        public InMemoryBackgroundJobStore()
        {
            jobs = new Dictionary<long, BackgroundJobInfo>();
        }

        public Task InsertAsync(BackgroundJobInfo jobInfo)
        {
            jobInfo.Id = Interlocked.Increment(ref lastId);
            jobs[jobInfo.Id] = jobInfo;

            return Task.FromResult(0);
        }

        public Task<List<BackgroundJobInfo>> GetWaitingJobsAsync(int maxResultCount)
        {
            var waitingJobs = jobs.Values
                .Where(t => !t.IsAbandoned && t.NextTryTime <= Clock.Now)
                .OrderByDescending(t => t.Priority)
                .ThenBy(t => t.TryCount)
                .ThenBy(t => t.NextTryTime)
                .Take(maxResultCount)
                .ToList();

            return Task.FromResult(waitingJobs);
        }

        public Task DeleteAsync(BackgroundJobInfo jobInfo)
        {
            if (!jobs.ContainsKey(jobInfo.Id))
            {
                return Task.FromResult(0);
            }

            jobs.Remove(jobInfo.Id);

            return Task.FromResult(0);
        }

        public Task UpdateAsync(BackgroundJobInfo jobInfo)
        {
            if (jobInfo.IsAbandoned)
            {
                return DeleteAsync(jobInfo);
            }

            return Task.FromResult(0);
        }
    }
}