﻿using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace Common.Microsoft.Services
{
    /// <summary>
    /// For offline access of Live services.  Only valid when app uses the wl.offline_access wl.signin scopes.  A RefreshToken is used to keep the AccessToken up-to-date
    /// </summary>
    public class LiveOfflineAccessToken : LiveAccessToken
    {
        #region Endpoints and Private member variables

        const string
            MSA_TOKEN_REFRESH_URL = "https://login.live.com/oauth20_token.srf",
            TOKEN_REFRESH_CONTENT_TYPE = "application/x-www-form-urlencoded",
            TOKEN_REFRESH_REDIRECT_URI = "https://login.live.com/oauth20_desktop.srf",
            TOKEN_REFRESH_REQUEST_BODY = "client_id={0}&redirect_uri={1}&grant_type=refresh_token&refresh_token={2}";

        DateTimeOffset now;

        #endregion




        #region Public Properties

        public string ClientId { get; set; }
        public string AccessToken { get; set; }
        //public DateTimeOffset AccessTokenExpiration { get; set; }
        public string RefreshToken { get; set; }

        #endregion




        #region Constructor

        // Empty constructor to allow for serialization
        public LiveOfflineAccessToken() { }

        public LiveOfflineAccessToken(string clientId, string accessToken, /*DateTimeOffset accessTokenExpiration,*/ string refreshToken)
        {
            this.ClientId = clientId;
            this.AccessToken = accessToken;
            //this.AccessTokenExpiration = accessTokenExpiration;
            this.RefreshToken = refreshToken;
        }

        #endregion




        #region LiveAccessToken implementation

        public async override Task<string> GetAccessToken()
        {
            await AttemptAccessTokenRefresh();
            //await RefreshAccessTokenIfNecessary();
            return AccessToken;
        }

        #endregion




        #region Token refresh

        //async Task RefreshAccessTokenIfNecessary()
        //{
        //    now = DateTimeOffset.UtcNow;
        //    if (now > AccessTokenExpiration)
        //        await AttemptAccessTokenRefresh();
        //}

        async Task AttemptAccessTokenRefresh()
        {
            var content = string.Format(CultureInfo.InvariantCulture, TOKEN_REFRESH_REQUEST_BODY,
                        ClientId,
                        TOKEN_REFRESH_REDIRECT_URI,
                        RefreshToken);

            var createMessage = new HttpRequestMessage(HttpMethod.Post, MSA_TOKEN_REFRESH_URL)
            {
                Content = new StringContent(
                    content,
                    Encoding.UTF8,
                    TOKEN_REFRESH_CONTENT_TYPE)
            };

            var client = new HttpClient();
            var response = await client.SendAsync(createMessage);
            await ParseRefreshTokenResponse(response);
        }

        async Task ParseRefreshTokenResponse(HttpResponseMessage response)
        {
            if (response.StatusCode == HttpStatusCode.OK)
            {
                var responseString = await response.Content.ReadAsStringAsync();
                dynamic responseObject = JsonConvert.DeserializeObject(responseString);
                AccessToken = responseObject.access_token;
                var expiresInSeconds = (double)responseObject.expires_in;
                //AccessTokenExpiration = now.AddSeconds(expiresInSeconds);
                RefreshToken = responseObject.refresh_token;
            }
        }

        #endregion




        #region Override ToString

        public override string ToString()
        {
            return AccessToken;
        }

        #endregion
    }
}