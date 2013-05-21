using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Weave.UserFeedAggregator.DTOs.ServerOutgoing
{
    /// <summary>
    /// DTO representing a list of the User's Feeds
    /// </summary>
    [DataContract]
    public class FeedsInfoList
    {
        [DataMember(Order= 1)]  public Guid UserId { get; set; }
        [DataMember(Order= 2)]  public int FeedCount { get; set; }
        [DataMember(Order= 3)]  public List<Feed> Feeds { get; set; }
        [DataMember(Order= 4)]  public int NewArticleCount { get; set; }
        [DataMember(Order= 5)]  public int UnreadArticleCount { get; set; }
        [DataMember(Order= 6)]  public int TotalArticleCount { get; set; }
    }
}
