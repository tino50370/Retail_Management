using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.DataAcess;

namespace RetailKing.Controllers
{   
    public class SalesController : Controller
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
            var px = np.GetAllSales();
            px = (from emp in px
                  where(emp.dated >= sdate && emp.dated < eedate)
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
             

            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);
        }

        [HttpPost]
        public ActionResult Index(string category ,string ItemCode,JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
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

            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }

            if (ItemCode != null && ItemCode != "")
            {
                var px = np.GetAllSales();
                px = (from emp in px
                      where (emp.dated >= sdate && emp.dated < eedate) && emp.customer == ItemCode + "%"
                      select emp).ToList();

                
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
                
                return PartialView(nn);
            }
            else 
            {
                var px = np.GetAllSales();
                px = (from emp in px
                      where (emp.dated >= sdate && emp.dated < eedate) 
                      select emp).ToList();


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
               return PartialView(nn);
            }
          
        }
        //
        // GET: /Items/Details/5

        [HttpPost]
        public JsonResult  newStock(string Qty, string ItemCode, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var itm = np.GetItemCode(ItemCode, Company);
            itm.NewStock = itm.NewStock + long.Parse(Qty);
            itm.Balance = itm.Balance + long.Parse(Qty);
            itm.Expected = itm.SellingPrice * itm.Balance;
            np.SaveOrUpdateItems(itm);
            return Json(new { Balance = itm.Balance, NewStock = itm.NewStock },
                JsonRequestBehavior.AllowGet);
        }

        public ActionResult Details(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = np.GetSalesLinesByReciept(id);
            return PartialView(item);
        }
        public ActionResult Hamper(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = np.GetSalesLinesByReciept(id);
            return PartialView(item);
        }
        //
        // GET: /Items/Create

        public ActionResult Create()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var comp = np.GetAllCompanies();
            var px = np.GetAccountsByCode("3");
            var category = (from e in px
                            where e.AccountCode.Length == 4
                            select e).ToList();

            var Subcategory = (from e in px
                            where e.AccountCode.Length == 8
                            select e).ToList();
            Item itm = new Item();
            itm.companies = comp;
            itm.categories = category;
            itm.SubCategories = Subcategory;
            return PartialView(itm);
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
        
        //
        // GET: /Items/Edit/5
 
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = np.GetItems(id);
            return PartialView(item);
        }

        //
        // POST: /Items/Edit/5

        [HttpPost]
        public ActionResult Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                np.SaveOrUpdateItems(item);
                return RedirectToAction("Index");
            }
            return PartialView(item);
        }

        //
        // GET: /Items/Delete/5
 
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


        #region daily payment method report


        public ActionResult paymentMethodReport(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            //Inventory inv = new Inventory();
            //JQueryDataTableParamModel param = new JQueryDataTableParamModel();

            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            var user = np.Getlogin(partz[1]);

            var compId = db.Companies.Where(u => u.name == user.Location).FirstOrDefault();



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

            var dateAndTime = DateTime.Now;
            var dat = dateAndTime.Date.ToString("dd/MM/yyyy");
            string sale = "SALE";
            var px = np.GetAllDailySales();
            px = (from emp in px
                  where (emp.Date.Contains(dat) && emp.CompanyId == compId.ID && emp.AccountName != "SALE                                                                                                ")
                  orderby emp.Sales descending
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


            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);
        }

        [HttpPost]
        public ActionResult paymentMethodReport(string category, string ItemCode, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            var user = np.Getlogin(partz[1]);

            var compId = db.Companies.Where(u => u.name == user.Location).FirstOrDefault();



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

            var dateAndTime = DateTime.Now;
            var dat = dateAndTime.Date.ToString("dd/MM/yyyy");

            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }

            if (ItemCode != null && ItemCode != "")
            {
                var px = np.GetAllDailySales();
                px = (from emp in px
                      where (emp.Date.Contains(dat) && emp.CompanyId == compId.ID)
                      orderby emp.Sales descending
                      select emp).ToList();


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

                return PartialView(nn);
            }
            else
            {
                var px = np.GetAllSales();
                px = (from emp in px
                      where (emp.dated >= sdate && emp.dated < eedate)
                      select emp).ToList();


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
                return PartialView(nn);
            }

        }


        #endregion



        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               // context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}