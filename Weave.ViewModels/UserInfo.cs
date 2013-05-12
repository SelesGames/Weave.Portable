using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Weave.ViewModels.Contracts.Client;

namespace Weave.ViewModels
{
    public class UserInfo
    {
        IViewModelRepository repo;

        public Guid Id { get; set; }
        public List<Feed> Feeds { get; set; }
        public DateTime PreviousLoginTime { get; set; }
        public DateTime CurrentLoginTime { get; set; }
        public List<NewsItem> LatestNews { get; set; }

        public UserInfo(IViewModelRepository repo)
        {
            this.repo = repo;
        }


        public async Task<NewsList> GetNewsForCategory(string category, bool refresh = false, bool markEntry = false, int skip = 0, int take = 10)
        {
            return await repo.GetNews(category, refresh, markEntry, skip, take);
        }

        public async Task<NewsList> GetNewsForFeed(Guid feedId, bool refresh = false, bool markEntry = false, int skip = 0, int take = 10)
        {
            return await repo.GetNews(feedId, refresh, markEntry, skip, take);
        }




        #region Feed management

        public async Task AddFeed(Feed feed)
        {
            await repo.AddFeed(feed);
        }

        public async Task RemoveFeed(Feed feed)
        {
            await repo.RemoveFeed(feed);
        }

        public async Task UpdateFeed(Feed feed)
        {
            await repo.UpdateFeed(feed);
        }

        public async Task BatchChange(List<Feed> added = null, List<Feed> removed = null, List<Feed> updated = null)
        {
            await repo.BatchChange(added, removed, updated);
        }

        #endregion




        #region Article management

        public async Task MarkArticleRead(NewsItem newsItem)
        {
            await repo.MarkArticleRead(newsItem);
        }

        public async Task MarkArticleUnread(NewsItem newsItem)
        {
            await repo.MarkArticleUnread(newsItem);
        }

        public async Task AddFavorite(NewsItem newsItem)
        {
            await repo.AddFavorite(newsItem);
        }

        public async Task RemoveFavorite(NewsItem newsItem)
        {
            await repo.RemoveFavorite(newsItem);
        }

        #endregion
    }
}
