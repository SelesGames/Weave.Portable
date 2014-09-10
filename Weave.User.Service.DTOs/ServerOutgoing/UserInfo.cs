using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Weave.User.Service.DTOs.ServerOutgoing
{
    [DataContract]
    public class UserInfo
    {
        [DataMember(Order= 1)]  public Guid Id { get; set; }
        [DataMember(Order= 2)]  public int FeedCount { get; set; }
        [DataMember(Order= 3)]  public List<Feed> Feeds { get; set; }

        [DataMember(Order= 4)]  public DateTime PreviousLoginTime { get; set; }
        [DataMember(Order= 5)]  public DateTime CurrentLoginTime { get; set; }

        [DataMember(Order= 6)]  public List<NewsItem> LatestNews { get; set; }

        [DataMember(Order= 7)]  public string ArticleDeletionTimeForMarkedRead { get; set; }
        [DataMember(Order= 8)]  public string ArticleDeletionTimeForUnread { get; set; }

        [DataMember(Order= 9)]  public DateTime LastModified { get; set; }

        //[DataMember(Order=98)]  public TimeSpan DataStoreReadTime { get; set; }
        //[DataMember(Order=99)]  public TimeSpan DataStoreWriteTime { get; set; }

        [DataMember(Order=100)] public object Timings { get; set; }
    }
}