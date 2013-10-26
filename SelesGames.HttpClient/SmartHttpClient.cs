﻿using Common.Net.Http.Compression;
using SelesGames.HttpClient.SerializerModules;
using System;
using System.IO;
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
            return new HttpClientCompressionHandler();
        }

        #endregion




        #region Get

        public Task<T> GetAsync<T>(string url)
        {
            return GetAsync<T>(url, CancellationToken.None);
        }

        public async Task<T> GetAsync<T>(string url, CancellationToken cancellationToken)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("HTTP GET : {0}", url);
#endif

            var response = await GetAsync(url, cancellationToken).ConfigureAwait(false);
            EnsureSuccessStatusCode(response);

            return await ReadObjectFromResponseMessage<T>(response);
        }

        #endregion




        #region Post

        public Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj)
        {
            return PostAsync<TPost, TResult>(url, obj, CancellationToken.None);
        }

        public async Task<TResult> PostAsync<TPost, TResult>(string url, TPost obj, CancellationToken cancelToken)
        {
            var response = await PostAsync(url, obj, cancelToken);
            EnsureSuccessStatusCode(response);

            return await ReadObjectFromResponseMessage<TResult>(response).ConfigureAwait(false);
        }

        public async Task<HttpResponseMessage> PostAsync<T>(string url, T obj, CancellationToken cancelToken)
        {
#if DEBUG
            System.Diagnostics.Debug.WriteLine("HTTP POST : {0}", url);
#endif
            var mediaType = new MediaTypeHeaderValue(encoderSettings.ContentType);
            var serializer = formatters.FindReader(mediaType);

            using (var ms = new MemoryStream())
            {
                serializer.WriteObject(ms, obj);
                ms.Position = 0;

                var content = new StreamContent(ms);
                content.Headers.ContentType = mediaType;

                var response = await base.PostAsync(url, content, cancelToken).ConfigureAwait(false);
                return response;
            }
        }

        #endregion




        #region helper methods

        async Task<T> ReadObjectFromResponseMessage<T>(HttpResponseMessage response)
        {
            var contentType = TryGetContentType(response);
            if (contentType == null)
                contentType = MediaTypeHeaderValue.Parse(encoderSettings.ContentType);

            var serializer = formatters.FindReader(contentType);

            if (serializer == null)
                throw new Exception(string.Format("No serializer found for content type: {0}", contentType));

            using (var stream = await response.Content.ReadAsStreamAsync().ConfigureAwait(false))
            {
                return serializer.ReadObject<T>(stream);
            }
        }

        MediaTypeHeaderValue TryGetContentType(HttpResponseMessage response)
        {
            if (response == null || response.Content == null || response.Content.Headers == null)
                return null;

            return response.Content.Headers.ContentType;
        }

        void EnsureSuccessStatusCode(HttpResponseMessage response)
        {
            try
            {
                response.EnsureSuccessStatusCode();
            }
            catch (Exception e)
            {
                throw new ResponseException(e, response);
            }
        }

        #endregion
    }
}