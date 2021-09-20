using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebHostChecker.Models;

namespace WebHostChecker
{
    public class HostCheck
    {
        private static HttpClient _client;

        public HostCheck(IHttpClientFactory clientFactory)
        {
            _client = clientFactory.CreateClient("Client");
        }

        //public async Task CheckStatusAsync(WebAddress webAddress)
        //{
        //    webAddress.Availability = await Task.Run(() => WebRequest(webAddress.AddressName));
        //    //await Task.Run(() => UpdateDB();
        //}
        public static bool WebRequest(string webAddress)
        {
            var result = _client.GetAsync(webAddress);
            if (result.Result.IsSuccessStatusCode) //"OK" == result.StatusCode
                return true;
            else
                return false;
        }

        public static DateTime AddTimeNextOfChecking(int minute, int hours)
        {
            DateTime nextTimeOfChecking = DateTime.Now.AddMinutes(minute).AddHours(hours);
            return nextTimeOfChecking;
        }
        //public static bool WebRequest(string webAddress)
        //{

        //    var result = _client.GetAsync(webAddress);
        //    if (result.Result.IsSuccessStatusCode) //"OK" == result.StatusCode
        //        return true;
        //    else
        //        return false;
        //}

        //public void UpdateDB(List<WebAddress> addressList)
        //{

        //}

        public static async Task<bool> GetStatusAsync(string webAddres)
        {
            var result = await _client.GetAsync(webAddres);
            if (result.StatusCode == HttpStatusCode.OK)
                return true;
            else
                return false;
        }
    }
}
