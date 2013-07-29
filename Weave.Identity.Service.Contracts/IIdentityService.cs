using System;
using System.Threading.Tasks;
using Weave.Identity.Service.DTOs;

namespace Weave.Identity.Service.Contracts
{
    public interface IIdentityService
    {
        Task<IdentityInfo> GetUserById(Guid userId);
        Task<IdentityInfo> GetUserFromFacebookToken(string facebookToken);
        Task<IdentityInfo> GetUserFromTwitterToken(string twitterToken);
        Task<IdentityInfo> GetUserFromMicrosoftToken(string microsoftToken);
        Task<IdentityInfo> GetUserFromGoogleToken(string googleToken);
        Task<IdentityInfo> GetUserFromUserNameAndPassword(string username, string password);
        Task Add(IdentityInfo user);
        Task Update(IdentityInfo user);
    }
}
