using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostChecker.Models
{
    public class WebAddress
    {
        public int WebAddressId { get; set; }
        public string AddressName { get; set; }
        public DateTime TimePeriod { get; set; }
        public DateTime TimeOfChecking { get; set; }
        public bool Availability { get; set; }
        public User User { get; set; }
    }
}
