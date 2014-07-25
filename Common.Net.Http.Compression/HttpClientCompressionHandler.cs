using System;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Net.Http.Compression
{
    public class HttpClientCompressionHandler : HttpClientHandler
    {
        public override bool SupportsAutomaticDecompression
        {
            get { return false; }
        }

        protected async override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            request = await CompressRequest(request).ConfigureAwait(false);

            var response = await base
                .SendAsync(request, cancellationToken)
                .ContinueWith(o => DecompressResponse(o.Result), TaskContinuationOptions.OnlyOnRanToCompletion);

            return await response;
        }

        static async Task<HttpRequestMessage> CompressRequest(HttpRequestMessage request)
        {
            var content = request.Content;
            if (content != null)
            {
                var contentEncoding = content.Headers.ContentEncoding;
                if (contentEncoding != null && contentEncoding.Any())
                {
                    string encodingType = contentEncoding.First() ?? "";
                    var compressionHandler = Common.Compression.Settings.CompressionHandlers.Find(encodingType);
                    if (compressionHandler == null)
                        throw new Exception("no compression handler was found for encoding type: " + encodingType);

                    request.Content = await content.AsByteArray(compressionHandler, Mode.Compress);
                }
            }
            return request;
        }

        static async Task<HttpResponseMessage> DecompressResponse(HttpResponseMessage response)
        {
            if (response.IsSuccessStatusCode)
            {
                var contentEncoding = response.Content.Headers.ContentEncoding;
                if (contentEncoding != null && contentEncoding.Any())
                {
                    string encodingType = contentEncoding.First() ?? "";
                    var compressionHandler = Common.Compression.Settings.CompressionHandlers.Find(encodingType);
                    if (compressionHandler == null)
                        throw new Exception("no compression handler was found for encoding type: " + encodingType);

                    response.Content = await response.Content.AsByteArray(compressionHandler, Mode.Decompress);
                }
            }

            return response;
        }
    }
}