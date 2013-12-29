using System.Runtime.Serialization;

namespace Weave.User.Service.DTOs.ServerIncoming
{
    [DataContract]
    public class ArticleDeleteTimes
    {
        [DataMember(Order= 1)]  public string ArticleDeletionTimeForMarkedRead { get; set; }
        [DataMember(Order= 2)]  public string ArticleDeletionTimeForUnread { get; set; }
    }
}
