using System;
using Weave.Mobilizer.DTOs;

namespace Weave.ViewModels.Mobilizer
{
    internal class ResultCombiner
    {
        NewsItem newsItem;
        MobilizerResult mobilizerResult;

        public ResultCombiner(NewsItem newsItem, MobilizerResult mobilizerResult)
        {
            if (newsItem == null) throw new ArgumentNullException("newsItem");
            if (mobilizerResult == null) throw new ArgumentNullException("mobilizerResult");

            this.newsItem = newsItem;
            this.mobilizerResult = mobilizerResult;
        }

        public MobilizedArticle Combine()
        {
            var heroImage = SelectBestImage();

            var fullDate =
                newsItem.LocalDateTime.ToString("dddd, MMMM dd • h:mm") +
                newsItem.LocalDateTime.ToString("tt").ToLowerInvariant();

            return new MobilizedArticle
            {
                Title = newsItem.Title,
                HeroImageUrl = heroImage,
                Link = newsItem.Link,
                Source = newsItem.OriginalSource,
                FullDate = fullDate,
                CombinedPublicationAndDate = newsItem.FormattedForMainPageSourceAndDate,
                ContentHtml = mobilizerResult.content,
                Author = mobilizerResult.author,
            };
        }

        string SelectBestImage()
        {
            if (!string.IsNullOrWhiteSpace(mobilizerResult.lead_image_url))
                return mobilizerResult.lead_image_url;

            if (newsItem.HasImage)
                return newsItem.HighestQualityImageUrl;

            return null;
        }
    }
}
