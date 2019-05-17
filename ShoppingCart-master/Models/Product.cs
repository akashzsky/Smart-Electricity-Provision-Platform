using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace ShoppingKart.Models
{
    public class Product
    {
        public int? Product_ID { get; set; }
        [Display(Name="Product Name")]
        public string ProductName { get; set; }
        public string Description { get; set; }
        public double Price { get; set; }
        public string Picture { get; set; }
        public int? qty { get; set; }
        public bool iscart { get; set; }

    }
}