using SelesGames.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Outgoing = Weave.UserFeedAggregator.DTOs.ServerOutgoing;

namespace Weave.Converters
{
    public class DTOsToViewModelsConverters : 
        IConverter<Outgoing.Feed, ViewModels.Feed>,
        IConverter<Outgoing.NewsItem, ViewModels.NewsItem>,
        IConverter<Outgoing.CategoryOrFeedTeaserImage, ViewModels.CategoryOrFeedTeaserImage>
    {
        public static readonly DTOsToViewModelsConverters Current = new DTOsToViewModelsConverters();

        public ViewModels.Feed Convert(Outgoing.Feed o)
        {
            return new ViewModels.Feed
            {
                Id = o.Id,
                Name = o.Name,
                Uri = o.Uri,
                Category = o.Category,
                ArticleViewingType = (ViewModels.ArticleViewingType)o.ArticleViewingType,
            };
        }

        public ViewModels.NewsItem Convert(Outgoing.NewsItem input)
        {
            throw new NotImplementedException();
        }

        public ViewModels.CategoryOrFeedTeaserImage Convert(Outgoing.CategoryOrFeedTeaserImage input)
        {
            throw new NotImplementedException();
        }
    }
}
