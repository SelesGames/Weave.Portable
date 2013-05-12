using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Weave.UserFeedAggregator.Contracts;
using Weave.ViewModels.Contracts.Client;
using DTOs = Weave.UserFeedAggregator.DTOs;
using Incoming = Weave.UserFeedAggregator.DTOs.ServerIncoming;
using Outgoing = Weave.UserFeedAggregator.DTOs.ServerOutgoing;

namespace Weave.ViewModels.Repository
{
    public class StandardRepository : IViewModelRepository
    {
        IWeaveUserService innerClient;
        Guid userId;

        public StandardRepository(Guid userId, IWeaveUserService innerClient)
        {
            this.userId = userId;
            this.innerClient = innerClient;
        }

        public async Task<UserInfo> GetUserInfo(bool refresh = false)
        {
            var user = await innerClient.GetUserInfo(userId, refresh);
            return Convert(user);
        }

        public async Task<NewsList> GetNews(string category, bool refresh = false, bool markEntry = false, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false)
        {
            var userNews = await innerClient.GetNews(userId, category, refresh, markEntry, skip, take, (DTOs.NewsItemType)type, requireImage);
            return Convert(userNews);
        }

        public async Task<NewsList> GetNews(Guid feedId, bool refresh = false, bool markEntry = false, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false)
        {
            var userNews = await innerClient.GetNews(userId, feedId, refresh, markEntry, skip, take, (DTOs.NewsItemType)type, requireImage);
            return Convert(userNews);
        }

        public Task AddFeed(Feed feed)
        {
            return innerClient.AddFeed(userId, ConvertToNewFeed(feed));
        }

        public Task RemoveFeed(Feed feed)
        {
            return innerClient.RemoveFeed(userId, feed.Id);
        }

        public Task UpdateFeed(Feed feed)
        {
            return innerClient.UpdateFeed(userId, ConvertToUpdatedFeed(feed));
        }

        public Task BatchChange(List<Feed> added = null, List<Feed> removed = null, List<Feed> updated = null)
        {
            return innerClient.BatchChange(userId,
                new Incoming.BatchFeedChange
                {
                    Added = added == null ? null : added.Select(ConvertToNewFeed).ToList(),
                    Removed = removed == null ? null : removed.Select(o => o.Id).ToList(),
                    Updated = updated == null ? null : updated.Select(ConvertToUpdatedFeed).ToList(),
                });
        }

        public Task MarkArticleRead(NewsItem newsItem)
        {
            return innerClient.MarkArticleRead(userId, newsItem.Feed.Id, newsItem.Id);
        }

        public Task MarkArticleUnread(NewsItem newsItem)
        {
            return innerClient.MarkArticleUnread(userId, newsItem.Feed.Id, newsItem.Id);
        }

        public Task AddFavorite(NewsItem newsItem)
        {
            return innerClient.AddFavorite(userId, newsItem.Feed.Id, newsItem.Id);
        }

        public Task RemoveFavorite(NewsItem newsItem)
        {
            return innerClient.RemoveFavorite(userId, newsItem.Feed.Id, newsItem.Id);
        }




        #region Helper methods

        //IList<NewsItem> CreateDistinctAndOrdered(Outgoing.NewsList newsList)
        //{
        //    var allNews = (from n in newsList.News
        //                  join f in newsList.Feeds on n.FeedId equals f.Id
        //                  select Convert(n, f))
        //                  .OrderBy(o => o.LocalDateTime)
        //                  .Distinct(NewsItemComparer.Instance)
        //                  .ToList(); 
        //    //var allNews = userNews.Feeds
        //    //    .SelectMany(o => o.News
        //    //        .Select(n => Convert(n, o)))
        //    //    .OrderBy(o => o.LocalDateTime)
        //    //    .Distinct(NewsItemComparer.Instance)
        //    //    .ToList();

        //    return allNews;
        //}

        //IList<NewsItem> CreateOrdered(Outgoing.LiveTileNewsList liveTileNewsList)
        //{
        //    var allNews = (from n in liveTileNewsList.News
        //                   join f in liveTileNewsList.Feeds on n.FeedId equals f.Id
        //                   select Convert(n, f))
        //                  .OrderBy(o => o.LocalDateTime)
        //                  .ToList(); 

        //    //var allNews = liveTileNewsList.Feeds
        //    //    .SelectMany(o => o.News
        //    //        .Select(n => Convert(n, o)))
        //    //    .OrderBy(o => o.LocalDateTime)
        //    //    .ToList();

        //    return allNews;
        //}

        #endregion




        #region Conversion helpers

        ViewModels.UserInfo Convert(Outgoing.UserInfo o)
        {
            return new UserInfo(this)
            {
                Id = o.Id,
                Feeds = o.Feeds == null ? null : o.Feeds.Select(Convert).ToList(),
                PreviousLoginTime = o.PreviousLoginTime,
                CurrentLoginTime = o.CurrentLoginTime,
                LatestNews = o.LatestNews == null ? null : GetJoinedNews(o.Feeds.Select(Convert).ToList(), o.LatestNews.Select(Convert).ToList()).ToList(),
            };
        }

        ViewModels.NewsList Convert(Outgoing.NewsList o)
        {
            return new ViewModels.NewsList
            {
                FeedCount = o.FeedCount,
                TotalNewsCount = o.TotalNewsCount,
                NewNewsCount = o.NewNewsCount,
                NewsCount = o.NewsCount,
                Skip = o.Skip,
                Take = o.Take,
                //News = o.News == null ? null : GetJoinedNews(o.Feeds, o.News).ToList(),
                News = o.News == null ? null : GetJoinedNews(o.Feeds.Select(Convert).ToList(), o.News.Select(Convert).ToList()).ToList(),
            };
        }

        IEnumerable<ViewModels.NewsItem> GetJoinedNews(IEnumerable<ViewModels.Feed> feeds, IEnumerable<ViewModels.NewsItem> news)
        {
            return from n in news
                   join f in feeds on n.Feed.Id equals f.Id
                   select Convert(n, f);
        }
        
        ViewModels.NewsItem Convert(ViewModels.NewsItem n, ViewModels.Feed f)
        {
            n.Feed = f;
            return n;
        }

        ViewModels.Feed Convert(Outgoing.Feed o)
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
                IsNew = o.IsNew,
                HasBeenViewed = o.HasBeenViewed,
                IsFavorite = o.IsFavorite,
            };
        }

        ViewModels.Image Convert(Outgoing.Image o)
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

        Incoming.NewFeed ConvertToNewFeed(Feed o)
        {
            return new Incoming.NewFeed
            {
                Uri = o.Uri,
                Name = o.Name,
                Category = o.Category,
                ArticleViewingType = (Weave.UserFeedAggregator.DTOs.ArticleViewingType)o.ArticleViewingType,
            };
        }

        Incoming.UpdatedFeed ConvertToUpdatedFeed(Feed o)
        {
            return new Incoming.UpdatedFeed
            {
                Id = o.Id,
                Name = o.Name,
                Category = o.Category,
                ArticleViewingType = (Weave.UserFeedAggregator.DTOs.ArticleViewingType)o.ArticleViewingType, 
            };
        }

        #endregion
    }
}
