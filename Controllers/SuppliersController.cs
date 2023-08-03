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
    public class SuppliersController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Suppliers/

         public ActionResult Index(string Company)
         {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
             
            var px = np.GetAllSuppliers();
            px = px.Where(u => u.Company == Company).ToList();
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
             var px = np.GetAllSuppliers();

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

        public ActionResult Search(string searchText)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var px = np.GetSuppliersSearch(searchText);
            return PartialView(px);
        }

        public ActionResult Browse(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var pp = np.GetAllSuppliers();
            var px = (from e in pp.Where(u => u.Company == Company) select e).ToList();
            return PartialView(px);
        } 
        //
        // GET: /Suppliers/Create

        public ActionResult Create( string Company)
        {
           NHibernateDataProvider np = new NHibernateDataProvider();
           var pz = np.GetAllSuppliers();
           var pp = (from e in pz select e).LastOrDefault();
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
                var accs = np.GetSuppliersByName(customer.SupplierName);
                if (accs == null || accs.Count == 0)
                {
                    customer.SupplierName  = customer.SupplierName.ToUpper();
                    customer.ContactPerson = customer.ContactPerson.ToUpper();
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
                    // Check is exists 
                 
                    return RedirectToAction("Index", new { Company = customer.Company });
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
            return PartialView(customer);
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
            return PartialView(customer);
        }

        //
        // GET: /Suppliers/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Supplier customer = np.GetSuppliers(id);
            return PartialView(customer);
        }

        //
        // POST: /Suppliers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Supplier customer = np.GetSuppliers(id);
            np.SaveOrUpdateSuppliers(customer);
            
            return PartialView("Index");
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