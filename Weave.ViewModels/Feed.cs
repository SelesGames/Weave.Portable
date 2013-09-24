using System;

namespace Weave.ViewModels
{
    public class Feed
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Uri { get; set; }
        public string IconUrl { get; set; }
        public string Category { get; set; }
        public ArticleViewingType ArticleViewingType { get; set; }
        public string TeaserImageUrl { get; set; }

        public int NewArticleCount { get; set; }
        public int UnreadArticleCount { get; set; }
        public int TotalArticleCount { get; set; }
    }
}
