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
            userName,
            passwordHash,
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

        public async Task LoadFromFacebook()
        {
            var identityInfo = await service.SyncFacebook(UserId, FacebookAuthToken);
            Load(identityInfo);
        }

        public async Task LoadFromTwitter()
        {
            var identityInfo = await service.SyncTwitter(UserId, TwitterAuthToken);
            Load(identityInfo);
        }

        public async Task LoadFromMicrosoft()
        {
            var identityInfo = await service.SyncMicrosoft(UserId, MicrosoftAuthToken);
            Load(identityInfo);
        }

        public async Task LoadFromGoogle()
        {
            var identityInfo = await service.SyncGoogle(UserId, GoogleAuthToken);
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

        public string UserName
        {
            get { return userName; }
            set { userName = value; Raise("UserName", "IsAccountLinked"); }
        }

        public string PasswordHash
        {
            get { return passwordHash; }
            set { passwordHash = value; Raise("PasswordHash"); }
        }

        public string FacebookAuthToken
        {
            get { return facebookAuthToken; }
            set { facebookAuthToken = value; Raise("FacebookAuthToken", "IsFacebookLinked"); }
        }

        public string TwitterAuthToken
        {
            get { return twitterAuthToken; }
            set { twitterAuthToken = value; Raise("TwitterAuthToken", "IsTwitterLinked"); }
        }

        public string MicrosoftAuthToken
        {
            get { return microsoftAuthToken; }
            set { microsoftAuthToken = value; Raise("MicrosoftAuthToken", "IsMicrosoftLinked"); }
        }

        public string GoogleAuthToken
        {
            get { return googleAuthToken; }
            set { googleAuthToken = value; Raise("GoogleAuthToken", "IsGoogleLinked"); }
        }

        #endregion




        #region Derived Readonly Properties

        public bool IsAccountLinked
        {
            get { return !string.IsNullOrEmpty(UserName); }
        }

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
            UserName = o.UserName;
            PasswordHash = o.PasswordHash;
            FacebookAuthToken = o.FacebookAuthToken;
            TwitterAuthToken = o.TwitterAuthToken;
            MicrosoftAuthToken = o.MicrosoftAuthToken;
            GoogleAuthToken = o.GoogleAuthToken;
        }

        #endregion
    }
}