using System.Collections.Generic;

namespace Weave.ViewModels
{
    public class FeedsInfoList
    {
        public int TotalFeedCount { get; set; }
        public List<CategoryInfo> Categories { get; set; }
        public List<Feed> Feeds { get; set; }
        public int NewArticleCount { get; set; }
        public int UnreadArticleCount { get; set; }
        public int TotalArticleCount { get; set; }
    }
}
