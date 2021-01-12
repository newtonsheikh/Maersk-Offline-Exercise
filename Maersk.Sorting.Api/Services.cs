using System;
using System.Collections.Generic;

namespace Maersk.Sorting.Api
{
    public static class ApplicationCache
    {
        private static Dictionary<Guid, SortJob>? cache;

        private static object cacheLock = new object();
        public static Dictionary<Guid, SortJob> AppCache
        {
            get
            {
                lock (cacheLock)
                {
                    if (cache == null)
                    {
                        cache = new Dictionary<Guid, SortJob>();
                    }
                    return cache;
                }
            }
        }
    }
}
