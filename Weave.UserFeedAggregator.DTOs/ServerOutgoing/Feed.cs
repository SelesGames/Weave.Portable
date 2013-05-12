using System;
using System.Runtime.Serialization;

namespace Weave.UserFeedAggregator.DTOs.ServerOutgoing
{
    /// <summary>
    /// DTO representing a single Feed object
    /// </summary>
    [DataContract]
    public class Feed
    {
        [DataMember(Order= 1)]  public Guid Id { get; set; }
        [DataMember(Order= 2)]  public string Name { get; set; }
        [DataMember(Order= 3)]  public string Uri { get; set; }
        [DataMember(Order= 4)]  public string Category { get; set; }
        [DataMember(Order= 5)]  public ArticleViewingType ArticleViewingType { get; set; }
        [DataMember(Order= 6)]  public int NewArticleCount { get; set; }
        [DataMember(Order= 7)]  public int UnreadCount { get; set; }
        [DataMember(Order= 8)]  public int TotalArticleCount { get; set; }
        [DataMember(Order= 9)]  public string TeaserImageUrl { get; set; }

        // delete for production
        [DataMember(Order=10)]  public DateTime LastRefreshedOn { get; set; }
        [DataMember(Order=11)]  public DateTime MostRecentEntrance { get; set; }
        [DataMember(Order=12)]  public DateTime PreviousEntrance { get; set; }
    }
}
