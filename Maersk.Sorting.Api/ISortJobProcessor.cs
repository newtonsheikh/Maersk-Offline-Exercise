using System.Threading.Tasks;
using System.Collections.Generic;
using System;

namespace Maersk.Sorting.Api
{
    public interface ISortJobProcessor
    {
        Task<SortJob> Process(SortJob job);

        //Queue incoming jobs
        Task<IEnumerable<SortJob>> QueueJob(SortJob job);

        Task<SortJob> GetJob(Guid jobId);

        Task<SortJob[]> GetJobs();

        Task BackgroundProcess(SortJob pendingJob);
    } 
}