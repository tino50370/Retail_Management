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
    public class CitiesController : Controller
    {
        RetailKingEntities db = new RetailKingEntities();

        //
        // GET: /Cities/

        public ActionResult Index()
        {
            return PartialView(db.Cities.ToList());
        }

        //
        // GET: /Cities/Details/5

        public ActionResult Details(long id = 0)
        {
            City city = db.Cities.Find(id);
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
        public ActionResult Create(City city)
        {
            if (ModelState.IsValid)
            {
                db.Cities.Add(city);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return PartialView(city);
        }

        public ActionResult Regions(string CityId)
        {
            var px = new List<Region>();
            if (string.IsNullOrEmpty(CityId))
            {
                px = db.Regions.ToList();
            }
            else
            {
                var ct = db.Cities.Where(u => u.Name.Trim() == CityId.Trim()).FirstOrDefault();
                px = db.Regions.Where(u => u.CityId == ct.Id).ToList();
                return Json(px, JsonRequestBehavior.AllowGet);
            }

            return PartialView(px);
        }

        public ActionResult CreateRegion()
        {
            var cities = db.Cities.ToList();

            List<SelectListItem> ct = new List<SelectListItem>();
            ct.Add(new SelectListItem
            {
                Text = "-Select City-",
                Value = "-Select City-"
            });
            foreach (var item in cities)
            {
                ct.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Cities = ct;
            var cid = cities.FirstOrDefault().Id;

            return PartialView();
        }

        //
        // POST: /Cities/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateRegion(Region city)
        {
            var cities = db.Cities.ToList();

            List<SelectListItem> ct = new List<SelectListItem>();
            ct.Add(new SelectListItem
            {
                Text = "-Select City-",
                Value = "-Select City-"
            });
            foreach (var item in cities)
            {
                ct.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Cities = ct;

            var cid = cities.FirstOrDefault().Id;
            if (ModelState.IsValid)
            {
                db.Regions.Add(city);
                db.SaveChanges();
                return PartialView();
            }


            return PartialView(city);
        }

        public ActionResult Suburbs(long regionId = 0)
        {
            var px = new List<Suburb>();
            if (regionId == 0)
            {
                px = db.Suburbs.ToList();
            }
            else
            {
                px = db.Suburbs.Where(u => u.RegionId == regionId).ToList();
                return Json(px, JsonRequestBehavior.AllowGet);
            }

            return PartialView(px);
        }

        public ActionResult PointbyCity(string CityId)
        {
            var px = new List<CollectionPoint>();
            if (string.IsNullOrEmpty(CityId))
            {
                px = db.CollectionPoints.ToList();
            }
            else
            {
                px = db.CollectionPoints.Where(u => u.City == CityId).ToList();
                return Json(px, JsonRequestBehavior.AllowGet);
            }

            return PartialView(px);
        }

        public ActionResult CreateSuburbs()
        {
            var cities = db.Cities.ToList();

            List<SelectListItem> ct = new List<SelectListItem>();
            ct.Add(new SelectListItem
            {
                Text = "-Select City-",
                Value = "-Select City-"
            });
            foreach (var item in cities)
            {
                ct.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Cities = ct;
            var cid = cities.FirstOrDefault().Id;

            var area = db.Regions.Where(u => u.CityId == cid).ToList();

            List<SelectListItem> rg = new List<SelectListItem>();
            rg.Add(new SelectListItem
            {
                Text = "-Select Region-",
                Value = "-Select Region-"
            });
            foreach (var item in area)
            {
                rg.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Area = rg;


            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateSuburbs(Suburb city)
        {
            #region Cities
            var cities = db.Cities.ToList();

            List<SelectListItem> ct = new List<SelectListItem>();
            ct.Add(new SelectListItem
            {
                Text = "-Select City-",
                Value = "-Select City-"
            });
            foreach (var item in cities)
            {
                ct.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Cities = ct;
            var cid = cities.FirstOrDefault().Id;

            var area = db.Regions.Where(u => u.CityId == cid).ToList();


            List<SelectListItem> rg = new List<SelectListItem>();
            rg.Add(new SelectListItem
            {
                Text = "-Select Region-",
                Value = "-Select Region-"
            });
            foreach (var item in area)
            {
                rg.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Area = rg;
            #endregion
            if (ModelState.IsValid)
            {

                db.Suburbs.Add(city);
                db.SaveChanges();
                city = new Suburb();
                return PartialView(city);
            }

            return PartialView(city);
        }
        //
        // GET: /Cities/Edit/5
        public ActionResult Edit(long id = 0)
        {
            City city = db.Cities.Find(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }
        //
        // POST: /Cities/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(City city)
        {
            if (ModelState.IsValid)
            {
                db.Entry(city).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(city);
        }

        //
        // GET: /Cities/Delete/5

        public ActionResult Delete(long id = 0)
        {
            City city = db.Cities.Find(id);
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
            City city = db.Cities.Find(id);
            db.Cities.Remove(city);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #region Collection settings
        public ActionResult CollectionPoints()
        {

            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            var user = np.Getlogin(partz[1]);

            var comp = db.Companies.Where(u => u.name == user.Location).FirstOrDefault();

            return PartialView(db.CollectionPoints.Where(u => u.Name == comp.name).ToList());
        }


        public ActionResult CreateCollection()
        {
            var cities = db.Cities.ToList();

            List<SelectListItem> ct = new List<SelectListItem>();
            ct.Add(new SelectListItem
            {
                Text = "-Select City-",
                Value = "-Select City-"
            });
            foreach (var item in cities)
            {
                ct.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Name
                });
            }
            ViewBag.Cities = ct;
            var cid = cities.FirstOrDefault().Id;

            var area = db.Regions.Where(u => u.CityId == cid).ToList();
            var rid = area.FirstOrDefault().Id;
            List<SelectListItem> rg = new List<SelectListItem>();
            rg.Add(new SelectListItem
            {
                Text = "-Select Region-",
                Value = "-Select Region-"
            });
            foreach (var item in area)
            {
                rg.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Area = rg;

            var surba = db.Suburbs.Where(u => u.RegionId == rid).ToList();

            List<SelectListItem> sg = new List<SelectListItem>();
            sg.Add(new SelectListItem
            {
                Text = "-Select Surburb-",
                Value = "-Select Surburb-"
            });
            foreach (var item in surba)
            {
                sg.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Name.ToString()
                });
            }
            ViewBag.Subarb = sg;



            return PartialView();
        }

        //
        // POST: /Cities/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult CreateCollection(CollectionPoint city)
        {
            if (ModelState.IsValid)
            {
                db.CollectionPoints.Add(city);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return PartialView(city);
        }
        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}