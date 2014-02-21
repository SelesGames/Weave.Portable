using System;

namespace Weave.ViewModels
{
    public class MobilizedArticle
    {
        public string Title { get; set; }
        public string Link { get; set; }
        public string Source { get; set; }
        public string HeroImageUrl { get; set; }
        public string ContentHtml { get; set; }
        public string FullDate { get; set; }
        public string CombinedPublicationAndDate { get; set; }
        public string Author { get; set; }

        public bool HasImage { get { return string.IsNullOrWhiteSpace(HeroImageUrl); } }
    }
}
