using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using System.Web.Security;
using crypto;
using RetailKing.DataAcess;


namespace RetailKing.Controllers
{   
    public class LoginController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        //
        //
        // GET: /Login/
        public long subcategoryid;
       
        public ActionResult Index()
        {
            var ip = getClientIP();
            login px = new login();
            return View("Index", "_PosLayout",px);
        }

        public ActionResult LogOff(string json_str)
        {
            /* NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
               System.Web.Script.Serialization.JavaScriptSerializer j = new System.Web.Script.Serialization.JavaScriptSerializer();
                 Subjectx ax = new Subjectx();
                 ax = (Subjectx)j.Deserialize(json_str, typeof(Subjectx));*/

            FormsAuthentication.SignOut();
            return RedirectToAction("ajax", "Login", new { ReturnUrl = "/Home/SubCategories?json_str=" + json_str });
          
        }

        public ActionResult ajax(string ReturnUrl)
        {
            //.var details=GetMaxMindOmniData(ip);
            ViewData["Redirect"] = ReturnUrl;
            if (Request.Browser.IsMobileDevice)
            {
                return PartialView();
            }
            else
            {
                return PartialView("ajaxWide");
            }
          
        }

        public ActionResult Success(string name)
        {
            
            return View();
        }

        [HttpPost]
        public ActionResult voucher()
        {
           
            return RedirectToAction("ajax", "Login", new { ReturnUrl = "/Student/Credit"  });
        }

        [HttpPost]
        public ActionResult ajaxWide(login model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    login pass = Authenticate(model.username , model.password);
                    // string Respons = wwhh.GetShop(message);
                    switch (pass.accesslevel.Trim())
                    {
                        case "Student":
                            if (ReturnUrl.ToLower() != "/home/newadvert")
                            {
                                FormsAuthentication.SetAuthCookie(model.username , false);
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid Username or Password");
                            }
                            break;
                        case "Mambo":
                            FormsAuthentication.SetAuthCookie(model.username, false);
                            return Redirect(ReturnUrl);
                        case "Administrator":
                            break;
                        case "Supervisor":
                            break;
                        case "Suspended":
                            ModelState.AddModelError("", "Sorry Your Account State is Suspended");
                            break;
                        case "Invalid":
                            ModelState.AddModelError("", "Invalid Username or Password");
                            break;
                        case "No":
                            ModelState.AddModelError("", "Sorry that Account does not exist");
                            break;
                        case "00008":
                            ModelState.AddModelError("", "Sorry that Account does not exist.Please Register First ");
                            break;
                    }

                    return PartialView();
                }
                catch
                {
                    ModelState.AddModelError("", "Sorry System Busy try again later");
                    return PartialView();
                }
            }
            return PartialView();
        }

        public ActionResult ajaxTop()
        {
            if (Request.IsAuthenticated)
            {
                return RedirectToAction("Logged", "Login");  
            }
            else
            {
                return PartialView();
            }
               
        }
        [HttpPost]
        public ActionResult ajaxTop(login model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    login pass = Authenticate(model.username, model.password);
                    // string Respons = wwhh.GetShop(message);
                    switch (pass.accesslevel.Trim())
                    {
                        case "Student":
                            if (ReturnUrl.ToLower() != "/home/newadvert")
                            {
                                FormsAuthentication.SetAuthCookie(model.username , false);
                                return Redirect(ReturnUrl);
                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid Username or Password");
                            }
                            break;
                        case "Mambo":
                            FormsAuthentication.SetAuthCookie(model.username , false);
                            return RedirectToAction("Logged");
                        case "Administrator":
                            break;
                        case "Supervisor":
                            break;
                        case "Suspended":
                            ModelState.AddModelError("", "Sorry Your Account State is Suspended");
                            break;
                        case "Invalid":
                            ModelState.AddModelError("", "Invalid Username or Password");
                            break;
                        case "No":
                            ModelState.AddModelError("", "Sorry that Account does not exist");
                            break;
                        case "00008":
                            ModelState.AddModelError("", "Sorry that Account does not exist.Please Register First ");
                            break;
                    }

                    return PartialView();
                }
                catch
                {
                    ModelState.AddModelError("", "Sorry System Busy try again later");
                    return PartialView();
                }
            }
            return PartialView();
        }

        [HttpPost]
        public ActionResult index(login model, string returnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    login pass = Authenticate(model.username , model.password);
                   // string Respons = wwhh.GetShop(message);
                    switch (pass.accesslevel.Trim())
                    {
                        case "ADMINISTRATOR":
                            if (returnUrl != "/Home/NewAdvert")
                            {
                                FormsAuthentication.SetAuthCookie(model.username, false);
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                ModelState.AddModelError("", "Invalid Username or Password");
                            }
                            break;
                        case "CASHIER":
                                FormsAuthentication.SetAuthCookie(model.username, false);
                                return Redirect(returnUrl);
                        case "Administrator":
                            break;
                        case "Supervisor":
                            break;
                        case "Suspended":
                            ModelState.AddModelError("", "Sorry Your Account State is Suspended");
                            break;
                        case "Invalid":
                            ModelState.AddModelError("", "Invalid Username or Password");
                            break;
                        case "11108":
                            ModelState.AddModelError("", "Sorry that Account does not exist");
                            break;
                        case "00008":
                            ModelState.AddModelError("", "Sorry that Account does not exist. Please register first");
                            break;
                    }

                    return View("Index", "_PosLayout");
                }
                catch
                {
                    ModelState.AddModelError("", "Sorry System Busy try again later");
                    return View("Index", "_PosLayout");
                }
            }
            return View("Index", "_PosLayout");
        }

        public string PaymentSec(string Account, string Pin)
        {
            try
            {
                login pass = Authenticate(Account, Pin);
               
                switch (pass.accesslevel.Trim())
                {
                    case "Mambo":
                        return "True";
                    case "Suspended":
                        return "false";
                    case "Invalid":
                        return "false";
                    case "No":
                        return "false";
                    case "00008":
                        return "false";
                }
                return "false";
            }
            catch
            {
                return "failed";
            }
        }

        private login Authenticate(string Account, string Pin)
        {
            try
            {
                //int cnt;
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
                ST_encrypt en = new ST_encrypt();
                login px = new login();
               // Account = en.encrypt(Account, "214ea9d5bda5277f6d1b3a3c58fd7034");
               // Pin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                login acc = np.Getlogin(Account);
                if (acc == null)
                {
                    Pin = "00008";
                    return px;
                }
                if (acc.username.Trim() == Account.Trim() && acc.password.Trim() == Pin.Trim())
                {
                    if (acc.ManagerMenu == true)
                    {
                        px = acc;
                    }
                    else
                    {     
                    }
                }
                else
                {
                }
                return px;
            }
            catch(Exception e)
            {
                login px = new login();
                px.username = "error";
                return px;
            }
        }

        //
        // GET: /Account/Register
       
        public ActionResult Logged()
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
                var currentUserId = User.Identity.Name;
                ViewData["SubjectId"] = User.Identity.Name;
                var stud = np.Getlogin(currentUserId);
                return PartialView(stud);
            }
            else
            {
                return RedirectToAction("ajaxTop", "Login");
            }
        }

        private string GetMaxMindOmniData(string IP)
        {
            System.Uri objUrl = new System.Uri("http://geoip.maxmind.com/e?l=YOUR_LICENSE_KEY&i=" + IP);
            System.Net.WebRequest objWebReq;
            System.Net.WebResponse objResp;
            System.IO.StreamReader sReader;
            string strReturn = string.Empty;

            try
            {
                objWebReq = System.Net.WebRequest.Create(objUrl);
                objResp = objWebReq.GetResponse();

                sReader = new System.IO.StreamReader(objResp.GetResponseStream());
                strReturn = sReader.ReadToEnd();

                sReader.Close();
                objResp.Close();
            }
            catch (Exception ex)
            {
            }
            finally
            {
                objWebReq = null;
            }

            return strReturn;
        }

        private string getClientIP()
        {
            //WebOperationContext webContext = WebOperationContext.Current;

            string ip = string.Empty;
            try
            {
                ip = HttpContext.Request.ServerVariables["HTTP_X_FORWARDED_FOR"];
                if (!string.IsNullOrEmpty(ip))
                {
                    if (ip.IndexOf(",") > 0)
                    {
                        string[] allIps = ip.Split(',');
                        int le = allIps.Length - 1;
                        ip = allIps[le];
                    }
                }
                else
                {
                    ip = HttpContext.Request.UserHostAddress;
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }

            return ip;
        }
        
    }
}