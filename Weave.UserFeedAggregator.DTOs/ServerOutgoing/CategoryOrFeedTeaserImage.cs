using System;
using System.Runtime.Serialization;

namespace Weave.UserFeedAggregator.DTOs.ServerOutgoing
{
    [DataContract]
    public class CategoryOrFeedTeaserImage
    {
        [DataMember(Order=1, IsRequired=false)] public string Category { get; set; }
        [DataMember(Order=1, IsRequired=false)] public Guid? FeedId { get; set; }
        [DataMember(Order=2)]                   public string ImageUrl { get; set; }
    }
}