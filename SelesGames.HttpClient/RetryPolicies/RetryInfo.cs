using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SelesGames.HttpClient.RetryPolicies
{
    public class RetryInfo
    {
        // Summary:
        //     Initializes a new instance of the Microsoft.WindowsAzure.Storage.RetryPolicies.RetryInfo
        //     class.
        public RetryInfo();
        //
        // Summary:
        //     Initializes a new instance of the Microsoft.WindowsAzure.Storage.RetryPolicies.RetryInfo
        //     class.
        //
        // Parameters:
        //   retryContext:
        //     The Microsoft.WindowsAzure.Storage.RetryPolicies.RetryContext object that
        //     was passed in to the retry policy.
        public RetryInfo(RetryContext retryContext);

        public bool ShouldRetry { get; set; }

        // Summary:
        //     Gets the interval until the next retry.
        public TimeSpan RetryInterval { get; set; }

        // Summary:
        //     Returns a string that represents the current Microsoft.WindowsAzure.Storage.RetryPolicies.RetryInfo
        //     instance.
        //
        // Returns:
        //     A string that represents the current Microsoft.WindowsAzure.Storage.RetryPolicies.RetryInfo
        //     instance.
        public override string ToString();
    }
}
