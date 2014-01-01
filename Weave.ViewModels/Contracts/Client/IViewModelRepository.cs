using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Weave.ViewModels.Contracts.Client
{
    public interface IViewModelRepository
    {
        Task<UserInfo> AddUserAndReturnUserInfo(UserInfo incomingUser);
        Task<UserInfo> GetUserInfo(Guid userId, bool refresh = false);
        
        Task<NewsList> GetNews(Guid userId, string category, EntryType entry = EntryType.Peek, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false);
        Task<NewsList> GetNews(Guid userId, Guid feedId, EntryType entry = EntryType.Peek, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false);

        Task<FeedsInfoList> GetFeeds(Guid userId, bool refresh = false, bool nested = false);
        Task<FeedsInfoList> GetFeeds(Guid userId, string category, bool refresh = false, bool nested = false);
        Task<FeedsInfoList> GetFeeds(Guid userId, Guid feedId, bool refresh = false, bool nested = false);
        Task<Feed> AddFeed(Guid userId, Feed feed);
        Task RemoveFeed(Guid userId, Feed feed);
        Task UpdateFeed(Guid userId, Feed feed);
        Task BatchChange(Guid userId, List<Feed> added = null, List<Feed> removed = null, List<Feed> updated = null);

        Task MarkArticleRead(Guid userId, NewsItem newsItem);
        Task MarkArticleUnread(Guid userId, NewsItem newsItem);
        Task MarkArticlesSoftRead(Guid userId, List<NewsItem> newsItems);
        Task MarkArticlesSoftRead(Guid userId, string category);
        Task MarkArticlesSoftRead(Guid userId, Guid feedId);
        Task<List<NewsItem>> GetRead(Guid userId, int skip = 0, int take = 10);

        Task AddFavorite(Guid userId, NewsItem newsItem);
        Task RemoveFavorite(Guid userId, NewsItem newsItem);
        Task<List<NewsItem>> GetFavorites(Guid userId, int skip = 0, int take = 10);

        Task Bookmark(Guid userId, NewsItem newsItem, BookmarkType bookmarkType);

        Task SetArticleDeleteTimes(Guid userId, string markedReadExpiry, string unreadExpiry);
    }
}
