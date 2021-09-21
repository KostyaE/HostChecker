using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace WebHostChecker.Models
{
    public interface IDbHelper
    {
        DbContextOptions<ApplicationDbContext> GetAllOptions();
        void GetListAddreses(HttpClient client);

    }
}
