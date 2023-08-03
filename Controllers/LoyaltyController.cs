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
using RetailKing.RavendbClasses;
using System.Web.Script.Serialization;
using System.Net.Mail;
using System.Threading;
using System.IO;
using Newtonsoft.Json;

namespace RetailKing.Controllers
{
    [Authorize]
    public class LoyaltyController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
         XmlCustomers cus = new XmlCustomers();
         XmlSupplier sup = new XmlSupplier();
         XmlTransactionLogs trnl = new XmlTransactionLogs();
        //
         // GET: /Customers/
        #region Customer
        public ActionResult CustomerLoyalty(string Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if(string.IsNullOrEmpty(Id))
            {
                Id = part[0];
            }
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            #region get branches
            var supl = db.Suppliers.Where(u => u.AccountCode == Id).FirstOrDefault();
            if (supl.ParentId == null)
            {
                var suply = db.Suppliers.Where(u => u.ParentId == Id).ToList();
                if (suply.Count > 0)
                {
                    List<SelectListItem> sr = new List<SelectListItem>();
                    sr.Add(new SelectListItem
                    {
                        Text = "ALL",
                        Value = "ALL"
                    });
                    foreach (var item in suply)
                    {
                        sr.Add(new SelectListItem
                        {
                            Text = item.Branch.Trim(),
                            Value = item.Branch.Trim()
                        });
                    }

                    ViewBag.Supplier = sr;
                }
            }
            else
            {
                List<SelectListItem> sr = new List<SelectListItem>();
                sr.Add(new SelectListItem
                {
                    Text = supl.Branch.Trim(),
                    Value = supl.Branch.Trim()
                });
                ViewBag.Supplier = sr;
            }

            #endregion
            var ss = db.Suppliers.Where(u => u.AccountCode == Id).FirstOrDefault();
            if (ss.ParentId != null)
            {
                Id = ss.ParentId;
            }
            var pp = sup.GetAllLoyalCustomers(Id);
            if(ss.ParentId != null )
            {
                pp.members = pp.members.Where(u => u.Branch == ss.Branch.Trim()).ToList();
            }
            var px = new List<Loyalty>();
            if (pp != null) px = pp.members.OrderByDescending(u => u.Points).ToList();;

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
            ViewData["Branch"] = "ALL";
            ViewData["Search"] = "";
            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);
          
        }
        [HttpPost]
        public ActionResult CustomerLoyalty(string Id, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (string.IsNullOrEmpty(Id))
            {
                Id = part[0];
            }
          
            var ss = db.Suppliers.Where(u => u.AccountCode == Id).FirstOrDefault();

            #region get branches
            //var supl = db.Suppliers.Where(u => u.AccountCode == Id).FirstOrDefault();
            if (ss.ParentId == null)
            {
                var suply = db.Suppliers.Where(u => u.ParentId == Id).ToList();
                if (suply.Count > 0)
                {
                    List<SelectListItem> sr = new List<SelectListItem>();
                    sr.Add(new SelectListItem
                    {
                        Text = "ALL",
                        Value = "ALL"
                    });
                    foreach (var item in suply)
                    {
                        sr.Add(new SelectListItem
                        {
                            Text = item.Branch.Trim(),
                            Value = item.Branch.Trim()
                        });
                    }

                    ViewBag.Supplier = sr;
                }
            }
            else
            {
                List<SelectListItem> sr = new List<SelectListItem>();
                sr.Add(new SelectListItem
                {
                    Text = ss.Branch.Trim(),
                    Value = ss.Branch.Trim()
                });
                ViewBag.Supplier = sr;
            }

            #endregion

            if (ss.ParentId != null)
            {
                Id = ss.ParentId;
            }
            var pp = sup.GetAllLoyalCustomers(Id);
            if (ss.ParentId != null)
            {
                pp.members = pp.members.Where(u => u.Branch == ss.Branch.Trim()).ToList();
            }
            if (!string.IsNullOrEmpty(param.sColumns) && param.sColumns != "ALL")
            {
                pp.members =  pp.members.Where(u => u.Branch.ToLower()== param.sColumns.ToLower()).ToList();
            }
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                pp.members =  pp.members.Where(u => u.PhoneNumber.Contains(param.sSearch.Trim()) || u.Id.Contains(param.sSearch.Trim())).ToList();
            }

            var px = new List<Loyalty>();
            if (pp != null) px = pp.members.OrderByDescending(u => u.Points).ToList();

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
            ViewData["Branch"] = param.sColumns;
            ViewData["Search"] = param.sSearch;
            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);

        }
       
        public ActionResult Create(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var pz = np.GetAllCustomers();
            var pp = (from e in pz select e).LastOrDefault();
            Customer px = new Customer();
            if (pp == null)
            {
                pp = new Customer();
                px.AccountCode = "8-0001-0000001";
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
                px.CompanyId = Company;
            }
            return PartialView(px);
        }

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            ServiceBiling sb = new ServiceBiling();
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    string SupplierId = part[0];
                    var service = db.YomoneyServices.Where(u => u.Name.Trim() == "NEW CUSTOMER").FirstOrDefault();
                    string[] nam = customer.ContactPerson.Split(' ');
                    var y = (from e in db.Customers where nam.Any(u => e.ContactPerson.ToLower().Contains(customer.ContactPerson.ToLower())) || e.Phone1 == customer.Phone1 select e).ToList();
                    if (y != null && y.Count == 0)
                    {
                        ST_encrypt en = new ST_encrypt();

                        var email = customer.Email;
                        string newAccount = en.encrypt(customer.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
                        Random rr = new Random();
                        var pass = rr.Next(00000, 99999).ToString();
                        string Pin = en.encrypt(pass, "214ea9d5bda5277f6d1b3a3c58fd7034");
                        try
                        {
                            var dd = db.Suppliers.Where(u => u.AccountCode.Trim() == SupplierId).FirstOrDefault();

                            string bracc = "";
                            string branch = "";
                            if (dd.ParentId != null)
                            {
                                bracc = dd.ParentId;
                                branch = SupplierId;
                            }
                            else
                            {
                                bracc = SupplierId;
                            }
                            var accs = db.Customers.Where(u => u.Phone1.Trim() == customer.Phone1.Trim()).ToList();
                            var sch = db.LoyaltySchemes.Where(u => u.SupplierId == bracc).FirstOrDefault();
                            if (accs == null || accs.Count == 0)
                            {
                                customer.Email = newAccount;
                                customer.Password = Pin;
                                customer.CustomerName = customer.CustomerName.ToUpper();
                                customer.Balance = 0;
                                customer.Wallet = 0;
                                customer.Purchases = 0;
                                customer.PurchasesToDate = 0;
                                customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                                //np.SaveOrUpdateCustomers(customer);
                                db.Customers.Add(customer);
                                db.SaveChanges();
                                // var cus = new XmlCustomers();
                                cus.CreateCustomerProfile(customer);


                                Loyalty loy = new Loyalty();
                                loy.Id = customer.ID.ToString();
                                loy.PhoneNumber = customer.Phone1;
                                loy.Name = customer.CustomerName.ToUpper();
                                loy.Points = 0;
                                loy.SupplierId = bracc;
                                loy.SupplierName = dd.SupplierName.Trim();
                                loy.Branch = dd.Branch.Trim();
                                loy.Id = customer.AccountCode;
                                loy.SchemeId = sch.Id;
                                loy.Email = email;
                                loy.ContactPerson = customer.ContactPerson;

                                //XmlCustomers cus = new XmlCustomers();

                                var px = cus.CreateCustomerLoyalty(loy);
                                #region External
                                var resp = sb.Bill(3, bracc, 0, branch);
                                if (resp != "11102")
                                {
                                    try
                                    {
                                        RoutoSMSTelecom routo = new RoutoSMSTelecom();
                                        routo.SetUser("Faithwork");//0772181813
                                        routo.SetPass("ks4kP1w");
                                        routo.SetNumber(customer.Phone1);
                                        routo.SetOwnNumber(dd.SupplierName);
                                        routo.SetType("0");

                                        routo.SetMessage("Welcome to GAIN Cash & Carry Super Gains reward scheme!! Keep buying from GAIN and stand to get great prizes! Tractors, Mini Buses, 3ton truck up for grabs!");
                                        //routo.unicodeEncodeMessage();
                                        string header = routo.SendDirect();
                                    }
                                    catch (Exception)
                                    {

                                    }

                                }
                                #endregion

                                #region TransactionLog
                                Tranlog log = new Tranlog();
                                log.DestinationAccountNumber = customer.AccountCode;
                                log.SourceAccountNumber = branch;
                                log.Description = "New Account Creation";
                                log.ParentId = bracc;
                                log.Amount = 0;
                                log.TransactionTotal = 0;
                                log.Username = part[1];
                                log.Status = "Pending";
                                log.TerminalId = "WEB";
                                log.TransactionCharge = decimal.Parse("0.00");
                                log.Commision = decimal.Parse("0.00");
                                log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
                                log.ServiceId = "NEW CUSTOMER";
                                log.TransactionType = service.Id;
                                log.BranchName = dd.Branch;
                                trnl.AddLog(log);
                                #endregion

                                #region stats
                                Stats st = new Stats();
                                st.ServiceProvider = service.ServiceProvider;
                                st.SupplierId = bracc;
                                st.BranchId = branch.Trim();
                                st.Date = DateTime.Now.Date;
                                st.ServiceName = "NEW CUSTOMER";
                                st.Successful = 1;
                                st.Failed = 0;
                                st.ServiceId = service.Id;
                                trnl.AddStats(st);
                                #endregion

                                #region Email
                                try
                                {
                                    // ST_encrypt en = new ST_encrypt();
                                    ST_decrypt dc = new ST_decrypt();
                                    MailMessage mailMessage = new MailMessage(
                                                             new MailAddress("accounts@yomoneyservice.com"), new MailAddress(email.ToLower()));

                                    var ee = en.encrypt(customer.Email.ToLower().Trim() + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");

                                    mailMessage.Subject = "Account Activation";
                                    mailMessage.IsBodyHtml = true;
                                    mailMessage.Body = "<p>Hello " + customer.ContactPerson + ", </p><p> You have been successfully registered to Yomoney Service.</p>" +

                                    "Use the following credentials to Signin on the yomoney app on google play.<br/><br/> Username = " + customer.Phone1.ToLower().Trim() + "<br/> Password = " + dc.st_decrypt(customer.Password, "214ea9d5bda5277f6d1b3a3c58fd7034") + "</p>" +
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
                                    // Send email
                                }
                                catch (Exception e)
                                {

                                }
                                #endregion


                                return RedirectToAction("CustomerLoyalty");
                            }
                            else
                            {
                                //ModelState.AddModelError("", "Sorry this customer name is already in use");
                                dd = db.Suppliers.Where(u => u.AccountCode.Trim() == SupplierId).FirstOrDefault();

                                if (dd.ParentId != null)
                                {
                                    bracc = dd.ParentId;
                                    branch = SupplierId;
                                }
                                else
                                {
                                    bracc = SupplierId;
                                }

                                Loyalty loy = new Loyalty();
                                loy.Id = customer.ID.ToString();
                                loy.PhoneNumber = customer.Phone1;
                                loy.Name = customer.CustomerName.ToUpper();
                                loy.Points = 0;
                                loy.SupplierId = bracc;
                                loy.SupplierName = dd.SupplierName.Trim();
                                loy.Branch = dd.Branch.Trim();
                                loy.Id = customer.AccountCode;
                                loy.SchemeId = sch.Id;
                                loy.Email = email;
                                loy.ContactPerson = customer.ContactPerson;

                                var px = cus.CreateCustomerLoyalty(loy);

                                #region External
                                var resp = sb.Bill(3, bracc, 0, branch);
                                if (resp != "11102")
                                {
                                    try
                                    {
                                        RoutoSMSTelecom routo = new RoutoSMSTelecom();
                                        routo.SetUser("Faithwork");//0772181813
                                        routo.SetPass("ks4kP1w");
                                        routo.SetNumber(customer.Phone1);
                                        routo.SetOwnNumber(dd.SupplierName);
                                        routo.SetType("0");

                                        routo.SetMessage("Welcome to GAIN Cash & Carry Super Gains reward scheme!! Keep buying from GAIN and stand to get great prizes! Tractors, Mini Buses, 3ton truck up for grabs!");
                                        //routo.unicodeEncodeMessage();
                                        string header = routo.SendDirect();
                                    }
                                    catch (Exception)
                                    {

                                    }

                                }
                                #endregion

                                #region TransactionLog
                                Tranlog log = new Tranlog();
                                log.DestinationAccountNumber = customer.AccountCode;
                                log.SourceAccountNumber = branch;
                                log.Description = "New Account Creation";
                                log.ParentId = bracc;
                                log.Amount = 0;
                                log.TransactionTotal = 0;
                                log.Username = part[1];
                                log.Status = "Pending";
                                log.TerminalId = "WEB";
                                log.TransactionCharge = decimal.Parse("0.00");
                                log.Commision = decimal.Parse("0.00");
                                log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
                                log.ServiceId = "NEW CUSTOMER";
                                log.TransactionType = service.Id;
                                log.BranchName = dd.Branch;
                                trnl.AddLog(log);
                                #endregion

                                #region stats
                                Stats st = new Stats();
                                st.ServiceProvider = service.ServiceProvider;
                                st.SupplierId = bracc;
                                st.BranchId = branch.Trim();
                                st.Date = DateTime.Now.Date;
                                st.ServiceName = "NEW CUSTOMER";
                                st.Successful = 1;
                                st.Failed = 0;
                                st.ServiceId = service.Id;
                                trnl.AddStats(st);
                                #endregion

                                return RedirectToAction("CustomerLoyalty");
                            }
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", "Make sure all fields are field in");
                            return PartialView(customer);
                        }
                    }
                    else
                    {

                        ModelState.AddModelError("", "This customer is already part of the scheme");
                        return PartialView(customer);
                    }
                }
                else
                {

                    ModelState.AddModelError("", "This customer is already part of the scheme");
                    return PartialView(customer);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditCustomer(string Id, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string SupplierId = part[0];
            var accs = db.Suppliers.Where(u => u.AccountCode.Trim() == SupplierId).FirstOrDefault();
            if(accs.ParentId != null)
            {
                SupplierId = accs.ParentId;
            }
            var px = sup.GetLoyalCustomers(SupplierId, Id);

            return PartialView(px);
        }

        [HttpPost]
        public ActionResult EditCustomer(Loyalty loy)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            ServiceBiling sb = new ServiceBiling();
            Customer customer = new Models.Customer();
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    string SupplierId = part[0];
                    var service = db.YomoneyServices.Where(u => u.Name.Trim() == "NEW CUSTOMER").FirstOrDefault();
                    string[] nam = customer.ContactPerson.Split(' ');
                    var y = (from e in db.Customers where nam.Any(u => e.ContactPerson.ToLower().Contains(customer.ContactPerson.ToLower())) || e.Phone1 == customer.Phone1 select e).ToList();
                    if (y != null && y.Count == 0)
                    {
                        ST_encrypt en = new ST_encrypt();

                        var email = customer.Email;
                        string newAccount = en.encrypt(customer.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
                        Random rr = new Random();
                        var pass = rr.Next(00000, 99999).ToString();
                        string Pin = en.encrypt(pass, "214ea9d5bda5277f6d1b3a3c58fd7034");
                        try
                        {
                            var dd = db.Suppliers.Where(u => u.AccountCode.Trim() == SupplierId).FirstOrDefault();

                            string bracc = "";
                            string branch = "";
                            if (dd.ParentId != null)
                            {
                                bracc = dd.ParentId;
                                branch = SupplierId;
                            }
                            else
                            {
                                bracc = SupplierId;
                            }
                            var accs = db.Customers.Where(u => u.Phone1.Trim() == customer.Phone1.Trim()).ToList();
                            var sch = db.LoyaltySchemes.Where(u => u.SupplierId == bracc).FirstOrDefault();
                            if (accs == null || accs.Count == 0)
                            {
                                customer.Email = newAccount;
                                customer.Password = Pin;
                                customer.CustomerName = customer.CustomerName.ToUpper();
                                customer.Balance = 0;
                                customer.Wallet = 0;
                                customer.Purchases = 0;
                                customer.PurchasesToDate = 0;
                                customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                                //np.SaveOrUpdateCustomers(customer);
                                db.Customers.Add(customer);
                                db.SaveChanges();
                                // var cus = new XmlCustomers();
                                cus.CreateCustomerProfile(customer);


                               // Loyalty loy = new Loyalty();
                                loy.Id = customer.ID.ToString();
                                loy.PhoneNumber = customer.Phone1;
                                loy.Name = customer.CustomerName.ToUpper();
                                loy.Points = 0;
                                loy.SupplierId = bracc;
                                loy.SupplierName = dd.SupplierName.Trim();
                                loy.Branch = dd.Branch.Trim();
                                loy.Id = customer.AccountCode;
                                loy.SchemeId = sch.Id;
                                loy.Email = email;
                                loy.ContactPerson = customer.ContactPerson;

                                //XmlCustomers cus = new XmlCustomers();

                                var px = cus.CreateCustomerLoyalty(loy);
                                #region External
                                var resp = sb.Bill(3, bracc, 0, branch);
                                if (resp != "11102")
                                {
                                    try
                                    {
                                        RoutoSMSTelecom routo = new RoutoSMSTelecom();
                                        routo.SetUser("Faithwork");//0772181813
                                        routo.SetPass("ks4kP1w");
                                        routo.SetNumber(customer.Phone1);
                                        routo.SetOwnNumber(dd.SupplierName);
                                        routo.SetType("0");

                                        routo.SetMessage("Welcome to GAIN Cash & Carry Super Gains reward scheme!! Keep buying from GAIN and stand to get great prizes! Tractors, Mini Buses, 3ton truck up for grabs!");
                                        //routo.unicodeEncodeMessage();
                                        string header = routo.SendDirect();
                                    }
                                    catch (Exception)
                                    {

                                    }

                                }
                                #endregion

                                #region TransactionLog
                                Tranlog log = new Tranlog();
                                log.DestinationAccountNumber = customer.AccountCode;
                                log.SourceAccountNumber = branch;
                                log.Description = "New Account Creation";
                                log.ParentId = bracc;
                                log.Amount = 0;
                                log.TransactionTotal = 0;
                                log.Username = part[1];
                                log.Status = "Pending";
                                log.TerminalId = "WEB";
                                log.TransactionCharge = decimal.Parse("0.00");
                                log.Commision = decimal.Parse("0.00");
                                log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
                                log.ServiceId = "NEW CUSTOMER";
                                log.TransactionType = service.Id;
                                log.BranchName = dd.Branch;
                                trnl.AddLog(log);
                                #endregion

                                #region stats
                                Stats st = new Stats();
                                st.ServiceProvider = service.ServiceProvider;
                                st.SupplierId = bracc;
                                st.BranchId = branch.Trim();
                                st.Date = DateTime.Now.Date;
                                st.ServiceName = "NEW CUSTOMER";
                                st.Successful = 1;
                                st.Failed = 0;
                                st.ServiceId = service.Id;
                                trnl.AddStats(st);
                                #endregion

                                #region Email
                                try
                                {
                                    // ST_encrypt en = new ST_encrypt();
                                    ST_decrypt dc = new ST_decrypt();
                                    MailMessage mailMessage = new MailMessage(
                                                             new MailAddress("accounts@yomoneyservice.com"), new MailAddress(email.ToLower()));

                                    var ee = en.encrypt(customer.Email.ToLower().Trim() + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");

                                    mailMessage.Subject = "Account Activation";
                                    mailMessage.IsBodyHtml = true;
                                    mailMessage.Body = "<p>Hello " + customer.ContactPerson + ", </p><p> You have been successfully registered to Yomoney Service.</p>" +

                                    "Use the following credentials to Signin on the yomoney app on google play.<br/><br/> Username = " + customer.Phone1.ToLower().Trim() + "<br/> Password = " + dc.st_decrypt(customer.Password, "214ea9d5bda5277f6d1b3a3c58fd7034") + "</p>" +
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
                                    // Send email
                                }
                                catch (Exception e)
                                {

                                }
                                #endregion


                                return RedirectToAction("CustomerLoyalty");
                            }
                            else
                            {
                                //ModelState.AddModelError("", "Sorry this customer name is already in use");
                                dd = db.Suppliers.Where(u => u.AccountCode.Trim() == SupplierId).FirstOrDefault();

                                if (dd.ParentId != null)
                                {
                                    bracc = dd.ParentId;
                                    branch = SupplierId;
                                }
                                else
                                {
                                    bracc = SupplierId;
                                }

                                //Loyalty loy = new Loyalty();
                                loy.Id = customer.ID.ToString();
                                loy.PhoneNumber = customer.Phone1;
                                loy.Name = customer.CustomerName.ToUpper();
                                loy.Points = 0;
                                loy.SupplierId = bracc;
                                loy.SupplierName = dd.SupplierName.Trim();
                                loy.Branch = dd.Branch.Trim();
                                loy.Id = customer.AccountCode;
                                loy.SchemeId = sch.Id;
                                loy.Email = email;
                                loy.ContactPerson = customer.ContactPerson;

                                var px = cus.CreateCustomerLoyalty(loy);

                                #region External
                                var resp = sb.Bill(3, bracc, 0, branch);
                                if (resp != "11102")
                                {
                                    try
                                    {
                                        RoutoSMSTelecom routo = new RoutoSMSTelecom();
                                        routo.SetUser("Faithwork");//0772181813
                                        routo.SetPass("ks4kP1w");
                                        routo.SetNumber(customer.Phone1);
                                        routo.SetOwnNumber(dd.SupplierName);
                                        routo.SetType("0");

                                        routo.SetMessage("Welcome to GAIN Cash & Carry Super Gains reward scheme!! Keep buying from GAIN and stand to get great prizes! Tractors, Mini Buses, 3ton truck up for grabs!");
                                        //routo.unicodeEncodeMessage();
                                        string header = routo.SendDirect();
                                    }
                                    catch (Exception)
                                    {

                                    }

                                }
                                #endregion

                                #region TransactionLog
                                Tranlog log = new Tranlog();
                                log.DestinationAccountNumber = customer.AccountCode;
                                log.SourceAccountNumber = branch;
                                log.Description = "New Account Creation";
                                log.ParentId = bracc;
                                log.Amount = 0;
                                log.TransactionTotal = 0;
                                log.Username = part[1];
                                log.Status = "Pending";
                                log.TerminalId = "WEB";
                                log.TransactionCharge = decimal.Parse("0.00");
                                log.Commision = decimal.Parse("0.00");
                                log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
                                log.ServiceId = "NEW CUSTOMER";
                                log.TransactionType = service.Id;
                                log.BranchName = dd.Branch;
                                trnl.AddLog(log);
                                #endregion

                                #region stats
                                Stats st = new Stats();
                                st.ServiceProvider = service.ServiceProvider;
                                st.SupplierId = bracc;
                                st.BranchId = branch.Trim();
                                st.Date = DateTime.Now.Date;
                                st.ServiceName = "NEW CUSTOMER";
                                st.Successful = 1;
                                st.Failed = 0;
                                st.ServiceId = service.Id;
                                trnl.AddStats(st);
                                #endregion

                                return RedirectToAction("CustomerLoyalty");
                            }
                        }
                        catch (Exception e)
                        {
                            ModelState.AddModelError("", "Make sure all fields are field in");
                            return PartialView(customer);
                        }
                    }
                    else
                    {

                        ModelState.AddModelError("", "This customer is already part of the scheme");
                        return PartialView(customer);
                    }
                }
                else
                {

                    ModelState.AddModelError("", "This customer is already part of the scheme");
                    return PartialView(customer);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult CustomerUpload(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string supplierId = part[0];
            var supm = db.Suppliers.Where(u => u.AccountCode == supplierId).FirstOrDefault();
           
            List<SelectListItem> sr = new List<SelectListItem>();
            sr.Add(new SelectListItem
            {
                Text = supm.SupplierName.Trim(),
                Value = supm.AccountCode.Trim()
            });

             if (supm.ParentId == null)
             {
                var supl = db.Suppliers.Where(u => u.ParentId == supplierId).ToList();
                foreach (var item in supl)
                {
                    sr.Add(new SelectListItem
                    {
                        Text = item.Branch.Trim(),
                        Value = item.AccountCode.Trim()
                    });
                }  
             }

            ViewBag.Supplier = sr;
            return PartialView();
        }
        [HttpPost]
        public ActionResult CustomerUpload(string Branch, HttpPostedFileBase Image)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string supplierId = part[0];
            if (Image != null)
            {
                var fileName = Image.FileName;
                string directory = Path.Combine(HttpContext.Server.MapPath("~/Content//BulkCustomer"));
                var path = Path.Combine(HttpContext.Server.MapPath("~/Content//BulkCustomer`"), fileName);
                var Dpath = Path.Combine(HttpContext.Server.MapPath("~/Content//BulkCustomer"), "Duplicates_" + fileName);
                Image.SaveAs(path);
                Thread.Sleep(2000);
                var line = "";
                if (System.IO.File.Exists(path))
                {
                    var file = new System.IO.StreamReader(path);
                    ST_encrypt en = new ST_encrypt();
                    var dup = new List<Customer>();
                    int cnt = 0;
                    while ((line = file.ReadLine()) != null)
                    {
                        cnt += 1;
                        Customer customer = new Customer();
                        char[] delimite = new char[] { ',' };
                        string[] parts = line.Split(delimite, StringSplitOptions.RemoveEmptyEntries);
                        //9,DOLLAR GEN DEALER,TAPERA,MPUNZI,785004241,66-051689N03,taperampunzi@gmail.com
                        if (parts.Length == 7)
                        {
                            customer.CustomerName = parts[1];
                            customer.ContactPerson = parts[2] + " " + parts[3];
                            customer.Email = parts[6];
                            customer.Phone1 = parts[4];
                            customer.Phone2 = parts[5];

                            var resp = BulkCustomer(customer, Branch);
                            if (resp.ResponseCode == "Error")
                            {
                                customer.Cc = resp.Description;
                                customer.Bcc = cnt.ToString();
                                dup.Add(customer);
                            }
                        }
                    }
                }
            }

            #region dropdown
            var supm = db.Suppliers.Where(u => u.AccountCode == supplierId).FirstOrDefault();

            List<SelectListItem> sr = new List<SelectListItem>();
            sr.Add(new SelectListItem
            {
                Text = supm.SupplierName.Trim(),
                Value = supm.AccountCode.Trim()
            });

            if (supm.ParentId == null)
            {
                var supl = db.Suppliers.Where(u => u.ParentId == supplierId).ToList();
                foreach (var item in supl)
                {
                    sr.Add(new SelectListItem
                    {
                        Text = item.Branch.Trim(),
                        Value = item.AccountCode.Trim()
                    });
                }
            }

            ViewBag.Supplier = sr;
            #endregion

            return PartialView();
        }

        private TransactionResponse BulkCustomer(Customer customer, string SupplierId)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            TransactionResponse response = new TransactionResponse();
           // Customer customer = new Customer();
            ServiceBiling sb = new ServiceBiling();
            
            if (customer.Phone1 != null && customer.Phone1.Length == 10)
            {
                customer.Phone1 = "263" + customer.Phone1.Substring(1, customer.Phone1.Length - 1);
            }
            else if (customer.Phone1 != null && customer.Phone1.Length == 9)
            {
                customer.Phone1 = "263" + customer.Phone1;
            }
            else if (customer.Phone1 != null && customer.Phone1.Length == 12)
            {
            }
            var service = db.YomoneyServices.Where(u => u.Name.Trim() == "NEW CUSTOMER").FirstOrDefault();
            string[] nam = customer.ContactPerson.Split(' ');
            var y = (from e in db.Customers where nam.Any(u => e.ContactPerson.ToLower().Contains(customer.ContactPerson.ToLower())) || e.Phone1 == customer.Phone1 select e).ToList();
            if (y != null && y.Count == 0)
            {
                #region accountid
                //var pz = np.GetAllCustomers();

                var pp = (from e in db.Customers.OrderBy(u => u.ID) select e).ToList().LastOrDefault();
                if (pp == null)
                {
                    customer.AccountCode = "8-0001-0000001";
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
                    customer.AccountCode = acc;
                    customer.CompanyId = "YOMONEY";
                }
                #endregion
                ST_encrypt en = new ST_encrypt();
                var email = customer.Email;
                string newAccount = en.encrypt(customer.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Random rr = new Random();
                var pass = rr.Next(00000, 99999).ToString();
                string Pin = en.encrypt(pass, "214ea9d5bda5277f6d1b3a3c58fd7034");
                try
                {
                    var dd = db.Suppliers.Where(u => u.AccountCode.Trim() == SupplierId).FirstOrDefault();

                    string bracc = "";
                    string branch = "";
                    if (dd.ParentId != null)
                    {
                        bracc = dd.ParentId;
                        branch = SupplierId;
                    }
                    else
                    {
                        bracc = SupplierId;
                        //branch = SupplierId;
                    }
                    var accs = db.Customers.Where(u => u.Phone1.Trim() == customer.Phone1.Trim()).ToList();
                    var sch = db.LoyaltySchemes.Where(u => u.SupplierId == bracc).FirstOrDefault();
                    if (accs == null || accs.Count == 0)
                    {
                        customer.Email = newAccount;
                        customer.Password = Pin;
                        customer.CustomerName = customer.CustomerName.ToUpper();
                        customer.Balance = 0;
                        customer.Wallet = 0;
                        customer.Purchases = 0;
                        customer.PurchasesToDate = 0;
                        //customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                        //np.SaveOrUpdateCustomers(customer);
                        db.Customers.Add(customer);
                        db.SaveChanges();
                        // var cus = new XmlCustomers();
                        cus.CreateCustomerProfile(customer);


                        Loyalty loy = new Loyalty();
                        loy.Id = customer.ID.ToString();
                        loy.PhoneNumber = customer.Phone1;
                        loy.Name = customer.CustomerName.ToUpper();
                        loy.Points = 0;
                        loy.SupplierId = bracc;
                        loy.SupplierName = dd.SupplierName.Trim();
                        loy.Branch = dd.Branch.Trim();
                        loy.Id = customer.AccountCode;
                        loy.SchemeId = sch.Id;
                        loy.Email = email;
                        loy.ContactPerson = customer.ContactPerson;

                        //XmlCustomers cus = new XmlCustomers();

                        var px = cus.CreateCustomerLoyalty(loy);
                        #region External
                        var resp = sb.Bill(3, bracc, 0, branch);
                        if (resp != "11102")
                        {
                            try
                            {
                                RoutoSMSTelecom routo = new RoutoSMSTelecom();
                                routo.SetUser("Faithwork");//0772181813
                                routo.SetPass("ks4kP1w");
                                routo.SetNumber(customer.Phone1);
                                routo.SetOwnNumber(dd.SupplierName);
                                routo.SetType("0");

                                routo.SetMessage("Welcome to GAIN Cash & Carry Super Gains reward scheme!! Keep buying from GAIN and stand to get great prizes! Tractors, Mini Buses, 3ton truck up for grabs!");
                                //routo.unicodeEncodeMessage();
                                string header = routo.SendDirect();
                            }catch(Exception)
                            {}
                        }
                        #endregion

                        #region TransactionLog
                        Tranlog log = new Tranlog();
                        log.DestinationAccountNumber = customer.AccountCode;
                        log.SourceAccountNumber = branch;
                        log.ParentId = bracc;
                        log.BranchName = dd.Branch;
                        log.Description = "New yomoney account creation";
                        log.Amount = 0;
                        log.TransactionTotal = 0;
                        log.Username = "Cashier";
                        log.Status = "Complete";
                        log.TerminalId = "TerminalId";
                        log.TransactionCharge = decimal.Parse("0.00");
                        log.Commision = decimal.Parse("0.00");
                        log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
                        log.ServiceId = "NEW CUSTOMER";
                        log.TransactionType = service.Id;
                        trnl.AddLog(log);
                        #endregion

                        #region stats
                        Stats st = new Stats();
                        st.ServiceProvider = service.ServiceProvider;
                        st.SupplierId = bracc;
                        st.BranchId = branch.Trim();
                        st.Date = DateTime.Now.Date;
                        st.ServiceName = "NEW CUSTOMER";
                        st.Successful = 1;
                        st.Failed = 0;
                        st.ServiceId = service.Id;
                        trnl.AddStats(st);
                        #endregion

                        #region Email
                        try
                        {
                            // ST_encrypt en = new ST_encrypt();
                            ST_decrypt dc = new ST_decrypt();
                            MailMessage mailMessage = new MailMessage(
                                                     new MailAddress("accounts@yomoneyservice.com"), new MailAddress(email.ToLower()));

                            var ee = en.encrypt(customer.Email.ToLower().Trim() + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");

                            mailMessage.Subject = "Account Activation";
                            mailMessage.IsBodyHtml = true;
                            mailMessage.Body = "<p>Hello " + customer.ContactPerson + ", </p><p> You have been successfully registered to Yomoney Service.</p>" +

                            "Use the following credentials to Signin on the yomoney app on google play.<br/><br/> Username = " + customer.Phone1.ToLower().Trim() + "<br/> Password = " + dc.st_decrypt(customer.Password, "214ea9d5bda5277f6d1b3a3c58fd7034") + "</p>" +
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
                            // Send email
                        }
                        catch (Exception e)
                        {

                        }
                        #endregion
                        response.ResponseCode = "Success";
                        response.Description = "Account created successfully";
                        return response;
                    }
                    else
                    {
                        customer = accs.FirstOrDefault();
                        Loyalty loy = new Loyalty();
                        loy.Id = customer.ID.ToString();
                        loy.PhoneNumber = customer.Phone1;
                        loy.Name = customer.CustomerName.ToUpper();
                        loy.Points = 0;
                        loy.SupplierId = SupplierId;
                        loy.SupplierName = dd.SupplierName.Trim();
                        loy.Branch = dd.Branch.Trim();
                        loy.Id = customer.AccountCode;
                        loy.SchemeId = sch.Id;
                        loy.Email = email;
                        loy.ContactPerson = customer.ContactPerson;
                        // XmlCustomers cus = new XmlCustomers();

                        var px = cus.CreateCustomerLoyalty(loy);
                        #region External
                        var resp = sb.Bill(3, bracc, 0, branch);
                        if (resp != "11102")
                        {
                            RoutoSMSTelecom routo = new RoutoSMSTelecom();
                            routo.SetUser("Faithwork");//0772181813
                            routo.SetPass("ks4kP1w");
                            routo.SetNumber(customer.Phone1);
                            routo.SetOwnNumber(dd.SupplierName);
                            routo.SetType("0");
                            routo.SetMessage("Welcome to GAIN Cash & Carry Super Gains reward scheme!! Keep buying from GAIN and stand to get great prizes! Tractors, Mini Buses, 3ton truck up for grabs!");
                            //routo.unicodeEncodeMessage();
                            string header = routo.SendDirect();
                        }
                        #endregion

                        #region TransactionLog
                        Tranlog log = new Tranlog();
                        log.DestinationAccountNumber = customer.AccountCode;
                        log.SourceAccountNumber = SupplierId;
                        log.Description = "New yomoney account creation";

                        log.Amount = 0;
                        log.TransactionTotal = 0;
                        log.Username = "Cashier";
                        log.Status = "Pending";
                        log.TerminalId = "TerminalId";
                        log.TransactionCharge = decimal.Parse("0.00");
                        log.Commision = decimal.Parse("0.00");
                        log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
                        log.ServiceId = "NEW CUSTOMER";
                        log.TransactionType = service.Id;
                        trnl.AddLog(log);
                        #endregion


                        response.ResponseCode = "Success";
                        response.Description = "Account created successfully";
                        return response;
                    }
                }
                catch (Exception e)
                {
                    response.ResponseCode = "Error";
                    response.Description = e.Message;
                    return response;
                }
            }
            response.ResponseCode = "Error";
            response.Description = "Sorry Customer already exists";
            return response;
        }
        #endregion

        #region supplier
        [HttpGet]
        public ActionResult SupplierLoyalty(string  Id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var CurrentUser = User.Identity.Name;
             char[] delimiter = new char[] { '~' };
             string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
             if (string.IsNullOrEmpty(Id))
             {   
                 Id = part[0];
             }
             
             var px = db.LoyaltySchemes.Where(u => u.SupplierId == Id).ToList();
            
            
                 return PartialView(px);
            
         }
         #region Scheme
        public ActionResult CreateScheme(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string account = part[0];
            var sc = db.LoyaltySchemes.Where(u => u.SupplierId == account).Count();
            LoyaltyScheme px = new LoyaltyScheme();
            px.SupplierId = Company;
            px.Tier = sc + 1;
            return PartialView(px);
        }
        [HttpPost]
        public ActionResult CreateScheme(LoyaltyScheme customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    string account = part[0];
                    var accs = db.Suppliers.Where(u => u.AccountCode.Trim() == account).FirstOrDefault();
                  
                    if (accs != null )
                    {
                        customer.DateCreated = DateTime.Now;
                        customer.SupplierId = accs.AccountCode.Trim();
                       //ustomer.AccessCost = 0;
                        customer.Balance = 0;
                        //customer.Tier = 2;
                        customer.Name = customer.Name.ToUpper();
                        db.LoyaltySchemes.Add(customer);
                        db.SaveChanges();
                        return RedirectToAction("SupplierLoyalty", new  { id = customer.SupplierId});
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sorry this customer name is already in use");
                    }
                }
                ModelState.AddModelError("", "Make sure all fields are field in");
                return PartialView(customer);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditScheme(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var scheme = db.LoyaltySchemes.Find(id);
            return PartialView(scheme);
        }

        [HttpPost]
        public ActionResult EditScheme(LoyaltyScheme scheme)
        {
            if (ModelState.IsValid)
            {
                db.Entry(scheme).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("SupplierLoyalty");
            }
            return PartialView(scheme);
        }

        public ActionResult DetailsScheme(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var customer = db.LoyaltySchemes.Find(id);
            return PartialView(customer);
        }

        public ActionResult Deletescheme(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var customer = db.LoyaltySchemes.Find(id);
            return View(customer);
        }

        
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var customer = db.LoyaltySchemes.Find(id);
            db.LoyaltySchemes.Remove(customer);

            return RedirectToAction("Index");
        }
        #endregion

         #region Rewards
        public ActionResult Rewards(string Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            
            var px = sup.GetRewardsBySchemeId(part[0],long.Parse(Id));
            if (px == null)
            {
                px = new List<Reward>();
            }
            ViewData["SupplierId"] = Id;
            if (Request.IsAjaxRequest())
                return PartialView(px);
            return View(px);
        } 
        public ActionResult CreateReward(long SchemeId)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string account = part[0];
            var sc = db.LoyaltySchemes.Find(SchemeId);
            Reward px = new Reward();
            px.SupplierId = sc.SupplierId;
            px.SupplerName = px.SupplerName;
            px.SchemeId = sc.Id;

            return PartialView(px);
        }
        [HttpPost]
        public ActionResult CreateReward(Reward  rewards)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    string account = part[0];
                    var lsc = db.LoyaltySchemes.Find(rewards.SchemeId);
                    rewards.MonetaryValue = decimal.Parse((rewards.Points * lsc.PointValue).ToString()) ;
                    var ee = sup.AddRewards(rewards);

                    return RedirectToAction("Rewards", new { id = rewards.SchemeId });
                   
                }
                ModelState.AddModelError("", "Make sure all fields are field in");
                return PartialView(rewards);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult EditReward(long Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string account = part[0];
            var px =sup.GetRewardsById(account, Id);

            return PartialView(px);
        }
        [HttpPost]
        public ActionResult EditReward(Reward rewards)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    string account = part[0];
                    var lsc = db.LoyaltySchemes.Find(rewards.SchemeId);
                    rewards.MonetaryValue = decimal.Parse((rewards.Points * lsc.PointValue).ToString());
                    var ee = sup.EditRewards(rewards);

                    return RedirectToAction("Rewards", new { id = rewards.SchemeId });

                }
                ModelState.AddModelError("", "Make sure all fields are field in");
                return PartialView(rewards);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        #endregion
        #endregion

        #region managePoints
        public ActionResult AddPoints(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            //string account = part[0];
            Loyalty px = new Loyalty();
            px.SupplierId = part[0];
            px.CardNumber = Company;
            //if (Request.IsAjaxRequest()) 
            return PartialView(px);

            //return View("AddPoints", "_AdminLayout", px);
        }
        [HttpPost]
        public ActionResult AddPoints(Loyalty customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            ServiceBiling sb = new ServiceBiling();
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    string account = part[0];
                    var cc = sup.GetLoyalCustomers(customer.SupplierId, customer.CardNumber);
                    var branch = "";
                    var Parent = "";
                    string Receipt = customer.Branch;
                    customer.SupplierName = part[4];
                    var supl = db.Suppliers.Where(u => u.Branch.Trim() == cc.Branch.ToUpper().Trim()).FirstOrDefault();
                    if (supl != null && supl.ParentId != null)
                    {
                        branch = supl.AccountCode;
                        customer.SupplierId = supl.ParentId;
                        Parent = supl.ParentId;
                        customer.Branch = supl.Branch;
                    }
                    #region Check duplication
                    var month = DateTime.Now.Month;
                    string ch = trnl.CheckLogReceipts(customer.SupplierId, "WEB", Receipt, month);
                    if (ch == "invalid")
                    {

                        return RedirectToAction("TransactionHistory");
                    }
                    #endregion
                    int points = 0;
                    var resp = sb.Bill(1, Parent, 0, branch);
                    if (resp != "11102")
                    {
                        if (cc != null && cc.Id != null)
                        {
                            #region Part of scheme
                            customer.Id = cc.Id;
                            customer.PhoneNumber = cc.PhoneNumber;

                            // customer exists add loyalty points 
                            var scheme = db.LoyaltySchemes.Find(cc.SchemeId);
                            points = Convert.ToInt32(Convert.ToDecimal(customer.MonetaryValue) / Convert.ToDecimal(scheme.PointCost));
                            customer.Points = points;
                            if (points == 0)
                            {
                                return RedirectToAction("CustomerLoyalty", new { id = customer.SupplierId });
                            }
                            var uu = sup.AddLoyaltyPoints(customer.SupplierId, cc.PhoneNumber, points, decimal.Parse(scheme.PointValue.ToString()));
                            customer.Points = uu.Points;

                            #endregion
                        }
                        else
                        {
                            if (customer.CardNumber.StartsWith("8") && customer.CardNumber.Length == 12)
                            {
                                customer.CardNumber = customer.CardNumber.Substring(0, 1) + "-" + customer.CardNumber.Substring(1, 4) + "-" + customer.CardNumber.Substring(5, 7);
                            }
                            else if (customer.CardNumber != null && customer.CardNumber.Length == 10)
                            {
                                customer.CardNumber = "263" + customer.CardNumber.Substring(1, customer.CardNumber.Length - 1);
                            }
                            else if (customer.CardNumber != null && customer.CardNumber.Length == 9)
                            {
                                customer.CardNumber = "263" + customer.CardNumber;
                            }
                            else if (customer.CardNumber != null && customer.CardNumber.Length == 12)
                            {
                            }
                            var accs = db.Customers.Where(u => u.Phone1.Trim() == customer.CardNumber || u.AccountCode.Trim() == customer.CardNumber).FirstOrDefault();
                            if (accs != null)
                            {
                                // Customer doesnt exist in this scheme
                                var lsc = db.LoyaltySchemes.Where(u => u.SupplierId == customer.SupplierId && u.Tier == 1).FirstOrDefault();
                                points = Convert.ToInt32(Convert.ToDecimal(customer.MonetaryValue) / Convert.ToDecimal(lsc.PointCost));
                                //customer.Points = points;
                                customer.MonetaryValue = Convert.ToDecimal(lsc.PointValue * points);
                                customer.Points = points;
                                customer.Id = accs.AccountCode.Trim();
                                customer.Name = accs.CustomerName;
                                customer.DOB = accs.Dated;
                                customer.PhoneNumber = accs.Phone1;
                                customer.SchemeId = lsc.Id;
                                customer.SupplierName = part[4];
                                var pp = sup.CreateCustomerLoyalty(customer);

                            }
                            else
                            {

                                return RedirectToAction("Create", new { Company = part[1] });

                            }

                        }
                    }
                    #region TransactionLog
                    Tranlog log = new Tranlog();
                    log.DestinationAccountNumber = customer.Id;
                    log.SourceAccountNumber = part[0];
                    log.ParentId = Parent;
                    log.Description = points + " loyalty points from receipt " + Receipt + ". Your points balance is now " + customer.Points + ".";
                    log.ReceiptNumber = Receipt;
                    log.Amount = customer.MonetaryValue;
                    log.TransactionTotal = customer.MonetaryValue;
                    log.TerminalId = "WEB";
                    log.Username = part[2];
                    log.Status = "Complete";
                    log.TransactionCharge = decimal.Parse("0.02");
                    log.Commision = decimal.Parse("0.00");
                    customer.Points = points;
                    log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
                    log.ServiceId = "ADD LOYALTY";
                    log.TransactionType = 1;
                    log.BranchName = supl.Branch.Trim();
                    trnl.AddLog(log);
                    #endregion

                    #region External
                    try
                    {
                        var r = sb.Bill(3, Parent, 0, branch);
                        RoutoSMSTelecom routo = new RoutoSMSTelecom();
                        routo.SetUser("Faithwork");//0772181813
                        routo.SetPass("ks4kP1w");
                        routo.SetNumber(customer.PhoneNumber);
                        routo.SetOwnNumber(customer.SupplierName);
                        routo.SetType("0");
                        if (cc != null && cc.Id != null)
                        {
                            routo.SetMessage("GAIN Cash & Carry Super Gains Reward Scheme – you have earned " + log.Description);
                        }
                        else
                        {
                            routo.SetMessage("Welcome to GAIN Cash & Carry Super Gains reward scheme!! Keep buying from GAIN and stand to get great prizes! Tractors, Mini Buses, 3ton truck up for grabs!");
                        }

                        //routo.unicodeEncodeMessage();
                        string header = routo.SendDirect();
                        

                    }
                    catch (Exception)
                    {

                    }
                    #endregion

                    return RedirectToAction("TransactionHistory");
                }
                else
                {
                    ModelState.AddModelError("", "Make sure all fields are field in");
                    return PartialView(customer);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult RedeemPoints(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Loyalty px = new Loyalty();
            px.SupplierId = Company;
            return PartialView(px);
        }

        public ActionResult SyncLogs(string Company, int month, int year)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var supplier = part[0];
            
            MultipleLogs ml = new MultipleLogs();
            
            ml = trnl.SyncBranchMaster(Company, month, year);
                
            return View("SyncLogs", "_AdminLayout",ml);
        }

        [HttpPost]
        public ActionResult RedeemPoints(Loyalty customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    string account = part[0];
                    var cc = sup.GetRewardsById(customer.SupplierId, customer.SchemeId);


                    if (cc != null)
                    {
                        // customer exists add loyalty points 
                        // var scheme = db.LoyaltySchemes.Find(cc.SchemeId);
                        int points = cc.Points;

                        var uu = sup.RedeemLoyaltyPoints(customer.SupplierId, customer.CardNumber, points);
                        customer.SupplierName = part[5];
                      
                        #region TransactionLog
                        Tranlog log = new Tranlog();
                        log.DestinationAccountNumber = customer.Id;
                        log.SourceAccountNumber = part[0];
                        log.Description = customer.Points + " " + customer.SupplierName + " loyalty points for the " + cc.Name + " reward." ;
                        log.ReceiptNumber = customer.Branch;
                        log.Amount = customer.MonetaryValue;
                        log.TransactionTotal = customer.MonetaryValue;
                        log.Username = part[2];
                        log.Status = "Complete";
                        log.TransactionCharge = decimal.Parse("0.02");
                        log.Commision = decimal.Parse("0.00");
                        log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
                        trnl.AddLog(log);
                        #endregion

                        #region External
                        RoutoSMSTelecom routo = new RoutoSMSTelecom();
                        routo.SetUser("Faithwork");//0772181813
                        routo.SetPass("ks4kP1w");
                        routo.SetNumber(customer.PhoneNumber);
                        routo.SetOwnNumber(customer.SupplierName);
                        routo.SetType("0");
                        routo.SetMessage("You have redeemed " + log.Description);
                        //routo.unicodeEncodeMessage();
                        string header = routo.SendDirect();
                        #endregion
                    }
                    return PartialView(customer);
                }
                return RedirectToAction("Index", "Home");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult CustomerPoints(Loyalty loyal)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string supplier = part[0];
            var cc = cus.GetCustomerLoyaltyBySupplier(loyal.CardNumber, supplier);
            var rews = new List<Reward>();
            foreach(var rr in cc)
            {
                var rewards = sup.GetRewardsBySchemeId(supplier, rr.SchemeId);
                rewards = rewards.Where(u => u.Points <= rr.Points).ToList();
                rews.AddRange(rewards);
            }
            var ee = cc.FirstOrDefault();
            ViewData["Name"] = ee.Name;
            ViewData["Points"] = ee.Points;
            ViewData["Money"] = ee.MonetaryValue;
            ViewData["Supplier"] = ee.SupplierId;
            ViewData["Card"] = ee.CardNumber;
            return PartialView(rews);
        }

        public ActionResult CashPoints(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var pz = np.GetAllCustomers();
            var pp = (from e in pz select e).LastOrDefault();
            LoyaltyScheme px = new LoyaltyScheme();
            px.SupplierId = Company;
            return PartialView(px);
        }

        [HttpPost]
        public ActionResult CashPoints(LoyaltyScheme customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    string account = part[0];
                    var accs = db.Suppliers.Where(u => u.AccountCode.Trim() == account).FirstOrDefault();

                    if (accs != null)
                    {
                        customer.DateCreated = DateTime.Now;
                        customer.SupplierId = accs.AccountCode.Trim();
                        db.LoyaltySchemes.Add(customer);
                        db.SaveChanges();
                        return RedirectToAction("SupplierLoyalty", new { id = customer.SupplierId });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sorry this customer name is already in use");
                    }
                }
                ModelState.AddModelError("", "Make sure all fields are field in");
                return PartialView(customer);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        public ActionResult PointsUpload(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string supplierId = part[0];
            var supm = db.Suppliers.Where(u => u.AccountCode == supplierId).FirstOrDefault();

            List<SelectListItem> sr = new List<SelectListItem>();
            sr.Add(new SelectListItem
            {
                Text = supm.SupplierName.Trim(),
                Value = supm.AccountCode.Trim()
            });

            if (supm.ParentId == null)
            {
                var supl = db.Suppliers.Where(u => u.ParentId == supplierId).ToList();
                foreach (var item in supl)
                {
                    sr.Add(new SelectListItem
                    {
                        Text = item.Branch.Trim(),
                        Value = item.AccountCode.Trim()
                    });
                }
            }

            ViewBag.Supplier = sr;
            return PartialView();
        }

        [HttpPost]
        public ActionResult PointsUpload(string Branch, HttpPostedFileBase Image)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string supplierId = part[0];
            if (Image != null)
            {
                var fileName = Image.FileName;
                string directory = Path.Combine(HttpContext.Server.MapPath("~/Content//BulkCustomer"));
                var path = Path.Combine(HttpContext.Server.MapPath("~/Content//BulkCustomer`"), fileName);
                var Dpath = Path.Combine(HttpContext.Server.MapPath("~/Content//BulkCustomer"), "Duplicates_" + fileName);
                Image.SaveAs(path);
                Thread.Sleep(2000);
                var line = "";
                if (System.IO.File.Exists(path))
                {
                    var file = new System.IO.StreamReader(path);
                    ST_encrypt en = new ST_encrypt();
                    var dup = new List<Customer>();
                    int cnt = 0;
                    while ((line = file.ReadLine()) != null)
                    {
                        cnt += 1;
                        Customer customer = new Customer();
                        char[] delimite = new char[] { ',' };
                        string[] parts = line.Split(delimite, StringSplitOptions.RemoveEmptyEntries);
                        //9,DOLLAR GEN DEALER,TAPERA,MPUNZI,785004241,66-051689N03,taperampunzi@gmail.com
                        if (parts.Length == 7)
                        {
                            customer.CustomerName = parts[1];
                            customer.ContactPerson = parts[2] + " " + parts[3];
                            customer.Email = parts[6];
                            customer.Phone1 = parts[4];
                            customer.Phone2 = parts[5];

                            var resp = BulkCustomer(customer, Branch);
                            if (resp.ResponseCode == "Error")
                            {
                                customer.Cc = resp.Description;
                                customer.Bcc = cnt.ToString();
                                dup.Add(customer);
                            }
                        }
                    }
                }
            }

            #region dropdown
            var supm = db.Suppliers.Where(u => u.AccountCode == supplierId).FirstOrDefault();

            List<SelectListItem> sr = new List<SelectListItem>();
            sr.Add(new SelectListItem
            {
                Text = supm.SupplierName.Trim(),
                Value = supm.AccountCode.Trim()
            });

            if (supm.ParentId == null)
            {
                var supl = db.Suppliers.Where(u => u.ParentId == supplierId).ToList();
                foreach (var item in supl)
                {
                    sr.Add(new SelectListItem
                    {
                        Text = item.Branch.Trim(),
                        Value = item.AccountCode.Trim()
                    });
                }
            }

            ViewBag.Supplier = sr;
            #endregion

            return PartialView();
        }

        #endregion

        #region trn history 
        public ActionResult TransactionHistory()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string account = part[0];
            if(part.Length == 3)
            {
                account = "5-0001-0000000";
            }
            ViewData["Branch"] = "ALL";
            ViewData["Date"] = DateTime.Now.Date.ToString("MM/dd/yyyy"); ;
            #region get branches
            var sup = db.Suppliers.Where(u => u.AccountCode == account).FirstOrDefault();
            if(string.IsNullOrEmpty(sup.ParentId))
            {
                var supl = db.Suppliers.Where(u => u.ParentId == account).ToList();
                if (supl.Count > 0)
                {
                    List<SelectListItem> sr = new List<SelectListItem>();
                    sr.Add(new SelectListItem
                    {
                        Text = "ALL",
                        Value = "ALL"
                    });
                    foreach (var item in supl)
                    {
                        sr.Add(new SelectListItem
                        {
                            Text = item.Branch.Trim(),
                            Value = item.Branch.Trim()
                        });
                    }

                    ViewBag.Supplier = sr;
                }
            }
            else
            {
                List<SelectListItem> sr = new List<SelectListItem>();
                sr.Add(new SelectListItem
                {
                    Text = sup.Branch.Trim(),
                    Value = sup.Branch.Trim()
                });
                ViewBag.Supplier = sr;
            }
            #endregion

            int month = DateTime.Now.Month;
            var date = DateTime.Now.Date;
            var pp = trnl.GetLogs(part[0], month);
            var px = new List<Tranlog>();
            if (pp != null) px = pp.logs.ToList();
            px = px.Where(u => u.TransactionType != 4  ).ToList();
            param.iDisplayStart = 1;
            param.iDisplayLength = 20;
            
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
            ViewData["sStart"] = DateTime.Now.Month.ToString();
            ViewData["listSize"] = param.iDisplayLength.ToString();
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;

            return PartialView(nn);
        }

        [HttpPost]
        public ActionResult TransactionHistory(JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string account = part[0];
            int month = 0;
            var date = DateTime.Now;
            ViewData["Branch"] = param.sColumns;
            ViewData["SearchType"] = param.sEcho;
            #region get branches
            var sup = db.Suppliers.Where(u => u.AccountCode == account).FirstOrDefault();
            if (sup.ParentId == null)
            {
                var supl = db.Suppliers.Where(u => u.ParentId == account).ToList();
                if (supl.Count > 0)
                {
                    List<SelectListItem> sr = new List<SelectListItem>();
                    sr.Add(new SelectListItem
                    {
                        Text = "ALL",
                        Value = "ALL"
                    });
                    foreach (var item in supl)
                    {
                        sr.Add(new SelectListItem
                        {
                            Text = item.Branch.Trim(),
                            Value = item.Branch.Trim()
                        });
                    }

                    ViewBag.Supplier = sr;
                }
            }
            else
            {
                List<SelectListItem> sr = new List<SelectListItem>();
                sr.Add(new SelectListItem
                {
                    Text = sup.Branch.Trim(),
                    Value = sup.Branch.Trim()
                });
                ViewBag.Supplier = sr;
            }

            #endregion

            if (string.IsNullOrEmpty(param.sStart))
            {
                ViewData["Date"] = DateTime.Now.Date.ToString("MM/dd/yyyy");
                month = DateTime.Now.Month;
            }
            else
            {
                date = DateTime.Parse(param.sStart);
                date = date.Date;
                ViewData["Date"] = date.ToString("MM/dd/yyyy"); ;
                month = date.Month;
            }
            var pp = trnl.GetLogs(part[0], month);
            var px = new List<Tranlog>();
            if (pp != null) px = pp.logs.ToList();
            if ( param.iTransactionType != 0)
            {
                if (param.sEcho == "Date")
                {
                    px = px.Where(u => u.TransactionType == param.iTransactionType && u.DateCreated.Date == date).ToList();
                }else
                {
                    px = px.Where(u => u.TransactionType == param.iTransactionType ).ToList();
                }
            }
            else
            {
                if (param.sEcho == "Date")
                {
                    px = px.Where(u => u.TransactionType != 4 && u.DateCreated.Date == date).ToList(); 
                }
                else
                {
                    px = px.Where(u => u.TransactionType != 4 ).ToList(); 
                }
            }
            if (!string.IsNullOrEmpty(param.sColumns) && param.sColumns != "ALL")
            {
                px = px.Where(u => u.BranchName == param.sColumns).ToList();
            }
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                px = px.Where(u => u.Miscellaneous.Contains(param.sSearch)).ToList();
            }
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
            if (string.IsNullOrEmpty(param.sStart)) param.sStart = DateTime.Now.Month.ToString();
            ViewData["sStart"] = param.sStart;
            ViewData["searchTerm"] = param.sSearch;
            ViewData["listSize"] = param.iDisplayLength.ToString();
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;

            return PartialView(nn);
        }

        public ActionResult TransactionDetails(string Id,string date)
         {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            char[] delir = new char[] { '_' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string[] search = Id.Split(delir, StringSplitOptions.RemoveEmptyEntries);
            string account = part[0];
            string name = part[2];
            var usa = db.AgentUsers.Where(u => u.FullName == name && u.AccountNumber == account && u.Role != null).FirstOrDefault();
            var theDate = DateTime.Parse(date);
            var month =theDate.Month;
            var pp = trnl.GetLogs(part[0], month);
            var px = pp.logs.ToList();
            string acc = search[1];
            string description = search[0];
            var tran = px.Where(u => u.DestinationAccountNumber == acc && u.Description == description).FirstOrDefault();
            ViewData["Reversal"] = tran.ServiceId;
            #region check for reversal
            if (tran != null)
            {
                var rev = px.Where(u => u.DestinationAccountNumber == acc && u.Amount == tran.Amount && u.ServiceId == "REVERSAL").FirstOrDefault();
                if (rev != null)
                {
                    ViewData["Reversal"] = rev.ServiceId;
                    ViewData["Reversedby"] = rev.Username;
                    ViewData["Reason"] = rev.Description;
                }
            }
            #endregion

            var dd = JsonConvert.DeserializeObject<Loyalty>(tran.Miscellaneous);
            string[] desc = tran.Description.Split(' ');
            int n;
            bool isNumeric = int.TryParse(desc[0], out n);
            if (isNumeric)
            {
                dd.Points = n;
            }
            tran.Miscellaneous = JsonConvert.SerializeObject(dd);
            var prof =  cus.GetCustomerProfile(tran.DestinationAccountNumber);
            ViewData["CustomerName"] = prof.Name;
            ViewData["CustomerMobile"] = prof.MobileNumber;
            ViewData["Role"] = usa.Role.Trim() ;
            return PartialView(tran);
        }

        public ActionResult RedemptionDetails(string Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            char[] delir = new char[] { '_' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string[] search = Id.Split(delir, StringSplitOptions.RemoveEmptyEntries);
            string account = part[0];
            string name = part[2];
            var usa = db.AgentUsers.Where(u => u.FullName == name && u.AccountNumber == account && u.Role != null).FirstOrDefault();
            //var ii = DateTime.Now.Date;
            var month = DateTime.Now.Date.Month;
            var pp = trnl.GetLogs(part[0], month);
            var px = pp.logs.ToList();
            string acc = search[1];
            string description = search[0];
            var tran = px.Where(u => u.DestinationAccountNumber == acc && u.Description == description).FirstOrDefault();
            ViewData["Reversal"] = tran.ServiceId;

            #region check for reversal
            if (tran != null)
            {
                var rev = px.Where(u => u.DestinationAccountNumber == acc && u.Amount == tran.Amount && u.ServiceId == "REVERSAL").FirstOrDefault();
                if (rev != null)
                {
                    ViewData["Reversal"] = rev.ServiceId;
                    ViewData["Reversedby"] = rev.Username;
                    ViewData["Reason"] = rev.Description;
                }
            }
            #endregion

            var dd = JsonConvert.DeserializeObject<Loyalty>(tran.Miscellaneous);
            string[] desc = tran.Description.Split(' ');
            int n;
            bool isNumeric = int.TryParse(desc[0], out n);
            if (isNumeric)
            {
                dd.Points = n;
            }
            tran.Miscellaneous = JsonConvert.SerializeObject(dd);
            var prof = cus.GetCustomerProfile(tran.DestinationAccountNumber);
            ViewData["CustomerName"] = prof.Name;
            ViewData["CustomerMobile"] = prof.MobileNumber;
            ViewData["Role"] = usa.Role.Trim();
            return PartialView(tran);
        }

        public ActionResult TransactionReversal(string Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            char[] delir = new char[] { '_' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string[] search = CurrentUser.Split(delir, StringSplitOptions.RemoveEmptyEntries);
            string account = part[0];

            var dd = JsonConvert.DeserializeObject<Loyalty>(Id);
            
            ViewData["Amount"] =dd.MonetaryValue;
            ViewData["Invoice"] = dd.DOB;
            dd.SupplierId = Id;
            return PartialView(dd);
        }

        [HttpPost]
        public ActionResult TransactionReversal(Loyalty loy)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (!string.IsNullOrEmpty(loy.Name))
            {
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                char[] delir = new char[] { '_' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                string[] search = CurrentUser.Split(delir, StringSplitOptions.RemoveEmptyEntries);
                string account = part[0];
                var customer  = JsonConvert.DeserializeObject<Loyalty>(loy.SupplierId);
                TransactionResponse trn = new TransactionResponse();
                ServiceBiling sb = new ServiceBiling();
                var supl = db.Suppliers.Where(u => u.AccountCode == customer.SupplierId).FirstOrDefault();
                string branch = customer.SupplierId;
                string Parent = "";
                if (supl != null && supl.ParentId != null)
                {
                    branch = supl.AccountCode;
                    customer.SupplierId = supl.ParentId;
                    Parent = supl.ParentId;
                    customer.Branch = supl.Branch;
                }
                else
                {
                    branch = supl.AccountCode;
                    //customer.SupplierId = supl.ParentId;
                    Parent = customer.SupplierId;
                    customer.Branch = supl.Branch;
                }
                var bill = sb.Bill(5, Parent, 0, branch);
                if (bill != "11102")
                {
                     int points = customer.Points;
                     var uu = sup.RedeemLoyaltyPoints(customer.SupplierId, customer.CardNumber, points);
                     
                    #region TransactionLog
                    Tranlog log = new Tranlog();
                        log.DestinationAccountNumber = customer.Id;
                        log.SourceAccountNumber = customer.SupplierId;
                        log.Description = loy.Name;
                        log.ReceiptNumber = customer.DOB;
                        log.Amount = customer.MonetaryValue;
                        log.BranchName = supl.Branch.ToUpper();
                        log.TransactionTotal = customer.MonetaryValue;
                        log.Username = part[2];
                        log.Status = "Complete";
                        log.TerminalId = "WEB";
                        log.TransactionCharge = decimal.Parse("0.02");
                        log.Commision = decimal.Parse("0.00");
                        log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
                        log.ServiceId = "REVERSAL";
                        log.TransactionType = 2;
                        trnl.AddLog(log);
                        //trnl.AddReward(log);
                        #endregion

                    #region External
                        bill = sb.Bill(3, Parent, 0, branch);
                        if (bill != "11102")
                        {
                            RoutoSMSTelecom routo = new RoutoSMSTelecom();
                            routo.SetUser("Faithwork");//0772181813
                            routo.SetPass("ks4kP1w");
                            routo.SetNumber(uu.PhoneNumber);
                            routo.SetOwnNumber(customer.SupplierId);
                            routo.SetType("0");
                            routo.SetMessage("Transaction reversal:/n " + log.Description);
                            //routo.unicodeEncodeMessage();
                            string header = routo.Send();
                        }
                    #endregion
                    var resp = sb.Bill(1, Parent, 0, branch);

                    string Receipt = loy.DOB;
                    var cc = sup.GetLoyalCustomers(customer.SupplierId, customer.CardNumber);
                    var scheme = db.LoyaltySchemes.Find(cc.SchemeId);
                    points = Convert.ToInt32(Convert.ToDecimal(loy.MonetaryValue) / Convert.ToDecimal(scheme.PointCost));
                    customer.Points = points;
                    var au = sup.AddLoyaltyPoints(customer.SupplierId, customer.CardNumber, points, decimal.Parse(scheme.PointValue.ToString()));
                    customer.Points = au.Points;
                    loy.Points = au.Points;
                    #region TransactionLog
                    log = new Tranlog();
                    log.DestinationAccountNumber = customer.Id;
                    log.SourceAccountNumber = part[0];
                    log.ParentId = Parent;
                    log.Description = points + " loyalty points from receipt " + Receipt + ". Your points balance is now " + customer.Points + ".";
                    log.ReceiptNumber = Receipt;
                    log.Amount = loy.MonetaryValue;
                    log.TransactionTotal = loy.MonetaryValue;
                    log.TerminalId = "WEB";
                    log.Username = part[2];
                    log.Status = "Complete";
                    log.TransactionCharge = decimal.Parse("0.02");
                    log.Commision = decimal.Parse("0.00");
                    customer.Points = points;
                    customer.DOB = loy.DOB;
                    log.Miscellaneous = new JavaScriptSerializer().Serialize(customer);
                    log.ServiceId = "ADD LOYALTY";
                    log.TransactionType = 1;
                    log.BranchName = supl.Branch.Trim();
                    trnl.AddLog(log);
                    #endregion

                    #region External
                    try
                    {
                        var r = sb.Bill(3, Parent, 0, branch);
                        RoutoSMSTelecom routo = new RoutoSMSTelecom();
                        routo.SetUser("Faithwork");//0772181813
                        routo.SetPass("ks4kP1w");
                        routo.SetNumber(customer.PhoneNumber);
                        routo.SetOwnNumber(customer.SupplierName);
                        routo.SetType("0");
                        if (cc != null && cc.Id != null)
                        {
                            routo.SetMessage("GAIN Cash & Carry Super Gains Reward Scheme – you have earned " + log.Description);
                        }
                        else
                        {
                            routo.SetMessage("Welcome to GAIN Cash & Carry Super Gains reward scheme!! Keep buying from GAIN and stand to get great prizes! Tractors, Mini Buses, 3ton truck up for grabs!");
                        }

                        //routo.unicodeEncodeMessage();
                        string header = routo.SendDirect();


                    }
                    catch (Exception)
                    {

                    }
                    #endregion

                }
                else
                {
                    trn.ResponseCode = "11102";
                    trn.Description = "Your account Balance is too low";
                }

                return RedirectToAction("TransactionHistory");
            }
            return PartialView(loy);
        }
             
        [HttpGet]
        public ActionResult RewardHistory()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string account = part[0];
            var px = new List<Tranlog>();
            int month = 0;
            if (string.IsNullOrEmpty(param.sStart))
            {
                month = DateTime.Now.Month;
            }
            else
            {
                month = int.Parse(param.sStart);
            }
            var pp = trnl.GetRewards(part[0]);
            if (pp != null)
            {
                pp.logs = pp.logs.Where(u => u.Status == "Complete").ToList();
                px = pp.logs.OrderByDescending(u => u.DateCreated).ToList();
            }
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

            return PartialView(px);
            
        }

        [HttpPost]
        public ActionResult RewardHistory(JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
           
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string account = part[0];
            var px = new List<Tranlog>();
            int month = 0;
            if (string.IsNullOrEmpty(param.sStart))
            {
                month = DateTime.Now.Month;
            }
            else
            {
                month = int.Parse(param.sStart);
            }
            var pp = trnl.GetRewards(part[0]);
           // pp.logs = pp.logs.Where(u => u.Status == "Pending").ToList();

            if (pp != null) px = pp.logs.OrderBy(u => u.DateCreated).ToList();

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

            return PartialView(px);
            
        }

        public ActionResult RewardDetails(long Id,string date)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string account = part[0];
            var month = DateTime.Parse(date).Month;
            var px = trnl.GetLogsById(part[0], Id.ToString(),month);
            var prof = cus.GetCustomerProfile(px.DestinationAccountNumber);
            ViewData["CustomerName"] = prof.Name;
            ViewData["CustomerMobile"] = prof.MobileNumber;
            ViewData["CustomerId"] = prof.Id;
            return PartialView(px);
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               //context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}