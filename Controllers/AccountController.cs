using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.DataAcess;
using System.Text.RegularExpressions;
using System.IO;
using System.Windows.Media.Imaging;
using System.Web.Security;
using crypto;
using System.Net.Mail;
using Google.GData.Client;
using Google.GData.Extensions;
using Google.Contacts;
using System.Globalization;

namespace RetailKing.Controllers
{
    public class AccountController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        public long subcategoryid;
        bool invalid = false;
        private void ResizeImage(string inputPath, string outputPath, int width, int height)
        {
            BitmapImage bitmap = new BitmapImage();

            using (var stream = new FileStream(inputPath, FileMode.Open))
            {
                bitmap.BeginInit();
                bitmap.DecodePixelWidth = width;
                bitmap.DecodePixelHeight = height;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
        //
        // GET: /Accounts/
        [HttpGet]
        public ActionResult Login(string error)
        {
           if (!Request.IsAuthenticated)
            {
               if(!string.IsNullOrEmpty(error))
               {
                   ModelState.AddModelError("", error);
               }
                LoginModel px = new LoginModel();
                if (Request.IsAjaxRequest()) return PartialView(px);
                return View("LoginFull", px);
            }
           else
            {
               var current = User.Identity.Name;
               char[] delimiter = new char[] { '~' };
               string[] part = current.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

               return Json("Hello " + part[0],JsonRequestBehavior.AllowGet);
            }
        }
        [HttpPost]
        public ActionResult Login(LoginModel model, string returnUrl)
        {

            if (ModelState.IsValid)
            {
                try
                {
                    var pass = Authenticate(model.User, model.Password);
                    char[] delimiterr = new char[] { '~' };
                    string[] partz = pass.User.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

                    switch (pass.redirect.Trim())
                    {
                        case "User":
                                    var authTicket = new FormsAuthenticationTicket(
                                        1,
                                        pass.User,  //user id
                                        DateTime.Now,
                                        DateTime.Now.AddMinutes(120),  // expiry
                                        model.Rememberme,  //true to remember
                                        "RegisteredUser", //roles 
                                        "/"
                                        );
                                
                                //encrypt the ticket and add it to a cookie
                                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName,   FormsAuthentication.Encrypt(authTicket));
                                Response.Cookies.Add(cookie);
                                Session.Add("UserId", partz[0]);

                                return RedirectToAction("Index", "Home");
                        case "Password":
                            ModelState.AddModelError("", "Invalid Username or Password");
                            break;
                        case "Error":
                            ModelState.AddModelError("", "Invalid Username or Password");
                            break;

                    }

                    if (Request.IsAjaxRequest() == true) return Json("Invalid Username or Password", JsonRequestBehavior.AllowGet);
                    return View("LoginFull", model);
                    // return PartialView(model);

                }
                catch
                {
                    ModelState.AddModelError("", "Invalid Username or Password");
                    return View("LoginFull", model);
                }
            }
            return View("LoginFull", model);


        }

        [HttpGet]
        public ActionResult PopLogin()
        {
            LoginModel px = new LoginModel();
            if (Request.IsAjaxRequest()) return PartialView(px);
            return View(px);
        }

        [HttpPost]
        public ActionResult PopLogin(LoginModel model, string returnUrl)
        {
            
                if (ModelState.IsValid)
                {
                    try
                    {
                        var pass = Authenticate(model.User, model.Password);

                        switch (pass.redirect.Trim())
                        {
                            case "User":
                                var authTicket = new FormsAuthenticationTicket(
                                           1,
                                           pass.User,  //user id
                                           DateTime.Now,
                                           DateTime.Now.AddMinutes(120),  // expiry
                                           model.Rememberme,  //true to remember
                                           "", //roles 
                                           "/"
                                           );

                                //encrypt the ticket and add it to a cookie
                                HttpCookie cookie = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                                Response.Cookies.Add(cookie);
                                return Json("Success", JsonRequestBehavior.AllowGet);//RedirectToAction("AddressList", "Cart");
                            case "Password":
                                ModelState.AddModelError("", "Invalid Username or Password");
                                break;
                            case "Error":
                                ModelState.AddModelError("", "Invalid Username or Password");
                                break;

                        }

                        if (Request.IsAjaxRequest() == true) return PartialView(model); //return Json("Invalid Username or Password", JsonRequestBehavior.AllowGet);
                        return View("PopLogin", model);
                        // return PartialView(model);

                    }
                    catch
                    {
                        ModelState.AddModelError("", "Invalid Username or Password");
                        if (Request.IsAjaxRequest() == true) return PartialView(model);
                        return View("PopLogin", model);
                    }
                
            }
            else 
            {

            }
            if (Request.IsAjaxRequest() == true) return  PartialView(model);
            return View("PopLogin", model);


        }

        [HttpGet]
        public ActionResult AccLogout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
        }

        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Index", "Home");
           // return RedirectToAction("Login", "Accounts");  
        }

        private LoginModel Authenticate(string Account, string Pin)
        {
            try
            {
                //int cnt;
                NHibernateDataProvider np = new NHibernateDataProvider();
                ST_encrypt en = new ST_encrypt();
                ST_decrypt dc = new ST_decrypt();
                LoginModel px = new LoginModel();
                //var oo = dc.st_decrypt("02ad569f9e66527a97", "214ea9d5bda5277f6d1b3a3c58fd7034");
                string newAccount = en.encrypt(Account, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Pin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Customer acc = new Customer();
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
                    return px;
                }
                if (acc.Password.Trim() == Pin.Trim())
                {
                   
                    px.User  = acc.CustomerName.Trim() + "~"+ acc.ID;
                   // px. = acc.Password.Trim();
                    px.redirect = "User";

                    var mm = DateTime.Now.Month;
                    if (acc.Period != mm)// change of month we start afresh
                    {
                        if (acc.wallet2 == null) acc.wallet2 = 0;
                        if (acc.wallet3 == null) acc.wallet3 = 0;
                        acc.Period = mm;
                        acc.Purchases = 0;
                        acc.wallet2 = acc.wallet2 + acc.Wallet;
                        acc.Wallet = 0;
                        acc.NetworkType = 0;
                        db.Entry(acc).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                }
                else
                {
                    px.redirect = "Password";
                }
                return px;
            }
            catch (Exception e)
            {
                LoginModel  px = new LoginModel();
                px.redirect = "Error";
                return px;
            }
        }

        [HttpGet]
        public ActionResult Forgot(string ic)
        {
            ViewData["Success"] = "start";
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       // [CaptchaValidation("CaptchaCode", "SampleCaptcha", "Incorrect CAPTCHA code!")]
        public ActionResult Forgot(ChangePasswordModel pw, bool captchaValid)
        {
            if (captchaValid)
            {
                ST_encrypt en = new ST_encrypt();
                ST_decrypt dec = new ST_decrypt();
                var usa = en.encrypt(pw.OldPassword, "214ea9d5bda5277f6d1b3a3c58fd7034");
                var CurrentUser = db.Customers.Where(u => u.Email == usa).FirstOrDefault();
                if (CurrentUser != null)
                {

                    #region Sendemail
                    // Initialize WebMail helper
                    var eml = dec.st_decrypt(CurrentUser.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");

                    MailMessage mailMessage = new MailMessage(
                                          new MailAddress("support@baskiti.com"), new MailAddress(eml));

                    var ee = en.encrypt(eml.ToLower().Trim() + "/" + CurrentUser.ID + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    //  var eml = dec.st_decrypt(CurrentUser.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");

                    mailMessage.Subject = "Password Recovery";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body =
                    "<style>a.buttons {display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;}</style>" +
                    "<div style='width: 100%'><div style='width:50%; margin:0 auto '><div style='height:45px; '>" +
                    "<a href='http://www.baskiti.com'><img style='width:200px' src='http://www.baskiti.com/Content/Template/img/logo.png' alt='baskiti'></a>" +
                    "</div><div style='margin-top:-20px'><div style='margin:10px 15px 10px 10px'><br /><p>Hi,</p><p>" +
                    "Click the Change Password button bellow to go to the password change page.<br/><br/> " +
                    "<a href=http://www.baskiti.com/Account/ForgotPassword?ic=" + ee + " style='" + "display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;" + "' >Change Password</a>" +
                    "</p></div></div></div></div>";


                    System.Net.NetworkCredential networkCredentials = new
                    System.Net.NetworkCredential("support@baskiti.com", "shoppa@2016");

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.EnableSsl = false;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = networkCredentials;
                    smtpClient.Host = "smtpout.secureserver.net";
                    smtpClient.Port = 25;
                    smtpClient.Timeout = 10000;
                    smtpClient.Send(mailMessage);
                    // Send email


                    #endregion
                    ViewData["Success"] = "Success";
                }
                else
                {
                    ViewData["Success"] = "Error";
                }
            }
            else
            {
                ViewData["Success"] = "Enter the characters in the image correctly";
            }
            return View();

        }

        [HttpGet]
        public ActionResult ForgotPassword(string ic)
        {
            ViewData["Success"] = "start";
               ST_decrypt dc = new ST_decrypt();
                var itm = dc.st_decrypt(ic, "214ea9d5bda5277f6d1b3a3c58fd7034");
                char[] delimiter = new char[] { '/' };
                string[] part = itm.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (part.Length == 3)
                {
                    ChangePasswordModel cp = new ChangePasswordModel();
                    cp.OldPassword = ic;
                    return View(cp);
                }
                else
                {
                    return RedirectToAction("Index", "Home");
                }
           
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ForgotPassword(ChangePasswordModel pw)
        {

            if (ModelState.IsValid)
            {
                ST_decrypt dc = new ST_decrypt();
                ST_encrypt en = new ST_encrypt();
             
                pw.OldPassword = dc.st_decrypt(pw.OldPassword, "214ea9d5bda5277f6d1b3a3c58fd7034");
                char[] delimiter = new char[] { '/' };
                string[] part = pw.OldPassword.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var cust = db.Customers.Find(long.Parse(part[1]));
                if (cust != null)
                {
                    cust.Password = en.encrypt(pw.NewPassword, "214ea9d5bda5277f6d1b3a3c58fd7034");

                    db.Entry(cust).State = EntityState.Modified;
                    db.SaveChanges();
                    ViewData["Success"] = "Success";
                    #region Sendemail
                    // Initialize WebMail helper
                    var eml = dc.st_decrypt(cust.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");

                    MailMessage mailMessage = new MailMessage(
                                          new MailAddress("support@baskiti.com"), new MailAddress(eml));

                    var ee = en.encrypt(eml.ToLower().Trim() + "/" + cust.ID + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    //  var eml = dec.st_decrypt(CurrentUser.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");

                    mailMessage.Subject = "Password Recovery";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body =
                    "<style>a.buttons {display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;}</style>" +
                    "<div style='width: 100%'><div style='width:50%; margin:0 auto '><div style='height:45px; '>" +
                    "<a href='http://www.baskiti.com'><img style='width:200px' src='http://www.baskiti.com/Content/Template/img/logo.png' alt='Mobistore'></a>" +
                    "</div><div style='margin-top:-20px'><div style='margin:10px 15px 10px 10px'><br /><p>Hi " + cust.CustomerName + ",</p><p>" +
                    
                    "Your baskiti password has been successfully changed"+
                   
                    "</p></div></div></div></div>";


                    System.Net.NetworkCredential networkCredentials = new
                    System.Net.NetworkCredential("support@baskiti.com", "shoppa@2016");

                    SmtpClient smtpClient = new SmtpClient();
                    smtpClient.EnableSsl = false;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = networkCredentials;
                    smtpClient.Host = "smtpout.secureserver.net";
                    smtpClient.Port = 25;
                    smtpClient.Timeout = 10000;
                    smtpClient.Send(mailMessage);
                    // Send email


                    #endregion
                    return View(pw);
                }
                else
                {
                    ViewData["Success"] = "Password";
                    return View(pw);
                }
            }
            else
            {
                ViewData["Success"] = "Error";
                return View(pw);
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult ChangePassword(string ic)
        {
            if (Request.IsAuthenticated || ic != null)
            {

                ViewData["Success"] = "";
                return PartialView();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize ]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel pw)
        {
         
                if (ModelState.IsValid)
                {
                    ST_encrypt en = new ST_encrypt();
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    var cust = db.Customers.Find(long.Parse(part[1]));
                     pw.OldPassword  = en.encrypt(pw.OldPassword, "214ea9d5bda5277f6d1b3a3c58fd7034");
                     if (cust.Password.Trim() == pw.OldPassword)
                     {
                         cust.Password = en.encrypt(pw.NewPassword, "214ea9d5bda5277f6d1b3a3c58fd7034");

                         db.Entry(cust).State = EntityState.Modified;
                         db.SaveChanges();
                         ViewData["Success"] = "Success";
                         return PartialView(pw);
                     }
                     else
                     {
                         ViewData["Success"] = "Password";
                         return PartialView(pw);
                     }
                }
                else
                {
                    ViewData["Success"] = "Error";
                    return PartialView(pw);
                }
         }
         
        [Authorize]
        [HttpGet]
        public ActionResult CustomerPurchases(string ic)
        {
            if (Request.IsAuthenticated || ic != null)
            {
                 var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    var cust = long.Parse(part[1]);
                    var pp = db.Payments.Where(u => u.CustomerId == cust && u.Status.Trim() == "Paid").ToList();
                    pp.OrderBy(u => u.DateCreated);
                    ViewData["Success"] = "";
                    return PartialView(pp);
            }
            else
            {
                return RedirectToAction("Account", "LoginFull");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult IncomeCalculator(string ic)
        {

            if (Request.IsAuthenticated || ic != null)
            {
                bool isMobile = false;
                string um = Request.ServerVariables["HTTP_USER_AGENT"];
                Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if ((b.IsMatch(um) || v.IsMatch(um.Substring(0, 4)) || Request.Browser.IsMobileDevice))
                {
                    isMobile = true;
                }

                var ana = new Analytics();
                ana.logViews("_Calculator", isMobile);
                return PartialView();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult CreateAcount(string ic)
        {
            if (Request.IsAuthenticated || ic != null)
            {
                List<SelectListItem> yr = new List<SelectListItem>();
                // List<SelectListItem> yrd = new List<SelectListItem>();
                yr.Add(new SelectListItem
                {
                    Text = "Year",
                    Value = ""
                });

                for (int i = DateTime.Now.Year; i >= 1935; i--)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }

                ViewBag.Usertype = yr;
                var nets = db.NetworkTypes.ToList();
                List<SelectListItem> cc = new List<SelectListItem>();

                foreach (var item in nets)
                {
                    cc.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewData["Success"] = "";
                ViewBag.Level = cc;
                ViewData["NetwokFee"] = nets.FirstOrDefault().Fee.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                var cities = db.Cities.ToList();
               
                List<SelectListItem> ct = new List<SelectListItem>();
                ct.Add(new SelectListItem
                {
                    Text = "-Select City-",
                    Value = "-Select City-"
                });
                foreach (var item in cities)
                {
                    ct.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Cities = ct;
                var cid = cities.FirstOrDefault().Id;
                var surb = db.Suburbs.Where(u => u.CityId == cid).ToList();
                var area = db.Regions.Where(u => u.CityId == cid).ToList();
                List<SelectListItem> sb = new List<SelectListItem>();
                sb.Add(new SelectListItem
                {
                    Text = "-Select Suburb-",
                    Value = "-Select Suburb-"
                });
                foreach (var item in surb)
                {
                    sb.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Suburb = sb;

                List<SelectListItem> rg = new List<SelectListItem>();
                rg.Add(new SelectListItem
                {
                    Text = "-Select Region-",
                    Value = "-Select Region-"
                });
                foreach (var item in area)
                {
                    rg.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Area = rg;

                var customers = new Customer();
                var pz = db.Customers.ToList();
                var pp = (from e in pz
                          .OrderBy(u => u.ID)
                          select e).LastOrDefault();

                if (pp == null)
                {
                    pp = new Customer();
                    customers.AccountCode = "8-01-00001";
                }
                else
                {
                    char[] delimiter = new char[] { '-' };
                    string[] part = pp.AccountCode.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    var Id = long.Parse(part[2]);
                    string acc = "";
                    var x = Id + 1;
                    if (x < 10)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "0000" + x.ToString();
                    }
                    else if (x > 9 && x < 100)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "000" + x.ToString();
                    }
                    else if (x > 99 && x < 1000)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "00" + x.ToString();
                    }
                    else if (x > 999 && x < 10000)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "0" + x.ToString();
                    }
                    else if (x > 9999 && x < 100000)
                    {
                        acc = part[0] + "-" + part[1] + "-" + x.ToString();
                    }
                    else if (x > 9999)
                    {
                        var Idd = long.Parse(part[1]);
                        var xx = Idd + 1;
                        if (xx < 10)
                        {
                            acc = part[0] + "-" + "0" + xx.ToString() + "-" + "00001";
                        }
                        else if (xx > 9 && xx < 99)
                        {
                            acc = part[0] + "-" + xx.ToString() + "-" + "00001";
                        }
                        else
                        {
                            acc = "Exhausted";
                        }
                    }
                    customers.AccountCode = acc;
                    customers.CompanyId = "1";
                }
                if (!string.IsNullOrEmpty(ic))
                {
                    try
                    {

                        ST_decrypt dc = new ST_decrypt();
                        var itm = dc.st_decrypt(ic, "214ea9d5bda5277f6d1b3a3c58fd7034");
                        char[] delimiter = new char[] { '/' };
                        string[] part = ic.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        if (part.Length == 3)
                        {
                            var inv = db.Invites.Find(long.Parse(part[1]));
                            var diff = DateTime.Now.Subtract(inv.Dated.Value);
                            if (inv != null && inv.Status.Trim() == "O")
                            {
                                customers.ParentId = inv.ParentId;
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }
                    catch( Exception)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                
                var slides = db.Items.Where(u => u.Promotion.Trim() == "YES" & u.AdPosition.StartsWith("Slide") & u.SDate <= DateTime.Now & u.EDate >= DateTime.Now).Take(4).ToList();
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                customers.categories = categories;
                return View(customers);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult CreateAcount(Customer customer)
        {
                #region params
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                List<SelectListItem> yr = new List<SelectListItem>();
                // List<SelectListItem> yrd = new List<SelectListItem>();
                yr.Add(new SelectListItem
                {
                    Text = "Year",
                    Value = ""
                });

                for (int i = DateTime.Now.Year; i >= 1935; i--)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }

                ViewBag.Usertype = yr;
                var nets = db.NetworkTypes.ToList();
                List<SelectListItem> cc = new List<SelectListItem>();

                foreach (var item in nets)
                {
                    cc.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewData["Success"] = "";
                ViewBag.Level = cc;
                ViewData["NetwokFee"] = nets.FirstOrDefault().Fee.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                var cities = db.Cities.ToList();
                List<SelectListItem> ct = new List<SelectListItem>();
                foreach (var item in cities)
                {
                    ct.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Cities = ct;
                var cid = cities.FirstOrDefault().Id;
                var surb = db.Suburbs.Where(u => u.CityId == cid).ToList();
                var area = db.Regions.Where(u => u.CityId == cid).ToList();
                List<SelectListItem> sb = new List<SelectListItem>();

                foreach (var item in surb)
                {
                    sb.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Suburb = sb;

                List<SelectListItem> rg = new List<SelectListItem>();
                rg.Add(new SelectListItem
                {
                    Text = "-Select Region-",
                    Value = "-Select Region-"
                });
                foreach (var item in area)
                {
                    rg.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Area = rg;

                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                customer.categories = categories;
#endregion

                if (ModelState.IsValid)
                {
                    var sub = new Suburb();
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    if(customer.ParentId != null)
                    {
                        part[1] = customer.ParentId.ToString();
                    }
                    if (customer.CustomerName  == null || customer.CustomerName == "")
                    {
                        ModelState.AddModelError("", "Customer Name is required");
                        return View(customer);
                    }
                    if (customer.Email  == null || customer.Email  == "")
                    {
                        ModelState.AddModelError("", "the email address is required");
                        return View(customer);
                    }
                    if( customer.City == "-Select City-")
                    {
                        ModelState.AddModelError("", "Please select City");
                        return View(customer);
                    }
                    if (customer.Suburb == "")
                    {
                        ModelState.AddModelError("", "Please select Suburb");
                        return View(customer);
                    }
                    else
                    {
                        sub = db.Suburbs.Where(u => u.Name == customer.Suburb).FirstOrDefault();
                        if (sub == null)
                        {
                            ModelState.AddModelError("", "Please enter a valid suburb");
                            return View(customer);
                        }
                    }
                    if(customer.Address1 == null || customer.Address1 == "")
                    {
                        ModelState.AddModelError("", "Please select Address");
                        return View(customer);
                    }
                    var age = DateTime.Now.Year - customer.year;
                    if(age < 18 )
                    {
                        ModelState.AddModelError("", "Sorry this site is for over 18yr olds");
                        return View(customer); 
                    }
                    ST_encrypt en = new ST_encrypt();

                    string em = customer.Email;
                    string newAccount = en.encrypt(customer.Email.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    var pass = System.Web.Security.Membership.GeneratePassword(9, 1);
                    string Pin = en.encrypt(pass, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    
                    var accs = db.Customers.Where(u => u.Email.Trim() == newAccount).ToList();
                    string dat = "";
                    string mon = "";
                    if (accs == null || accs.Count == 0)
                    {
                        if (customer.Date < 10)
                        {
                            dat = "0" + customer.Date;
                        }
                        else
                        {
                            dat = customer.Date.ToString();
                        }
                        if (customer.month < 10)
                        {
                            mon = "0" + customer.month;
                        }
                        else
                        {
                            mon = customer.month.ToString();
                        }

                        customer.Dated = dat + "/" + mon + "/" + customer.year.ToString();
                      
                        customer.Suburb = sub.Id.ToString();
                        customer.City = sub.CityId.ToString();
                        customer.RegionId = sub.RegionId;
                        customer.Email = newAccount;
                        customer.Password = Pin;
                        customer.CustomerName = customer.CustomerName.ToUpper();
                        customer.Username = customer.CustomerName.ToUpper();
                        customer.ParentId = long.Parse(part[1]);
                        customer.Balance = 0;
                        customer.Wallet = 0;
                        customer.Purchases = 0;
                        customer.PurchasesToDate = 0;
                        customer.Password = Pin;
                        customer.CompanyId = "1";
                        customer.NetworkType = 0;
                        customer.Level1Count = 0;
                        customer.Level2Count = 0;
                        customer.Level3Count = 0;
                        customer.wallet2 = 0;
                        customer.wallet3 = 0;
                        
                        //customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                        db.Customers.Add(customer);
                        db.SaveChanges();
                       // var level = db.NetworkTypes.Find(1);
                        
                        #region commissions
                        // this section the system will look through all this customers network for commissions
                       /*var parent0 = db.Customers.Find(long.Parse(part[1]));
                        if (parent0 != null)
                        {
                            #region Parent0
                            var totalsales = db.NetworkSales.Find(part[1] + "_" + DateTime.Now.Year);// total salesfor current user
                            bool update = true;
                            if (totalsales == null)
                            {
                                totalsales = new NetworkSale();
                                update = false;
                            }
                            #region totals
                            switch (DateTime.Now.Month)
                            {
                                case 1:
                                    if (totalsales.JanJ0 == null)
                                    {
                                        totalsales.JanJ0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.JanJ0 = totalsales.JanJ0 + level.Fee;
                                    }
                                    break;
                                case 2:
                                    if (totalsales.FebJ0 == null)
                                    {
                                        totalsales.FebJ0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.FebJ0 = totalsales.FebJ0 + level.Fee;
                                    }
                                    break;
                                case 3:
                                    if (totalsales.MarJ0 == null)
                                    {
                                        totalsales.MarJ0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.MarJ0 = totalsales.MarJ0 + level.Fee;
                                    }
                                    break;
                                case 4:
                                    if (totalsales.AprJ0 == null)
                                    {
                                        totalsales.Apr0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.AprJ0 = totalsales.AprJ0 + level.Fee;
                                    }
                                    break;
                                case 5:
                                    if (totalsales.MayJ0 == null)
                                    {
                                        totalsales.MayJ0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.MayJ0 = totalsales.MayJ0 + level.Fee;
                                    }
                                    break;
                                case 6:
                                    if (totalsales.JunJ0 == null)
                                    {
                                        totalsales.JunJ0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.JunJ0 = totalsales.JunJ0 + level.Fee;
                                    }
                                    break;
                                case 7:
                                    if (totalsales.JulJ0 == null)
                                    {
                                        totalsales.JulJ0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.Jul0 = totalsales.JulJ0 + level.Fee;
                                    }
                                    break;
                                case 8:
                                    if (totalsales.AugJ0 == null)
                                    {
                                        totalsales.AugJ0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.AugJ0 = totalsales.AugJ0 + level.Fee;
                                    }
                                    break;
                                case 9:
                                    if (totalsales.SepJ0 == null)
                                    {
                                        totalsales.SepJ0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.SepJ0 = totalsales.SepJ0 + level.Fee;
                                    }
                                    break;
                                case 10:
                                    if (totalsales.OctJ0 == null)
                                    {
                                        totalsales.OctJ0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.OctJ0 = totalsales.OctJ0 + level.Fee;
                                    }
                                    break;
                                case 11:
                                    if (totalsales.NovJ0 == null)
                                    {
                                        totalsales.NovJ0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.NovJ0 = totalsales.NovJ0 + level.Fee;
                                    }
                                    break;
                                case 12:
                                    if (totalsales.DecJ0 == null)
                                    {
                                        totalsales.DecJ0 = level.Fee;
                                    }
                                    else
                                    {
                                        totalsales.DecJ0 = totalsales.DecJ0 + level.Fee;
                                    }
                                    break;
                            }
                            #endregion

                            if (update == false)
                            {
                                totalsales.Id = customer.ParentId + "_" + DateTime.Now.Year;
                                totalsales.Year = DateTime.Now.Year;
                                db.NetworkSales.Add(totalsales);
                                db.SaveChanges();
                            }
                            else
                            {
                                db.Entry(totalsales).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            parent0.Wallet = parent0.Wallet + (level.Fee) * (decimal)0.15;
                            if (parent0.Level1Count == null) parent0.Level1Count = 0;
                            parent0.Level1Count  = parent0.Level1Count + 1;
                            db.Entry(parent0).State = EntityState.Modified;
                            db.SaveChanges();
                            #endregion
                            var parent1 = db.Customers.Find(parent0.ParentId);
                            if (parent1 != null)
                            {
                                #region Parent1
                                var totalsales1 = db.NetworkSales.Find(parent0.ParentId + "_" + DateTime.Now.Year);// total salesfor current user
                                update = true;
                                if (totalsales1 == null)
                                {
                                    totalsales1 = new NetworkSale();
                                    update = false;
                                }

                                #region totals1
                                switch (DateTime.Now.Month)
                                {
                                    case 1:
                                        if (totalsales1.JanJ1 == null)
                                        {
                                            totalsales1.JanJ1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales1.JanJ1 = totalsales1.JanJ1 + level.Fee;
                                        }
                                        break;
                                    case 2:
                                        if (totalsales1.FebJ1 == null)
                                        {
                                            totalsales1.FebJ1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales1.FebJ1 = totalsales1.FebJ1 + level.Fee;
                                        }
                                        break;
                                    case 3:
                                        if (totalsales1.MarJ1 == null)
                                        {
                                            totalsales1.MarJ1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales1.MarJ1 = totalsales1.MarJ0 + level.Fee;
                                        }
                                        break;
                                    case 4:
                                        if (totalsales1.AprJ1 == null)
                                        {
                                            totalsales1.Apr1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales1.AprJ1 = totalsales.AprJ1 + level.Fee;
                                        }
                                        break;
                                    case 5:
                                        if (totalsales1.MayJ1 == null)
                                        {
                                            totalsales1.MayJ1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales1.MayJ1 = totalsales.MayJ1 + level.Fee;
                                        }
                                        break;
                                    case 6:
                                        if (totalsales1.JunJ1 == null)
                                        {
                                            totalsales1.JunJ1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales1.JunJ1 = totalsales.JunJ1 + level.Fee;
                                        }
                                        break;
                                    case 7:
                                        if (totalsales1.JulJ1 == null)
                                        {
                                            totalsales1.JulJ1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales1.Jul1 = totalsales1.JulJ1 + level.Fee;
                                        }
                                        break;
                                    case 8:
                                        if (totalsales1.AugJ1 == null)
                                        {
                                            totalsales1.AugJ1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales1.AugJ1 = totalsales.AugJ1 + level.Fee;
                                        }
                                        break;
                                    case 9:
                                        if (totalsales1.SepJ1 == null)
                                        {
                                            totalsales1.SepJ1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales1.SepJ1 = totalsales.SepJ1 + level.Fee;
                                        }
                                        break;
                                    case 10:
                                        if (totalsales1.OctJ1 == null)
                                        {
                                            totalsales1.OctJ1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales1.OctJ1 = totalsales.OctJ1 + level.Fee;
                                        }
                                        break;
                                    case 11:
                                        if (totalsales1.NovJ1 == null)
                                        {
                                            totalsales1.NovJ1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales1.NovJ1 = totalsales1.NovJ1 + level.Fee;
                                        }
                                        break;
                                    case 12:
                                        if (totalsales1.DecJ1 == null)
                                        {
                                            totalsales1.DecJ1 = level.Fee;
                                        }
                                        else
                                        {
                                            totalsales.DecJ1 = totalsales.DecJ1 + level.Fee;
                                        }
                                        break;
                                }
                                #endregion

                                if (update == false)
                                {
                                    totalsales1.Id = parent1.ParentId + "_" + DateTime.Now.Year;
                                    totalsales1.Year = DateTime.Now.Year;
                                    db.NetworkSales.Add(totalsales1);
                                    db.SaveChanges();
                                }
                                else
                                {
                                    db.Entry(totalsales1).State = EntityState.Modified;
                                    db.SaveChanges();
                                }
                                parent1.Wallet = parent1.Wallet + (level.Fee) * (decimal)0.12;
                                if (parent1.Level2Count == null) parent1.Level2Count = 0;
                                parent1.Level2Count = parent1.Level2Count + 1;
                                db.Entry(parent1).State = EntityState.Modified;
                                db.SaveChanges();
                                #endregion
                                var parent2 = db.Customers.Find(parent1.ParentId);
                                if (parent2 != null)
                                {
                                    #region Parent2

                                    var totalsales2 = db.NetworkSales.Find(parent2.ParentId + "_" + DateTime.Now.Year);// total salesfor current iser
                                    update = true;
                                    if (totalsales2 == null)
                                    {
                                        totalsales2 = new NetworkSale();
                                        update = false;
                                    }

                                    #region totals2
                                    switch (DateTime.Now.Month)
                                    {
                                        case 1:
                                            if (totalsales2.JanJ2 == null)
                                            {
                                                totalsales2.JanJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.JanJ2 = totalsales2.JanJ2 + level.Fee;
                                            }
                                            break;
                                        case 2:
                                            if (totalsales2.FebJ2 == null)
                                            {
                                                totalsales2.FebJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.FebJ2 = totalsales2.FebJ2 + level.Fee;
                                            }
                                            break;
                                        case 3:
                                            if (totalsales2.MarJ2 == null)
                                            {
                                                totalsales2.MarJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.MarJ2 = totalsales2.MarJ2 + level.Fee;
                                            }
                                            break;
                                        case 4:
                                            if (totalsales2.AprJ2 == null)
                                            {
                                                totalsales2.AprJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.AprJ2 = totalsales2.AprJ2 + level.Fee;
                                            }
                                            break;
                                        case 5:
                                            if (totalsales2.MayJ2 == null)
                                            {
                                                totalsales2.MayJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.MayJ2 = totalsales2.MayJ2 + level.Fee;
                                            }
                                            break;
                                        case 6:
                                            if (totalsales2.JunJ2 == null)
                                            {
                                                totalsales2.JunJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.JunJ2 = totalsales2.JunJ2 + level.Fee;
                                            }
                                            break;
                                        case 7:
                                            if (totalsales2.JulJ2 == null)
                                            {
                                                totalsales2.JulJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.JulJ2 = totalsales2.JulJ2 + level.Fee;
                                            }
                                            break;
                                        case 8:
                                            if (totalsales2.AugJ2 == null)
                                            {
                                                totalsales2.AugJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.AugJ2 = totalsales2.AugJ2 + level.Fee;
                                            }
                                            break;
                                        case 9:
                                            if (totalsales2.SepJ2 == null)
                                            {
                                                totalsales2.SepJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.SepJ2 = totalsales2.SepJ2 + level.Fee;
                                            }
                                            break;
                                        case 10:
                                            if (totalsales2.OctJ2 == null)
                                            {
                                                totalsales2.OctJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.OctJ2 = totalsales2.OctJ2 + level.Fee;
                                            }
                                            break;
                                        case 11:
                                            if (totalsales2.OctJ2 == null)
                                            {
                                                totalsales2.OctJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.OctJ2 = totalsales2.OctJ2 + level.Fee;
                                            }
                                            break;
                                        case 12:
                                            if (totalsales2.DecJ2 == null)
                                            {
                                                totalsales2.DecJ2 = level.Fee;
                                            }
                                            else
                                            {
                                                totalsales2.DecJ2 = totalsales2.DecJ2 + level.Fee;
                                            }
                                            break;
                                    }

                                    #endregion

                                    if (update == false)
                                    {
                                        totalsales2.Id = parent2.ParentId + "_" + DateTime.Now.Year;
                                        totalsales2.Year = DateTime.Now.Year;
                                        db.NetworkSales.Add(totalsales2);
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        db.Entry(totalsales2).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    if (parent2.NetworkType >= 2)
                                    {
                                        parent2.Wallet = parent2.Wallet + (level.Fee) * (decimal)0.08;
                                    }
                                    if (parent2.Level3Count == null) parent2.Level3Count = 0;
                                    parent2.Level3Count = parent2.Level3Count + 1;
                                    db.Entry(parent1).State = EntityState.Modified;
                                    db.SaveChanges();
                                    #endregion
                                    var parent3 = db.Customers.Find(parent2.ParentId);
                                    if (parent3 != null)
                                    {
                                        #region Parent3

                                        var totalsales3 = db.NetworkSales.Find(parent3.ParentId + "_" + DateTime.Now.Year);// total salesfor current iser
                                        update = true;
                                        if (totalsales3 == null)
                                        {
                                            totalsales3 = new NetworkSale();
                                            update = false;
                                        }

                                        #region totals3
                                        switch (DateTime.Now.Month)
                                        {
                                            case 1:
                                                if (totalsales3.JanJ3 == null)
                                                {
                                                    totalsales3.JanJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.JanJ3 = totalsales3.JanJ3 + level.Fee;
                                                }
                                                break;
                                            case 2:
                                                if (totalsales3.FebJ3 == null)
                                                {
                                                    totalsales3.FebJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.FebJ3 = totalsales3.FebJ3 + level.Fee;
                                                }
                                                break;
                                            case 3:
                                                if (totalsales3.MarJ3 == null)
                                                {
                                                    totalsales3.MarJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.MarJ3 = totalsales3.MarJ3 + level.Fee;
                                                }
                                                break;
                                            case 4:
                                                if (totalsales3.AprJ3 == null)
                                                {
                                                    totalsales3.AprJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.AprJ3 = totalsales3.AprJ3 + level.Fee;
                                                }
                                                break;
                                            case 5:
                                                if (totalsales3.MayJ3 == null)
                                                {
                                                    totalsales3.MayJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.MayJ3 = totalsales3.MayJ3 + level.Fee;
                                                }
                                                break;
                                            case 6:
                                                if (totalsales3.JunJ3 == null)
                                                {
                                                    totalsales3.JunJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.JunJ3 = totalsales3.JunJ3 + level.Fee;
                                                }
                                                break;
                                            case 7:
                                                if (totalsales3.JulJ3 == null)
                                                {
                                                    totalsales3.JulJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.JulJ3 = totalsales3.JulJ3 + level.Fee;
                                                }
                                                break;
                                            case 8:
                                                if (totalsales3.AugJ3 == null)
                                                {
                                                    totalsales3.AugJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.AugJ3 = totalsales3.AugJ3 + level.Fee;
                                                }
                                                break;
                                            case 9:
                                                if (totalsales3.SepJ3 == null)
                                                {
                                                    totalsales3.SepJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.SepJ3 = totalsales3.SepJ3 + level.Fee;
                                                }
                                                break;
                                            case 10:
                                                if (totalsales3.OctJ3 == null)
                                                {
                                                    totalsales3.OctJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.OctJ3 = totalsales3.OctJ3 + level.Fee;
                                                }
                                                break;
                                            case 11:
                                                if (totalsales3.OctJ3 == null)
                                                {
                                                    totalsales3.OctJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.OctJ3 = totalsales3.OctJ3 + level.Fee;
                                                }
                                                break;
                                            case 12:
                                                if (totalsales3.DecJ3 == null)
                                                {
                                                    totalsales3.DecJ3 = level.Fee;
                                                }
                                                else
                                                {
                                                    totalsales3.DecJ3 = totalsales3.DecJ3 + level.Fee;
                                                }
                                                break;
                                        }
                        
                                        #endregion

                                        if (update == false)
                                        {
                                            totalsales3.Id = parent3.ParentId + "_" + DateTime.Now.Year;
                                            totalsales3.Year = DateTime.Now.Year;
                                            db.NetworkSales.Add(totalsales3);
                                            db.SaveChanges();
                                        }
                                        else
                                        {
                                            db.Entry(totalsales3).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        if (parent3.NetworkType == 3)
                                        {
                                            parent3.Wallet = parent3.Wallet + (level.Fee) * (decimal)0.05;
                                            db.Entry(parent3).State = EntityState.Modified;
                                            db.SaveChanges();
                                        }
                                        #endregion
                                    }

                                }
                            }
                        }
                     */
                        #endregion

                        #region Sendemail
                        // Initialize WebMail helper
                        var par = db.Customers.Find(long.Parse(part[1]));
                        MailMessage mailMessage = new MailMessage(
                                              new MailAddress("support@baskiti.com"), new MailAddress(em));

                        var ee = en.encrypt(customer.Email.ToLower().Trim() + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");

                        mailMessage.Subject = "Account Registration";
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = "<style>a.buttons {display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;}.capitalize {text-transform: capitalize;}</style>" +
                        "<div style='width: 100%'><div style='width:70%; margin:0 auto;'><div style='width:100%'><div style='height:45px;'>" +
                        "<a href='http://www.baskiti.com'><img style='width:200px' src='http://www.baskiti.com/Content/Template/img/logo.png' alt='Mobistore'></a>" +
                          "<p class='" + "capitalize" + "'> Hi " + customer.Username + ",</p><p><span class='" + "capitalize" + "'>  " + par.ContactPerson + "</span> is a member of the baskiti Network and  has just created an account for you, this means you can now explore an exiting business opportunity on the first-ever online store that turns your grocery expense into income,"
                        + "There are <span style='color:red'>no joining fees</span>, you just have to buy your essentials online, and you will be on your way to earning money. Start building your Network by simply inviting others to join. You can invite them two ways,<br/> 1) Get their information and create an account for them on the “Create Account” menu which is at the top left bar in your logged-in account  <br/> 2) Invite them by using the “Send Invite” menu where they will register themselves. " +
                         "<p>Use the following credentials to Signin.<br/><br/> Email Address = " + em + "<br/> Password = " + pass + "</p><br/>" +
                         "<p>To get started right away " +
                         "<a href=http://www.baskiti.com style='" + "display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;" + "' >Click This </a>" +
                         "to visit mobistore site.<br/> Or you can copy the link below and paste it in your browser address bar. </p>" +
                         "<p> http://www.baskiti.com </p><br/>" +
                         "<p>you can delete this email once you have signed in to Mobistore to protect your password</p>" +
                          "</div></div></div></div>";


                        System.Net.NetworkCredential networkCredentials = new
                         System.Net.NetworkCredential("support@baskiti.com", "shoppa@2016");

                        SmtpClient smtpClient = new SmtpClient();
                        smtpClient.EnableSsl = false;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = networkCredentials;
                        smtpClient.Host = "smtpout.secureserver.net";
                        smtpClient.Port = 25;
                        smtpClient.Timeout = 30000;
                        smtpClient.Send(mailMessage);
                        // Send email
                       

                        #endregion 

                        var parent0 = db.Customers.Find(long.Parse(part[1]));
                        if (parent0 != null)
                        {
                            if (parent0.Level1Count == null) parent0.Level1Count = 0;
                            parent0.Level1Count = parent0.Level1Count + 1;
                            db.Entry(parent0).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        var parent1 = db.Customers.Find(parent0.ParentId);
                        if (parent1 != null)
                        {
                            if (parent1.Level2Count == null) parent1.Level2Count = 0;
                            parent1.Level2Count = parent1.Level2Count + 1;
                            db.Entry(parent1).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        var parent2 = db.Customers.Find(parent1.ParentId);
                        if (parent2 != null)
                        {

                            if (parent2.Level3Count == null) parent2.Level3Count = 0;
                            parent2.Level3Count = parent2.Level3Count + 1;
                            db.Entry(parent2).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        ViewData["Success"] = "Success";
                        return View(customer);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sorry this customer name is already in use");
                        return View(customer);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Make sure all fields are field in");
                    return View(customer);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
}

        [HttpGet]
        public ActionResult Register(string ic, string code)
        {

            var customers = new Customer();
            if (!Request.IsAuthenticated && !string.IsNullOrEmpty(ic))
            {
                List<SelectListItem> yr = new List<SelectListItem>();
                // List<SelectListItem> yrd = new List<SelectListItem>();
                yr.Add(new SelectListItem
                {
                    Text = "Year",
                    Value = ""
                });

                for (int i = DateTime.Now.Year; i >= 1935; i--)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }

                ViewBag.Usertype = yr;
                var nets = db.NetworkTypes.ToList();
                List<SelectListItem> cc = new List<SelectListItem>();

                foreach (var item in nets)
                {
                    cc.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewData["Success"] = "";
                ViewBag.Level = cc;
                ViewData["NetwokFee"] = nets.FirstOrDefault().Fee.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                var cities = db.Cities.ToList();

                List<SelectListItem> ct = new List<SelectListItem>();
                ct.Add(new SelectListItem
                {
                    Text = "-Select City-",
                    Value = "-Select City-"
                });
                foreach (var item in cities)
                {
                    ct.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Cities = ct;
                var cid = cities.FirstOrDefault().Id;
                var surb = db.Suburbs.Where(u => u.CityId == cid).ToList();
                var area = db.Regions.Where(u => u.CityId == cid).ToList();
                List<SelectListItem> sb = new List<SelectListItem>();
                sb.Add(new SelectListItem
                {
                    Text = "-Select Suburb-",
                    Value = "-Select Suburb-"
                });
                foreach (var item in surb)
                {
                    sb.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Suburb = sb;

                List<SelectListItem> rg = new List<SelectListItem>();
                rg.Add(new SelectListItem
                {
                    Text = "-Select Region-",
                    Value = "-Select Region-"
                });
                foreach (var item in area)
                {
                    rg.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Area = rg;

                customers = new Customer();
                var pz = db.Customers.ToList();
                var pp = (from e in pz
                          .OrderBy(u => u.ID)
                          select e).LastOrDefault();

                if (pp == null)
                {
                    pp = new Customer();
                    customers.AccountCode = "8-01-00001";
                }
                else
                {
                    char[] delimiter = new char[] { '-' };
                    string[] part = pp.AccountCode.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    var Id = long.Parse(part[2]);
                    string acc = "";
                    var x = Id + 1;
                    if (x < 10)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "0000" + x.ToString();
                    }
                    else if (x > 9 && x < 100)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "000" + x.ToString();
                    }
                    else if (x > 99 && x < 1000)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "00" + x.ToString();
                    }
                    else if (x > 999 && x < 10000)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "0" + x.ToString();
                    }
                    else if (x > 9999 && x < 100000)
                    {
                        acc = part[0] + "-" + part[1] + "-" + x.ToString();
                    }
                    else if (x > 9999)
                    {
                        var Idd = long.Parse(part[1]);
                        var xx = Idd + 1;
                        if (xx < 10)
                        {
                            acc = part[0] + "-" + "0" + xx.ToString() + "-" + "00001";
                        }
                        else if (xx > 9 && xx < 99)
                        {
                            acc = part[0] + "-" + xx.ToString() + "-" + "00001";
                        }
                        else
                        {
                            acc = "Exhausted";
                        }
                    }
                    customers.AccountCode = acc;
                    customers.CompanyId = "1";
                }

                if (!string.IsNullOrEmpty(ic)) //invite registrattion
                {
                    try
                    {
                       
                        ST_decrypt dc = new ST_decrypt();
                        var itm = dc.st_decrypt(ic, "214ea9d5bda5277f6d1b3a3c58fd7034");
                        char[] delimiter = new char[] { '/' };
                        string[] part = itm.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        if (part.Length == 3)
                        {
                            var inv = db.Invites.Find(long.Parse(part[1]));
                            var diff = DateTime.Now.Subtract(inv.Dated.Value);
                            if (inv != null && inv.Status.Trim() == "O")
                            {
                                customers.Email = inv.Email;
                                customers.ParentId = inv.ParentId;
                            }
                            else
                            {
                                return RedirectToAction("Index", "Home");
                            }
                        }
                        else
                        {
                            return RedirectToAction("Index", "Home");
                        }

                    }
                    catch (Exception)
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                if (code != null && code != "") customers.code = code;
                var slides = db.Items.Where(u => u.Promotion.Trim() == "YES" & u.AdPosition.StartsWith("Slide") & u.SDate <= DateTime.Now & u.EDate >= DateTime.Now).Take(4).ToList();
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                customers.categories = categories;
                return View(customers);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Register(Customer customer)
        {
            #region params
            NHibernateDataProvider np = new NHibernateDataProvider();
            List<SelectListItem> yr = new List<SelectListItem>();
            // List<SelectListItem> yrd = new List<SelectListItem>();
            yr.Add(new SelectListItem
            {
                Text = "Year",
                Value = ""
            });

            for (int i = DateTime.Now.Year; i >= 1935; i--)
            {
                yr.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });
            }

            ViewBag.Usertype = yr;
                var nets = db.NetworkTypes.ToList();
                List<SelectListItem> cc = new List<SelectListItem>();

                foreach (var item in nets)
                {
                    cc.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewData["Success"] = "";
                ViewBag.Level = cc;
                ViewData["NetwokFee"] = nets.FirstOrDefault().Fee.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                var cities = db.Cities.ToList();
                List<SelectListItem> ct = new List<SelectListItem>();
                foreach (var item in cities)
                {
                    ct.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Cities = ct;
                var cid = cities.FirstOrDefault().Id;
                var surb = db.Suburbs.Where(u => u.CityId == cid).ToList();
                var area = db.Regions.Where(u => u.CityId == cid).ToList();
                List<SelectListItem> sb = new List<SelectListItem>();

                foreach (var item in surb)
                {
                    sb.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Suburb = sb;

                List<SelectListItem> rg = new List<SelectListItem>();
                rg.Add(new SelectListItem
                {
                    Text = "-Select Region-",
                    Value = "-Select Region-"
                });
                foreach (var item in area)
                {
                    rg.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Area = rg;

                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                customer.categories = categories;
            #endregion

                if (ModelState.IsValid)
                {
                    var sub = new Suburb();

                    long? uid = 0;
                    if (customer.ParentId != null)
                    {
                       uid = customer.ParentId;
                    }
                    if (customer.CustomerName == null || customer.CustomerName == "")
                    {
                        ModelState.AddModelError("", "Customer Name is required");
                        return View(customer);
                    }
                    if (customer.Email == null || customer.Email == "")
                    {
                        ModelState.AddModelError("", "the email address is required");
                        return View(customer);
                    }
                    if (customer.City == "-Select City-")
                    {
                        ModelState.AddModelError("", "Please select City");
                        return View(customer);
                    }
                    if (customer.Suburb == "")
                    {
                        ModelState.AddModelError("", "Please select Suburb");
                        return View(customer);
                    }
                    else
                    {
                         sub = db.Suburbs.Where(u => u.Name == customer.Suburb).FirstOrDefault();
                        if(sub == null)
                        {
                            ModelState.AddModelError("", "Please enter a valid suburb");
                            return View(customer);
                        }
                        else
                        {
                            customer.RegionId = sub.RegionId;  
                        }
                    }
                    if (customer.Address1 == null || customer.Address1 == "")
                    {
                        ModelState.AddModelError("", "Please select Address");
                        return View(customer);
                    }
                    ST_encrypt en = new ST_encrypt();

                    string em = customer.Email;
                    string newAccount = en.encrypt(customer.Email.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    var pass = System.Web.Security.Membership.GeneratePassword(9, 1);
                    string Pin = en.encrypt(pass, "214ea9d5bda5277f6d1b3a3c58fd7034");

                    var accs = db.Customers.Where(u => u.Email.Trim() == newAccount).ToList();
                    if (accs == null || accs.Count == 0)
                    {
                        var invt = db.Invites.Where(u => u.Email.Trim() == customer.Email).FirstOrDefault();
                        if (invt != null )
                        {
                            invt.Dated = DateTime.Now;
                            invt.Status = "A";
                            db.Entry(invt).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                         string dat = "";
                    string mon = "";
                  
                        if (customer.Date < 10)
                        {
                            dat = "0" + customer.Date;
                        }
                        else
                        {
                            dat = customer.Date.ToString();
                        }
                        if (customer.month < 10)
                        {
                            mon = "0" + customer.month;
                        }
                        else
                        {
                            mon = customer.month.ToString();
                        }

                        customer.Dated = dat + "/" + mon + "/" + customer.year.ToString();
                        customer.Suburb = sub.Id.ToString();
                        customer.City = sub.CityId.ToString();
                        customer.RegionId = sub.RegionId;
                        customer.Email = newAccount;
                        customer.Password = pass;
                        customer.CustomerName = customer.CustomerName.ToUpper();
                        customer.Username = customer.CustomerName.ToUpper();
                        customer.ParentId = uid;
                        customer.Balance = 0;
                        customer.Wallet = 0;
                        customer.Purchases = 0;
                        customer.PurchasesToDate = 0;
                        customer.Password = Pin;
                        customer.CompanyId = "1";
                        customer.NetworkType = 0;
                        customer.Level1Count = 0;
                        customer.Level2Count = 0;
                        customer.Level3Count = 0;
                        customer.wallet2 = 0;
                        customer.wallet3 = 0;

                        //customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                        db.Customers.Add(customer);
                        db.SaveChanges();

                        if (!string.IsNullOrEmpty(customer.code))
                        {
                            var cde = db.CustomerCodes.Where(u => u.Code.Trim() == customer.code).FirstOrDefault();
                            cde.Size = cde.Size - 1;
                            db.Entry(cde).State = EntityState.Modified;
                            db.SaveChanges();
                        }

                        #region Sendemail
                        // Initialize WebMail helper
                        var par = db.Customers.Find(uid);
                        MailMessage mailMessage = new MailMessage(
                                              new MailAddress("marketing@baskiti.com"), new MailAddress(em));

                        var ee = en.encrypt(customer.Email.ToLower().Trim() + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");

                        mailMessage.Subject = "Account Registration";
                        mailMessage.IsBodyHtml = true;
                        mailMessage.Body = "<style>a.buttons {display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;}p.capitalize {text-transform: capitalize;}</style>" +
                        "<div style='width: 100%'><div style='width:70%; margin:0 auto;'><div style='width:100%'><div style='height:45px;'>" +
                        "<a href='http://www.baskiti.com'><img style='width:200px' src='http://www.baskiti.com/Content/Template/img/logo.png' alt='baskiti'></a>" +
                        "<p class='" + "capitalize" + "'> Hi " + customer.Username + ",</p><p> Your registration to baskiti was successful!</p><p> Now that you have an account," +
                         " you can start enjoying this amazing shopping experience on the baskiti web site. We have thousands of product lines and brands you already know and use, we add new product lines everyday for your convenience.</p><p> Start building your network by simply inviting others to join the network. Invite a friend in one of two ways <br/>" +
                        " 1. Get their information and create an account for them using the “Create Account” menu which is at the top menu bar in your logged-in account <br/>" +
                        " 2. Invite them by using the “Send Invite” menu where they will register themselves.</p>" +
                        "<p>Use the following credentials to Signin.<br/><br/> Email Address = " + em + "<br/> Password = " + pass + "</p><br/>" +
                         "<p>To get started right away " +
                         "<a href=http://www.baskiti.com style='" + "display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;" + "' >Click This </a>" +
                         " to visit mobistore site.<br/> Or you can copy the link below and paste it in your browser address bar. </p>" +
                         "<p> http://www.baskiti.com </p><br/>" +
                         "<p>you can delete this email once you have signed in to baskiti.com to protect your password</p>" +
                         "</div></div></div></div>";


                        System.Net.NetworkCredential networkCredentials = new
                       System.Net.NetworkCredential("marketing@baskiti.com", "shoppa@2016");

                        SmtpClient smtpClient = new SmtpClient();
                        smtpClient.EnableSsl = false;
                        smtpClient.UseDefaultCredentials = false;
                        smtpClient.Credentials = networkCredentials;
                        smtpClient.Host = "smtpout.secureserver.net";
                        smtpClient.Port = 25;
                        smtpClient.Timeout = 10000;
                        smtpClient.Send(mailMessage);

                        #endregion
                       // var level = db.NetworkTypes.Find(1);
                        var parent0 = db.Customers.Find(uid);
                        if (parent0 != null)
                        {
                        if (parent0.Level1Count == null) parent0.Level1Count = 0;
                        parent0.Level1Count = parent0.Level1Count + 1;
                        db.Entry(parent0).State = EntityState.Modified;
                        db.SaveChanges();
                        }
                        var parent1 = db.Customers.Find(parent0.ParentId);
                        if (parent1 != null)
                        {
                            if (parent1.Level2Count == null) parent1.Level2Count = 0;
                            parent1.Level2Count = parent1.Level2Count + 1;
                            db.Entry(parent1).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        var parent2 = db.Customers.Find(parent1.ParentId);
                        if (parent2 != null)
                        {
                            
                            if (parent2.Level3Count == null) parent2.Level3Count = 0;
                            parent2.Level3Count = parent2.Level3Count + 1;
                            db.Entry(parent2).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                     
                        #region commissions
                        // this section the system will look through all this customers network for commissions
                        /*var parent0 = db.Customers.Find(long.Parse(part[1]));
                         if (parent0 != null)
                         {
                             #region Parent0
                             var totalsales = db.NetworkSales.Find(part[1] + "_" + DateTime.Now.Year);// total salesfor current user
                             bool update = true;
                             if (totalsales == null)
                             {
                                 totalsales = new NetworkSale();
                                 update = false;
                             }
                             #region totals
                             switch (DateTime.Now.Month)
                             {
                                 case 1:
                                     if (totalsales.JanJ0 == null)
                                     {
                                         totalsales.JanJ0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.JanJ0 = totalsales.JanJ0 + level.Fee;
                                     }
                                     break;
                                 case 2:
                                     if (totalsales.FebJ0 == null)
                                     {
                                         totalsales.FebJ0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.FebJ0 = totalsales.FebJ0 + level.Fee;
                                     }
                                     break;
                                 case 3:
                                     if (totalsales.MarJ0 == null)
                                     {
                                         totalsales.MarJ0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.MarJ0 = totalsales.MarJ0 + level.Fee;
                                     }
                                     break;
                                 case 4:
                                     if (totalsales.AprJ0 == null)
                                     {
                                         totalsales.Apr0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.AprJ0 = totalsales.AprJ0 + level.Fee;
                                     }
                                     break;
                                 case 5:
                                     if (totalsales.MayJ0 == null)
                                     {
                                         totalsales.MayJ0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.MayJ0 = totalsales.MayJ0 + level.Fee;
                                     }
                                     break;
                                 case 6:
                                     if (totalsales.JunJ0 == null)
                                     {
                                         totalsales.JunJ0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.JunJ0 = totalsales.JunJ0 + level.Fee;
                                     }
                                     break;
                                 case 7:
                                     if (totalsales.JulJ0 == null)
                                     {
                                         totalsales.JulJ0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.Jul0 = totalsales.JulJ0 + level.Fee;
                                     }
                                     break;
                                 case 8:
                                     if (totalsales.AugJ0 == null)
                                     {
                                         totalsales.AugJ0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.AugJ0 = totalsales.AugJ0 + level.Fee;
                                     }
                                     break;
                                 case 9:
                                     if (totalsales.SepJ0 == null)
                                     {
                                         totalsales.SepJ0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.SepJ0 = totalsales.SepJ0 + level.Fee;
                                     }
                                     break;
                                 case 10:
                                     if (totalsales.OctJ0 == null)
                                     {
                                         totalsales.OctJ0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.OctJ0 = totalsales.OctJ0 + level.Fee;
                                     }
                                     break;
                                 case 11:
                                     if (totalsales.NovJ0 == null)
                                     {
                                         totalsales.NovJ0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.NovJ0 = totalsales.NovJ0 + level.Fee;
                                     }
                                     break;
                                 case 12:
                                     if (totalsales.DecJ0 == null)
                                     {
                                         totalsales.DecJ0 = level.Fee;
                                     }
                                     else
                                     {
                                         totalsales.DecJ0 = totalsales.DecJ0 + level.Fee;
                                     }
                                     break;
                             }
                             #endregion

                             if (update == false)
                             {
                                 totalsales.Id = customer.ParentId + "_" + DateTime.Now.Year;
                                 totalsales.Year = DateTime.Now.Year;
                                 db.NetworkSales.Add(totalsales);
                                 db.SaveChanges();
                             }
                             else
                             {
                                 db.Entry(totalsales).State = EntityState.Modified;
                                 db.SaveChanges();
                             }
                             parent0.Wallet = parent0.Wallet + (level.Fee) * (decimal)0.15;
                             if (parent0.Level1Count == null) parent0.Level1Count = 0;
                             parent0.Level1Count  = parent0.Level1Count + 1;
                             db.Entry(parent0).State = EntityState.Modified;
                             db.SaveChanges();
                             #endregion
                             var parent1 = db.Customers.Find(parent0.ParentId);
                             if (parent1 != null)
                             {
                                 #region Parent1
                                 var totalsales1 = db.NetworkSales.Find(parent0.ParentId + "_" + DateTime.Now.Year);// total salesfor current user
                                 update = true;
                                 if (totalsales1 == null)
                                 {
                                     totalsales1 = new NetworkSale();
                                     update = false;
                                 }

                                 #region totals1
                                 switch (DateTime.Now.Month)
                                 {
                                     case 1:
                                         if (totalsales1.JanJ1 == null)
                                         {
                                             totalsales1.JanJ1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales1.JanJ1 = totalsales1.JanJ1 + level.Fee;
                                         }
                                         break;
                                     case 2:
                                         if (totalsales1.FebJ1 == null)
                                         {
                                             totalsales1.FebJ1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales1.FebJ1 = totalsales1.FebJ1 + level.Fee;
                                         }
                                         break;
                                     case 3:
                                         if (totalsales1.MarJ1 == null)
                                         {
                                             totalsales1.MarJ1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales1.MarJ1 = totalsales1.MarJ0 + level.Fee;
                                         }
                                         break;
                                     case 4:
                                         if (totalsales1.AprJ1 == null)
                                         {
                                             totalsales1.Apr1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales1.AprJ1 = totalsales.AprJ1 + level.Fee;
                                         }
                                         break;
                                     case 5:
                                         if (totalsales1.MayJ1 == null)
                                         {
                                             totalsales1.MayJ1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales1.MayJ1 = totalsales.MayJ1 + level.Fee;
                                         }
                                         break;
                                     case 6:
                                         if (totalsales1.JunJ1 == null)
                                         {
                                             totalsales1.JunJ1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales1.JunJ1 = totalsales.JunJ1 + level.Fee;
                                         }
                                         break;
                                     case 7:
                                         if (totalsales1.JulJ1 == null)
                                         {
                                             totalsales1.JulJ1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales1.Jul1 = totalsales1.JulJ1 + level.Fee;
                                         }
                                         break;
                                     case 8:
                                         if (totalsales1.AugJ1 == null)
                                         {
                                             totalsales1.AugJ1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales1.AugJ1 = totalsales.AugJ1 + level.Fee;
                                         }
                                         break;
                                     case 9:
                                         if (totalsales1.SepJ1 == null)
                                         {
                                             totalsales1.SepJ1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales1.SepJ1 = totalsales.SepJ1 + level.Fee;
                                         }
                                         break;
                                     case 10:
                                         if (totalsales1.OctJ1 == null)
                                         {
                                             totalsales1.OctJ1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales1.OctJ1 = totalsales.OctJ1 + level.Fee;
                                         }
                                         break;
                                     case 11:
                                         if (totalsales1.NovJ1 == null)
                                         {
                                             totalsales1.NovJ1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales1.NovJ1 = totalsales1.NovJ1 + level.Fee;
                                         }
                                         break;
                                     case 12:
                                         if (totalsales1.DecJ1 == null)
                                         {
                                             totalsales1.DecJ1 = level.Fee;
                                         }
                                         else
                                         {
                                             totalsales.DecJ1 = totalsales.DecJ1 + level.Fee;
                                         }
                                         break;
                                 }
                                 #endregion

                                 if (update == false)
                                 {
                                     totalsales1.Id = parent1.ParentId + "_" + DateTime.Now.Year;
                                     totalsales1.Year = DateTime.Now.Year;
                                     db.NetworkSales.Add(totalsales1);
                                     db.SaveChanges();
                                 }
                                 else
                                 {
                                     db.Entry(totalsales1).State = EntityState.Modified;
                                     db.SaveChanges();
                                 }
                                 parent1.Wallet = parent1.Wallet + (level.Fee) * (decimal)0.12;
                                 if (parent1.Level2Count == null) parent1.Level2Count = 0;
                                 parent1.Level2Count = parent1.Level2Count + 1;
                                 db.Entry(parent1).State = EntityState.Modified;
                                 db.SaveChanges();
                                 #endregion
                                 var parent2 = db.Customers.Find(parent1.ParentId);
                                 if (parent2 != null)
                                 {
                                     #region Parent2

                                     var totalsales2 = db.NetworkSales.Find(parent2.ParentId + "_" + DateTime.Now.Year);// total salesfor current iser
                                     update = true;
                                     if (totalsales2 == null)
                                     {
                                         totalsales2 = new NetworkSale();
                                         update = false;
                                     }

                                     #region totals2
                                     switch (DateTime.Now.Month)
                                     {
                                         case 1:
                                             if (totalsales2.JanJ2 == null)
                                             {
                                                 totalsales2.JanJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.JanJ2 = totalsales2.JanJ2 + level.Fee;
                                             }
                                             break;
                                         case 2:
                                             if (totalsales2.FebJ2 == null)
                                             {
                                                 totalsales2.FebJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.FebJ2 = totalsales2.FebJ2 + level.Fee;
                                             }
                                             break;
                                         case 3:
                                             if (totalsales2.MarJ2 == null)
                                             {
                                                 totalsales2.MarJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.MarJ2 = totalsales2.MarJ2 + level.Fee;
                                             }
                                             break;
                                         case 4:
                                             if (totalsales2.AprJ2 == null)
                                             {
                                                 totalsales2.AprJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.AprJ2 = totalsales2.AprJ2 + level.Fee;
                                             }
                                             break;
                                         case 5:
                                             if (totalsales2.MayJ2 == null)
                                             {
                                                 totalsales2.MayJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.MayJ2 = totalsales2.MayJ2 + level.Fee;
                                             }
                                             break;
                                         case 6:
                                             if (totalsales2.JunJ2 == null)
                                             {
                                                 totalsales2.JunJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.JunJ2 = totalsales2.JunJ2 + level.Fee;
                                             }
                                             break;
                                         case 7:
                                             if (totalsales2.JulJ2 == null)
                                             {
                                                 totalsales2.JulJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.JulJ2 = totalsales2.JulJ2 + level.Fee;
                                             }
                                             break;
                                         case 8:
                                             if (totalsales2.AugJ2 == null)
                                             {
                                                 totalsales2.AugJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.AugJ2 = totalsales2.AugJ2 + level.Fee;
                                             }
                                             break;
                                         case 9:
                                             if (totalsales2.SepJ2 == null)
                                             {
                                                 totalsales2.SepJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.SepJ2 = totalsales2.SepJ2 + level.Fee;
                                             }
                                             break;
                                         case 10:
                                             if (totalsales2.OctJ2 == null)
                                             {
                                                 totalsales2.OctJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.OctJ2 = totalsales2.OctJ2 + level.Fee;
                                             }
                                             break;
                                         case 11:
                                             if (totalsales2.OctJ2 == null)
                                             {
                                                 totalsales2.OctJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.OctJ2 = totalsales2.OctJ2 + level.Fee;
                                             }
                                             break;
                                         case 12:
                                             if (totalsales2.DecJ2 == null)
                                             {
                                                 totalsales2.DecJ2 = level.Fee;
                                             }
                                             else
                                             {
                                                 totalsales2.DecJ2 = totalsales2.DecJ2 + level.Fee;
                                             }
                                             break;
                                     }

                                     #endregion

                                     if (update == false)
                                     {
                                         totalsales2.Id = parent2.ParentId + "_" + DateTime.Now.Year;
                                         totalsales2.Year = DateTime.Now.Year;
                                         db.NetworkSales.Add(totalsales2);
                                         db.SaveChanges();
                                     }
                                     else
                                     {
                                         db.Entry(totalsales2).State = EntityState.Modified;
                                         db.SaveChanges();
                                     }
                                     if (parent2.NetworkType >= 2)
                                     {
                                         parent2.Wallet = parent2.Wallet + (level.Fee) * (decimal)0.08;
                                     }
                                     if (parent2.Level3Count == null) parent2.Level3Count = 0;
                                     parent2.Level3Count = parent2.Level3Count + 1;
                                     db.Entry(parent1).State = EntityState.Modified;
                                     db.SaveChanges();
                                     #endregion
                                     var parent3 = db.Customers.Find(parent2.ParentId);
                                     if (parent3 != null)
                                     {
                                         #region Parent3

                                         var totalsales3 = db.NetworkSales.Find(parent3.ParentId + "_" + DateTime.Now.Year);// total salesfor current iser
                                         update = true;
                                         if (totalsales3 == null)
                                         {
                                             totalsales3 = new NetworkSale();
                                             update = false;
                                         }

                                         #region totals3
                                         switch (DateTime.Now.Month)
                                         {
                                             case 1:
                                                 if (totalsales3.JanJ3 == null)
                                                 {
                                                     totalsales3.JanJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.JanJ3 = totalsales3.JanJ3 + level.Fee;
                                                 }
                                                 break;
                                             case 2:
                                                 if (totalsales3.FebJ3 == null)
                                                 {
                                                     totalsales3.FebJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.FebJ3 = totalsales3.FebJ3 + level.Fee;
                                                 }
                                                 break;
                                             case 3:
                                                 if (totalsales3.MarJ3 == null)
                                                 {
                                                     totalsales3.MarJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.MarJ3 = totalsales3.MarJ3 + level.Fee;
                                                 }
                                                 break;
                                             case 4:
                                                 if (totalsales3.AprJ3 == null)
                                                 {
                                                     totalsales3.AprJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.AprJ3 = totalsales3.AprJ3 + level.Fee;
                                                 }
                                                 break;
                                             case 5:
                                                 if (totalsales3.MayJ3 == null)
                                                 {
                                                     totalsales3.MayJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.MayJ3 = totalsales3.MayJ3 + level.Fee;
                                                 }
                                                 break;
                                             case 6:
                                                 if (totalsales3.JunJ3 == null)
                                                 {
                                                     totalsales3.JunJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.JunJ3 = totalsales3.JunJ3 + level.Fee;
                                                 }
                                                 break;
                                             case 7:
                                                 if (totalsales3.JulJ3 == null)
                                                 {
                                                     totalsales3.JulJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.JulJ3 = totalsales3.JulJ3 + level.Fee;
                                                 }
                                                 break;
                                             case 8:
                                                 if (totalsales3.AugJ3 == null)
                                                 {
                                                     totalsales3.AugJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.AugJ3 = totalsales3.AugJ3 + level.Fee;
                                                 }
                                                 break;
                                             case 9:
                                                 if (totalsales3.SepJ3 == null)
                                                 {
                                                     totalsales3.SepJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.SepJ3 = totalsales3.SepJ3 + level.Fee;
                                                 }
                                                 break;
                                             case 10:
                                                 if (totalsales3.OctJ3 == null)
                                                 {
                                                     totalsales3.OctJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.OctJ3 = totalsales3.OctJ3 + level.Fee;
                                                 }
                                                 break;
                                             case 11:
                                                 if (totalsales3.OctJ3 == null)
                                                 {
                                                     totalsales3.OctJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.OctJ3 = totalsales3.OctJ3 + level.Fee;
                                                 }
                                                 break;
                                             case 12:
                                                 if (totalsales3.DecJ3 == null)
                                                 {
                                                     totalsales3.DecJ3 = level.Fee;
                                                 }
                                                 else
                                                 {
                                                     totalsales3.DecJ3 = totalsales3.DecJ3 + level.Fee;
                                                 }
                                                 break;
                                         }
                        
                                         #endregion

                                         if (update == false)
                                         {
                                             totalsales3.Id = parent3.ParentId + "_" + DateTime.Now.Year;
                                             totalsales3.Year = DateTime.Now.Year;
                                             db.NetworkSales.Add(totalsales3);
                                             db.SaveChanges();
                                         }
                                         else
                                         {
                                             db.Entry(totalsales3).State = EntityState.Modified;
                                             db.SaveChanges();
                                         }
                                         if (parent3.NetworkType == 3)
                                         {
                                             parent3.Wallet = parent3.Wallet + (level.Fee) * (decimal)0.05;
                                             db.Entry(parent3).State = EntityState.Modified;
                                             db.SaveChanges();
                                         }
                                         #endregion
                                     }

                                 }
                             }
                         }
                      */
                        #endregion

                        ViewData["Success"] = "Success";
                        return View(customer);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sorry this customer name is already in use");
                        return View(customer);
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Make sure all fields are field in");
                    return View(customer);
                }
            }
      
        [HttpGet]
        public ActionResult CodeRegister()
        {
           return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
       // [CaptchaValidation("CaptchaCode", "SampleCaptcha", "Incorrect CAPTCHA code!")]
        public ActionResult CodeRegister(string regcode, bool captchaValid)
        {
            var customers = new Customer();
            if ( regcode != null && captchaValid)
            {
                List<SelectListItem> yr = new List<SelectListItem>();
                // List<SelectListItem> yrd = new List<SelectListItem>();
                yr.Add(new SelectListItem
                {
                    Text = "Year",
                    Value = ""
                });

                for (int i = DateTime.Now.Year; i >= 1935; i--)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = i.ToString(),
                        Value = i.ToString()
                    });
                }

                ViewBag.Usertype = yr;
                var nets = db.NetworkTypes.ToList();
                List<SelectListItem> cc = new List<SelectListItem>();

                foreach (var item in nets)
                {
                    cc.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewData["Success"] = "";
                ViewBag.Level = cc;
                ViewData["NetwokFee"] = nets.FirstOrDefault().Fee.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                var cities = db.Cities.ToList();

                List<SelectListItem> ct = new List<SelectListItem>();
                ct.Add(new SelectListItem
                {
                    Text = "-Select City-",
                    Value = "-Select City-"
                });
                foreach (var item in cities)
                {
                    ct.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Cities = ct;
                var cid = cities.FirstOrDefault().Id;
                var surb = db.Suburbs.Where(u => u.CityId == cid).ToList();
                var area = db.Regions.Where(u => u.CityId == cid).ToList();
                List<SelectListItem> sb = new List<SelectListItem>();
                sb.Add(new SelectListItem
                {
                    Text = "-Select Suburb-",
                    Value = "-Select Suburb-"
                });
                foreach (var item in surb)
                {
                    sb.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Suburb = sb;

                List<SelectListItem> rg = new List<SelectListItem>();
                rg.Add(new SelectListItem
                {
                    Text = "-Select Region-",
                    Value = "-Select Region-"
                });
                foreach (var item in area)
                {
                    rg.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Area = rg;

                customers = new Customer();
                var pz = db.Customers.ToList();
                var pp = (from e in pz
                          .OrderBy(u => u.ID)
                          select e).LastOrDefault();

                if (pp == null)
                {
                    pp = new Customer();
                    customers.AccountCode = "8-01-00001";
                }
                else
                {
                    char[] delimiter = new char[] { '-' };
                    string[] part = pp.AccountCode.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    var Id = long.Parse(part[2]);
                    string acc = "";
                    var x = Id + 1;
                    if (x < 10)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "0000" + x.ToString();
                    }
                    else if (x > 9 && x < 100)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "000" + x.ToString();
                    }
                    else if (x > 99 && x < 1000)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "00" + x.ToString();
                    }
                    else if (x > 999 && x < 10000)
                    {
                        acc = part[0] + "-" + part[1] + "-" + "0" + x.ToString();
                    }
                    else if (x > 9999 && x < 100000)
                    {
                        acc = part[0] + "-" + part[1] + "-" + x.ToString();
                    }
                    else if (x > 9999)
                    {
                        var Idd = long.Parse(part[1]);
                        var xx = Idd + 1;
                        if (xx < 10)
                        {
                            acc = part[0] + "-" + "0" + xx.ToString() + "-" + "00001";
                        }
                        else if (xx > 9 && xx < 99)
                        {
                            acc = part[0] + "-" + xx.ToString() + "-" + "00001";
                        }
                        else
                        {
                            acc = "Exhausted";
                        }
                    }
                    customers.AccountCode = acc;
                    customers.CompanyId = "1";
                }
                if (regcode != null)
                {
                    try
                    {
                        var rcc = regcode;
                        var cod = db.CustomerCodes.Where(u => u.Code.Trim() == rcc).FirstOrDefault();

                        if (cod != null && cod.Size  > 0)
                        {
                            customers.ParentId = cod.CustomerId;
                        }
                        else
                        {
                            ModelState.AddModelError("","Sorry the registration code is invalid. Check to see if you entered the correct code");
                            return View();
                        }

                    }
                    catch (Exception)
                    {
                         ModelState.AddModelError("","Sorry the registration code is invalid. Check to see if you entered the correct code");
                        return View();
                    }
                }

                var slides = db.Items.Where(u => u.Promotion.Trim() == "YES" & u.AdPosition.StartsWith("Slide") & u.SDate <= DateTime.Now & u.EDate >= DateTime.Now).Take(4).ToList();
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                customers.categories = categories;
                return View("Register",customers);
            }
            else
            {
                string error="Please retry your registration. Make sure you enter exactly what you see in the image";
                ModelState.AddModelError("", error);
                return View();
            }
        }
  
        [Authorize]
        [HttpGet]
        public ActionResult Invite()
        {
            Invite inv = new Invite();
             var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
             inv.categories = categories;
             ViewData["Resp"] = "start";
            // return RedirectToAction("GmailContacts");
            return View(inv);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Invite(Invite inv)
        {
            var invt = db.Invites.Where(u => u.Email.Trim() == inv.Email).FirstOrDefault();
            var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
            inv.categories = categories;
            if ((invt != null && (DateTime.Now.Month - invt.Dated.Value.Month) > 1 && invt.Status.Trim() == "O") || invt == null)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
               
                if (inv.Email == null || inv.FriendName == null)
                {
                    ViewData["Resp"] = "Error";

                    return View(inv);
                }
                if (invt != null)
                {
                    invt.Dated = DateTime.Now;
                    invt.ParentId = long.Parse(part[1]);
                    db.Entry(invt).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    inv.ParentId = long.Parse(part[1]);
                    inv.Dated = DateTime.Now;
                    inv.Status = "O";
                    db.Invites.Add(inv);
                    db.SaveChanges();
                }
    

                ST_encrypt en = new ST_encrypt();
                ST_decrypt dec = new ST_decrypt();
                #region Sendemail
                // Initialize WebMail helper
                var par = db.Customers.Find(long.Parse(part[1]));
                MailMessage mailMessage = new MailMessage(
                                      new MailAddress("marketing@baskiti.com"), new MailAddress(inv.Email));

                var ee = en.encrypt(inv.Email.ToLower().Trim() + "/" + inv.Id + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");
                var eml = dec.st_decrypt(par.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
                if (inv.sender != null) par.ContactPerson = inv.sender;
                if (inv.senderEmail != null) eml = inv.senderEmail;
                MailAddress from = new MailAddress("marketing@baskiti.com", par.ContactPerson);
                mailMessage.From = from;
                mailMessage.Subject = "Invitation";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body =
                "<style>a.buttons {display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;} .capitalize {text-transform: capitalize;}</style>" +
                "<div style='width: 100%'><div style='width:70%; margin:0 auto '><div style='height:45px; '>" +
                "<a href='http://www.baskiti.com'><img style='width:200px' src='http://www.baskiti.com/Content/Template/img/logo.png' alt='baskiti'></a>" +
                "</div><div style='margin-top:-20px'><div style='margin:10px 15px 10px 10px'><br /><p class='" + "capitalize" + "'>Hi " + inv.FriendName + "</p><p>" +
                "<span class='capitalize'>" + par.ContactPerson + "</span> is a member of the Baskiti Network and is inviting you to explore an exciting business opportunity on the first ever online store which turns your expense into income." +
                "There are <strong style='color:red'>no joining fees </strong>, you just buy your essentials online and earn money. How? Grow your Shopper’s Network by inviting others to join and you will be awarded a percentage of all purchases made by people you register or refer to the Baskiti Network." +
                "</p><p>To find out more contact <span class='capitalize'> " + par.ContactPerson + "</span> on email " + eml + " for an explanation on how the business works. Click the “Accept Invite”" +
                " button below to get your registration form OR click the “Learn More” button to get more information.</p><br />" +
                "</div></div><div style='background-color:#F3F3F3; height:50px; margin-top:-20px'><div style='margin:5px 5px; padding-top:5px'>" +
                "<a href=http://www.baskiti.com/Account/Register?ic=" + ee + " style='" + "display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;" + "' >Accept Invite </a>" +
                "<a href='http://www.baskiti.com/Home/About' style='" + "display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;" + "'>Learn More</a></div></div></div></div>";


                System.Net.NetworkCredential networkCredentials = new
                System.Net.NetworkCredential("marketing@baskiti.com", "shoppa@2016");

                SmtpClient smtpClient = new SmtpClient();
                
                smtpClient.EnableSsl = false;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = networkCredentials;
                smtpClient.Host = "smtpout.secureserver.net";
                smtpClient.Port = 25;
                smtpClient.Timeout = 10000;
                smtpClient.Send(mailMessage);
                // Send email


                #endregion
                ViewData["Resp"] = "Success";
            }
            else
            {
                if (invt.Status.Trim() == "O")
                {
                    ViewData["Resp"] = "This person has already been invited but has not accepted, try again after a month if they are not a member your invite will go through";
                }
                else if (invt.Status.Trim() == "A")
                {
                    ViewData["Resp"] = "This person is already a member";
                }
            }
            return View(inv);
           
        }

        public ActionResult GmailContacts()
        {
          
            List<MailContacts> emailList = new List<MailContacts>();
            RequestSettings _requestSettings = new RequestSettings("Mobistore","");
            _requestSettings.AutoPaging = true;
            ContactsRequest _contactsRequest = new ContactsRequest(_requestSettings);
            Feed<Google.Contacts.Contact> ContactList = _contactsRequest.GetContacts();
            try
            {

                foreach (var contact in ContactList.Entries)
                {
                    foreach (EMail email in contact.Emails)
                    {
                        MailContacts gc = new MailContacts();
                        gc.Address = email.Address;
                        gc.Name = email.Label;
                        emailList.Add(gc);
                    }
                }

            }

            catch (Exception ex)
            {

            }


            return View(emailList);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Email(long id=0)
        {
            ST_encrypt en = new ST_encrypt();
            ST_decrypt dec = new ST_decrypt();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

          
            var inv = db.Invites.Find(id);
            var tt = (DateTime.Now - inv.Dated);
            ViewData["sent"] = "sent";
            ViewData["Id"] = id;
            if(inv.Status.Trim() == "F" ||(tt.Value.Days) < 8 )
            { 
                ViewData["sent"]= "This email has already been sent. Try again in a week.";
            }

                #region Sendemail
                // Initialize WebMail helper
                var par = db.Customers.Find(long.Parse(part[1]));
                MailMessage mailMessage = new MailMessage(
                                      new MailAddress("marketing@baskiti.com"), new MailAddress(inv.Email));

                var ee = en.encrypt(inv.Email.ToLower().Trim() + "/" + inv.Id + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");
                var eml = dec.st_decrypt(par.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
                if (inv.sender != null) par.ContactPerson = inv.sender;
                if (inv.senderEmail != null) eml = inv.senderEmail;
                MailAddress from = new MailAddress(eml, par.ContactPerson);
                mailMessage.From = from;
                mailMessage.Subject = inv.FriendName + " please join my network.";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body =
                "<style>a.buttons {display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;} .capitalize {text-transform: capitalize;}</style>" +
                "<div style='width: 100%'><div style='width:70%; margin:0 auto '><div style='height:45px; '>" +
                "<a href='http://www.baskiti.com'><img style='width:200px' src='http://www.baskiti.com/Content/Template/img/logo.png' alt='baskiti'></a>" +
                "</div><div style='margin-top:-20px'><div style='margin:10px 15px 10px 10px'><br /><p class='" + "capitalize" + "'>Hi " + inv.FriendName + "</p><p>" +
                "<span class='capitalize'>" + par.ContactPerson + "</span> making a follow-up on the baskiti invite that was sent to you a while back.To sign-up onto baskiti, click the “Accept Invite” button below and complete the short registration form. This should take about 2-5 minutes, and you’ll be well on your way to convenient shopping" +
                ". There are <strong style='color:red'>no joining fees </strong>, you just buy your essentials online and earn money. " +
                "</p><p>If you are having any challenges, send our support team an email on support@baskiti.com and they will assist you" +
                ".</p><br />" +
                "</div></div><div style='background-color:#F3F3F3; height:50px; margin-top:-20px'><div style='margin:5px 5px; padding-top:5px'>" +
                "<a href=http://www.baskiti.com/Account/Register?ic=" + ee + " style='" + "display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;" + "' >Accept Invite </a>" +
                "<a href='http://www.baskiti.com/Home/About' style='" + "display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;" + "'>Learn More</a></div></div></div></div>";

                System.Net.NetworkCredential networkCredentials = new
                System.Net.NetworkCredential("marketing@baskiti.com", "shoppa@2016");

                SmtpClient smtpClient = new SmtpClient();

                smtpClient.EnableSsl = false;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = networkCredentials;
                smtpClient.Host = "smtpout.secureserver.net";
                smtpClient.Port = 25;
                smtpClient.Timeout = 30000;
                smtpClient.Send(mailMessage);
                // Send email


                #endregion

                ViewData["message"] = "<style>a.buttons {display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;} .capitalize {text-transform: capitalize;}</style>" +
                "<div style='width: 100%'><div style='width:70%; margin:0 auto '><div style='height:45px; '>" +
                "<a href='http://www.baskiti.com'><img style='width:200px' src='http://www.baskiti.com/Content/Template/img/logo.png' alt='baskiti'></a>" +
                "</div><div style='margin-top:-20px'><div style='margin:10px 15px 10px 10px'><br /><p class='" + "capitalize" + "'>Hi " + inv.FriendName + "</p><p>" +
                "<span class='capitalize'>" + par.ContactPerson + "</span> making a follow-up on the Baskiti invite that was sent to you a while back.To sign-up onto baskiti, click the “Accept Invite” button below and complete the short registration form. This should take about 2-5 minutes, and you’ll be well on your way to convenient shopping" +
                ". There are <strong style='color:red'>no joining fees </strong>, you just buy your essentials online and earn money. " +
                "</p><p>If you are having any challenges, send our support team an email on support@baskiti.com and they will assist you" +
                ".</p><br />" +
                "</div></div><div style='background-color:#F3F3F3; height:50px; margin-top:-20px'><div style='margin:5px 5px; padding-top:5px'>" +
                "<a  style='" + "display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;" + "' >Accept Invite </a>" +
                "<a  style='" + "display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;" + "'>Learn More</a></div></div></div></div>";

                inv.ParentId = long.Parse(part[1]);
                inv.Dated = DateTime.Now;
                inv.Status = "F";
                db.Entry(inv).State = EntityState.Modified;
                db.SaveChanges();
                
           
            return PartialView();
        }

        [HttpGet]
        public ActionResult GetLevels(long Id)
        {
           var level = db.NetworkTypes.Find(Id);
           var ll = level.Fee.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
           return Json("$" + ll, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult GetSuburbs(string Id, long City)
        {
            if (Id == null) Id = "";
            Id = Id.Trim();
            var level = db.Suburbs.Where(u=>u.Name.StartsWith(Id) && u.CityId == City ).ToList();
            List<SelectListItem> sb = new List<SelectListItem>();
            sb.Add(new SelectListItem
            {
                Text = "-Select Suburb-",
                Value = "-Select Suburb-"
            });
            foreach (var item in level)
            {
                sb.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Suburb = sb;

            return PartialView(level);
        }
     
        [HttpGet]
        public ActionResult GetRegions(long Id)
        {
            var level = db.Regions.Where(u => u.CityId == Id).ToList();
            List<SelectListItem> sb = new List<SelectListItem>();
            sb.Add(new SelectListItem
            {
                Text = "-Select Region-",
                Value = "-Select Region-"
            });
            foreach (var item in level)
            {
                sb.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            ViewBag.Regions = sb;

            return PartialView();
        }

        [HttpGet]
        public ActionResult DashBoard()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                bool isMobile = false;
                string um = Request.ServerVariables["HTTP_USER_AGENT"];
                Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if ((b.IsMatch(um) || v.IsMatch(um.Substring(0, 4)) || Request.Browser.IsMobileDevice))
                {
                    isMobile = true;
                }

                var ana = new Analytics();
                ana.logViews("_Dash", isMobile);


                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var Id = long.Parse(part[1]);
                var acc = db.Customers.Find(long.Parse(part[1]));
                if(acc.Period != DateTime.Now.Month)
                {
                    Monthend mend = new Monthend();
                    mend.ChangePeriod(acc); // run month end
                }
                var cc = db.CustomerCodes.Where(u => u.CustomerId == Id).FirstOrDefault();
                ViewBag.code = cc;
                var totals = db.NetworkSales.Find(acc.ID + "_" + DateTime.Now.Year);
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                acc.totals = totals;
                acc.categories = categories;
                acc.networktyp = db.NetworkTypes.ToList();
                
                return View(acc);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult MyProject() // a list of all the people you want to invite 
        {
            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parts = CurrentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                long id = long.Parse(parts[1]); 
                var cust = db.Customers.Find(long.Parse(parts[1]));
                var inv = db.Invites.Where(u => u.ParentId == id).ToList();
                return PartialView(inv);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult ProgressCheck(long id = 0) // a list of all the people you want to invite 
        {
            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parts = CurrentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                long Mid = long.Parse(parts[1]); 
                var th = db.Invites.Find(id);
                ST_encrypt en = new ST_encrypt();

                string newAccount = en.encrypt(th.Email.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                var cust = db.Customers.Where(u => u.Email.Trim() == newAccount).FirstOrDefault();
                if (cust == null) cust = db.Customers.Where(u => u.CustomerName.Contains(th.FriendName.Trim()) && u.ParentId == Mid).FirstOrDefault();
               

                if (cust != null)
                {
                    var inv = db.Invites.Where(u => u.ParentId == cust.ID).ToList();
                    if (cust.Purchases == null) cust.Purchases = 0;
                    if (cust.Period == null) cust.Period = 0;
                    if (cust.Purchases > 0 && cust.Period == DateTime.Now.Month)
                    {
                        ViewData["Purchases"] = cust.CustomerName.Trim() + " has made a purchase this month.";
                    }
                    else if (cust.Purchases == 0 && cust.Period == DateTime.Now.Month)
                    {
                        ViewData["Purchases"] = cust.CustomerName.Trim() + " has not yet made a purchase.";
                    }
                    else
                    {
                        ViewData["Purchases"] = "We had not heard from " + cust.CustomerName.Trim() + " this month. You may want to get in touch.";
                    }

                    ViewData["Size"] = inv.Count();
                    var Acceptd = inv.Where(u => u.Status.Trim() == "A").Count();
                    var pend = inv.Where(u => u.Status.Trim() == "O" || u.Status.Trim() == "F").Count();
                    ViewData["Accepted"] = Acceptd;
                    ViewData["Pending"] = pend;


                    if (inv.Count() == 0)
                    {
                        ViewData["Action"] = "You might need to explain the benefits of recruiting to " + cust.CustomerName.Trim();
                    }
                    else if (pend > Acceptd || Acceptd == 0)
                    {
                        ViewData["Action"] = cust.CustomerName.Trim() + " might need your help to explaining the concept to new recruits";
                    }
                    else
                    {
                        ViewData["Action"] = "Give " + cust.CustomerName.Trim() + " a pat on the back for effort.";
                    }
                    ViewData["Error"] = "False";
                    return PartialView();
                }
                else
                {
                    ViewData["Error"] = th.FriendName;
                    return PartialView();
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }
        
        [HttpGet]
        public ActionResult CodePurchase()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var Id = long.Parse(part[1]);
                var acc = db.Customers.Find(long.Parse(part[1]));
                var codes = db.RegCodes.ToList();
                if (Request.IsAjaxRequest()) return PartialView(codes);
                return View(codes);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

       
        [HttpGet]
        public ActionResult CodePay(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var Id = long.Parse(part[1]);
                var cust = db.Customers.Find(long.Parse(part[1]));
                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                if (oda != null)
                {
                    var ols = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept.Trim()).ToList();

                    db.Orders.Remove(oda);
                    db.SaveChanges();
                    foreach (var itm in ols)
                    {
                        db.OrderLines.Remove(itm);
                        db.SaveChanges();
                    }
                }

                var item = db.RegCodes.Find(id);

                OrderLine ordaline = new OrderLine();
                Order orda = new Order();
                
                    ordaline.Discount = 0;
                    ordaline.item = item.Name ;
                    ordaline.ItemCode = item.Id.ToString() ;
                    ordaline.Category = "REGISTRATION";
                    ordaline.SubCategory = "CODE";
                    ordaline.quantity = item.Size;
                    ordaline.price = item.Price;
                    ordaline.Description = "REG CODES";
                    ordaline.priceinc = item.Price;
                    ordaline.tax = 0;
                
                    ordaline.Dated = DateTime.Now;
                    ordaline.Company = "MOBISTORE";

                    var rand = new Random();
                    var num = rand.Next(0, cust.CustomerName.Length - 3);
                    orda.Account = cust.AccountCode;
                    orda.customer = cust.CustomerName;
                    orda.dated = DateTime.Now;
                    orda.discount = ordaline.Discount;
                    orda.Tax = ordaline.tax;
                    orda.total = item.Price;
                    orda.state = "O";
                    orda.CollectionId = 3; // collection types are delivery 0, collection 1, sms 2, none 3
                    orda.DeliveryType = "None";
                

                    db.Orders.Add(orda);
                    db.SaveChanges();
                    orda.Reciept = cust.CustomerName.Substring(num, 3) + orda.ID + "." + DateTime.Now.TimeOfDay.ToString("hhmmss") + "." + DateTime.Now.ToString("ddMMyyyy");
                    db.Entry(orda).State = EntityState.Modified;
                    db.SaveChanges();

                    ordaline.Reciept = orda.Reciept;
                    db.OrderLines.Add(ordaline);
                    db.SaveChanges();
                    ViewData["OrderStatus"] = "OPEN";

                    return RedirectToAction("Payment", "Cart");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpGet]
        public ActionResult CashOut()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var Id = long.Parse(part[1]);
                var cust = db.Customers.Find(long.Parse(part[1]));
                if (cust.wallet2 == null) cust.wallet2 = 0;
                ViewData["Total"] = cust.wallet2;
                ViewData["Status"] = "";
                var cm = db.CashoutTypes.ToList();

                if (Request.IsAjaxRequest()) return PartialView(cm);
                return View(cm);
            }
            else
            {
                return RedirectToAction("Index", "Home");
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

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               // np.dis();
            }
            base.Dispose(disposing);
        }
    }
}