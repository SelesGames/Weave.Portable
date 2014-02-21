using System.Threading.Tasks;
using Weave.Mobilizer.Contracts;

namespace Weave.ViewModels.Mobilizer
{
    public class MobilizedArticleRepository
    {
        IMobilizerService service;

        public MobilizedArticleRepository(IMobilizerService service)
        {
            this.service = service;
        }

        public async Task<MobilizedArticle> Get(NewsItem key)
        {
            var newsItem = key;
            var mobilizerResult = await service.Get(newsItem.Link, stripLeadImage: true);
            var coalescer = new ResultCombiner(newsItem, mobilizerResult);
            return coalescer.Combine();
        }
    }
}
