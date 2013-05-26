using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Incoming = Weave.Article.Service.DTOs.ServerIncoming;
using Outgoing = Weave.Article.Service.DTOs.ServerOutgoing;

namespace Weave.Article.Service.Contracts
{
    public interface IWeaveArticleService
    {
        Task<bool> MarkRead(Guid userId, Incoming.SavedNewsItem newsItem);
        Task RemoveRead(Guid userId, Guid newsItemId);
        Task<List<Outgoing.SavedNewsItem>> GetRead(Guid userId, int take, int skip = 0);

        Task<bool> AddFavorite(Guid userId, Incoming.SavedNewsItem newsItem);
        Task RemoveFavorite(Guid userId, Guid newsItemId);
        Task<List<Outgoing.SavedNewsItem>> GetFavorites(Guid userId, int take, int skip = 0);
    }
}
