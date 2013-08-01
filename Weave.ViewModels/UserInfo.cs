using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using Weave.ViewModels.Contracts.Client;

namespace Weave.ViewModels
{
    public class UserInfo : ViewModelBase
    {
        IViewModelRepository repo;
        bool shouldRefreshFeeds = false;
        DateTime lastFeedsInfoRefresh = DateTime.UtcNow;

        public Guid Id { get; set; }
        public ObservableCollection<Feed> Feeds { get; set; }
        public DateTime PreviousLoginTime { get; set; }
        public DateTime CurrentLoginTime { get; set; }
        public List<NewsItem> LatestNews { get; set; }

        public UserInfo(IViewModelRepository repo)
        {
            this.repo = repo;
            Feeds = new ObservableCollection<Feed>();
        }

        public async Task Load(bool refreshNews = false)
        {
            var user = await repo.GetUserInfo(Id, refreshNews);
            UpdateTo(user);
        }

        public async Task Save()
        {
            var user = await repo.AddUserAndReturnUserInfo(this);
            UpdateTo(user);
        }

        void UpdateTo(UserInfo user)
        {
            if (user == null)
                return;

            PreviousLoginTime = user.PreviousLoginTime;
            CurrentLoginTime = user.CurrentLoginTime;
            LatestNews = user.LatestNews;
            Feeds = user.Feeds;
            Raise("PreviousLoginTime", "CurrentLoginTime", "LatestNews", "Feeds");
            //var setComparison = Feeds.GetSetComparison(user.Feeds, new FeedEqualityComparer());
            //foreach (var feed in setComparison.Same)
            //    feed.UpdateTo
        }




        public async Task<NewsList> GetNewsForCategory(string category, EntryType entry = EntryType.Peek, int skip = 0, int take = 10)
        {
            return await repo.GetNews(Id, category, entry, skip, take);
        }

        public async Task<NewsList> GetNewsForFeed(Guid feedId, EntryType entry = EntryType.Peek, int skip = 0, int take = 10)
        {
            return await repo.GetNews(Id, feedId, entry, skip, take);
        }




        #region Feed management

        public bool ShouldRefreshFeedsInfo()
        {
            return shouldRefreshFeeds || DateTime.UtcNow - lastFeedsInfoRefresh > TimeSpan.FromMinutes(1);
        }

        public async Task RefreshFeedsInfo()
        {
            var feedsInfo = await repo.GetFeeds(Id);
            var feeds = feedsInfo.Feeds;

            Feeds = new ObservableCollection<Feed>(feeds);
            Raise("Feeds");

            shouldRefreshFeeds = false;
        }

        public async Task AddFeed(Feed feed)
        {
            var returnedFeed = await repo.AddFeed(Id, feed);
            Feeds.Add(returnedFeed);
        }

        public async Task RemoveFeed(Feed feed)
        {
            await repo.RemoveFeed(Id, feed);
            Feeds.Remove(feed);
        }

        public async Task UpdateFeed(Feed feed)
        {
            await repo.UpdateFeed(Id, feed);
        }

        public async Task BatchChange(List<Feed> added = null, List<Feed> removed = null, List<Feed> updated = null)
        {
            await repo.BatchChange(Id, added, removed, updated);
            if (added != null)
            {
                foreach (var feed in added)
                    Feeds.Add(feed);
            }
            if (removed != null)
            {
                foreach (var feed in removed)
                    Feeds.Remove(feed);
            }
            shouldRefreshFeeds = true;
        }

        #endregion




        #region Article management

        public async Task MarkArticleRead(NewsItem newsItem)
        {
            await repo.MarkArticleRead(Id, newsItem);
            newsItem.HasBeenViewed = true;
        }

        public async Task MarkArticleUnread(NewsItem newsItem)
        {
            await repo.MarkArticleUnread(Id, newsItem);
            newsItem.HasBeenViewed = false;
        }

        public Task MarkArticlesSoftRead(List<NewsItem> newsItems)
        {
            return repo.MarkArticlesSoftRead(Id, newsItems);
        }

        public Task<List<NewsItem>> GetRead(int skip = 0, int take = 10)
        {
            return repo.GetRead(Id, skip, take);
        }

        public async Task AddFavorite(NewsItem newsItem)
        {
            await repo.AddFavorite(Id, newsItem);
            newsItem.IsFavorite = true;
        }

        public async Task RemoveFavorite(NewsItem newsItem)
        {
            await repo.RemoveFavorite(Id, newsItem);
            newsItem.IsFavorite = false;
        }

        public Task<List<NewsItem>> GetFavorites(int skip = 0, int take = 10)
        {
            return repo.GetFavorites(Id, skip, take);
        }

        #endregion
    }
}
