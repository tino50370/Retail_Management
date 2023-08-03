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
using System.Windows.Media.Imaging;
using Newtonsoft.Json;
using crypto;
using System.Net.Mail;

namespace RetailKing.Controllers
{   
    [Authorize]
    public class AdvertsController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Companies/
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
        #region Adverts
        public ActionResult Index()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var supplier = part[0];
            var dd = DateTime.Now.AddDays(-1);
            var px = new List<Advert>();
            if (supplier.StartsWith("5-"))
            {
                if(supplier == "5-0001-0000000")
                {
                    px = db.Adverts.Where(u => u.ExpireryDate > dd).ToList();
                }
                else
                {
                    px = db.Adverts.Where(u => u.SupplierId == supplier).ToList();
                }
               
            }
            else
            {
                px = db.Adverts.Where(u =>u.ExpireryDate > dd).ToList();
               // return PartialView(px);
            }
            param.iDisplayStart = 1;
            param.iDisplayLength = 20;

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
            ViewData["sStart"] = DateTime.Now.Month.ToString();
            ViewData["listSize"] = param.iDisplayLength.ToString();
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;

            return PartialView(nn);
        }

        [HttpPost]
        public ActionResult Index(JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var supplier = part[0];
            var dd = DateTime.Now.AddDays(-1);
            var px = new List<Advert>();
            if (supplier.StartsWith("5-"))
            {
                px = db.Adverts.Where(u => u.SupplierId == supplier && u.ExpireryDate > dd).ToList();
                
            }
            else
            {
                px = db.Adverts.Where(u => u.ExpireryDate > dd).ToList();
               
            }
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
            if (string.IsNullOrEmpty(param.sStart)) param.sStart = DateTime.Now.Month.ToString();
            ViewData["sStart"] = param.sStart;
            ViewData["searchTerm"] = param.sSearch;
            ViewData["listSize"] = param.iDisplayLength.ToString();
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;
            return PartialView(nn);
        }

        //
        // GET: /Companies/Details/5

        public ViewResult Details(long id)
        {
             NHibernateDataProvider np = new NHibernateDataProvider();
             Company company = np.GetCompaniesById(id);
            return View(company);
        }

        //
        // GET: /Companies/Create
        [Authorize]
        public ActionResult Create()
        {
            List<SelectListItem> min = new List<SelectListItem>();
            List<SelectListItem> max = new List<SelectListItem>();
            min.Add(new SelectListItem
            {
                Text = "All",
                Value = "79"
            });

            for (int i =18; i < 80; i++)
            {
                min.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });

                max.Add(new SelectListItem
                {
                    Text = (i + 1).ToString(),
                    Value = (i + 1).ToString()
                });
            }
            ViewBag.Min = min;
            ViewBag.Max = max;
            Advert ad = new Advert();
            ad.ExpireryDate = DateTime.Now.AddDays(5).Date;
            return PartialView(ad);
        } 

        //
        // POST: /Companies/Create
        [Authorize]
        [HttpPost]
        public ActionResult Create(Advert company, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimit = new char[] { '~' };
                string[] part = CurrentUser.Split(delimit, StringSplitOptions.RemoveEmptyEntries);

                if (Image != null && Image.ContentLength > 0)
                {
                    try
                    {
                        var fileName = Image.FileName;
                        char[] delimiter = new char[] { '.' };
                        string[] parts = fileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        var ext = parts[1];
                        string regExp = "[^a-zA-Z0-9]";
                        fileName = Regex.Replace(parts[0], regExp, "_");
                        ST_encrypt en = new ST_encrypt();

                        fileName = en.encrypt(part[0] + "_" + fileName, "214ea9d5bda5277f6d1b3a3c58fd7034");
                        fileName = fileName + "." + ext;
                        var tempP = Path.Combine(HttpContext.Server.MapPath("~/Content//"), "temp");
                        var temPath = Path.Combine(HttpContext.Server.MapPath("~/Content//temp//"), fileName);
                        var path = Path.Combine(HttpContext.Server.MapPath("~/Content//Adverts//"), fileName);
                        company.Image  = "~/Content//Adverts//" + fileName;
                       
                        if (System.IO.Directory.Exists(path))
                        {
                            Image.SaveAs(temPath);
                        }
                        else
                        {
                            Directory.CreateDirectory(path);
                            Image.SaveAs(path);
                        }
                       
                       // ResizeImage(temPath, path, 480, 515);
                    }
                    catch (Exception e)
                    {
                    }
                   
                    company.SupplierId = part[0];
                    company.DateCreated = DateTime.Now;
                    company.DailyUsage = 0;
                    Geolocator gl = new RetailKing.Geolocator();
                    var maxs = gl.GetBounderies((double)company.minLatitude, (double)company.minLongitude,(int)company.Radius);
                    company.maxLatitude = double.Parse(maxs.Latitude);
                    company.maxLongitude = double.Parse(maxs.Longitude);
                    db.Adverts.Add(company);
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                ModelState.AddModelError("", "Please select an image for this advert");
            }
            
            #region rr
            List<SelectListItem> min = new List<SelectListItem>();
            List<SelectListItem> max = new List<SelectListItem>();
            min.Add(new SelectListItem
            {
                Text = "All",
                Value = "79"
            });

            for (int i = 18; i < 80; i++)
            {
                min.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });

                max.Add(new SelectListItem
                {
                    Text = (i + 1).ToString(),
                    Value = (i + 1).ToString()
                });
            }
            ViewBag.Min = min;
            ViewBag.Max = max;

            #endregion
            return PartialView(company);
        }
        
        //
        // GET: /Companies/Edit/5
 
        public ActionResult Edit(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            List<SelectListItem> min = new List<SelectListItem>();
            List<SelectListItem> max = new List<SelectListItem>();
            min.Add(new SelectListItem
            {
                Text = "All",
                Value = "79"
            });

            for (int i = 18; i < 80; i++)
            {
                min.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });

                max.Add(new SelectListItem
                {
                    Text = (i + 1).ToString(),
                    Value = (i + 1).ToString()
                });
            }
            ViewBag.Min = min;
            ViewBag.Max = max;
          
            Advert company = db.Adverts.Find(id);
            return PartialView(company);
        }

        //
        // POST: /Companies/Edit/5

        [HttpPost]
        public ActionResult Edit(Advert company, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimit = new char[] { '~' };
                string[] part = CurrentUser.Split(delimit, StringSplitOptions.RemoveEmptyEntries);

                if (Image != null && Image.ContentLength > 0)
                {
                    try
                    {
                        var fileName = Image.FileName;
                        char[] delimiter = new char[] { '.' };
                        string[] parts = fileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        var ext = parts[1];
                        string regExp = "[^a-zA-Z0-9]";
                        fileName = Regex.Replace(parts[0], regExp, "_");
                        ST_encrypt en = new ST_encrypt();

                        fileName = en.encrypt(part[0] + "_" + fileName, "214ea9d5bda5277f6d1b3a3c58fd7034");
                        fileName = fileName + "." + ext;
                        var tempP = Path.Combine(HttpContext.Server.MapPath("~/Content//"), "temp");
                        var temPath = Path.Combine(HttpContext.Server.MapPath("~/Content//temp//"), fileName);
                        var path = Path.Combine(HttpContext.Server.MapPath("~/Content//Adverts//"), fileName);
                        company.Image = "~/Content//Adverts//" + fileName;

                        if (System.IO.Directory.Exists(tempP))
                        {
                            Image.SaveAs(path);
                        }
                        else
                        {
                            Directory.CreateDirectory(tempP);
                            Image.SaveAs(path);
                        }

                       // ResizeImage(temPath, path, 480, 515);
                    }
                    catch (Exception e)
                    {
                    }
                }
                    Geolocator gl = new RetailKing.Geolocator();
                    var maxs = gl.GetBounderies((double)company.minLatitude, (double)company.minLongitude, (int)company.Radius);
                    company.maxLatitude = double.Parse(maxs.Latitude);
                    company.maxLongitude = double.Parse(maxs.Longitude);

                    db.Entry(company).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                
                
            }

            #region rr
            List<SelectListItem> min = new List<SelectListItem>();
            List<SelectListItem> max = new List<SelectListItem>();
            min.Add(new SelectListItem
            {
                Text = "All",
                Value = "79"
            });

            for (int i = 18; i < 80; i++)
            {
                min.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });

                max.Add(new SelectListItem
                {
                    Text = (i + 1).ToString(),
                    Value = (i + 1).ToString()
                });
            }
            ViewBag.Min = min;
            ViewBag.Max = max;

            #endregion
            return PartialView(company);
        }

        //
        // GET: /Company/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            Advert company =db.Adverts.Find(id);
            return PartialView(company);
        }

        //
        // POST: /Company/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Advert company = db.Adverts.Find(id);
          //  db.Entry(company).State = EntityState.Deleted;
          //  db.SaveChanges();
            
           
            return RedirectToAction("Index");
        }
        #endregion

        #region Adspace
        public ActionResult AdSpaces(string Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string supplier = Id;
            if (string.IsNullOrEmpty(supplier))
            {
                supplier = part[0];
            }
            
            var dd = DateTime.Now.AddDays(-1);
            var px = new List<AdSpace>();
            if (supplier.StartsWith("5-"))
            {
                if (supplier == "5-0001-0000000")
                {
                    px = db.AdSpaces.ToList();
                }
                else
                {
                    
                    px = db.AdSpaces.Where(u => u.SupplierId == supplier).ToList();
                }

            }
            
            param.iDisplayStart = 1;
            param.iDisplayLength = 20;

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
            ViewData["sStart"] = DateTime.Now.Month.ToString();
            ViewData["listSize"] = param.iDisplayLength.ToString();
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;
            ViewData["Publisher"] = Id;
            return PartialView(nn);
        }

        public ActionResult AdSpaceAllocations(string Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            
            var ad = db.Adverts.Find(long.Parse(Id));
            List<AdSpace> px = new List<Models.AdSpace>();
            var AdSpac = db.AdSpaces.ToList();
            if (ad.Placements != null)
            {
                char[] delim = new char[] { ',' };
                string[] places = ad.Placements.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                for(int i= 0; i < places.Length; i++)
                {
                    var sid = long.Parse(places[i]);
                    var AdSp = AdSpac.Where(u => u.Id ==sid ).FirstOrDefault();
                    AdSp.State = "checked";
                    px.Add(AdSp);
                }
            }
            else
            {
              
                px = AdSpac;
            }
           
            param.iDisplayStart = 1;
            param.iDisplayLength = 20;

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
            ViewData["sStart"] = DateTime.Now.Month.ToString();
            ViewData["listSize"] = param.iDisplayLength.ToString();
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;
            ViewData["Advert"] = Id;
            ViewData["Places"] = ad.Placements;
            ViewBag.Spaces = nn;
            
            return PartialView(ad);
        }

        [HttpPost]
        public ActionResult AdSpaceAllocations(Advert company)
        {
            if (ModelState.IsValid)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimit = new char[] { '~' };
                string[] part = CurrentUser.Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                var ad = db.Adverts.Find(company.Id);
                ad.Placements = company.Placements;
                db.Entry(ad).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");  
            }

            return PartialView(company);
        }

        [Authorize]
        public ActionResult CreateAdSpace(string Id)
        {
            List<SelectListItem> min = new List<SelectListItem>();
            
            min.Add(new SelectListItem
            {
                Text = "South Africa",
                Value = "ZA"
            });
            min.Add(new SelectListItem
            {
                Text = "Zimbabwe",
                Value = "ZW"
            });

            
            ViewBag.Country = min;
            
            AdSpace ad = new AdSpace();
            ad.Id = 0;
            ad.SupplierId = Id;
            return PartialView(ad);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateAdSpace(AdSpace company)
        {
            try
            {
                company.DateCreated = DateTime.Now;
                db.AdSpaces.Add(company);
                db.SaveChanges();
                return RedirectToAction("AdSpaces", new { Id = company.SupplierId });
            }
            catch (Exception )
            {
                #region rr
                List<SelectListItem> min = new List<SelectListItem>();

                min.Add(new SelectListItem
                {
                    Text = "South Africa",
                    Value = "ZA"
                });
                min.Add(new SelectListItem
                {
                    Text = "Zimbabwe",
                    Value = "ZW"
                });


                ViewBag.Country = min;

                #endregion
            }
            return PartialView(company);
        }

        [Authorize]
        public ActionResult EditAdSpace(string Id)
        {
            //AdSpace ad = new AdSpace();
            AdSpace ad = db.AdSpaces.Find(Id);
            List<SelectListItem> min = new List<SelectListItem>();
            List<SelectListItem> max = new List<SelectListItem>();
            min.Add(new SelectListItem
            {
                Text = "All",
                Value = "79"
            });

            for (int i = 18; i < 80; i++)
            {
                min.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });

                max.Add(new SelectListItem
                {
                    Text = (i + 1).ToString(),
                    Value = (i + 1).ToString()
                });
            }
            ViewBag.Min = min;
            ViewBag.Max = max;
          
            ad.SupplierId = Id;
            return PartialView(ad);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditAdSpace(AdSpace company)
        {
            if (ModelState.IsValid)
            {
               
                db.Entry(company).State = EntityState.Modified;
                db.SaveChanges();

                return RedirectToAction("Index");

            }

            #region rr 
            List<SelectListItem> min = new List<SelectListItem>();
            List<SelectListItem> max = new List<SelectListItem>();
            min.Add(new SelectListItem
            {
                Text = "All",
                Value = "79"
            });

            for (int i = 18; i < 80; i++)
            {
                min.Add(new SelectListItem
                {
                    Text = i.ToString(),
                    Value = i.ToString()
                });

                max.Add(new SelectListItem
                {
                    Text = (i + 1).ToString(),
                    Value = (i + 1).ToString()
                });
            }
            ViewBag.Min = min;
            ViewBag.Max = max;

            #endregion
            return PartialView(company);
        }
        #endregion

        #region Advertiser
        public ActionResult Publishers(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();

            var px = db.Suppliers.Where(u => u.ServiceType == "Publisher").ToList();

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
        public ActionResult Publishers(JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var px = db.Suppliers.Where(u => u.ServiceType == "Publisher").ToList();

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

        public ActionResult CreatePublisher(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var pz = np.GetAllSuppliers();
            var pp = (from e in pz select e).LastOrDefault();
            Supplier px = new Supplier();
            px.Company = Company;
            if (pp == null)
            {
                pp = new Supplier();
                px.AccountCode = "5-0001-0000001";
            }
            else
            {
                char[] delimiter = new char[] { '-' };
                string[] part = pp.AccountCode.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var Id = long.Parse(part[2]);
                string acc = "";
                var x = Id + 1;
                if (x < 10)
                {
                    acc = part[0] + "-" + part[1] + "-" + "000000" + x.ToString();
                }
                else if (x > 9 && x < 100)
                {
                    acc = part[0] + "-" + part[1] + "-" + "00000" + x.ToString();
                }
                else if (x > 99 && x < 1000)
                {
                    acc = part[0] + "-" + part[1] + "-" + "0000" + x.ToString();
                }
                else if (x > 999 && x < 10000)
                {
                    acc = part[0] + "-" + part[1] + "-" + "000" + x.ToString();
                }
                else if (x > 9999 && x < 100000)
                {
                    acc = part[0] + "-" + part[1] + "-" + "00" + x.ToString();
                }
                else if (x > 99999 && x < 1000000)
                {
                    acc = part[0] + "-" + part[1] + "-" + "0" + x.ToString();
                }
                else if (x > 999999 && x < 10000000)
                {
                    acc = part[0] + "-" + part[1] + "-" + x.ToString();
                }
                else if (x > 999999)
                {
                    var Idd = long.Parse(part[1]);
                    var xx = Idd + 1;
                    if (xx < 10)
                    {
                        acc = part[0] + "-" + "000" + xx.ToString() + "-" + "0000001";
                    }
                    else if (xx > 9 && xx < 100)
                    {
                        acc = part[0] + "-" + "00" + xx.ToString() + "-" + "0000001";
                    }
                    else if (xx > 99 && xx < 1000)
                    {
                        acc = part[0] + "-" + "0" + xx.ToString() + "-" + "0000001";
                    }
                    else if (xx > 999 && xx < 10000)
                    {
                        acc = part[0] + "-" + xx.ToString() + "-" + "0000001";
                    }
                    else
                    {
                        acc = "Exhausted";
                    }
                }
                px.AccountCode = acc;
            }
            return PartialView(px);
        }

        [HttpPost]
        public ActionResult CreatePublisher(Supplier customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid)
            {
                var accs = np.GetSuppliersByName(customer.SupplierName);
                if (accs == null || accs.Count == 0)
                {
                    customer.SupplierName = customer.SupplierName.ToUpper();
                    customer.ContactPerson = customer.ContactPerson.ToUpper();
                    customer.Balance = 0;
                    //customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                    np.AddSupplier(customer);

                    ST_encrypt en = new ST_encrypt();
                    var unam = customer.Email;
                    string newAccount = en.encrypt(unam, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    Random rr = new Random();
                    var Pin = rr.Next(00000, 99999).ToString();
                    var ePin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    var question = en.encrypt(customer.AccountCode, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    string Answer = customer.ServiceType;// "Agent";
                    string ssAnswer = en.encrypt(Answer, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    string conn = en.encrypt(customer.SupplierName, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    // Check is exists 
                    AccountCredenitial acnt = db.AccountCredenitials.Find(newAccount);
                    if (acnt == null || acnt.AccountNumber.Trim() != newAccount)
                    {

                        AgentUser uza = new AgentUser();
                        uza.UserName = customer.AccountCode;
                        uza.FullName = customer.ContactPerson;
                        uza.CreationDate = DateTime.Now;
                        uza.AccountNumber = customer.AccountCode;
                        uza.TerminalId = 1;
                        uza.SessionCode = customer.AccountCode;
                        db.AgentUsers.Add(uza);
                        db.SaveChanges();

                        AgentUser usa = new AgentUser();
                        usa.UserName = customer.Email;
                        usa.FullName = customer.ContactPerson;
                        usa.CreationDate = DateTime.Now;
                        usa.AccountNumber = customer.AccountCode;
                        usa.SessionCode = customer.AccountCode;
                        usa.Role = "ADMINISTRATOR";
                        usa.TerminalId = 1;
                        db.AgentUsers.Add(usa);
                        db.SaveChanges();

                        AccountCredenitial accr = new AccountCredenitial();
                        accr.AccountNumber = newAccount;
                        accr.Pin = ePin;
                        accr.Question = question;
                        accr.Answer = Answer;
                        accr.Active = true;
                        accr.Access = "Agent";

                        accr.Connection = customer.Company;
                        db.AccountCredenitials.Add(accr);
                        db.SaveChanges();

                        AccountCredenitial accmr = new AccountCredenitial();
                        accmr.AccountNumber = question;
                        accmr.Pin = ePin;
                        accmr.Question = question;
                        accmr.Answer = Answer;
                        accmr.Active = true;
                        accmr.Access = "System";
                        accmr.Connection = customer.Company;
                        db.AccountCredenitials.Add(accmr);
                        db.SaveChanges();

                        #region Email
                        try
                        {
                            MailMessage mailMessage = new MailMessage(
                                                     new MailAddress("accounts@yomoneyservice.com"), new MailAddress(customer.Email.ToLower()));

                            var ee = en.encrypt(customer.Email.ToLower().Trim() + "/" + DateTime.Now.ToString("ddmmyyyyhhmmss"), "214ea9d5bda5277f6d1b3a3c58fd7034");

                            mailMessage.Subject = "Account Activation";
                            mailMessage.IsBodyHtml = true;
                            mailMessage.Body = "<p>Hello " + customer.ContactPerson + ", </p><p> You have been successfully registered as a Yomoney Agent.</p>" +

                            "Use the following credentials to Signin.<br/><br/> Username = " + unam + "<br/> Password = " + Pin + "<br/> Website = www.yomoney.co.zw </p>" +
                            "<p>For assistance contact support on email addresss support@yomoneyservice.com </p>";


                            System.Net.NetworkCredential networkCredentials = new
                            System.Net.NetworkCredential("accounts@yomoneyservice.com", "Accounts@123");

                            SmtpClient smtpClient = new SmtpClient();
                            smtpClient.EnableSsl = false;
                            smtpClient.UseDefaultCredentials = false;
                            smtpClient.Credentials = networkCredentials;
                            smtpClient.Host = "smtpout.secureserver.net";
                            smtpClient.Port = 25;
                            smtpClient.Send(mailMessage);
                        }
                        catch (Exception e)
                        {

                        }
                        // Send email
                        #endregion
                        return RedirectToAction("Publishers", new { Company = customer.Company });
                    }
                    else
                    {
                        ModelState.AddModelError("", "Sorry this customer name is already in use");
                    }

                }
                else
                {
                    ModelState.AddModelError("", "Sorry this customer name is already in use");
                }
            }

            return PartialView(customer);
        }

        #endregion

      
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
                //context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}