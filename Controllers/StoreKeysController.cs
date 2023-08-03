using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;

namespace RetailKing.Controllers
{
    public class StoreKeysController : Controller
    {
        RetailKingEntities db = new RetailKingEntities();

        //
        // GET: /Cities/

        public ActionResult Index()
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string Company = part[2];
            return PartialView(db.Storekeys.Where(u => u.Company ==Company).ToList());
        }

        //
        // GET: /Cities/Details/5

        public ActionResult Details(long id = 0)
        {
            var city = db.Storekeys.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }

        //
        // GET: /Cities/Create

        public ActionResult Create()
        {
            return PartialView();
        }

        //
        // POST: /Cities/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Storekey city)
        {
            if (ModelState.IsValid)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                city.Company = part[2]; 
                db.Storekeys.Add(city);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return PartialView(city);
        }


        // GET: /Cities/Edit/5

        public ActionResult Edit(long id = 0)
        {
            var city = db.Storekeys.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return PartialView(city);
        }

        //
        // POST: /Cities/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Storekey city)
        {
            if (ModelState.IsValid)
            {
                var sysk = db.Storekeys.Find(city.Id);
                sysk.Value = city.Value;
                db.Entry(sysk).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return PartialView(city);
        }

        //
        // GET: /Cities/Delete/5

        public ActionResult Delete(long id = 0)
        {
            var city = db.Storekeys.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        //
        // POST: /Cities/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            var city = db.Storekeys.Find(id);
            db.Storekeys.Remove(city);
            db.SaveChanges();
            return RedirectToAction("Index");
        }


        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}