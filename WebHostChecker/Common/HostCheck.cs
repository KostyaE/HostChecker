using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using WebHostChecker.Common;

namespace WebHostChecker
{
    public class HostCheck : IHostCheck
    {
        private ILogger<HostCheck> _logger;
        public HostCheck(ILogger<HostCheck> logger)
        {
            _logger = logger;
        }

        public Task<bool> WebRequest(string webAddress, HttpClient client)
        {
            try
            {
                var response = client.GetAsync(webAddress);
                System.Diagnostics.Debug.WriteLine($"WebRequest---: {response.Result.IsSuccessStatusCode}");
                if (response.Result.IsSuccessStatusCode)
                    return Task.FromResult(true);
                else
                    return Task.FromResult(false);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR into WebRequest. {ex.Message}");
            }
            return Task.FromResult(false);

        }

        public DateTime AddTimeNextOfChecking(int minute, int hours)
        {
            DateTime nextTimeOfChecking = DateTime.Now.AddMinutes(minute).AddHours(hours);
            return nextTimeOfChecking;
        }
    }
}
