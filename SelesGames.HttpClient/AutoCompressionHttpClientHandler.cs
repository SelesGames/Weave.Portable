using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace SelesGames.HttpClient
{
    public class AutoCompressionHttpClientHandler : HttpClientHandler
    {
        public AutoCompressionHttpClientHandler()
        {
            if (base.SupportsAutomaticDecompression)
            {
                AutomaticDecompression = DecompressionMethods.GZip | DecompressionMethods.Deflate;
            }
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
#if DEBUG
            Debug.WriteLine(request);
            return base.SendAsync(request, cancellationToken).ContinueWith(t => OnDebugContinue(t), cancellationToken);
#else
            return base.SendAsync(request, cancellationToken);
#endif
        }

        HttpResponseMessage OnDebugContinue(Task<HttpResponseMessage> responseAsync)
        {
            var response = responseAsync.Result;
            Debug.WriteLine(response);
            return response;
        }
    }
}
