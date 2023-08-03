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
    public class FacilityController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
         //
         // GET: /Items/
         #region facility
         public ActionResult Index(long Id,string Company, JQueryDataTableParamModel param)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();

             ViewData["AssetId"] = Id;
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
             var px = db.Facilities.Where(u => u.AssetId == Id && u.ParentId == null).ToList();


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
             return View("Index","_AdminLayout",nn);
         }

         public ActionResult Details(long id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var item = db.Facilities.Find(id);
             return PartialView(item);
         }

         //
         // GET: /Items/Create

         public ActionResult Create(long Id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var countries = db.FacilityTypes.ToList();
             List<SelectListItem> yr = new List<SelectListItem>();

             foreach (var item in countries)
             {
                 yr.Add(new SelectListItem
                 {
                     Text = item.Name,
                     Value = item.Id.ToString()
                 });
             }

             ViewBag.Company = yr;
             Facility ff = new Facility();

             ff.AssetId = Id;
             return PartialView(ff);
         }


         // POST: /Items/Create

         [HttpPost]
         [ValidateAntiForgeryToken]
         [ValidateInput(false)]
         public ActionResult Create(Facility item)
         {
             if (ModelState.IsValid)
             {
                 NHibernateDataProvider np = new NHibernateDataProvider();
                 item.FeatureCount = 0;
                 item.EquipmentCount = 0;
                 db.Facilities.Add(item);
                 db.SaveChanges();
                 var px = db.Assets.Find(item.AssetId);
                 px.FacilityCount += 1;
                 db.Entry(px).State = EntityState.Modified;
                 db.SaveChanges();

                 return RedirectToAction("Index", new  {id = item.AssetId});
             }
             var countries = db.FacilityTypes.ToList();
             List<SelectListItem> yr = new List<SelectListItem>();

             foreach (var itm in countries)
             {
                 yr.Add(new SelectListItem
                 {
                     Text = itm.Name,
                     Value = itm.Id.ToString()
                 });
             }

             ViewBag.Company = yr;
             return PartialView(item);
         }

         //
         // GET: /Items/Edit/5

         public ActionResult Edit(int id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var countries = db.FacilityTypes.ToList();
             List<SelectListItem> yr = new List<SelectListItem>();

             foreach (var itm in countries)
             {
                 yr.Add(new SelectListItem
                 {
                     Text = itm.Name,
                     Value = itm.Id.ToString()
                 });
             }

             ViewBag.Company = yr;
             var item = db.Facilities.Find(id);
             return PartialView(item);
         }

         //
         // POST: /Items/Edit/5

         [HttpPost]
         [ValidateInput(false)]
         public ActionResult Edit(Facility item)
         {
             if (ModelState.IsValid)
             {
                 NHibernateDataProvider np = new NHibernateDataProvider();
                 db.Entry(item).State = EntityState.Modified;
                 db.SaveChanges();
                 return RedirectToAction("Index", new  {id = item.AssetId});
             }
             var countries = db.FacilityTypes.ToList();
             List<SelectListItem> yr = new List<SelectListItem>();

             foreach (var itm in countries)
             {
                 yr.Add(new SelectListItem
                 {
                     Text = itm.Name,
                     Value = itm.Id.ToString()
                 });
             }

             ViewBag.Company = yr;
             return PartialView(item);
         }

         //
         // GET: /Items/Delete/5

         public ActionResult Delete(int id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var item = db.Facilities.Find(id);
             return View(item);
         }

         //
         // POST: /Items/Delete/5

         [HttpPost, ActionName("Delete")]
         public ActionResult DeleteConfirmed(long id)
         {
             Facility variation = db.Facilities.Find(id);
             db.Facilities.Remove(variation);
             db.SaveChanges();
             return RedirectToAction("Index");
         }
        #endregion

         #region Feature
         public ActionResult Children(long Id, JQueryDataTableParamModel param)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();

             ViewData["ParentId"] = Id;
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
             var px = db.Facilities.Where(u => u.ParentId == Id).ToList();


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
             return View("Index", "_AdminLayout", nn);
         }

         public ActionResult DetailsChild(long id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var item = db.Facilities.Find(id);
             return PartialView(item);
         }

         //
         // GET: /Items/Create

         public ActionResult CreateChild(long Id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var countries = db.FacilityTypes.ToList();
             List<SelectListItem> yr = new List<SelectListItem>();

             foreach (var item in countries)
             {
                 yr.Add(new SelectListItem
                 {
                     Text = item.Name,
                     Value = item.Id.ToString()
                 });
             }

             ViewBag.Company = yr;
             var fc = np.GetFacility(Id);
             Facility ff = new Facility();
             ff.ParentId = Id;
             ff.AssetId = fc.AssetId;
             return PartialView(ff);
         }


         // POST: /Items/Create

         [HttpPost]
         [ValidateAntiForgeryToken]
         [ValidateInput(false)]
         public ActionResult CreateChild(Facility item)
         {
             if (ModelState.IsValid)
             {
                 NHibernateDataProvider np = new NHibernateDataProvider();
                 item.FeatureCount = 0;
                 item.EquipmentCount = 0;
                 db.Facilities.Add(item);
                 db.SaveChanges();
                 var px = db.Facilities.Find(item.ParentId);
                 px.FeatureCount += 1;
                 db.Entry(px).State = EntityState.Modified;
                 db.SaveChanges();
                 return RedirectToAction("Children", new { id = item.AssetId });
             }
             var countries = db.FacilityTypes.ToList();
             List<SelectListItem> yr = new List<SelectListItem>();

             foreach (var itm in countries)
             {
                 yr.Add(new SelectListItem
                 {
                     Text = itm.Name,
                     Value = itm.Id.ToString()
                 });
             }

             ViewBag.Company = yr;
             return PartialView(item);
         }

         //
         // GET: /Items/Edit/5

         public ActionResult EditChild(int id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var countries = db.FacilityTypes.ToList();
             List<SelectListItem> yr = new List<SelectListItem>();

             foreach (var itm in countries)
             {
                 yr.Add(new SelectListItem
                 {
                     Text = itm.Name,
                     Value = itm.Id.ToString()
                 });
             }

             ViewBag.Company = yr;
             var item = db.Facilities.Find(id);
             return PartialView(item);
         }

         //
         // POST: /Items/Edit/5

         [HttpPost]
         [ValidateInput(false)]
         public ActionResult EditChild(Facility item)
         {
             if (ModelState.IsValid)
             {
                 NHibernateDataProvider np = new NHibernateDataProvider();
                 db.Entry(item).State = EntityState.Modified;
                 db.SaveChanges();
                 return RedirectToAction("Index", new { id = item.AssetId });
             }
             var countries = db.FacilityTypes.ToList();
             List<SelectListItem> yr = new List<SelectListItem>();

             foreach (var itm in countries)
             {
                 yr.Add(new SelectListItem
                 {
                     Text = itm.Name,
                     Value = itm.Id.ToString()
                 });
             }

             ViewBag.Company = yr;
             return PartialView(item);
         }

         //
         // GET: /Items/Delete/5

         public ActionResult DeleteChild(int id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var item = db.Facilities.Find(id);
             return View(item);
         }

         //
         // POST: /Items/Delete/5

         [HttpPost, ActionName("Delete")]
         public ActionResult DeleteConfirmedChild(long id)
         {
             Facility variation = db.Facilities.Find(id);
             db.Facilities.Remove(variation);
             db.SaveChanges();
             return RedirectToAction("Index");
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