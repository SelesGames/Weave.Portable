using System.Runtime.Serialization;

namespace Weave.Services.User.DTOs.ServerIncoming
{
    [DataContract]
    public class ArticleDeleteTimes
    {
        [DataMember(Order= 1)]  public string ArticleDeletionTimeForMarkedRead { get; set; }
        [DataMember(Order= 2)]  public string ArticleDeletionTimeForUnread { get; set; }
    }
}