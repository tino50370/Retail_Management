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
    public class ServicesController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Suppliers/
        #region Service
        public ActionResult Index(string Company)
         {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();

            var px = db.YomoneyServices.ToList();
           // px = px.Where(u => u.Company == Company).ToList();
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
             var px = db.YomoneyServices.ToList();

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
            var service = db.YomoneyServices.Find(id);
            return PartialView(service);
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
            var px = db.Trantypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in px)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.type.Trim(),
                    Value = item.ID.ToString(),
                });
            }

            ViewBag.Trantype = yr;

            return PartialView();
        } 

        //
        // POST: /Suppliers/Create

        [HttpPost]
        public ActionResult Create(YomoneyService customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid)
            {
                var accs = db.YomoneyServices.Where(u => u.Name == customer.Name).ToList();
                if (accs == null || accs.Count == 0)
                {
                    customer.Name = customer.Name.ToUpper();
                    customer.ServiceProvider = customer.ServiceProvider.ToUpper();
                    db.YomoneyServices.Add(customer);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    ModelState.AddModelError("", "Sorry this service name is already in use");
                }
            }
            var px = db.Trantypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in px)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.type.Trim(),
                    Value = item.ID.ToString(),
                });
            }

            ViewBag.Trantype = yr;
            return PartialView(customer);
        }
        
        //
        // GET: /Suppliers/Edit/5
 
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            YomoneyService  customer = db.YomoneyServices.Find(id);
            var px = db.Trantypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in px)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.type.Trim(),
                    Value = item.ID.ToString(),
                });
            }

            ViewBag.Trantype = yr;
            return PartialView(customer);
        }

        //
        // POST: /Suppliers/Edit/5

        [HttpPost]
        public ActionResult Edit(YomoneyService customer)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                db.Entry(customer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            var px = db.Trantypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in px)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.type.Trim(),
                    Value = item.ID.ToString(),
                });
            }

            ViewBag.Trantype = yr;
            return PartialView(customer);
        }

        //
        // GET: /Suppliers/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            YomoneyService customer = db.YomoneyServices.Find(id);
            return View(customer);
        }

        //
        // POST: /Suppliers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            YomoneyService customer = db.YomoneyServices.Find(id);
            db.Entry(customer).State = EntityState.Deleted;
            db.SaveChanges();

            return RedirectToAction("Index");
        }
        #endregion

        #region Service action types

        public ActionResult ActionTypes(string ServiceType)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();

            var px = db.ServiceActionTypes.Where(u => u.ServiceType.Trim() == ServiceType.Trim()).ToList();
            // px = px.Where(u => u.Company == Company).ToList();
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

           // ViewData["Company"] = Company;

            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);

        }

        public ActionResult CreateActionType(string ServiceType)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            var px = db.Trantypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in px)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.type.Trim(),
                    Value = item.ID.ToString(),
                });
            }

            ViewBag.Trantype = yr;
            var yx = new List<YomoneyService>();
            if (partz[0] == "5-0001-0000000")
            {
                 yx = db.YomoneyServices.ToList();
            }
            else
            {
                 yx = db.YomoneyServices.Where(u => u.IsSupplierService == true).ToList();
            }
            
            List<SelectListItem> ys = new List<SelectListItem>();

            foreach (var item in yx)
            {
                ys.Add(new SelectListItem
                {
                    Text = item.Name.Trim(),
                    Value = item.Id.ToString(),
                });
            }

            ViewBag.YmService = ys;

            if (string.IsNullOrEmpty(ServiceType)) ServiceType = "Merchant";
            ServiceActionType sx = new ServiceActionType();
            sx.ServiceType = ServiceType;
            return PartialView(sx);
        }

        [HttpPost]
        public ActionResult CreateActionType(ServiceActionType customer)
        {
            var px = db.Trantypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in px)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.type.Trim(),
                    Value = item.ID.ToString(),
                });
            }

            ViewBag.Trantype = yr;

            var yx = db.YomoneyServices.ToList();
            List<SelectListItem> ys = new List<SelectListItem>();

            foreach (var item in yx)
            {
                ys.Add(new SelectListItem
                {
                    Text = item.Name.Trim(),
                    Value = item.Id.ToString(),
                });
            }

            ViewBag.YmService = ys;

            if (ModelState.IsValid)
            {
                var accs = db.ServiceActionTypes.Where(u => u.ActionName == customer.ActionName).ToList();
                if (accs == null || accs.Count == 0)
                {
                    customer.ActionName  = customer.ActionName.ToUpper();
                    db.ServiceActionTypes.Add(customer);
                    db.SaveChanges();
                    ServiceActionType sx = new ServiceActionType();
                    sx.ServiceType = customer.ServiceType;
                    return PartialView(sx);
                }
                else
                {
                    ModelState.AddModelError("", "Sorry this service name is already in use");
                }
            }
            
            return PartialView(customer);
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