using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using WebHostChecker.Common;
using WebHostChecker.Models;
using WebHostChecker.ViewModels;

namespace WebHostChecker.Controllers
{
    public class HomeController : Controller
    {
        private readonly IHostCheck _hostCheck;
        private readonly ApplicationDbContext _context;
        //private DbSet<WebAddress> addresses;
        //private IHttpClientFactory _clientFactory;
        public HomeController(IHostCheck hostCheck, ApplicationDbContext context)//, IHttpClientFactory clientFactory, ILogger<HostCheck> logger)
        {
            _context = context;
            _hostCheck = hostCheck;
            //addresses = _context.Addresses;
            //_clientFactory = clientFactory;
        }

        [Authorize(Roles = "admin, user")]
        public IActionResult AddressList()
        {
            return View(_context.Addresses.ToList());

            //string role = User.FindFirst(x => x.Type == ClaimsIdentity.DefaultRoleClaimType).Value;
            //return Content($"ваша роль: {role}");
        }
        [Authorize(Roles = "admin, user")]
        public IActionResult AddUrl()
        {
            return View();
        }

        [Authorize(Roles = "admin, user")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddUrl(SetAddressModel model)
        {
            //bool availResult = false;// = HostCheck.GetStatus(model.WebAddress).Result;
            if (ModelState.IsValid)
            {

                WebAddress address = new WebAddress
                {
                    AddressName = model.WebAddress,
                    TimePeriod = model.TimePeriod,
                    TimeOfChecking = _hostCheck.AddTimeNextOfChecking(model.TimePeriod.Minute, model.TimePeriod.Hour),
                    Availability = false//_hostCheck.GetStatusAsync(model.WebAddress).Result
                };
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                return RedirectToAction("AddressList", "Home");
            }
            else
                ModelState.AddModelError("", "Некорректные данные");
            return View("AddressList");
        }


        [Authorize(Roles = "admin, user")]
        [ValidateAntiForgeryToken]
        public IActionResult CheckStateAdreses()
        {
            //_hostCheck.CheckDB();
            return RedirectToAction("AddressList", "Home");
        }


        [Authorize(Roles = "admin, user")]
        public IActionResult SetPeriod(string webaddress, DateTime startdate, DateTime enddate)//DateTime start, DateTime end, string url)
        {
            //реализавать фильтр
            return Content($"WebAddress: {webaddress}  StartDate:{startdate}  EndDate: {enddate}");

            //List<RequestHistory> histories = new List<RequestHistory>();
            //return View(histories);
        }

        [Authorize(Roles = "admin, user")]
        public IActionResult History()
        {
            return View(_context.History.ToList());
        }

        //[Authorize]
        //public IActionResult Index()
        //{
        //    return Content(User.Identity.Name);
        //}


        //private readonly ILogger<HomeController> _logger;

        //public HomeController(ILogger<HomeController> logger)
        //{
        //    _logger = logger;
        //}

        //public IActionResult Index()
        //{
        //    return View();
        //}

        //public IActionResult Privacy()
        //{
        //    return View();
        //}

        //[ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        //public IActionResult Error()
        //{
        //    return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        //}
    }
}
