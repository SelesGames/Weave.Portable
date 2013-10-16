using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Weave.ViewModels
{
    public abstract class NewsItemGroup : ViewModelBase
    {
        string displayName;
        int feedCount, newArticleCount, unreadArticleCount, totalArticleCount;




        #region Properties

        public string DisplayName
        {
            get { return displayName; }
            set { displayName = value; Raise("DisplayName"); }
        }

        public int FeedCount
        {
            get { return feedCount; }
            set { feedCount = value; Raise("FeedCount"); }
        }

        public int NewArticleCount
        {
            get { return newArticleCount; }
            set { newArticleCount = value; Raise("NewArticleCount"); }
        }

        public int UnreadArticleCount
        {
            get { return unreadArticleCount; }
            set { unreadArticleCount = value; Raise("UnreadArticleCount"); }
        }

        public int TotalArticleCount
        {
            get { return totalArticleCount; }
            set { totalArticleCount = value; Raise("TotalArticleCount"); }
        }

        #endregion




        public async Task<IEnumerable<NewsItem>> GetNews(EntryType entryType, int skip, int take)
        {
            var news = await GetNewsList(entryType, skip, take);

            FeedCount = news.FeedCount;
            NewArticleCount = news.NewArticleCount;
            UnreadArticleCount = news.UnreadArticleCount;
            TotalArticleCount = news.TotalArticleCount;

            return news.News;
        }

        public abstract Task<NewsList> GetNewsList(EntryType entryType, int skip, int take);
        public abstract void MarkEntry();
    }

    public class FeedGroup : NewsItemGroup
    {
        UserInfo user;
        Guid feedId;
        string feedName;
        CategoryGroup category;

        public FeedGroup(UserInfo user, Guid feedId, string feedName, CategoryGroup category)
        {
            this.user = user;
            this.feedId = feedId;
            this.feedName = feedName;
            this.category = category;

            DisplayName = feedName;
        }

        public override Task<NewsList> GetNewsList(EntryType entryType, int skip, int take)
        {
            return user.GetNewsForFeed(feedId, entryType, skip, take);
        }

        public override void MarkEntry()
        {
            int prevNewArticleCount = NewArticleCount;
            NewArticleCount = 0;
            if (category != null)
            {
                category.NewArticleCount -= prevNewArticleCount;
            }
        }
    }

    public class CategoryGroup : NewsItemGroup
    {
        UserInfo user;
        string category;
        IEnumerable<FeedGroup> feeds;

        public CategoryGroup(UserInfo user, string category, IEnumerable<FeedGroup> feeds)
        {
            this.user = user;
            this.category = category;
            this.feeds = feeds;

            DisplayName = category;
        }

        public override Task<NewsList> GetNewsList(EntryType entryType, int skip, int take)
        {
            return user.GetNewsForCategory(category, entryType, skip, take);
        }

        public override void MarkEntry()
        {
            NewArticleCount = 0;
            foreach (var feed in feeds)
                feed.MarkEntry();
        }
    }

    public class NewsItemGroupingCollection : ViewModelBase
    {
        public ObservableCollection<NewsItemGroup> Groups { get; private set; }

        public NewsItemGroupingCollection(IEnumerable<Feed> feeds)
        {
            
        }

        static IEnumerable<NewsItemGroup> CreateGroups(IEnumerable<Feed> feeds, Func<string, string> categoryCasing, Func<string, string> feedCasing)
        {
            var groupedFeeds = feeds.GroupBy(o => o.Category).ToList();

            var categories = 
                from c in groupedFeeds.Where(o => !string.IsNullOrEmpty(o.Key))
                select new CategoryGroup(null, c.Key, 
                    c.Select(o => new FeedGroup(null, o.Id, o.Name, null)));


            var looseFeeds = groupedFeeds
                .Where(o => string.IsNullOrEmpty(o.Key))
                .SelectMany(o => o)
                .Where(o => o.Name != null)
                .Select(o => new FeedGroup(null, o.Id, o.Name, null));

            var sources = new List<NewsItemGroup>();

            //sources.Add(
            //    new CategoryOrLooseFeedViewModel
            //    {
            //        Name = categoryCasing("all news"),
            //        Type = CategoryOrLooseFeedViewModel.CategoryOrFeedType.Category,
            //        NewArticleCount = feeds.Sum(o => o.NewArticleCount)
            //    });

            sources.AddRange(
                ((IEnumerable<NewsItemGroup>)categories)
                    .Union(((IEnumerable<NewsItemGroup>)looseFeeds)));

            return sources;
        }
    }
}
