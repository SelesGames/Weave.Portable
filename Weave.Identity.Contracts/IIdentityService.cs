using System.Threading.Tasks;
using Weave.Identity.DTOs;

namespace Weave.Identity.Contracts
{
    public interface IIdentityService
    {
        Task<IdentityInfo> GetUserFromFacebookToken(string facebookToken);
        Task<IdentityInfo> GetUserFromTwitterToken(string twitterToken);
        Task<IdentityInfo> GetUserFromMicrosoftToken(string microsoftToken);
        Task<IdentityInfo> GetUserFromGoogleToken(string googleToken);
        Task Add(IdentityInfo user);
        Task Update(IdentityInfo user);
    }
}
