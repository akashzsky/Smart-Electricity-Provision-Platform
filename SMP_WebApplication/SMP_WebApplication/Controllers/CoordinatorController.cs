using SMP_WebApplication.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMP_WebApplication.Controllers
{
    public class CoordinatorController : Controller
    {
        // GET: Coordinator
        SMPEntities entities = new SMPEntities();
        public ActionResult Index()
        {
            HttpCookie c = Request.Cookies["Coordinator"];
            if (c != null)
            {
                return RedirectToAction("welcome");
            }
            else
                return View();
        }

        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(UserProfile user)
        {
            user.RoleId = 2;
            entities.UserProfiles.Add(user);
            entities.SaveChanges();
            return RedirectToAction("login", "Coordinator");
        }

        public ActionResult Login()
        {
            HttpCookie c = Request.Cookies["Coordinator"];

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
            HttpCookie c = Request.Cookies["Coordinator"];

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
                if (User != null)
                    found = true;

                if (found == true)
                {
                    HttpCookie cookie = new HttpCookie("Coordinator");
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
            HttpCookie c = Request.Cookies["Coordinator"];
            if (c == null)
            {
                return RedirectToAction("index");
            }

            // For Assigning coordinator Id for job Id 
            long? JobId = 0, CoordId = 0, SuppId = 0;
            if (Request["Complete"] != null)
            {
                JobId = long.Parse(Request["Complete"]);
                long custid=0;
                long jobtypeid = 0;
                using (SMPEntities contextt = new SMPEntities())
                {
                    var Job = contextt.Jobs.FirstOrDefault(x => x.ID==JobId);
                    {
                        custid = Job.CusID;
                        CoordId = Job.CoordinatorID;
                        SuppId = Job.SupplierID;
                        jobtypeid = Job.WorkTypeID;
                        contextt.Jobs.Remove(Job);
                        contextt.SaveChanges();
                    }

                }
                if (jobtypeid == 1)
                {
                    using (SMPEntities contextt = new SMPEntities())
                    {

                        var Data = new Power_Consumption();


                        if (Data != null)
                        {
                            Data.CustID = custid;
                            Data.Usage = 100;
                            contextt.Power_Consumption.Add(Data);

                            contextt.SaveChanges();
                        }

                    }

                }
                using (SMPEntities contextt = new SMPEntities())
                {
                    var user = contextt.UserProfiles.FirstOrDefault(x => x.ID == CoordId);
                    if (user != null)
                    {
                        user.Assigned = 0;
                        contextt.SaveChanges();
                    }
                  
                }
                using (SMPEntities contextt = new SMPEntities())
                {
                    var user = contextt.UserProfiles.FirstOrDefault(x => x.ID == SuppId);
                    if (user != null)
                    {
                        user.Assigned = 0;
                        contextt.SaveChanges();
                    }

                }
            }
            

            var list = entities.Jobs.Where(x=>x.CoordinatorID!=null).ToList();

            List<DisplayJob> Dlist = new List<DisplayJob>();

            foreach (var x in list)
            {
                DisplayJob job = new DisplayJob();
                job.ID = x.ID;
                job.CusID = x.CusID;
                job.JobStatus = entities.JobStatus.FirstOrDefault(y => y.ID == x.JobStatusID).Name;
                job.ReceiveDate = x.ReceiveDate;
                job.SupplierID = x.SupplierID;
                job.WorkType = entities.WorkTypes.FirstOrDefault(y => y.ID == x.WorkTypeID).Name;
                job.CompletedBy = x.CompletedBy;
                job.CompletedDate = x.CompletedDate;
                job.Amount = entities.Finances.FirstOrDefault(y => y.ID == x.FinanceID).Sale;
                job.CoordinatorID = x.CoordinatorID;

                Dlist.Add(job);
            }

            return View(Dlist);
        }


        public ActionResult Logout()
        {
            HttpCookie cookie = new HttpCookie("Coordinator");
            cookie.Value = "";

            cookie.Expires = DateTime.Now.AddDays(-1);
            //otherwise the cookie acts like a Session - temporary cookie

            Response.Cookies.Add(cookie);
            return RedirectToAction("Login");
        }
        public ActionResult UserProfile()
        {
            HttpCookie c = Request.Cookies["Coordinator"];
            if (c != null)
            {
                var email = c.Value;
                var user = entities.UserProfiles.FirstOrDefault(x => x.Email == email);

                return View(user);
            }
            return View();
        }
    }
}