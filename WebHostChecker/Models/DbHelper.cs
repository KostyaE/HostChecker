using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebHostChecker.Common;

namespace WebHostChecker.Models
{
    public class DbHelper
    {
        private ApplicationDbContext dbContext;
        //private ILogger<DbHelper> _logger;
        //private IQueryable<WebAddress> _addreses;
        private IHostCheck _hostCheck;

        public DbHelper(IHostCheck hostCheck)
        {
            _hostCheck = hostCheck;
            //_addreses = addreses;
            //_logger = logger;
        }

        public DbContextOptions<ApplicationDbContext> GetAllOptions()
        {
            var optionBuilder = new DbContextOptionsBuilder<ApplicationDbContext>();
            optionBuilder.UseSqlServer(AppSettings.ConnectionString);
            return optionBuilder.Options;
        }

        public void GetListAddreses(HttpClient client)
        {
            using (dbContext = new ApplicationDbContext(GetAllOptions()))
            {
                IQueryable<WebAddress> _addreses = dbContext.Addresses;
                try
                {
                    var maxTime = DateTime.Now.AddMinutes(1);
                    _addreses = _addreses.Where(a => a.TimeOfChecking >= DateTime.Now && a.TimeOfChecking <= maxTime);

                    //_logger.LogInformation("Try to found Adreses. maxTime = {0}, timeNow = {1}", maxTime, DateTime.Now);
                    
                    foreach (WebAddress obj in _addreses)
                    {
                        //_logger.LogInformation("Before---Address: {0}, Availability: {1}, TimeOfChecking: {2}", obj.AddressName, obj.Availability, obj.TimeOfChecking);
                        System.Diagnostics.Debug.WriteLine($"Before---Address: {obj.AddressName}, Availability: {obj.Availability}, TimeOfChecking: {obj.TimeOfChecking}");
                        obj.Availability = _hostCheck.WebRequest(obj.AddressName, client);
                        obj.TimeOfChecking = _hostCheck.AddTimeNextOfChecking(obj.TimePeriod.Minute, obj.TimePeriod.Hour);
                        System.Diagnostics.Debug.WriteLine($"After    Address: {obj.AddressName}, Availability: {obj.Availability}, TimeOfChecking: {obj.TimeOfChecking}");
                        //_logger.LogInformation("After    Address: {0}, Availability: {1}, TimeOfChecking: {2}", obj.AddressName, obj.Availability, obj.TimeOfChecking);
                        dbContext.Entry(obj).State = EntityState.Modified;
                    }
                    if (_addreses.Any())
                        dbContext.SaveChanges();
                }
                catch (Exception ex)
                {
                    System.Diagnostics.Debug.WriteLine($"ERROR into GetListAddreses. {ex.Message}");
                    //_logger.LogInformation("ERROR into GetListAddreses. {0}", ex.Message);
                }
                finally
                {

                }
            }
        }
    }
}
