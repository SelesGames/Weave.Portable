﻿using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Incoming = Weave.UserFeedAggregator.DTOs.ServerIncoming;
using Outgoing = Weave.UserFeedAggregator.DTOs.ServerOutgoing;

namespace Weave.UserFeedAggregator.Contracts
{
    public interface IWeaveUserService
    {
        Task<Outgoing.UserInfo> AddUserAndReturnUserInfo(Incoming.UserInfo incomingUser);
        Task<Outgoing.UserInfo> GetUserInfo(Guid userId, bool refresh = false);
        
        Task<Outgoing.UserNews> GetNews(Guid userId, string category, bool refresh = false, int skip = 0, int take = 10);
        Task<Outgoing.UserNews> GetNews(Guid userId, Guid feedId, bool refresh = false, int skip = 0, int take = 10);

        Task<Outgoing.LiveTileNewsList> GetFeaturedNews(Guid userId, string category, int? take, bool refresh = false);
        Task<Outgoing.LiveTileNewsList> GetFeaturedNews(Guid userId, Guid feedId, int? take, bool refresh = false);

        Task AddFeed(Guid userId, Incoming.NewFeed feed);
        Task RemoveFeed(Guid userId, Guid feedId);
        Task UpdateFeed(Guid userId, Incoming.UpdatedFeed feed);
        Task BatchChange(Guid userId, Incoming.BatchFeedChange changeSet);


        Task MarkArticleRead(Guid userId, Guid feedId, Guid newsItemId);
        Task MarkArticleUnread(Guid userId, Guid feedId, Guid newsItemId);
       
        Task MarkArticlesSoftRead(Guid userId, List<Guid> newsItemIds);
    }
}