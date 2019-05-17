using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingKart.Models
{
    public class User
    {
        public int? UserId { get; set; }
        public string Username { get; set; }
        public string firstname { get; set; }
        public string lastname { get; set; }
        public string password { get; set; }

    }
}