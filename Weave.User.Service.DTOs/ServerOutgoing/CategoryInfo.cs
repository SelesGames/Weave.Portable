using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Weave.User.Service.DTOs.ServerOutgoing
{
    [DataContract]
    public class CategoryInfo
    {
        [DataMember(Order= 1)]  public string Category { get; set; }
        [DataMember(Order= 2)]  public int TotalFeedCount { get; set; }
        [DataMember(Order= 3)]  public List<Feed> Feeds { get; set; }
        [DataMember(Order= 4)]  public int NewArticleCount { get; set; }
        [DataMember(Order= 5)]  public int UnreadArticleCount { get; set; }
        [DataMember(Order= 6)]  public int TotalArticleCount { get; set; }
    }
}