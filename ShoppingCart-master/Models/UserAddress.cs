using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingKart.Models
{
    public class UserAddress
    {
        public int? UserId { get; set; }
        public string Address { get; set; }
        public int? AddressId { get; set; }
    }
}