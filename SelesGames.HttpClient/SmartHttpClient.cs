using SelesGames.HttpClient.SerializerModules;
using System;
using System.IO;
using System.Net;
using System.Net.Http;
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
    public class SmartHttpClient : System.Net.Http.HttpClient
    {
        ContentSerializerCollection formatters;
        ContentEncoderSettings encoderSettings;




        #region Constructors

        public SmartHttpClient()
            : this(CreateDefaultFormatters(), ContentEncoderSettings.Default) { }

        public SmartHttpClient(ContentEncoderSettings encoderSettings)
            : this(CreateDefaultFormatters(), encoderSettings) { }

        public SmartHttpClient(
            ContentSerializerCollection formatters,
            ContentEncoderSettings encoderSettings)
            
            : base(CreateHandler())
        {
            this.formatters = formatters;
            this.encoderSettings = encoderSettings;

            var accept = encoderSettings.Accept;
            if (!string.IsNullOrEmpty(accept))
                DefaultRequestHeaders.TryAddWithoutValidation("Accept", accept);
        }

        static ContentSerializerCollection CreateDefaultFormatters()
        {
            var serializers = new ContentSerializerCollection();
            serializers.Add(new JsonSerializer());
            serializers.Add(new ProtobufSerializer());
            return serializers;
        }

        static HttpClientHandler CreateHandler()
        {
            var handler = new HttpClientHandler();
            if (handler.SupportsAutomaticDecompression)
            {
                handler.AutomaticDecompression = DecompressionMethods.GZip |
                                                 DecompressionMethods.Deflate;
            }
            return handler;
        }

        #endregion




        #region Get

        public Task<T> GetAsync<T>(string url)
        {
            return GetAsync<T>(url, CancellationToken.None);
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
            var response = await GetAsync(url, cancellationToken).ConfigureAwait(false);
            response.EnsureSuccessStatusCode();

            return await ReadObjectFromResponseMessage<T>(response);
        }

        #endregion




        #region Post

        public Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj)
        {
            return PostAsync<TPost, TResult>(url, obj, CancellationToken.None);
        }

        public Task PostAsync<TPost>(string url, TPost obj)
        {
            return PostAsync(url, obj, CancellationToken.None);
        }

        public async Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj, CancellationToken cancelToken)
        {
            var response = await PostAsync(url, obj, cancelToken);
            return await ReadObjectFromResponseMessage<TResult>(response).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T obj, CancellationToken cancelToken)
        {
            var mediaType = new MediaTypeHeaderValue(encoderSettings.ContentType);
            var serializer = formatters.FindReader(mediaType);

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                ms.Position = 0;

                var content = new StreamContent(ms);
                content.Headers.ContentType = mediaType;

                var response = await PostAsync(url, content, cancelToken).ConfigureAwait(false);

                response.EnsureSuccessStatusCode();
                return response;
            }
        }

        #endregion




        #region helper methods

        async Task<T> ReadObjectFromResponseMessage<T>(HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();

                T result;

                var contentType = TryGetContentType(response);
                var serializer = formatters.FindReader(contentType);

                if (serializer == null)
                    throw new Exception(string.Format("No serializer found for content type: {0}", contentType));

                using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
                {
                    result = serializer.ReadObject<T>(stream);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new ResponseException(ex, response);
            }
        }

        MediaTypeHeaderValue TryGetContentType(HttpResponseMessage response)
        {
            if (response == null || response.Content == null || response.Content.Headers == null)
                return null;

            return response.Content.Headers.ContentType;
        }

        //MediaTypeFormatter FindWriteFormatter<T>(MediaTypeHeaderValue mediaType)
        //{
        //    MediaTypeFormatter formatter = null;

        //    var type = typeof(T);
        //    if (mediaType != null)
        //    {
        //        formatter = formatters.FindWriter(type, mediaType);
        //    }

        //    if (formatter == null)
        //        throw new Exception(string.Format("unable to find a valid MediaTypeFormatter that matches {0}", mediaType));

        //    return formatter;
        //}

        //static MediaTypeFormatterCollection CreateDefaultMediaTypeFormatters()
        //{
        //    var collection = new MediaTypeFormatterCollection();
        //    collection.Add(new SelesGames.WebApi.Protobuf.ProtobufFormatter());
        //    return collection;
        //}

        #endregion
    }
}