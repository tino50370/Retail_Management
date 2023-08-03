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
    public class CRMController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Customers/

        public ActionResult CustomerList()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllCustomers();

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

        [HttpPost]
        public ActionResult CustomerList(string category, string ItemCode, JQueryDataTableParamModel param)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var px = np.GetAllCustomers();

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
        // GET: /Customers/Details/5

        public ActionResult Details(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Customer customer = np.GetCustomers(id);
            return PartialView(customer);
        }

        public ActionResult Search(string searchText)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var px = np.GetCustomersSearch(searchText);
            return PartialView(px);
        } 
        //
        // GET: /Customers/Create

        public ActionResult Create(string Company)
        {
           NHibernateDataProvider np = new NHibernateDataProvider();
           var pz = np.GetAllCustomers();
            var pp = (from e in pz select e ).LastOrDefault();
           Customer px = new Customer();
            if (pp == null)
            {
                pp = new Customer();
                px.AccountCode = "8-01-00001";
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
                    acc = part[0] + "-" + part[1] + "-"  + "00"+ x.ToString();
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
                px.AccountCode = acc;
                px.CompanyId = Company;
            }
            return PartialView(px);
        } 

        //
        // POST: /Suppliers/Create

        [HttpPost]
        public ActionResult Create(Customer customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                if (ModelState.IsValid)
                {
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    ST_encrypt en = new ST_encrypt();

                    string newAccount = en.encrypt(customer.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    var pass = System.Web.Security.Membership.GeneratePassword(6,1);
                    string Pin = en.encrypt(pass, "214ea9d5bda5277f6d1b3a3c58fd7034");
                   

                    var accs = db.Customers.Where(u => u.Email.Trim() == customer.Email).ToList();
                    if (accs == null || accs.Count == 0)
                    {
                        customer.Email = newAccount;
                        customer.Password = pass;
                        customer.CustomerName = customer.CustomerName.ToUpper();
                        customer.ParentId = long.Parse(part[1]);
                        customer.Balance = 0;
                        customer.Wallet = 0;
                        customer.Purchases = 0;
                        customer.PurchasesToDate = 0;
                        //customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                        np.SaveOrUpdateCustomers(customer);

                        return RedirectToAction("Index");
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

        public ActionResult IncomeCalculator()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            return PartialView();
        }

        public ActionResult Payment(Customer customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            return PartialView(customer);
        }

        [HttpPost]
        public ActionResult Payment(string customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            return PartialView(customer);
        } 
        //
        // GET: /Suppliers/Edit/5
 
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Customer customer = np.GetCustomers(id);
            return View(customer);
        }

        //
        // POST: /Suppliers/Edit/5

        [HttpPost]
        public ActionResult Edit(Customer customer)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                np.SaveOrUpdateCustomers(customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //
        // GET: /Suppliers/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Customer customer = np.GetCustomers(id);
            return View(customer);
        }

        //
        // POST: /Suppliers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Customer customer = np.GetCustomers(id);
            np.SaveOrUpdateCustomers(customer);
            
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