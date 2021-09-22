using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostChecker.Models
{
    public class RequestHistory
    {
        public int RequestHistoryId { get; set; }
        public DateTime CheckDate { get; set; }
        public string HostName { get; set; }
        public bool Availability { get; set; }
        public int? UserId { get; set; }
        public int? WebAddressId { get; set; }
        public User User { get; set; }
    }
}
