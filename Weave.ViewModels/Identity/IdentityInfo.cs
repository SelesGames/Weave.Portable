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
            var identityInfo = await service.GetUserById(UserId);
            Load(identityInfo);
        }

        public async Task LoadFromUsernameAndPassword()
        {
            var identityInfo = await service.GetUserFromUserNameAndPassword(UserName, PasswordHash);
            Load(identityInfo);
        }

        public async Task LoadFromFacebook()
        {
            bool userFound = false;

            try
            {
                var identityInfo = await service.GetUserFromFacebookToken(FacebookAuthToken);
                Load(identityInfo);
                userFound = true;
            }
            catch (NoMatchingUserException)
            {
            }

            if (userFound)
                return;

            await Save();
        }

        public async Task LoadFromTwitter()
        {
            bool userFound = false;

            try
            {
                var identityInfo = await service.GetUserFromTwitterToken(TwitterAuthToken);
                Load(identityInfo);
                userFound = true;
            }
            catch (NoMatchingUserException)
            {
            }

            if (userFound)
                return;

            await Save();
        }

        public async Task LoadFromMicrosoft()
        {
            bool userFound = false;

            try
            {
                var identityInfo = await service.GetUserFromMicrosoftToken(MicrosoftAuthToken);
                Load(identityInfo);
                userFound = true;
            }
            catch (NoMatchingUserException)
            {
            }

            if (userFound)
                return;

            await Save();
        }

        public async Task LoadFromGoogle()
        {
            bool userFound = false;

            try
            {
                var identityInfo = await service.GetUserFromGoogleToken(GoogleAuthToken);
                Load(identityInfo);
                userFound = true;
            }
            catch (NoMatchingUserException)
            {
            }

            if (userFound)
                return;

            await Save();
        }

        async Task Save()
        {
            var o = new DTOs.IdentityInfo
            {
                UserId = UserId,
                UserName = UserName,
                PasswordHash = PasswordHash,
                FacebookAuthToken = FacebookAuthToken,
                TwitterAuthToken = TwitterAuthToken,
                MicrosoftAuthToken = MicrosoftAuthToken,
                GoogleAuthToken = GoogleAuthToken,
            };

            await service.Add(o);
        }



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
