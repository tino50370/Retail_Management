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
    public class ValuationController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
         //
         // GET: /Items/

         public ActionResult Index(string id ,JQueryDataTableParamModel param)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var xx = db.Valuations.ToList();
             
             if (Request.IsAjaxRequest())
                 return PartialView(xx);
             return View("Index","_AdminLayout",xx);
         }

         public ActionResult Details(string id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var item = db.Assets.Find(id);
             return PartialView(item);
         }

         //
         // GET: /Items/Create

         public ActionResult Create(string Id)
         {
             NHibernateDataProvider np = new NHibernateDataProvider();
             var countries = db.RatingZones.ToList();
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
             Asset ff = new Asset();
            
             return PartialView(ff);
         }


         // POST: /Items/Create

         [HttpPost]
         [ValidateAntiForgeryToken]
         [ValidateInput(false)]
         public ActionResult Create(Asset item)
         {
             if (ModelState.IsValid)
             {
                 NHibernateDataProvider np = new NHibernateDataProvider();
                 item.FacilityCount = 0;
                 item.EquipmentCount = 0;
                 db.Assets.Add(item);
                 db.SaveChanges();
                 return RedirectToAction("Index", new  {id = item.CompanyId });
             }
             var countries = db.RatingZones.ToList();
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
             var countries = db.RatingZones.ToList();
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
             var countries = db.RatingZones.ToList();
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