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
        public List<CategoryOrFeedTeaserImage> TeaserImages { get; set; }




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

        #endregion
    }
}
