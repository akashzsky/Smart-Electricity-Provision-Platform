using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using SMP_WebApplication.Models;
using System.Data.Entity;

namespace SMP_WebApplication.Controllers
{
    public class AdminController : Controller
    {
        SMPEntities entities = new SMPEntities();
        public ActionResult Index()
        {
            HttpCookie c = Request.Cookies["Admin"];
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
            user.RoleId = 1;
            entities.UserProfiles.Add(user);
            entities.SaveChanges();
            return RedirectToAction("login", "Admin");
        }

        public ActionResult Login()
        {
            HttpCookie c = Request.Cookies["Admin"];

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
            HttpCookie c = Request.Cookies["Admin"];

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
                    HttpCookie cookie = new HttpCookie("Admin");
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
            HttpCookie c = Request.Cookies["Admin"];
            if (c == null)
            {
                return RedirectToAction("index");
            }

            // For Assigning coordinator Id for job Id 
            long JobId=0,CoordId=0,SuppId=0;
            ViewBag.Coord = 1;
            ViewBag.Supplier = 1;
            if (Request["AssignCoordinator"] != null)
            {
                JobId = long.Parse(Request["AssignCoordinator"]);
                using (SMPEntities contextt = new SMPEntities())
                {
                    var user  = contextt.UserProfiles.FirstOrDefault(x => x.RoleId == 2 && x.Assigned == 0);
                    

                    if (user != null)
                    {
                        CoordId = user.ID;
                        user.Assigned = 1;
                        contextt.SaveChanges();
                    }
                    else
                    {
                        ViewBag.Coord = 0;
                    }

                  
                }

                if (CoordId != 0)
                {
                    var job = new Job() { ID = JobId, JobStatusID = 2, CoordinatorID = CoordId };
                    if (job != null)
                    {
                        entities.Jobs.Attach(job);
                        entities.Entry(job).Property(x => x.JobStatusID).IsModified = true;
                        entities.Entry(job).Property(x => x.CoordinatorID).IsModified = true;

                        entities.SaveChanges();
                        entities.Entry(job).State = EntityState.Detached;
                    }
                }
            }
            if (Request["AssignSupplier"] != null)
            {
                JobId = long.Parse(Request["AssignSupplier"]);
                using (SMPEntities contextt = new SMPEntities())
                {
                    var user = contextt.UserProfiles.FirstOrDefault(x => x.RoleId == 4 && x.Assigned == 0);

                    if (user != null)
                    {
                        SuppId = user.ID;

                        user.Assigned = 1;

                        contextt.SaveChanges();
                    }
                    else
                    {
                        ViewBag.Supplier = 0;
                    }


                }

                if (SuppId != 0)
                {
                    var job = new Job() { ID = JobId, JobStatusID = 3, SupplierID = SuppId };
                    if (job != null)
                    {
                        entities.Jobs.Attach(job);
                        entities.Entry(job).Property(x => x.JobStatusID).IsModified = true;
                        entities.Entry(job).Property(x => x.SupplierID).IsModified = true;
                        entities.SaveChanges();
                        entities.Entry(job).State = EntityState.Detached;
                    }
                }
            }


            var list = entities.Jobs.ToList();

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
            HttpCookie cookie = new HttpCookie("Admin");
            cookie.Value = "";

            cookie.Expires = DateTime.Now.AddDays(-1);
            //otherwise the cookie acts like a Session - temporary cookie

            Response.Cookies.Add(cookie);
            return RedirectToAction("Login");
        }

        public ActionResult Customers()
        {
            var list = entities.UserProfiles.Where(x => x.RoleId == 3).ToList();

            
            return View(list);
        }

        public ActionResult Coordinators()
        {
            var list = entities.UserProfiles.ToList();
            //ViewBag.UnAssigned = list;
            //var list2 = entities.UserProfiles.Where(x => x.RoleId == 2 && x.Assigned == 1).ToList();
            //ViewBag.Assigned = list2;
            return View(list);
        }
        public ActionResult Suppliers()
        {
            var list = entities.UserProfiles.Where(x=>x.RoleId==4).ToList();
            //ViewBag.UnAssigned = list;
            //var list2 = entities.UserProfiles.Where(x => x.RoleId == 2 && x.Assigned == 1).ToList();
            //ViewBag.Assigned = list2;
            return View(list);
        }
        public ActionResult UserProfile()
        {
            HttpCookie c = Request.Cookies["Admin"];
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