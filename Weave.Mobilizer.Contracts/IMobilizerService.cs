using System.Threading.Tasks;
using Weave.Mobilizer.DTOs;

namespace Weave.Mobilizer.Contracts
{
    public interface IMobilizerService
    {
        Task<ReadabilityResult> Get(string url, bool stripLeadImage);
        Task Post(string url, ReadabilityResult article);
    }
}
