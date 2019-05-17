using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingKart.Models
{
    public class CartProd
    {
        //public User_cart Usercart { get; set; }
        //public Product Product { get; set; }
        public int? Product_ID { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Picture { get; set; }
        public int? qty { get; set; }
        public bool remove { get; set; }
    }
}