using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebHostChecker.Models;

namespace WebHostChecker.Common
{
    public class TimerHostCheck : ITimerHostCheck
    {
        private ApplicationDbContext _dbContext;
        //private readonly IDbContextFactory<ApplicationDbContext> _dbcontextFactory;
        private HttpClient _client;
        private ILogger<TimerHostCheck> _logger;

        public TimerHostCheck(ILogger<TimerHostCheck> logger, IHttpClientFactory clientFactory, ApplicationDbContext dbContext)
        {
            //_dbcontextFactory = dbcontextFactory;
            _client = clientFactory.CreateClient("Client2");
            _logger = logger;
            _dbContext = dbContext;
        }
        public DateTime AddTimeNextOfChecking(int minute, int hours)
        {
            DateTime nextTimeOfChecking = DateTime.Now.AddMinutes(minute).AddHours(hours);
            return nextTimeOfChecking;
        }

        public void CheckDB()
        {
            // Останавливаем таймер
            //_timer.Change(Timeout.Infinite, 0);
            try
            {
                IQueryable<WebAddress> addreses = _dbContext.Addresses;
                var maxTime = DateTime.Now.AddMinutes(1);
                addreses = addreses.Where(a => a.TimeOfChecking >= DateTime.Now && a.TimeOfChecking <= maxTime);

                _logger.LogInformation("Try to found Adreses. maxTime = {0}, timeNow = {1}", maxTime, DateTime.Now);
                foreach (WebAddress obj in addreses)
                {
                    _logger.LogInformation("Before---Address: {0}, Availability: {1}, TimeOfChecking: {2}", obj.AddressName, obj.Availability, obj.TimeOfChecking);
                    obj.Availability = WebRequest(obj.AddressName);
                    obj.TimeOfChecking = AddTimeNextOfChecking(obj.TimePeriod.Minute, obj.TimePeriod.Hour);
                    _logger.LogInformation("After    Address: {0}, Availability: {1}, TimeOfChecking: {2}", obj.AddressName, obj.Availability, obj.TimeOfChecking);
                    _dbContext.Entry(obj).State = EntityState.Modified;
                }
                if (addreses.Any())
                    _dbContext.SaveChanges();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("ERROR into TimeService. {0}", ex.Message);
            }
            finally
            {
                // Возвращаем таймер в исходное состояние
                //_timer.Change(checkPeriod, checkPeriod);
            }

            _logger.LogInformation("------CheckDB completed.");
        }

        public bool WebRequest(string webAddress)
        {
            var result = _client.GetAsync(webAddress);
            if (result.Result.IsSuccessStatusCode) //"OK" == result.StatusCode
                return true;
            else
                return false;
        }
    }
}
