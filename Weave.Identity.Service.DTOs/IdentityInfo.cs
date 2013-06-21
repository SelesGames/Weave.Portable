using System;

namespace Weave.Identity.Service.DTOs
{
    public class IdentityInfo
    {
        public Guid UserId { get; set; }
        public string UserName { get; set; }
        public string PasswordHash { get; set; }
        public string FacebookAuthToken { get; set; }
        public string TwitterAuthToken { get; set; }
        public string MicrosoftAuthToken { get; set; }
        public string GoogleAuthToken { get; set; }
    }
}
