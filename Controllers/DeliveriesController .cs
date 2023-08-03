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
    public class DeliveriesController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Items/
        [Authorize]
        [HttpGet]
        public ActionResult Index(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
        {
            // list of all ready deliveries
          
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
            var px = db.Deliveries.Where(u => u.DeliveryType.Trim() == "Delivery" && u.Status == "R").ToList();
            px = (from emp in px
                  where (emp.DeadLine.Value.Date  >= sdate.Date  && emp.DeadLine.Value.Date <= eedate.Date )
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

        [Authorize]
        [HttpGet]
        public ActionResult DeliverySammery(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
        {
            // list of all ready deliveries

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

            DateTime sdate = DateTime.ParseExact(param.sStart, "yyyy-MM-dd",null);
            DateTime edate = DateTime.ParseExact(param.sEnd, "yyyy-MM-dd",null);
            DateTime eedate = edate.AddDays(2);
            var px = db.SalesLineSammaries.Where(u => u.DeliveryType.Trim() == "Delivery" && u.DeliveryPicking == false).ToList();
            px = (from emp in px
                  where (emp.Deadline.Value.Date >= sdate.Date && emp.Deadline.Value.Date <= eedate.Date)
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
        [Authorize]
        [HttpGet]
        public ActionResult DeliveryPickingList(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
        {
            // list of all ready deliveries
            
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

            DateTime sdate = DateTime.ParseExact(param.sStart, "yyyy-MM-dd", null);
            DateTime edate = DateTime.ParseExact(param.sEnd, "yyyy-MM-dd",null);
            DateTime eedate = edate.AddDays(1);
            var px = db.Deliveries.Where(u => u.DeliveryType.Trim() == "Delivery" && u.Status == "O").ToList();
            px = (from emp in px
                  where (emp.DeadLine.Value.Date  >= sdate.Date  && emp.DeadLine.Value.Date  <= eedate.Date)
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

        [Authorize]
        [HttpGet]
        public ActionResult Collections(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
        {
            // list of all ready collections
          
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
            var px = db.Deliveries.Where(u => u.DeliveryType.Trim() == "Collection" && u.Status == "R").ToList();
            px = (from emp in px
                  where (emp.DeadLine.Value.Date  >= sdate.Date  && emp.DeadLine.Value.Date  <= eedate.Date )
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

        [Authorize]
        [HttpGet]
        public ActionResult CollectionsSammary(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
        {
            // list of all ready collections
            
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
            var px = db.SalesLineSammaries.Where(u => u.DeliveryType.Trim() == "Collection" && u.DeliveryPicking == false).ToList();
            px = (from emp in px
                  where (emp.Deadline.Value.Date  >= sdate.Date  && emp.Deadline.Value.Date  <= eedate.Date )
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

        [Authorize]
        [HttpGet]
        public ActionResult CollectionPickingList(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
        {
            // list of all ready deliveries
            // list of all ready collections
         
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
            var px = db.Deliveries.Where(u => u.DeliveryType.Trim() == "Collection" && u.Status == "O").ToList();
            px = (from emp in px
                  where (emp.DeadLine  >= sdate )
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
  
        //
        // GET: /Items/Create
        public ActionResult PickHamper(long id, bool chec, int size)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var comp = db.Sales_Lines.Find(id);
            comp.Picked = chec;
            var cd = db.PickinChecks.Where(u => u.Reciept == comp.Reciept).FirstOrDefault();
            
            if (cd == null)
            {
                cd = new PickinCheck();
                cd.Reciept = comp.Reciept;
                cd.Lines = size;
                cd.Picled = 1;
                db.PickinChecks.Add(cd);
                db.SaveChanges();
                
            }
            else
            {
                if (chec == true)
                {
                    cd.Picled = cd.Picled + 1;
                    db.Entry(cd).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    cd.Picled = cd.Picled - 1;
                    db.Entry(cd).State = EntityState.Modified;
                    db.SaveChanges();
                }
            }
             if(cd.Lines == cd.Picled )
             {
                 var del = db.Deliveries.Where(u => u.Receipt == comp.Reciept).FirstOrDefault();
                del.Status = "R";
                db.Entry(del).State = EntityState.Modified;
                db.SaveChanges();
             }
            db.Entry(comp).State = EntityState.Modified;
            db.SaveChanges();
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult PickSupplier(long id, bool chec)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var comp = db.SalesLineSammaries.Find(id);
           
            comp.SuppierPicking = chec;
           
            db.Entry(comp).State = EntityState.Modified;
            db.SaveChanges();

            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult PickDelivery(long id, bool chec)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var comp = db.SalesLineSammaries.Find(id);
            comp.DeliveryPicking  = chec;
            db.Entry(comp).State = EntityState.Modified;
            db.SaveChanges();
            return Json("",JsonRequestBehavior.AllowGet );
        }

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
        
        //
        // GET: /Items/Edit/5
 
 
        public ActionResult EmptyCart(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;

                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                var cust = db.Customers.Find(long.Parse(part[1]));
                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                
                var ols = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept.Trim()).ToList();

                db.Orders.Remove(oda);
                db.SaveChanges();
                foreach(var item  in ols)
                {
                    db.OrderLines.Remove(item);
                    db.SaveChanges();
                }

                return RedirectToAction("Cartitems");
            }
            else
            {
                return Json("0", JsonRequestBehavior.AllowGet); ;
            }
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