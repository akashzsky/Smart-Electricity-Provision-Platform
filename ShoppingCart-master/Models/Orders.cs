using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingKart.Models
{
    public class Orders
    {
        //public int? Id { get; set; }
        public int? UserId { get; set; }
        public string OrderTime { get; set; }
        public string Address { get; set; }
        public int? OrderNo { get; set; }
        public double OrderTotal { get; set; }
        public Boolean processed { get; set; }
    }
}