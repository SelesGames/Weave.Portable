using System.Threading.Tasks;
using Weave.Services.Mobilizer.DTOs;

namespace Weave.Services.Mobilizer.Contracts
{
    public interface IMobilizerService
    {
        Task<MobilizerResult> Get(string url, bool stripLeadImage);
        Task Post(string url, MobilizerResult article);
    }
}
