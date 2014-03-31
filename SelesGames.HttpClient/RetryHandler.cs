using SelesGames.HttpClient.RetryPolicies;
using System;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;

namespace SelesGames.HttpClient
{
    public class RetryHandler : DelegatingHandler
    {
        IRetryPolicy retryPolicy;

        public Action<string> Log { get; set; }

        public RetryHandler(HttpMessageHandler innerHandler, IRetryPolicy retryPolicy)
            : base(innerHandler)
        {
            if (retryPolicy == null) throw new ArgumentNullException("retryPolicy");

            this.retryPolicy = retryPolicy;

#if DEBUG
            Log = s => System.Diagnostics.Debug.WriteLine(s);
#endif
        }

        protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            return SendAsync(request, null, cancellationToken);
        }

        async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, RetryContext retryContext, CancellationToken cancellationToken)
        {
            if (retryContext != null)
            {
                var retryInfo = retryPolicy.Evaluate(retryContext);
                if (!retryInfo.ShouldRetry)
                    throw new ErrorResponseException(retryContext.LastRequestResult);

                await Task.Delay(retryInfo.RetryInterval).ConfigureAwait(false);
                cancellationToken.ThrowIfCancellationRequested();
            }

            await TraceRequest(request, retryContext);

            var response = await base.SendAsync(request, cancellationToken).ConfigureAwait(false);

            await TraceResponse(response);

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
                return await SendAsync(request, retryContext, cancellationToken);
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




        #region Trace the request and response using the specificied Log delegate

        async Task TraceRequest(HttpRequestMessage request, RetryContext retryContext)
        {
            if (Log == null)
                return;

            string output;
            var contentString = await ReadContentAsString(request.Content);

            if (string.IsNullOrEmpty(contentString))
            {
                output = string.Format(
                    "Sending request... (attempt {1})\r\n{0}\r\n\r\n",
                    request.ToString(),
                    retryContext == null ? 0 : retryContext.CurrentRetryCount);
            }
            else
            {
                output = string.Format(
                    "Sending request... (attempt {2})\r\n{0}\r\nCONTENT:\r\n{1}\r\n\r\n",
                    request.ToString(),
                    contentString,
                    retryContext == null ? 0 : retryContext.CurrentRetryCount);
            }

            Log(output);
        }

        async Task<string> ReadContentAsString(HttpContent content)
        {
            if (!(content is ObjectContent))
                return null;

            var objectContent = (ObjectContent)content;
            var copy = new ObjectContent(objectContent.ObjectType, objectContent.Value, objectContent.Formatter);

            return await copy.ReadAsStringAsync();
        }

        async Task TraceResponse(HttpResponseMessage response)
        {
            if (Log == null)
                return;

            string output;

            var responseString = await response.Content.ReadAsStringAsync();

            if (string.IsNullOrEmpty(responseString))
            {
                output = string.Format(
                    "Received response ... \r\n{0}\r\n\r\n",
                    response.ToString());
            }
            else
            {
                output = string.Format(
                    "Received response ... \r\n{0}\r\nRESPONSE:\r\n{1}\r\n\r\n",
                    response.ToString(),
                    responseString);
            }

            Log(output);
        }

        #endregion
    }
}