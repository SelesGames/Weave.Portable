using System.Collections.Generic;

namespace Weave.ViewModels
{
    public class LiveTileNewsList
    {
        public int FeedCount { get; set; }
        public int NewNewsCount { get; set; }
        public int FeaturedNewsCount { get; set; }

        public List<NewsItem> News { get; set; }
    }
}
