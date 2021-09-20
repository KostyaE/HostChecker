using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebHostChecker.Models
{
    public class User
    {
        public int UserId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public int? RoleId { get; set; }
        public Role Role { get; set; }
        public List<WebAddress> WebAddreses { get; set; }
        public List<RequestHistory> RequestsHistory { get; set; }
        public User()
        {
            WebAddreses = new List<WebAddress>();
            RequestsHistory = new List<RequestHistory>();
        }
    }
    public class Role
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public List<User> Users { get; set; }
        public Role()
        {
            Users = new List<User>();
        }
    }
}
