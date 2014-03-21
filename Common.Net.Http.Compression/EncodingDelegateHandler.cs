using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Net.Http.Compression
{
    /// <summary>
    /// Compresses outgoing HttpResponseMessage Content if the original request 
    /// specifed that it could accept compressed content 
    /// (via Accept-Encoding: gzip, deflate).
    /// 
    /// Decompresses incoming content if the Content-Encoding is gzip or deflate
    /// </summary>
    public class EncodingDelegateHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base
                .SendAsync(request, cancellationToken)
                .ContinueWith(o => CompressIfRequested(o.Result), TaskContinuationOptions.OnlyOnRanToCompletion);
        }

        static HttpRequestMessage DecompressContentIfNeeded(HttpRequestMessage request)
        {
            var content = request.Content;
            if (content != null)
            {
                var contentEncoding = content.Headers.ContentEncoding;
                if (contentEncoding != null && contentEncoding.Any())
                {
                    string encodingType = contentEncoding.First() ?? "";

                    if (encodingType.IsGzipOrDeflate())
                    {
                        request.Content = new DecompressedContent(request.Content, encodingType);
                    }
                }
            }
            return request;
        }

        static HttpResponseMessage CompressIfRequested(HttpResponseMessage response)
        {
            if (response.HasEncodeableContent())
            {
                var acceptEncoding = response.RequestMessage.Headers.AcceptEncoding;

                if (acceptEncoding != null && acceptEncoding.Any())
                {
                    string encodingType = GetFilteredEncodings(acceptEncoding).FirstOrDefault();

                    if (encodingType != null)
                    {
                        response.Content = new CompressedContent(response.Content, encodingType);
                    }
                }
            }

            return response;
        }

        static IEnumerable<string> GetFilteredEncodings(IEnumerable<StringWithQualityHeaderValue> headerValues)
        {
            foreach (var headerValue in headerValues)
            {
                if (headerValue != null && headerValue.Value != null && headerValue.Value.IsGzipOrDeflate())
                    yield return headerValue.Value;
            }
        }
    }
}