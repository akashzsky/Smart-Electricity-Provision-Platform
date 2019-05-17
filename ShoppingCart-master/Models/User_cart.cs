using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingKart.Models
{
    public class User_cart
    {
        public int? UserId { get; set; }
        public int? Product_ID { get; set; }
        public int? qty { get; set; }
        public int? cartId { get; set; }
        bool remove { get; set; }
    }
}