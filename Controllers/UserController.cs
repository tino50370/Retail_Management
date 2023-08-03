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

namespace RetailKing.Controllers
{   
    public class UserController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Accounts/

        public ActionResult Index(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var users = np.GetAlllogin(Company);
            var px = users.Where(u => u.Status.Contains("Active")).ToList();
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
        public ActionResult Index(string Company, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
           // JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAlllogin(Company);
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
            var account = np.GetloginById(id);
            return View(account);
        }

        //
        // GET: /Accounts/Create

        public ActionResult Create()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
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
                        Text = item.company.Trim() ,
                        Value = item.company.Trim()
                    });
            }

            ViewBag.Company = yr;

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
            
            return PartialView();
        }

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
        //
        // POST: /Accounts/Create

        [HttpPost]
        public ActionResult Create(login user)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid)
            {
                ST_encrypt en = new ST_encrypt();
                user.Firstname = user.Firstname.ToUpper();
                user.Surname = user.Surname.ToUpper();
                user.prefix = user.prefix.ToUpper();
                user.username = user.username;
                user.Status = "Active";
                var  Pin = en.encrypt(user.password, "214ea9d5bda5277f6d1b3a3c58fd7034");
                user.password = Pin;
                var px = np.Getlogin(user.username);
                if (px == null)
                {
                    np.SaveOrUpdatelogin(user);
                    return RedirectToAction("Index", new { Company = user.Location });
                }
                else
                {
                    ModelState.AddModelError("", "Username is already in use");
                }
              
                  
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
 
        public ActionResult Edit(int Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            
            
            var acc = np.GetAllActiveco();
            var user = np.GetloginById(Id);
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
            string id = Id.ToString();
            ViewBag.Company = yr;
            login account = np.GetloginById(Id);
            return PartialView(account);
        }

        //
        // POST: /Accounts/Edit/5

        [HttpPost]
        public ActionResult Edit(login account)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid)
            {
                login acnt = np.GetloginById(account.ID);
                acnt.Firstname = account.Firstname.ToUpper();
                acnt.Surname = account.Surname.ToUpper();
                acnt.prefix = account.prefix.ToUpper();

                acnt.username = account.username;
                acnt.accesslevel = account.accesslevel;
                acnt.Location = account.Location;
                acnt.PosUser = account.PosUser;
                acnt.RetailUser = account.RetailUser;
                acnt.UsesBoth = account.UsesBoth;
                acnt.Shifts = account.Shifts;
                //acnt.AddsProducts = account.AddsProducts;
                acnt.Barcoding = account.Barcoding;
                //acnt.CashCollection = account.CashCollection;
                //acnt.ManagerMenu = account.ManagerMenu;
                acnt.Tables = account.Tables;
                acnt.Payments = account.Payments;
                acnt.Reporting = account.Reporting;
                acnt.Sales = account.Sales;
                acnt.Pricing = account.Pricing;
                acnt.Purchases = account.Purchases;
                acnt.ManageUsers = account.ManageUsers;
                acnt.ManagePos = account.ManagePos;
                acnt.Transfers = account.Transfers;
                acnt.Accounts = account.Accounts;
                //acnt.OnShift = account.OnShift;
                acnt.xreport = account.xreport;
                acnt.zreport = account.zreport;
                acnt.voidsale = account.voidsale;
                //acnt.CashCollection = account.CashCollection;
                acnt.InventoryManagement = account.InventoryManagement;
                acnt.LossAuthorize = account.LossAuthorize;
                acnt.GivesDiscounts = account.GivesDiscounts;
                acnt.PosCustomers = account.PosCustomers;
                //acnt.LossControl = account.LossControl;
                //acnt.MultiCompanyAccess = account.MultiCompanyAccess;

                //ST_encrypt en = new ST_encrypt();
                //var Pin = en.encrypt(account.password, "214ea9d5bda5277f6d1b3a3c58fd7034");
                //acnt.password = Pin;
                np.UpdateLogin(acnt);
                return RedirectToAction("Index", new { Company = account.Location });  
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
            
            return PartialView(account);
        }

        //
        // GET: /Accounts/Delete/5
 
        public ActionResult DeleteUser(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            login user = np.GetloginById(id);
            return PartialView(user);
        }

        // POST: /Accounts/Delete/5

        [HttpPost, ActionName("DeleteUser")]
        public ActionResult DeleteUser(login account)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid)
            {
                login acnt = np.GetloginById(account.ID);
                acnt.Status = "Deleted";
                acnt.password = acnt.password + "_";
                np.UpdateLogin(acnt);
            }
            return RedirectToAction("Index");
        }
        public ActionResult ResetPassword(int id = 0)
        {
            login Login = db.logins.Find(id);
            ST_decrypt dc = new ST_decrypt();
            var oo = dc.st_decrypt(Login.password, "214ea9d5bda5277f6d1b3a3c58fd7034");
            Login.password = oo;
            if (Login == null)
            {
                return HttpNotFound();
            }
            return PartialView(Login);
        }

        //
        // POST: /Company/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(login Login)
        {
            if (Login.password == Request["ResetPass"])
            {


                if (ModelState.IsValid)
                {
                    ST_encrypt en = new ST_encrypt();
                    Login.password = en.encrypt(Login.password, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    db.Entry(Login).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index",new {Company = Login.Location });
                }
            }
            else
            {
                ModelState.AddModelError("", "Invalid Username or Password");
            }
            return PartialView(Login);
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