using SMP_WebApplication.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMP_WebApplication.Controllers
{
    public class CustomerController : Controller
    {
        // Some Important Methods
        static long W_install=0, W_remove=0, W_reallocate=0, W_exchange=0;






        // GET: Customer
        SMPEntities entities = new SMPEntities();
        public ActionResult Index()
        {
            HttpCookie c = Request.Cookies["Username"];
            if (c != null)
            {
                return RedirectToAction("welcome");
            }
            else
                return RedirectToAction("Index", "Home");
            //return View();
        }

        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(UserProfile user)
        {
            user.RoleId = 3;
            entities.UserProfiles.Add(user);
            entities.SaveChanges();
            return RedirectToAction("login");
        }

        public ActionResult Login()
        {
            HttpCookie c = Request.Cookies["Username"];

            if (c != null)
            {

                ViewBag.UserId = c.Value;
                return RedirectToAction("Welcome");
            }
            return View();
        }

        [HttpPost]
        public ActionResult Login(FormCollection frm)
        {
            HttpCookie c = Request.Cookies["Username"];

            if (c != null)
            {

                ViewBag.UserId = c.Value;
                return RedirectToAction("Welcome");
            }


            if (frm["btnLogin"] != null)
            {
                string username = frm["tbUsername"];
                string password = frm["tbPassword"];

                //do some real database check...
                var user = entities.UserProfiles.FirstOrDefault(x => x.Email == username);

                bool found = false;
                if (User!= null)
                        found = true;
                
                if (found == true)
                {
                    HttpCookie cookie = new HttpCookie("Username");
                    cookie.Value = username;


                    string chk = frm["chkRemember"];

                    if (chk != null)
                        cookie.Expires = DateTime.Now.AddDays(7);

                    Response.Cookies.Add(cookie);
                    return RedirectToAction("Welcome");
                }
                else
                {
                    ViewBag.Message = "Invalid Credentials";
                }

            }

            return View();
        }

        public ActionResult Welcome()
        {
            HttpCookie c = Request.Cookies["Username"];
            if (c == null)
            {
                return RedirectToAction("index");
            }
         
            return View();
        }
        public ActionResult Logout()
        {
            HttpCookie cookie = new HttpCookie("Username");
            cookie.Value = "";

            cookie.Expires = DateTime.Now.AddDays(-1);
            //otherwise the cookie acts like a Session - temporary cookie

            Response.Cookies.Add(cookie);
            return RedirectToAction("Login");
        }
        public ActionResult Helpdesk()
        {
            
            return View();
        }
        
        [HttpGet]
        public ActionResult NewRequest()
        {
            HttpCookie c = Request.Cookies["Username"];
            string username = null;
            long id;
            if (c != null)
            {
                username = c.Value;
            }
            username = c.Value;
            var user = entities.UserProfiles.FirstOrDefault(x => x.Email == username);
            id = user.ID;
            var job = entities.Jobs.FirstOrDefault(x =>x.CusID==id);
            ViewBag.Request_In_Job = 0;
            if (job != null)
            {
                ViewBag.Request_In_Job = 1;
            }
            //var job2 = entities.Jobs.FirstOrDefault(x => x.CusID == id && x.WorkTypeID == 2);
            //ViewBag.Insatll_Request_In_Job = 0;
            //if (job != null)
            //{
            //    ViewBag.Exchange_Request_In_Job = 1;
            //}
            //var job3 = entities.Jobs.FirstOrDefault(x => x.CusID == id && x.WorkTypeID == 1);
            //ViewBag.Insatll_Request_In_Job = 0;
            //if (job != null)
            //{
            //    ViewBag.Insatll_Request_In_Job = 1;
            //}

            return View();
        }
        [HttpPost]
        public ActionResult NewRequest(FormCollection frm)
        {
            string jobType = Request["Job"];

            switch (jobType)
            {
                case "Install":
                    return RedirectToAction("Install");
                    
                case "Exchange":
                    return RedirectToAction("Exchange");
                    
                case "Remove":
                    return RedirectToAction("Remove");
                    
                case "Re-Allocate":
                    return RedirectToAction("ReAllocate");
                    
            }
            return View();
        }

        public ActionResult Install()
        {
            W_install = 1;
            return View();
        }

        public ActionResult Checkout(string myid)
        {
            HttpCookie c = Request.Cookies["Username"];
            string username=null;
            long id;
            UserAddress ShowAddress = new UserAddress() ;
            
            // Retrieving UserId from the username and also fetching the user address.
            if (c != null)
            {
                username = c.Value;
                var user = entities.UserProfiles.FirstOrDefault(x => x.Email == username);
                id = user.ID;
                ShowAddress = entities.UserAddresses.FirstOrDefault(x => x.UserID == id);
            }
            else
                return RedirectToAction("Index", "Customer");


            //when you customer is saving a new address, it should be saved to UserAddress table in database
            if (Request["SaveAdd"] != null)
            {

                string House = Request["HouseNo"];
                string Street = Request["Street"];
                string Area = Request["Area"];
                string State = Request["State"];
                long Pin = long.Parse(Request["Pin"]);
                long UserID = id;

                //Creating a Adress Object for the User
                UserAddress Address = new UserAddress();
                Address.House = House;
                Address.Pin = Pin;
                Address.State = State;
                Address.Street = Street;
                Address.Area = Area;
                Address.UserID = UserID;
                // Creation done




                //As the worktype Id is saved in cookie.

                Job job = new Job();
                long val = long.Parse(myid);
                job = CustomerController.CreateNewJob(UserID, long.Parse(myid), 1);

                //Now add this New Job to the Job Table
                if (job != null)
                {
                    entities.Jobs.Add(job);
                    entities.SaveChanges();
                }
                //Addition of new Job Done.//

                //Now Saving the Address.
                if (Address != null)
                {

                    entities.UserAddresses.Add(Address);
                    entities.SaveChanges();

                    //As the address is saved now redirect to Complete Webpage
                    return RedirectToAction("Complete", "Customer");
                }
                
            }
            ViewBag.Address = ShowAddress;

            if (Request["PlaceOrder"] != null)
            {

                Job job = new Job();

                job = CustomerController.CreateNewJob(id,long.Parse(myid), 1);
                //Job NewJob = new Job();
                //NewJob.CusID = id;
                //NewJob.JobStatusID = 1;
                //NewJob.WorkTypeID = W_install;

                //NewJob.ReceiveDate = DateTime.Now.Date;
                //NewJob.CompletedBy = default(string);
                //NewJob.CompletedDate = default(DateTime);
                //NewJob.CoordinatorID = default(long);
                //NewJob.FinanceID = default(long);
                //NewJob.SupplierID = default(long);
                //NewJob.ProductID = default(long);

                //Now add this New Job to the Job Table
                if (job != null)
                {
                    entities.Jobs.Add(job);
                    entities.SaveChanges();
                }
                
                return RedirectToAction("Complete", "customer");
            }
            return View(ShowAddress);
        }

        private static Job CreateNewJob(long userID, long workTypeId, int v)
        {
            //Creating the Mater Job Object
            Job NewJob = new Job();
            NewJob.CusID = userID;
            NewJob.JobStatusID = 1;
            NewJob.WorkTypeID = workTypeId;

            NewJob.ReceiveDate = DateTime.Now.Date;
            //NewJob.CompletedBy = default(string);
            //NewJob.CompletedDate = default(DateTime);
            //NewJob.CoordinatorID = default(long);
            SMPEntities entity = new SMPEntities();
            NewJob.FinanceID = entity.Finances.FirstOrDefault(x => x.WorkTypeID == workTypeId).ID;
            //NewJob.SupplierID = default(long);
            //NewJob.ProductID = default(long);
            //Creation Done.
            return NewJob;

        }
        private string ConvertViewToString(string viewName)
        {
            // ViewData.Model = model;
            using (StringWriter writer = new StringWriter())
            {
                ViewEngineResult vResult = ViewEngines.Engines.FindPartialView(ControllerContext, viewName);
                ViewContext vContext = new ViewContext(this.ControllerContext, vResult.View, ViewData, new TempDataDictionary(), writer);
                vResult.View.Render(vContext, writer);
                return writer.ToString();
            }
        }
        public ActionResult Complete()
        {
            W_install = 0;
            W_exchange = 0;
            W_reallocate = 0;
            W_remove = 0;
           
            HttpCookie c = Request.Cookies["Username"];
            string username = null;
         
            // Retrieving UserId from the username and also fetching the user address.

            SMPEntities context = new SMPEntities();
            if (c != null)
            {
                username = c.Value;
                
            }
            string HtmlString = ConvertViewToString("~/Views/Customer/Email.cshtml");
            bool m = Email.Send(username, "Your Order Confirmation", HtmlString);
            return View();
        }


        public ActionResult Exchange()
        {
            W_exchange = 2;
            return RedirectToAction("Checkout", "Customer", new { myid = "2" });
        }
        public ActionResult Remove()
        {
            W_remove = 3;
            //string work = "3";
            return RedirectToAction("Checkout","Customer", new { myid = "3" });
        }
        public ActionResult ReAllocate()
        {
            W_reallocate = 4;
            return RedirectToAction("Checkout", "Customer", new { myid = "4" });
        }
        public ActionResult Status()
        {
            HttpCookie c = Request.Cookies["Username"];
            string username = null;
            long id=0;
            UserAddress ShowAddress = new UserAddress();
            ViewBag.Status_Result = 0;
            ViewBag.CurrStatus = "First make a new request!";
            // Retrieving UserId from the username and also fetching the user address.
            if (c != null)
            {
                username = c.Value;
                var user = entities.UserProfiles.FirstOrDefault(x => x.Email == username);
                id = user.ID;
                ShowAddress = entities.UserAddresses.FirstOrDefault(x => x.UserID == id);
                Job job = entities.Jobs.FirstOrDefault(x => x.CusID == id);
                if (job != null)
                {
                    var status = entities.JobStatus.FirstOrDefault(x => x.ID == job.JobStatusID);
                    if (status != null)
                    {
                        ViewBag.CurrStatus = status.Name;
                    }
                }
            }
            else
                ViewBag.Status_Result=1;

            
            //if (id != 0)
            //{
            //    Job job = entities.Jobs.FirstOrDefault(x => x.CusID == id);
            //    if (job != null)
            //    {
            //        var status = entities.JobStatus.FirstOrDefault(x => x.ID == job.JobStatusID);
            //        if (status != null)
            //        {
            //            ViewBag.CurrStatus = status.Name;
            //        }
            //    }

            //}


            return View();
        }
        public ActionResult Billing()
        {

            return View();
        }
        public ActionResult Recharge()
        {

            return View();
        }

        public ActionResult UserProfile()
        {
            HttpCookie c = Request.Cookies["Username"];
            var email = c.Value;
            var user = entities.UserProfiles.FirstOrDefault(x => x.Email == email);

            ViewBag.Usage = " You don't have any connection Yet!";
            ViewBag.find = 0;
            if (user != null)
            {
                var data = entities.Power_Consumption.FirstOrDefault(x => x.CustID == user.ID);
                if (data != null)
                {
                    ViewBag.find = 1;
                    ViewBag.Usage = "Your Electricity Consumption for Current Month is "+data.Usage.ToString()+" unit";
                }
            }
            

            return View(user);
        }


    }
    
}