using System;
using System.Net.Http;

namespace SelesGames.HttpClient
{
    public static class HttpClientExtensions
    {
        public static IDisposable PollChangesToResource(this System.Net.Http.HttpClient client, string resourceUrl, TimeSpan pollingInterval, IObserver<HttpResponseMessage> observer)
        {
            var listener = new HttpResourceListener(client, resourceUrl, observer.OnNext, observer.OnError);
            listener.PollingInterval = pollingInterval;
            listener.BeginListening();

            return listener;
        }

        public static IDisposable PollChangesToResource(this System.Net.Http.HttpClient client,
            string resourceUrl,
            TimeSpan pollingInterval,
            Action<HttpResponseMessage> onUpdated,
            Action<Exception> onException = null)
        {
            var listener = new HttpResourceListener(client, resourceUrl, onUpdated, onException);
            listener.PollingInterval = pollingInterval;
            listener.BeginListening();

            return listener;
        }
    }
}
