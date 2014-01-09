using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Weave.Article.Service.Contracts;
using Weave.User.Service.Contracts;
using Weave.ViewModels.Contracts.Client;
using DTOs = Weave.User.Service.DTOs;
using Incoming = Weave.User.Service.DTOs.ServerIncoming;
using Outgoing = Weave.User.Service.DTOs.ServerOutgoing;

namespace Weave.ViewModels.Repository
{
    public class StandardRepository : IViewModelRepository
    {
        IWeaveUserService userService;
        IWeaveArticleService articleService;

        public StandardRepository(IWeaveUserService userService, IWeaveArticleService articleService)
        {
            this.userService = userService;
            this.articleService = articleService;
        }

        public async Task<UserInfo> AddUserAndReturnUserInfo(UserInfo incomingUser)
        {
            var outgoingUser = Convert(incomingUser);
            var user = await userService.AddUserAndReturnUserInfo(outgoingUser);
            return Convert(user);
        }

        public async Task<UserInfo> GetUserInfo(Guid userId, bool refresh = false)
        {
            var user = await userService.GetUserInfo(userId, refresh);
            return Convert(user);
        }

        public async Task<NewsList> GetNews(Guid userId, string category, EntryType entry = EntryType.Peek, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false)
        {
            var userNews = await userService.GetNews(userId, category, (Weave.User.Service.Contracts.EntryType)entry, skip, take, (DTOs.NewsItemType)type, requireImage);
            return Convert(userNews);
        }

        public async Task<NewsList> GetNews(Guid userId, Guid feedId, EntryType entry = EntryType.Peek, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false)
        {
            var userNews = await userService.GetNews(userId, feedId, (Weave.User.Service.Contracts.EntryType)entry, skip, take, (DTOs.NewsItemType)type, requireImage);
            return Convert(userNews);
        }

        public async Task<FeedsInfoList> GetFeeds(Guid userId, bool refresh = false, bool nested = false)
        {
            var feedsInfoList = await userService.GetFeeds(userId, refresh, nested);
            return Convert(feedsInfoList);
        }

        public async Task<FeedsInfoList> GetFeeds(Guid userId, string category, bool refresh = false, bool nested = false)
        {
            var feedsInfoList = await userService.GetFeeds(userId, category, refresh, nested);
            return Convert(feedsInfoList);
        }

        public async Task<FeedsInfoList> GetFeeds(Guid userId, Guid feedId, bool refresh = false, bool nested = false)
        {
            var feedsInfoList = await userService.GetFeeds(userId, feedId, refresh, nested);
            return Convert(feedsInfoList);
        }

        public async Task<Feed> AddFeed(Guid userId, Feed feed)
        {
            var returnedFeed = await userService.AddFeed(userId, ConvertToNewFeed(feed));
            return Convert(returnedFeed);
        }

        public Task RemoveFeed(Guid userId, Feed feed)
        {
            return userService.RemoveFeed(userId, feed.Id);
        }

        public Task UpdateFeed(Guid userId, Feed feed)
        {
            return userService.UpdateFeed(userId, ConvertToUpdatedFeed(feed));
        }

        public Task BatchChange(Guid userId, List<Feed> added = null, List<Feed> removed = null, List<Feed> updated = null)
        {
            return userService.BatchChange(userId,
                new Incoming.BatchFeedChange
                {
                    Added = added == null ? null : added.Select(ConvertToNewFeed).ToList(),
                    Removed = removed == null ? null : removed.Select(o => o.Id).ToList(),
                    Updated = updated == null ? null : updated.Select(ConvertToUpdatedFeed).ToList(),
                });
        }

        public Task MarkArticleRead(Guid userId, NewsItem newsItem)
        {
            return userService.MarkArticleRead(userId, newsItem.Id);
        }

        public Task MarkArticleUnread(Guid userId, NewsItem newsItem)
        {
            return userService.MarkArticleUnread(userId, newsItem.Id);
        }

        public Task MarkArticlesSoftRead(Guid userId, List<NewsItem> newsItems)
        {
            return userService.MarkArticlesSoftRead(userId, newsItems == null ? null : newsItems.Select(o => o.Id).ToList());
        }

        public Task MarkArticlesSoftRead(Guid userId, string category)
        {
            return userService.MarkArticlesSoftRead(userId, category);
        }

        public Task MarkArticlesSoftRead(Guid userId, Guid feedId)
        {
            return userService.MarkArticlesSoftRead(userId, feedId);
        }

        public Task AddFavorite(Guid userId, NewsItem newsItem)
        {
            return userService.AddFavorite(userId, newsItem.Id);
        }

        public Task RemoveFavorite(Guid userId, NewsItem newsItem)
        {
            return userService.RemoveFavorite(userId, newsItem.Id);
        }

        public async Task<List<NewsItem>> GetRead(Guid userId, int skip = 0, int take = 10)
        {
            var read = await articleService.GetRead(userId, take, skip);
            return read == null ? null : read.Select(ConvertToRead).ToList();
        }

        public async Task<List<NewsItem>> GetFavorites(Guid userId, int skip = 0, int take = 10)
        {
            var favorites = await articleService.GetFavorites(userId, take, skip);
            return favorites == null ? null : favorites.Select(ConvertToFavorite).ToList();
        }

        public Task Bookmark(Guid userId, NewsItem newsItem, BookmarkType bookmarkType)
        {
            switch (bookmarkType)
            {
                case BookmarkType.Favorite:
                    return articleService.AddFavorite(userId, Convert(newsItem));

                case BookmarkType.Read:
                    return articleService.MarkRead(userId, Convert(newsItem));

                default:
                    throw new ArgumentException(
                        string.Format("unrecognized bookmarkType: {0}", bookmarkType));
            }
        }

        public Task SetArticleDeleteTimes(Guid userId, string markedReadExpiry, string unreadExpiry)
        {
            return userService.SetArticleDeleteTimes(userId,
                new Incoming.ArticleDeleteTimes
                {
                    ArticleDeletionTimeForMarkedRead = markedReadExpiry,
                    ArticleDeletionTimeForUnread = unreadExpiry,
                });
        }




        #region Conversion helpers

        Incoming.UserInfo Convert(UserInfo o)
        {
            return new Incoming.UserInfo
            {
                Id = o.Id,
                Feeds = o.Feeds == null ? null : o.Feeds.Select(ConvertToNewFeed).ToList(),
            };
        }

        UserInfo Convert(Outgoing.UserInfo o)
        {
            return new UserInfo(this)
            {
                Id = o.Id,
                Feeds = o.Feeds == null ? null : new ObservableCollection<Feed>(o.Feeds.Select(Convert)),
                PreviousLoginTime = o.PreviousLoginTime,
                CurrentLoginTime = o.CurrentLoginTime,
                ArticleDeletionTimeForMarkedRead = o.ArticleDeletionTimeForMarkedRead,
                ArticleDeletionTimeForUnread = o.ArticleDeletionTimeForUnread,
                LatestNews = o.LatestNews == null ? null : GetJoinedNews(o.Feeds.Select(Convert).ToList(), o.LatestNews.Select(Convert).ToList()).ToList(),
            };
        }

        NewsList Convert(Outgoing.NewsList o)
        {
            return new NewsList
            {
                FeedCount = o.FeedCount,
                NewArticleCount = o.NewArticleCount,
                UnreadArticleCount = o.UnreadArticleCount,
                TotalArticleCount = o.TotalArticleCount,
                IncludedArticleCount = o.Page == null ? 0 : o.Page.IncludedArticleCount,
                Skip = o.Page == null ? 0 : o.Page.Skip,
                Take = o.Page == null ? 0 : o.Page.Take,
                News = o.News == null ? null : GetJoinedNews(o.Feeds.Select(Convert).ToList(), o.News.Select(Convert).ToList()).ToList(),
            };
        }

        FeedsInfoList Convert(Outgoing.FeedsInfoList o)
        {
            return new FeedsInfoList
            {
                TotalFeedCount = o.TotalFeedCount,
                Categories = o.Categories == null ? null : o.Categories.Select(Convert).ToList(),
                Feeds = o.Feeds == null ? null : o.Feeds.Select(Convert).ToList(),
                NewArticleCount = o.NewArticleCount,
                UnreadArticleCount = o.UnreadArticleCount,
                TotalArticleCount = o.TotalArticleCount,
            };
        }

        IEnumerable<NewsItem> GetJoinedNews(IEnumerable<Feed> feeds, IEnumerable<NewsItem> news)
        {
            return from n in news
                   join f in feeds on n.Feed.Id equals f.Id
                   select Convert(n, f);
        }
        
        NewsItem Convert(NewsItem n, Feed f)
        {
            n.Feed = f;
            return n;
        }

        CategoryInfo Convert(Outgoing.CategoryInfo o)
        {
            return new CategoryInfo
            {
                Category = o.Category,
                TotalFeedCount = o.TotalFeedCount,
                Feeds = o.Feeds == null ? null : o.Feeds.Select(Convert).ToList(),
                NewArticleCount = o.NewArticleCount,
                UnreadArticleCount = o.UnreadArticleCount,
                TotalArticleCount = o.TotalArticleCount,
            };
        }

        Feed Convert(Outgoing.Feed o)
        {
            return new Feed
            {
                Id = o.Id,
                Name = o.Name,
                Uri = o.Uri,
                IconUrl = o.IconUri,
                Category = o.Category,
                ArticleViewingType = (ArticleViewingType)o.ArticleViewingType,
                TeaserImageUrl = o.TeaserImageUrl,
                NewArticleCount = o.NewArticleCount,
                UnreadArticleCount = o.UnreadArticleCount,
                TotalArticleCount = o.TotalArticleCount,
            };
        }

        NewsItem Convert(Outgoing.NewsItem o)
        {
            return new NewsItem
            {
                Id = o.Id,
                Feed = new Feed { Id = o.FeedId },
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

        Image Convert(Outgoing.Image o)
        {
            return new Image
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
                ArticleViewingType = (Weave.User.Service.DTOs.ArticleViewingType)o.ArticleViewingType,
            };
        }

        Incoming.UpdatedFeed ConvertToUpdatedFeed(Feed o)
        {
            return new Incoming.UpdatedFeed
            {
                Id = o.Id,
                Name = o.Name,
                Category = o.Category,
                ArticleViewingType = (Weave.User.Service.DTOs.ArticleViewingType)o.ArticleViewingType, 
            };
        }

        NewsItem ConvertToFavorite(Weave.Article.Service.DTOs.ServerOutgoing.SavedNewsItem o)
        {
            var newsItem = ConvertToRead(o);
            newsItem.IsFavorite = true;
            return newsItem;
        }

        NewsItem ConvertToRead(Weave.Article.Service.DTOs.ServerOutgoing.SavedNewsItem o)
        {
            return new NewsItem
            {
                Id = o.Id,
                Feed = new Feed { Name = o.SourceName, ArticleViewingType = ArticleViewingType.Mobilizer },
                Title = o.Title,
                Link = o.Link,
                UtcPublishDateTime = o.UtcPublishDateTime,
                ImageUrl = o.ImageUrl,
                YoutubeId = o.YoutubeId,
                VideoUri = o.VideoUri,
                PodcastUri = o.PodcastUri,
                ZuneAppId = o.ZuneAppId,
            };
        }

        Weave.Article.Service.DTOs.ServerIncoming.SavedNewsItem Convert(NewsItem o)
        {
            return new Article.Service.DTOs.ServerIncoming.SavedNewsItem
            {
                SourceName = o.OriginalSource,
                Title = o.Title,
                Link = o.Link,
                UtcPublishDateTime = o.UtcPublishDateTime,
                ImageUrl = o.ImageUrl,
                YoutubeId = o.YoutubeId,
                VideoUri = o.VideoUri,
                PodcastUri = o.PodcastUri,
                ZuneAppId = o.ZuneAppId,
                Image  = o.Image == null ? null : Convert(o.Image),
                Tags = new List<string> {  o.Feed.Category },
            };
        }

        Weave.Article.Service.DTOs.Image Convert(Image o)
        {
            return new Weave.Article.Service.DTOs.Image
            {
                BaseImageUrl = o.BaseImageUrl,
                Width = o.Width,
                Height = o.Height,
                OriginalUrl = o.OriginalUrl,
                SupportedFormats = o.SupportedFormats,
            };
        }

        #endregion
    }
}