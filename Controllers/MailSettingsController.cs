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
using System.IO;
using System.Net.Mail;
using System.Text.RegularExpressions;
using System.Web.Script.Serialization;
using Newtonsoft.Json;
using System.Net;
using System.Net.Mime;
using Microsoft.Exchange.WebServices.Data;

using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Principal;
using Microsoft.Win32;
using Microsoft.Win32.SafeHandles;
using System.Collections.Specialized;
using System.Text;
using System.Threading;
using RetailKing.RavendbClasses;

namespace RetailKing.Controllers
{
    [StructLayout(LayoutKind.Sequential)]
    public struct ProfileInfo
    {
        /// 
        /// Specifies the size of the structure, in bytes.
        /// 
        public int dwSize;

        /// 
        /// This member can be one of the following flags: PI_NOUI or PI_APPLYPOLICY
        /// 
        public int dwFlags;

        /// 
        /// Pointer to the name of the user. 
        /// This member is used as the base name of the directory in which to store a new profile. 
        /// 
        public string lpUserName;

        /// 
        /// Pointer to the roaming user profile path. 
        /// If the user does not have a roaming profile, this member can be NULL.
        /// 
        public string lpProfilePath;

        /// 
        /// Pointer to the default user profile path. This member can be NULL. 
        /// 
        public string lpDefaultPath;

        /// 
        /// Pointer to the name of the validating domain controller, in NetBIOS format. 
        /// If this member is NULL, the Windows NT 4.0-style policy will not be applied. 
        /// 
        public string lpServerName;

        /// 
        /// Pointer to the path of the Windows NT 4.0-style policy file. This member can be NULL. 
        /// 
        public string lpPolicyPath;

        /// 
        /// Handle to the HKEY_CURRENT_USER registry key. 
        /// 
        public IntPtr hProfile;
    }

    [Authorize]
    public class MailSettingsController : Controller
    {
        #region impersonation
     
        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_PROVIDER_DEFAULT = 0;

        #region PInvoke
        [DllImport("advapi32.dll")]
        public static extern int LogonUser(String lpszUserName,
            String lpszDomain,
            String lpszPassword,
            int dwLogonType,
            int dwLogonProvider,
            ref IntPtr phToken);

        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern int DuplicateToken(IntPtr hToken, int impersonationLevel, ref IntPtr hNewToken);

        /// <summary>
        /// A process should call the RevertToSelf function after finishing any impersonation begun by using the DdeImpersonateClient, ImpersonateDdeClientWindow, ImpersonateLoggedOnUser, ImpersonateNamedPipeClient, ImpersonateSelf, ImpersonateAnonymousToken or SetThreadToken function.
        /// If RevertToSelf fails, your application continues to run in the context of the client, which is not appropriate. You should shut down the process if RevertToSelf fails.
        /// RevertToSelf Function: http://msdn.microsoft.com/en-us/library/aa379317(VS.85).aspx
        /// </summary>
        /// <returns>A boolean value indicates the function succeeded or not.</returns>
        [DllImport("advapi32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool RevertToSelf();

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern bool CloseHandle(IntPtr handle);

        [DllImport("userenv.dll", SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool LoadUserProfile(IntPtr hToken, ref ProfileInfo lpProfileInfo);

        [DllImport("Userenv.dll", CallingConvention = CallingConvention.Winapi, SetLastError = true, CharSet = CharSet.Auto)]
        public static extern bool UnloadUserProfile(IntPtr hToken, IntPtr lpProfileInfo);

        #endregion

        private static WindowsImpersonationContext m_ImpersonationContext = null;
        #endregion

        private const string SMTP_SERVER = "http://schemas.microsoft.com/cdo/configuration/smtpserver";
        private const string SMTP_SERVER_PORT = "http://schemas.microsoft.com/cdo/configuration/smtpserverport";
        private const string SEND_USING = "http://schemas.microsoft.com/cdo/configuration/sendusing";
        private const string SMTP_USE_SSL = "http://schemas.microsoft.com/cdo/configuration/smtpusessl";
        private const string SMTP_AUTHENTICATE = "http://schemas.microsoft.com/cdo/configuration/smtpauthenticate";
        private const string SEND_USERNAME = "http://schemas.microsoft.com/cdo/configuration/sendusername";
        private const string SEND_PASSWORD = "http://schemas.microsoft.com/cdo/configuration/sendpassword";
        private const string SMTP_HOST = "http://schemas.microsoft.com/cdo/configuration/host";

        public delegate void sendMailDelegate(object sender, FileSystemEventArgs e, MailService m,long c);
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        public static string emailFile;
        XmlSupplier sup = new XmlSupplier();
        XmlTransactionLogs trnl = new XmlTransactionLogs();
        public ActionResult Dashboard()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                ViewData["UserId"] = part[1];
                var user = np.Getlogin(part[1]);
                // var com = np.GetAllCompanies();
                var compId = db.Companies.Where(u => u.name == user.Location).FirstOrDefault();
                var company = np.GetCompanies(part[2]);
                var dat = DateTime.Now.Date.ToString("ddMMyyyy");
                var dd = DateTime.Now.Date;
                NotificationDash dash = new NotificationDash();
                //dash.services = db.MailingServices.ToList();
                var id = part[0] + "_" + DateTime.Now.Month;
                if (part.Length == 4)
                {
                    id = "5-0001-0000000" + "_" + DateTime.Now.Month;
                }
          
                List<Stats> stats = new List<Stats>();
                Stats stat = new Stats();
                List<GraphData> gdatas = new List<GraphData>();
                List<string> keys = new List<string>();
                var date = DateTime.Now.Date.ToString("ddMMyyyy");
                var px = db.DailySales.Find(date + company.ID + "_SALE");
                var ms = db.MonthlySales.Find(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + company.ID);
                var ac = db.Activecoes.FirstOrDefault();
                stat.Amount = (decimal)ac.dailyTarget;
                stat.ServiceName = "DAILY TARGET";
                stats.Add(stat);
                stat.Amount = (decimal)ac.dailyTarget;
                stat.ServiceName = "MONTHLY TARGET";
                stats.Add(stat);
                if (px != null)
                {
                    if (px.Sales == null) px.Sales = 0;
                    stat.Amount = (decimal)px.Sales;
                    stat.ServiceName = "DAILY SALES";  
                }
                else
                { 
                    stat.Amount = 0;
                    stat.ServiceName = "DAILY SALES";   
                }
                stats.Add(stat);
                if (ms != null)
                {
                    if (ms.Sales == null) ms.Sales = 0;
                    stat.Amount = (decimal)ms.Sales;
                    stat.ServiceName = "MONTHLY SALES";
                }
                else

                 
                {
                    stat.Amount = 0;
                    stat.ServiceName = "MONTHLY SALES";
                }
                stats.Add(stat);
                ViewData["Graph"] = "";
                string gr = "";
                if (px != null)
                {   
                    
                    var grx =db.DailySales.Where(u => u.AccountName == "SALE" && u.CompanyId == compId.ID).OrderByDescending(u => u.TransactionDate).Take(7).ToList();
                    var gx = grx.OrderBy(u => u.Id.Substring(0, 2)).Take(7).ToList();
                    ViewData["Graph"] = gr;

                    foreach (var item in gx)
                    {
                        GraphData gd = new GraphData();
                        gd.Period = item.Date;
                        if (item.Sales == null) item.Sales = 0;
                        gd.Sales = (decimal)item.Sales;
                        gdatas.Add(gd);
                    }

                    // var graph = (from e in stats select new { service = e.ServiceName.Trim(), hits = e.Successful}).ToArray();
                    var rx = gdatas.ToList();

                    var x = JsonConvert.SerializeObject(rx);
                    ViewBag.data = x;
                    dash.stats = stats.Where(u => u.Date == dd).OrderBy(u => u.Date).Take(5).ToList();
                }
              
                return View("Dashboard", "_AdminLayout", dash);
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        #region Mail Settings
        public ActionResult Index()
        {
            return PartialView(db.MailSettings.ToList());
        }

        public ActionResult ServiceList()
        {
            return PartialView(db.MailingServices.ToList());
        }

        public ActionResult Details(long id = 0)
        {
            var city = db.MailSettings.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }

        public ActionResult Create()
        {
            var countries = db.Companies.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
          
            foreach (var item in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.name,
                    Value = item.name.ToString()
                });
            }

            ViewBag.Company = yr;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(MailSetting city)
        {
            if (ModelState.IsValid)
            {
                db.MailSettings.Add(city);
                db.SaveChanges();
                #region logs
                var currentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                var amsg = "Mail setting for: " + city.Domain .Trim() + " :created on: " + DateTime.Now + ": by user :" + parta[0];

                if (!System.IO.File.Exists(patha))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(patha))
                    {
                        sw.WriteLine(amsg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                }
                #endregion
                return RedirectToAction("Index");
            }

            return PartialView(city);
        }

        public ActionResult Edit(long id = 0)
        {
            var countries = db.Companies.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.name,
                    Value = item.name.ToString()
                });
            }

            ViewBag.Company = yr;
            var city = db.MailSettings.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(MailSetting city)
        {
            if (ModelState.IsValid)
            {
                city.UseDefaultCredentials = false;
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                #region logs
                var currentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                var amsg = "Mail Setting: " + city.Domain.Trim() + " :edited on: " + DateTime.Now + ": by user :" + parta[0];

                if (!System.IO.File.Exists(patha))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(patha))
                    {
                        sw.WriteLine(amsg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                }
                #endregion
                return RedirectToAction("Index");
            }
            var countries = db.Companies.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.name,
                    Value = item.name.ToString()
                });
            }

            ViewBag.Company = yr;
            return PartialView(city);
        }

        public ActionResult Delete(long id = 0)
        {
            var city = db.Syskeys.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            var city = db.Syskeys.Find(id);
            db.Syskeys.Remove(city);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region Template
        public ActionResult MailTemplate()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usa = long.Parse(partz[3]);
            var px = db.MailTemplates.Where(u => u.UserId == usa).ToList();
            if (Request.IsAjaxRequest()) return PartialView(px);
            return View("MailTemplate", "_AdminLayout", px);
        }

        public ActionResult CreateMailTemplate()
        {
            MailTemplate tmp = new MailTemplate();
            return PartialView(tmp);
        }
    
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateMailTemplate(MailTemplate city)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                var currentUser = User.Identity.Name;
                
                char[] delimiter = new char[] { '~' };
                string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var usa = db.AgentUsers.Find(long.Parse(partz[0]));
                city.UserId = long.Parse(partz[3]);
                if(city.FixedEmail == true)
                {
                   
                    city.EmailAddress = usa.UserName;
                  //city.Password = usa.EmailPw;
                }
                else
                {
                    city.Password = city.NewPassword;
                }
                db.MailTemplates.Add(city);
                db.SaveChanges();
                #region logs
                 currentUser = User.Identity.Name;
                char[] delimitar = new char[] { '~' };
                string[] parta = currentUser.Split(delimitar, StringSplitOptions.RemoveEmptyEntries);

                string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                var amsg = "Template: " + city.Subject.Trim() + " :created on: " + DateTime.Now + ": by user :" + parta[0];

                if (!System.IO.File.Exists(patha))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(patha))
                    {
                        sw.WriteLine(amsg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                }
                #endregion
                return RedirectToAction("MailTemplate");
            }

            return PartialView(city);
        }

        public ActionResult EditMailTemplate(long id = 0)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var countries = db.Customers.ToList();//Where(u => u.CompanyId == partz[1]).ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            ST_encrypt en = new ST_encrypt();
            ST_decrypt dec = new ST_decrypt();
            foreach (var item in countries)
            {

                item.Email = dec.st_decrypt(item.Email.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
            }

            ViewBag.Customer = countries;
            var city = db.MailTemplates.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditMailTemplate(MailTemplate city)
        {
            if (ModelState.IsValid)
            {
                if( !string.IsNullOrEmpty(city.NewPassword))
                {
                     city.Password = city.NewPassword;
                }
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                #region logs
                var currentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                var amsg = "Template:" + city.Subject.Trim() + " :edited on: " + DateTime.Now + ": by user :" + parta[0];

                if (!System.IO.File.Exists(patha))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(patha))
                    {
                        sw.WriteLine(amsg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                }
                #endregion
                return RedirectToAction("MailTemplate");
            }
            return PartialView(city);
        }

        public ActionResult TemplateDetail(long id = 0)
        {
            var city = db.MailTemplates.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }

        public ActionResult DeleteMailTemplate(long id = 0)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
           
            var city = db.MailTemplates.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }
        [HttpPost]
        public ActionResult DeleteMailTemplate(MailTemplate mt)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
 
            var city = db.MailTemplates.Find(mt.Id);
            
            if (city == null)
            {
                return HttpNotFound();
            }
            db.MailTemplates.Remove(city);
            db.SaveChanges();
            #region logs
             currentUser = User.Identity.Name;
            char[] delimita = new char[] { '~' };
            string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

            string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
            var amsg = "Template: " + city.Subject.Trim() + " :deleted on: " + DateTime.Now + ": by user :" + parta[0];

            if (!System.IO.File.Exists(patha))
            {
                // Create a file to write to. 
                using (StreamWriter sw = System.IO.File.CreateText(patha))
                {
                    sw.WriteLine(amsg);
                }
            }
            else
            {
                System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
            }
            #endregion
            return RedirectToAction("MailTemplate");
        }


        #endregion

        #region MailGroups
        public ActionResult MailGroups()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            //var usa = np.Getlogin(partz[0]).ID;

            var px = db.MailGroups.ToList();
            return PartialView(px);
        }

        public ActionResult CreateMailGroups()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var countries = db.Customers.ToList();//Where(u => u.CompanyId == partz[1]).ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            ST_encrypt en = new ST_encrypt();
            ST_decrypt dec = new ST_decrypt();
            foreach (var item in countries)
            {

                item.Email = dec.st_decrypt(item.Email.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
            }

            ViewBag.Customer = countries;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMailGroups(MailGroup  city)
        {
            if (ModelState.IsValid)
            {
                db.MailGroups.Add(city);
                db.SaveChanges();
                #region logs
                var currentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                var amsg = "Mail group for :" + city.Receiver.Trim() + " :created on: " + DateTime.Now + ": by user :" + parta[0];

                if (!System.IO.File.Exists(patha))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(patha))
                    {
                        sw.WriteLine(amsg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                }
                #endregion
                return RedirectToAction("MailGroups");
            }
            return PartialView(city);
        }

        public ActionResult EditMailGroups(long Id)
        {
            var city = db.MailGroups.Find(Id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMailGroups(MailGroup city)
        {
            if (ModelState.IsValid)
            {
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                #region logs
                var currentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                var amsg = "Mail group for :" + city.Receiver.Trim() + " :edited on: " + DateTime.Now + ": by user :" + parta[0];

                if (!System.IO.File.Exists(patha))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(patha))
                    {
                        sw.WriteLine(amsg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                }
                #endregion
                return RedirectToAction("MailGroups");
            }
            return PartialView(city);
        }

        public ActionResult DeleteMailGroups(long id = 0)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
           
            var city = db.MailGroups.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }
        [HttpPost]
        public ActionResult DeleteMailGroups(MailGroup mt)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            var city = db.MailGroups.Find(mt.Id);

            if (city == null)
            {
                return HttpNotFound();
            }
            db.MailGroups.Remove(city);
            db.SaveChanges();
            #region logs
             currentUser = User.Identity.Name;
            char[] delimita = new char[] { '~' };
            string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

            string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
            var amsg = "Mail group for:" + city.Receiver.Trim() + " :deleted on: " + DateTime.Now + ": by user :" + parta[0];

            if (!System.IO.File.Exists(patha))
            {
                // Create a file to write to. 
                using (StreamWriter sw = System.IO.File.CreateText(patha))
                {
                    sw.WriteLine(amsg);
                }
            }
            else
            {
                System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
            }
            #endregion
            return RedirectToAction("MailGroups");
        }


        #endregion

        #region MailingList
        public ActionResult MailingList()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usa = long.Parse(partz[3]);
            var px = db.MailLists.Where(u => u.UserId == usa).ToList();
            return PartialView(px);
        }

        public ActionResult SelectMailing()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usa =long.Parse(partz[3]);

            var px = db.MailLists.Where(u => u.UserId == usa).ToList();
            return PartialView(px);
        }

        public ActionResult CreateMailingList()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string supplierId = partz[0];
            var supplier = db.Suppliers.Where(u => u.AccountCode == supplierId).FirstOrDefault();
            if(supplier.ParentId  != null )
            {
                supplierId = supplier.ParentId.Trim();
            }
            var suplierCus = sup.GetAllLoyalCustomers(supplierId);//db.Customers.ToList();//Where(u => u.CompanyId == partz[1]).ToList();
            var mem = suplierCus.members.Where(u => u.Branch == supplier.Branch).ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            ST_encrypt en = new ST_encrypt();
            ST_decrypt dec = new ST_decrypt();
            ViewBag.Customer = mem;

            var serv = db.MailingServices.ToList();
            List<SelectListItem> sr = new List<SelectListItem>();
            sr.Add(new SelectListItem
            {
                Text = "Select Service",
                Value = "0"
            });
            foreach (var item in serv)
            {
                sr.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            ViewBag.Services = sr;
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMailingList(MailList city)
        {
            if (ModelState.IsValid)
            { 
                if(city.Name == null)
                {
                    return PartialView(city);
                }
                if (city.EmailList == null)
                {
                    return PartialView(city);
                }
                NHibernateDataProvider np = new NHibernateDataProvider();
                var currentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var usa = np.Getlogin(partz[0]).ID;

                city.UserId = usa;
                db.MailLists.Add(city);
                db.SaveChanges();
                #region logs
                currentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                var amsg = "Mailing List: " + city.Name.Trim() + " :created on: " + DateTime.Now + ": by user :" + parta[0];

                if (!System.IO.File.Exists(patha))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(patha))
                    {
                        sw.WriteLine(amsg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                }
                #endregion
                return RedirectToAction("MailingList");
            }
            return PartialView(city);
        }
        
        public ActionResult EditMailingList(long id = 0)
        {
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var countries = db.Customers.ToList();//Where(u => u.CompanyId == partz[1]).ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            ST_encrypt en = new ST_encrypt();
            ST_decrypt dec = new ST_decrypt();
            foreach (var item in countries)
            {
                item.Email = dec.st_decrypt(item.Email.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
            }

            ViewBag.Customer = countries;

            var serv = db.MailingServices.ToList();
            List<SelectListItem> sr = new List<SelectListItem>();
            sr.Add(new SelectListItem
            {
                Text = "Select Service",
                Value = "0"
            });
            foreach (var item in serv)
            {
                sr.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            ViewBag.Services = sr;
            var city = db.MailLists.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMailingList(MailList city)
        {
            if (ModelState.IsValid)
            {
                if (city.Name == null)
                {
                    return PartialView(city);
                }
                if (city.EmailList == null)
                {
                    return PartialView(city);
                }
                NHibernateDataProvider np = new NHibernateDataProvider();
                var currentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var usa = np.Getlogin(partz[0]).ID;

                city.UserId = usa;
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                #region logs
                 currentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                var amsg = "Mailing List:" + city.Name.Trim() + " :edited on: " + DateTime.Now + ": by user :" + parta[0];

                if (!System.IO.File.Exists(patha))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(patha))
                    {
                        sw.WriteLine(amsg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                }
                #endregion
                return RedirectToAction("MailingList");
            }
            return PartialView(city);
        }

        public ActionResult DeleteMailingList(long id = 0)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            var city = db.MailLists.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }
        [HttpPost]
        public ActionResult DeleteMailingList(MailList mt)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            var city = db.MailLists.Find(mt.Id);

            if (city == null)
            {
                return HttpNotFound();
            }
            db.MailLists.Remove(city);
            db.SaveChanges();
            #region logs
            currentUser = User.Identity.Name;
            char[] delimita = new char[] { '~' };
            string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

            string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
            var amsg = "Mailing list: " + city.Name.Trim() + " :deleted on: " + DateTime.Now + ": by user :" + parta[0];

            if (!System.IO.File.Exists(patha))
            {
                // Create a file to write to. 
                using (StreamWriter sw = System.IO.File.CreateText(patha))
                {
                    sw.WriteLine(amsg);
                }
            }
            else
            {
                System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
            }
            #endregion
            return RedirectToAction("MailingList");
        }

        #endregion

        #region MailingService
        public ActionResult MailingServices()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            
            var px = db.MailingServices.ToList();
            return PartialView(px);
        }

        public ActionResult CreateMailingService()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var countries = db.Companies.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.name,
                    Value = item.name.ToString()
                });
            }

            ViewBag.Company = yr;

            MailingService ms = new MailingService();
            return PartialView(ms);
        }

        public ActionResult ServiceDetails(long Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var ms = db.MailingServices.Find(Id);
          
            return PartialView(ms);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateMailingService(MailingService city)
        {
            if (ModelState.IsValid)
            {
                city.Status = "Stopped";
                city.HasMailingList = "No";
                db.MailingServices.Add(city);
                db.SaveChanges();
                #region logs
                var currentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                var amsg = "Service: " + city.Name.Trim() + " :created on: " + DateTime.Now + ": by user :" + parta[0];

                if (!System.IO.File.Exists(patha))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(patha))
                    {
                        sw.WriteLine(amsg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                }
                #endregion
                return RedirectToAction("MailingServices");
            }
            var countries = db.Companies.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.name,
                    Value = item.name.ToString()
                });
            }

            ViewBag.Company = yr;
            return PartialView(city);
        }
        
        public ActionResult EditMailingService(long id = 0)
        {
            var countries = db.Companies.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.name,
                    Value = item.name.ToString()
                });
            }

            ViewBag.Company = yr;
            var city = db.MailingServices.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditMailingService(MailingService city)
        {
            if (ModelState.IsValid)
            {
                city.ListiningFolder = city.ListiningFolder.Trim();
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                #region logs
                var currentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                var amsg = "Service: " + city.Name.Trim() + " :edited on: " + DateTime.Now + ": by user :" + parta[0];

                if (!System.IO.File.Exists(patha))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(patha))
                    {
                        sw.WriteLine(amsg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                }
                #endregion
                return RedirectToAction("MailingServices");
            }
            return PartialView(city);
        }

        public ActionResult EditDomainUser()
        {
            ViewData["Success"] = "";
            var du = db.DomainUsers.FirstOrDefault();
            return PartialView(du);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditDomainUser(DomainUser city)
        {
            if (ModelState.IsValid)
            {

                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                #region logs
                var currentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                var amsg = "Domain User:" + city.Name.Trim() + " :edited on: " + DateTime.Now + ": by user :" + parta[0];

                if (!System.IO.File.Exists(patha))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(patha))
                    {
                        sw.WriteLine(amsg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                }
                #endregion
                ViewData["Success"] = "Success";
                return PartialView(city);
            }
            ViewData["Success"] = "Error";
            return PartialView(city);
        }

        public ActionResult StartService(long Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var city = db.MailingServices.Find(Id);
            #region logs
            var currentUser = User.Identity.Name;
            char[] delimita = new char[] { '~' };
            string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

            string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
            var amsg = "Service:" + city.Name.Trim() + " :started on: " + DateTime.Now + ": by user :" + parta[0];

            if (!System.IO.File.Exists(patha))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(patha))
                        {
                            sw.WriteLine(amsg);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
                    }
                    #endregion
            
                        char[] delimiter = new char[] { '\\' };
                        string[] partz = city.ListiningFolder.Trim().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                        char[] delir = new char[] { ':' };
                        string[] pat = partz[0].Split(delir, StringSplitOptions.RemoveEmptyEntries);

                        sendMailDelegate mymethod = new sendMailDelegate(InvokeMethod);
                        string directory = "";

                        if (partz.Length > 1)
                        {
                            var cnt = 0;
                            foreach (var itm in partz)
                            {
                                if (cnt == 0)
                                {
                                    if (pat[0].Length == 1)
                                    {
                                        directory = @"" + partz[cnt];
                                    }
                                    else
                                    {
                                        directory = @"\\" + partz[cnt];
                                    }
                                }
                                else
                                {
                                    directory = directory + @"\" + partz[cnt];
                                }
                                cnt += 1;
                            }
                        }
                        else
                        {
                            if (pat.Length == 2)
                            {
                                directory = @"" + partz[0];
                            }
                            else
                            {
                                directory = @"\\" + partz[0];
                            }
                        }
            try 
            { 
     #region Impersonate
                     WindowsIdentity m_ImpersonatedUser;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;
            const int SecurityImpersonation = 2;
            const int TokenType = 1;

            try
            {
                if (RevertToSelf())
                {
                   
                    String userName = "TempUser";

                    var du =db.DomainUsers.FirstOrDefault();
                    IntPtr password = GetPassword(du.Password.Trim());
                    IntPtr logonToken = LogonUser();
                    IntPtrConstructor(logonToken);
                    IntPtrStringConstructor(logonToken);
                    IntPtrStringTypeConstructor(logonToken);
                    IntPrtStringTypeBoolConstructor(logonToken);

                   // Property implementations.
                   UseProperties(logonToken);

                   // Method implementations.
                  
                   ImpersonateIdentity(logonToken);
                   var uu = ("Before impersonation: " +
                                    WindowsIdentity.GetCurrent().Name);
                   #region logs
                   string path = @"C:\YoLog\" +"Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                   if (!System.IO.File.Exists(path))
                   {
                       // Create a file to write to. 
                       using (StreamWriter sw = System.IO.File.CreateText(path))
                       {
                           sw.WriteLine(uu);
                       }
                   }
                   else
                   {
                       System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                   }
                   #endregion
                    var lon = LogonUser(du.Name.Trim(), du.Domain.Trim(), du.Password.Trim(), LOGON32_LOGON_INTERACTIVE,
                                  LOGON32_PROVIDER_DEFAULT, ref token);
                    if ( lon != 0)
                    {
                        if (DuplicateToken(token, SecurityImpersonation, ref tokenDuplicate) != 0)
                        {
                            m_ImpersonatedUser = new WindowsIdentity(tokenDuplicate);
                            using (m_ImpersonationContext = m_ImpersonatedUser.Impersonate())
                            {
                                if (m_ImpersonationContext != null)
                                {
                                    uu = ("After Impersonation succeeded: " + Environment.NewLine +
                                                      "User Name: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).Name +
                                                      Environment.NewLine +
                                                      "SID: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).User.
                                                          Value);
                                    #region logs
                                   // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                    if (!System.IO.File.Exists(path))
                                    {
                                        // Create a file to write to. 
                                        using (StreamWriter sw = System.IO.File.CreateText(path))
                                        {
                                            sw.WriteLine(uu);
                                        }
                                    }
                                    else
                                    {
                                        System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                    }
                                    #endregion

                                    #region LoadUserProfile
                                    // Load user profile
                                    ProfileInfo profileInfo = new ProfileInfo();
                                    profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                                    profileInfo.lpUserName = userName;
                                    profileInfo.dwFlags = 1;
                                    Boolean loadSuccess = LoadUserProfile(tokenDuplicate, ref profileInfo);

                                    if (!loadSuccess)
                                    {
                                        uu =("LoadUserProfile() failed with error code: " +
                                                          Marshal.GetLastWin32Error());
                                        throw new Win32Exception(Marshal.GetLastWin32Error());
                                        #region logs
                                        //string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                        if (!System.IO.File.Exists(path))
                                        {
                                            // Create a file to write to. 
                                            using (StreamWriter sw = System.IO.File.CreateText(path))
                                            {
                                                sw.WriteLine(uu);
                                            }
                                        }
                                        else
                                        {
                                            System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                        }
                                        #endregion
                                    }

                                    if (profileInfo.hProfile == IntPtr.Zero)
                                    {
                                       uu = (
                                            "LoadUserProfile() failed - HKCU handle was not loaded. Error code: " +
                                            Marshal.GetLastWin32Error());
                                       #region logs
                                      // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                       if (!System.IO.File.Exists(path))
                                       {
                                           // Create a file to write to. 
                                           using (StreamWriter sw = System.IO.File.CreateText(path))
                                           {
                                               sw.WriteLine(uu);
                                           }
                                       }
                                       else
                                       {
                                           System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                       }
                                       #endregion
                                    }
                                    #endregion

                                    CloseHandle(token);
                                    CloseHandle(tokenDuplicate);

                        #endregion
                 MailService ms = new MailService(directory, city.FileFilter.Trim(),city.HasMailingList.Trim() ,city.Id, mymethod);
                 ms.StartWatch();
                 city.Status = "Running";
                 db.Entry(city).State = EntityState.Modified;
                 db.SaveChanges();
      #region EndImpression
                                    //  UnloadUserProfile(tokenDuplicate, profileInfo.hProfile);

                                    // Undo impersonation
                                    //  m_ImpersonationContext.Undo();
                                }
                            }
                        }
                        else
                        {
                            uu = ("DuplicateToken() failed with error code: " + Marshal.GetLastWin32Error());
                            #region logs
                            // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                            if (!System.IO.File.Exists(path))
                            {
                                // Create a file to write to. 
                                using (StreamWriter sw = System.IO.File.CreateText(path))
                                {
                                    sw.WriteLine(uu);
                                }
                            }
                            else
                            {
                                System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                            }
                            #endregion
                            //throw new Win32Exception(Marshal.GetLastWin32Error());

                        }
                    }
                }
            }
            catch (Win32Exception we)
            {
                city.Status = "Stopped";
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                //throw we;
            }
            catch
            {
                city.Status = "Stopped";
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                // throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                if (token != IntPtr.Zero) CloseHandle(token);
                if (tokenDuplicate != IntPtr.Zero) CloseHandle(tokenDuplicate);

                string uu = ("After finished impersonation: " + WindowsIdentity.GetCurrent().Name);
                #region logs
                string path = @"C:\YoLog\" + "Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";


                if (!System.IO.File.Exists(path))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(path))
                    {
                        sw.WriteLine(uu);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                }
                #endregion
            }
                                    #endregion              
            }
            catch (Exception ex)
            {
                city.Status = "Stopped";
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                var msg = "Service:" + city.Name.Trim() + " : on: " + DateTime.Now + ": Failed with error :" + ex.Message;

                #region stats
                var dd = DateTime.Now.Date.ToString("ddMMyyyy");
                var sId = dd + "_" + city.Id;
                var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                if (stat != null)
                {
                    stat.Errors += 1;
                    stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                    db.Entry(stat).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    stat = new NotificationStat();
                    stat.ServiceId = city.Id;
                    stat.Id = sId;
                    stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                    stat.Errors = 1;
                    db.Entry(stat).State = EntityState.Added;
                    db.SaveChanges();
                }
                #endregion

                #region logs
                string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                if (!System.IO.File.Exists(path))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(path))
                    {
                        sw.WriteLine(msg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(path, msg + Environment.NewLine);
                }
                #endregion
            }
            return RedirectToAction("MailingServices");
          
        }

        public ActionResult StopService(long Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
        
            var city = db.MailingServices.Find(Id);
            #region logs
            var currentUser = User.Identity.Name;
            char[] delimita = new char[] { '~' };
            string[] parta = currentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

            string patha = @"C:\YoLog\Audit" + DateTime.Now.ToString("ddMMyyy") + ".txt";
            var amsg = "Service:" + city.Name.Trim() + " :started on: " + DateTime.Now + ": by user :" + parta[0];

            if (!System.IO.File.Exists(patha))
            {
                // Create a file to write to. 
                using (StreamWriter sw = System.IO.File.CreateText(patha))
                {
                    sw.WriteLine(amsg);
                }
            }
            else
            {
                System.IO.File.AppendAllText(patha, amsg + Environment.NewLine);
            }
            #endregion
            char[] delimiter = new char[] { '/' };
            string[] partz = city.ListiningFolder.Trim().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            char[] delir = new char[] { ':' };
            string[] pat = partz[0].Split(delir, StringSplitOptions.RemoveEmptyEntries);

            sendMailDelegate mymethod = new sendMailDelegate(InvokeMethod);
            string directory = "";
            if (partz.Length > 1)
            {
                var cnt = 0;
                foreach (var itm in partz)
                {
                    if (cnt == 0)
                    {
                        if (pat[0].Length == 1)
                        {
                            directory = @"" + partz[cnt];
                        }
                        else
                        {
                            directory = @"\\" + partz[cnt];
                        }
                    }
                    else
                    {
                        directory = directory + @"\" + partz[cnt];
                    }
                    cnt += 1;
                }
            }
            else
            {
                if (pat.Length == 2)
                {
                    directory = @"" + partz[0];
                }
                else
                {
                    directory = @"\\" + partz[0];
                }
            }
       #region Impersonate
                     WindowsIdentity m_ImpersonatedUser;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;
            const int SecurityImpersonation = 2;
            const int TokenType = 1;

            try
            {
                if (RevertToSelf())
                {
                   
                    String userName = "TempUser";

                    var du =db.DomainUsers.FirstOrDefault();
                    IntPtr password = GetPassword(du.Password.Trim());
                    IntPtr logonToken = LogonUser();
                    IntPtrConstructor(logonToken);
                    IntPtrStringConstructor(logonToken);
                    IntPtrStringTypeConstructor(logonToken);
                    IntPrtStringTypeBoolConstructor(logonToken);

                   // Property implementations.
                   UseProperties(logonToken);

                   // Method implementations.
                  
                   ImpersonateIdentity(logonToken);
                   var uu = ("Before impersonation: " +
                                    WindowsIdentity.GetCurrent().Name);
                   #region logs
                   string path = @"C:\YoLog\" +"Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                   if (!System.IO.File.Exists(path))
                   {
                       // Create a file to write to. 
                       using (StreamWriter sw = System.IO.File.CreateText(path))
                       {
                           sw.WriteLine(uu);
                       }
                   }
                   else
                   {
                       System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                   }
                   #endregion
                    var lon = LogonUser(du.Name.Trim(), du.Domain.Trim(), du.Password.Trim(), LOGON32_LOGON_INTERACTIVE,
                                  LOGON32_PROVIDER_DEFAULT, ref token);
                    if ( lon != 0)
                    {
                        if (DuplicateToken(token, SecurityImpersonation, ref tokenDuplicate) != 0)
                        {
                            m_ImpersonatedUser = new WindowsIdentity(tokenDuplicate);
                            using (m_ImpersonationContext = m_ImpersonatedUser.Impersonate())
                            {
                                if (m_ImpersonationContext != null)
                                {
                                    uu = ("After Impersonation succeeded: " + Environment.NewLine +
                                                      "User Name: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).Name +
                                                      Environment.NewLine +
                                                      "SID: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).User.
                                                          Value);
                                    #region logs
                                   // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                    if (!System.IO.File.Exists(path))
                                    {
                                        // Create a file to write to. 
                                        using (StreamWriter sw = System.IO.File.CreateText(path))
                                        {
                                            sw.WriteLine(uu);
                                        }
                                    }
                                    else
                                    {
                                        System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                    }
                                    #endregion
                                    #region LoadUserProfile
                                    // Load user profile
                                    ProfileInfo profileInfo = new ProfileInfo();
                                    profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                                    profileInfo.lpUserName = userName;
                                    profileInfo.dwFlags = 1;
                                    Boolean loadSuccess = LoadUserProfile(tokenDuplicate, ref profileInfo);

                                    if (!loadSuccess)
                                    {
                                        uu =("LoadUserProfile() failed with error code: " +
                                                          Marshal.GetLastWin32Error());
                                       // throw new Win32Exception(Marshal.GetLastWin32Error());
                                        #region logs
                                        //string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                        if (!System.IO.File.Exists(path))
                                        {
                                            // Create a file to write to. 
                                            using (StreamWriter sw = System.IO.File.CreateText(path))
                                            {
                                                sw.WriteLine(uu);
                                            }
                                        }
                                        else
                                        {
                                            System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                        }
                                        #endregion
                                    }

                                    if (profileInfo.hProfile == IntPtr.Zero)
                                    {
                                       uu = (
                                            "LoadUserProfile() failed - HKCU handle was not loaded. Error code: " +
                                            Marshal.GetLastWin32Error());
                                       #region logs
                                      // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                       if (!System.IO.File.Exists(path))
                                       {
                                           // Create a file to write to. 
                                           using (StreamWriter sw = System.IO.File.CreateText(path))
                                           {
                                               sw.WriteLine(uu);
                                           }
                                       }
                                       else
                                       {
                                           System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                       }
                                       #endregion
                                    }
                                    #endregion

                                    CloseHandle(token);
                                    CloseHandle(tokenDuplicate);

                        #endregion    
            MailService ms = new MailService(directory, city.FileFilter.Trim(), city.HasMailingList.Trim(), city.Id, mymethod);
            ms.StopWatch();
            city.Status = "Stopped";
            db.Entry(city).State = EntityState.Modified;
            db.SaveChanges();
         #region EndImpression
                                    //  UnloadUserProfile(tokenDuplicate, profileInfo.hProfile);

                                    // Undo impersonation
                                    //  m_ImpersonationContext.Undo();
                                }
                            }
                        }
                        else
                        {
                            uu = ("DuplicateToken() failed with error code: " + Marshal.GetLastWin32Error());
                            #region logs
                            // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                            if (!System.IO.File.Exists(path))
                            {
                                // Create a file to write to. 
                                using (StreamWriter sw = System.IO.File.CreateText(path))
                                {
                                    sw.WriteLine(uu);
                                }
                            }
                            else
                            {
                                System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                            }
                            #endregion
                            //throw new Win32Exception(Marshal.GetLastWin32Error());

                        }
                    }
                }
            }
            catch (Win32Exception we)
            {
                city.Status = "Stopped";
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                //throw we;
            }
            catch
            {
                city.Status = "Stopped";
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                // throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
                if (token != IntPtr.Zero) CloseHandle(token);
                if (tokenDuplicate != IntPtr.Zero) CloseHandle(tokenDuplicate);

                string uu = ("After finished impersonation: " + WindowsIdentity.GetCurrent().Name);
                #region logs
                string path = @"C:\YoLog\" + "Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";


                if (!System.IO.File.Exists(path))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(path))
                    {
                        sw.WriteLine(uu);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                }
                #endregion
            }
                                    #endregion              
            return RedirectToAction("MailingServices");
        }

        public ActionResult StartAllServices()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var citites = db.MailingServices.ToList();
            var dt = DateTime.Now.AddMinutes(-2);
            var ss = db.Emails.Where(u => u.Date <= dt).ToList();
            if (ss.Count() > 0)
            {
                foreach (var itm in ss)
                {
                    db.Emails.Remove(itm);
                }
                db.SaveChanges();
            }
            
            foreach (var city in citites)         
            {
                try
                {
                        char[] delimiter = new char[] { '\\' };
                        string[] partz = city.ListiningFolder.Trim().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                        char[] delir = new char[] { ':' };
                        string[] pat = partz[0].Split(delir, StringSplitOptions.RemoveEmptyEntries);

                        sendMailDelegate mymethod = new sendMailDelegate(InvokeMethod);
                        string directory = "";

                        if (partz.Length > 1)
                        {
                            var cnt = 0;
                            foreach (var itm in partz)
                            {
                                if (cnt == 0)
                                {
                                    if (pat[0].Length == 1)
                                    {
                                        directory = @"" + partz[cnt];
                                    }
                                    else
                                    {
                                        directory = @"\\" + partz[cnt];
                                    }
                                }
                                else
                                {
                                    directory = directory + @"\" + partz[cnt];
                                }
                                cnt += 1;
                            }
                        }
                        else
                        {
                            if (pat.Length == 2)
                            {
                                directory = @"" + partz[0];
                            }
                            else
                            {
                                directory = @"\\" + partz[0];
                            }
                        }
                    try
                    {

                        #region Impersonate
                        WindowsIdentity m_ImpersonatedUser;
                        IntPtr token = IntPtr.Zero;
                        IntPtr tokenDuplicate = IntPtr.Zero;
                        const int SecurityImpersonation = 2;
                        const int TokenType = 1;

                        if (RevertToSelf())
                        {
                   
                    String userName = "TempUser";

                    var du =db.DomainUsers.FirstOrDefault();
                    IntPtr password = GetPassword(du.Password.Trim());
                    IntPtr logonToken = LogonUser();
                    IntPtrConstructor(logonToken);
                    IntPtrStringConstructor(logonToken);
                    IntPtrStringTypeConstructor(logonToken);
                    IntPrtStringTypeBoolConstructor(logonToken);

                   // Property implementations.
                   UseProperties(logonToken);

                   // Method implementations.
                  
                   ImpersonateIdentity(logonToken);
                   var uu = ("Before impersonation: " +
                                    WindowsIdentity.GetCurrent().Name);
                   #region logs
                   string path = @"C:\YoLog\" +"Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                   if (!System.IO.File.Exists(path))
                   {
                       // Create a file to write to. 
                       using (StreamWriter sw = System.IO.File.CreateText(path))
                       {
                           sw.WriteLine(uu);
                       }
                   }
                   else
                   {
                       System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                   }
                   #endregion
                    var lon = LogonUser(du.Name.Trim(), du.Domain.Trim(), du.Password.Trim(), LOGON32_LOGON_INTERACTIVE,
                                  LOGON32_PROVIDER_DEFAULT, ref token);
                    if ( lon != 0)
                    {
                        if (DuplicateToken(token, SecurityImpersonation, ref tokenDuplicate) != 0)
                        {
                            m_ImpersonatedUser = new WindowsIdentity(tokenDuplicate);
                            using (m_ImpersonationContext = m_ImpersonatedUser.Impersonate())
                            {
                                if (m_ImpersonationContext != null)
                                {
                                    uu = ("After Impersonation succeeded: " + Environment.NewLine +
                                                      "User Name: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).Name +
                                                      Environment.NewLine +
                                                      "SID: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).User.
                                                          Value);
                                    #region logs
                                   // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                    if (!System.IO.File.Exists(path))
                                    {
                                        // Create a file to write to. 
                                        using (StreamWriter sw = System.IO.File.CreateText(path))
                                        {
                                            sw.WriteLine(uu);
                                        }
                                    }
                                    else
                                    {
                                        System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                    }
                                    #endregion
                                    #region LoadUserProfile
                                    // Load user profile
                                    ProfileInfo profileInfo = new ProfileInfo();
                                    profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                                    profileInfo.lpUserName = userName;
                                    profileInfo.dwFlags = 1;
                                    Boolean loadSuccess = LoadUserProfile(tokenDuplicate, ref profileInfo);

                                    if (!loadSuccess)
                                    {
                                        uu =("LoadUserProfile() failed with error code: " +
                                                          Marshal.GetLastWin32Error());
                                        //throw new Win32Exception(Marshal.GetLastWin32Error());
                                        #region logs
                                        //string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                        if (!System.IO.File.Exists(path))
                                        {
                                            // Create a file to write to. 
                                            using (StreamWriter sw = System.IO.File.CreateText(path))
                                            {
                                                sw.WriteLine(uu);
                                            }
                                        }
                                        else
                                        {
                                            System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                        }
                                        #endregion
                                    }

                                    if (profileInfo.hProfile == IntPtr.Zero)
                                    {
                                       uu = (
                                            "LoadUserProfile() failed - HKCU handle was not loaded. Error code: " +
                                            Marshal.GetLastWin32Error());
                                       #region logs
                                      // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                       if (!System.IO.File.Exists(path))
                                       {
                                           // Create a file to write to. 
                                           using (StreamWriter sw = System.IO.File.CreateText(path))
                                           {
                                               sw.WriteLine(uu);
                                           }
                                       }
                                       else
                                       {
                                           System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                       }
                                       #endregion
                                    }
                                    #endregion

                                 //   CloseHandle(token);
                                   // CloseHandle(tokenDuplicate);

                        #endregion
                        MailService ms = new MailService(directory, city.FileFilter.Trim(), city.HasMailingList.Trim(), city.Id, mymethod);
                        ms.StopWatch();
                      

                        if (city.StartMode.Trim() == "Automatic")
                        {
                            CleanService(city.Id);
                            ms.StartWatch();
                            city.Status = "Running";
                            db.Entry(city).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        else
                        {
                            city.Status = "Stopped";
                            db.Entry(city).State = EntityState.Modified;
                            db.SaveChanges();
                        }
               #region EndImpression
                                    //  UnloadUserProfile(tokenDuplicate, profileInfo.hProfile);

                                    // Undo impersonation
                                    //  m_ImpersonationContext.Undo();
                                }
                            }
                        }
                        else
                        {
                            uu = ("DuplicateToken() failed with error code: " + Marshal.GetLastWin32Error());
                            #region logs
                            // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                            if (!System.IO.File.Exists(path))
                            {
                                // Create a file to write to. 
                                using (StreamWriter sw = System.IO.File.CreateText(path))
                                {
                                    sw.WriteLine(uu);
                                }
                            }
                            else
                            {
                                System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                            }
                            #endregion
                            //throw new Win32Exception(Marshal.GetLastWin32Error());

                        }
                    }
                }
            }
            catch (Win32Exception we)
            {
                city.Status = "Stopped";
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                //throw we;
            }
            catch(Exception e)
            {
               // city.Status = "Stopped";
               // db.Entry(city).State = EntityState.Modified;
               // db.SaveChanges();
                // throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
              //  if (token != IntPtr.Zero) CloseHandle(token);
              //  if (tokenDuplicate != IntPtr.Zero) CloseHandle(tokenDuplicate);

                string uu = ("After finished impersonation: " + WindowsIdentity.GetCurrent().Name);
                #region logs
                string path = @"C:\YoLog\" + "Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";


                if (!System.IO.File.Exists(path))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(path))
                    {
                        sw.WriteLine(uu);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                }
                #endregion
            }
                                    #endregion              
                }
                catch(Exception ex )
                {
                    city.Status = "Stopped";
                    db.Entry(city).State = EntityState.Modified;
                    db.SaveChanges();
                    var msg = "Service:" + city.Name.Trim() + " : on: " + DateTime.Now + ": Failed with error :" + ex.Message;
                   
                    #region stats
                    var dd = DateTime.Now.Date.ToString("ddMMyyyy");
                    var sId = dd + "_" + city.Id;
                    var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                    if (stat != null)
                    {
                        stat.Errors += 1;
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        db.Entry(stat).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        stat = new NotificationStat();
                        stat.ServiceId = city.Id;
                        stat.Id = sId;
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        stat.Errors = 1;
                        db.Entry(stat).State = EntityState.Added;
                        db.SaveChanges();
                    }
                    #endregion

                    #region logs
                    string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                    if (!System.IO.File.Exists(path))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(path))
                        {
                            sw.WriteLine(msg);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(path, msg + Environment.NewLine);
                    }
                    #endregion
                }
            }
            
                return RedirectToAction("MailingServices");

        }

        public ActionResult RestartAllServices()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var citites = db.MailingServices.ToList();
            var dt = DateTime.Now.AddMinutes(-2);
            var ss = db.Emails.Where(u => u.Date <= dt).ToList();
            if (ss.Count() > 0)
            {
                foreach (var itm in ss)
                {
                    db.Emails.Remove(itm);
                }
                db.SaveChanges();
            }

            foreach (var city in citites)
            {
                try
                {
                    char[] delimiter = new char[] { '\\' };
                    string[] partz = city.ListiningFolder.Trim().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    char[] delir = new char[] { ':' };
                    string[] pat = partz[0].Split(delir, StringSplitOptions.RemoveEmptyEntries);

                    sendMailDelegate mymethod = new sendMailDelegate(InvokeMethod);
                    string directory = "";

                    if (partz.Length > 1)
                    {
                        var cnt = 0;
                        foreach (var itm in partz)
                        {
                            if (cnt == 0)
                            {
                                if (pat[0].Length == 1)
                                {
                                    directory = @"" + partz[cnt];
                                }
                                else
                                {
                                    directory = @"\\" + partz[cnt];
                                }
                            }
                            else
                            {
                                directory = directory + @"\" + partz[cnt];
                            }
                            cnt += 1;
                        }
                    }
                    else
                    {
                        if (pat.Length == 2)
                        {
                            directory = @"" + partz[0];
                        }
                        else
                        {
                            directory = @"\\" + partz[0];
                        }
                    }
                    try
                    {

                      #region Impersonate
                        WindowsIdentity m_ImpersonatedUser;
                    IntPtr token = IntPtr.Zero;
                    IntPtr tokenDuplicate = IntPtr.Zero;
                    const int SecurityImpersonation = 2;
                    const int TokenType = 1;

                   
                        if (RevertToSelf())
                        {

                            String userName = "TempUser";

                            var du = db.DomainUsers.FirstOrDefault();
                            IntPtr password = GetPassword(du.Password.Trim());
                            IntPtr logonToken = LogonUser();
                            IntPtrConstructor(logonToken);
                            IntPtrStringConstructor(logonToken);
                            IntPtrStringTypeConstructor(logonToken);
                            IntPrtStringTypeBoolConstructor(logonToken);

                            // Property implementations.
                            UseProperties(logonToken);

                            // Method implementations.

                            ImpersonateIdentity(logonToken);
                            var uu = ("Before impersonation: " +
                                             WindowsIdentity.GetCurrent().Name);
                            #region logs
                            string path = @"C:\YoLog\" + "Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                            if (!System.IO.File.Exists(path))
                            {
                                // Create a file to write to. 
                                using (StreamWriter sw = System.IO.File.CreateText(path))
                                {
                                    sw.WriteLine(uu);
                                }
                            }
                            else
                            {
                                System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                            }
                            #endregion
                            var lon = LogonUser(du.Name.Trim(), du.Domain.Trim(), du.Password.Trim(), LOGON32_LOGON_INTERACTIVE,
                                          LOGON32_PROVIDER_DEFAULT, ref token);
                            if (lon != 0)
                            {
                                if (DuplicateToken(token, SecurityImpersonation, ref tokenDuplicate) != 0)
                                {
                                    m_ImpersonatedUser = new WindowsIdentity(tokenDuplicate);
                                    using (m_ImpersonationContext = m_ImpersonatedUser.Impersonate())
                                    {
                                        if (m_ImpersonationContext != null)
                                        {
                                            uu = ("After Impersonation succeeded: " + Environment.NewLine +
                                                              "User Name: " +
                                                              WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).Name +
                                                              Environment.NewLine +
                                                              "SID: " +
                                                              WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).User.
                                                                  Value);
                                            #region logs
                                            // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                            if (!System.IO.File.Exists(path))
                                            {
                                                // Create a file to write to. 
                                                using (StreamWriter sw = System.IO.File.CreateText(path))
                                                {
                                                    sw.WriteLine(uu);
                                                }
                                            }
                                            else
                                            {
                                                System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                            }
                                            #endregion
                                            #region LoadUserProfile
                                            // Load user profile
                                            ProfileInfo profileInfo = new ProfileInfo();
                                            profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                                            profileInfo.lpUserName = userName;
                                            profileInfo.dwFlags = 1;
                                            Boolean loadSuccess = LoadUserProfile(tokenDuplicate, ref profileInfo);

                                            if (!loadSuccess)
                                            {
                                                uu = ("LoadUserProfile() failed with error code: " +
                                                                  Marshal.GetLastWin32Error());
                                                //throw new Win32Exception(Marshal.GetLastWin32Error());
                                                #region logs
                                                //string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                                if (!System.IO.File.Exists(path))
                                                {
                                                    // Create a file to write to. 
                                                    using (StreamWriter sw = System.IO.File.CreateText(path))
                                                    {
                                                        sw.WriteLine(uu);
                                                    }
                                                }
                                                else
                                                {
                                                    System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                                }
                                                #endregion
                                            }

                                            if (profileInfo.hProfile == IntPtr.Zero)
                                            {
                                                uu = (
                                                     "LoadUserProfile() failed - HKCU handle was not loaded. Error code: " +
                                                     Marshal.GetLastWin32Error());
                                                #region logs
                                                // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                                if (!System.IO.File.Exists(path))
                                                {
                                                    // Create a file to write to. 
                                                    using (StreamWriter sw = System.IO.File.CreateText(path))
                                                    {
                                                        sw.WriteLine(uu);
                                                    }
                                                }
                                                else
                                                {
                                                    System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                                }
                                                #endregion
                                            }
                                            #endregion

                                            //   CloseHandle(token);
                                            // CloseHandle(tokenDuplicate);

                    #endregion

                       System.Web.HttpRuntime.UnloadAppDomain();
                                            
                      #region EndImpression
                                            //  UnloadUserProfile(tokenDuplicate, profileInfo.hProfile);

                                            // Undo impersonation
                                            //  m_ImpersonationContext.Undo();
                                        }
                                    }
                                }
                                else
                                {
                                    uu = ("DuplicateToken() failed with error code: " + Marshal.GetLastWin32Error());
                                    #region logs
                                    // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                    if (!System.IO.File.Exists(path))
                                    {
                                        // Create a file to write to. 
                                        using (StreamWriter sw = System.IO.File.CreateText(path))
                                        {
                                            sw.WriteLine(uu);
                                        }
                                    }
                                    else
                                    {
                                        System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                    }
                                    #endregion
                                    //throw new Win32Exception(Marshal.GetLastWin32Error());

                                }
                            }
                        }
                    }
                    catch (Win32Exception we)
                    {
                        city.Status = "Stopped";
                        db.Entry(city).State = EntityState.Modified;
                        db.SaveChanges();
                        //throw we;
                    }
                    catch
                    {
                        city.Status = "Stopped";
                        db.Entry(city).State = EntityState.Modified;
                        db.SaveChanges();
                        // throw new Win32Exception(Marshal.GetLastWin32Error());
                    }
                    finally
                    {
                        //  if (token != IntPtr.Zero) CloseHandle(token);
                        //  if (tokenDuplicate != IntPtr.Zero) CloseHandle(tokenDuplicate);

                        string uu = ("After finished impersonation: " + WindowsIdentity.GetCurrent().Name);
                        #region logs
                        string path = @"C:\YoLog\" + "Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";


                        if (!System.IO.File.Exists(path))
                        {
                            // Create a file to write to. 
                            using (StreamWriter sw = System.IO.File.CreateText(path))
                            {
                                sw.WriteLine(uu);
                            }
                        }
                        else
                        {
                            System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                        }
                        #endregion
                    }
                                            #endregion
                }
                catch (Exception ex)
                {
                    city.Status = "Stopped";
                    db.Entry(city).State = EntityState.Modified;
                    db.SaveChanges();
                    var msg = "Service:" + city.Name.Trim() + " : on: " + DateTime.Now + ": Failed with error :" + ex.Message;

                    #region stats
                    var dd = DateTime.Now.Date.ToString("ddMMyyyy");
                    var sId = dd + "_" + city.Id;
                    var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                    if (stat != null)
                    {
                        stat.Errors += 1;
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        db.Entry(stat).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        stat = new NotificationStat();
                        stat.ServiceId = city.Id;
                        stat.Id = sId;
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        stat.Errors = 1;
                        db.Entry(stat).State = EntityState.Added;
                        db.SaveChanges();
                    }
                    #endregion

                    #region logs
                    string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                    if (!System.IO.File.Exists(path))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(path))
                        {
                            sw.WriteLine(msg);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(path, msg + Environment.NewLine);
                    }
                    #endregion
                }
            }

            return RedirectToAction("MailingServices");

        }

        static void InvokeMethod(object sender, FileSystemEventArgs e, MailService m,long dd)
        {
           RetailKingEntities db = new RetailKingEntities();
            var line = "";
                var fileName = e.Name;
                var serv = db.MailingServices.Find(m.ServiceId);
                
                var pre = DateTime.Now.AddMinutes(-1);
                var ww = db.Emails.Where(u => u.Receiver.Trim() == e.FullPath &&  u.Date >= pre ).FirstOrDefault();
            
                if (ww.Id == dd)
                {
                    
                    #region File Name
                    string[] partz;
                    if (serv.SendingDetails.Trim() == "File Name")
                    {
                        char[] delimita = serv.EmailSeparator.Trim().ToCharArray();
                        partz = fileName.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                        #region Has Sms
                        if (serv.HasSms == true)
                        {
                            if (m.HasList == "Yes")
                            {
                                var mlist = db.MailLists.Where(u => u.ServiceId == m.ServiceId && u.ListType.Trim() == "Sms");

                                foreach (var item in mlist)
                                {
                                    var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                    string[] emaillist = item.EmailList.Split(',');
                                    foreach (var itm in emaillist)
                                    {
                                        var sms = new Sms();
                                        sms.MSISDN = itm;
                                        sms.Message = temp.SmsMessage;
                                        sms.Sender = serv.Company.Trim();
                                        sms.Service = temp.Service;
                                        sms.ServiceId = serv.Id;
                                        sms.Filename = e.Name;
                                        SendSms(sms);
                                    }
                                }
                            }
                            else
                            {
                                char[] delimiter = new char[] { '.' };
                                string[] msisdn = partz[(int)serv.SmsPosition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                                string[] msg = partz[(int)serv.SmsMessagePosition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                                if (serv.HasTemplate == true)
                                {
                                    var temp = db.MailTemplates.Where(u => u.ServiceId == serv.Id).FirstOrDefault();

                                    var sms = new Sms();
                                    sms.MSISDN = msisdn[0];
                                    sms.Message = temp.SmsMessage;
                                    sms.Sender = serv.Company.Trim();
                                    sms.Service = temp.Service;
                                    sms.ServiceId = serv.Id;
                                    sms.Filename = e.Name;
                                    SendSms(sms);
                                }
                                else
                                {
                                    var sms = new Sms();
                                    sms.MSISDN = msisdn[0];
                                    sms.Message = msg[0];
                                    sms.Sender = serv.Company.Trim();
                                    sms.Service = serv.Name;
                                    sms.ServiceId = serv.Id;
                                    sms.Filename = e.Name;
                                    SendSms(sms);
                                }
                            }
                        }
                        #endregion

                        #region Has Email
                        if (serv.HasEmail == true)
                        {
                            var ext = "";
                            char[] delimiter = new char[] { '.' };
                            string[] parts = partz[(int)serv.StartPossition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            var len = parts.Length;
                            if ((int)serv.StartPossition == partz.Length - 1)
                            {
                                ext = parts[len - 1];
                            }
                            else
                            {
                                len = parts.Length + 1;
                            }
                            string regExp = "[^a-zA-Z0-9]";
                            if (len > 2)
                            {
                                fileName = "";
                                for (int i = 0; i < len - 1; i++)
                                {
                                    if (i == 0)
                                    {
                                        fileName = parts[i];
                                    }
                                    else
                                    {
                                        fileName = fileName + "." + parts[i];
                                    }
                                }
                            }
                            else
                            {
                                fileName = parts[0];
                            }
                            #region logs
                            /* path = @"C:\YoLog\log_" + serv.Name.Trim() + "" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                       Lmsg = "file name "+ fileName + " sent at: " + DateTime.Now;
                       if (!System.IO.File.Exists(path))
                       {
                           // Create a file to write to. 
                           using (StreamWriter sw = System.IO.File.CreateText(path))
                           {
                               sw.WriteLine(Lmsg);
                           }
                       }
                       else
                       {
                           System.IO.File.AppendAllText(path, Lmsg + Environment.NewLine);
                       }*/
                            #endregion
                            if (m.HasList == "Yes")
                            {
                                var mlist = db.MailLists.Where(u => u.ServiceId == m.ServiceId && u.ListType.Trim() == "Email");

                                foreach (var item in mlist)
                                {
                                    var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                    string[] emaillist = item.EmailList.Split(',');
                                    foreach (var itm in emaillist)
                                    {
                                        Mail mail = new Mail();
                                        mail.body = temp.Message;
                                        mail.Subject = temp.Subject;
                                        mail.Sender = temp.EmailAddress;
                                        mail.Password = temp.Password;
                                        mail.recipient = itm;
                                        mail.Attachments = e.FullPath;
                                        mail.ServiceId = serv.Id;
                                        mail.Filename = e.Name;
                                        mail.SeviceName = serv.Name;
                                        SendMail(mail);
                                    }
                                }
                            }
                            else
                            {
                                var temp = db.MailTemplates.Where(u => u.ServiceId == m.ServiceId).FirstOrDefault();

                                if (temp != null)
                                {
                                    Mail mail = new Mail();
                                    mail.body = temp.Message;
                                    mail.Subject = temp.Subject;
                                    mail.Sender = temp.EmailAddress;
                                    mail.Password = temp.Password;
                                    mail.recipient = fileName;
                                    mail.Attachments = e.FullPath;
                                    mail.ServiceId = serv.Id;
                                    mail.Filename = e.Name;
                                    mail.SeviceName = serv.Name;
                                    SendMail(mail);
                                }
                            }
                        }
                        #endregion
                    }
                    #endregion

                    #region text File
                    if (serv.SendingDetails.Trim() == "Text File")
                    {
                        var file = new System.IO.StreamReader(e.FullPath);
                        while ((line = file.ReadLine()) != null)
                        {
                            char[] delimita = serv.EmailSeparator.Trim().ToCharArray();
                            partz = line.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                            #region Send Sms
                            if (m.HasList == "Yes")
                            {
                                var mlist = db.MailLists.Where(u => u.ServiceId == m.ServiceId && u.ListType.Trim() == "Sms");

                                foreach (var item in mlist)
                                {
                                    var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                    string[] emaillist = item.EmailList.Split(',');
                                    foreach (var itm in emaillist)
                                    {
                                        var sms = new Sms();
                                        sms.MSISDN = itm;
                                        sms.Message = temp.SmsMessage;
                                        sms.Sender = serv.Company.Trim();
                                        sms.Service = temp.Service;
                                        sms.ServiceId = serv.Id;
                                        sms.Filename = e.Name;
                                        SendSms(sms);
                                    }
                                }
                            }
                            else
                            {
                                if (serv.HasTemplate == true)
                                {
                                    var temp = db.MailTemplates.Where(u => u.ServiceId == serv.Id).FirstOrDefault();

                                    var sms = new Sms();
                                    sms.MSISDN = partz[(int)serv.SmsPosition];
                                    sms.Message = temp.SmsMessage;
                                    sms.Sender = serv.Company.Trim();
                                    sms.Service = temp.Service;
                                    sms.ServiceId = serv.Id;
                                    sms.Filename = e.Name;
                                    SendSms(sms);
                                }
                                else
                                {
                                    var sms = new Sms();
                                    sms.MSISDN = partz[(int)serv.SmsPosition];
                                    sms.Message = partz[(int)serv.SmsMessagePosition];
                                    sms.Sender = serv.Company.Trim();
                                    sms.Service = serv.Name;
                                    sms.ServiceId = serv.Id;
                                    sms.Filename = e.Name;
                                    SendSms(sms);
                                }
                            }
                            #endregion

                            #region Has Email
                            if (serv.HasEmail == true)
                            {


                                if (m.HasList == "Yes")
                                {
                                    var mlist = db.MailLists.Where(u => u.ServiceId == m.ServiceId && u.ListType.Trim() == "Email");

                                    foreach (var item in mlist)
                                    {
                                        var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                        string[] emaillist = item.EmailList.Split(',');
                                        foreach (var itm in emaillist)
                                        {
                                            Mail mail = new Mail();
                                            mail.body = temp.Message;
                                            mail.Subject = temp.Subject;
                                            mail.Sender = temp.EmailAddress;
                                            mail.Password = temp.Password;
                                            mail.recipient = itm;
                                            mail.Attachments = e.FullPath;
                                            mail.ServiceId = serv.Id;
                                            mail.Filename = e.Name;
                                            mail.SeviceName = serv.Name;
                                            SendMail(mail);
                                        }
                                    }
                                }
                                else if (serv.HasTemplate == true)
                                {
                                    var temp = db.MailTemplates.Where(u => u.ServiceId == m.ServiceId).FirstOrDefault();
                                    if (temp != null)
                                    {
                                        Mail mail = new Mail();
                                        mail.body = temp.Message;
                                        mail.Subject = temp.Subject;
                                        mail.Sender = temp.EmailAddress;
                                        mail.Password = temp.Password;
                                        mail.recipient = partz[(int)serv.StartPossition];
                                        mail.Attachments = e.FullPath;
                                        mail.ServiceId = serv.Id;
                                        mail.Filename = e.Name;
                                        mail.SeviceName = serv.Name;
                                        SendMail(mail);
                                    }
                                }
                                else
                                {
                                    var ms = db.MailSettings.FirstOrDefault();
                                    Mail mail = new Mail();
                                    mail.body = partz[(int)serv.EmailMessagePosition];
                                    mail.Subject = serv.Name;
                                    mail.Sender = ms.EmailAddress;
                                    mail.Password = ms.Password;
                                    mail.recipient = partz[(int)serv.StartPossition];
                                    mail.Attachments = e.FullPath;
                                    mail.ServiceId = serv.Id;
                                    mail.Filename = e.Name;
                                    mail.SeviceName = serv.Name;
                                    SendMail(mail);
                                }
                            }
                            
                            #endregion

                        }

                        if (serv.HasEmail == false)
                        {
                                file.Close();
                                file.Dispose();
                                DeleteFile(e.FullPath);
                        }
                        else
                        {
                            if (serv.SendFileAs.Trim() == "Send As Attachment")
                            {
                                file.Close();
                                file.Dispose();
                            }
                            else
                            {
                                file.Close();
                                file.Dispose();
                                DeleteFile(e.FullPath);
                            }
                        }
                    }
                    #endregion
                }
           
        }

        static void InvokeManually( string  fileName, long ServiceId, string directory)
        {
            RetailKingEntities db = new RetailKingEntities();
            var line = "";
            char[] deli = new char[] { '\\' };
            var partf = fileName.Split(deli, StringSplitOptions.RemoveEmptyEntries);
            fileName = partf[partf.Length -1];
            var serv = db.MailingServices.Find(ServiceId);

                #region File Name
                string[] partz;
                if (serv.SendingDetails.Trim() == "File Name")
                {
                    char[] delimita = serv.EmailSeparator.Trim().ToCharArray();
                    partz = fileName.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                    #region Has Sms
                    if (serv.HasSms == true)
                    {
                        if (serv.HasMailingList.Trim() == "Yes")
                        {
                            var mlist = db.MailLists.Where(u => u.ServiceId == ServiceId && u.ListType.Trim() == "Sms");

                            foreach (var item in mlist)
                            {
                                var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                string[] emaillist = item.EmailList.Split(',');
                                foreach (var itm in emaillist)
                                {
                                    var sms = new Sms();
                                    sms.MSISDN = itm;
                                    sms.Message = temp.SmsMessage;
                                    sms.Sender = serv.Company.Trim();
                                    sms.Service = temp.Service;
                                    sms.ServiceId = serv.Id;
                                    sms.Filename = fileName;
                                    SendSms(sms);
                                }
                            }
                        }
                        else
                        {
                            char[] delimiter = new char[] { '.' };
                            string[] msisdn = partz[(int)serv.SmsPosition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            string[] msg = partz[(int)serv.SmsMessagePosition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            if (serv.HasTemplate == true)
                            {
                                var temp = db.MailTemplates.Where(u => u.ServiceId == serv.Id).FirstOrDefault();

                                var sms = new Sms();
                                sms.MSISDN = msisdn[0];
                                sms.Message = temp.SmsMessage;
                                sms.Sender = serv.Company.Trim();
                                sms.Service = temp.Service;
                                sms.ServiceId = serv.Id;
                                sms.Filename = fileName;
                                SendSms(sms);
                            }
                            else
                            {
                                var sms = new Sms();
                                sms.MSISDN = msisdn[0];
                                sms.Message = msg[0];
                                sms.Sender = serv.Company.Trim();
                                sms.Service = serv.Name;
                                sms.ServiceId = serv.Id;
                                sms.Filename = fileName;
                                SendSms(sms);
                            }
                        }
                    }
                    #endregion

                    #region Has Email
                    if (serv.HasEmail == true)
                    {
                        var ext = "";
                        char[] delimiter = new char[] { '.' };
                        string[] parts = partz[(int)serv.StartPossition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        var len = parts.Length;
                        if ((int)serv.StartPossition == partz.Length - 1)
                        {
                            ext = parts[len - 1];
                        }
                        else
                        {
                            len = parts.Length + 1;
                        }
                        string regExp = "[^a-zA-Z0-9]";
                        if (len > 2)
                        {
                            fileName = "";
                            for (int i = 0; i < len - 1; i++)
                            {
                                if (i == 0)
                                {
                                    fileName = parts[i];
                                }
                                else
                                {
                                    fileName = fileName + "." + parts[i];
                                }
                            }
                        }
                        else
                        {
                            fileName = parts[0];
                        }
                        #region logs
                        /* path = @"C:\YoLog\log_" + serv.Name.Trim() + "" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                       Lmsg = "file name "+ fileName + " sent at: " + DateTime.Now;
                       if (!System.IO.File.Exists(path))
                       {
                           // Create a file to write to. 
                           using (StreamWriter sw = System.IO.File.CreateText(path))
                           {
                               sw.WriteLine(Lmsg);
                           }
                       }
                       else
                       {
                           System.IO.File.AppendAllText(path, Lmsg + Environment.NewLine);
                       }*/
                        #endregion
                        if (serv.HasMailingList.Trim() == "Yes")
                        {
                            var mlist = db.MailLists.Where(u => u.ServiceId == ServiceId && u.ListType.Trim() == "Email");

                            foreach (var item in mlist)
                            {
                                var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                string[] emaillist = item.EmailList.Split(',');
                                foreach (var itm in emaillist)
                                {
                                    Mail mail = new Mail();
                                    mail.body = temp.Message;
                                    mail.Subject = temp.Subject;
                                    mail.Sender = temp.EmailAddress;
                                    mail.Password = temp.Password;
                                    mail.recipient = itm;
                                    mail.Attachments = directory +  @"\\" + fileName;
                                    mail.ServiceId = serv.Id;
                                    mail.Filename = fileName;
                                    mail.SeviceName = serv.Name;
                                    SendMail(mail);
                                }
                            }
                        }
                        else
                        {
                            var temp = db.MailTemplates.Where(u => u.ServiceId == ServiceId).FirstOrDefault();

                            if (temp != null)
                            {
                                Mail mail = new Mail();
                                mail.body = temp.Message;
                                mail.Subject = temp.Subject;
                                mail.Sender = temp.EmailAddress;
                                mail.Password = temp.Password;
                                mail.recipient = fileName;
                                mail.Attachments = directory +  @"\\" + fileName;
                                mail.ServiceId = serv.Id;
                                mail.Filename = fileName;
                                mail.SeviceName = serv.Name;
                                SendMail(mail);
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region text File
                if (serv.SendingDetails.Trim() == "Text File")
                {
                    var file = new System.IO.StreamReader(directory +  @"\\" + fileName);
                    while ((line = file.ReadLine()) != null)
                    {
                        char[] delimita = serv.EmailSeparator.Trim().ToCharArray();
                        partz = line.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                        #region Send Sms
                        if (serv.HasMailingList.Trim() == "Yes")
                        {
                            var mlist = db.MailLists.Where(u => u.ServiceId == ServiceId && u.ListType.Trim() == "Sms");

                            foreach (var item in mlist)
                            {
                                var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                string[] emaillist = item.EmailList.Split(',');
                                foreach (var itm in emaillist)
                                {
                                    var sms = new Sms();
                                    sms.MSISDN = itm;
                                    sms.Message = temp.SmsMessage;
                                    sms.Sender = serv.Company.Trim();
                                    sms.Service = temp.Service;
                                    sms.ServiceId = serv.Id;
                                    sms.Filename = fileName;
                                    SendSms(sms);
                                }
                            }
                        }
                        else
                        {
                            if (serv.HasTemplate == true)
                            {
                                var temp = db.MailTemplates.Where(u => u.ServiceId == serv.Id).FirstOrDefault();

                                var sms = new Sms();
                                sms.MSISDN = partz[(int)serv.SmsPosition];
                                sms.Message = temp.SmsMessage;
                                sms.Sender = serv.Company.Trim();
                                sms.Service = temp.Service;
                                sms.ServiceId = serv.Id;
                                sms.Filename = fileName;
                                SendSms(sms);
                            }
                            else
                            {
                                var sms = new Sms();
                                sms.MSISDN = partz[(int)serv.SmsPosition];
                                sms.Message = partz[(int)serv.SmsMessagePosition];
                                sms.Sender = serv.Company.Trim();
                                sms.Service = serv.Name;
                                sms.ServiceId = serv.Id;
                                sms.Filename = fileName;
                                SendSms(sms);
                            }
                        }
                        #endregion

                        #region Has Email
                        if (serv.HasEmail == true)
                        {

                            if (serv.HasMailingList.Trim() == "Yes")
                            {
                                var mlist = db.MailLists.Where(u => u.ServiceId == ServiceId && u.ListType.Trim() == "Email");

                                foreach (var item in mlist)
                                {
                                    var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                    string[] emaillist = item.EmailList.Split(',');
                                    foreach (var itm in emaillist)
                                    {
                                        Mail mail = new Mail();
                                        mail.body = temp.Message;
                                        mail.Subject = temp.Subject;
                                        mail.Sender = temp.EmailAddress;
                                        mail.Password = temp.Password;
                                        mail.recipient = itm;
                                        mail.Attachments = directory +  @"\\" + fileName;
                                        mail.ServiceId = serv.Id;
                                        mail.Filename = fileName;
                                        mail.SeviceName = serv.Name;
                                        SendMail(mail);
                                    }
                                }
                            }
                            else if (serv.HasTemplate == true)
                            {
                                var temp = db.MailTemplates.Where(u => u.ServiceId == ServiceId).FirstOrDefault();
                                if (temp != null)
                                {
                                    Mail mail = new Mail();
                                    mail.body = temp.Message;
                                    mail.Subject = temp.Subject;
                                    mail.Sender = temp.EmailAddress;
                                    mail.Password = temp.Password;
                                    mail.recipient = partz[(int)serv.StartPossition];
                                    mail.Attachments =directory +  @"\\" + fileName;
                                    mail.ServiceId = serv.Id;
                                    mail.Filename = fileName;
                                    mail.SeviceName = serv.Name;
                                    SendMail(mail);
                                }
                            }
                            else
                            {
                                var ms = db.MailSettings.FirstOrDefault();
                                Mail mail = new Mail();
                                mail.body = partz[(int)serv.EmailMessagePosition];
                                mail.Subject = serv.Name;
                                mail.Sender = ms.EmailAddress;
                                mail.Password = ms.Password;
                                mail.recipient = partz[(int)serv.StartPossition];
                                mail.Attachments = directory +  @"\\" + fileName;
                                mail.ServiceId = serv.Id;
                                mail.Filename = fileName;
                                mail.SeviceName = serv.Name;
                                SendMail(mail);
                            }
                        }

                        #endregion

                    }

                    if (serv.HasEmail == false)
                    {
                        file.Close();
                        file.Dispose();
                        DeleteFile(directory +  @"\\" + fileName);
                    }
                    else
                    {
                        if (serv.SendFileAs.Trim() == "Send As Attachment")
                        {
                            file.Close();
                            file.Dispose();
                        }
                        else
                        {
                            file.Close();
                            file.Dispose();
                            DeleteFile(directory +  @"\\" + fileName);
                        }
                    }
                }
                #endregion
            

        }

        static void InvokeNonfile(string fileName, long ServiceId, string directory)
        {
            RetailKingEntities db = new RetailKingEntities();
            var line = "";
            char[] deli = new char[] { '\\' };
            var partf = fileName.Split(deli, StringSplitOptions.RemoveEmptyEntries);
            fileName = partf[partf.Length - 1];
            var serv = db.MailingServices.Find(ServiceId);

            #region File Name
            string[] partz;
            if (serv.SendingDetails.Trim() == "File Name")
            {
                char[] delimita = serv.EmailSeparator.Trim().ToCharArray();
                partz = fileName.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                #region Has Sms
                if (serv.HasSms == true)
                {
                    if (serv.HasMailingList.Trim() == "Yes")
                    {
                        var mlist = db.MailLists.Where(u => u.ServiceId == ServiceId && u.ListType.Trim() == "Sms");

                        foreach (var item in mlist)
                        {
                            var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                            string[] emaillist = item.EmailList.Split(',');
                            foreach (var itm in emaillist)
                            {
                                var sms = new Sms();
                                sms.MSISDN = itm;
                                sms.Message = temp.SmsMessage;
                                sms.Sender = serv.Company.Trim();
                                sms.Service = temp.Service;
                                sms.ServiceId = serv.Id;
                                sms.Filename = fileName;
                                SendSms(sms);
                            }
                        }
                    }
                    else
                    {
                        char[] delimiter = new char[] { '.' };
                        string[] msisdn = partz[(int)serv.SmsPosition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        string[] msg = partz[(int)serv.SmsMessagePosition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        if (serv.HasTemplate == true)
                        {
                            var temp = db.MailTemplates.Where(u => u.ServiceId == serv.Id).FirstOrDefault();

                            var sms = new Sms();
                            sms.MSISDN = msisdn[0];
                            sms.Message = temp.SmsMessage;
                            sms.Sender = serv.Company.Trim();
                            sms.Service = temp.Service;
                            sms.ServiceId = serv.Id;
                            sms.Filename = fileName;
                            SendSms(sms);
                        }
                        else
                        {
                            var sms = new Sms();
                            sms.MSISDN = msisdn[0];
                            sms.Message = msg[0];
                            sms.Sender = serv.Company.Trim();
                            sms.Service = serv.Name;
                            sms.ServiceId = serv.Id;
                            sms.Filename = fileName;
                            SendSms(sms);
                        }
                    }
                }
                #endregion

                #region Has Email
                if (serv.HasEmail == true)
                {
                    var ext = "";
                    char[] delimiter = new char[] { '.' };
                    string[] parts = partz[(int)serv.StartPossition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    var len = parts.Length;
                    if ((int)serv.StartPossition == partz.Length - 1)
                    {
                        ext = parts[len - 1];
                    }
                    else
                    {
                        len = parts.Length + 1;
                    }
                    string regExp = "[^a-zA-Z0-9]";
                    if (len > 2)
                    {
                        fileName = "";
                        for (int i = 0; i < len - 1; i++)
                        {
                            if (i == 0)
                            {
                                fileName = parts[i];
                            }
                            else
                            {
                                fileName = fileName + "." + parts[i];
                            }
                        }
                    }
                    else
                    {
                        fileName = parts[0];
                    }
                    #region logs
                    /* path = @"C:\YoLog\log_" + serv.Name.Trim() + "" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                       Lmsg = "file name "+ fileName + " sent at: " + DateTime.Now;
                       if (!System.IO.File.Exists(path))
                       {
                           // Create a file to write to. 
                           using (StreamWriter sw = System.IO.File.CreateText(path))
                           {
                               sw.WriteLine(Lmsg);
                           }
                       }
                       else
                       {
                           System.IO.File.AppendAllText(path, Lmsg + Environment.NewLine);
                       }*/
                    #endregion
                    if (serv.HasMailingList.Trim() == "Yes")
                    {
                        var mlist = db.MailLists.Where(u => u.ServiceId == ServiceId && u.ListType.Trim() == "Email");

                        foreach (var item in mlist)
                        {
                            var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                            string[] emaillist = item.EmailList.Split(',');
                            foreach (var itm in emaillist)
                            {
                                Mail mail = new Mail();
                                mail.body = temp.Message;
                                mail.Subject = temp.Subject;
                                mail.Sender = temp.EmailAddress;
                                mail.Password = temp.Password;
                                mail.recipient = itm;
                                mail.Attachments = directory + @"\\" + fileName;
                                mail.ServiceId = serv.Id;
                                mail.Filename = fileName;
                                mail.SeviceName = serv.Name;
                                SendMail(mail);
                            }
                        }
                    }
                    else
                    {
                        var temp = db.MailTemplates.Where(u => u.ServiceId == ServiceId).FirstOrDefault();

                        if (temp != null)
                        {
                            Mail mail = new Mail();
                            mail.body = temp.Message;
                            mail.Subject = temp.Subject;
                            mail.Sender = temp.EmailAddress;
                            mail.Password = temp.Password;
                            mail.recipient = fileName;
                            mail.Attachments = directory + @"\\" + fileName;
                            mail.ServiceId = serv.Id;
                            mail.Filename = fileName;
                            mail.SeviceName = serv.Name;
                            SendMail(mail);
                        }
                    }
                }
                #endregion
            }
            #endregion

            #region text File
            if (serv.SendingDetails.Trim() == "Text File")
            {
                var file = new System.IO.StreamReader(directory + @"\\" + fileName);
                while ((line = file.ReadLine()) != null)
                {
                    char[] delimita = serv.EmailSeparator.Trim().ToCharArray();
                    partz = line.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                    #region Send Sms
                    if (serv.HasMailingList.Trim() == "Yes")
                    {
                        var mlist = db.MailLists.Where(u => u.ServiceId == ServiceId && u.ListType.Trim() == "Sms");

                        foreach (var item in mlist)
                        {
                            var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                            string[] emaillist = item.EmailList.Split(',');
                            foreach (var itm in emaillist)
                            {
                                var sms = new Sms();
                                sms.MSISDN = itm;
                                sms.Message = temp.SmsMessage;
                                sms.Sender = serv.Company.Trim();
                                sms.Service = temp.Service;
                                sms.ServiceId = serv.Id;
                                sms.Filename = fileName;
                                SendSms(sms);
                            }
                        }
                    }
                    else
                    {
                        if (serv.HasTemplate == true)
                        {
                            var temp = db.MailTemplates.Where(u => u.ServiceId == serv.Id).FirstOrDefault();

                            var sms = new Sms();
                            sms.MSISDN = partz[(int)serv.SmsPosition];
                            sms.Message = temp.SmsMessage;
                            sms.Sender = serv.Company.Trim();
                            sms.Service = temp.Service;
                            sms.ServiceId = serv.Id;
                            sms.Filename = fileName;
                            SendSms(sms);
                        }
                        else
                        {
                            var sms = new Sms();
                            sms.MSISDN = partz[(int)serv.SmsPosition];
                            sms.Message = partz[(int)serv.SmsMessagePosition];
                            sms.Sender = serv.Company.Trim();
                            sms.Service = serv.Name;
                            sms.ServiceId = serv.Id;
                            sms.Filename = fileName;
                            SendSms(sms);
                        }
                    }
                    #endregion

                    #region Has Email
                    if (serv.HasEmail == true)
                    {

                        if (serv.HasMailingList.Trim() == "Yes")
                        {
                            var mlist = db.MailLists.Where(u => u.ServiceId == ServiceId && u.ListType.Trim() == "Email");

                            foreach (var item in mlist)
                            {
                                var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                string[] emaillist = item.EmailList.Split(',');
                                foreach (var itm in emaillist)
                                {
                                    Mail mail = new Mail();
                                    mail.body = temp.Message;
                                    mail.Subject = temp.Subject;
                                    mail.Sender = temp.EmailAddress;
                                    mail.Password = temp.Password;
                                    mail.recipient = itm;
                                    mail.Attachments = directory + @"\\" + fileName;
                                    mail.ServiceId = serv.Id;
                                    mail.Filename = fileName;
                                    mail.SeviceName = serv.Name;
                                    SendMail(mail);
                                }
                            }
                        }
                        else if (serv.HasTemplate == true)
                        {
                            var temp = db.MailTemplates.Where(u => u.ServiceId == ServiceId).FirstOrDefault();
                            if (temp != null)
                            {
                                Mail mail = new Mail();
                                mail.body = temp.Message;
                                mail.Subject = temp.Subject;
                                mail.Sender = temp.EmailAddress;
                                mail.Password = temp.Password;
                                mail.recipient = partz[(int)serv.StartPossition];
                                mail.Attachments = directory + @"\\" + fileName;
                                mail.ServiceId = serv.Id;
                                mail.Filename = fileName;
                                mail.SeviceName = serv.Name;
                                SendMail(mail);
                            }
                        }
                        else
                        {
                            var ms = db.MailSettings.FirstOrDefault();
                            Mail mail = new Mail();
                            mail.body = partz[(int)serv.EmailMessagePosition];
                            mail.Subject = serv.Name;
                            mail.Sender = ms.EmailAddress;
                            mail.Password = ms.Password;
                            mail.recipient = partz[(int)serv.StartPossition];
                            mail.Attachments = directory + @"\\" + fileName;
                            mail.ServiceId = serv.Id;
                            mail.Filename = fileName;
                            mail.SeviceName = serv.Name;
                            SendMail(mail);
                        }
                    }

                    #endregion

                }

                if (serv.HasEmail == false)
                {
                    file.Close();
                    file.Dispose();
                    DeleteFile(directory + @"\\" + fileName);
                }
                else
                {
                    if (serv.SendFileAs.Trim() == "Send As Attachment")
                    {
                        file.Close();
                        file.Dispose();
                    }
                    else
                    {
                        file.Close();
                        file.Dispose();
                        DeleteFile(directory + @"\\" + fileName);
                    }
                }
            }
            #endregion


        }

        private void CleanService(long Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var city = db.MailingServices.Find(Id);


            char[] delimiter = new char[] { '\\' };
            string[] partz = city.ListiningFolder.Trim().Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            char[] delir = new char[] { ':' };
            string[] pat = partz[0].Split(delir, StringSplitOptions.RemoveEmptyEntries);

            sendMailDelegate mymethod = new sendMailDelegate(InvokeMethod);
            string directory = "";

            if (partz.Length > 1)
            {
                var cnt = 0;
                foreach (var itm in partz)
                {
                    if (cnt == 0)
                    {
                        if (pat[0].Length == 1)
                        {
                            directory = @"" + partz[cnt];
                        }
                        else
                        {
                            directory = @"\\" + partz[cnt];
                        }
                    }
                    else
                    {
                        directory = directory + @"\" + partz[cnt];
                    }
                    cnt += 1;
                }
            }
            else
            {
                if (pat.Length == 2)
                {
                    directory = @"" + partz[0];
                }
                else
                {
                    directory = @"\\" + partz[0];
                }
            }
            try
            {
                #region Impersonate
                WindowsIdentity m_ImpersonatedUser;
                IntPtr token = IntPtr.Zero;
                IntPtr tokenDuplicate = IntPtr.Zero;
                const int SecurityImpersonation = 2;
                const int TokenType = 1;

                try
                {
                    if (RevertToSelf())
                    {

                        String userName = "TempUser";

                        var du = db.DomainUsers.FirstOrDefault();
                        IntPtr password = GetPassword(du.Password.Trim());
                        IntPtr logonToken = LogonUser();
                        IntPtrConstructor(logonToken);
                        IntPtrStringConstructor(logonToken);
                        IntPtrStringTypeConstructor(logonToken);
                        IntPrtStringTypeBoolConstructor(logonToken);

                        // Property implementations.
                        UseProperties(logonToken);

                        // Method implementations.

                        ImpersonateIdentity(logonToken);
                        var uu = ("Before impersonation: " +
                                         WindowsIdentity.GetCurrent().Name);
                        #region logs
                        string path = @"C:\YoLog\" + "Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                        if (!System.IO.File.Exists(path))
                        {
                            // Create a file to write to. 
                            using (StreamWriter sw = System.IO.File.CreateText(path))
                            {
                                sw.WriteLine(uu);
                            }
                        }
                        else
                        {
                            System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                        }
                        #endregion
                        var lon = LogonUser(du.Name.Trim(), du.Domain.Trim(), du.Password.Trim(), LOGON32_LOGON_INTERACTIVE,
                                      LOGON32_PROVIDER_DEFAULT, ref token);
                        if (lon != 0)
                        {
                            if (DuplicateToken(token, SecurityImpersonation, ref tokenDuplicate) != 0)
                            {
                                m_ImpersonatedUser = new WindowsIdentity(tokenDuplicate);
                                using (m_ImpersonationContext = m_ImpersonatedUser.Impersonate())
                                {
                                    if (m_ImpersonationContext != null)
                                    {
                                        uu = ("After Impersonation succeeded: " + Environment.NewLine +
                                                          "User Name: " +
                                                          WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).Name +
                                                          Environment.NewLine +
                                                          "SID: " +
                                                          WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).User.
                                                              Value);
                                        #region logs
                                        // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                        if (!System.IO.File.Exists(path))
                                        {
                                            // Create a file to write to. 
                                            using (StreamWriter sw = System.IO.File.CreateText(path))
                                            {
                                                sw.WriteLine(uu);
                                            }
                                        }
                                        else
                                        {
                                            System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                        }
                                        #endregion

                                        #region LoadUserProfile
                                        // Load user profile
                                        ProfileInfo profileInfo = new ProfileInfo();
                                        profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                                        profileInfo.lpUserName = userName;
                                        profileInfo.dwFlags = 1;
                                        Boolean loadSuccess = LoadUserProfile(tokenDuplicate, ref profileInfo);

                                        if (!loadSuccess)
                                        {
                                            uu = ("LoadUserProfile() failed with error code: " +
                                                              Marshal.GetLastWin32Error());
                                            throw new Win32Exception(Marshal.GetLastWin32Error());
                                            #region logs
                                            //string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                            if (!System.IO.File.Exists(path))
                                            {
                                                // Create a file to write to. 
                                                using (StreamWriter sw = System.IO.File.CreateText(path))
                                                {
                                                    sw.WriteLine(uu);
                                                }
                                            }
                                            else
                                            {
                                                System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                            }
                                            #endregion
                                        }

                                        if (profileInfo.hProfile == IntPtr.Zero)
                                        {
                                            uu = (
                                                 "LoadUserProfile() failed - HKCU handle was not loaded. Error code: " +
                                                 Marshal.GetLastWin32Error());
                                            #region logs
                                            // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                            if (!System.IO.File.Exists(path))
                                            {
                                                // Create a file to write to. 
                                                using (StreamWriter sw = System.IO.File.CreateText(path))
                                                {
                                                    sw.WriteLine(uu);
                                                }
                                            }
                                            else
                                            {
                                                System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                            }
                                            #endregion
                                        }
                                        #endregion

                                        CloseHandle(token);
                                        CloseHandle(tokenDuplicate);

                #endregion
                                        // System.IO.FileInfo[] files = null;
                                        var files = System.IO.Directory.GetFiles(directory);
                                        if (files != null)
                                        {
                                            foreach (var fi in files)
                                            {
                                                // In this example, we only access the existing FileInfo object. If we
                                                // want to open, delete or modify the file, then
                                                // a try-catch block is required here to handle the case
                                                // where the file has been deleted since the call to TraverseTree().
                                                //Console.WriteLine(fi.FullName);
                                                // city.HasMailingList;
                                                InvokeManually(fi, city.Id, directory);
                                            }
                                        }

                                        #region EndImpression
                                        //  UnloadUserProfile(tokenDuplicate, profileInfo.hProfile);

                                        // Undo impersonation
                                        //  m_ImpersonationContext.Undo();
                                    }
                                }
                            }
                            else
                            {
                                uu = ("DuplicateToken() failed with error code: " + Marshal.GetLastWin32Error());
                                #region logs
                                // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                if (!System.IO.File.Exists(path))
                                {
                                    // Create a file to write to. 
                                    using (StreamWriter sw = System.IO.File.CreateText(path))
                                    {
                                        sw.WriteLine(uu);
                                    }
                                }
                                else
                                {
                                    System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                }
                                #endregion
                                //throw new Win32Exception(Marshal.GetLastWin32Error());

                            }
                        }
                    }
                }
                catch (Win32Exception we)
                {
                    city.Status = "Stopped";
                    db.Entry(city).State = EntityState.Modified;
                    db.SaveChanges();
                    //throw we;
                }
                catch
                {
                    city.Status = "Stopped";
                    db.Entry(city).State = EntityState.Modified;
                    db.SaveChanges();
                    // throw new Win32Exception(Marshal.GetLastWin32Error());
                }
                finally
                {
                    if (token != IntPtr.Zero) CloseHandle(token);
                    if (tokenDuplicate != IntPtr.Zero) CloseHandle(tokenDuplicate);

                    string uu = ("After finished impersonation: " + WindowsIdentity.GetCurrent().Name);
                    #region logs
                    string path = @"C:\YoLog\" + "Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";


                    if (!System.IO.File.Exists(path))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(path))
                        {
                            sw.WriteLine(uu);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                    }
                    #endregion
                }
                                        #endregion
            }
            catch (Exception ex)
            {
               
                var msg = "Service:" + city.Name.Trim() + " : on: " + DateTime.Now + ": Failed with error :" + ex.Message;

                #region stats
                var dd = DateTime.Now.Date.ToString("ddMMyyyy");
                var sId = dd + "_" + city.Id;
                var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                if (stat != null)
                {
                    stat.Errors += 1;
                    stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                    db.Entry(stat).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    stat = new NotificationStat();
                    stat.ServiceId = city.Id;
                    stat.Id = sId;
                    stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                    stat.Errors = 1;
                    db.Entry(stat).State = EntityState.Added;
                    db.SaveChanges();
                }
                #endregion

                #region logs
                string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                if (!System.IO.File.Exists(path))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(path))
                    {
                        sw.WriteLine(msg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(path, msg + Environment.NewLine);
                }
                #endregion
            }


        }

        #region net.mail
        public ActionResult ManualMail()
        {
            return PartialView();
        }
       
        public static void SendMail(Mail mail)
        {
            string emailFile = "";
            string path = "";
            MailMessage mailMessage = new MailMessage();
            RetailKingEntities db = new RetailKingEntities();
            #region Impersonate
            WindowsIdentity m_ImpersonatedUser;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;
            const int SecurityImpersonation = 2;
            const int TokenType = 1;

            try
            {
                if (RevertToSelf())
                {

                    String userName = "TempUser";

                    var du = db.DomainUsers.FirstOrDefault();
                    IntPtr password = GetPassword(du.Password.Trim());
                    IntPtr logonToken = LogonUser();
                    IntPtrConstructor(logonToken);
                    IntPtrStringConstructor(logonToken);
                    IntPtrStringTypeConstructor(logonToken);
                    IntPrtStringTypeBoolConstructor(logonToken);

                    // Property implementations.
                    UseProperties(logonToken);

                    // Method implementations.

                    ImpersonateIdentity(logonToken);
                    var uu = ("Before impersonation: " +
                                     WindowsIdentity.GetCurrent().Name);
                    #region logs
                    path = @"C:\YoLog\" + "Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                    if (!System.IO.File.Exists(path))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(path))
                        {
                            sw.WriteLine(uu);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                    }
                    #endregion
                    var lon = LogonUser(du.Name.Trim(), du.Domain.Trim(), du.Password.Trim(), LOGON32_LOGON_INTERACTIVE,
                                  LOGON32_PROVIDER_DEFAULT, ref token);
                    if (lon != 0)
                    {
                        if (DuplicateToken(token, SecurityImpersonation, ref tokenDuplicate) != 0)
                        {
                            m_ImpersonatedUser = new WindowsIdentity(tokenDuplicate);
                            using (m_ImpersonationContext = m_ImpersonatedUser.Impersonate())
                            {
                                if (m_ImpersonationContext != null)
                                {
                                    uu = ("After Impersonation succeeded: " + Environment.NewLine +
                                                      "User Name: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).Name +
                                                      Environment.NewLine +
                                                      "SID: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).User.
                                                          Value);
                                    #region logs
                                    // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                    if (!System.IO.File.Exists(path))
                                    {
                                        // Create a file to write to. 
                                        using (StreamWriter sw = System.IO.File.CreateText(path))
                                        {
                                            sw.WriteLine(uu);
                                        }
                                    }
                                    else
                                    {
                                        System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                    }
                                    #endregion
                                    #region LoadUserProfile
                                    // Load user profile
                                    ProfileInfo profileInfo = new ProfileInfo();
                                    profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                                    profileInfo.lpUserName = userName;
                                    profileInfo.dwFlags = 1;
                                    Boolean loadSuccess = LoadUserProfile(tokenDuplicate, ref profileInfo);

                                    if (!loadSuccess)
                                    {
                                        uu = ("LoadUserProfile() failed with error code: " +
                                                          Marshal.GetLastWin32Error());
                                        //throw new Win32Exception(Marshal.GetLastWin32Error());
                                        #region logs
                                        //string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                        if (!System.IO.File.Exists(path))
                                        {
                                            // Create a file to write to. 
                                            using (StreamWriter sw = System.IO.File.CreateText(path))
                                            {
                                                sw.WriteLine(uu);
                                            }
                                        }
                                        else
                                        {
                                            System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                        }
                                        #endregion
                                    }

                                    if (profileInfo.hProfile == IntPtr.Zero)
                                    {
                                        uu = (
                                             "LoadUserProfile() failed - HKCU handle was not loaded. Error code: " +
                                             Marshal.GetLastWin32Error());
                                        #region logs
                                        // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                        if (!System.IO.File.Exists(path))
                                        {
                                            // Create a file to write to. 
                                            using (StreamWriter sw = System.IO.File.CreateText(path))
                                            {
                                                sw.WriteLine(uu);
                                            }
                                        }
                                        else
                                        {
                                            System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                        }
                                        #endregion
                                    }
                                    #endregion

                                  //  CloseHandle(token);
                                   // CloseHandle(tokenDuplicate);

            #endregion
           
            #region logs
            path = @"C:\YoLog\log_" + mail.SeviceName.Trim() + "" + DateTime.Now.ToString("ddMMyyy") + ".txt";
            var Lmsg = "Now sending mail to " + mail.recipient + " sent at: " + DateTime.Now;
            if (!System.IO.File.Exists(path))
            {
                // Create a file to write to. 
                using (StreamWriter sw = System.IO.File.CreateText(path))
                {
                    sw.WriteLine(Lmsg);
                }
            }
            else
            {
                System.IO.File.AppendAllText(path, Lmsg + Environment.NewLine);
            }
            #endregion
            try
            {
                var settings = db.MailSettings.FirstOrDefault();
                var grps = db.MailGroups.Where(u => u.Receiver.Trim() == mail.recipient).FirstOrDefault();
               
                if (settings.UseExchange == true)
                {
                    #region logs
                   /* path = @"C:\YoLog\" + mail.SeviceName.Trim() + "" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                    string thenote = "About to sent through exchange file: " + mail.Filename;
                    if (!System.IO.File.Exists(path))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(path))
                        {
                            sw.WriteLine(thenote);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(path, thenote + Environment.NewLine);
                    }*/
                    #endregion
                    #region Exchange
                  
                    ServicePointManager.ServerCertificateValidationCallback = CertificateValidationCallBack;

                    ExchangeService service = new ExchangeService(ExchangeVersion.Exchange2013 );
                    service.Credentials = new WebCredentials(mail.Sender.Trim(), mail.Password.Trim(), settings.Domain.Trim());

                    // service.AutodiscoverUrl("username@yourdomain.com", RedirectionUrlValidationCallback);
                    service.Url = new System.Uri("https://172.18.0.130/EWS/Exchange.asmx");//exchange.metbank.co.zw

                    EmailMessage email = new EmailMessage(service);
                    email.ToRecipients.Add(mail.recipient.Trim());
                    email.Subject = mail.Subject;
                  
                    if (grps != null)
                    {
                        char[] delimit = new char[] { ',' };
                        if (grps.Bcc != null)
                        {
                            string[] parz = grps.Bcc.Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var bcc in parz)
                            {
                                email.BccRecipients.Add(bcc);
                            }
                        }
                        if (grps.Cc != null)
                        {
                            string[] parz = grps.Cc.Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var Cc in parz)
                            {
                                email.CcRecipients.Add(Cc);
                            }
                        }
                    }
                    if (!string.IsNullOrEmpty(mail.Attachments))
                    {
                        char[] delimiter = new char[] { ',' };
                        string[] parts = mail.Attachments.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < parts.Length; i++)
                        {
                            // Create  the file attachment for this e-mail message.
                            string file = parts[i];
                            email.Attachments.AddFileAttachment(file);  
                        }
                    }

                    email.Body = new MessageBody(mail.body);
                    email.SendAndSaveCopy();
                    #region stats
                    var dd = DateTime.Now.Date.ToString("ddMMyyyy");
                    var sId = dd + "_" + mail.ServiceId;
                    var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                    if (stat != null)
                    {
                        stat.SentMail += 1;
                       
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        db.Entry(stat).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        stat = new NotificationStat();
                        stat.Id = sId;
                        stat.SentMail = 1;
                        stat.ServiceId = mail.ServiceId;
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        db.Entry(stat).State = EntityState.Added ;
                        db.SaveChanges();
                    }
                    #endregion
                    #region logs
                    path = @"C:\YoLog\" + mail.SeviceName.Trim() + "" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                    var msg = "mail to: "+ mail.recipient + " sent at: " + DateTime.Now;
                    if (!System.IO.File.Exists(path))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(path))
                        {
                            sw.WriteLine(msg);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(path, msg + Environment.NewLine);
                    }
                    #endregion

                    if (!string.IsNullOrEmpty(mail.Attachments))
                    {

                        char[] delimiter = new char[] { ',' };
                        string[] parts = mail.Attachments.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
             
                        for (int i = 0; i < parts.Length; i++)
                        {
                            // Create  the file attachment for this e-mail message.
                            DeleteFile(parts[i]);
                        }
                    }
                    #endregion 
                }
                else
                {
                    #region logs
                    /* path = @"C:\YoLog\" + mail.SeviceName.Trim() + "" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                    string thenote = "About to sent through smtp file: " + mail.Filename;
                    if (!System.IO.File.Exists(path))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(path))
                        {
                            sw.WriteLine(thenote);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(path, thenote + Environment.NewLine);
                    }*/
                    #endregion
                    #region Smtp
                   
                    if (settings.UseLocalSmtp == false)
                    {
                        mailMessage = new MailMessage(
                                                   new MailAddress(mail.Sender.Trim()), new MailAddress(mail.recipient.Trim()));
                    }

                    System.Net.NetworkCredential networkCredentials = new NetworkCredential();
                    if (settings.UseLocalSmtp == true)
                    {
                        MailAddress fromAddress = new MailAddress(mail.Sender.Trim());
                        mailMessage.From = fromAddress;
                        mailMessage.To.Add(mail.recipient.Trim());
                    }
                    else
                    {
                        networkCredentials = new
                        System.Net.NetworkCredential(mail.Sender.Trim(), mail.Password.Trim());
                    }

                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = mail.body.Trim();
                    if (grps != null)
                    {
                        char[] delimit = new char[] { ',' };
                        if (grps.Bcc != null)
                        {
                            string[] parz = grps.Bcc.Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var bcc in parz)
                            {
                                mailMessage.Bcc.Add(bcc);
                            }
                        }
                        if (grps.Cc != null)
                        {
                            string[] parz = grps.Cc.Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                            foreach (var Cc in parz)
                            {
                                mailMessage.Bcc.Add(Cc);
                            }
                        }
                    }
               
                    SmtpClient smtpClient = new SmtpClient();
                    if (settings.UseLocalSmtp == false)
                    {             
                        smtpClient.Host = settings.SmtpHost.Trim();
                    }
                    else
                    {
                        smtpClient.Host = "localhost";  
                    }
                    smtpClient.EnableSsl = (bool)settings.EnableSSl;
                    smtpClient.UseDefaultCredentials = false;
                    smtpClient.Credentials = networkCredentials;
                    smtpClient.Port = (int)settings.Port;
                    smtpClient.Timeout = 20000;
                    //smtpClient.ClientCertificates 
                    /* We use the following variables to keep track of
                       attachments and after we can delete them */
                    if (!string.IsNullOrEmpty(mail.Attachments))
                    {
                        char[] delimiter = new char[] { ',' };
                        string[] parts = mail.Attachments.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < parts.Length; i++)
                        {
                            // Create  the file attachment for this e-mail message.
                            string file = parts[i];
                            System.Net.Mail.Attachment data = new System.Net.Mail.Attachment(file, MediaTypeNames.Application.Octet);
                            // Add time stamp information for the file.
                            ContentDisposition disposition = data.ContentDisposition;
                            disposition.CreationDate = System.IO.File.GetCreationTime(file);
                            disposition.ModificationDate = System.IO.File.GetLastWriteTime(file);
                            disposition.ReadDate = System.IO.File.GetLastAccessTime(file);
                            // Add the file attachment to this e-mail message.
                            mailMessage.Attachments.Add(data);
                            // data.Dispose();
                        }
                    }
                    //exchange.metbank.co.zw                                                                      
                    smtpClient.Send(mailMessage);
                    #region stats
                    var dd = DateTime.Now.Date.ToString("ddMMyyyy");
                    var sId = dd + "_" + mail.ServiceId;
                    var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                    if (stat != null)
                    {
                        stat.SentMail += 1;
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        db.Entry(stat).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        stat = new NotificationStat();
                        stat.Id = sId;
                        stat.SentMail = 1;
                        stat.ServiceId = mail.ServiceId;
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        db.Entry(stat).State = EntityState.Added;
                        db.SaveChanges();
                    }
                    #endregion

                    #region logs
                    path = @"C:\YoLog\" + mail.SeviceName.Trim() + "" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                    var msg = "mail to:" + mail.recipient + " sent at: " + DateTime.Now;
                    if (!System.IO.File.Exists(path))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(path))
                        {
                            sw.WriteLine(msg);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(path, msg + Environment.NewLine);
                    }
                    #endregion
                    if (!string.IsNullOrEmpty(mail.Attachments))
                    {
                        char[] delimiter = new char[] { ',' };
                        string[] parts = mail.Attachments.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                        for (int i = 0; i < parts.Length; i++)
                        {
                            if (System.IO.File.Exists(parts[i]))
                            {
                                emailFile = parts[i];
                               
                                foreach (System.Net.Mail.Attachment attachment in mailMessage.Attachments)
                                {
                                    attachment.Dispose();
                                }
                                mailMessage.Attachments.Dispose();
                                mailMessage = null;
                                DeleteFile(parts[i]);
                            }
                        }
                    }
                    
                    #endregion
                }
            }
            catch (Exception ex)
            {
                var msg = "Email:" + mail.Filename + " : sent at: " + DateTime.Now + ": Failed with error :" + ex.Message;
                #region stats
                var dd = DateTime.Now.Date.ToString("ddMMyyyy");
                var sId = dd + "_" + mail.ServiceId;
                var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                if (stat != null)
                {
                    stat.Errors += 1;
                    stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                    db.Entry(stat).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    stat = new NotificationStat();
                    stat.ServiceId = mail.ServiceId;
                    stat.Id = sId;
                    stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                    stat.Errors = 1;
                    db.Entry(stat).State = EntityState.Added;
                    db.SaveChanges();
                }
                #endregion
                #region logs
                path = @"C:\YoLog\" + mail.SeviceName.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";
            
                if (!System.IO.File.Exists(path))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(path))
                    {
                        sw.WriteLine(msg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(path, msg + Environment.NewLine);
                }
                #endregion
                foreach (System.Net.Mail.Attachment attachment in mailMessage.Attachments)
                {
                    attachment.Dispose();
                }
                mailMessage.Attachments.Dispose();
                mailMessage = null; 
            }
                                    #region EndImpression
                                    //  UnloadUserProfile(tokenDuplicate, profileInfo.hProfile);

                                    // Undo impersonation
                                    //  m_ImpersonationContext.Undo();
                                }
                            }
                        }
                        else
                        {
                            uu = ("DuplicateToken() failed with error code: " + Marshal.GetLastWin32Error());
                            #region logs
                            // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                            if (!System.IO.File.Exists(path))
                            {
                                // Create a file to write to. 
                                using (StreamWriter sw = System.IO.File.CreateText(path))
                                {
                                    sw.WriteLine(uu);
                                }
                            }
                            else
                            {
                                System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                            }
                            #endregion
                            //throw new Win32Exception(Marshal.GetLastWin32Error());

                        }
                    }
                }
            }
            catch (Win32Exception we)
            {
               
            }
            catch
            {
                
                // throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
               // if (token != IntPtr.Zero) CloseHandle(token);
                //if (tokenDuplicate != IntPtr.Zero) CloseHandle(tokenDuplicate);

               
            }
          #endregion              
        }

        private static bool CertificateValidationCallBack(
                object sender,
                System.Security.Cryptography.X509Certificates.X509Certificate certificate,
                System.Security.Cryptography.X509Certificates.X509Chain chain,
                System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            // If the certificate is a valid, signed certificate, return true.
            if (sslPolicyErrors == System.Net.Security.SslPolicyErrors.None)
            {
                return true;
            }

            // If there are errors in the certificate chain, look at each error to determine the cause.
            if ((sslPolicyErrors & System.Net.Security.SslPolicyErrors.RemoteCertificateChainErrors) != 0)
            {
                if (chain != null && chain.ChainStatus != null)
                {
                    foreach (System.Security.Cryptography.X509Certificates.X509ChainStatus status in chain.ChainStatus)
                    {
                        if ((certificate.Subject == certificate.Issuer) &&
                           (status.Status == System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.UntrustedRoot))
                        {
                            // Self-signed certificates with an untrusted root are valid. 
                            continue;
                        }
                        else
                        {
                            if (status.Status != System.Security.Cryptography.X509Certificates.X509ChainStatusFlags.NoError)
                            {
                                // If there are any other errors in the certificate chain, the certificate is invalid,
                                // so the method returns false.
                                return false;
                            }
                        }
                    }
                }

                // When processing reaches this line, the only errors in the certificate chain are 
                // untrusted root errors for self-signed certificates. These certificates are valid
                // for default Exchange server installations, so return true.
                return true;
            }
            else
            {
                // In all other cases, return false.
                return false;
            }
        }
   
        #endregion
    
        public static void SendLocalSms(Sms tran)
        {
            RetailKingEntities db = new RetailKingEntities();

            try
            {
                var msg = "";
                if (tran.MSISDN != null && tran.MSISDN.Length == 10)
                {
                    tran.MSISDN = "263" + tran.MSISDN.Substring(1, tran.MSISDN.Length - 1);
                }
                else if (tran.MSISDN != null && tran.MSISDN.Length == 12)
                {
                    tran.MSISDN = tran.MSISDN;
                }
                else if (tran.MSISDN != null && tran.MSISDN.Length == 9)
                {
                    tran.MSISDN = "263" + tran.MSISDN;
                }
                #region logs
                string path = @"C:\YoLog\" + tran.Service.Trim() + "_sms" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                msg = "about to send sms";
                if (!System.IO.File.Exists(path))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(path))
                    {
                        sw.WriteLine(msg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(path, msg);
                }
                #endregion


                string uri = "http://196.29.38.126:4004/sendSMS.aspx?username=Brainstorm&password=Brainstorm123&mobile=" + tran.MSISDN + "&sender= " + tran.Sender + "&body=" + tran.Message;
                WebRequest request = HttpWebRequest.CreateHttp(uri) ;
                IWebProxy proxy = request.Proxy;
                request.Proxy = proxy;
                WebResponse respo = request.GetResponse();
                StreamReader sr = new StreamReader(respo.GetResponseStream());

                var returnvalue = sr.ReadToEnd();
                #region logs
                path = @"C:\YoLog\" + tran.Service.Trim() + "_sms" + DateTime.Now.ToString("ddMMyyy") + ".txt";
                msg = " sms response:" + returnvalue;
                if (!System.IO.File.Exists(path))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(path))
                    {
                        sw.WriteLine(msg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(path, msg);
                }
                #endregion
                if (returnvalue == "Success")
                {
                    msg = "SMS:" + tran.Filename + ": on: " + DateTime.Now + ": sent to: " + tran.MSISDN + " was Successful";
                     #region stats
                     var dd = DateTime.Now.Date.ToString("ddMMyyyy");
                     var sId = dd + "_" + tran.ServiceId;
                     var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                     if (stat != null)
                     {
                         stat.SentSms += 1;
                         stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                         db.Entry(stat).State = EntityState.Modified;
                         db.SaveChanges();
                     }
                     else
                     {
                         stat = new NotificationStat();
                         stat.Id = sId;
                         stat.SentSms = 1;
                         stat.ServiceId = tran.ServiceId;
                         stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                         db.Entry(stat).State = EntityState.Added;
                         db.SaveChanges();
                     }
                     #endregion

                     #region logs
                     path = @"C:\YoLog\" + tran.Service.Trim() + "_sms" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                     if (!System.IO.File.Exists(path))
                     {
                         // Create a file to write to. 
                         using (StreamWriter sw = System.IO.File.CreateText(path))
                         {
                             sw.WriteLine(msg);
                         }
                     }
                     else
                     {
                         System.IO.File.AppendAllText(path, msg);
                     }
                     #endregion
                }
                else
                {
                    msg = "SMS:" + tran.Filename + ": on :" + DateTime.Now + ": sent to: " + tran.MSISDN + " : Failed with error :" + returnvalue;
                     #region stats
                     var dd = DateTime.Now.Date.ToString("ddMMyyyy");
                     var sId = dd + "_" + tran.ServiceId;
                     var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                     if (stat != null)
                     {
                         stat.Errors += 1;
                         stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                         db.Entry(stat).State = EntityState.Modified;
                         db.SaveChanges();
                     }
                     else
                     {
                         stat = new NotificationStat();
                         stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                         stat.Id = sId;
                         stat.Errors = 1;
                         stat.ServiceId = tran.ServiceId;
                         db.Entry(stat).State = EntityState.Added;
                         db.SaveChanges();
                     }
                     #endregion

                     #region logs
                     path = @"C:\YoLog\" + tran.Service.Trim() + "_smsError" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                     if (!System.IO.File.Exists(path))
                     {
                         // Create a file to write to. 
                         using (StreamWriter sw = System.IO.File.CreateText(path))
                         {
                             sw.WriteLine(msg);
                         }
                     }
                     else
                     {
                         System.IO.File.AppendAllText(path, msg);
                     }
                     #endregion
                }
               
                
            }
                        

        catch (Exception g)
        {
            #region stats
            var dd = DateTime.Now.Date.ToString("ddMMyyyy");
            var sId = dd + "_" + tran.ServiceId;
            var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
            if (stat != null)
            {
                stat.Errors += 1;
                stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                db.Entry(stat).State = EntityState.Modified;
                db.SaveChanges();
            }
            else
            {
                stat = new NotificationStat();
                stat.Errors = 1;
                stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                stat.Id = sId;
                stat.ServiceId = tran.ServiceId;
                db.Entry(stat).State = EntityState.Added;
                db.SaveChanges();
            }
            #endregion

            var msg = "SMS:" + tran.Filename + ": on: " + DateTime.Now + " : sent to: " + tran.MSISDN + " : Failed with error :" + g.Message;
            #region logs
            string path = @"C:\YoLog\" + tran.Service.Trim() + "_smsError" + DateTime.Now.ToString("ddMMyyy") + ".txt";

            if (!System.IO.File.Exists(path))
            {
                // Create a file to write to. 
                using (StreamWriter sw = System.IO.File.CreateText(path))
                {
                    sw.WriteLine(msg);
                }
            }
            else
            {
                System.IO.File.AppendAllText(path, msg);
            }
            #endregion
        }
        
}

        public static string SendSms(Sms tran)
        {
            RetailKingEntities db = new RetailKingEntities();
            try
            {
                if (tran.MSISDN != null && tran.MSISDN.Length == 10)
                {
                    tran.MSISDN = "263" + tran.MSISDN.Substring(1, tran.MSISDN.Length - 1);
                }
                else if (tran.MSISDN != null && tran.MSISDN.Length == 12)
                {
                    tran.MSISDN = tran.MSISDN;
                }
                else if (tran.MSISDN != null && tran.MSISDN.Length == 9)
                {
                    tran.MSISDN = "263" + tran.MSISDN;
                }

                #region External
                RoutoSMSTelecom routo = new RoutoSMSTelecom();
                routo.SetUser("Faithwork");//0772181813
                routo.SetPass("ks4kP1w");
                routo.SetNumber(tran.MSISDN);
                routo.SetOwnNumber(tran.Sender);
                routo.SetType("0");
                routo.SetMessage(tran.Message);
               //routo.unicodeEncodeMessage();
                string header = routo.Send();


                #endregion
                var returnvalue = header;
                var msg = "";
                if (returnvalue == "Success")
                {
                    msg = "SMS:" + tran.Filename + ": on: " + DateTime.Now + ": sent to: " + tran.MSISDN + " was Successful";
                    #region stats
                    string dd = DateTime.Now.Date.ToString("ddMMyyyy");
                    var sId = dd + "_" + tran.ServiceId;
                    var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                    if (stat != null)
                    {
                        stat.SentSms += 1;
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        db.Entry(stat).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        stat = new NotificationStat();
                        stat.Id = sId;
                        stat.SentSms = 1;
                        stat.ServiceId = tran.ServiceId;
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        db.Entry(stat).State = EntityState.Added;
                        db.SaveChanges();
                    }
                    #endregion

                    #region logs
                    string path = @"C:\YoLog\" + tran.Service.Trim() + "_sms" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                    if (!System.IO.File.Exists(path))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(path))
                        {
                            sw.WriteLine(msg);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(path, msg + Environment.NewLine);
                    }
                    #endregion
                }
                else
                {
                    msg = "SMS:" + tran.Filename + ": on :" + DateTime.Now + ": sent to: " + tran.MSISDN + " : Failed with error :" + returnvalue;
                    #region stats
                    var dd = DateTime.Now.Date.ToString("ddMMyyyy");
                    var sId = dd + "_" + tran.ServiceId;
                    var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                    if (stat != null)
                    {
                        stat.Errors += 1;
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        db.Entry(stat).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        stat = new NotificationStat();
                        stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                        stat.Id = sId;
                        stat.Errors = 1;
                        stat.ServiceId = tran.ServiceId;
                        db.Entry(stat).State = EntityState.Added;
                        db.SaveChanges();
                    }
                    #endregion

                    #region logs
                    string path = @"C:\YoLog\" + tran.Service.Trim() + "_smsError" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                    if (!System.IO.File.Exists(path))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(path))
                        {
                            sw.WriteLine(msg);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(path, msg + Environment.NewLine);
                    }
                    #endregion
                }

                return returnvalue;
            }


            catch (Exception g)
            {
                #region stats
                var dd = DateTime.Now.Date.ToString("ddMMyyyy");
                var sId = dd + "_" + tran.ServiceId;
                var stat = db.NotificationStats.Where(u => u.Id == sId).FirstOrDefault();
                if (stat != null)
                {
                    stat.Errors += 1;
                    stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                    db.Entry(stat).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    stat = new NotificationStat();
                    stat.Errors = 1;
                    stat.Date = DateTime.Now.Date.ToString("ddMMyyyy");
                    stat.Id = sId;
                    stat.ServiceId = tran.ServiceId;
                    db.Entry(stat).State = EntityState.Added;
                    db.SaveChanges();
                }
                #endregion

                var msg = "SMS:" + tran.Filename + ": on: " + DateTime.Now + " : sent to: " + tran.MSISDN + " : Failed with error :" + g.Message;
                #region logs
                string path = @"C:\YoLog\" + tran.Service.Trim() + "_smsError" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                if (!System.IO.File.Exists(path))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(path))
                    {
                        sw.WriteLine(msg);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(path, msg + Environment.NewLine);
                }
                #endregion
                return "error";
            }
        }

        public static void DeleteFile(string filepath)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            RetailKingEntities db = new RetailKingEntities();
             #region Impersonate
            WindowsIdentity m_ImpersonatedUser;
            IntPtr token = IntPtr.Zero;
            IntPtr tokenDuplicate = IntPtr.Zero;
            const int SecurityImpersonation = 2;
            const int TokenType = 1;

            try
            {
                if (RevertToSelf())
                {

                    String userName = "TempUser";

                    var du = db.DomainUsers.FirstOrDefault();
                    IntPtr password = GetPassword(du.Password.Trim());
                    IntPtr logonToken = LogonUser();
                    IntPtrConstructor(logonToken);
                    IntPtrStringConstructor(logonToken);
                    IntPtrStringTypeConstructor(logonToken);
                    IntPrtStringTypeBoolConstructor(logonToken);

                    // Property implementations.
                    UseProperties(logonToken);

                    // Method implementations.

                    ImpersonateIdentity(logonToken);
                    var uu = ("Before impersonation: " +
                                     WindowsIdentity.GetCurrent().Name);
                    #region logs
                    string path = @"C:\YoLog\" + "Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                    if (!System.IO.File.Exists(path))
                    {
                        // Create a file to write to. 
                        using (StreamWriter sw = System.IO.File.CreateText(path))
                        {
                            sw.WriteLine(uu);
                        }
                    }
                    else
                    {
                        System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                    }
                    #endregion
                    var lon = LogonUser(du.Name.Trim(), du.Domain.Trim(), du.Password.Trim(), LOGON32_LOGON_INTERACTIVE,
                                  LOGON32_PROVIDER_DEFAULT, ref token);
                    if (lon != 0)
                    {
                        if (DuplicateToken(token, SecurityImpersonation, ref tokenDuplicate) != 0)
                        {
                            m_ImpersonatedUser = new WindowsIdentity(tokenDuplicate);
                            using (m_ImpersonationContext = m_ImpersonatedUser.Impersonate())
                            {
                                if (m_ImpersonationContext != null)
                                {
                                    uu = ("After Impersonation succeeded: " + Environment.NewLine +
                                                      "User Name: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).Name +
                                                      Environment.NewLine +
                                                      "SID: " +
                                                      WindowsIdentity.GetCurrent(TokenAccessLevels.MaximumAllowed).User.
                                                          Value);
                                    #region logs
                                    // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                    if (!System.IO.File.Exists(path))
                                    {
                                        // Create a file to write to. 
                                        using (StreamWriter sw = System.IO.File.CreateText(path))
                                        {
                                            sw.WriteLine(uu);
                                        }
                                    }
                                    else
                                    {
                                        System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                    }
                                    #endregion
                                    #region LoadUserProfile
                                    // Load user profile
                                    ProfileInfo profileInfo = new ProfileInfo();
                                    profileInfo.dwSize = Marshal.SizeOf(profileInfo);
                                    profileInfo.lpUserName = userName;
                                    profileInfo.dwFlags = 1;
                                    Boolean loadSuccess = LoadUserProfile(tokenDuplicate, ref profileInfo);

                                    if (!loadSuccess)
                                    {
                                        uu = ("LoadUserProfile() failed with error code: " +
                                                          Marshal.GetLastWin32Error());
                                        //throw new Win32Exception(Marshal.GetLastWin32Error());
                                        #region logs
                                        //string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                        if (!System.IO.File.Exists(path))
                                        {
                                            // Create a file to write to. 
                                            using (StreamWriter sw = System.IO.File.CreateText(path))
                                            {
                                                sw.WriteLine(uu);
                                            }
                                        }
                                        else
                                        {
                                            System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                        }
                                        #endregion
                                    }

                                    if (profileInfo.hProfile == IntPtr.Zero)
                                    {
                                        uu = (
                                             "LoadUserProfile() failed - HKCU handle was not loaded. Error code: " +
                                             Marshal.GetLastWin32Error());
                                        #region logs
                                        // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                                        if (!System.IO.File.Exists(path))
                                        {
                                            // Create a file to write to. 
                                            using (StreamWriter sw = System.IO.File.CreateText(path))
                                            {
                                                sw.WriteLine(uu);
                                            }
                                        }
                                        else
                                        {
                                            System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                                        }
                                        #endregion
                                    }
                                    #endregion

                                   // CloseHandle(token);
                                   // CloseHandle(tokenDuplicate);

            #endregion

                try
                {
                    System.IO.File.Delete(filepath);
                }
                catch (System.IO.IOException e)
                {

                }
                                  
             #region EndImpression
                                   //   UnloadUserProfile(tokenDuplicate, profileInfo.hProfile);

                                    //Undo impersonation
                                   //  m_ImpersonationContext.Undo();
                                }
                            }
                        }
                        else
                        {
                            uu = ("DuplicateToken() failed with error code: " + Marshal.GetLastWin32Error());
                            #region logs
                            // string path = @"C:\YoLog\" + city.Name.Trim() + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

                            if (!System.IO.File.Exists(path))
                            {
                                // Create a file to write to. 
                                using (StreamWriter sw = System.IO.File.CreateText(path))
                                {
                                    sw.WriteLine(uu);
                                }
                            }
                            else
                            {
                                System.IO.File.AppendAllText(path, uu + Environment.NewLine);
                            }
                            #endregion
                            throw new Win32Exception(Marshal.GetLastWin32Error());

                        }
                    }
                }
            }
            catch (Win32Exception we)
            {
               // throw we;
            }
            catch
            {
              //  throw new Win32Exception(Marshal.GetLastWin32Error());
            }
            finally
            {
               // if (token != IntPtr.Zero) CloseHandle(token);
               // if (tokenDuplicate != IntPtr.Zero) CloseHandle(tokenDuplicate);

                Console.WriteLine("After finished impersonation: " + WindowsIdentity.GetCurrent().Name);
            }
                                    #endregion
           
        }

        #endregion

        #region ErrorService
        [HttpGet]
        public ActionResult ResendMail(string service, string message)
        {
            try
            {
                char[] delim = new char[] { ':' };
                string[] par = message.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                
                RetailKingEntities db = new RetailKingEntities();
                var line = "";
                var fileName = par[2];
                var serv = db.MailingServices.Find(long.Parse(service));
                #region filepath
                char[] deli = new char[] { '/' };
                string[] parz = serv.ListiningFolder.Trim().Split(deli, StringSplitOptions.RemoveEmptyEntries);

                string directory;
                if (parz.Length == 2)
                {
                    directory = parz[0] + @"\" + parz[1];
                }
                else
                {
                    directory = @"C:\" + parz[0];
                }
                #endregion

                #region Check retry
                string rpath = "";
                var dr = par[4].Substring(0, par[4].Length - 2);
                var dd = DateTime.Parse(dr).ToString("ddMMyyyy");
                if (par[1].Trim() == "SMS")
                {

                    rpath = @"C:\YoLog\" + serv.Name.Trim() + "_smsRetry" + dd + ".txt";
                }
                else
                {
                    rpath = @"C:\YoLog\" + serv.Name.Trim() + "_Retry" + dd + ".txt";
                }
                try
                {
                    var file = new System.IO.StreamReader(rpath);
                    while ((line = file.ReadLine()) != null)
                    {
                        if(par[0] == line)
                        {
                            return Json("This retry has been done before", JsonRequestBehavior.AllowGet);
                        }
                    }
                    file.Close();
                }
                catch { }

                #region logs

                if (!System.IO.File.Exists(rpath))
                {
                    // Create a file to write to. 
                    using (StreamWriter sw = System.IO.File.CreateText(rpath))
                    {
                        sw.WriteLine(par[0]);
                    }
                }
                else
                {
                    System.IO.File.AppendAllText(rpath, par[0]);
                }
                #endregion

                #endregion

                #region File Name
                string[] partz;
                if (serv.SendingDetails.Trim() == "File Name")
                {
                    char[] delimita = serv.EmailSeparator.Trim().ToCharArray();
                    partz = fileName.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                    #region Has Sms
                    if (serv.HasSms == true)
                    {
                        if (serv.HasMailingList.Trim() == "Yes")
                        {
                            var mlist = db.MailLists.Where(u => u.ServiceId == serv.Id && u.ListType.Trim() == "Sms");

                            foreach (var item in mlist)
                            {
                                var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                string[] emaillist = item.EmailList.Split(',');
                                foreach (var itm in emaillist)
                                {
                                    var sms = new Sms();
                                    sms.MSISDN = itm;
                                    sms.Message = temp.SmsMessage;
                                    sms.Sender = serv.Company.Trim();
                                    sms.Service = temp.Service;
                                    sms.ServiceId = serv.Id;
                                    sms.Filename = par[2];
                                    SendSms(sms);
                                }
                            }
                        }
                        else
                        {
                            char[] delimiter = new char[] { '.' };
                            string[] msisdn = partz[(int)serv.SmsPosition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            string[] msg = partz[(int)serv.SmsMessagePosition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            if (serv.HasTemplate == true)
                            {
                                var temp = db.MailTemplates.Where(u => u.ServiceId == serv.Id).FirstOrDefault();

                                var sms = new Sms();
                                sms.MSISDN = msisdn[0];
                                sms.Message = temp.SmsMessage;
                                sms.Sender = serv.Company.Trim();
                                sms.Service = temp.Service;
                                sms.ServiceId = serv.Id;
                                sms.Filename = par[2];
                                SendSms(sms);
                            }
                            else
                            {
                                var sms = new Sms();
                                sms.MSISDN = msisdn[0];
                                sms.Message = msg[0];
                                sms.Sender = serv.Company.Trim();
                                sms.Service = serv.Name;
                                sms.ServiceId = serv.Id;
                                sms.Filename = par[2];
                                SendSms(sms);
                            }
                        }
                    }
                    #endregion

                    #region Has Email
                    if (serv.HasEmail == true)
                    {
                        var ext = "";
                        char[] delimiter = new char[] { '.' };
                        string[] parts = partz[(int)serv.StartPossition].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        var len = parts.Length;
                        if ((int)serv.StartPossition == partz.Length - 1)
                        {
                            ext = parts[len - 1];
                        }
                        else
                        {
                            len = parts.Length + 1;
                        }
                        string regExp = "[^a-zA-Z0-9]";
                        if (len > 2)
                        {
                            fileName = "";
                            for (int i = 0; i < len - 1; i++)
                            {
                                if (i == 0)
                                {
                                    fileName = parts[i];
                                }
                                else
                                {
                                    fileName = fileName + "." + parts[i];
                                }
                            }
                        }
                        else
                        {
                            fileName = parts[0];
                        }

                        if (serv.HasMailingList.Trim() == "Yes")
                        {
                            var mlist = db.MailLists.Where(u => u.ServiceId == serv.Id && u.ListType.Trim() == "Email");

                            foreach (var item in mlist)
                            {
                                var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                string[] emaillist = item.EmailList.Split(',');
                                foreach (var itm in emaillist)
                                {
                                    Mail mail = new Mail();
                                    mail.body = temp.Message;
                                    mail.Subject = temp.Subject;
                                    mail.Sender = temp.EmailAddress;
                                    mail.Password = temp.Password;
                                    mail.recipient = itm;
                                    mail.Attachments = @"C:\" + serv.ListiningFolder + "\\" + par[2];
                                    mail.ServiceId = serv.Id;
                                    mail.Filename = par[2];
                                    mail.SeviceName = serv.Name;
                                    SendMail(mail);
                                }
                            }
                        }
                        else
                        {
                            var temp = db.MailTemplates.Where(u => u.ServiceId == serv.Id).FirstOrDefault();
                            if (temp != null)
                            {
                                Mail mail = new Mail();
                                mail.body = temp.Message;
                                mail.Subject = temp.Subject;
                                mail.Sender = temp.EmailAddress;
                                mail.Password = temp.Password;
                                mail.recipient = fileName;
                                 mail.Attachments = @"C:\" + serv.ListiningFolder + "\\" + par[2];
                                mail.ServiceId = serv.Id;
                                mail.Filename = par[2];
                                mail.SeviceName = serv.Name;
                                SendMail(mail);
                            }
                        }
                    }
                    #endregion
                }
                #endregion

                #region text File
                if (serv.SendingDetails.Trim() == "Text File")
                {
                    var FullPath = directory + "\\" + fileName;
                    var file = new System.IO.StreamReader(FullPath);
                    while ((line = file.ReadLine()) != null)
                    {
                        char[] delimita = serv.EmailSeparator.Trim().ToCharArray();
                        partz = line.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                        #region Send Sms
                        if (serv.HasMailingList.Trim() == "Yes")
                        {
                            var mlist = db.MailLists.Where(u => u.ServiceId == serv.Id && u.ListType.Trim() == "Sms");

                            foreach (var item in mlist)
                            {
                                var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                string[] emaillist = item.EmailList.Split(',');
                                foreach (var itm in emaillist)
                                {
                                    var sms = new Sms();
                                    sms.MSISDN = itm;
                                    sms.Message = temp.SmsMessage;
                                    sms.Sender = serv.Company.Trim();
                                    sms.Service = temp.Service;
                                    sms.ServiceId = serv.Id;
                                    sms.Filename = par[2];
                                    SendSms(sms);
                                }
                            }
                        }
                        else
                        {
                            if (serv.HasTemplate == true)
                            {
                                var temp = db.MailTemplates.Where(u => u.ServiceId == serv.Id).FirstOrDefault();

                                var sms = new Sms();
                                sms.MSISDN = partz[(int)serv.SmsPosition];
                                sms.Message = temp.SmsMessage;
                                sms.Sender = serv.Company.Trim();
                                sms.Service = temp.Service;
                                sms.ServiceId = serv.Id;
                                sms.Filename = par[2];
                                SendSms(sms);
                            }
                            else
                            {
                                var sms = new Sms();
                                sms.MSISDN = partz[(int)serv.SmsPosition];
                                sms.Message = partz[(int)serv.SmsMessagePosition];
                                sms.Sender = serv.Company.Trim();
                                sms.Service = serv.Name;
                                sms.ServiceId = serv.Id;
                                sms.Filename = par[2];
                                SendSms(sms);
                            }
                        }
                        #endregion

                        #region Has Email
                        if (serv.HasEmail == true)
                        {
                            fileName = partz[(int)serv.EmailMessagePosition];

                            if (serv.HasMailingList.Trim() == "Yes")
                            {
                                var mlist = db.MailLists.Where(u => u.ServiceId == serv.Id && u.ListType.Trim() == "Email");

                                foreach (var item in mlist)
                                {
                                    var temp = db.MailTemplates.Where(u => u.GroupId == item.Id).FirstOrDefault();
                                    string[] emaillist = item.EmailList.Split(',');
                                    foreach (var itm in emaillist)
                                    {
                                        Mail mail = new Mail();
                                        mail.body = temp.Message;
                                        mail.Subject = temp.Subject;
                                        mail.Sender = temp.EmailAddress;
                                        mail.Password = temp.Password;
                                        mail.recipient = itm;
                                         mail.Attachments = @"C:\" + serv.ListiningFolder + "\\" + par[2];
                                        mail.ServiceId = serv.Id;
                                        mail.Filename = par[2];
                                        mail.SeviceName = serv.Name;
                                        SendMail(mail);
                                    }
                                }
                            }
                            else if (serv.HasTemplate == true)
                            {
                                var temp = db.MailTemplates.Where(u => u.ServiceId == serv.Id).FirstOrDefault();
                                if (temp != null)
                                {
                                    Mail mail = new Mail();
                                    mail.body = temp.Message;
                                    mail.Subject = temp.Subject;
                                    mail.Sender = temp.EmailAddress;
                                    mail.Password = temp.Password;
                                    mail.recipient = fileName;
                                    mail.Attachments = @"C:\" + serv.ListiningFolder + "\\" + par[2];
                                    mail.ServiceId = serv.Id;
                                    mail.Filename = par[2];
                                    mail.SeviceName = serv.Name;
                                    SendMail(mail);
                                }
                            }
                            else
                            {
                                var ms = db.MailSettings.FirstOrDefault();
                                Mail mail = new Mail();
                                mail.body = partz[(int)serv.EmailMessagePosition];
                                mail.Subject = fileName;
                                mail.Sender = ms.EmailAddress;
                                mail.Password = ms.Password;
                                mail.recipient = partz[(int)serv.EmailMessagePosition];
                                mail.Attachments = @"C:\" + serv.ListiningFolder + "\\" + par[2];
                                mail.ServiceId = serv.Id;
                                mail.Filename = par[2];
                                mail.SeviceName = serv.Name;
                                SendMail(mail);
                            }
                        }
                        #endregion

                    }
                }
                #endregion

            }
            catch { }
            return Json("Retry sent check todays logs for response", JsonRequestBehavior.AllowGet);
        }
        [HttpGet]
        public ActionResult GetMaillogs(string type, DateTime date, string service)
        {
            List<string> errs = new List<string>();
            string path = "";
            string Smspath = "";
            string line = "";
            var dd = "";
            if (date == null)
            {
                dd = DateTime.Now.Date.ToString("ddMMyyyy");
            }
            else
            {
                dd = date.Date.ToString("ddMMyyyy");
            }
            ViewData["ServiceId"] = service.Trim();
            var sv = db.MailingServices.Find(long.Parse(service.Trim()));
            ViewData["Service"] = sv.Name.Trim();
            if (type == "Error" || type == "Errors")
            {
                path = @"C:\YoLog\" + sv.Name.Trim() + "_Error" + dd + ".txt";
                Smspath = @"C:\YoLog\" + sv.Name.Trim() + "_smsError" + dd + ".txt";
            }
            else
            {
                path = @"C:\YoLog\" + sv.Name.Trim() + "" + dd + ".txt";

            }
            //emails
            try
            {
                var file = new System.IO.StreamReader(path);
                var cnt = 0;
                while ((line = file.ReadLine()) != null)
                {
                    errs.Add(cnt + ": " + line);
                    cnt += 1;
                }

            }
            catch { }
            try
            {
                var file = new System.IO.StreamReader(Smspath);
                var cnt = 0;
                while ((line = file.ReadLine()) != null)
                {
                    errs.Add(cnt + ": " + line);
                    cnt += 1;
                }
                file.Close();
                file.Dispose();
            }
            catch { }

            if (type == "Error" || type == "Errors")
           {
               return PartialView("GetErrorLogs",errs);
           }
           else
           {
               return PartialView(errs);
           }

        }
  
        [HttpGet]
        public ActionResult GetSmslogs(string type, DateTime date, string service)
        {
            List<string> errs = new List<string>();
            string path = "";
            string Smspath = "";
            string line = "";
            var dd = "";
            if (date == null)
            {
                dd = DateTime.Now.Date.ToString("ddMMyyyy");
            }
            else
            {
                dd = date.Date.ToString("ddMMyyyy");
            }
            ViewData["ServiceId"] = service.Trim();
            var sv = db.MailingServices.Find(long.Parse(service.Trim()));
            ViewData["Service"] = sv.Name.Trim();
            if (type == "Error")
            {
                path = @"C:\YoLog\" + sv.Name.Trim() + "_Error" + dd + ".txt";
                Smspath = @"C:\YoLog\" + sv.Name.Trim() + "_smsError" + dd + ".txt";
            }
            else
            {
                path = @"C:\YoLog\" + sv.Name.Trim() + "_sms" + dd + ".txt";

            }
            //emails
            try
            {
                var file = new System.IO.StreamReader(path);
                var cnt = 0;
                while ((line = file.ReadLine()) != null)
                {
                    errs.Add(cnt + ": " + line);
                    cnt += 1;
                }
                file.Close();
                file.Dispose();
            }
            catch { }
            try
            {
                var file = new System.IO.StreamReader(Smspath);

                while ((line = file.ReadLine()) != null)
                {
                    errs.Add(line);
                }

            }
            catch { }

           

            return PartialView(errs);
        }
    
        #endregion

        #region ProxySettings
        public ActionResult ProxySettings(string company)
        {
            var pp = db.ProxySettings.FirstOrDefault();
            if(pp == null)
            {
                pp = new ProxySetting();
                pp.IpAddress = "0.0.0.0";
                pp.Port = "00";
                pp.Id = 0;
            }
            ViewData["Success"] = "null";
            return PartialView(pp);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ProxySettings(ProxySetting city)
        {
            if (ModelState.IsValid)
            {
                if (city.Id == 0)
                {
                    db.ProxySettings.Add(city);
                    db.SaveChanges();
                }
                else
                {
                    db.Entry(city).State = EntityState.Modified;
                    db.SaveChanges();
                }
                ViewData["Success"] = "Success";
                return PartialView(city);
            }
            ViewData["Success"] = "Error";
            return PartialView(city);
        }
        
        #endregion

        private static IntPtr GetPassword(string pw)
        {
            IntPtr password = IntPtr.Zero;

            using (SecureString secureString = new SecureString())
            {
                foreach (char c in pw)
                    secureString.AppendChar(c);

                // Lock the password down
                secureString.MakeReadOnly();

                password = Marshal.SecureStringToBSTR(secureString);
            }

            return password;
        }

        private static IntPtr LogonUser()
        {
            IntPtr accountToken = WindowsIdentity.GetCurrent().Token;
            Console.WriteLine("Token number is: " + accountToken.ToString());

            return accountToken;
        }

        #region newimp
        private static void IntPtrConstructor(IntPtr logonToken)
        {
            // Construct a WindowsIdentity object using the input account token.
            WindowsIdentity windowsIdentity = new WindowsIdentity(logonToken);

            Console.WriteLine("Created a Windows identity object named " +
                windowsIdentity.Name + ".");
        }


        // Create a WindowsIdentity object for the user represented by the
        // specified account token and authentication type.
        private static void IntPtrStringConstructor(IntPtr logonToken)
        {
            // Construct a WindowsIdentity object using the input account token 
            // and the specified authentication type.
            string authenticationType = "WindowsAuthentication";
            WindowsIdentity windowsIdentity =
                            new WindowsIdentity(logonToken, authenticationType);

            Console.WriteLine("Created a Windows identity object named " +
                windowsIdentity.Name + ".");
        }

        // Create a WindowsIdentity object for the user represented by the
        // specified account token, authentication type, and Windows account
        // type.
        private static void IntPtrStringTypeConstructor(IntPtr logonToken)
        {
            // Construct a WindowsIdentity object using the input account token,
            // and the specified authentication type, and Windows account type.
            string authenticationType = "WindowsAuthentication";
            WindowsAccountType guestAccount = WindowsAccountType.Guest;
            WindowsIdentity windowsIdentity =
                new WindowsIdentity(logonToken, authenticationType, guestAccount);

            Console.WriteLine("Created a Windows identity object named " +
                windowsIdentity.Name + ".");
        }

        // Create a WindowsIdentity object for the user represented by the
        // specified account token, authentication type, Windows account type, and
        // Boolean authentication flag.
        private static void IntPrtStringTypeBoolConstructor(IntPtr logonToken)
        {
            // Construct a WindowsIdentity object using the input account token,
            // and the specified authentication type, Windows account type, and
            // authentication flag.
            string authenticationType = "WindowsAuthentication";
            WindowsAccountType guestAccount = WindowsAccountType.Guest;
            bool isAuthenticated = true;
            WindowsIdentity windowsIdentity = new WindowsIdentity(
                logonToken, authenticationType, guestAccount, isAuthenticated);

            Console.WriteLine("Created a Windows identity object named " +
                windowsIdentity.Name + ".");
        }
        // Access the properties of a WindowsIdentity object.
        private static void UseProperties(IntPtr logonToken)
        {
            WindowsIdentity windowsIdentity = new WindowsIdentity(logonToken);
            string propertyDescription = "The Windows identity named ";

            // Retrieve the Windows logon name from the Windows identity object.
            propertyDescription += windowsIdentity.Name;

            // Verify that the user account is not considered to be an Anonymous
            // account by the system.
            if (!windowsIdentity.IsAnonymous)
            {
                propertyDescription += " is not an Anonymous account";
            }

            // Verify that the user account has been authenticated by Windows.
            if (windowsIdentity.IsAuthenticated)
            {
                propertyDescription += ", is authenticated";
            }

            // Verify that the user account is considered to be a System account
            // by the system.
            if (windowsIdentity.IsSystem)
            {
                propertyDescription += ", is a System account";
            }
            // Verify that the user account is considered to be a Guest account
            // by the system.
            if (windowsIdentity.IsGuest)
            {
                propertyDescription += ", is a Guest account";
            }

            // Retrieve the authentication type for the 
            String authenticationType = windowsIdentity.AuthenticationType;

            // Append the authenication type to the output message.
            if (authenticationType != null)
            {
                propertyDescription += (" and uses " + authenticationType);
                propertyDescription += (" authentication type.");
            }

            Console.WriteLine(propertyDescription);

            // Display the SID for the owner.
            Console.Write("The SID for the owner is : ");
            SecurityIdentifier si = windowsIdentity.Owner;
            Console.WriteLine(si.ToString());
            // Display the SIDs for the groups the current user belongs to.
            Console.WriteLine("Display the SIDs for the groups the current user belongs to.");
            IdentityReferenceCollection irc = windowsIdentity.Groups;
            foreach (IdentityReference ir in irc)
                Console.WriteLine(ir.Value);
            TokenImpersonationLevel token = windowsIdentity.ImpersonationLevel;
            Console.WriteLine("The impersonation level for the current user is : " + token.ToString());
        }
        private static void ImpersonateIdentity(IntPtr logonToken)
        {
            // Retrieve the Windows identity using the specified token.
            WindowsIdentity windowsIdentity = new WindowsIdentity(logonToken);

            // Create a WindowsImpersonationContext object by impersonating the
            // Windows identity.
            WindowsImpersonationContext impersonationContext =
                windowsIdentity.Impersonate();

            var uu = ("Name of the identity after impersonation: "
                + WindowsIdentity.GetCurrent().Name + ".");
            Console.WriteLine(windowsIdentity.ImpersonationLevel);
            // Stop impersonating the user.
           // impersonationContext.Undo();
            
            #region logs
            string path = @"C:\YoLog\" +"Impersonate" + "_Error" + DateTime.Now.ToString("ddMMyyy") + ".txt";

            if (!System.IO.File.Exists(path))
            {
                // Create a file to write to. 
                using (StreamWriter sw = System.IO.File.CreateText(path))
                {
                    sw.WriteLine(uu);
                }
            }
            else
            {
                System.IO.File.AppendAllText(path, uu + Environment.NewLine);
            }
            #endregion
            // Check the identity name.
           
        }

        #endregion

        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
        
    }

}