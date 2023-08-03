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
    public class ItemVariationsController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Items/

       public ActionResult Index(string ItemCode, JQueryDataTableParamModel param)
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
            var px = db.ItemVariations.Where(u=> u.ItemCode.Trim() == ItemCode.Trim()).ToList();
           
                         
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

        public ActionResult Details(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.ItemVariations.Find(id);
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
        [ValidateAntiForgeryToken]
        public ActionResult Create(ItemVariation item)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                db.ItemVariations.Add(item);
                db.SaveChanges();
                return RedirectToAction("Index");  
            }

            return PartialView(item);
        }
        
        //
        // GET: /Items/Edit/5
 
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.ItemVariations.Find(id);
            return PartialView(item);
        }

        //
        // POST: /Items/Edit/5

        [HttpPost]
        public ActionResult Edit(ItemVariation  item)
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
            var item = db.ItemVariations.Find(id);
            return View(item);
        }

        //
        // POST: /Items/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            ItemVariation variation = db.ItemVariations.Find(id);
            db.ItemVariations.Remove(variation);
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