using Common.Compression;
using SelesGames.HttpClient.RetryPolicies;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace SelesGames.HttpClient
{
    /// <summary>
    /// A version of HttpClient that reads the Content-Type header to deserialize a web request response into an object.  
    /// It also auto-decompresses responses.
    /// 
    /// By default, requests compressed Json objects
    /// </summary>
    public class SmartHttpClient
    {
        readonly MediaTypeFormatterCollection formatters;
        readonly ContentEncoderSettings encoderSettings;
        readonly CompressionSettings compressionSettings;
        readonly IEnumerable<string> acceptEncodings;

        public MediaTypeFormatterCollection Formatters { get { return formatters; } }
        public Func<System.Net.Http.HttpClient> HttpClientFactory { get; set; }

        public static IRetryPolicy DefaultRetryPolicy { get; set; }

    


        #region Constructors

        static SmartHttpClient()
        {
            DefaultRetryPolicy = new LinearRetry(TimeSpan.FromSeconds(1), 3);
        }

        public SmartHttpClient(CompressionSettings compressionSettings = CompressionSettings.OnRequest | CompressionSettings.OnContent)
            : this(new StandardMediaTypeFormatters(), ContentEncoderSettings.Default, compressionSettings) { }

        public SmartHttpClient(ContentEncoderSettings encoderSettings, CompressionSettings compressionSettings = CompressionSettings.OnRequest | CompressionSettings.OnContent)
            : this(new StandardMediaTypeFormatters(), encoderSettings, compressionSettings) { }

        SmartHttpClient(
            MediaTypeFormatterCollection formatters,
            ContentEncoderSettings encoderSettings,
            CompressionSettings compressionSettings = CompressionSettings.OnRequest | CompressionSettings.OnContent)
        {
            this.formatters = formatters;
            this.encoderSettings = encoderSettings;
            this.compressionSettings = compressionSettings;
            this.acceptEncodings = Settings.CompressionHandlers.GetSupportedEncodings().ToArray();

            HttpClientFactory = () => new AutoCompressionClient();
        }

        #endregion




        #region Create the HttpRequestMessage based on settings

        public HttpRequestMessage CreateRequest(HttpMethod method, string url)
        {
            var request = new HttpRequestMessage(method, url);

            var accept = encoderSettings.Accept;
            if (!string.IsNullOrEmpty(accept))
                request.Headers.TryAddWithoutValidation("Accept", accept);

            if (compressionSettings.HasFlag(CompressionSettings.OnRequest))
                request.Headers.TryAddWithoutValidation("Accept-Encoding", acceptEncodings);

            return request;
        }

        public HttpRequestMessage CreateRequest<T>(HttpMethod method, string url, T obj)
        {
            var request = CreateRequest(method, url);

            var mediaType = new MediaTypeHeaderValue(encoderSettings.ContentType);
            var formatter = Formatters.FindWriteFormatter<T>(mediaType);

            var content = new ObjectContent<T>(obj, formatter, mediaType);

            //if (compressionSettings.HasFlag(CompressionSettings.OnContent))
            //    content.Headers.TryAddWithoutValidation("Content-Encoding", "gzip");

            request.Content = content;
            return request;
        }

        #endregion




        #region Send the Request

        internal async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, RetryContext retryContext, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        {
            if (retryPolicy == null)
                throw new ArgumentNullException("retryPolicy in SmartHttpClient.SendAsync");

            if (retryContext != null)
            {
                var retryInfo = retryPolicy.Evaluate(retryContext);
                if (!retryInfo.ShouldRetry)
                    throw new ErrorResponseException(retryContext.LastRequestResult);

                await Task.Delay(retryInfo.RetryInterval).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

#if DEBUG
            System.Diagnostics.Debug.WriteLine(
                "Sending request... (attempt {2})\r\n{0}\r\n{1}\r\n\r\n",
                request.ToString(),
                request.Content,
                retryContext == null ? 0 : retryContext.CurrentRetryCount);
#endif

            var client = HttpClientFactory();
            var response = await client.SendAsync(request, cancellationToken).ConfigureAwait(false);
            cancellationToken.ThrowIfCancellationRequested();

            if (response.IsSuccessStatusCode)
            {
                return response;
            }

            else
            {
                retryContext = retryContext ?? new RetryContext();

                retryContext.CurrentRetryCount++;
                retryContext.LastRequestResult = response;

                request = Copy(request);
                return await SendAsync(request, retryContext, retryPolicy, cancellationToken);
            }
        }

        HttpRequestMessage Copy(HttpRequestMessage original)
        {
            var request = new HttpRequestMessage(original.Method, original.RequestUri)
            {
                Content = original.Content,
                Version = original.Version,
            };

            foreach (var header in original.Headers)
            {
                request.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            foreach (var property in original.Properties)
            {
                request.Properties.Add(property.Key, property.Value);
            }

            return request;
        }

        #endregion




        #region GET

        public Task<HttpResponseMessage> GetAsync(string url, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        {
            return SendAsync(CreateRequest(HttpMethod.Get, url), null, retryPolicy, cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(string url, IRetryPolicy retryPolicy)
        {
            return GetAsync(url, retryPolicy, CancellationToken.None);
        }
        
        public Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken)
        {
            return GetAsync(url, DefaultRetryPolicy, cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return GetAsync(url, DefaultRetryPolicy, CancellationToken.None);
        }

        public async Task<T> GetAsync<T>(string url, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        {
            var response = await GetAsync(url, retryPolicy, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            response.EnsureSuccessStatusCode2();

            return await response.ReadResponseContentAsync<T>(Formatters).ConfigureAwait(false);
        }

        public Task<T> GetAsync<T>(string url, IRetryPolicy retryPolicy)
        {
            return GetAsync<T>(url, retryPolicy, CancellationToken.None);
        }

        public Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            return GetAsync<T>(url, DefaultRetryPolicy, cancellationToken);
        }

        public Task<T> GetAsync<T>(string url)
        {
            return GetAsync<T>(url, DefaultRetryPolicy, CancellationToken.None);
        }

        #endregion




        #region POST

        public Task<HttpResponseMessage> PostAsync<T>(string url, T obj, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        {
            return SendAsync(CreateRequest(HttpMethod.Post, url, obj), null, retryPolicy, cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsync<T>(string url, T obj, CancellationToken cancellationToken)
        {
            return PostAsync(url, obj, DefaultRetryPolicy, cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsync<T>(string url, T obj, IRetryPolicy retryPolicy)
        {
            return PostAsync(url, obj, retryPolicy, CancellationToken.None);
        }

        public Task<HttpResponseMessage> PostAsync<T>(string url, T obj)
        {
            return PostAsync(url, obj, DefaultRetryPolicy, CancellationToken.None);
        }

        public async Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj, IRetryPolicy retryPolicy, CancellationToken cancellationToken)
        {
            var response = await PostAsync(url, obj, retryPolicy, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            response.EnsureSuccessStatusCode2();

            return await response.ReadResponseContentAsync<TResult>(Formatters).ConfigureAwait(false);
        }

        public Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj, CancellationToken cancellationToken)
        {
            return PostAsync<TPost, TResult>(url, obj, DefaultRetryPolicy, cancellationToken);
        }

        public Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj, IRetryPolicy retryPolicy)
        {
            return PostAsync<TPost, TResult>(url, obj, retryPolicy, CancellationToken.None);
        }

        public Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj)
        {
            return PostAsync<TPost, TResult>(url, obj, DefaultRetryPolicy, CancellationToken.None);
        }

        #endregion
    }
}