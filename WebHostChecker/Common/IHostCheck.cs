using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using WebHostChecker.Models;

namespace WebHostChecker.Common
{
    public interface IHostCheck
    {
        bool WebRequest(string webAddress, HttpClient client);
        DateTime AddTimeNextOfChecking(int minute, int hours);
    }
}
