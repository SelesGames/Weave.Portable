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

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(CompressIfNeeded(request), cancellationToken).ContinueWith(o => DecompressIfNeeded(o), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        static HttpRequestMessage CompressIfNeeded(HttpRequestMessage request)
        {
            var content = request.Content;
            if (content != null)
            {
                var contentEncoding = content.Headers.ContentEncoding;
                if (contentEncoding != null && contentEncoding.Any())
                {
                    string encodingType = contentEncoding.First();

                    if (IsGzipOrDeflate(encodingType))
                    {
                        request.Content = new CompressedContent(request.Content, encodingType);
                    }
                }
            }
            return request;
        }

        static HttpResponseMessage DecompressIfNeeded(Task<HttpResponseMessage> o)
        {
            HttpResponseMessage response = o.Result;

            if (response.IsSuccessStatusCode)
            {
                var contentEncoding = response.Content.Headers.ContentEncoding;

                if (contentEncoding != null && contentEncoding.Any())
                {
                    string encodingType = contentEncoding.First();

                    if (IsGzipOrDeflate(encodingType))
                    {
                        response.Content = new DecompressedContent(response.Content, encodingType);
                    }
                }
            }

            return response;
        }

        static bool IsGzipOrDeflate(string encodingType)
        {
            return 
                encodingType.Equals("gzip", StringComparison.OrdinalIgnoreCase) ||
                encodingType.Equals("deflate", StringComparison.OrdinalIgnoreCase);
        }
    }
}