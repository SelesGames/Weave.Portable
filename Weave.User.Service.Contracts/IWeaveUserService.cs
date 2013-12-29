using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Weave.User.Service.DTOs;
using Incoming = Weave.User.Service.DTOs.ServerIncoming;
using Outgoing = Weave.User.Service.DTOs.ServerOutgoing;

namespace Weave.User.Service.Contracts
{
    public interface IWeaveUserService
    {
        Task<Outgoing.UserInfo> AddUserAndReturnUserInfo(Incoming.UserInfo incomingUser);
        Task<Outgoing.UserInfo> GetUserInfo(Guid userId, bool refresh = false);
        
        Task<Outgoing.NewsList> GetNews(Guid userId, string category, EntryType entry = EntryType.Peek, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false);
        Task<Outgoing.NewsList> GetNews(Guid userId, Guid feedId, EntryType entry = EntryType.Peek, int skip = 0, int take = 10, NewsItemType type = NewsItemType.Any, bool requireImage = false);

        Task<Outgoing.FeedsInfoList> GetFeeds(Guid userId, bool refresh = false, bool nested = false);
        Task<Outgoing.FeedsInfoList> GetFeeds(Guid userId, string category, bool refresh = false, bool nested = false);
        Task<Outgoing.FeedsInfoList> GetFeeds(Guid userId, Guid feedId, bool refresh = false, bool nested = false);
        Task<Outgoing.Feed> AddFeed(Guid userId, Incoming.NewFeed feed);
        Task RemoveFeed(Guid userId, Guid feedId);
        Task UpdateFeed(Guid userId, Incoming.UpdatedFeed feed);
        Task BatchChange(Guid userId, Incoming.BatchFeedChange changeSet);

        Task MarkArticleRead(Guid userId, Guid newsItemId);
        Task MarkArticleUnread(Guid userId, Guid newsItemId);
        Task MarkArticlesSoftRead(Guid userId, List<Guid> newsItemIds);
        Task MarkArticlesSoftRead(Guid userId, string category);
        Task MarkArticlesSoftRead(Guid userId, Guid feedId);
        
        Task AddFavorite(Guid userId, Guid newsItemId);
        Task RemoveFavorite(Guid userId, Guid newsItemId);

        Task SetArticleDeleteTimes(Guid userId, Incoming.ArticleDeleteTimes articleDeleteTimes);
    }
}