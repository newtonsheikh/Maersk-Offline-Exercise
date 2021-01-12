using System.Threading.Tasks;
using System.Collections.Generic;

namespace Maersk.Sorting.Api
{
    public interface ISortJobProcessor
    {
        Task<SortJob> Process(SortJob job);

        //Queue incoming jobs
        Task<IEnumerable<SortJob>> QueueJob(SortJob job);
    } 
}