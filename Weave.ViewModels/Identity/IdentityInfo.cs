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
            bool userFound = false;

            try
            {
                var identityInfo = await service.GetUserById(UserId);
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

        public async Task LoadFromUsernameAndPassword()
        {
            var identityInfo = await service.GetUserFromUserNameAndPassword(UserName, PasswordHash);
            Load(identityInfo);
        }

        public Task LoadFromFacebook()
        {
            return LoadInnerImp(() => service.GetUserFromFacebookToken(FacebookAuthToken));
        }

        public Task LoadFromTwitter()
        {
            return LoadInnerImp(() => service.GetUserFromTwitterToken(TwitterAuthToken));
        }

        public Task LoadFromMicrosoft()
        {
            return LoadInnerImp(() => service.GetUserFromMicrosoftToken(MicrosoftAuthToken));
        }

        public Task LoadFromGoogle()
        {
            return LoadInnerImp(() => service.GetUserFromGoogleToken(GoogleAuthToken));
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

        // Implements basic pattern of load IdentityInfo - if not not found, Save
        async Task LoadInnerImp(Func<Task<DTOs.IdentityInfo>> loadAction)
        {
            bool userFound = false;

            try
            {
                var identityInfo = await loadAction();
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

            await service.Update(o);
        }

        #endregion
    }
}
