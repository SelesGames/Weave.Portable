using SelesGames.Common;
using Outgoing = Weave.UserFeedAggregator.DTOs.ServerOutgoing;

namespace Weave.Converters
{
    public class DTOsToViewModelsConverters : 
        IConverter<Outgoing.Feed, ViewModels.Feed>,
        IConverter<Outgoing.NewsItem, ViewModels.NewsItem>
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
                TeaserImageUrl = o.TeaserImageUrl,
            };
        }

        public ViewModels.NewsItem Convert(Outgoing.NewsItem o)
        {
            return new ViewModels.NewsItem
            {
                Id = o.Id,
                Feed = new ViewModels.Feed { Id = o.FeedId },
                Title = o.Title,
                Link = o.Link,
                UtcPublishDateTime = o.UtcPublishDateTime,
                ImageUrl = o.ImageUrl,
                YoutubeId = o.YoutubeId,
                VideoUri = o.VideoUri,
                PodcastUri = o.PodcastUri,
                ZuneAppId = o.ZuneAppId,
                OriginalDownloadDateTime = o.OriginalDownloadDateTime,
                Image = new ViewModels.Image(),
                HasBeenViewed = o.HasBeenViewed,
                IsFavorite = o.IsFavorite,
            };
        }
    }
}
