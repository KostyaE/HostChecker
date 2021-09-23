using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
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
        private readonly IHostCheck _hostCheck;
        private readonly IHttpClientFactory _clientFactory;
        private readonly IServiceScopeFactory _serviceScopeFactory;
        private Timer _timer;
        private int checkPeriod = 30;

        public TimerService(IHttpClientFactory clientFactory, IServiceScopeFactory serviceScopeFactory, IHostCheck hostCheck)
        {
            _hostCheck = hostCheck;
            _serviceScopeFactory = serviceScopeFactory;
            _clientFactory = clientFactory;
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            // запускает CheckDB каждые 30 сек
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
                using (var scope = _serviceScopeFactory.CreateScope())
                {
                    var dbContext = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
                    IQueryable<WebAddress> addreses = dbContext.Addresses;
                    try
                    {
                        var minTime = DateTime.Now.AddMinutes(-1);
                        // выбираем адреса которые нужно проверить в промежутке от minTime до сейчас
                        addreses = addreses.Where(a => a.TimeOfChecking <= DateTime.Now && a.TimeOfChecking >= minTime);
                        foreach (WebAddress obj in addreses)
                        {
                            // проверяем доступность сайта
                            obj.Availability = _hostCheck.WebRequest(obj.AddressName, client).Result;
                            // прибавляем промежуточное время для следующей проверки
                            obj.TimeOfChecking = _hostCheck.AddTimeNextOfChecking(obj.TimePeriod.Minute, obj.TimePeriod.Hour);

                            RequestHistory history = new RequestHistory
                            {
                                UserId = obj.UserId,
                                CheckDate = DateTime.Now,
                                Availability = obj.Availability,
                                HostName = obj.AddressName,
                                WebAddressId = obj.WebAddressId
                            };

                            // добавлям запись в историю
                            dbContext.History.Add(history);
                            dbContext.Entry(obj).State = EntityState.Modified;
                        }
                        if (addreses.Any())
                            dbContext.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        System.Diagnostics.Debug.WriteLine($"ERROR into GetListAddreses. {ex.Message}");
                    }
                }

            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"ERROR into CheckDB. {ex.Message}");
            }
            finally
            {
                // Возвращаем таймер в исходное состояние
                _timer.Change(TimeSpan.FromSeconds(checkPeriod), TimeSpan.FromSeconds(checkPeriod));
            }
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _timer.Change(Timeout.Infinite, 0);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
            _timer?.Dispose();
        }
    }
}
