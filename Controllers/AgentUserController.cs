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
using System.Text.RegularExpressions;
using System.Net.Mail;
using System.Globalization;

namespace RetailKing.Controllers
{   
    [Authorize]
    public class AgentUserController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        bool invalid = false;

        //
        // GET: /Accounts/

        public ActionResult Index(string SupplierId)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (string.IsNullOrEmpty(SupplierId))
            {
                SupplierId = part[0];
            }
            // JQueryDataTableParamModel param = new JQueryDataTableParamModel();
             var sup = db.Suppliers.Where(u => u.AccountCode == SupplierId).FirstOrDefault();
            ViewData["Branch"] = "ALL";

            #region get branches
           
            if (string.IsNullOrEmpty(sup.ParentId))
            {
                var supl = db.Suppliers.Where(u => u.ParentId == SupplierId).ToList();
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
                            Value = item.AccountCode.Trim()
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
            // JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = db.AgentUsers.Where(u => u.UserName != u.AccountNumber).ToList();
            if (sup != null && !string.IsNullOrEmpty(sup.ParentId))
            {
                px = px.Where(u => u.AccountNumber == SupplierId ).ToList();
            }
            else
            {
                px = px.Where(u => u.SessionCode.Trim() == SupplierId).ToList();
            }
            // var px = np.GetAlllogin(Company);
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
        public ActionResult Index(string SupplierId, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
           // if(string.IsNullOrEmpty(SupplierId))
           // {
                SupplierId =part[0];
            //}
            var sup = db.Suppliers.Where(u => u.AccountCode == SupplierId).FirstOrDefault();
            #region get branches

            if (string.IsNullOrEmpty(sup.ParentId))
            {

                var supl = db.Suppliers.Where(u => u.ParentId == SupplierId).ToList();
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
                            Value = item.AccountCode.Trim()
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
                    Value = sup.AccountCode.Trim()
                });
                ViewBag.Supplier = sr;
            }
            #endregion
            // JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            // session code is the parentId fopr the agent user
            var px = db.AgentUsers.Where(u => u.UserName != u.AccountNumber).ToList();
            if (sup != null && !string.IsNullOrEmpty(sup.ParentId))
            {
              
                px = px.Where(u => u.AccountNumber.Trim() == SupplierId).ToList();
            }
            else
            {
                if (param.sColumns != null)
                {
                    SupplierId = param.sColumns;
                    ViewData["Branch"] = param.sColumns;
                }
                px = px.Where(u => u.AccountNumber.Trim() == SupplierId).ToList();
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

        //
        // GET: /Accounts/Details/5

        public ActionResult Details(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var account = db.AgentUsers.Find(id);
            return PartialView(account);
        }

        //
        // GET: /Accounts/Create

        public ActionResult secondLevel(string json_str)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var acc = np.GetAccountsByCode(json_str);
            
            List<SelectListItem> typ = new List<SelectListItem>();
           

            typ.Add(new SelectListItem
            {
                Text = "Select Account",
                Value = "Select Account"
            });

            foreach (var item in acc)
            {
            if (item.AccountCode.Length == 4)
                {
                    typ.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }
            }

            return Json(typ, JsonRequestBehavior.AllowGet);
        }

        public ActionResult Create()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string accr = part[1];
            string supplier = part[0];
            var acc = db.Roles.Where(u => u.InstitutionId == accr).ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in acc)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.Name.ToUpper().Trim(),
                    Value = item.Name.ToUpper().Trim()
                });
            }

            ViewBag.Role = yr;

            List<SelectListItem> cc = new List<SelectListItem>();
            var syl = db.Menus.ToList();
            cc.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            foreach (var item in syl)
            {
                cc.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Name
                });
            }
            ViewBag.Menus = cc;

            var supm = db.Suppliers.Where(u => u.AccountCode == supplier).FirstOrDefault();

            var supl = db.Suppliers.Where(u => u.ParentId == supplier).ToList();
            List<SelectListItem> sr = new List<SelectListItem>();
            sr.Add(new SelectListItem
            {
                Text = supm.SupplierName.Trim(),
                Value = supm.AccountCode.Trim()
            });
            foreach (var item in supl)
            {
                sr.Add(new SelectListItem
                {
                    Text = item.Branch.Trim(),
                    Value = item.AccountCode.Trim()
                });
            }

            ViewBag.Supplier = sr;
            return PartialView();
        }

        [HttpPost]
        public ActionResult Create(AgentUser user)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string accr = user.AccountNumber;// part[0];
            var spn =db.Suppliers.Where(u => u.AccountCode == accr).FirstOrDefault();
            string supname = spn.SupplierName.Trim();
            if (ModelState.IsValid)
            {
                ST_encrypt en = new ST_encrypt();
                var unam = user.UserName;
               // string newAccount = en.encrypt(unam, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Random rr = new Random();
                var Pin = user.SessionCode;
                var ePin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                var question = en.encrypt(accr, "214ea9d5bda5277f6d1b3a3c58fd7034");
                var account = en.encrypt(unam.ToLower(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                string Answer = "MerchantUser";// "Agent";
                string ssAnswer = en.encrypt(Answer, "214ea9d5bda5277f6d1b3a3c58fd7034");
                string conn = en.encrypt(supname, "214ea9d5bda5277f6d1b3a3c58fd7034");

                if (IsValidEmail(user.UserName))
                {
                    
                    #region Admins
                    AgentUser usa = new AgentUser();
                    usa.UserName = user.UserName.ToLower();
                    usa.FullName = user.FullName.ToUpper();
                    usa.CreationDate = DateTime.Now;
                    usa.AccountNumber = accr;
                    usa.TerminalId = 1;
                    usa.Role = user.Role.ToUpper();
                    if (spn.ParentId == null)
                    {
                        usa.SessionCode = spn.AccountCode;
                    }else
                    {
                        usa.SessionCode = spn.ParentId;
                    }
                    
                    db.AgentUsers.Add(usa);
                    db.SaveChanges();

                    AccountCredenitial acca = new AccountCredenitial();
                    acca.AccountNumber = account;
                    acca.Pin = ePin;
                    acca.Question = question;
                    acca.Answer = Answer;
                    acca.Active = true;
                    acca.Access = "Agent";
                    acca.Connection = conn;
                    db.AccountCredenitials.Add(acca);
                    db.SaveChanges();

                    #region Email
                    MailMessage mailMessage = new MailMessage(
                                             new MailAddress("accounts@yomoneyservice.com"), new MailAddress(unam.ToLower()));

                    var ee = en.encrypt(user.UserName.ToLower().Trim() + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");

                    mailMessage.Subject = "Account Activation";
                    mailMessage.IsBodyHtml = true;
                    mailMessage.Body = "<p>Hello " + user.FullName  + ", </p><p> You have been successfully registered as a Yomoney Administrator for "+ supname + ".</p>" +

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
                    // Send email
                    #endregion

                    #endregion
                }
                else
                {
                    #region Cashier
                    
                    AccountCredenitial accmr = new AccountCredenitial();
                    accmr.AccountNumber = account;
                    accmr.Pin = ePin;
                    accmr.Question = question;
                    accmr.Answer = Answer;
                    accmr.Active = true;
                    accmr.Access = "Agent";
                    accmr.Connection = conn;
                    db.AccountCredenitials.Add(accmr);
                    db.SaveChanges();

                    user.AccountNumber = accr;
                    user.FullName = user.FullName.ToUpper();
                    user.UserName = user.UserName.ToUpper();
                    user.Role = user.Role.ToUpper();
                    user.CreationDate = DateTime.Now;
                    if (spn.ParentId == null)
                    {
                        user.SessionCode = spn.AccountCode;
                    }
                    else
                    {
                        user.SessionCode = spn.ParentId;
                    }

                    db.AgentUsers.Add(user);
                    db.SaveChanges();
                    #endregion
                }

                return RedirectToAction("Index");  
            }
            var acc = np.GetAllActiveco();
            List<SelectListItem> yr = new List<SelectListItem>();

            yr.Add(new SelectListItem
            {
                Text = "Select Company",
                Value = "Select Company"
            });

            foreach (var item in acc)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.company.Trim(),
                    Value = item.company.Trim()
                });
            }

            ViewBag.Company = yr;
            @ViewData["listSize"] = 20;
            return PartialView(user);
        }
        
        //
        // GET: /Accounts/Edit/5
 
        public ActionResult Edit(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string accr = part[1];
            string supplier = part[0];
            var acc = db.Roles.Where(u => u.InstitutionId == accr).ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            yr.Add(new SelectListItem
            {
                Text = "Select Role",
                Value = "Select Role"
            });

            foreach (var item in acc)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.Name.ToUpper().Trim(),
                    Value = item.Name.ToUpper().Trim()
                });
            }

            ViewBag.Role = yr;

            List<SelectListItem> cc = new List<SelectListItem>();
            var syl = db.Menus.ToList();
            cc.Add(new SelectListItem
            {
                Text = "",
                Value = ""
            });
            foreach (var item in syl)
            {
                cc.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Name
                });
            }
            ViewBag.Menus = cc;

            var supm = db.Suppliers.Where(u => u.AccountCode == supplier).FirstOrDefault();

            var supl = db.Suppliers.Where(u => u.ParentId == supplier).ToList();
            List<SelectListItem> sr = new List<SelectListItem>();
            sr.Add(new SelectListItem
            {
                Text = supm.SupplierName.Trim(),
                Value = supm.AccountCode.Trim()
            });
            foreach (var item in supl)
            {
                sr.Add(new SelectListItem
                {
                    Text = item.Branch.Trim(),
                    Value = item.AccountCode.Trim()
                });
            }

            ViewBag.Supplier = sr;

            AgentUser  user = db.AgentUsers.Find(long.Parse(id));
            var unam = user.UserName.Trim();
            ST_encrypt en = new ST_encrypt();
            ST_decrypt dec = new ST_decrypt();
            if (!IsValidEmail(unam))
            {
                
                string newAccount = en.encrypt(unam, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Random rr = new Random();
                // var Pin = user.SessionCode;
                 
                var question = en.encrypt(user.AccountNumber.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                var account = en.encrypt("Nobuhle", "214ea9d5bda5277f6d1b3a3c58fd7034");

                var accmr = db.AccountCredenitials.Where(u => u.Question.Trim() == question).ToList();
                foreach(var it in accmr)
                {
                    var ePin = dec.st_decrypt(it.AccountNumber.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    var Pin = dec.st_decrypt(it.Pin.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    if (unam == ePin.ToUpper())
                    {
                        user.DeviceId = it.Question;
                    }
                }
               
                //char[] charArray = accmr.Pin.Trim().ToCharArray();
                //Array.Reverse(charArray);
               // user.DeviceId = accmr.Question;
                //user.SessionCode = new string(charArray);
            }
            else
            {
                string newAccount = en.encrypt(unam, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Random rr = new Random();
                // var Pin = user.SessionCode;
                // var ePin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                var question = en.encrypt(user.AccountNumber.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                var account = en.encrypt(unam, "214ea9d5bda5277f6d1b3a3c58fd7034");

                var accmr = db.AccountCredenitials.Where(u => u.AccountNumber.Trim() == account && u.Question.Trim() == question).FirstOrDefault();
                char[] charArray = accmr.Pin.Trim().ToCharArray();
                Array.Reverse(charArray);
                user.DeviceId = accmr.Question;
               // user.Datelast  = new string(charArray);

            }
            return PartialView(user);
        }

        //
        // POST: /Accounts/Edit/5

        [HttpPost]
        public ActionResult Edit(AgentUser user)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            string accr = user.AccountNumber;// part[0];
            var spn = db.Suppliers.Where(u => u.AccountCode == accr).FirstOrDefault();
            string supname = spn.SupplierName.Trim();

            if (ModelState.IsValid)
            {
                ST_encrypt en = new ST_encrypt();
                var unam = user.UserName;
                if (!IsValidEmail(unam))
                {
                    #region email
                    string newAccount = en.encrypt(unam, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    Random rr = new Random();
                    char[] charArray = user.SessionCode.ToCharArray();
                    Array.Reverse(charArray);

                    user.SessionCode = new string(charArray);
                    var question = en.encrypt(accr, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    var account = en.encrypt(unam, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    string Answer = "agentUser";// "Agent";
                    string ssAnswer = en.encrypt(Answer, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    string conn = en.encrypt(supname, "214ea9d5bda5277f6d1b3a3c58fd7034");

                    var accmr = db.AccountCredenitials.Where(u => u.AccountNumber == account && u.Pin == user.SessionCode).FirstOrDefault();
                   
                    accmr.AccountNumber = account;
                    accmr.Question = question;
                    accmr.Answer = Answer;
                    accmr.Active = true;
                    accmr.Access = "System";
                    accmr.Connection = conn;
                    db.Entry(accmr).State = EntityState.Modified;
                    db.SaveChanges();

                    // user.AccountNumber = accr;
                    user.FullName = user.FullName.ToUpper();
                    user.UserName = user.UserName.ToUpper();
                    user.Role = user.Role.ToUpper();
                    user.CreationDate = DateTime.Now;
                    user.SessionCode = "";

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();
#endregion
                }
                else
                {
                    #region email
                    string newAccount = en.encrypt(unam, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    Random rr = new Random();
                    char[] charArray = user.SessionCode.ToCharArray();
                    Array.Reverse(charArray);

                    user.SessionCode = new string(charArray);
                    // var Pin = user.SessionCode;
                    // var ePin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    var question = en.encrypt(accr, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    var account = en.encrypt(unam, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    string Answer = "agentUser";// "Agent";
                    string ssAnswer = en.encrypt(Answer, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    string conn = en.encrypt(supname, "214ea9d5bda5277f6d1b3a3c58fd7034");

                    var accmr = db.AccountCredenitials.Where(u => u.AccountNumber == account && u.Pin == user.SessionCode).FirstOrDefault();
                    // AccountCredenitial accmr = new AccountCredenitial();
                    accmr.AccountNumber = account;
                    // accmr.Pin = ePin;
                    accmr.Question = question;
                    accmr.Answer = Answer;
                    accmr.Active = true;
                    accmr.Access = "System";
                    accmr.Connection = conn;
                    db.Entry(accmr).State = EntityState.Modified;
                    db.SaveChanges();

                    // user.AccountNumber = accr;
                    user.FullName = user.FullName.ToUpper();
                    user.UserName = user.UserName.ToUpper();
                    user.Role = user.Role.ToUpper();
                    user.CreationDate = DateTime.Now;
                    user.SessionCode = "";

                    db.Entry(user).State = EntityState.Modified;
                    db.SaveChanges();

                    #endregion
                }
                return RedirectToAction("Index");  
            }
            var acc = np.GetAllActiveco();
            List<SelectListItem> yr = new List<SelectListItem>();

            yr.Add(new SelectListItem
            {
                Text = "Select Company",
                Value = "Select Company"
            });

            foreach (var item in acc)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.company.Trim(),
                    Value = item.company.Trim()
                });
            }

            ViewBag.Company = yr;
            var supm = db.Suppliers.Where(u => u.AccountCode == spn.ParentId).FirstOrDefault();

            var supl = db.Suppliers.Where(u => u.ParentId == spn.ParentId).ToList();
            List<SelectListItem> sr = new List<SelectListItem>();
            sr.Add(new SelectListItem
            {
                Text = supm.SupplierName.Trim(),
                Value = supm.AccountCode.Trim()
            });
            foreach (var item in supl)
            {
                sr.Add(new SelectListItem
                {
                    Text = item.Branch.Trim(),
                    Value = item.AccountCode.Trim()
                });
            }

            ViewBag.Supplier = sr;

            return PartialView(user);
        }

        //
        // GET: /Accounts/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
           var account = db.AgentUsers.Find(id);
            return PartialView(account);
        }

        // POST: /Accounts/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult Delete(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string accr = part[1];
            string supplier = part[0];
      

            AgentUser user = db.AgentUsers.Find(long.Parse(id));
            var unam = user.UserName.Trim();
            ST_encrypt en = new ST_encrypt();
            ST_decrypt dec = new ST_decrypt();
           

                string newAccount = en.encrypt(unam, "214ea9d5bda5277f6d1b3a3c58fd7034");
                Random rr = new Random();
            // var Pin = user.SessionCode;
            if (IsValidEmail(user.UserName.Trim()))
            {
                var question = en.encrypt(user.UserName.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");

                var accmr = db.AccountCredenitials.Where(u => u.AccountNumber.Trim() == question).ToList();
                foreach (var it in accmr)
                {
                    var ePin = dec.st_decrypt(it.AccountNumber.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    var Pin = dec.st_decrypt(it.Pin.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    if (unam.ToUpper() == ePin.ToUpper())
                    {
                        db.Entry(it).State = EntityState.Deleted;
                        db.SaveChanges();
                        break;
                    }
                }
            }
            else
            {
                var question = en.encrypt(user.AccountNumber.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");

                var accmr = db.AccountCredenitials.Where(u => u.Question.Trim() == question).ToList();
                foreach (var it in accmr)
                {
                    var ePin = dec.st_decrypt(it.AccountNumber.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    var Pin = dec.st_decrypt(it.Pin.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
                    if (unam.ToUpper() == ePin.ToUpper())
                    {
                        db.Entry(it).State = EntityState.Deleted;
                        db.SaveChanges();
                        break;
                    }
                }
            }
                db.Entry(user).State = EntityState.Deleted;
                db.SaveChanges();

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               // np.dis();
            }
            base.Dispose(disposing);
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
                var addr = new System.Net.Mail.MailAddress(strIn);
                return addr.Address == strIn;
           
            }
            catch (Exception )
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

    }
}