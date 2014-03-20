using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Net.Http.Compression
{
    public class EncodingDelegateHandler : DelegatingHandler
    {
        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return base.SendAsync(request, cancellationToken).ContinueWith(responseToCompleteTask =>
            {
                HttpResponseMessage response = responseToCompleteTask.Result;

                if (response.HasEncodeableContent())
                {
                    var acceptEncoding = response.RequestMessage.Headers.AcceptEncoding;

                    if (acceptEncoding != null && acceptEncoding.Any())
                    {
                        string encodingType = acceptEncoding.First().Value ?? "";

                        if (encodingType.IsGzipOrDeflate())
                        {
                            response.Content = new CompressedContent(response.Content, encodingType);
                        }
                    }
                }

                return response;
            },
            TaskContinuationOptions.OnlyOnRanToCompletion);
        }
    }
}