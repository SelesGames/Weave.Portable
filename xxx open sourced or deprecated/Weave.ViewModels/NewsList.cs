using System;
using System.Collections.Generic;

namespace Weave.ViewModels
{
    public class NewsList
    {
        public int FeedCount { get; set; }
        public int NewArticleCount { get; set; }
        public int UnreadArticleCount { get; set; }
        public int TotalArticleCount { get; set; }

        public int IncludedArticleCount { get; set; }
        public int Skip { get; set; }
        public int Take { get; set; }

        public IEnumerable<NewsItem> News { get; set; }

        public int GetPageCount(int pageSize)
        {
            return (int)Math.Ceiling((double)TotalArticleCount / (double)pageSize);
        }
    }
}