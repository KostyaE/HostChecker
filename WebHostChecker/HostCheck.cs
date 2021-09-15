using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostChecker
{
    public class HostCheck
    {
        public static async Task<bool> GetStatus(string webAddres)
        {
            using (var client = new System.Net.Http.HttpClient())
            {
                var result = await client.GetAsync(webAddres);
                if ("OK" == result.StatusCode.ToString())
                    return true;
                else
                    return false;
            }
        }
    }
}
