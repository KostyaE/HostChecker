using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebHostChecker.Models;
using Microsoft.Extensions.Logging;
using Microsoft.EntityFrameworkCore;
using WebHostChecker.Common;

namespace WebHostChecker
{
    public class HostCheck : IHostCheck
    {
        //private readonly IHttpClientFactory _clientFactory;
        private ILogger<HostCheck> _logger;
        public HostCheck(ILogger<HostCheck> logger, IHttpClientFactory clientFactory)
        {
            //_clientFactory = clientFactory;
            _logger = logger;
        }

        public Task<bool> WebRequest(string webAddress, HttpClient client)
        {
            //var client = _clientFactory.CreateClient();
            //var response = await client.SendAsync(request);
            try
            {
                var response = client.GetAsync(webAddress);
                System.Diagnostics.Debug.WriteLine($"WebRequest---: {response.Result.IsSuccessStatusCode}");
                if (response.Result.IsSuccessStatusCode) //"OK" == result.StatusCode
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

    //public class HostCheck : IHostCheck
    //{
    //    private ApplicationDbContext _dbContext;
    //    //private readonly IDbContextFactory<ApplicationDbContext> _dbcontextFactory;
    //    private HttpClient _client;
    //    private ILogger<HostCheck> _logger;

    //    public HostCheck(ILogger<HostCheck> logger, IHttpClientFactory clientFactory, ApplicationDbContext dbContext)
    //    {
    //        //_dbcontextFactory = dbcontextFactory;
    //        _client = clientFactory.CreateClient("Client");
    //        _logger = logger;
    //        _dbContext = dbContext;
    //    }

    //    //public async Task CheckStatusAsync(WebAddress webAddress)
    //    //{
    //    //    webAddress.Availability = await Task.Run(() => WebRequest(webAddress.AddressName));
    //    //    //await Task.Run(() => UpdateDB();
    //    //}
    //    public void CheckDB()
    //    {
    //        // Останавливаем таймер
    //        //_timer.Change(Timeout.Infinite, 0);
    //        try
    //        {
    //            IQueryable<WebAddress> addreses = _dbContext.Addresses;
    //            var maxTime = DateTime.Now.AddMinutes(1);
    //            addreses = addreses.Where(a => a.TimeOfChecking >= DateTime.Now && a.TimeOfChecking <= maxTime);

    //            _logger.LogInformation("Try to found Adreses. maxTime = {0}, timeNow = {1}", maxTime, DateTime.Now);
    //            foreach (WebAddress obj in addreses)
    //            {
    //                _logger.LogInformation("Before---Address: {0}, Availability: {1}, TimeOfChecking: {2}", obj.AddressName, obj.Availability, obj.TimeOfChecking);
    //                obj.Availability = WebRequest(obj.AddressName);
    //                obj.TimeOfChecking = AddTimeNextOfChecking(obj.TimePeriod.Minute, obj.TimePeriod.Hour);
    //                _logger.LogInformation("After    Address: {0}, Availability: {1}, TimeOfChecking: {2}", obj.AddressName, obj.Availability, obj.TimeOfChecking);
    //                _dbContext.Entry(obj).State = EntityState.Modified;
    //            }
    //            if(addreses.Any())
    //                _dbContext.SaveChanges();
    //        }
    //        catch (Exception ex)
    //        {
    //            _logger.LogInformation("ERROR into TimeService. {0}", ex.Message);
    //        }
    //        finally
    //        {
    //            // Возвращаем таймер в исходное состояние
    //            //_timer.Change(checkPeriod, checkPeriod);
    //        }

    //        _logger.LogInformation("------CheckDB completed.");
    //    }
    //    public bool WebRequest(string webAddress)
    //    {
    //        var result = _client.GetAsync(webAddress);
    //        if (result.Result.IsSuccessStatusCode) //"OK" == result.StatusCode
    //            return true;
    //        else
    //            return false;
    //    }

    //    public DateTime AddTimeNextOfChecking(int minute, int hours)
    //    {
    //        DateTime nextTimeOfChecking = DateTime.Now.AddMinutes(minute).AddHours(hours);
    //        return nextTimeOfChecking;
    //    }
    //    //public static bool WebRequest(string webAddress)
    //    //{

    //    //    var result = _client.GetAsync(webAddress);
    //    //    if (result.Result.IsSuccessStatusCode) //"OK" == result.StatusCode
    //    //        return true;
    //    //    else
    //    //        return false;
    //    //}

    //    //public void UpdateDB(List<WebAddress> addressList)
    //    //{

    //    //}

    //    public async Task<bool> GetStatusAsync(string webAddres)
    //    {
    //        try
    //        {
    //            var result = await _client.GetAsync(webAddres);
    //            if (result.StatusCode == HttpStatusCode.OK)
    //                return true;
    //            else
    //                return false;
    //        }
    //        catch(Exception ex)
    //        {
    //            _logger.LogInformation("ERROR into GetStatusAsync. {0}", ex.Message);
    //        }
    //        return false;
    //    }
    //}
}
