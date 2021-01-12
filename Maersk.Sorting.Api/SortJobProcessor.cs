using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;

namespace Maersk.Sorting.Api
{
    public class SortJobProcessor : ISortJobProcessor
    {
        private readonly ILogger<SortJobProcessor> _logger;

        public SortJobProcessor(ILogger<SortJobProcessor> logger)
        {
            _logger = logger;
        }


        public async Task<SortJob> Process(SortJob job)
        {
            _logger.LogInformation("Processing job with ID '{JobId}'.", job.Id);

            var stopwatch = Stopwatch.StartNew();

            var output = job.Input.OrderBy(n => n).ToArray();
            await Task.Delay(5000); // NOTE: This is just to simulate a more expensive operation

            var duration = stopwatch.Elapsed;

            _logger.LogInformation("Completed processing job with ID '{JobId}'. Duration: '{Duration}'.", job.Id, duration);

            return new SortJob(
                id: job.Id,
                status: SortJobStatus.Completed,
                duration: duration,
                input: job.Input,
                output: output);
        }

        /// <summary>
        /// Enqueues incoming jobs to be sorted
        /// </summary>
        /// <param name="job">incoming job</param>
        /// <returns>List of all the jobs in the queue</returns>
        
        async Task<IEnumerable<SortJob>> ISortJobProcessor.QueueJob(SortJob job)
        {
            List<SortJob> sortJob = new List<SortJob>();

            await Task.Run(() =>{
                ApplicationCache.AppCache.Add(job.Id, job); //Insert the job in inmemory cache
            });

            foreach(var item in ApplicationCache.AppCache.Keys)
            {
                sortJob.Add(ApplicationCache.AppCache[item]);
            }
            
            return sortJob.ToList();
        }
    }
}
