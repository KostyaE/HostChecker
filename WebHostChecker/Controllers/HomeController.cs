using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using WebHostChecker.Models;
using WebHostChecker.ViewModels;

namespace WebHostChecker.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationContext _context;

        public HomeController(ApplicationContext context)
        {
            _context = context;
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
            if (ModelState.IsValid)
            {

                WebAddress address = new WebAddress
                {
                    AddressName = model.WebAddress,
                    Period = model.Period,
                    Availability = HostCheck.GetStatus(model.WebAddress).Result
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
        public IActionResult SetPeriod(DateTime start, DateTime end, string url)
        {
            //реализавать фильтр
            List<RequestHistory> histories = new List<RequestHistory>();
            return View(histories);
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
