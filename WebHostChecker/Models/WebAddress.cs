using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostChecker.Models
{
    public class WebAddress
    {
        public int Id { get; set; }
        public string AddressName { get; set; }
        public int Period { get; set; }
        public bool Accessed { get; set; }

    }
}
