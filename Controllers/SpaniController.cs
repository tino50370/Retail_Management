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
using System.Text.RegularExpressions;
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using RetailKing.RavendbClasses;

namespace RetailKing.Controllers
{   //[Authorize]
    public class SpaniController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        
        //
        // GET: /Suppliers/
        private void ResizeImage(string inputPath, string outputPath, int width, int height)
        {
            BitmapImage bitmap = new BitmapImage();

            using (var stream = new FileStream(inputPath, FileMode.Open))
            {
                bitmap.BeginInit();
                bitmap.DecodePixelWidth = width;
                bitmap.DecodePixelHeight = height;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }

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
        #region Job Category
        public ActionResult JobCategories(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();

            var px = db.JobCategories.ToList();
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
        public ActionResult JobCategories(string category, string ItemCode, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var px = db.JobCategories.ToList();

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

        public ActionResult CreateCategory( string Company)
        {
           NHibernateDataProvider np = new NHibernateDataProvider();
          
            return PartialView();
        } 

        [HttpPost]
        public ActionResult CreateCategory(JobCategory customer, HttpPostedFileBase Images)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid)
            {
                var accs = db.JobCategories.Where(u => u.Name == customer.Name).ToList();
                if (accs == null || accs.Count == 0)
                {
                    if (Images != null)
                    {
                        #region image
                        try
                        {
                            var fileName = Images.FileName;
                            char[] delimiter = new char[] { '.' };
                            string[] parts = fileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            var ext = parts[1];
                            string regExp = "[^a-zA-Z0-9]";
                            fileName = Regex.Replace(parts[0], regExp, "_");

                            var thumbname = fileName + "_Thumb" + "." + ext;
                            fileName = fileName + "." + ext;
                            var temp = Path.Combine(HttpContext.Server.MapPath("~/Content/Temp/Images/"), fileName);
                            var pathDir = Path.Combine(HttpContext.Server.MapPath("~/Content/Spani/Images/"));
                            var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Spani/Images/"), fileName);
                            var tempDir = Path.Combine(HttpContext.Server.MapPath("~/Content/Temp/Images/"));
                            var image = Images;
                            var Thimage = image;
                            if (System.IO.Directory.Exists(tempDir))
                            {
                                image.SaveAs(temp);

                            }
                            else
                            {
                                Directory.CreateDirectory(tempDir);
                                image.SaveAs(temp);
                            }

                            ResizeImage(temp, path, 128, 128);
                            
                            if (System.IO.File.Exists(temp))
                            {
                                // Use a try block to catch IOExceptions, to 
                                // handle the case of the file already being 
                                // opened by another process. 
                                try
                                {
                                    System.IO.File.Delete(temp);
                                }
                                catch (System.IO.IOException e)
                                {

                                }
                            }


                            customer.Image = "~/Content/Spani/Images/" + fileName;
                            
                        }
                        catch (Exception)
                        {
                        }

                        #endregion
                    }
                    db.JobCategories.Add(customer);
                    db.SaveChanges();
                    return RedirectToAction("JobCategories");
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

        public ActionResult EditCategory(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JobCategory  customer = db.JobCategories.Find(id);
           
            return PartialView(customer);
        }

        [HttpPost]
        public ActionResult EditCategory(JobCategory customer, HttpPostedFileBase Images)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid)
            {
                var accs = db.JobCategories.Where(u => u.Name == customer.Name).ToList();
                if (accs == null || accs.Count == 0)
                {
                    if (Images != null)
                    {
                        #region image
                        try
                        {
                            var fileName = Images.FileName;
                            char[] delimiter = new char[] { '.' };
                            string[] parts = fileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                            var ext = parts[1];
                            string regExp = "[^a-zA-Z0-9]";
                            fileName = Regex.Replace(parts[0], regExp, "_");

                            var thumbname = fileName + "_Thumb" + "." + ext;
                            fileName = fileName + "." + ext;
                            var temp = Path.Combine(HttpContext.Server.MapPath("~/Content/Temp/Images/"), fileName);
                            var pathDir = Path.Combine(HttpContext.Server.MapPath("~/Content/Spani/Images/"));
                            var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/"), fileName);
                            var thumbPath = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/"), thumbname);
                            var image = Images;
                            var Thimage = image;
                            if (System.IO.Directory.Exists(pathDir))
                            {
                                image.SaveAs(temp);

                            }
                            else
                            {
                                Directory.CreateDirectory(pathDir);
                                image.SaveAs(temp);
                            }

                            ResizeImage(temp, path, 128, 128);

                            if (System.IO.File.Exists(temp))
                            {
                                // Use a try block to catch IOExceptions, to 
                                // handle the case of the file already being 
                                // opened by another process. 
                                try
                                {
                                    System.IO.File.Delete(temp);
                                }
                                catch (System.IO.IOException e)
                                {

                                }
                            }


                            customer.Image = "~/Content/Spani/Images/" + fileName;

                        }
                        catch (Exception)
                        {
                        }

                        #endregion
                    }
                    db.JobCategories.Add(customer);
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
        #endregion

        #region Job SubCategory
        public ActionResult JobSubCategories(long Id, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();

            var px = db.JobSubCategories.Where(u=> u.JobCategoryId == Id).ToList();
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
            ViewData["Id"] = Id;
            ViewData["Company"] = Company;

            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);

        }

        [HttpPost]
        public ActionResult JobCategories(string category, long Id, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
           var px = db.JobSubCategories.Where(u => u.JobCategoryId == Id).ToList(); ;

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
            ViewData["Id"] = Id;
            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);
        }

        public ActionResult CreateSubCategory(long Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var cat = db.JobCategories.Find(Id);
            JobSubCategory js = new JobSubCategory();
            js.JobCategoryId = Id;
            js.Image = cat.Image;
            return PartialView(js);
        }

        [HttpPost]
        public ActionResult CreateSubCategory(JobSubCategory customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid)
            {
                var accs = db.JobSubCategories.Where(u => u.Name == customer.Name).ToList();
                if (accs == null || accs.Count == 0)
                {
                    db.JobSubCategories.Add(customer);
                    db.SaveChanges();
                    return RedirectToAction("JobSubCategories", new  { Id = customer.JobCategoryId });
                }
                else
                {
                    ModelState.AddModelError("", "Sorry this Job is already in use");
                }
            }
            var px = db.JobCategories.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in px)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.Name.Trim(),
                    Value = item.Id.ToString(),
                });
            }

            ViewBag.Trantype = yr;
            return PartialView(customer);
        }

        public ActionResult EditSubCategory(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var customer = db.JobSubCategories.Find(id);
            var px = db.JobCategories.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in px)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.Name.Trim(),
                    Value = item.Id.ToString(),
                });
            }

            ViewBag.Trantype = yr;
            return PartialView(customer);
        }

        [HttpPost]
        public ActionResult EditSubCategory(YomoneyService customer)
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
        #endregion

        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            YomoneyService customer = db.YomoneyServices.Find(id);
            return View(customer);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            YomoneyService customer = db.YomoneyServices.Find(id);
            db.Entry(customer).State = EntityState.Deleted;
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