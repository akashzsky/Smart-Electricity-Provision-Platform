using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ShoppingKart.Models
{
    public class OrderDetail
    {
        public int? Id { get; set; }
        public int? UserId { get; set; }
        public int? Product_ID { get; set; }
        public int? qty { get; set; }
        public int? OrderId { get; set; }
        public string OrderTime { get; set; }
        public double OrderTotal { get; set; }
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Picture { get; set; }
        public string Address { get; set; }
        public int? OrderNo { get; set; }
    }
}