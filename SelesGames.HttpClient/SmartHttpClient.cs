//using Common.Net.Http.Compression;
//using System.Net.Http;
//using System.Net.Http.Formatting;
//using System.Net.Http.Headers;
//using System.Threading;
//using System.Threading.Tasks;

//namespace SelesGames.HttpClient
//{
//    /// <summary>
//    /// A version of HttpClient that reads the Content-Type header to deserialize a web request response into an object.  
//    /// It also auto-decompresses responses.
//    /// 
//    /// By default, requests compressed Json objects
//    /// </summary>
//    public class SmartHttpClient : System.Net.Http.HttpClient
//    {
//        MediaTypeFormatterCollection formatters;
//        ContentEncoderSettings encoderSettings;
//        CompressionSettings compressionSettings;

//        public MediaTypeFormatterCollection Formatters { get { return formatters; } }




//        #region Constructors

//        public SmartHttpClient(CompressionSettings compressionSettings = CompressionSettings.OnRequest | CompressionSettings.OnContent)
//            : this(new StandardMediaTypeFormatters(), ContentEncoderSettings.Default, compressionSettings) { }

//        public SmartHttpClient(ContentEncoderSettings encoderSettings, CompressionSettings compressionSettings = CompressionSettings.OnRequest | CompressionSettings.OnContent)
//            : this(new StandardMediaTypeFormatters(), encoderSettings, compressionSettings) { }

//        SmartHttpClient(
//            MediaTypeFormatterCollection formatters,
//            ContentEncoderSettings encoderSettings,
//            CompressionSettings compressionSettings = CompressionSettings.OnRequest | CompressionSettings.OnContent)

//            : base(new HttpClientCompressionHandler())
//        {
//            this.formatters = formatters;
//            this.encoderSettings = encoderSettings;
//            this.compressionSettings = compressionSettings;

//            var accept = encoderSettings.Accept;
//            if (!string.IsNullOrEmpty(accept))
//                DefaultRequestHeaders.TryAddWithoutValidation("Accept", accept);

//            if (compressionSettings.HasFlag(CompressionSettings.OnRequest))
//                DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", new[] { "gzip", "deflate" });
//        }

//        #endregion




//        #region Get

//        public Task<T> GetAsync<T>(string url)
//        {
//            return GetAsync<T>(url, CancellationToken.None);
//        }

//        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
//        {
//            var response = await GetAsync(url, cancellationToken).ConfigureAwait(false);
//            response.EnsureSuccessStatusCode2();

//            return await response.ReadResponseContentAsync<T>(Formatters).ConfigureAwait(false);
//        }

//        #endregion




//        #region Post

//        public Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj)
//        {
//            return PostAsync<TPost, TResult>(url, obj, CancellationToken.None);
//        }

//        public async Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj, CancellationToken cancelToken)
//        {
//            var response = await PostAsync(url, obj, cancelToken);
//            response.EnsureSuccessStatusCode2();

//            return await response.ReadResponseContentAsync<TResult>(Formatters).ConfigureAwait(false);
//        }

//        public Task<HttpResponseMessage> PostAsync<T>(string url, T obj, CancellationToken cancelToken)
//        {
//            var mediaType = new MediaTypeHeaderValue(encoderSettings.ContentType);
//            var formatter = Formatters.FindWriteFormatter<T>(mediaType);

//            var content = new ObjectContent<T>(obj, formatter, mediaType);

//            return base.PostAsync(url, content, cancelToken);
//        }

//        #endregion
//    }
//}


using Common.Net.Http.Compression;
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

        public MediaTypeFormatterCollection Formatters { get { return formatters; } }




        #region Constructors

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
        }

        #endregion




        #region Get

        public Task GetAsync(string url)
        {
            return CreateClient().GetAsync(url, CancellationToken.None);
        }

        public Task GetAsync(string url, CancellationToken cancelToken)
        {
            return CreateClient().GetAsync(url, cancelToken);           
        }

        public Task<T> GetAsync<T>(string url)
        {
            return GetAsync<T>(url, CancellationToken.None);
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            var client = CreateClient();
            var response = await client.GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode2();

            return await response.ReadResponseContentAsync<T>(Formatters).ConfigureAwait(false);
        }

        #endregion




        #region Post

        public Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj)
        {
            return PostAsync<TPost, TResult>(url, obj, CancellationToken.None);
        }

        public async Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj, CancellationToken cancelToken)
        {
            var response = await PostAsync(url, obj, cancelToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode2();

            return await response.ReadResponseContentAsync<TResult>(Formatters).ConfigureAwait(false);
        }

        public Task<HttpResponseMessage> PostAsync<T>(string url, T obj, CancellationToken cancelToken)
        {
            var mediaType = new MediaTypeHeaderValue(encoderSettings.ContentType);
            var formatter = Formatters.FindWriteFormatter<T>(mediaType);

            var content = new ObjectContent<T>(obj, formatter, mediaType);

            //if (compressionSettings.HasFlag(CompressionSettings.OnContent))
            //    content.Headers.TryAddWithoutValidation("Content-Encoding", "gzip");

            var client = CreateClient();
            return client.PostAsync(url, content, cancelToken);
        }

        #endregion



        System.Net.Http.HttpClient CreateClient()
        {
            var client = new AutoCompressionClient();

            var accept = encoderSettings.Accept;
            if (!string.IsNullOrEmpty(accept))
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept", accept);

            if (compressionSettings.HasFlag(CompressionSettings.OnRequest))
                client.DefaultRequestHeaders.TryAddWithoutValidation("Accept-Encoding", new[] { "gzip", "deflate" });

            return client;
        }
    }
}