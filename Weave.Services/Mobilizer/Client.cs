using SelesGames.HttpClient;
using System;
using System.Threading;
using System.Threading.Tasks;
using Weave.Services.Mobilizer.Contracts;
using Weave.Services.Mobilizer.DTOs;

namespace Weave.Services.Mobilizer
{
    public class Client : IMobilizerService
    {
        //const string R_URL_TEMPLATE = "http://mobilizer.cloudapp.net/ipf?url={0}&stripLeadImage=true";
        const string SERVICE_URL = "http://mobilizer.cloudapp.net/api/";

        public async Task<MobilizerResult> Get(string url, bool stripLeadImage)
        {
            if (string.IsNullOrEmpty(url)) throw new ArgumentException("Requires a valid url");

            string append = "ipf";
            var fUrl = new UriBuilder(SERVICE_URL + append)
                .AddParameter("url", url)
                .ToString();

            var client = CreateClient();
            var result = await client.GetAsync<MobilizerResult>(fUrl, CancellationToken.None);
            return result;
        }

        public Task Post(string url, MobilizerResult article)
        {
            throw new NotImplementedException();
        }

        SmartHttpClient CreateClient()
        {
            return new SmartHttpClient();
        }
    }
}