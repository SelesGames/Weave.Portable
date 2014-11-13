using System.Threading.Tasks;

namespace Common.Microsoft.Services
{
    public abstract class LiveAccessToken
    {
        public abstract Task<string> GetAccessToken();
    }
}