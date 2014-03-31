using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelesGames.HttpClient.RetryPolicies
{
    public class RetryInfo
    {
        public RetryInfo() { }
        public RetryInfo(bool shouldRetry)
        {
            ShouldRetry = shouldRetry;
        }

        //public RetryInfo();
        //public RetryInfo(RetryContext retryContext);

        public bool ShouldRetry { get; set; }

        // Summary:
        //     Gets the interval until the next retry.
        public TimeSpan RetryInterval { get; set; }
    }
}
