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
    public class ProjectController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
         //
         // GET: /Items/

         public ActionResult Index(string id ,JQueryDataTableParamModel param)
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
             var px = np.GetAllAssets();


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

         public ActionResult Tickets(string id, JQueryDataTableParamModel param)
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
             var px = db.Projects.ToList();


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

         public ActionResult Details(string id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var item = db.Assets.Find(id);
             return PartialView(item);
         }

         //
         // GET: /Items/Create

         public ActionResult CreateTicket(string Id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             Project ff = new Project();
             var comp = np.GetAllCompanies();
             var countries = db.Assets.ToList();
             List<SelectListItem> yr = new List<SelectListItem>();
             yr.Add(new SelectListItem
             {
                 Text = "Select A Site",
                 Value = "Select A Site"
             });
             foreach (var item in countries)
             {
                 yr.Add(new SelectListItem
                 {
                     Text = item.Name,
                     Value = item.Id.ToString()
                 });
             }

             ViewBag.Assets = yr;
             return PartialView(ff);
         }

       
         // POST: /Items/Create
        public ActionResult GetFacilities(long Id)
         {
                 NHibernateDataProvider np = new NHibernateDataProvider();
                 var fx =  db.Facilities.Where(u => u.AssetId == Id).ToList();
                 return PartialView(fx);
         }

         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Create(Project item)
         {
             if (ModelState.IsValid)
             {
                 NHibernateDataProvider np = new NHibernateDataProvider();
                 item.Status  = "Request";
                 db.Projects.Add(item);
                 db.SaveChanges();
                 return RedirectToAction("Tickets");
             }

             return PartialView(item);
         }

         //
         // GET: /Items/Edit/5

         public ActionResult Edit(int id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var item = db.Assets.Find(id);
             return PartialView(item);
         }

         //
         // POST: /Items/Edit/5

         [HttpPost]
         public ActionResult Edit(Feature item)
         {
             if (ModelState.IsValid)
             {
                 NHibernateDataProvider np = new NHibernateDataProvider();
                 db.Entry(item).State = EntityState.Modified;
                 db.SaveChanges();
                 return RedirectToAction("Index");
             }
             return PartialView(item);
         }

         //
         // GET: /Items/Delete/5

         public ActionResult Delete(int id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var item = db.Assets.Find(id);
             return View(item);
         }

         //
         // POST: /Items/Delete/5

         [HttpPost, ActionName("Delete")]
         public ActionResult DeleteConfirmed(int id)
         {
             Feature variation = db.Features.Find(id);
             db.Features.Remove(variation);
             db.SaveChanges();
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