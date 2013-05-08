using System;
using System.Collections.Generic;

namespace Weave.ViewModels
{
    public class NewsList
    {
        public int FeedCount { get; set; }
        public int TotalNewsCount { get; set; }
        public int NewNewsCount { get; set; }
        public int NewsCount { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }
        public IEnumerable<NewsItem> News { get; set; }

        public int GetPageCount(int pageSize)
        {
            return (int)Math.Ceiling((double)TotalNewsCount / (double)pageSize);
        }
    }
}
