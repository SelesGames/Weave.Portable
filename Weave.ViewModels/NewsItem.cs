using System;
using Weave.ViewModels.Extensions;

namespace Weave.ViewModels
{
    public class NewsItem : ViewModelBase
    {
        string utcPublishDateTime;
        bool isFavorite, hasBeenViewed;


        public Guid Id { get; set; }
        public Feed Feed { get; set; }
        public string Title { get; set; }
        public string Link { get; set; }
        public string UtcPublishDateTime 
        {
            get { return utcPublishDateTime; } 
            set
            {
                utcPublishDateTime = value;
                ParseLocalAndUniversalDateTimes();
            }
        }

        public string ImageUrl { get; set; }
        public string YoutubeId { get; set; }
        public string VideoUri { get; set; }
        public string PodcastUri { get; set; }
        public string ZuneAppId { get; set; }
        public DateTime OriginalDownloadDateTime { get; set; }
        public Image Image { get; set; }


        public DateTime LocalDateTime { get; private set; }
        public DateTime UniversalDateTime { get; private set; }

        public bool IsNew { get; set; }
        public bool IsFavorite
        {
            get { return isFavorite; }
            set
            {
                isFavorite = value;
                Raise("DisplayState");
            }
        }

        public bool HasBeenViewed
        {
            get { return hasBeenViewed; }
            set
            {
                hasBeenViewed = value;
                Raise("DisplayState");
            }
        }



        #region Public derived properties

        public bool IsDisplayedAsNew
        {
            get { return !HasBeenViewed && IsNew; }
        }

        public enum ColoringDisplayState
        {
            Normal,
            Viewed,
            Favorited
        }

        public ColoringDisplayState DisplayState
        {
            get
            {
                if (IsFavorite)
                    return ColoringDisplayState.Favorited;
                else
                    return HasBeenViewed ? ColoringDisplayState.Viewed : ColoringDisplayState.Normal;
            }
        }

        public string OriginalSource
        {
            get { return Feed.Name; }
        }

        public string OriginalFeedUri
        {
            get { return Feed.Uri; }
        }

        public string PublishDate
        {
            get { return LocalDateTime.ElapsedTime(); }
        }

        public string FormattedForMainPageSourceAndDate
        {
            get
            {
                return string.Format("{0} • {1}", Feed.Name, LocalDateTime.ElapsedTime());
            }
        }

        public bool HasImage
        {
            get
            {
                return !string.IsNullOrEmpty(ImageUrl) &&
                    Uri.IsWellFormedUriString(ImageUrl, UriKind.Absolute);
            }
        }

        public string HighestQualityImageUrl
        {
            get
            {
                if (Image != null && !string.IsNullOrWhiteSpace(Image.OriginalUrl))
                    return Image.OriginalUrl;
                else
                    return ImageUrl;
            }
        }

        #endregion




        #region private helper methods

        void ParseLocalAndUniversalDateTimes()
        {
            try
            {
                UniversalDateTime = DateTime.Parse(utcPublishDateTime, null, System.Globalization.DateTimeStyles.AdjustToUniversal);
                LocalDateTime = UniversalDateTime.ToLocalTime();
            }
            catch
            {
                LocalDateTime = DateTime.Now;
                UniversalDateTime = DateTime.UtcNow;
            }
        }

        #endregion




        public override string ToString()
        {
            return string.Format("{0}: {1}   from {2}", Feed.Category, Title, OriginalFeedUri);
        }
    }
}