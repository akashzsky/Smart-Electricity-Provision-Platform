using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SMP_WebApplication.Models
{
    public class DisplayJob
    {
        public long ID { get; set; }
        public long CusID { get; set; }
        public string WorkType { get; set; }
        public System.DateTime ReceiveDate { get; set; }
        public string JobStatus { get; set; }
        public Nullable<long> CoordinatorID { get; set; }
        public Nullable<long> SupplierID { get; set; }
        public Nullable<System.DateTime> CompletedDate { get; set; }
        public double Amount { get; set; }
        public string CompletedBy { get; set; }
    }
}