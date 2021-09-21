using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using WebHostChecker.Common;
using WebHostChecker.Models;

namespace WebHostChecker.Services
{
    internal sealed class TimerService : IHostedService, IDisposable
    {
        //private ApplicationDbContext _dbContext;
        //private readonly IDbContextFactory<ApplicationDbContext> _dbcontextFactory;
        //private readonly ITimerHostCheck _hostCheck;
        //private readonly IDbContextFactory<ApplicationDbContext> _contextFactory;
        //private HttpClient _client;
        //private readonly IDbHelper _dbHelper;
        private readonly DbHelper _dbHelper;
        private readonly IHttpClientFactory _clientFactory;
        private ILogger<TimerService> _logger;
        private Timer _timer;
        private int checkPeriod = 30;

        public TimerService(ILogger<TimerService> logger, IHttpClientFactory clientFactory, IHostCheck hostCheck)
        {
            _dbHelper = new DbHelper(hostCheck);
            _clientFactory = clientFactory;
            _logger = logger;
        }
        //public TimerService(ILogger<TimerService> logger, IHttpClientFactory clientFactory, IDbHelper dbHelper)//, ITimerHostCheck hostCheck, IHttpClientFactory clientFactory)//, ApplicationDbContext dbContext)
        //{
        //    //_dbHelper = new DbHelper(ILogger<DbHelper> logger, IQueryable<WebAddress> addreses, IHostCheck hostcheck);
        //    _dbHelper = dbHelper;
        //    _clientFactory = clientFactory;
        //    _logger = logger;
        //}
        public Task StartAsync(CancellationToken cancellationToken)
        {
            _logger.LogInformation("---StartAsync");
            _timer = new Timer(CheckDB, cancellationToken, TimeSpan.FromSeconds(checkPeriod), TimeSpan.FromSeconds(checkPeriod));
            return Task.CompletedTask;
        }

        private void CheckDB(object state)
        {
            var client = _clientFactory.CreateClient();

            // Останавливаем таймер
            _timer.Change(Timeout.Infinite, 0);
            try
            {
                _dbHelper.GetListAddreses(client);

                //var response = client.GetAsync("https://docs.microsoft.com");
                //if (response.Result.IsSuccessStatusCode) //"OK" == result.StatusCode
                //    _logger.LogInformation("Web address availibility. {0}", true);
                //else
                //    _logger.LogInformation("Web address availibility. {0}", false);
                //_dbHelper.GetListAddreses();
            }
            catch (Exception ex)
            {
                _logger.LogInformation("ERROR into CheckDB. {0}", ex.Message);
            }
            finally
            {
                // Возвращаем таймер в исходное состояние
                _timer.Change(TimeSpan.FromSeconds(checkPeriod), TimeSpan.FromSeconds(checkPeriod));
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
        public DateTime AddTimeNextOfChecking(int minute, int hours)
        {
            DateTime nextTimeOfChecking = DateTime.Now.AddMinutes(minute).AddHours(hours);
            return nextTimeOfChecking;
        }

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
