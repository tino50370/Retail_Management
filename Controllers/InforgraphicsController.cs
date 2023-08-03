using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using System.Web.Helpers;
using System.IO;

namespace RetailKing.Controllers
{
    public class InforgraphicsController : Controller
    {
       RetailKingEntities db = new RetailKingEntities();

        #region Inforgraphics 
        public ActionResult Index()
        {
            if (Request.IsAjaxRequest()) return PartialView(db.Infographics.ToList());
            return  View("Index", "_AdminLayout",db.Infographics.ToList());
        }

        public ActionResult Details(long id = 0)
        {
            Infographic infographic = db.Infographics.Find(id);
            if (infographic == null)
            {
                return HttpNotFound();
            }
            return View(infographic);
        }

        public ActionResult Create( string section)
        {
            #region type
            var types = db.InfographicTypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            
            foreach (var item in types)
            {
                if (string.IsNullOrEmpty(section))
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Name.ToString()
                    });
                }
                else if (item.Name.Trim() == section)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Name.ToString()
                    });
                }
            }

            ViewBag.type = yr;
            #endregion
            return PartialView();
        }
       
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Infographic infographic)
        {
            if (ModelState.IsValid)
            {
                db.Infographics.Add(infographic);
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            #region type
            var types = db.InfographicTypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in types)
            {
                if (string.IsNullOrEmpty(infographic.Type))
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Name.ToString()
                    });
                }
                else if (item.Name.Trim() == infographic.Type)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Name.ToString()
                    });
                }
            }

            ViewBag.type = yr;
            #endregion
            return PartialView(infographic);
        }

        [HttpPost]
        public ActionResult tnymceImage(HttpPostedFileBase Images)
        {
            var CurrentUser = User.Identity.Name;
            var image = WebImage.GetImageFromRequest();
            var fileName = image.FileName;
            var path = Path.Combine(HttpContext.Server.MapPath("~/Content/ChatRoom/Profile/" + CurrentUser + "/Uploads/Images/"), fileName);
            var imgPath = "/Content/ChatRoom/Profile/" + CurrentUser + "/Uploads/Images/" + fileName;


            if (image.Width > 500)
            {
                image.Resize(500, ((500 * image.Height) / image.Width));
            }
            image.Save(path);

            return JavaScript("<script>top.$('.mce-btn.mce-open').parent().find('.mce-textbox').val('" + imgPath + "').closest('.mce-window').find('.mce-primary').click();</script>");
        }

        public ActionResult Edit(long id = 0)
        {
            #region type
            var types = db.InfographicTypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in types)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Name.ToString()
                });
            }

            ViewBag.type = yr;
            #endregion
            Infographic infographic = db.Infographics.Find(id);
            if (infographic == null)
            {
                return HttpNotFound();
            }
            return PartialView(infographic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Infographic infographic)
        {
            if (ModelState.IsValid)
            {
                db.Entry(infographic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            #region type
            var types = db.InfographicTypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in types)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Name.ToString()
                });
            }

            ViewBag.type = yr;
            #endregion
            return View(infographic);
        }

        public ActionResult Delete(long id = 0)
        {
            Infographic infographic = db.Infographics.Find(id);
            if (infographic == null)
            {
                return HttpNotFound();
            }
            return View(infographic);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Infographic infographic = db.Infographics.Find(id);
            db.Infographics.Remove(infographic);
            db.SaveChanges();
            return RedirectToAction("Index");
        }
        #endregion

        #region FAQ
        public ActionResult FaqList(string section)
        {
            if (string.IsNullOrEmpty(section))
            {
                if (Request.IsAjaxRequest()) return PartialView(db.Faqs.ToList());
                return View("FaqList", "_AdminLayout", db.Faqs.ToList());
            }else
            {
                var px = db.Faqs.Where(u => u.FaqType.Trim() == section).ToList();
                if (Request.IsAjaxRequest()) return PartialView(px);
                return View("FaqList", "_AdminLayout", px);
            }
        }

        public ActionResult CreateFaq(string section)
        {
            #region types
            var types = db.FaqTypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in types)
            {
                if (string.IsNullOrEmpty(section))
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.Title,
                        Value = item.Title.ToString()
                    });
                }
                else if (item.Title.Trim() == section)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.Title,
                        Value = item.Title.ToString()
                    });
                }
            }

            ViewBag.type = yr;
            #endregion
            return PartialView();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult CreateFaq(Faq infographic)
        {
            if (ModelState.IsValid)
            {
                db.Faqs.Add(infographic);
                db.SaveChanges();
                return RedirectToAction("FaqList");
            }
            #region types
            var types = db.FaqTypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in types)
            {
                if (string.IsNullOrEmpty(infographic.FaqType))
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.Title,
                        Value = item.Title.ToString()
                    });
                }
                else if (item.Title.Trim() == infographic.FaqType)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.Title,
                        Value = item.Title.ToString()
                    });
                }
            }

            ViewBag.type = yr;
            #endregion
            return PartialView(infographic);
        }

        public ActionResult EditFaq(long id = 0)
        {
            #region types
            var types = db.FaqTypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in types)
            {
               
                    yr.Add(new SelectListItem
                    {
                        Text = item.Title,
                        Value = item.Title.ToString()
                    });
                
            }

            ViewBag.type = yr;
            #endregion

            Faq infographic = db.Faqs.Find(id);
            if (infographic == null)
            {
                return HttpNotFound();
            }
            return PartialView(infographic);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult EditFaq(Faq infographic)
        {
            if (ModelState.IsValid)
            {
                db.Entry(infographic).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            #region types
            var types = db.FaqTypes.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in types)
            {

                yr.Add(new SelectListItem
                {
                    Text = item.Title,
                    Value = item.Title.ToString()
                });

            }

            ViewBag.type = yr;
            #endregion
            return View(infographic);
        }

        public ActionResult DeleteFaq(long id = 0)
        {
            Faq infographic = db.Faqs.Find(id);
            if (infographic == null)
            {
                return HttpNotFound();
            }
            return View(infographic);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteFaqConfirmed(long id)
        {
            Faq infographic = db.Faqs.Find(id);
            db.Faqs.Remove(infographic);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        #endregion
        protected override void Dispose(bool disposing)
        {
            db.Dispose();
            base.Dispose(disposing);
        }
    }
}