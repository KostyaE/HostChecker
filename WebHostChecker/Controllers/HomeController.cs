using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Net.Http;
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
        private readonly IHttpClientFactory _clientFactory;
        private User _user;
        public HomeController(IHostCheck hostCheck,
            ApplicationDbContext context,
            IHttpClientFactory clientFactory)
        {
            _clientFactory = clientFactory;
            _context = context;
            _hostCheck = hostCheck;
        }

        [Authorize(Roles = "admin, user")]
        public async Task<IActionResult> AddressList()
        {
            _user = await _context.Users.Include(u => u.WebAddreses)
                .Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            return View(_user.WebAddreses);
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
            _user = await _context.Users.Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            var client = _clientFactory.CreateClient();
            if (ModelState.IsValid)
            {
                WebAddress address = new WebAddress
                {
                    AddressName = model.WebAddress,
                    TimePeriod = model.TimePeriod,
                    TimeOfChecking = _hostCheck.AddTimeNextOfChecking(model.TimePeriod.Minute, model.TimePeriod.Hour),
                    Availability = await _hostCheck.WebRequest(model.WebAddress, client),
                    User = _user
                };
                _context.Addresses.Add(address);
                await _context.SaveChangesAsync();

                return RedirectToAction("AddressList", "Home");
            }
            else
                ModelState.AddModelError("", "Некорректные данные");
            return View("AddressList");
        }

        public async Task<IActionResult> Delete([FromQuery(Name = "WebAddressId")] int addressId)
        {
            _user = await _context.Users.Include(u => u.WebAddreses)
                .Include(h => h.RequestsHistory)
                .Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();

            var addressItem = _context.Addresses.FirstOrDefault(a => a.WebAddressId == addressId);
            var historyItems = _context.History.Where(a => a.WebAddressId == addressId);
            
            _context.Addresses.Remove(addressItem);

            foreach (var item in historyItems)
            {
                _context.History.Remove(item);
            }

            await _context.SaveChangesAsync();

            return RedirectToAction("AddressList");
        }

        [Authorize(Roles = "admin, user")]
        public async Task<ActionResult> History()
        {
            _user = await _context.Users.Include(h => h.RequestsHistory)
                .Where(u => u.Email == User.Identity.Name).FirstOrDefaultAsync();
            return View(_user.RequestsHistory);
        }
    }
}
