using Hangfire;
using Hangfire.Storage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Spotcc
{
    public static class HangfireExtensions
    {
        public static void PurgeProcessingJobs(this IMonitoringApi monitor)
        {
            foreach (var job in monitor.ProcessingJobs(0, (int)monitor.ProcessingCount()))
            {
                BackgroundJob.Delete(job.Key);
            }
        }
    }
}
