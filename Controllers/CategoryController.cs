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
using System.Web.Security;
using crypto;


namespace RetailKing.Controllers
{
    public class CategoryController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Category/
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

        public ActionResult Index(string Id, JQueryDataTableParamModel param,long catId = 0)
        {
            var px = new List<Item>();
            var main_px = new List<Item>();
            ViewData["Catecory"] = Id;
            var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3-") && u.CompanyId == 1).ToList();
            var c = categories.Where(u => u.AccountName.Trim() == Id).FirstOrDefault();
            if (c.AccountCode.Length > 4)
            { 
                var cat = categories.Where(u => u.ID == catId ).FirstOrDefault();
                main_px = db.Items.Where(u => u.category.Trim() == cat.AccountName.Trim() && u.SubCategory.Trim() == Id && (u.NotForSale == null || u.NotForSale != true)).OrderByDescending(u => u.Balance).ToList();
                ViewData["CatId"] = catId;
            }
            else
            {
                main_px = db.Items.Where(u => u.category == Id && (u.NotForSale == null || u.NotForSale != true)).OrderByDescending(u=> u.Balance).ToList();
                ViewData["CatId"] = c.ID;
            }


            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 21;
            }
            var nn = (from emp in main_px.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                      select emp).ToList();

            int pages = main_px.Count / param.iDisplayLength;
            if (main_px.Count > 0 && pages == 0)
            {
                pages = 1;
            }
            else if (main_px.Count > 0 && main_px.Count % param.iDisplayLength > 0)
            {
                pages += 1;
            }
            int page = param.iDisplayStart;

            int start = (param.iDisplayStart - 1) * param.iDisplayLength;
            int end = (param.iDisplayStart) * param.iDisplayLength;
            if (start == 0) start = 1;
            int column = param.iDisplayStart / 5;
            ViewData["listSize"] = param.iDisplayLength;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["Columns"] = column + 1;
            ViewData["RecordData"] = "Showing " + start + " to " + end + " of " + main_px.Count;

            var partnerz = db.Partners.Take(10).ToList();
            HomeData hd = new HomeData();
            hd.items = nn;
            hd.Categories = categories;
            hd.Partners = partnerz;
            if (Request.IsAjaxRequest()) return PartialView(hd);
            return View(hd);
        }

        //
        // GET: /Category/Details/5

        public ActionResult Details(long id = 0)
        {
            Category category = db.Categories.Find(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // GET: /Category/Create
        public ActionResult CategoryList(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllAccounts();
            var comp = np.GetActivecoByName(Company);
            px = (from e in px
                      .Where(u => u.CompanyId == comp.CompanyId && u.AccountCode.StartsWith("3-") && u.AccountCode.Length == 4)
                  select e).ToList();

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

            return PartialView(px);
        }

        [HttpPost]
        public ActionResult CategoryList(string Company, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            //JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllAccounts();
            var comp = np.GetActivecoByName(Company);
            px = (from e in px
                    .Where(u => u.CompanyId == comp.CompanyId && u.AccountCode.StartsWith("3-") && u.AccountCode.Length == 4)
                  select e).ToList();

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

            return PartialView(px);
        }

        public ActionResult SubCategoryList(long Id, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllAccounts();
            // Company = "MOBISTORE";
            var comp = np.GetActivecoByName(Company);
            var cat = px.Where(u => u.ID == Id).FirstOrDefault();
            px = (from e in px
                      .Where(u => u.CompanyId == comp.CompanyId && u.AccountCode.StartsWith(cat.AccountCode.Trim() + "-"))
                  select e).ToList();

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
            ViewData["Id"] = Id;
            int start = (param.iDisplayStart - 1) * param.iDisplayLength;
            if (start == 0) start = 1;
            ViewData["listSize"] = param.iDisplayLength;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;

            return PartialView(px);
        }

        [HttpPost]
        public ActionResult SubCategoryList(long Id, string Company, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            //JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            // Company = "MOBISTORE";
            var px = np.GetAllAccounts();
            var comp = np.GetActivecoByName(Company);
            var cat = px.Where(u => u.ID == Id).FirstOrDefault();
            px = (from e in px
                      .Where(u => u.CompanyId == comp.CompanyId && u.AccountCode.StartsWith(cat.AccountCode.Trim() + "-"))
                  select e).ToList();

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

            return PartialView(px);
        }

        [HttpGet]
        public ActionResult Create(string Company, string error)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var acc = np.GetAllAccounts().Where(u => u.AccountCode.StartsWith("3-"));
            List<SelectListItem> yr = new List<SelectListItem>();

            List<SelectListItem> comp = new List<SelectListItem>();
            var co = np.GetAllActiveco();
            Company = co.FirstOrDefault().company;
            var account = new Account();
            yr.Add(new SelectListItem
            {
                Text = "Select An Account",
                Value = "Select An Account"
            });



            foreach (var item in acc)
            {
                if (item.AccountCode.Length == 1)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }

            }

            foreach (var item in co)
            {
                comp.Add(new SelectListItem
                {
                    Text = item.company,
                    Value = item.CompanyId.ToString()
                });
            }
            var coid = (from e in co where e.company == Company select e).FirstOrDefault().ID;
            ViewBag.Accountz = yr;

            ViewBag.Company = comp;
            ViewData["Comp"] = coid;
            account.Level1 = "3";
            var code = new Account();
            string typ = "";
            int numb = 0;

            code = acc.OrderByDescending(u => u.AccountCode).Where(u => u.AccountCode.Length == 4).FirstOrDefault();
            if (code == null)
            {
                numb = 1;
            }
            else
            {
                numb = int.Parse(code.AccountCode.Substring(2, code.AccountCode.Length - 2)) + 1;
            }
            if (numb < 10)
            {
                typ = "0" + numb;
            }
            else
            {
                typ = numb.ToString();
            }

            account.Level2 = typ;
            account.CompanyId = coid;
            if (!string.IsNullOrEmpty(error))
            {
                ModelState.AddModelError("", error);
            }
            return PartialView(account);
        }

        [HttpPost]
        public ActionResult Create(Account account, HttpPostedFileBase Image)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var accnt = db.Accounts.Where(u => u.AccountCode.StartsWith(account.Level1)).ToList();
            var code = new Account();

            int numb = 0;
            if (account.Level2 == null)
            {
                code = accnt.OrderByDescending(u => u.AccountCode).Where(u => u.AccountCode.Length == 4).FirstOrDefault();
                numb = int.Parse(code.AccountCode.Substring(2, code.AccountCode.Length - 2)) + 1;
                if (numb < 10)
                {
                    account.Level2 = "0" + numb;
                }
                else
                {
                    account.Level2 = numb.ToString();
                }
            }

            string err = "";
            var px = db.Companies.Find(account.CompanyId);
            if (ModelState.IsValid && account.Level2 != null && account.AccountName != null)
            {
                if (Image != null)
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
                        var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), fileName);
                        var thumbPath = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), thumbname);
                        var image = Image;
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

                        account.Icon = "~/Content/Template/Images/Partners/" + fileName;
                        account.Icon = "~/Content/Template/Images/Partners/" + fileName;

                    }
                    catch (Exception)
                    {
                    }

                    #endregion
                }


                if ((account.Level2 == null || account.Level2 == "") && (account.Level3 == null || account.Level3 == ""))
                {
                    account.AccountCode = account.Level1;
                }
                else if (account.Level3 == null || account.Level3 == "")
                {
                    account.AccountCode = account.Level1 + "-" + account.Level2;
                }
                else
                {
                    account.AccountCode = account.Level2 + "-" + account.Level3;
                }
                account.LinkAccount = db.Accounts.Where(u => u.AccountCode.Trim() == account.Level1).FirstOrDefault().LinkAccount;
                account.AccountName = account.AccountName.ToUpper();
                var ck = np.GetAccountsByCode(account.AccountCode);
                ck = (from e in ck where e.CompanyId == account.CompanyId select e).ToList();
                if (ck == null || ck.Count == 0)
                {
                    np.SaveOrUpdateAccounts(account);
                    return RedirectToAction("CategoryList", new { Company = px.name });
                }
                // err= "This account code is already in use";
            }
            else
            {
                err = "Make Sure all fields are entered";
            }
            return RedirectToAction("Create", new { company = px.name, error = err });
        }

        [HttpGet]
        public ActionResult CreateSubCategory(long Id, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var acc = np.GetAllAccounts();
            var cat = acc.Where(u => u.ID == Id).FirstOrDefault();
            acc = acc.Where(u => u.AccountCode.StartsWith(cat.AccountCode + "-")).ToList();

            List<SelectListItem> yr = new List<SelectListItem>();

            List<SelectListItem> comp = new List<SelectListItem>();
            var co = np.GetAllActiveco();
            //  Company = "MOBISTORE";

            var account = new Account();
            yr.Add(new SelectListItem
            {
                Text = "Select An Account",
                Value = "Select An Account"
            });



            foreach (var item in acc)
            {
                if (item.AccountCode.Length == 1)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }

            }

            foreach (var item in co)
            {
                comp.Add(new SelectListItem
                {
                    Text = item.company,
                    Value = item.CompanyId.ToString()
                });
            }
            var coid = (from e in co where e.company == Company select e).FirstOrDefault().ID;
            ViewBag.Accountz = yr;

            ViewBag.Company = comp;
            ViewData["Comp"] = coid;
            ViewData["Id"] = Id;
            account.CompanyId = 1;
            account.Level1 = "3";
            account.Level2 = cat.AccountCode.Substring(2, 2);
            account.CategoryId = Id;
            var code = new Account();
            string typ = "";
            int numb = 0;
            code = acc.OrderByDescending(u => u.AccountCode).Where(u => u.AccountCode.Length > 4).FirstOrDefault();
            if (code == null)
            {
                numb = 1;
            }
            else
            {
                numb = int.Parse(code.AccountCode.Substring(5, code.AccountCode.Length - 5)) + 1;
            }
            if (numb < 10)
            {
                account.Level3 = "000" + numb;
            }
            else if (numb > 9 && numb < 100)
            {
                account.Level3 = "00" + numb;
            }
            else if (numb > 99 && numb < 1000)
            {
                account.Level3 = "0" + numb;
            }
            else
            {
                account.Level3 = numb.ToString();
            }
            return PartialView(account);
        }

        [HttpPost]
        public ActionResult CreateSubCategory(Account account, HttpPostedFileBase Image)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var accnt = db.Accounts.Where(u => u.AccountCode.StartsWith(account.Level1)).ToList();
            var code = new Account();

            int numb = 0;
            if (account.Level2 == null)
            {
                code = accnt.OrderByDescending(u => u.AccountCode).Where(u => u.AccountCode.Length == 4).FirstOrDefault();
                numb = int.Parse(code.AccountCode.Substring(2, code.AccountCode.Length - 2)) + 1;
                if (numb < 10)
                {
                    account.Level2 = "0" + numb;
                }
                else
                {
                    account.Level2 = numb.ToString();
                }
            }
            else if (account.Level2 != null && account.Level3 == null)
            {
                code = accnt.OrderByDescending(u => u.AccountCode).Where(u => u.AccountCode.Length > 4).FirstOrDefault();
                numb = int.Parse(code.AccountCode.Substring(5, code.AccountCode.Length - 5)) + 1;
                if (numb < 10)
                {
                    account.Level3 = "000" + numb;
                }
                else if (numb > 9 && numb < 100)
                {
                    account.Level3 = "00" + numb;
                }
                else if (numb > 99 && numb < 1000)
                {
                    account.Level3 = "0" + numb;
                }
                else
                {
                    account.Level3 = numb.ToString();
                }
            }

            if (ModelState.IsValid && account.Level2 != null && account.AccountName != null)
            {
                if (Image != null)
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
                        var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), fileName);
                        var thumbPath = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), thumbname);
                        var image = Image;
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

                        account.Icon = "~/Content/Template/Images/Partners/" + fileName;
                        account.Icon = "~/Content/Template/Images/Partners/" + fileName;

                    }
                    catch (Exception)
                    {
                    }

                    #endregion
                }



                if ((account.Level2 == null || account.Level2 == "") && (account.Level3 == null || account.Level3 == ""))
                {
                    account.AccountCode = account.Level1;
                }
                else if (account.Level3 == null || account.Level3 == "")
                {
                    account.AccountCode = account.Level1 + "-" + account.Level2;
                }
                else
                {
                    account.AccountCode = account.Level1 + "-" + account.Level2 + "-" + account.Level3;
                }
                account.LinkAccount = db.Accounts.Where(u => u.AccountCode.Trim() == account.Level1).FirstOrDefault().LinkAccount;
                account.AccountName = account.AccountName.ToUpper();
                var ck = np.GetAccountsByCode(account.AccountCode);
                ck = (from e in ck where e.CompanyId == account.CompanyId select e).ToList();
                if (ck == null || ck.Count == 0)
                {
                    np.SaveOrUpdateAccounts(account);
                    var px = np.GetAllAccounts();
                    ViewData["listSize"] = "20";
                    px = (from e in px
                          where e.CompanyId == account.CompanyId
                          select e).ToList();
                    var coco = np.GetCompaniesById(long.Parse(account.CompanyId.ToString())).name;
                    return RedirectToAction("SubCategoryList", new { Id = account.CategoryId, Company = coco });
                }
                ModelState.AddModelError("", "This account code is already in use");
            }
            else
            {
                ModelState.AddModelError("", "Make Sure all fields are entered");
            }
            var acc = np.GetAllAccounts();
            List<SelectListItem> yr = new List<SelectListItem>();
            List<SelectListItem> typ = new List<SelectListItem>();
            List<SelectListItem> comp = new List<SelectListItem>();
            var co = np.GetAllActiveco();
            yr.Add(new SelectListItem
            {
                Text = "Select An Account",
                Value = "Select An Account"
            });


            typ.Add(new SelectListItem
            {
                Text = "Select Account type",
                Value = "Select Account type"
            });

            foreach (var item in acc)
            {
                if (item.AccountCode.Length == 1)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }
            }

            foreach (var item in co)
            {
                comp.Add(new SelectListItem
                {
                    Text = item.company,
                    Value = item.CompanyId.ToString()
                });
            }


            ViewBag.Accountz = yr;
            ViewBag.Type = typ;
            ViewBag.Company = comp;
            ViewData["Comp"] = account.CompanyId;
            // ViewData["Company"] = Company;
            if (Request.IsAjaxRequest()) return PartialView();
            return View();
        }
        //
        // GET: /Category/Edit/5

        public ActionResult Edit(int id, string Company, string error)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Account account = np.GetAccounts(id);
            var acc = np.GetAllAccounts();
            List<SelectListItem> yr = new List<SelectListItem>();
            List<SelectListItem> typ = new List<SelectListItem>();
            List<SelectListItem> comp = new List<SelectListItem>();
            var co = np.GetAllActiveco();
            yr.Add(new SelectListItem
            {
                Text = "Select An Account",
                Value = "Select An Account"
            });


            typ.Add(new SelectListItem
            {
                Text = "Select Account type",
                Value = "Select Account type"
            });

            foreach (var item in acc)
            {
                if (item.AccountCode.Length == 1)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }

            }

            foreach (var item in co)
            {
                comp.Add(new SelectListItem
                {
                    Text = item.company,
                    Value = item.CompanyId.ToString()
                });
            }
            char[] delimiter = new char[] { '-' };
            string[] part = account.AccountCode.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            account.Level1 = part[0];
            account.Level2 = part[1];
            //account.Level3 = part[2];
            var coid = (from e in co where e.company == Company select e).FirstOrDefault().ID;
            ViewBag.Accountz = yr;
            ViewBag.Type = typ;
            ViewBag.Company = comp;
            ViewData["Comp"] = coid;
            if (!string.IsNullOrEmpty(error))
            {
                ModelState.AddModelError("", error);
            }
            return PartialView(account);
        }

        //
        // POST: /Accounts/Edit/5

        [HttpPost]
        public ActionResult Edit(Account account, HttpPostedFileBase Image)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var conam = np.GetCompaniesById((long)account.CompanyId).name;
            string err = "";
            if (ModelState.IsValid && !string.IsNullOrEmpty(account.AccountName))
            {
                if (Image != null)
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
                        var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), fileName);
                        var thumbPath = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), thumbname);
                        var image = Image;
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

                        account.Icon = "~/Content/Template/Images/Partners/" + fileName;
                        account.Icon = "~/Content/Template/Images/Partners/" + fileName;

                    }
                    catch (Exception)
                    {
                    }

                    #endregion
                }

                np.UpdateAccounts(account);
                return RedirectToAction("CategoryList", new { Company = conam });
            }
            else
            {
                err = "Make Sure all fields are entered";
            }

            // ViewData["Company"] = Company;
            return RedirectToAction("Edit", new { id = account.ID, company = conam, error = err });
            //return PartialView(account);
        }

        public ActionResult EditSubCategory(int id, string Company, string error)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Account account = np.GetAccounts(id);
            var acc = np.GetAllAccounts();
            List<SelectListItem> yr = new List<SelectListItem>();
            List<SelectListItem> typ = new List<SelectListItem>();
            List<SelectListItem> comp = new List<SelectListItem>();
            var co = np.GetAllActiveco();
            yr.Add(new SelectListItem
            {
                Text = "Select An Account",
                Value = "Select An Account"
            });


            typ.Add(new SelectListItem
            {
                Text = "Select Account type",
                Value = "Select Account type"
            });

            foreach (var item in acc)
            {
                if (item.AccountCode.Length == 1)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }

            }

            foreach (var item in co)
            {
                comp.Add(new SelectListItem
                {
                    Text = item.company,
                    Value = item.CompanyId.ToString()
                });
            }
            char[] delimiter = new char[] { '-' };
            string[] part = account.AccountCode.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            account.Level1 = part[0];
            account.Level2 = part[1];
            account.Level3 = part[2];
            var coid = (from e in co where e.company == Company select e).FirstOrDefault().ID;
            ViewBag.Accountz = yr;
            ViewBag.Type = typ;
            ViewBag.Company = comp;
            ViewData["Comp"] = coid;
            ViewData["Id"] = id;
            return PartialView(account);
        }

        //
        // POST: /Accounts/Edit/5

        [HttpPost]
        public ActionResult EditSubCategory(Account account, HttpPostedFileBase Image)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var conam = np.GetCompaniesById((long)account.CompanyId).name;
            string err = "";
            if (ModelState.IsValid && !string.IsNullOrEmpty(account.AccountName))
            {
                if (Image != null)
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
                        var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), fileName);
                        var thumbPath = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), thumbname);
                        var image = Image;
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

                        account.Icon = "~/Content/Template/Images/Partners/" + fileName;
                        account.Icon = "~/Content/Template/Images/Partners/" + fileName;

                    }
                    catch (Exception)
                    {
                    }

                    #endregion
                }


                np.UpdateAccounts(account);
                return RedirectToAction("Index", conam);
            }
            else
            {
                err = "Make Sure all fields are entered";
            }

            return RedirectToAction("EditSubCategory", new { id = account.ID, company = conam, error = err });

        }

        //
        // GET: /Category/Delete/5

        public ActionResult Delete(long id = 0)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Account category = np.GetAccounts(id);
            if (category == null)
            {
                return HttpNotFound();
            }
            return View(category);
        }

        //
        // POST: /Category/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(long id)
        {
            Category category = db.Categories.Find(id);
            db.Categories.Remove(category);
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