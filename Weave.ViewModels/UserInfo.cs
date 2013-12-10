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
        Guid id;




        #region Public Properties

        public Guid Id
        {
            get { return id; }
            set { id = value; Raise("Id"); }
        }

        public ObservableCollection<Feed> Feeds { get; set; }
        public List<NewsItem> LatestNews { get; set; }
        public DateTime PreviousLoginTime { get; set; }
        public DateTime CurrentLoginTime { get; set; }
        public bool AreFeedsModified { get; private set; }

        #endregion




        #region Constructor

        public UserInfo(IViewModelRepository repo)
        {
            this.repo = repo;
            Feeds = new ObservableCollection<Feed>();
        }

        #endregion




        #region User Creation and Retrieval

        public async Task Create()
        {
            var user = await repo.AddUserAndReturnUserInfo(this);
            UpdateTo(user);
        }

        public async Task Load(bool refreshNews = false)
        {
            var user = await repo.GetUserInfo(Id, refreshNews);
            UpdateTo(user);
        }

        #endregion




        #region Get News (by category or feed)

        public async Task<NewsList> GetNewsForCategory(string category, EntryType entry = EntryType.Peek, int skip = 0, int take = 10)
        {
            return await repo.GetNews(Id, category, entry, skip, take);
        }

        public async Task<NewsList> GetNewsForFeed(Guid feedId, EntryType entry = EntryType.Peek, int skip = 0, int take = 10)
        {
            return await repo.GetNews(Id, feedId, entry, skip, take);
        }

        #endregion




        #region Feed management

        public async Task LoadFeeds(bool refresh = false)
        {
            var feedsInfo = await repo.GetFeeds(Id, refresh: refresh, nested: false);
            var feeds = feedsInfo.Feeds;

            Feeds = new ObservableCollection<Feed>(feeds);
            Raise("Feeds");

            AreFeedsModified = false;
        }

        public async Task AddFeed(Feed feed)
        {
            var returnedFeed = await repo.AddFeed(Id, feed);
            Feeds.Add(returnedFeed);
            AreFeedsModified = true;
        }

        public async Task RemoveFeed(Feed feed)
        {
            await repo.RemoveFeed(Id, feed);
            Feeds.Remove(feed);
            AreFeedsModified = true;
        }

        public async Task UpdateFeed(Feed feed)
        {
            await repo.UpdateFeed(Id, feed);
            AreFeedsModified = true;
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
            AreFeedsModified = true;
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

        public async Task MarkCategorySoftRead(string category)
        {
            await repo.MarkArticlesSoftRead(Id, category);
        }

        public async Task MarkFeedSoftRead(Guid feedId)
        {
            await repo.MarkArticlesSoftRead(Id, feedId);
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

        public Task Bookmark(NewsItem newsItem, BookmarkType bookmarkType)
        {
            return repo.Bookmark(Id, newsItem, bookmarkType);
        }

        #endregion




        #region private helper functions

        void UpdateTo(UserInfo user)
        {
            if (user == null)
                return;

            PreviousLoginTime = user.PreviousLoginTime;
            CurrentLoginTime = user.CurrentLoginTime;
            LatestNews = user.LatestNews;
            Feeds = user.Feeds;
            Raise("PreviousLoginTime", "CurrentLoginTime", "LatestNews", "Feeds");
        }

        #endregion
    }
}
