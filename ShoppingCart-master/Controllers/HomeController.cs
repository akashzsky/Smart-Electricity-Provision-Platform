using System;
using ShoppingKart.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.IO;
using System.Drawing;

namespace ShoppingKart.Controllers
{
    public class HomeController : Controller
    {
        string conStr = ConfigurationManager.ConnectionStrings["ConStr"].ConnectionString;

        // GET: Home
        public ActionResult Signup()
        {
            return View();
        }
        [HttpPost]
        public ActionResult Signup(FormCollection frm)
        {
            string Firstname = frm["Firstname"];
            string Lastname = frm["Lastname"];
            string Username = frm["Username"];
            string Password = frm["Password"];

            SqlConnection con = new SqlConnection(Settings.ConStr);
            SqlCommand cmd = new SqlCommand("INSERT INTO UserDetail values (@Firstname, @Lastname, @Username, @Password)", con);

            cmd.Parameters.Add(new SqlParameter("Firstname", Firstname));
            cmd.Parameters.Add(new SqlParameter("Lastname", Lastname));
            cmd.Parameters.Add(new SqlParameter("Username", Username));
            cmd.Parameters.Add(new SqlParameter("Password", Password));

            con.Open();
            cmd.ExecuteNonQuery();
            con.Close();
            ViewBag.Message = "Done Signing Up!";

            return View();
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
                SqlConnection con = new SqlConnection(Settings.ConStr);
                SqlCommand cmd = new SqlCommand("select * from UserDetail where Username=@Username  and Password=@Password", con);
                cmd.Parameters.Add(new SqlParameter("Username", username));
                cmd.Parameters.Add(new SqlParameter("Password", password));
                con.Open();
                bool found = false;
                SqlDataReader r = cmd.ExecuteReader();
                var User = new User();
                while (r.Read())
                {
                    User.UserId = (int)r["UserId"];
                    User.Username = r["Username"].ToString();
                    User.password = (string)r["Password"];
                    User.firstname = r["Firstname"].ToString();
                    User.lastname = (string)r["Lastname"];
                    if (User.Username != null)
                        found = true;

                }
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
        public ActionResult index(string id)
        {
            HttpCookie c = Request.Cookies["Username"];
            if(c!=null)
            {
                return RedirectToAction("welcome");
            }       

            //if there is no user logged-in then
            var list = new List<Product>();
            SqlConnection con = new SqlConnection(Settings.ConStr);
            SqlCommand cmd = new SqlCommand("select * from Product", con);
            //cmd.Parameters.Add(new SqlParameter("Username", c.Value));
            con.Open();

            SqlDataReader r = cmd.ExecuteReader();

            while (r.Read())
            {
                var Pro = new Product();
                Pro.Product_ID = (int)r["Product_ID"];
                Pro.ProductName = r["ProductName"].ToString();
                Pro.Description = (string)r["Description"];
                Pro.Price = (double)r["Price"];
                Pro.Picture = (string)r["Picture"];
                list.Add(Pro);
            }
            if (Request["Checkout"] != null)
            {
                return RedirectToAction("Login");
            }

            //Its time for pagenation
            var pagelist = new List<Product>();
            int start, end;

            int x,flag=0;
            if (int.TryParse(id, out x))
                flag = 1;
            
            if(flag==1)
            {
                start = (int.Parse(id) - 1) * 5;
                end = (int.Parse(id) * 5) - 1;
                ViewBag.Currpage = int.Parse(id);
                //

               
            }
            else
            {
                start = 0;
                ViewBag.Currpage = 1;
                end = 4;

            }

            for (int i = start; i < end; i++)
            {
                if (i > list.Count - 1)
                    break;
                pagelist.Add(list[i]);
            }
            int total =list.Count;
            int rem = total % 10;
            ViewBag.start = 1;

            //if (rem <= 5)
            //    ViewBag.end = total / 5;
            //else
                ViewBag.end = (total / 5)+1;

            return View(pagelist);
        }
        //[HttpPost]
        //public ActionResult index(FormCollection frm)
        //{
          
        //    //int br = 0;
        //    for (int i = 0; i < 100; i++)
        //    {
        //        string cheke = Request["chk+" + i.ToString()];
        //        if (cheke == "on")
        //        {
        //           // br = 1;
        //            return RedirectToAction("Login");
        //        }
        //    }

        //    return View();
        //}

        public ActionResult Welcome(string id,FormCollection frm)
        {
            HttpCookie c = Request.Cookies["Username"];

           
                var CartList = new List<User_cart>();
               // for (int i = 1; i < 1000; i++)
                //{
                    string cheke = Request["AddToCart"];
            if (cheke != null)
            {
                var cart = new User_cart();
                string Username = (string)c.Value;
                int Product_ID, Quantity, UserId = 0;
                Product_ID = int.Parse(cheke);
                cart.Product_ID = Product_ID;
                string d = Request["Qty+" + cheke];
                Quantity = int.Parse(Request["Qty+" + cheke]);
                cart.qty = Quantity;

                SqlConnection con = new SqlConnection(Settings.ConStr);
                SqlCommand cmd1 = new SqlCommand("select UserId from UserDetail where Username=@Username", con);
                cmd1.Parameters.Add(new SqlParameter("Username", Username));
                con.Open();

                SqlDataReader r = cmd1.ExecuteReader();

                while (r.Read())
                {
                    UserId = (int)r["UserId"];
                    cart.UserId = UserId;
                }
                con.Close();
                CartList.Add(cart);

                //first checking whether the item is already added to cart or not--
                SqlConnection ccon = new SqlConnection(Settings.ConStr);
                SqlCommand ccmd1 = new SqlCommand("select Quantity from Usercart where Product_ID=@Product_ID AND UserId=@UserId", ccon);
                ccmd1.Parameters.Add(new SqlParameter("Product_ID", Product_ID));
                ccmd1.Parameters.Add(new SqlParameter("UserId", UserId));
                ccon.Open();

                SqlDataReader rr = ccmd1.ExecuteReader();
                int qty = 0;
                while (rr.Read())
                {
                    qty = (int)rr["Quantity"];

                }
                ccon.Close();

                //if productId is null means the item is not bben added previously do normal insert the product into cart

                if (qty == 0)
                {
                    SqlConnection con2 = new SqlConnection(Settings.ConStr);
                    SqlCommand cmd2 = new SqlCommand("INSERT INTO Usercart values (@UserId, @Product_ID, @Quantity)", con2);

                    cmd2.Parameters.Add(new SqlParameter("UserId", UserId));
                    cmd2.Parameters.Add(new SqlParameter("Product_ID", Product_ID));
                    cmd2.Parameters.Add(new SqlParameter("Quantity", Quantity));

                    con2.Open();
                    cmd2.ExecuteNonQuery();
                    con2.Close();
                    ViewBag.IsAdded = Product_ID;
                    
                }
                else
                {
                    Quantity = qty + Quantity;
                    SqlConnection ccon2 = new SqlConnection(Settings.ConStr);
                    SqlCommand ccmd2 = new SqlCommand("UPDATE Usercart SET Quantity=@Quantity where Product_ID=@Product_ID", ccon2);
                    ccmd2.Parameters.Add(new SqlParameter("Quantity", Quantity));
                    ccmd2.Parameters.Add(new SqlParameter("Product_ID", Product_ID));


                    ccon2.Open();
                    ccmd2.ExecuteNonQuery();
                    ccon2.Close();
                    //  ViewBag.IsAdded = Product_ID;
                }
            }
                  



            var list = new List<Product>();
            if (Request["btnLogOut"] != null)
            {
                HttpCookie cookie = new HttpCookie("Username");
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
                SqlConnection con = new SqlConnection(Settings.ConStr);
                SqlCommand cmd = new SqlCommand("select * from Product", con);
                //cmd.Parameters.Add(new SqlParameter("Username", c.Value));
                con.Open();

                SqlDataReader r = cmd.ExecuteReader();

                while (r.Read())
                {
                    var Pro = new Product();
                    Pro.Product_ID = (int)r["Product_ID"];
                    Pro.ProductName = r["ProductName"].ToString();
                    Pro.Description = (string)r["Description"];
                    Pro.Price = (double)r["Price"];
                    Pro.Picture = (string)r["Picture"];
                    list.Add(Pro);
                }

            }
            else
            {
                return RedirectToAction("Login");
            }
            //Its time for pagenation
            var pagelist = new List<Product>();
            int start, end;
            int x, flag = 0;
            if (int.TryParse(id, out x))
                flag = 1;
            if (flag==0)
            {
                start = 0;
                ViewBag.Currpage = 1;
                end = 4;
            }
            else
            {
                start = (int.Parse(id) - 1) * 5;
                end = (int.Parse(id) * 5) - 1;
                ViewBag.Currpage = int.Parse(id);
            }

            for (int i = start; i < end; i++)
            {
                if (i > list.Count - 1)
                    break;
                pagelist.Add(list[i]);
            }
            int total = list.Count;
            int rem = total % 10;
            ViewBag.start = 1;

            //if (rem <= 5)
            //    ViewBag.end = total / 5;
            //else
                ViewBag.end = (total / 5) + 1;

            return View(pagelist);
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

        public ActionResult Checkout()
        {
            HttpCookie c = Request.Cookies["Username"];
            string Username = c.Value;
            int UserId = 0;
            SqlConnection con2 = new SqlConnection(Settings.ConStr);
            SqlCommand cmd1 = new SqlCommand("select UserId from UserDetail where Username=@Username", con2);
            cmd1.Parameters.Add(new SqlParameter("Username", Username));
            con2.Open();

            SqlDataReader r = cmd1.ExecuteReader();

            while (r.Read())
            {
                UserId = (int)r["UserId"];
            }
            con2.Close();
            if (Request["SaveAdd"]!=null)
            {
                string Address = Request["AddText"];
                if (Address != "")
                {
                    SqlConnection con = new SqlConnection(Settings.ConStr);
                    SqlCommand cmd = new SqlCommand("INSERT INTO UserAddress values (@UserId, @Address)", con);

                    cmd.Parameters.Add(new SqlParameter("UserId", UserId));
                    cmd.Parameters.Add(new SqlParameter("Address", Address));

                    con.Open();
                    cmd.ExecuteNonQuery();
                    con.Close();
                }
            }

            var Laddress = new List<UserAddress>();
            SqlConnection conn = new SqlConnection(Settings.ConStr);
            SqlCommand cmdd = new SqlCommand("select * from UserAddress where UserId=@UserId", conn);
            cmdd.Parameters.Add(new SqlParameter("UserId", UserId));
            conn.Open();

            SqlDataReader r2 = cmdd.ExecuteReader();

            
            while (r2.Read())
            {
                var UserAddress = new UserAddress();
                UserAddress.Address = (string)r2["Address"];
                UserAddress.AddressId = (int)r2["Address_Id"];

                Laddress.Add(UserAddress);
            }
           
            if (Request["PlaceOrder"] != null)
            {
               
                    if (Request["SelectAdd"] != null)
                    {

                        //
                        string Address_Id = Request["SelectAdd"];
                        SqlConnection con3 = new SqlConnection(Settings.ConStr);
                        SqlCommand cmd2 = new SqlCommand("Select Address from UserAddress where Address_Id=@Address_Id", con3);

                        cmd2.Parameters.Add(new SqlParameter("Address_Id", Address_Id));
                        
                        con3.Open();
                        // cmd2.ExecuteNonQuery();
                        string Address=null;
                        SqlDataReader reader = cmd2.ExecuteReader();

                        while (reader.Read())
                        {
                            Address = (string)reader["Address"];
                        }
                        con3.Close();
                    
                    //Now sending Products from Usercart to OrderDetails before deleting items from cart after placing order
                    DateTime OrderTime = DateTime.Now;
                    double OrderTotal = Convert.ToDouble(Session["OrderTotal"]);
                    Session["OrderTime"] = OrderTime;
                    SqlConnection conOrder = new SqlConnection(Settings.ConStr);

                    //First capturing time of the Order placed
                        SqlCommand cmdOrder = new SqlCommand("INSERT INTO Orders values (@OrderTime,@UserId,@Address,@OrderTotal)", conOrder);

                        cmdOrder.Parameters.Add(new SqlParameter("OrderTime", OrderTime));
                        cmdOrder.Parameters.Add(new SqlParameter("UserId", UserId));
                        cmdOrder.Parameters.Add(new SqlParameter("Address", Address));
                        cmdOrder.Parameters.Add(new SqlParameter("OrderTotal", OrderTotal));
                    conOrder.Open();
                        cmdOrder.ExecuteNonQuery();
                        conOrder.Close();

                   
                    SqlCommand cmdOrderDetail = new SqlCommand("INSERT INTO OrderDetail (UserId,Product_ID,Quantity) select UserId,Product_ID,Quantity from Usercart where Usercart.UserId=@UserId", conOrder);
                    cmdOrderDetail.Parameters.Add(new SqlParameter("UserId", UserId));

                    SqlCommand cmdOrderDetail2 = new SqlCommand("UPDATE OrderDetail SET OrderId = (select OrderNo from Orders where OrderTime=@OrderTime) where OrderDetail.UserId=@UserId", conOrder);
                    cmdOrderDetail2.Parameters.Add(new SqlParameter("OrderTime", OrderTime));
                    cmdOrderDetail2.Parameters.Add(new SqlParameter("UserId", UserId));
                    conOrder.Open();
                    cmdOrderDetail.ExecuteNonQuery();
                    cmdOrderDetail2.ExecuteNonQuery();
                    conOrder.Close();

                   //table Order has been updated. Now we can delete data from the Usercart
                        SqlConnection cconn = new SqlConnection(Settings.ConStr);
                        SqlCommand ccmd2 = new SqlCommand("DELETE FROM Usercart where UserId=@UserId", cconn);

                        ccmd2.Parameters.Add(new SqlParameter("UserId", UserId));

                        cconn.Open();
                        ccmd2.ExecuteNonQuery();
                        cconn.Close();

                        
                    Session["Address"] = Address;

                    // Email mail = new Email();
                    string HtmlString = ConvertViewToString("~/Views/Home/Email.cshtml");

                    bool m = Email.Send(Username, "Your Order Confirmation", HtmlString);
                    if (m == true)
                        ViewBag.email = "Check your mail";
                
                  
                        return RedirectToAction("OrderDetails");
                    //    break;
                    
                }
                else
                {
                    ViewBag.SelectError = "No Address has been selected! Please select one.";
                }

            }
            return View(Laddress);
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
        public ActionResult OrderDetails()
        {

           //First find UserId from the Username
            HttpCookie c = Request.Cookies["Username"];
            string Username = c.Value;
            int UserId = 0;
            
            SqlConnection con = new SqlConnection(Settings.ConStr);
            SqlCommand CmdUserId = new SqlCommand("select UserId from UserDetail where Username=@Username", con);
            CmdUserId.Parameters.Add(new SqlParameter("Username", Username));
            con.Open();

            SqlDataReader r = CmdUserId.ExecuteReader();

            while (r.Read())
            {
                UserId = (int)r["UserId"];
            }
            con.Close();

            //I have UserId From Now--We can Use it.

            //Here i will extract all the products from OrderDetail table for this UserId and convert this into a list of Product from the OrderDetails.
            var Lprod = new List<OrderDetail>();
            DateTime OrderTime = Convert.ToDateTime(Session["OrderTime"]);
            SqlCommand CmdOrders = new SqlCommand("select OrderTime,OrderNo,Address,OrderTotal From Orders where UserId=@UserId and OrderTime=@OrderTime", con);
            CmdOrders.Parameters.Add(new SqlParameter("UserId", UserId));
            CmdOrders.Parameters.Add(new SqlParameter("OrderTime", Session["OrderTime"]));

            con.Open();

            SqlDataReader r2 = CmdOrders.ExecuteReader();

            //string OrderTime, Address;
            //int OrderNo = 0;
           
            while (r2.Read())
            {
                ViewBag.OrderTime = r2["OrderTime"].ToString();
                ViewBag.OrderNo = (int)r2["OrderNo"];
                ViewBag.Address = (string)r2["Address"];
                ViewBag.OrderTotal=(double)r2["OrderTotal"];
            }
            con.Close();
            return View();
        

        }
        public ActionResult Mycart()
        {
            //var Lcart = new List<User_cart>();
            //var Lprod= new List<Product>();
            HttpCookie c = Request.Cookies["Username"];
            string Username="";
            if (c!=null)
             Username = c.Value;
            int UserId = 0;
            SqlConnection con2 = new SqlConnection(Settings.ConStr);
            SqlCommand cmd1 = new SqlCommand("select UserId from UserDetail where Username=@Username", con2);
            cmd1.Parameters.Add(new SqlParameter("Username", Username));
            con2.Open();

            SqlDataReader r = cmd1.ExecuteReader();

            while (r.Read())
            {
                UserId = (int)r["UserId"];
            }
            con2.Close();
           
            if (Request["Add"] != null)
            {
                int Product_ID = int.Parse(Request["Add"]);
                SqlConnection conn = new SqlConnection(Settings.ConStr);
                SqlCommand cmdd = new SqlCommand("select Product.*,Usercart.Quantity from Product,Usercart,UserDetail where Usercart.UserId=@UserId AND Usercart.UserId=UserDetail.UserId AND Usercart.Product_ID=Product.Product_ID AND Usercart.Product_ID=@Product_ID", conn);

                cmdd.Parameters.Add(new SqlParameter("UserId", UserId));
                cmdd.Parameters.Add(new SqlParameter("Product_ID", Product_ID));
                conn.Open();

                SqlDataReader rr = cmdd.ExecuteReader();
                int Quantity=0;
                while (rr.Read())
                {
                    Quantity= (int)rr["Quantity"];
                   
                }
                Quantity++;
                conn.Close();
                //SqlConnection conn = new SqlConnection(Settings.ConStr);
                SqlConnection conn1 = new SqlConnection(Settings.ConStr);
                SqlCommand cmd2 = new SqlCommand("Update Usercart set Quantity=@Quantity where UserId=@UserId AND Product_ID=@Product_ID", conn1);
                cmd2.Parameters.Add(new SqlParameter("Quantity", Quantity));
                cmd2.Parameters.Add(new SqlParameter("UserId", UserId));
                cmd2.Parameters.Add(new SqlParameter("Product_ID", Product_ID));

                conn1.Open();
                cmd2.ExecuteNonQuery();
                conn1.Close();
            }
            if (Request["Minus"] != null)
            {
                int Product_ID = int.Parse(Request["Minus"]);
                SqlConnection conn = new SqlConnection(Settings.ConStr);
                SqlCommand cmdd = new SqlCommand("select Product.*,Usercart.Quantity from Product,Usercart,UserDetail where Usercart.UserId=@UserId AND Usercart.UserId=UserDetail.UserId AND Usercart.Product_ID=Product.Product_ID AND Usercart.Product_ID=@Product_ID", conn);

                cmdd.Parameters.Add(new SqlParameter("UserId", UserId));
                cmdd.Parameters.Add(new SqlParameter("Product_ID", Product_ID));
                conn.Open();

                SqlDataReader rr = cmdd.ExecuteReader();
                int Quantity = 1;
                while (rr.Read())
                {
                    Quantity = (int)rr["Quantity"];

                }
                if (Quantity == 0)
                    Quantity = 0;
                else
                    Quantity--;
                conn.Close();
                //SqlConnection conn = new SqlConnection(Settings.ConStr);
                SqlConnection conn1 = new SqlConnection(Settings.ConStr);
                SqlCommand cmd2 = new SqlCommand("Update Usercart set Quantity=@Quantity where UserId=@UserId AND Product_ID=@Product_ID", conn1);
                cmd2.Parameters.Add(new SqlParameter("Quantity", Quantity));
                cmd2.Parameters.Add(new SqlParameter("UserId", UserId));
                cmd2.Parameters.Add(new SqlParameter("Product_ID", Product_ID));

                conn1.Open();
                cmd2.ExecuteNonQuery();
                conn1.Close();
            }
            if (Request["Remove"]!=null)
            {

                //
                int Product_ID = int.Parse(Request["Remove"]);
                SqlConnection conn = new SqlConnection(Settings.ConStr);
                SqlCommand cmd2 = new SqlCommand("DELETE FROM Usercart where UserId=@UserId AND Product_ID=@Product_ID", conn);

                cmd2.Parameters.Add(new SqlParameter("UserId", UserId));
                cmd2.Parameters.Add(new SqlParameter("Product_ID", Product_ID));

                conn.Open();
                cmd2.ExecuteNonQuery();
                conn.Close();
                //   break;
            }
            
            

            string cheke = Request["checkout"];
           
          //  HttpCookie c = Request.Cookies["Username"];
            
            var Lmycart = new List<CartProd>();
            SqlConnection con = new SqlConnection(Settings.ConStr);
            SqlCommand cmd = new SqlCommand("select Product.*,Usercart.Quantity from Product,Usercart,UserDetail where Usercart.UserId=@UserId AND Usercart.UserId=UserDetail.UserId AND Usercart.Product_ID=Product.Product_ID;", con);
            cmd.Parameters.Add(new SqlParameter("UserId", UserId));
            con.Open();

            SqlDataReader r2 = cmd.ExecuteReader();
             ViewBag.sum =0;
            ViewBag.TotalItems = 0;
            ViewBag.DeliveryCharge=0;

            while (r2.Read())
            {
                //var Pro = new Product();
                //var cart = new User_cart();
                var mycart = new CartProd();
                mycart.Product_ID= (int)r2["Product_ID"]; 
                mycart.ProductName = r2["ProductName"].ToString();
                mycart.Description = (string)r2["Description"];
                mycart.Price = (double)r2["Price"];
                mycart.Picture = (string)r2["Picture"];
                
                mycart.qty = (int)r2["Quantity"];
               
                //Lprod.Add(Pro);
                //Lcart.Add(cart);
                //mycart.Usercart = cart;
                //mycart.Product = Pro;
                ViewBag.TotalItems += mycart.qty;
                ViewBag.sum += (mycart.Price * mycart.qty);
                ViewBag.DeliveryCharge = 100;
                ViewBag.AmtPayable = ViewBag.sum + ViewBag.DeliveryCharge;
                Lmycart.Add(mycart);
            }

            

            Session["email_data"] =Lmycart;

            Session["OrderTotal"] = ViewBag.AmtPayable;
            if (Request["checkout"] != null)
            {
                if (ViewBag.AmtPayable == null)
                    ViewBag.CheckoutMsg = "Add atleast one Item to your cart!";
                else
                    return RedirectToAction("checkout");

            }
            return View(Lmycart);
        }

        public ActionResult YourOrders()
        {
            HttpCookie c = Request.Cookies["Username"];
            string Username = c.Value;
            int UserId = 0;
            SqlConnection con = new SqlConnection(Settings.ConStr);
            SqlCommand cmd1 = new SqlCommand("select UserId from UserDetail where Username=@Username", con);
            cmd1.Parameters.Add(new SqlParameter("Username", Username));
            con.Open();

            SqlDataReader r = cmd1.ExecuteReader();

            while (r.Read())
            {
                UserId = (int)r["UserId"];
            }
            con.Close();
            
            SqlCommand cmd2 = new SqlCommand("select * from Orders where UserId=@UserId", con);
            cmd2.Parameters.Add(new SqlParameter("UserId", UserId));
            
            var LorderDetail = new List<OrderDetail>();
            con.Open();
           SqlDataReader r2 = cmd2.ExecuteReader();
            while (r2.Read())
            {
                OrderDetail urorder = new OrderDetail();
                urorder.OrderNo=(int)r2["OrderNo"];
                urorder.OrderTime = (r2["OrderTime"]).ToString();
                urorder.OrderTotal =Convert.ToDouble(r2["OrderTotal"]);
                urorder.Address=(string)r2["Address"];
                LorderDetail.Add(urorder);
            }
            con.Close();

            if(Request["ViewDetail"]!=null)
            {
                int OrderNo = int.Parse(Request["ViewDetail"]);
               // var LorderDetail = new List<OrderDetail>();
                SqlCommand cmd3 = new SqlCommand("select * from Orders o1,OrderDetail o2 where o1.OrderNo=o2.OrderId and o1.orderNo=@OrderNo", con);
                cmd3.Parameters.Add(new SqlParameter("OrderNo", OrderNo));

                con.Open();
                SqlDataReader r3 = cmd3.ExecuteReader();
                while (r3.Read())
                {
                    OrderDetail urorderDetail = new OrderDetail();
                    urorderDetail.Product_ID = (int)r3["Product_ID"];
                    int? Product_ID = urorderDetail.Product_ID;
                    SqlCommand cmd4 = new SqlCommand("select * from Product where Product_ID=@Product_ID", con);
                    cmd3.Parameters.Add(new SqlParameter("Product_ID", Product_ID));
                    SqlDataReader r4 = cmd4.ExecuteReader();

                    while (r4.Read())
                    {
                        urorderDetail.ProductName = r4["ProductName"].ToString();
                        urorderDetail.Description = (string)r4["Description"];
                        urorderDetail.Price = (double)r4["Price"];
                        urorderDetail.Picture = (string)r4["Picture"];
                    }
                    urorderDetail.qty = (int)(r3["qty"]);
                    urorderDetail.OrderTime =(r3["OrderTime"]).ToString();

                    LorderDetail.Add(urorderDetail);
                }
                con.Close();
               
                //SqlCommand cmd = new SqlCommand("select * from Product", con);
               // return View();

            }

            return View(LorderDetail);
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
            return Content( DateTime.Now.ToString());
        }
    }
    public class OrderProduct
    {
        public OrderDetail OrderDetail { get; set; }
        public Orders Orders { get; set; }
       
    }
}