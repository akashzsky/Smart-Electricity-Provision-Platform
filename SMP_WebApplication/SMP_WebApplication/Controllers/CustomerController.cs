using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace SMP_WebApplication.Controllers
{
    public class CustomerController : Controller
    {
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
                return RedirectToAction("Index", "HomeController");
            //return View();
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

        public ActionResult UserProfile()
        {
            HttpCookie c = Request.Cookies["Username"];
            var email = c.Value;
            var user = entities.UserProfiles.FirstOrDefault(x => x.Email == email);

            return View(user);
        }


    }
}