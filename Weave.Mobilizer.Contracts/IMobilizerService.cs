using System.Threading.Tasks;
using Weave.Mobilizer.DTOs;

namespace Weave.Mobilizer.Contracts
{
    public interface IMobilizerService
    {
        Task<MobilizerResult> Get(string url, bool stripLeadImage);
        Task Post(string url, MobilizerResult article);
    }
}
