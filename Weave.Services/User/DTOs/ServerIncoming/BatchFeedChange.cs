using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace Weave.Services.User.DTOs.ServerIncoming
{
    [DataContract]
    public class BatchFeedChange
    {
        [DataMember(Order=1, IsRequired=false)]  public List<NewFeed> Added { get; set; }
        [DataMember(Order=2, IsRequired=false)]  public List<Guid> Removed { get; set; }
        [DataMember(Order=3, IsRequired=false)]  public List<UpdatedFeed> Updated { get; set; }
    }
}
