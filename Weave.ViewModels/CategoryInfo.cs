using System.Collections.Generic;

namespace Weave.ViewModels
{
    public class CategoryInfo
    {
        public string Category { get; set; }
        public int TotalFeedCount { get; set; }
        public List<Feed> Feeds { get; set; }
        public int NewArticleCount { get; set; }
        public int UnreadArticleCount { get; set; }
        public int TotalArticleCount { get; set; }
    }
}
