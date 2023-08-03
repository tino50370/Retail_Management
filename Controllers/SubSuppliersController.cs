using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.DataAcess;
using crypto;
using System.Net.Mail;
using System.Net;
using System.IO;
using System.Web.Script.Serialization;

namespace RetailKing.Controllers
{   [Authorize]
    public class SubSuppliersController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Suppliers/

         public ActionResult Index(string Company)
         {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            string parent = part[0];
            var px = db.Suppliers.Where(u => u.ParentId == parent).ToList();
           
            //px = px.Where(u => u.Company == Company).ToList();
            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }
            var nn = (from emp in px.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                      select emp).ToList();

            int pages = px.Count / param.iDisplayLength;
            if (px.Count > 0 && pages == 0)
            {
                pages = 1;
            }
            else if (px.Count > 0 && px.Count % param.iDisplayLength > 0)
            {
                pages += 1;
            }
            int page = param.iDisplayStart;

            int start = (param.iDisplayStart - 1) * param.iDisplayLength;
            if (start == 0) start = 1;
            ViewData["listSize"] = param.iDisplayLength;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;

            ViewData["Company"] = Company;

            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);
          
        }

         [HttpPost]
         public ActionResult Index(string category, string ItemCode, JQueryDataTableParamModel param)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var CurrentUser = User.Identity.Name;
             char[] delimiter = new char[] { '~' };
             string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

             string parent = part[0];
             var px = db.Suppliers.Where(u => u.ParentId == parent).ToList();

             if (param.iDisplayStart == 0)
             {
                 param.iDisplayStart = 1;
             }
             if (param.iDisplayLength == 0)
             {
                 param.iDisplayLength = 20;
             }
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                px = px.Where(u => u.Branch.ToLower().Contains(param.sSearch.ToLower())).ToList();
            }
            var nn = (from emp in px.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                       select emp).ToList();

             int pages = px.Count / param.iDisplayLength;
             if (px.Count > 0 && pages == 0)
             {
                 pages = 1;
             }
             else if (px.Count > 0 && px.Count % param.iDisplayLength > 0)
             {
                 pages += 1;
             }
             int page = param.iDisplayStart;

             int start = (param.iDisplayStart - 1) * param.iDisplayLength;
             if (start == 0) start = 1;
             ViewData["listSize"] = param.iDisplayLength;
             ViewData["Pages"] = pages;
             ViewData["ThisPage"] = page;
             ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;

             if (Request.IsAjaxRequest())
                 return PartialView(nn);
             return View(nn);
         }

        public ActionResult Inactive(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            string parent = part[0];
            var datee = DateTime.Now.AddDays(-3);
            var px = db.Suppliers.Where(u => u.ParentId == parent &&(u.Dated == null || u.Dated <= datee)).ToList();

            //px = px.Where(u => u.Company == Company).ToList();
            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                px = px.Where(u => u.Branch.Contains(param.sSearch)).ToList();
            }
            var nn = (from emp in px.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                      select emp).ToList();

            int pages = px.Count / param.iDisplayLength;
            if (px.Count > 0 && pages == 0)
            {
                pages = 1;
            }
            else if (px.Count > 0 && px.Count % param.iDisplayLength > 0)
            {
                pages += 1;
            }
            int page = param.iDisplayStart;

            int start = (param.iDisplayStart - 1) * param.iDisplayLength;
            if (start == 0) start = 1;
            ViewData["listSize"] = param.iDisplayLength;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;

            ViewData["Company"] = Company;

            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);

        }

        [HttpPost]
        public ActionResult Inactive(string category, string ItemCode, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            string parent = part[0];
            var datee = DateTime.Now.AddDays(-3);
            var px = db.Suppliers.Where(u => u.ParentId == parent && (u.Dated == null || u.Dated <= datee)).ToList();


            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }
            var nn = (from emp in px.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                      select emp).ToList();

            int pages = px.Count / param.iDisplayLength;
            if (px.Count > 0 && pages == 0)
            {
                pages = 1;
            }
            else if (px.Count > 0 && px.Count % param.iDisplayLength > 0)
            {
                pages += 1;
            }
            int page = param.iDisplayStart;

            int start = (param.iDisplayStart - 1) * param.iDisplayLength;
            if (start == 0) start = 1;
            ViewData["listSize"] = param.iDisplayLength;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;

            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);
        }
        //
        // GET: /Suppliers/Details/5

        public ActionResult Details(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
           
            Supplier customer = np.GetSuppliers(id);
            return PartialView(customer);
        }

 
        public ActionResult Browse(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string parent = part[0];
            var px = db.Suppliers.Where(u => u.ParentId == parent).ToList();
           
            return PartialView(px);
        } 
        //
        // GET: /Suppliers/Create

        public ActionResult Create( string Company)
        {
           NHibernateDataProvider np = new NHibernateDataProvider();
           var CurrentUser = User.Identity.Name;
           char[] delimitr = new char[] { '~' };
           string[] partz = CurrentUser.Split(delimitr, StringSplitOptions.RemoveEmptyEntries);
           string parent = partz[0];
           var pfr = db.Suppliers.ToList();
           var pr = pfr.OrderBy(u => u.ID).ToList();
           var ppar = pr.Where(u => u.AccountCode ==parent ).FirstOrDefault();
           Supplier pp = new Supplier();
           if (pr != null)
           {
               pp = (from e in pr select e).LastOrDefault();
           }
           Supplier px = new Supplier();
           px.Company = Company;
           if (pp == null)
           {
               pp = new Supplier();
               px.AccountCode = "5-0001-0000001";
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
                   acc = part[0] + "-" + part[1] + "-" + "000000" + x.ToString();
               }
               else if (x > 9 && x < 100)
               {
                   acc = part[0] + "-" + part[1] + "-" + "00000" + x.ToString();
               }
               else if (x > 99 && x < 1000)
               {
                   acc = part[0] + "-" + part[1] + "-" + "0000" + x.ToString();
               }
               else if (x > 999 && x < 10000)
               {
                   acc = part[0] + "-" + part[1] + "-" + "000" + x.ToString();
               }
               else if (x > 9999 && x < 100000)
               {
                   acc = part[0] + "-" + part[1] + "-" + "00" + x.ToString();
               }
               else if (x > 99999 && x < 1000000)
               {
                   acc = part[0] + "-" + part[1] + "-" + "0" + x.ToString();
               }
               else if (x > 999999 && x < 10000000)
               {
                   acc = part[0] + "-" + part[1] + "-" + x.ToString();
               }
               else if (x > 999999)
               {
                   var Idd = long.Parse(part[1]);
                   var xx = Idd + 1;
                   if (xx < 10)
                   {
                       acc = part[0] + "-" + "000" + xx.ToString() + "-" + "0000001";
                   }
                   else if (xx > 9 && xx < 100)
                   {
                       acc = part[0] + "-" + "00" + xx.ToString() + "-" + "0000001";
                   }
                   else if (xx > 99 && xx < 1000)
                   {
                       acc = part[0] + "-" + "0" + xx.ToString() + "-" + "0000001";
                   }
                   else if (xx > 999 && xx < 10000)
                   {
                       acc = part[0] + "-" + xx.ToString() + "-" + "0000001";
                   }
                   else
                   {
                       acc = "Exhausted";
                   }
               }
                px.AccountCode = acc;
            }
           px.ParentId = partz[0];
           px.ServiceType = ppar.ServiceType;
           px.SupplierName = ppar.SupplierName;
           return PartialView(px);
        } 

        //
        // POST: /Suppliers/Create

        [HttpPost]
        public ActionResult Create(Supplier customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid)
            {
                var accs = db.Suppliers.Where(u => u.Branch == customer.Branch).ToList();
                if (accs == null || accs.Count == 0)
                {
                    customer.SupplierName  = customer.SupplierName.ToUpper();
                    customer.ContactPerson = customer.ContactPerson.ToUpper();
                    customer.Branch = customer.Branch.ToUpper();
                    customer.Balance = 0;
                    //customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                    np.AddSupplier(customer);

                    ST_encrypt en = new ST_encrypt();
                    var unam = customer.Email;
                    string newAccount = en.encrypt(unam, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    Random rr = new Random();
                    var Pin = rr.Next(00000, 99999).ToString();
                    var ePin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    var question = en.encrypt(customer.AccountCode, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    string Answer = customer.ServiceType;// "Agent";
                    string ssAnswer = en.encrypt(Answer, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    string conn = en.encrypt(customer.SupplierName, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    //customer.Company = en.encrypt(customer.Company.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    // Check is exists 
                    AccountCredenitial acnt = db.AccountCredenitials.Find(newAccount);
                    if (acnt == null || acnt.AccountNumber.Trim() != newAccount)
                    {

                        AgentUser uza = new AgentUser();
                        uza.UserName = customer.AccountCode;
                        uza.FullName = customer.ContactPerson;
                        uza.CreationDate = DateTime.Now;
                        uza.AccountNumber = customer.AccountCode;
                        uza.TerminalId = 1;
                        uza.SessionCode = customer.ParentId;
                        db.AgentUsers.Add(uza);
                        db.SaveChanges();

                        AgentUser usa = new AgentUser();
                        usa.UserName = customer.Email;
                        usa.FullName = customer.ContactPerson;
                        usa.CreationDate = DateTime.Now;
                        usa.AccountNumber = customer.AccountCode;
                        usa.TerminalId = 1;
                        usa.Role = "Suppervisor";
                        uza.SessionCode = customer.ParentId;
                        db.AgentUsers.Add(usa);
                        db.SaveChanges();

                        AccountCredenitial accr = new AccountCredenitial();
                        accr.AccountNumber= newAccount;
                        accr.Pin = ePin;
                        accr.Question = question;
                        accr.Answer = Answer;
                        accr.Active = true;
                        accr.Access = "Agent";
                        accr.Connection = conn;
                        db.AccountCredenitials.Add(accr);
                        db.SaveChanges();

                        AccountCredenitial accmr = new AccountCredenitial();
                        accmr.AccountNumber = question;
                        accmr.Pin = ePin;
                        accmr.Question = question;
                        accmr.Answer = Answer;
                        accmr.Active = true;
                        accmr.Access = "System";
                        accmr.Connection = conn;
                        db.AccountCredenitials.Add(accmr);
                        db.SaveChanges();

                        #region create yomoney account
                        try
                        {
                            var acnts = new YomoneyAccount();
                            string[] ssize = customer.ContactPerson.Split(null);
                            String Body = "";
                            Body += "Id=" + customer.AccountCode;
                            Body += "&AccountName=" + customer.SupplierName;
                            Body += "&FirstName=" + ssize[0];
                            if (ssize.Length > 1) Body += ssize[1];
                            Body += "&Email=" + customer.Email;
                            Body += "&Address=" + customer.Address1  + "," + customer.Address2 ;
                            Body += "&MerchantPoints=" + customer.Phone1 ;
                            Body += "&LastAccessFrom=" + Pin;
                            /*acnts.Id = customer.AccountCode;// +item.AccountSufix.Trim();
                            acnts.AccountName = customer.SupplierName;
                            acnts.FirstName = ssize[0];
                            if (ssize.Length > 1) acnts.Surname = ssize[1];
                            acnts.LastAccessFrom   = 0;
                            acnts.MerchantPoints   = 0;
                            acnts.ActualBalance = 0;
                            acnts.CommissionWallet = 0;
                            acnts.TerminalId = 0;
                            acnts.ServiceType = 0;*/
                            String Host = "yomoneyservice.com";
                            JavaScriptSerializer jss = new JavaScriptSerializer();
                            
                            var ContentLength = Body.Length;
                            var pro = db.ProxySettings.FirstOrDefault();
                            HttpWebRequest httpWReq = (HttpWebRequest)WebRequest.Create("https://" + Host + "/api/Vend/create");
                            httpWReq.ContentType = "application/x-www-form-urlencoded";
                            String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("263778100222:44148"));
                            //live
                            // String encoded = System.Convert.ToBase64String(System.Text.Encoding.GetEncoding("ISO-8859-1").GetBytes("600024:a99L3v1N3@2017#"));
                            httpWReq.Headers.Add("Authorization", "Basic " + encoded);
                            httpWReq.Method = "Post";

                            if (pro != null && pro.IpAddress.Trim() != "0.0.0.0")
                            {
                                //"172.18.0.140"
                                WebProxy myproxy = new WebProxy(pro.IpAddress.Trim(), int.Parse(pro.Port));
                                myproxy.BypassProxyOnLocal = true;
                                httpWReq.Proxy = myproxy;
                            }
                            httpWReq.ContentLength = ContentLength;
                            using (var streamWriter = new StreamWriter(httpWReq.GetRequestStream()))
                            {

                                streamWriter.Write(Body, 0, Body.Length);
                            }
                            HttpWebResponse response = (HttpWebResponse)httpWReq.GetResponse();
                            StreamReader sr = new StreamReader(response.GetResponseStream());
                            var returnvalue = sr.ReadToEnd();
                            if (returnvalue.Length >= 4 && returnvalue.Substring(0, 4) == "1701")
                            {
                                returnvalue = "Success";
                            }
                            else
                            {

                                string uu = jss.DeserializeObject(returnvalue).ToString() ;
                                if (uu == "Success")
                                {
                                    returnvalue = uu;
                                }
                                else
                                {
                                    returnvalue = "Error";
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //throw new Exception(ex.Message);
                        }
                        #endregion

                        #region Email
                        try
                        {
                            MailMessage mailMessage = new MailMessage(
                                                 new MailAddress("accounts@yomoneyservice.com"), new MailAddress(customer.Email.ToLower()));

                            var ee = en.encrypt(customer.Email.ToLower().Trim() + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");

                            mailMessage.Subject = "Account Activation";
                            mailMessage.IsBodyHtml = true;
                            mailMessage.Body = "<p>Hello " + customer.ContactPerson + ", </p><p> You have been successfully registered as a Yomoney Agent.</p>" +

                            "Use the following credentials to Signin.<br/><br/> Username = " + unam + "<br/> Password = " + Pin + "<br/> Website = www.yomoney.co.zw </p>" +
                            "<p>For assistance contact support on email addresss support@yomoneyservice.com </p>";


                            System.Net.NetworkCredential networkCredentials = new
                            System.Net.NetworkCredential("accounts@yomoneyservice.com", "Accounts@123");

                            SmtpClient smtpClient = new SmtpClient();
                            smtpClient.EnableSsl = false;
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = networkCredentials;
                            smtpClient.Host = "smtpout.secureserver.net";
                            smtpClient.Port = 25;
                            smtpClient.Send(mailMessage);
                        }
                        catch(Exception e)
                        {

                        }
                        // Send email
                        #endregion
                        return RedirectToAction("Index", new { Company = customer.Company });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sorry this contact's email is already in use");
                    }
                   
                }
                else
                {
                    ModelState.AddModelError("", "Sorry this customer name is already in use");
                }
            }

            return PartialView(customer);
        }
        
        //
        // GET: /Suppliers/Edit/5
 
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Supplier customer = np.GetSuppliers(id);
            return View(customer);
        }

        //
        // POST: /Suppliers/Edit/5

        [HttpPost]
        public ActionResult Edit(Supplier customer)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                np.SaveOrUpdateSuppliers(customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //
        // GET: /Suppliers/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Supplier customer = np.GetSuppliers(id);
            return View(customer);
        }

        //
        // POST: /Suppliers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Supplier customer = np.GetSuppliers(id);
            np.SaveOrUpdateSuppliers(customer);
            
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               // context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}