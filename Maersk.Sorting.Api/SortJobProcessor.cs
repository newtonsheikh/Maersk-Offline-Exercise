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

        /// <summary>
        /// Query the Dictionay for a job
        /// </summary>
        /// <param name="jobId">Guid of the job</param>
        /// <returns>Job</returns>
        public async Task<SortJob> GetJob(Guid jobId)
        {
            object j = new object();

            await Task.Run(() =>{
                j = ApplicationCache.AppCache[jobId];
            });

            var job = (SortJob)j;

            return new SortJob(
                id: job.Id,
                status: job.Status,
                duration: job.Duration,
                input: job.Input,
                output: job.Output);
        }

        /// <summary>
        /// Query the dictionary for all the jobs in the queue
        /// </summary>
        /// <returns>List of jobs</returns>
        public async Task<SortJob[]> GetJobs(){

            List<SortJob> allJobs = new List<SortJob>();

            await Task.Run(() =>{
                 foreach(var item in ApplicationCache.AppCache.Keys)
                {
                    allJobs.Add(ApplicationCache.AppCache[item]);
                }
            });
           
            return allJobs.ToArray();
        }

        public async Task BackgroundProcess(SortJob pendingJob){

            object j = new object();

            var completedJob = await Process(pendingJob);

            await Task.Run(() =>{
                j = ApplicationCache.AppCache[completedJob.Id];
            });

             var job = (SortJob)j;

              var updatedJob = new SortJob(
                id: Guid.NewGuid(),
                status: SortJobStatus.Completed,
                duration: completedJob.Duration,
                input: job.Input,
                output: completedJob.Output);

            await Task.Run(() =>{
                ApplicationCache.AppCache[job.Id] = updatedJob; //Update the job in inmemory cache dictionary
            });
        }
    }
}
