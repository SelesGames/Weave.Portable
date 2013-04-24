using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Weave.UserFeedAggregator.DTOs.ServerOutgoing
{
    [DataContract]
    public class UserInfo
    {
        [DataMember(Order= 1)]  public Guid Id { get; set; }
        [DataMember(Order= 2)]  public int FeedCount { get; set; }
        [DataMember(Order= 3)]  public List<FeedInfo> Feeds { get; set; }

        [DataMember(Order= 4)]  public DateTime PreviousLoginTime { get; set; }
        [DataMember(Order= 5)]  public DateTime CurrentLoginTime { get; set; }

        [DataMember(Order= 6)]  public List<NewsItem> LatestNews { get; set; }

        [DataMember(Order= 7)]  public List<CategoryOrFeedTeaserImage> TeaserImages { get; set; }


        [DataMember(Order=98)]  public TimeSpan DataStoreReadTime { get; set; }
        [DataMember(Order=99)]  public TimeSpan DataStoreWriteTime { get; set; }
    }
}