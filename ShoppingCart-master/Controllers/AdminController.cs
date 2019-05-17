using System;
using System.Collections.Generic;
using ShoppingKart.Models;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;
using System.Data.SqlClient;

namespace ShoppingKart.Controllers
{
    public class AdminController : Controller
    {

        // GET: Admin
        string conStr = ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString;
        public ActionResult Index()
        {
            return View();
        }
        public ActionResult Addproducts()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Addproducts(FormCollection frm, HttpPostedFileBase file)
        {
            string Picture = null;
            try
            {
                if (file.ContentLength > 0)
                {
                    //extract just the  name.jpeg of pic
                    string picname = Path.GetFileName(file.FileName);

                    //setting physical addrss of the pic file in server machine
                    string ServerPhysicalAddress = Path.Combine(Server.MapPath("~/Content/images"), picname);

                    //saving the file in server physical address
                    file.SaveAs(ServerPhysicalAddress);

                    Picture = picname;// just an temporary variable storing the picture name

                    //extracting image using physical address
                    Image img = Image.FromFile(ServerPhysicalAddress);

                    //Now doing some operations with height and width for saving the thumbnail-- this is copied bdw 😁
                    int imgHeight = 100;
                    int imgWidth = 100;
                    if (img.Width < img.Height)
                    {
                        //portrait image  
                        imgHeight = 100;
                        var imgRatio = (float)imgHeight / (float)img.Height;
                        imgWidth = Convert.ToInt32(img.Height * imgRatio);
                    }
                    else if (img.Height < img.Width)
                    {
                        //landscape image  
                        imgWidth = 100;
                        var imgRatio = (float)imgWidth / (float)img.Width;
                        imgHeight = Convert.ToInt32(img.Height * imgRatio);
                    }
                    //-- next line will extract thumbnail out of the image "img"
                    Image thumb = img.GetThumbnailImage(imgWidth, imgHeight, () => false, IntPtr.Zero);

                    //-- Saving thumb to the mentioned path folder with the same name as that of original pic"
                    thumb.Save(Path.Combine(Server.MapPath("~/Content/thumbnails"), picname));
                }
            }
            catch
            {
                ViewBag.file_error = "can't get the file!";
            }

            string ProductName = frm["name"];
            string Description = frm["description"];
            string Price = frm["price"];
            //  double x;
            SqlConnection con = new SqlConnection(Settings.ConStr);
            SqlCommand cmd = new SqlCommand("INSERT INTO Product values (@ProductName, @Description, @Price, @Picture)", con);

            cmd.Parameters.Add(new SqlParameter("ProductName", ProductName));
            cmd.Parameters.Add(new SqlParameter("Description", Description));
            cmd.Parameters.Add(new SqlParameter("Price", double.Parse(Price)));
            cmd.Parameters.Add(new SqlParameter("Picture", Picture));

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            ViewBag.Message = "Product Added Successfulluy! ";
            return View();
        }

        public ActionResult GetTime()
        {
            return Content(DateTime.Now.ToString());
        }
        public ActionResult ManageOrders()
        {
            SqlConnection con = new SqlConnection(Settings.ConStr);
            string shk = Request["processed"];
            if (Request["processed"] != null)
            {
                int OrderNo = int.Parse(Request["processed"]);
                SqlCommand cmd2 = new SqlCommand("update Orders set processed=1 where OrderNo=@OrderNo", con);
                cmd2.Parameters.Add(new SqlParameter("OrderNo", OrderNo));
                con.Open();
                cmd2.ExecuteNonQuery();
                con.Close();


                //email processing details to User

                SqlCommand cmd3 = new SqlCommand("select UserId from Orders where OrderNo=@OrderNo", con);
                cmd3.Parameters.Add(new SqlParameter("OrderNo", OrderNo));
                con.Open();

                SqlDataReader r_un = cmd3.ExecuteReader();
                int UserId = 0;
                while (r_un.Read())
                {
                    UserId = (int)r_un["UserId"];
                }
                con.Close();
                SqlConnection conn = new SqlConnection(Settings.ConStr);
                SqlCommand cmd4 = new SqlCommand("select Username from UserDetail where UserId=@UserId", conn);
                cmd4.Parameters.Add(new SqlParameter("UserId", UserId));
                conn.Open();


                SqlDataReader r_ud = cmd4.ExecuteReader();
                string Username = "";
                while (r_ud.Read())
                {
                    Username = (string)r_ud["Username"];
                }
                conn.Close();

                // string HtmlString = ConvertViewToString("~/Views/Admin/EmailProcessed.cshtml");

                bool m = Email.Send(Username, "Order Processed", "Congratulations! Your Order has been processed. Your Order will be placed to your delivery address within 2-3 working days.");
            }

            SqlCommand cmd1 = new SqlCommand("select * from Orders order by OrderTime desc", con);
            //            cmd1.Parameters.Add(new SqlParameter("Username", Username));
            con.Open();
            var list = new List<Orders>();
            SqlDataReader r = cmd1.ExecuteReader();

            while (r.Read())
            {
                Orders order = new Orders();
                order.OrderNo = (int)r["OrderNo"];
                order.UserId = (int)r["UserId"];
                order.OrderTime = r["OrderTime"].ToString();
                order.OrderTotal = Convert.ToDouble(r["OrderTotal"]);
                order.Address = (string)r["Address"];
                order.processed = Convert.ToBoolean(r["processed"]);
                list.Add(order);
            }
            con.Close();
            return View(list);
        }
        public ActionResult Login()
        {
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
                if (username == "Admin" && password == "Admin")
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
        public ActionResult Welcome(FormCollection frm)
        {
            HttpCookie c = Request.Cookies["Admin"];



            if (Request["btnLogOut"] != null)
            {
                HttpCookie cookie = new HttpCookie("Admin");
                cookie.Value = "";

                cookie.Expires = DateTime.Now.AddDays(-1);
                //otherwise the cookie acts like a Session - temporary cookie

                Response.Cookies.Add(cookie);
                return RedirectToAction("Login");
            }


            //  HttpCookie c = Request.Cookies["Username"];

            if (c != null)
            {
                ViewBag.UserId = c.Value;

            }
            else
            {
                return RedirectToAction("Login");
            }
            return View();

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
    }
}