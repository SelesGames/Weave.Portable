using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace Weave.ViewModels.Contracts.Client
{
    public interface IViewModelRepository
    {
        //Task<UserInfo> AddUserAndReturnUserInfo(UserInfo incomingUser);
        Task<UserInfo> GetUserInfo(bool refresh = false);
        Task<NewsList> GetNews(string category, bool refresh = false, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false);
        Task<NewsList> GetNews(Guid feedId, bool refresh = false, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false);

        Task AddFeed(Feed feed);
        Task RemoveFeed(Feed feed);
        Task UpdateFeed(Feed feed);
        Task BatchChange(List<Feed> added = null, List<Feed> removed = null, List<Feed> updated = null);
        
        Task MarkArticleRead(NewsItem newsItem);
        Task MarkArticleUnread(NewsItem newsItem);
        Task AddFavorite(NewsItem newsItem);
        Task RemoveFavorite(NewsItem newsItem);
    }
}
