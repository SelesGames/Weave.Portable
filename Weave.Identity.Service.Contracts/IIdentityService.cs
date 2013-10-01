using System;
using System.Threading.Tasks;
using Weave.Identity.Service.DTOs;

namespace Weave.Identity.Service.Contracts
{
    public interface IIdentityService
    {
        Task<IdentityInfo> GetUserById(Guid userId);

        //Task<IdentityInfo> GetUserFromFacebookToken(string facebookToken);
        //Task<IdentityInfo> GetUserFromTwitterToken(string twitterToken);
        //Task<IdentityInfo> GetUserFromMicrosoftToken(string microsoftToken);
        //Task<IdentityInfo> GetUserFromGoogleToken(string googleToken);

        //Task<IdentityInfo> GetUserFromUserNameAndPassword(string username, string password);
        //Task Add(IdentityInfo user);
        //Task Update(IdentityInfo user);

        Task<IdentityInfo> SyncFacebook(Guid userId, string facebookToken);
        Task<IdentityInfo> SyncTwitter(Guid userId, string twitterToken);
        Task<IdentityInfo> SyncMicrosoft(Guid userId, string microsoftToken);
        Task<IdentityInfo> SyncGoogle(Guid userId, string googleToken);
    }
}
