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
using System.Net.Mail;

namespace RetailKing.Controllers
{
    public class CustomersController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Customers/
        [Authorize]
        public ActionResult Index()
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
         [Authorize]
         [HttpPost]
         public ActionResult Index(string category, string ItemCode, JQueryDataTableParamModel param)
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

        public ActionResult Browse(string Company, JQueryDataTableParamModel param, string cc)
        {
            ViewData["searchcustomer"] = param.sSearch;
            NHibernateDataProvider np = new NHibernateDataProvider();
            var pp = new List<Customer>();
            if (string.IsNullOrEmpty(cc))
            {
                pp = db.Customers.Take(50).ToList();
            }else
            {
                pp = db.Customers.Where(u => u.CustomerName.Contains(cc)).ToList();
            }
            //var px = (from e in pp.Where(u => u.CompanyId == Company) select e).ToList();
            return PartialView(pp);
        }
        public ActionResult GetCustomer(string Transaction, string searchtext, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            IList<Customer> puu = new List<Customer>();
            var px = np.GetCustomersSearch(searchtext);
            foreach (var item in px)
            {
                Customer aa = new Customer();
                aa.Username = item.Username;
                aa.AccountCode = item.AccountCode;
                
                puu.Add(aa);
            }

            //if (puu.Count == 0 && searchtext.Length >= 5)
            //{
            //    var px = np.GetCustomersSearch(searchtext);
            //    foreach (var item in px)
            //    {
            //        Customer aa = new Customer();
            //        aa.Username = item.Username;
            //        aa.AccountCode = item.AccountCode;
                    
            //        puu.Add(aa);
            //    }

            //}
            return PartialView(puu);
        }
    // GET: /Customers/Details/5
    [Authorize]
        [HttpGet]
        public ActionResult Details(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Customer customer = np.GetCustomers(id);
            return PartialView(customer);
        }

        [Authorize]
        [HttpGet]
        public ActionResult Search(string searchText)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if(searchText.StartsWith("0"))
            {
                searchText = "263" + searchText.Substring(1, searchText.Length - 1);
            }
            
            var px = np.GetCustomersSearch(searchText);
            return PartialView(px);
        } 
        //
        // GET: /Customers/Create
        [Authorize]
        [HttpGet]
        public ActionResult Create(string Company)
        {
           NHibernateDataProvider np = new NHibernateDataProvider();
           var pz = np.GetAllCustomers();
            var pp = (from e in pz select e ).LastOrDefault();
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

        //
        // POST: /Suppliers/Create
        [Authorize]
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
                    Random rr = new Random();
                    var pass = rr.Next(00000, 99999).ToString();
                    string Pin = en.encrypt(pass, "214ea9d5bda5277f6d1b3a3c58fd7034");

                    var uu = np.GetSyskeysByName("CustomerNetworking", part[1]);
                    //.Value.Trim();
                    string sk = "";
                    if (uu != null) sk = uu.Value;
                    var accs = db.Customers.Where(u => u.Phone1.Trim() == customer.Phone1).ToList();
                    if (accs == null || accs.Count == 0)
                    {
                        customer.Email = newAccount;
                        customer.Password = pass;
                        customer.CustomerName = customer.CustomerName.ToUpper();
                        if(sk == "Yes")
                        customer.ParentId = long.Parse(part[1]);
                        customer.Balance = 0;
                        customer.Wallet = 0;
                        customer.Purchases = 0;
                        customer.PurchasesToDate = 0;
                        //customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                        np.SaveOrUpdateCustomers(customer);

                       // XmlCustomers cus = new XmlCustomers();
                       // cus.CreateCustomerProfile(customer);
                      
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

        [Authorize]
        [HttpGet]
        public ActionResult IncomeCalculator()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            return PartialView();
        }
        [Authorize]
        [HttpGet]
        public ActionResult Payment(Customer customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            return PartialView(customer);
        }
        [Authorize]
        [HttpPost]
        public ActionResult Payment(string customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            return PartialView(customer);
        } 
        //
        // GET: /Suppliers/Edit/5

        [Authorize]
        [HttpGet]
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            ST_decrypt dec = new ST_decrypt();
            Customer customer = np.GetCustomers(id);
            if(customer != null)
            {
                customer.Email = dec.st_decrypt(customer.Email.Trim(), "214ea9d5bda5277f6d1b3a3c58fd7034");
            }
            return View(customer);
        }

        //
        // POST: /Suppliers/Edit/5
        [Authorize]
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
        
        [Authorize]
        [HttpGet]
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
        public ActionResult CustomerStatement(int ID)
        {
            custSt pd = new custSt();
            pd.cust = db.Customers.Find(ID);
            pd.Expenses = db.Expenses.Where(u => u.AccountCode == pd.cust.AccountCode).ToList(); 
            return PartialView(pd);
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