using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;

namespace SelesGames.HttpClient
{
    class HttpResourceListener : IDisposable
    {
        System.Net.Http.HttpClient innerClient;
        string eTag, lastModified;
        string resourceUrl;

        HttpResponseMessage response;
        Action<HttpResponseMessage> onUpdated;
        Action<Exception> onException;

        bool isDisposed = false, isListening = false;

        public bool UseHttpHead { get; set; }
        public TimeSpan PollingInterval { get; set; }

        public event EventHandler Updated;

        public HttpResourceListener(System.Net.Http.HttpClient innerClient, string resourceUrl, Action<HttpResponseMessage> onUpdated, Action<Exception> onException = null)
        {
            this.innerClient = innerClient;
            this.resourceUrl = resourceUrl;
            this.onUpdated = onUpdated;
            this.onException = onException;

            PollingInterval = TimeSpan.FromMinutes(15);
        }

        public async void BeginListening()
        {
            if (isListening)
                return;

            isListening = true;

            while (true && !isDisposed)
            {
                await TryCheckResource();
                await Task.Delay(PollingInterval);
            }
        }




        #region Private helper functions

        async Task TryCheckResource()
        {
            try
            {
                var isUpdated = await CheckForUpdate();

                if (isUpdated)
                {
                    if (Updated != null)
                        Updated(this, EventArgs.Empty);

                    if (response != null && onUpdated != null)
                        onUpdated(response);
                }
            }
            catch (Exception ex)
            {
                if (onException != null)
                    onException(ex);
            }
        }

        /// <summary>
        /// Queries the resource url to see if there was an update
        /// </summary>
        /// <returns>A boolean specifying whether the resource was updated</returns>
        async Task<bool> CheckForUpdate()
        {
            #region CONDITIONAL GET

            if (!string.IsNullOrEmpty(eTag))
            {
                innerClient.DefaultRequestHeaders.TryAddWithoutValidation("If-None-Match", eTag);
            }

            if (!string.IsNullOrEmpty(lastModified))
            {
                innerClient.DefaultRequestHeaders.TryAddWithoutValidation("If-Modified-Since", lastModified);
            }

            #endregion


            if (UseHttpHead)
            {
                response = await innerClient.SendAsync(new HttpRequestMessage(HttpMethod.Head, resourceUrl)).ConfigureAwait(false);
            }
            else
            {
                response = await innerClient.GetAsync(resourceUrl).ConfigureAwait(false);
            }

            if (response.StatusCode == HttpStatusCode.NotModified)
            {
                return false;
            }

            else if (response.StatusCode == HttpStatusCode.OK)
            {
                SetConditionalHeaders();
                return true;
            }

            else
            {
                throw new Exception(response.StatusCode.ToString());
            }
        }

        void SetConditionalHeaders()
        {
            if (response == null)
                return;

            eTag = response.Headers.GetValueForHeader("ETag");
            lastModified = response.Content.Headers.GetValueForHeader("Last-Modified");
        }

        #endregion




        #region IDisposable

        public void Dispose()
        {
            isDisposed = true;
        }

        #endregion
    }
}