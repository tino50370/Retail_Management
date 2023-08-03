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
using System.Text.RegularExpressions;
using System.Globalization;


namespace RetailKing.Controllers
{   
    public class LoginController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        //
        //
        // GET: /Login/
        public long subcategoryid;
        bool invalid = false;

        public ActionResult Ajax()
        {
            var ip = getClientIP();
            login px = new login();
            return View("Index", "_AdminLayout", px);
        }

        public ActionResult Index()
        {
            var ip = getClientIP();
            login px = new login();
            return View("Index", "_LoginLayout", px);
        }

        public ActionResult RetailLogOff()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Login");
        }

        public ActionResult LogOff()
        {
            FormsAuthentication.SignOut();
            if (Request.IsAjaxRequest())
                return PartialView("PosLogin");
            return View("PosLogin", "_PosLayout");
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
   
        public ActionResult PosLogin()
        {
            if(Request.IsAjaxRequest())
                   return PartialView();
            return View("PosLogin","_PosLayout");
        }

        [HttpPost]
        public ActionResult PosLogin(login model, string ReturnUrl)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    login pass = PosAuthenticate(model.password);
                    // string Respons = wwhh.GetShop(message);
                    if (pass.PosUser == true)
                    {
                        switch (pass.accesslevel.Trim())
                        {
                            case "ADMINISTRATOR":
                            case "CASHIER":

                                var authTicket = new FormsAuthenticationTicket(
                                       1,
                                       pass.Firstname + "~" + pass.Location + "~" + pass.accesslevel,  //user id
                                       DateTime.Now,
                                       DateTime.Now.AddMinutes(120),  // expiry
                                       true,  //true to rememberme
                                       pass.accesslevel.Trim(), //roles 
                                       "/"
                                       );

                                //encrypt the ticket and add it to a cookie
                                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                                Response.Cookies.Add(cookie);
                                return RedirectToAction("Index","Pos");
                            case "Mambo":
                                FormsAuthentication.SetAuthCookie(pass.Firstname, false);
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
                            default:
                                ModelState.AddModelError("", "Invalid Username or Password");
                                break;
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", "Invalid Username or Password");
                    }
                    if (Request.IsAjaxRequest())
                        return PartialView();
                    return View("PosLogin", "_PosLayout");
                }
                catch
                {
                    ModelState.AddModelError("", "Invalid Username or Password");
                    if (Request.IsAjaxRequest())
                        return PartialView();
                    return View("PosLogin", "_PosLayout");
                }
            }
            ModelState.AddModelError("", "Invalid Username or Password");
            if (Request.IsAjaxRequest())
                return PartialView();
            return View("PosLogin", "_PosLayout");
        }

        [HttpPost]
        public ActionResult index(login model, string returnUrl)
        {


            if (ModelState.IsValid)
            {
                try
                {
                    if(model.password == null || model.username == null)
                    {
                        ModelState.AddModelError("", "Invalid Username or Password");
                        return View("Index", "_LoginLayout");
                    }
                    
                        login pass = Authenticate(model.username, model.password);
                        // string Respons = wwhh.GetShop(message);
                        if(pass.Status.Contains("Deleted"))
                        {
                            ModelState.AddModelError("", "Sorry, your account is no longer active");
                            return View("Index", "_LoginLayout");
                        }
                        switch (pass.accesslevel.Trim())
                        {
                            case "ADMINISTRATOR":
                          
                                //FormsAuthentication.SetAuthCookie(pass.username + "~" + pass.Location + "~" + pass.accesslevel, false);
                                var authTicket = new FormsAuthenticationTicket(
                                        1,
                                        "5-0001-0000000" + "~" + pass.username + "~" + pass.Location + "~" + pass.accesslevel,  //user id
                                        DateTime.Now,
                                        DateTime.Now.AddMinutes(120),  // expiry
                                        true,  //true to rememberme
                                        pass.accesslevel, //roles 
                                        "/"
                                        );
                                    HttpCookie cookies = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                                    Response.Cookies.Add(cookies);
                                    if (returnUrl == "" || returnUrl == null)
                                    {
                                        return RedirectToAction("Index", "RetailKing" );
                                    }
                                    else
                                    {
                                        return Redirect(returnUrl);
                                    }
                                
                                break;
                           // case "CASHIER":
                            case "SUPERVISOR":
                            case "MANAGER":
                                var authTicketo = new FormsAuthenticationTicket(
                                       1,
                                       "5-0001-0000000" + "~" + pass.username + "~" + pass.Location + "~" + pass.accesslevel,  //user id
                                       DateTime.Now,
                                       DateTime.Now.AddMinutes(120),  // expiry
                                       true,  //true to rememberme 
                                       pass.accesslevel, //roles 
                                       "/"
                                       );
                                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicketo));
                                Response.Cookies.Add(cookie);
                                if (returnUrl == "" || returnUrl == null)
                                {
                                    return RedirectToAction("Index", "RetailKing");
                                }
                                else
                                {
                                    return Redirect(returnUrl);
                                }

                            case "Invalid":
                                ModelState.AddModelError("", "Invalid Username or Password");
                                break;

                        }
                    
                    return View("Index", "_LoginLayout");
                }
                catch
                {
 
                    ModelState.AddModelError("", "Invalid Username or Password");
                    return View("Index", "_LoginLayout");
                    
                }
            }
            return View("Index", "_LoginLayout");
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
                NHibernateDataProvider np = new NHibernateDataProvider();
                ST_encrypt en = new ST_encrypt();
                login px = new login();
               // Account = en.encrypt(Account, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Pin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                login acc = np.Getlogin(Account);
                if (acc == null)
                {
                    Pin = "00008";
                    return px;
                }

                if (acc.Status.Contains("Deleted"))
                {
                       px = acc;
                }
                    

                if (acc.username.Trim() == Account.Trim() && acc.password.Trim() == Pin.Trim() && acc.Status.Contains("Active"))
                {              
                        px = acc;    
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

        public string  AuthenticateClient(string Account, string Pin)
        {
            try
            {
                //int cnt;
                NHibernateDataProvider np = new NHibernateDataProvider();
                ST_encrypt en = new ST_encrypt();
                ST_decrypt dc = new ST_decrypt();
                LoginModel px = new LoginModel();
                var oo = dc.st_decrypt("067d9583beef74bbba", "214ea9d5bda5277f6d1b3a3c58fd7034");
                string newAccount = en.encrypt(Account, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Pin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Customer acc = new Models.Customer();
                if (IsValidEmail(Account))
                {
                    acc = db.Customers.Where(u => u.Email.Trim() == newAccount.Trim()).FirstOrDefault();
                }
                else
                {
                    acc = db.Customers.Where(u => u.Phone1.Trim() == Account.Trim()).FirstOrDefault();
                }

                if (acc == null)
                {
                    Pin = "00008";
                    return Pin;
                }
                if (acc.Password.Trim() == Pin.Trim())
                {
                    if (acc.Image == null) acc.Image = "none";
                    px.User = acc.AccountCode + "~" + acc.Phone1.Trim();
                    px.Password = acc.AccountCode.Trim();
                    px.redirect = "User";
                }
                else
                {
                    px.redirect = "Password";
                }
                return px.User;
            }
            catch (Exception e)
            {
                LoginModel px = new LoginModel();
                px.redirect = "Error";
                return px.redirect;
            }
        }

        public string AuthenticateSupplier(string Account, string Pin)
        {
            try
            {
                //int cnt;
                NHibernateDataProvider np = new NHibernateDataProvider();
                ST_encrypt en = new ST_encrypt();
                ST_decrypt ee = new ST_decrypt();
                var yy = ee.st_decrypt("062a27b6d9dbea09b6", "214ea9d5bda5277f6d1b3a3c58fd7034");
                var een = en.encrypt("mganyiwa@gmail.com", "214ea9d5bda5277f6d1b3a3c58fd7034");
                var eph = en.encrypt("TG15MUZVANYA@GMAIL.COM", "214ea9d5bda5277f6d1b3a3c58fd7034");
                Account = en.encrypt(Account, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Pin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                AccountCredenitial acc = db.AccountCredenitials.Find(Account);
               // 0649a1f796935e37215779d04bb2af6bc111d029a8ddf6dbbb
                if (acc.AccountNumber.Trim() == Account.Trim() && acc.Pin.Trim() == Pin.Trim())
                {
                    if (acc.Active == true)
                    {
                        Pin = acc.Access + "," + ee.st_decrypt(acc.Question.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034") + "," + ee.st_decrypt(acc.Connection.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    }
                    else
                    {
                        Pin = "Suspended";
                    }

                }
                else
                {
                    Pin = "Invalid";


                   // bool so = BruteForceProtection("www",cnt);

                }
                return Pin;
            }
            catch
            {
                return "11108";
            }
        }

        public string AuthenticateMerchant(string Account, string Pin)
        {
            try
            {
                //int cnt;
                NHibernateDataProvider np = new NHibernateDataProvider();
                ST_encrypt en = new ST_encrypt();
                ST_decrypt ee = new ST_decrypt();
                var yy = ee.st_decrypt("065bebb495aeb4e820", "214ea9d5bda5277f6d1b3a3c58fd7034");
                var een = en.encrypt("Gain", "214ea9d5bda5277f6d1b3a3c58fd7034");
                // var eph = en.encrypt("0773108328", "214ea9d5bda5277f6d1b3a3c58fd7034");
                Account = en.encrypt(Account, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Pin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                var  xx = db.AccountCredenitials.Where(u => u.Question.Trim() == Account && u.Pin.Trim() == Pin ).ToList();
                AccountCredenitial acc = xx.Where(u => u.AccountNumber.Trim() != Account ).FirstOrDefault();
             //  && u.AccountNumber.Trim() != Account
                if (acc != null)
                {
                    if (acc.Active == true)
                    {
                        Pin = ee.st_decrypt(acc.AccountNumber.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034") + "," + ee.st_decrypt(acc.Question.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034") + "," + ee.st_decrypt(acc.Connection.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    }
                    else
                    {
                        Pin = "Suspended";
                    }

                }
                else
                {
                    Pin = "Invalid";


                    // bool so = BruteForceProtection("www",cnt);

                }
                return Pin;
            }
            catch
            {
                return "11108";
            }
        }

        private login PosAuthenticate(string Pin)
        {
            try
            {
                //int cnt;
                NHibernateDataProvider np = new NHibernateDataProvider();
                ST_encrypt en = new ST_encrypt();
                login px = new login();
                // Account = en.encrypt(Account, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Pin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                login acc = np.GetloginPW(Pin);
                if (acc == null)
                {
                    Pin = "00008";
                    return px;
                }
                if (acc.PosUser == true)
                {

                    acc.username = "";
                    acc.password = "";
                    px = acc;

                }
                else
                {
                }
                return px;
            }
            catch (Exception e)
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
                NHibernateDataProvider np = new NHibernateDataProvider();
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

        
        public bool IsValidEmail(string strIn)
        {
            invalid = false;
            if (String.IsNullOrEmpty(strIn))
                return false;

            // Use IdnMapping class to convert Unicode domain names. 
            try
            {
                strIn = Regex.Replace(strIn, @"(@)(.+)$", this.DomainMapper,
                                      RegexOptions.None, TimeSpan.FromMilliseconds(200));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }

            if (invalid)
                return false;

            // Return true if strIn is in valid e-mail format. 
            try
            {
                return Regex.IsMatch(strIn,
                      @"^(?("")(""[^""]+?""@)|(([0-9a-z]((\.(?!\.))|[-!#\$%&'\*\+/=\?\^`\{\}\|~\w])*)(?<=[0-9a-z])@))" +
                      @"(?(\[)(\[(\d{1,3}\.){3}\d{1,3}\])|(([0-9a-z][-\w]*[0-9a-z]*\.)+[a-z0-9]{2,24}))$",
                      RegexOptions.IgnoreCase, TimeSpan.FromMilliseconds(250));
            }
            catch (RegexMatchTimeoutException)
            {
                return false;
            }
        }

        private string DomainMapper(Match match)
        {
            // IdnMapping class with default property values.
            IdnMapping idn = new IdnMapping();

            string domainName = match.Groups[2].Value;
            try
            {
                domainName = idn.GetAscii(domainName);
            }
            catch (ArgumentException)
            {
                invalid = true;
            }
            return match.Groups[1].Value + domainName;
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