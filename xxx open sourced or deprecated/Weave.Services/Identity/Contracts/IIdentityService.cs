using System;
using System.Threading.Tasks;
using Weave.Services.Identity.DTOs;

namespace Weave.Services.Identity.Contracts
{
    public interface IIdentityService
    {
        Task<IdentityInfo> GetUserById(Guid userId);

        //Task<IdentityInfo> GetUserFromUserNameAndPassword(string username, string password);

        Task<IdentityInfo> SyncFacebook(Guid userId, string facebookToken);
        Task<IdentityInfo> SyncTwitter(Guid userId, string twitterToken);
        Task<IdentityInfo> SyncMicrosoft(Guid userId, string microsoftToken);
        Task<IdentityInfo> SyncGoogle(Guid userId, string googleToken);
    }
}
