using Common.Compression;
using Common.Net.Http.Compression;
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
        readonly IRetryPolicy retryPolicy;

        public MediaTypeFormatterCollection Formatters { get { return formatters; } }

    


        #region Constructors

        public SmartHttpClient(CompressionSettings compressionSettings = CompressionSettings.OnRequest | CompressionSettings.OnContent)
            : this(new StandardMediaTypeFormatters(), ContentEncoderSettings.Default, CreateDefaultRetryPolicy(), compressionSettings) { }

        public SmartHttpClient(ContentEncoderSettings encoderSettings, CompressionSettings compressionSettings = CompressionSettings.OnRequest | CompressionSettings.OnContent)
            : this(new StandardMediaTypeFormatters(), encoderSettings, CreateDefaultRetryPolicy(), compressionSettings) { }

        SmartHttpClient(
            MediaTypeFormatterCollection formatters,
            ContentEncoderSettings encoderSettings,
            IRetryPolicy retryPolicy,
            CompressionSettings compressionSettings = CompressionSettings.OnRequest | CompressionSettings.OnContent)
        {
            this.formatters = formatters;
            this.encoderSettings = encoderSettings;
            this.retryPolicy = retryPolicy;
            this.compressionSettings = compressionSettings;
            this.acceptEncodings = Settings.CompressionHandlers.GetSupportedEncodings().ToArray();
        }

        static IRetryPolicy CreateDefaultRetryPolicy()
        {
            return new LinearRetry(TimeSpan.FromSeconds(1), 5);
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




        #region GET

        public Task<HttpResponseMessage> GetAsync(string url, CancellationToken cancellationToken)
        {
            return CreateClient().SendAsync(CreateRequest(HttpMethod.Get, url), cancellationToken);
        }

        public Task<HttpResponseMessage> GetAsync(string url)
        {
            return GetAsync(url, CancellationToken.None);
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            var response = await GetAsync(url, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            response.EnsureSuccessStatusCode2();

            return await response.ReadResponseContentAsync<T>(Formatters).ConfigureAwait(false);
        }

        public Task<T> GetAsync<T>(string url)
        {
            return GetAsync<T>(url, CancellationToken.None);
        }

        #endregion




        #region POST

        public Task<HttpResponseMessage> PostAsync<T>(string url, T obj, CancellationToken cancellationToken)
        {
            return CreateClient().SendAsync(CreateRequest(HttpMethod.Post, url, obj), cancellationToken);
        }

        public Task<HttpResponseMessage> PostAsync<T>(string url, T obj)
        {
            return PostAsync(url, obj, CancellationToken.None);
        }

        public async Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj, CancellationToken cancellationToken)
        {
            var response = await PostAsync(url, obj, cancellationToken).ConfigureAwait(false);

            cancellationToken.ThrowIfCancellationRequested();
            response.EnsureSuccessStatusCode2();

            return await response.ReadResponseContentAsync<TResult>(Formatters).ConfigureAwait(false);
        }

        public Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj)
        {
            return PostAsync<TPost, TResult>(url, obj, CancellationToken.None);
        }

        #endregion




        System.Net.Http.HttpClient CreateClient()
        {
            return new System.Net.Http.HttpClient(new RetryHandler(new HttpClientCompressionHandler(), retryPolicy));
        }
    }
}