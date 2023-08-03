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
    public class OrdersController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Items/

       public ActionResult Index(string date, string enddate,string category ,string ItemCode, string company, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            //Inventory inv = new Inventory();
            //JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            if (param.sStart == "" || param.sStart == null)
            {
                param.sStart = DateTime.Today.ToString("yyyy-MM-dd");
            }
            else
            {
                param.sStart = DateTime.Parse(param.sStart).ToString("yyyy-MM-dd");
            }
            if (param.sEnd == "" || param.sEnd == null)
            {
                param.sEnd = DateTime.Today.ToString("yyyy-MM-dd");
            }
            else
            {
                param.sEnd = DateTime.Parse(param.sEnd).ToString("yyyy-MM-dd");
            }
            
              DateTime sdate = DateTime.Parse(param.sStart);
              DateTime edate = DateTime.Parse(param.sEnd);
              DateTime eedate = edate.AddDays(1);
            var px = np.GetAllOrders();
            px = (from emp in px
                  where (emp.dated >= sdate && emp.dated < eedate)
                  select emp).ToList();
                         
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
            ViewData["sdate"] = DateTime.Parse(param.sStart).ToString("dd MMM yyyy");
            ViewData["edate"] = DateTime.Parse(param.sEnd).ToString("dd MMM yyyy");
            ViewData["listSize"] = param.iDisplayLength;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;
            var cat = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
            var SubCategories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            foreach (var item in cat)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.AccountName  ,
                    Value = item.AccountName.Trim()
                });
            }

            ViewBag.Cat = yr;

            List<SelectListItem> yt = new List<SelectListItem>();
            foreach (var item in cat)
            {
                yt.Add(new SelectListItem
                {
                    Text = item.AccountName,
                    Value = item.AccountName.Trim()
                });
            }

            ViewBag.SCat = yt;

            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);
        }

        [HttpPost]
        public ActionResult Index(string category ,string ItemCode,JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            
            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }
            
            if (category == "" && ItemCode == "")
            {
                var px = np.GetAllItems();

                var nn = (from u in px.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                          select u).ToList();

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
                ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + nn.Count;
                Inventory inv = new Inventory();
                inv.categories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
                inv.SubCategories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
                inv.items = nn;
                return PartialView(inv);
            }
            else if (category != "")
            {
                var xx = np.GetAllItems();

                var px = (from ee in xx
                         .Where(ee => ee.category == category.Trim())
                          select ee).ToList();

                var nn = (from u in px.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                          select u).ToList();

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
                ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + nn.Count;
                Inventory inv = new Inventory();
                inv.categories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
                inv.SubCategories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
                inv.items = nn;
                return PartialView(inv);
            }
            else if (ItemCode == "")
            {
                var xx = np.GetAllItems();

                var px = (from ee in xx
                         .Where(ee => ee.ItemCode == ItemCode.Trim())
                          select ee).ToList();

                var nn = (from u in px.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                          select u).ToList();

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
                ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + nn.Count;
                Inventory inv = new Inventory();
                inv.categories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
                inv.SubCategories =(from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
                inv.items = nn;
                return PartialView(inv);
            }
            return PartialView();
                
        }
        //
        // GET: /Items/Details/5

     
        public ActionResult Details(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = np.GetOrderLinesByReciept(id);
            ViewData["Id"] = id;
            return PartialView(item);
        }

        public ActionResult Dispatch(string Id, string item,string company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            char[] delimiters = new char[] { '/' };
            string[] parts = item.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            List<Printdata> pds = new List<Printdata>();
            foreach (var itm in parts)
            {
                char[] delimite = new char[] { ',' };
                string[] par = itm.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                var sal = np.GetOrderLines(long.Parse(par[1]));
                var sls = np.GetSalesLinesByReciept(sal.Reciept);
                var sl = (from e in sls 
                          .Where(e => e.ItemCode == sal.ItemCode )
                          select e).FirstOrDefault();
                sl.Description  = par[0];
                np.SaveOrUpdateSalesLines(sl);
                np.DeleteOrderLines(sal);

                Printdata pd = new Printdata();
                pd.item = sl.item;
                pd.qty = sl.quantity.ToString();
                pd.amount = sl.priceinc.ToString();
                pd.prize = sl.Description;
                pds.Add(pd);
            }

            var ord = np.GetOrderByReciept(Id).FirstOrDefault();
            np.DeleteOrders(ord);
            var toprint = np.GetSalesLinesByReciept(Id);
            var comp = np.GetCompanies(company.Trim());

            ReceiptVM receipt = new ReceiptVM();
            receipt.posd = pds;
            receipt.company = comp;
            return PartialView(receipt);
        }

        //
        // GET: /Items/Create

        public ActionResult Create()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var comp = np.GetAllCompanies();
            
            return PartialView();
        }

        
        // POST: /Items/Create

        [HttpPost]
        public ActionResult Create(Item item)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                np.SaveOrUpdateItems(item);
                return RedirectToAction("Index");  
            }

            return PartialView(item);
        }
         
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = np.GetItems(id);
            return View(item);
        }

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