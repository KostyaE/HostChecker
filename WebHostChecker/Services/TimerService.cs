using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebHostChecker.Models;

namespace WebHostChecker.Services
{
    public interface IMyDependency
    {

    }
    internal sealed class TimerService : IHostedService, IDisposable
    {
        //private HttpClient _client = clientFactory.CreateClient("Client2");
        private ApplicationDBContext _dbContext;
        private int checkPeriod = 30;
        private ILogger<TimerService> _logger;
        private Timer _timer;
        public TimerService(ILogger<TimerService> logger, ApplicationDBContext dbContext)
        {
            //_client = clientFactory.CreateClient("Client2");
            _logger = logger;
            _dbContext = dbContext;
        }
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("---StartAsync");
            _timer = new Timer(CheckDB, cancellationToken, TimeSpan.FromSeconds(checkPeriod), TimeSpan.FromSeconds(checkPeriod));
            return Task.CompletedTask;
        }

        private void CheckDB(object state)
        {
            // Останавливаем таймер
            _timer.Change(Timeout.Infinite, 0);
            try
            {
                IQueryable<WebAddress> addreses = _dbContext.Addresses;
                addreses = addreses.Where(a => a.TimeOfChecking == DateTime.Now);

                foreach (WebAddress obj in addreses)
                {
                    _logger.LogInformation("Before---Address: {0}, Availability: {1}, TimeOfChecking: {2}", obj.AddressName, obj.Availability, obj.TimeOfChecking);
                    obj.Availability = HostCheck.WebRequest(obj.AddressName);
                    obj.TimeOfChecking = HostCheck.AddTimeNextOfChecking(obj.TimePeriod.Minute, obj.TimePeriod.Hour);
                    _logger.LogInformation("After    Address: {0}, Availability: {1}, TimeOfChecking: {2}", obj.AddressName, obj.Availability, obj.TimeOfChecking);
                    _dbContext.Entry(obj).State = EntityState.Modified;
                }

                _dbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                _logger.LogInformation("ERROR into TimeService. {0}", ex.Message);
            }
            finally
            {
                // Возвращаем таймер в исходное состояние
                _timer.Change(checkPeriod, checkPeriod);
            }

            _logger.LogInformation("------CheckDB completed.");
        }

        //public bool WebRequest(string webAddress)
        //{
        //    var result = _client.GetAsync(webAddress);
        //    if (result.Result.IsSuccessStatusCode) //"OK" == result.StatusCode
        //        return true;
        //    else
        //        return false;
        //}

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation(("----StopAsync."));
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _logger.LogInformation(("----Dispose"));
            _timer?.Dispose();
        }
    }
}
