using SelesGames.Common;
using System.Collections.Generic;
using System.Linq;
using Outgoing = Weave.UserFeedAggregator.DTOs.ServerOutgoing;

namespace Weave.Converters
{
    public class DTOsToViewModelsConverters : 
        IConverter<Outgoing.Feed, ViewModels.Feed>,
        IConverter<Outgoing.NewsList, ViewModels.NewsList>,
        IConverter<Outgoing.LiveTileNewsList, ViewModels.LiveTileNewsList>,
        IConverter<Outgoing.Image, ViewModels.Image>
    {
        public static readonly DTOsToViewModelsConverters Current = new DTOsToViewModelsConverters();

        public ViewModels.Feed Convert(Outgoing.Feed o)
        {
            return new ViewModels.Feed
            {
                Id = o.Id,
                Name = o.Name,
                Uri = o.Uri,
                Category = o.Category,
                ArticleViewingType = (ViewModels.ArticleViewingType)o.ArticleViewingType,
                TeaserImageUrl = o.TeaserImageUrl,
            };
        }

        public ViewModels.NewsList Convert(Outgoing.NewsList o)
        {
            return new ViewModels.NewsList
            {
                FeedCount = o.FeedCount,
                TotalNewsCount = o.TotalNewsCount,
                PageNewsCount = o.PageNewsCount,
                Skip = o.Skip,
                Take = o.Take,
                //News = o.News == null ? null : GetJoinedNews(o.Feeds, o.News).ToList(),
                News = o.News == null ? null : GetJoinedNews(o.Feeds.Select(Convert).ToList(), o.News.Select(Convert).ToList()).ToList(),
            };
        }

        public ViewModels.Image Convert(Outgoing.Image o)
        {
            return new ViewModels.Image
            {
                BaseImageUrl = o.BaseImageUrl,
                Width = o.Width,
                Height = o.Height,
                OriginalUrl = o.OriginalUrl,
                SupportedFormats = o.SupportedFormats,
            };
        }

        //ViewModels.NewsItem Convert(Outgoing.NewsItem n, Outgoing.Feed f)
        //{
        //    return new ViewModels.NewsItem
        //    {
        //        Id = n.Id,
        //        Feed = Convert(f),
        //        Title = n.Title,
        //        Link = n.Link,
        //        UtcPublishDateTime = n.UtcPublishDateTime,
        //        ImageUrl = n.ImageUrl,
        //        YoutubeId = n.YoutubeId,
        //        VideoUri = n.VideoUri,
        //        PodcastUri = n.PodcastUri,
        //        ZuneAppId = n.ZuneAppId,
        //        OriginalDownloadDateTime = n.OriginalDownloadDateTime,
        //        Image =  n.Image == null ? null : Convert(n.Image), 
        //        HasBeenViewed = n.HasBeenViewed,
        //        IsFavorite = n.IsFavorite,
        //    };
        //}
        
        ViewModels.NewsItem Convert(ViewModels.NewsItem n, ViewModels.Feed f)
        {
            n.Feed = f;
            return n;
        }


        //IEnumerable<ViewModels.NewsItem> GetJoinedNews(IEnumerable<Outgoing.Feed> feeds, IEnumerable<Outgoing.NewsItem> news)
        //{
        //    return from n in news                  
        //           join f in feeds on n.FeedId equals f.Id         
        //           select Convert(n, f);
        //}

        IEnumerable<ViewModels.NewsItem> GetJoinedNews(IEnumerable<ViewModels.Feed> feeds, IEnumerable<ViewModels.NewsItem> news)
        {
            return from n in news                  
                   join f in feeds on n.Feed.Id equals f.Id         
                   select Convert(n, f);
        }

        ViewModels.NewsItem Convert(Outgoing.NewsItem o)
        {
            return new ViewModels.NewsItem
            {
                Id = o.Id,
                Feed = new ViewModels.Feed { Id = o.FeedId },
                Title = o.Title,
                Link = o.Link,
                UtcPublishDateTime = o.UtcPublishDateTime,
                ImageUrl = o.ImageUrl,
                YoutubeId = o.YoutubeId,
                VideoUri = o.VideoUri,
                PodcastUri = o.PodcastUri,
                ZuneAppId = o.ZuneAppId,
                OriginalDownloadDateTime = o.OriginalDownloadDateTime,
                Image = o.Image == null ? null : Convert(o.Image),
                HasBeenViewed = o.HasBeenViewed,
                IsFavorite = o.IsFavorite,
            };
        }

        public ViewModels.LiveTileNewsList Convert(Outgoing.LiveTileNewsList o)
        {
            return new ViewModels.LiveTileNewsList
            {
                FeedCount = o.FeedCount,
                NewNewsCount = o.NewNewsCount,
                FeaturedNewsCount = o.FeaturedNewsCount,
                News = o.News == null ? null : GetJoinedNews(o.Feeds.Select(Convert).ToList(), o.News.Select(Convert).ToList()).ToList(),
            };
        }
    }
}
