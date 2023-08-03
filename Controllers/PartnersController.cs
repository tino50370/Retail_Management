using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.DataAcess;
using System.Text.RegularExpressions;
using System.IO;
using System.Web.Helpers;
using System.Windows.Media.Imaging;


namespace RetailKing.Controllers
{   
    public class PartnersController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
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
             var px = db.Partners.ToList();


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
             var item = db.ItemImages.Find(id);
             return PartialView(item);
         }

         //
         // GET: /Items/Create

         public ActionResult Create()
         {
             
             return PartialView();
         }


         // POST: /Items/Create

         [HttpPost]
         [ValidateAntiForgeryToken]
         public ActionResult Create(Partner  item,HttpPostedFileBase Image)
         {
             if (ModelState.IsValid)
             {
                 NHibernateDataProvider np = new NHibernateDataProvider();
                  if(Image != null)
                  { 
                      #region itemimage
                 try
                 {
                    
                     var fileName = Image.FileName;
                     char[] delimiter = new char[] { '.' };
                     string[] parts = fileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                     var ext = parts[1];
                     string regExp = "[^a-zA-Z0-9]";
                     fileName = Regex.Replace(parts[0], regExp, "_");

                     var thumbname = fileName + "_Thumb" + "." + ext;
                     fileName = fileName + "." + ext;
                     var temp = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/"), fileName);
                     var pathDir = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"));
                     var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/" ), fileName);
                     var thumbPath = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), thumbname);
                     var image = Image ;
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

                     ResizeImage(temp, path, 166, 100);
                     
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

                     item.Image = "~/Content/Template/Images/Partners/" + fileName;
                     item.Logo = "~/Content/Template/Images/Partners/"  + fileName;
                    
                 }
                 catch (Exception)
                 {
                 }
                 
                 #endregion
                  }
                 db.Partners.Add(item);
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
             var item = db.ItemImages.Find(id);
             return PartialView(item);
         }

         //
         // POST: /Items/Edit/5

         [HttpPost]
         public ActionResult Edit(ItemImage item)
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
             var item = db.ItemImages.Find(id);
             return View(item);
         }

         //
         // POST: /Items/Delete/5

         [HttpPost, ActionName("Delete")]
         public ActionResult DeleteConfirmed(int id)
         {
             ItemImage variation = db.ItemImages.Find(id);
             db.ItemImages.Remove(variation);
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