using System;
using System.Threading.Tasks;
using Weave.Identity.Service.Contracts;
using DTOs = Weave.Identity.Service.DTOs;

namespace Weave.ViewModels.Identity
{
    public class IdentityInfo : ViewModelBase
    {
        #region Private member variables

        IIdentityService service;

        Guid userId;

        string
            facebookAuthToken,
            twitterAuthToken,
            microsoftAuthToken,
            googleAuthToken;

        #endregion




        public event EventHandler UserIdChanged;


        public IdentityInfo(IIdentityService service)
        {
            this.service = service;
        }

        public async Task LoadFromUserId()
        {
            try
            {
                var identityInfo = await service.GetUserById(UserId);
                Load(identityInfo);
            }
            catch (NoMatchingUserException) { }
        }

        public async Task SyncFacebook(string facebookToken)
        {
            var identityInfo = await service.SyncFacebook(UserId, facebookToken);
            Load(identityInfo);
        }

        public async Task SyncTwitter(string twitterToken)
        {
            var identityInfo = await service.SyncTwitter(UserId, twitterToken);
            Load(identityInfo);
        }

        public async Task SyncMicrosoft(string microsoftToken)
        {
            var identityInfo = await service.SyncMicrosoft(UserId, microsoftToken);
            Load(identityInfo);
        }

        public async Task SyncGoogle(string googleToken)
        {
            var identityInfo = await service.SyncGoogle(UserId, googleToken);
            Load(identityInfo);
        }




        #region Public Properties

        public Guid UserId 
        {
            get { return userId; }
            set
            {
                if (userId != value)
                {
                    userId = value;
                    if (UserIdChanged != null)
                        UserIdChanged(this, EventArgs.Empty);
                }        
            }
        }

        public string FacebookAuthToken
        {
            get { return facebookAuthToken; }
            private set { facebookAuthToken = value; Raise("FacebookAuthToken", "IsFacebookLinked"); }
        }

        public string TwitterAuthToken
        {
            get { return twitterAuthToken; }
            private set { twitterAuthToken = value; Raise("TwitterAuthToken", "IsTwitterLinked"); }
        }

        public string MicrosoftAuthToken
        {
            get { return microsoftAuthToken; }
            private set { microsoftAuthToken = value; Raise("MicrosoftAuthToken", "IsMicrosoftLinked"); }
        }

        public string GoogleAuthToken
        {
            get { return googleAuthToken; }
            private set { googleAuthToken = value; Raise("GoogleAuthToken", "IsGoogleLinked"); }
        }

        #endregion




        #region Derived Readonly Properties

        public bool IsFacebookLoginEnabled
        {
            get { return string.IsNullOrEmpty(FacebookAuthToken); }
        }

        public bool IsTwitterLoginEnabled
        {
            get { return string.IsNullOrEmpty(TwitterAuthToken); }
        }

        public bool IsMicrosoftLoginEnabled
        {
            get { return string.IsNullOrEmpty(MicrosoftAuthToken); }
        }
        
        public bool IsGoogleLoginEnabled
        {
            get { return string.IsNullOrEmpty(GoogleAuthToken); }
        }

        #endregion




        #region private helper methods

        void Load(DTOs.IdentityInfo o)
        {
            UserId = o.UserId;
            FacebookAuthToken = o.FacebookAuthToken;
            TwitterAuthToken = o.TwitterAuthToken;
            MicrosoftAuthToken = o.MicrosoftAuthToken;
            GoogleAuthToken = o.GoogleAuthToken;
        }

        #endregion
    }
}