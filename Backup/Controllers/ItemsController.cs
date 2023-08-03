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
    public class ItemsController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();

        //
        // GET: /Items/

        public ActionResult Index()
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Inventory inv = new Inventory();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllItems();
                         
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
            inv.categories = np.GetAllCategory();
            inv.items = nn;

            if (Request.IsAjaxRequest())
                return PartialView(inv);
            return View(inv);
        }

        [HttpPost]
        public ActionResult Index(string category ,string ItemCode,JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            
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
                inv.categories = np.GetAllCategory();
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
                inv.categories = np.GetAllCategory();
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
                inv.categories = np.GetAllCategory();
                inv.items = nn;
                return PartialView(inv);
            }
            return PartialView();
                
        }
        //
        // GET: /Items/Details/5

        [HttpPost]
        public JsonResult  newStock(string Qty, string ItemCode, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            var itm = np.GetItemCode(ItemCode, Company);
            itm.NewStock = itm.NewStock + long.Parse(Qty);
            itm.Balance = itm.Balance + long.Parse(Qty);
            itm.Expected = itm.SellingPrice * itm.Balance;
            np.SaveOrUpdateItems(itm);
            return Json(new { Balance = itm.Balance, NewStock = itm.NewStock },
                JsonRequestBehavior.AllowGet);
        }

        public ViewResult Details(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Items item = np.GetItems(id);
            return View(item);
        }

        //
        // GET: /Items/Create

        public ActionResult Create()
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            var comp = np.GetAllCompanies();
            var category = np.GetAllCategory();
            Items itm = new Items();
            itm.companies = comp;
            itm.categories = category;
            return View(itm);
        }

        [HttpPost]
        public JsonResult GetCategories(string json_str)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            //System.Web.Script.Serialization.JavaScriptSerializer j = new System.Web.Script.Serialization.JavaScriptSerializer();

            var currentUserId = User.Identity.Name;
           // Subjectx ax = new Subjectx();
          //  ax = (Subjectx)j.Deserialize(json_str, typeof(Subjectx));
            var px = np.GetCategoryByLocation(json_str);

            var subs = (from x in px
                        select new { Id = x.name, Name = x.name.Trim() });

            return Json(subs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSubCategories(string json_str)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            //System.Web.Script.Serialization.JavaScriptSerializer j = new System.Web.Script.Serialization.JavaScriptSerializer();

            var currentUserId = User.Identity.Name;
            // Subjectx ax = new Subjectx();
            //  ax = (Subjectx)j.Deserialize(json_str, typeof(Subjectx));
            var px = np.GetSubCategory(json_str);

            var subs = (from x in px
                        select new { Id = x.name , Name = x.name.Trim() });

            return Json(subs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetItemsByCategory(string json_str)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            //System.Web.Script.Serialization.JavaScriptSerializer j = new System.Web.Script.Serialization.JavaScriptSerializer();

            var currentUserId = User.Identity.Name;
            // Subjectx ax = new Subjectx();
            //  ax = (Subjectx)j.Deserialize(json_str, typeof(Subjectx));
            var px = np.GetSubCategory(json_str);

            var subs = (from x in px
                        select new { Id = x.name, Name = x.name.Trim() });

            return Json(subs, JsonRequestBehavior.AllowGet);
        }
        //
        // POST: /Items/Create

        [HttpPost]
        public ActionResult Create(Items item)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
                np.SaveOrUpdateItems(item);
                return RedirectToAction("Index");  
            }

            return PartialView(item);
        }
        
        //
        // GET: /Items/Edit/5
 
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Items item = np.GetItems(id);
            return View(item);
        }

        //
        // POST: /Items/Edit/5

        [HttpPost]
        public ActionResult Edit(Items item)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
                np.SaveOrUpdateItems(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        //
        // GET: /Items/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Items item = np.GetItems(id);
            return View(item);
        }

        //
        // POST: /Items/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Items item = np.GetItems(id);
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