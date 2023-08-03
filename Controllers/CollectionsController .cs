using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.ViewModels;
using RetailKing.DataAcess;

namespace RetailKing.Controllers
{   
    public class CollectionsController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Items/

        [HttpGet]
        public ActionResult Index()
        {
            return View(db.CollectionPoints.ToList());          
        }

        [HttpGet]
        public ActionResult Phone(string Id)
        {
            ViewData["ItemId"] = Id ;
            return View();

        }
        //
        // GET: /Items/Create
        [HttpGet]
        public ActionResult UserReceipt(string Id)
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                var CurrentUser = User.Identity.Name;
                ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                ViewData["Document"] = "Copy Receipt";
                char[] delimiters = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                var cust = db.Customers.Find(long.Parse(part[1]));
                var oda = db.Sales.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.Reciept.Trim() == Id).FirstOrDefault();
                var odalines = db.Sales_Lines.Where(u => u.Reciept.Trim() == oda.Reciept).ToList();

                var sal = new Order();
                sal.Account = oda.Account;
                sal.company = oda.company;
                sal.customer = oda.customer;
                sal.dated = DateTime.Now;
                sal.discount = oda.discount;
                sal.total = oda.total;
                sal.DeliveryType = oda.DeliveryType;
                sal.CollectionId = oda.CollectionId;
                sal.Reciept = oda.Reciept;
                sal.Tax = oda.Tax;
                sal.state = oda.state;

                List<OrderLine> ols = new List<OrderLine>();
                foreach(var itm in odalines)
                {
                    var sls = new OrderLine();
                    sls.Reciept = sal.Reciept;
                    sls.item = itm.item;
                    sls.ItemCode = itm.ItemCode;
                    sls.Category = itm.Category;
                    sls.SubCategory = itm.SubCategory;
                    sls.price = itm.price;
                    sls.priceinc = itm.priceinc;
                    sls.quantity = itm.quantity;
                    sls.SubCategory = itm.SubCategory;
                    sls.Category = itm.Category;
                    sls.Dated = sal.dated;
                    sls.Discount = itm.Discount;
                    sls.tax = itm.tax;
                    sls.Company = itm.Company;
                    ols.Add(sls);
                }
                ViewData["OrderNumber"] = oda.Reciept;
                var mm = DateTime.Now.Month;
                if (cust.Period != mm)// change of month we start afresh
                {
                    Monthend mend = new Monthend();
                    mend.ChangePeriod(cust);
                }
      
               
                var regionz = db.Regions.Find(cust.RegionId);
                var surburb = db.Suburbs.Find(long.Parse(cust.Suburb));
                var city = db.Cities.Find(long.Parse(cust.City));
                cust.Suburb = surburb.Name;
                cust.City = city.Name;

                var resp = new ShoppingCart();
                resp.orderlines = ols;
                resp.customer = cust;
                resp.region = regionz;
                resp.order = sal;
                return View("UserReceipt", "_PrintLayout", resp);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Phone(long Id ,string phone)
        {
           var ol= db.OrderLines.Find(Id);
           ol.Description = phone;
           db.Entry(ol).State = EntityState.Modified;
           db.SaveChanges();

           return Json(phone, JsonRequestBehavior.AllowGet);
        }
        
        //
        // GET: /Items/Edit/5
 
 
        //
        // POST: /Items/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = np.GetItems(id);
            np.DeleteItems(item); 
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