using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Threading.Tasks;
using Weave.ViewModels.Contracts.Client;

namespace Weave.ViewModels
{
    public class UserInfo : INotifyPropertyChanged
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
        }


        public async Task<NewsList> GetNewsForCategory(string category, EntryType entry = EntryType.Peek, int skip = 0, int take = 10)
        {
            return await repo.GetNews(category, entry, skip, take);
        }

        public async Task<NewsList> GetNewsForFeed(Guid feedId, EntryType entry = EntryType.Peek, int skip = 0, int take = 10)
        {
            return await repo.GetNews(feedId, entry, skip, take);
        }




        #region Feed management

        public bool ShouldRefreshFeedsInfo()
        {
            return shouldRefreshFeeds || DateTime.UtcNow - lastFeedsInfoRefresh > TimeSpan.FromMinutes(1);
        }

        public async Task RefreshFeedsInfo()
        {
            var feedsInfo = await repo.GetFeeds();
            var feeds = feedsInfo.Feeds;

            Feeds = new ObservableCollection<Feed>(feeds);
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs("Feeds"));

            shouldRefreshFeeds = false;
        }

        public async Task AddFeed(Feed feed)
        {
            var returnedFeed = await repo.AddFeed(feed);
            Feeds.Add(returnedFeed);
        }

        public async Task RemoveFeed(Feed feed)
        {
            await repo.RemoveFeed(feed);
            Feeds.Remove(feed);
        }

        public async Task UpdateFeed(Feed feed)
        {
            await repo.UpdateFeed(feed);
        }

        public async Task BatchChange(List<Feed> added = null, List<Feed> removed = null, List<Feed> updated = null)
        {
            await repo.BatchChange(added, removed, updated);
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
            await repo.MarkArticleRead(newsItem);
            newsItem.HasBeenViewed = true;
        }

        public async Task MarkArticleUnread(NewsItem newsItem)
        {
            await repo.MarkArticleUnread(newsItem);
            newsItem.HasBeenViewed = false;
        }

        public Task MarkArticlesSoftRead(List<NewsItem> newsItems)
        {
            return repo.MarkArticlesSoftRead(newsItems);
        }

        public Task<List<NewsItem>> GetRead(int skip = 0, int take = 10)
        {
            return repo.GetRead(skip, take);
        }

        public async Task AddFavorite(NewsItem newsItem)
        {
            await repo.AddFavorite(newsItem);
            newsItem.IsFavorite = true;
        }

        public async Task RemoveFavorite(NewsItem newsItem)
        {
            await repo.RemoveFavorite(newsItem);
            newsItem.IsFavorite = false;
        }

        public Task<List<NewsItem>> GetFavorites(int skip = 0, int take = 10)
        {
            return repo.GetFavorites(skip, take);
        }

        #endregion




        public event PropertyChangedEventHandler PropertyChanged;
    }
}
