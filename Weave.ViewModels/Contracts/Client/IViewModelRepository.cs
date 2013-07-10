using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Weave.ViewModels.Contracts.Client
{
    public interface IViewModelRepository
    {
        //Task<UserInfo> AddUserAndReturnUserInfo(UserInfo incomingUser);
        Task<UserInfo> GetUserInfo(bool refresh = false);
        Task<NewsList> GetNews(string category, EntryType entry = EntryType.Peek, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false);
        Task<NewsList> GetNews(Guid feedId, EntryType entry = EntryType.Peek, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false);

        Task<FeedsInfoList> GetFeeds(bool refresh = false, bool nested = false);
        Task<FeedsInfoList> GetFeeds(string category, bool refresh = false, bool nested = false);
        Task<FeedsInfoList> GetFeeds(Guid feedId, bool refresh = false, bool nested = false);
        Task<Feed> AddFeed(Feed feed);
        Task RemoveFeed(Feed feed);
        Task UpdateFeed(Feed feed);
        Task BatchChange(List<Feed> added = null, List<Feed> removed = null, List<Feed> updated = null);
        
        Task MarkArticleRead(NewsItem newsItem);
        Task MarkArticleUnread(NewsItem newsItem);
        Task MarkArticlesSoftRead(List<NewsItem> newsItems);
        Task<List<NewsItem>> GetRead(int skip = 0, int take = 10);

        Task AddFavorite(NewsItem newsItem);
        Task RemoveFavorite(NewsItem newsItem);
        Task<List<NewsItem>> GetFavorites(int skip = 0, int take = 10);
    }
}
