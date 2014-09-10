using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Weave.User.Service.DTOs.ServerOutgoing
{
    /// <summary>
    /// DTO representing a list of the User's Feeds
    /// </summary>
    [DataContract]
    public class FeedsInfoList
    {
        [DataMember(Order= 1)]  public Guid UserId { get; set; }
        [DataMember(Order= 2)]  public int TotalFeedCount { get; set; }

        [DataMember(Order= 3, IsRequired=false)]  
        public List<CategoryInfo> Categories { get; set; }

        [DataMember(Order= 4)]  public List<Feed> Feeds { get; set; }
        [DataMember(Order= 5)]  public int NewArticleCount { get; set; }
        [DataMember(Order= 6)]  public int UnreadArticleCount { get; set; }
        [DataMember(Order= 7)]  public int TotalArticleCount { get; set; }


        //[DataMember(Order=98)]  public TimeSpan DataStoreReadTime { get; set; }
        //[DataMember(Order=99)]  public TimeSpan DataStoreWriteTime { get; set; }

        [DataMember(Order=100)] public object Timings { get; set; }
    }
}