using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.DataAcess;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Windows.Media.Imaging;
using RetailKing.Controllers;

namespace RetailKing.Controllers
{
    [Authorize]
    public class ItemsController : Controller
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
       
        public ActionResult Index(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            
            var currentUserId = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = currentUserId.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var co = np.GetActivecoByName(part[1]);
            ViewData["category"] = "--Select A Category--";
            ViewData["SubCategory"] = "--Select A SubCategory--";
            ViewData["DataPost"] = "Get";
            Inventory inv = new Inventory();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllItems();
            px = (from e in px
                  where e.company == Company
                  && e.HasRecipe != true
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
            Account acc = new Account();
            acc.AccountName = "--Select A Category--";
            IList<Account> cate = new List<Account>();
            IList<Account> SubC = new List<Account>();
            cate.Add(acc);
            var catz = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4  select e).ToList();
            
            inv.categories = catz;
            acc.AccountName = "--Select A SubCategory--";
            SubC.Add(acc);
            var ct =catz.FirstOrDefault();
            var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8  select e).ToList();
            
            inv.SubCategories = subCs;
            inv.items = nn;

            if (Request.IsAjaxRequest())
                return PartialView(inv);
            return View(inv);
        }

        [HttpPost]
        public ActionResult Index(string category, string ItemCode, string company, string SubCategory, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUserId = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = currentUserId.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var co = np.GetActivecoByName(part[2]);
            
            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }
                ViewData["DataPost"] = "Post";
                ViewData["category"] = category;
                ViewData["SubCategory"] = SubCategory;
                 var gx = new List<Item>();
                 var px = np.GetAllItems();
                 px = (from e in px
                  where e.company == co.company.Trim()
                  && e.HasRecipe != true
                  select e).ToList();
                if (!string.IsNullOrEmpty(ItemCode))
                {
                    try
                    {

                      px = px.Where(u => u.ItemCode.ToLower().Contains(ItemCode.Trim().ToLower()) || u.ItemName.ToLower().Contains(ItemCode.Trim().ToLower()))
                      .ToList();
                    }catch
                    {

                     }
                }
                if (category.Substring(0,2) != "--" && (SubCategory == "--Select A SubCategory--" ||  SubCategory == null))
                {
                    px = (from ee in px
                      .Where(u => u.category.ToLower() == category.Trim().ToLower())
                          select ee).ToList();
                }
                if (!string.IsNullOrEmpty(SubCategory) && SubCategory != "--Select A SubCategory--")
                {
                      px = (from ee in px
                      .Where(u => u.SubCategory.ToLower() == SubCategory.Trim().ToLower())
                          select ee).ToList();
                }
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
                Account acc = new Account();
                acc.AccountName = "--Select A Category--";
                IList<Account> cate = new List<Account>();
                IList<Account> SubC = new List<Account>();
                cate.Add(acc);
                var catz = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4  select e).ToList();
                foreach (var item in catz)
                {
                    cate.Add(item);
                }
                inv.categories = cate;
                acc.AccountName = "--Select A SubCategory--";
                SubC.Add(acc);
                var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8  select e).ToList();
                foreach (var item in subCs)
                {
                    SubC.Add(item);
                }
                inv.SubCategories = SubC;
                inv.items = nn;
                return PartialView("Index",inv);
        }

        public ActionResult IndexSearches(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUserId = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = currentUserId.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var co = np.GetActivecoByName(part[1]);
            ViewData["category"] = "--Select A Category--";
            ViewData["SubCategory"] = "--Select A SubCategory--";
            Inventory inv = new Inventory();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllItems();
            px = (from e in px
                  where e.company == co.company
                  && e.HasRecipe != true
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
            Account acc = new Account();
            acc.AccountName = "--Select A Category--";
            IList<Account> cate = new List<Account>();
            IList<Account> SubC = new List<Account>();
            cate.Add(acc);
            var catz = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
            foreach (var item in catz)
            {
                cate.Add(item);
            }
            inv.categories = cate;
            acc.AccountName = "--Select A SubCategory--";
            SubC.Add(acc);
            var ct = catz.FirstOrDefault();
            var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
            foreach (var item in subCs)
            {
                SubC.Add(item);
            }
            inv.SubCategories = SubC;
            inv.items = nn;

            if (Request.IsAjaxRequest())
                return PartialView(inv);
            return View(inv);
        }

        [HttpPost]
        public ActionResult IndexSearches(string category, string ItemCode, string company, string SubCategory, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUserId = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = currentUserId.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var co = np.GetActivecoByName(part[2]);

            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }

            ViewData["category"] = category;
            ViewData["SubCategory"] = SubCategory;
            var gx = new List<Item>();
            var px = np.GetAllItems();
            px = (from e in px
                  where e.company == co.company
                  && e.HasRecipe != true
                  select e).ToList();

            if (!string.IsNullOrEmpty(ItemCode))
            {
                px = (from ee in px
                      where ItemCode.ToLower() == ItemCode.Trim().ToLower()
                      select ee).ToList();
            }
            if (category != "--Select A Category--" && SubCategory == "--Select A SubCategory--")
            {
                px = (from ee in px
                  .Where(u => u.category.ToLower() == category.Trim().ToLower())
                      select ee).ToList();
            }
            if (!string.IsNullOrEmpty(SubCategory) && SubCategory != "--Select A SubCategory--")
            {
                px = (from ee in px
                .Where(u => u.SubCategory.ToLower() == SubCategory.Trim().ToLower())
                      select ee).ToList();
            }
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
            Account acc = new Account();
            acc.AccountName = "--Select A Category--";
            IList<Account> cate = new List<Account>();
            IList<Account> SubC = new List<Account>();
            cate.Add(acc);
            var catz = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
            foreach (var item in catz)
            {
                cate.Add(item);
            }
            inv.categories = cate;
            acc.AccountName = "--Select A SubCategory--";
            SubC.Add(acc);
            var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8  select e).ToList();
            foreach (var item in subCs)
            {
                SubC.Add(item);
            }
            inv.SubCategories = SubC;
            inv.items = nn;
            return PartialView("Index", inv);
        }

        [HttpGet]
        public ActionResult Stocks(string Company, int? currentPage)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (partz.Length == 4)
            {
                ViewData["Role"] = partz[3];

                var co = np.GetActivecoByName(Company);
                ViewData["category"] = "--Select A Category--";
                ViewData["SubCategory"] = "--Select A SubCategory--";
                Inventory inv = new Inventory();
                JQueryDataTableParamModel param = new JQueryDataTableParamModel();
                var px = np.GetAllItems();
                px = (from e in px
                      where e.company == Company
                      select e).ToList();
                if (currentPage == null)
                {
                    currentPage = 1;
                }


                param.iDisplayStart = Convert.ToInt32(currentPage);

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
                Account acc = new Account();
                Account sacc = new Account();
                acc.AccountName = "--Select A Category--";
                IList<Account> cate = new List<Account>();
                IList<Account> SubC = new List<Account>();
                cate.Add(acc);
                var catz = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 && e.CompanyId == co.CompanyId select e).ToList();
                foreach (var item in catz)
                {
                    cate.Add(item);
                }
                inv.categories = cate;
                sacc.AccountName = "--Select A SubCategory--";
                SubC.Add(sacc);
                var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
                foreach (var item in subCs)
                {
                    SubC.Add(item);
                }
                inv.SubCategories = SubC;
                inv.items = nn;

                if (Request.IsAjaxRequest()) return PartialView(inv);
                return View("Stocks", "_AdminLayout", inv);
            }
            else
            {
                return RedirectToAction("AccLogout", "Accounts");
            }
        }

        [HttpPost]
        public ActionResult Stocks(string category, string ItemCode, string company, string SubCategory, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUserId = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = currentUserId.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var co = np.GetActivecoByName(part[1]);

            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }

            ViewData["category"] = category;
            ViewData["SubCategory"] = SubCategory;
            var gx = new List<Item>();
            var px = np.GetAllItems();
            px = (from e in px
                  where e.company == company
                  select e).ToList();
            if (!string.IsNullOrEmpty(ItemCode))
            {
                px = px.Where(u => u.ItemCode.ToLower().Contains(ItemCode.Trim().ToLower()) || u.ItemName.ToLower().Contains(ItemCode.Trim().ToLower()))
                      .ToList();
            }
            if ((category != "--Select A Category--" && category != "--Select A SubCategory--") && (SubCategory == "--Select A SubCategory--" || string.IsNullOrEmpty(SubCategory)))
            {
                px = px.Where(u => u.category.Trim() == category.Trim())
                     .ToList();
            }
            if (!string.IsNullOrEmpty(SubCategory) && SubCategory != "--Select A SubCategory--" && SubCategory != "--Select A Category--")
            {
                px = px.Where(u => u.SubCategory.Trim() == SubCategory.Trim())
                      .ToList();
            }
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
            var startt = start + 1;
            var finish = start + param.iDisplayLength;
            if(page == 1)
            {
                ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;
            }
            else { 
            ViewData["RecordData"] = "Showing " + startt + " to " + finish + " of " + px.Count;
            }
            Inventory inv = new Inventory();
            Account acc = new Account();
            acc.AccountName = "--Select A Category--";
            IList<Account> cate = new List<Account>();
            IList<Account> SubC = new List<Account>();
            cate.Add(acc);
            var catz = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
            foreach (var item in catz)
            {
                cate.Add(item);
            }
            inv.categories = cate;
            acc.AccountName = "--Select A SubCategory--";
            SubC.Add(acc);
            var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 && e.CompanyId == co.CompanyId select e).ToList();
            foreach (var item in subCs)
            {
                SubC.Add(item);
            }
            inv.SubCategories = SubC;
            inv.items = nn;
            if (Request.IsAjaxRequest()) return PartialView(inv);
            return View("Stocks", "_AdminLayout", inv);

        }
        
        #region GRVs
        public ActionResult GrvList(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            //Inventory inv = new Inventory();
            //JQueryDataTableParamModel param = new JQueryDataTableParamModel();

            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            var user = np.Getlogin(partz[1]);

            var currentLocation = user.Location;
            if (user.MultiCompanyAccess == true) currentLocation = partz[2];
           var comp = db.Companies.Where(u => u.name == currentLocation).FirstOrDefault();

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
            var px = db.Purchases.Where(u => u.IsManufactured != true && u.company == comp.name).OrderBy(u => u.dated).ToList();

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
            var cat = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
            var SubCategories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            foreach (var item in cat)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.AccountName,
                    Value = item.AccountName.Trim()
                });
            }

            ViewBag.Cat = yr;

            List<SelectListItem> yt = new List<SelectListItem>();
            foreach (var item in cat)
            {
                yt.Add(new SelectListItem
                {
                    Text = item.AccountName,
                    Value = item.AccountName.Trim()
                });
            }

            ViewBag.SCat = yt;

            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);
        }
        public ActionResult GrvLines(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.PurchaseLines.Where(u => u.Reciept == id).ToList();// np.GetOrderLinesByReciept(id);
            ViewData["Id"] = id;
            return PartialView(item);
        }

        public ActionResult NewGrv(string Company, string SupplierId, string PurchaseId,  JQueryDataTableParamModel param, string DataPost = "Get")
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            Company = partz[2];
            ViewData["Company"] = Company;

            var date = DateTime.Now.Date;
            var edate = DateTime.Now.AddDays(1).Date;

            ViewData["Date"] = DateTime.Now.Date.ToString("MM/dd/yyyy");
            ViewData["sdate"] = date;
            ViewData["edate"] = edate;

            GRV px = new Models.GRV();
            var catz = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
            Account acc = new Account();
            acc.AccountName = "--Select A Category--";
            catz.Insert(0, acc);
            ViewBag.categories = catz;
            var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
            Account accs = new Account();
            accs.AccountName = "--Select A SubCategory--";
            subCs.Insert(0, accs);
            ViewBag.SubCategories = subCs;
            //px.supplierDetails= db.Suppliers.Where(u);
            if(string.IsNullOrEmpty(PurchaseId))
            {
                px.grvDetails = db.Purchases.Where(u => u.Status == "O" && u.company == Company).FirstOrDefault();
                if (px.grvDetails != null)
                {
                    DataPost = "Post";
                    px.supplierDetails = db.Suppliers.Where(u => u.SupplierName == px.grvDetails.supplier).FirstOrDefault();
                }
            }
            else
            {
                px.grvDetails = db.Purchases.Find(long.Parse(PurchaseId));
                if (px.grvDetails != null)
                {
                    if (px.grvDetails.supplier != null)
                    {
                        SupplierId = px.grvDetails.supplier;
                    }
                    DataPost = "Post";
                }
            }
            if(px.grvDetails != null)
                px.supplierDetails = db.Suppliers.Where(u => u.SupplierName == px.grvDetails.supplier).FirstOrDefault();
            if (px.grvDetails != null)
                px.purchaseLines = db.PurchaseLines.Where(u => u.Reciept ==  px.grvDetails.Reciept).ToList();
            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }
            int pages = 0;
            int start = (param.iDisplayStart - 1) * param.iDisplayLength;
            if (px.purchaseLines != null)
            {
                var nn = (from emp in px.purchaseLines.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                          select emp).ToList();

                pages = px.purchaseLines.Count() / param.iDisplayLength;
                if (px.purchaseLines.Count() > 0 && pages == 0)
                {
                    pages = 1;
                }
                else if (px.purchaseLines.Count() > 0 && px.purchaseLines.Count() % param.iDisplayLength > 0)
                {
                    pages += 1;
                }
                ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.purchaseLines.Count();
            }
            
            int page = param.iDisplayStart;

            
            if (start == 0) start = 1;
            ViewData["category"] = "--Select A Category--";
            ViewData["SubCategory"] = "--Select A SubCategory--";
            ViewData["DataPost"] = DataPost;
            ViewData["listSize"] = 20;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            
            return PartialView(px);
        }

        [HttpGet]
        public ActionResult VoidGRVLine(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.PurchaseLines.Find(id);
            return PartialView(item);
        }

        [HttpPost]
        public ActionResult VoidGRVLineConfirmed(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            var co = np.GetCompanies(partz[2]);

            var item = db.PurchaseLines.Find(id);
            
            var itm = db.Items.Where(u => u.ItemCode == item.ItemCode).FirstOrDefault();
            if (itm.NewStock == null) itm.NewStock = 0;
            if (itm.Balance == null) itm.Balance = 0;
            if (itm.Quantity == null) itm.Quantity = 0;
            if (itm.Expected == null) itm.Expected = 0;
            itm.NewStock = itm.NewStock - item.quantity;
            itm.Balance = itm.Balance - item.quantity;
            itm.Quantity = itm.Quantity - item.quantity;
            itm.Expected = itm.SellingPrice * itm.Balance;
            np.UpdateItems(itm);
            db.Entry(item).State = EntityState.Deleted;
            db.SaveChanges();

            var x = np.GetAccountsByName("STOCK");
            var acc = (from p in x
                       .Where(u => u.CompanyId == co.ID)
                       select p).FirstOrDefault();
            acc.Balance = acc.Balance - (item.quantity * (itm.SellingPrice));
            np.UpdateAccounts(acc);

            if (itm.category != null && itm.category != "")
            {
                try
                {
                    var xx = np.GetAccountsByName(itm.category);
                    var accs = (from p in xx
                               .Where(u => u.CompanyId == co.ID)
                                select p).FirstOrDefault();
                    accs.Balance = accs.Balance - (item.quantity * (itm.SellingPrice));
                    np.UpdateAccounts(accs);
                }
                catch
                {

                }
            }

            if (itm.SubCategory != null && itm.SubCategory != "")
            {
                try
                {
                    var xx = np.GetAccountsByName(itm.SubCategory);
                    var accs = (from p in xx
                               .Where(u => u.CompanyId == co.ID)
                                select p).FirstOrDefault();
                    accs.Balance = accs.Balance - (item.quantity * (itm.SellingPrice));
                    np.UpdateAccounts(accs);
                }
                catch
                {

                }
            }

            var purch = np.GetPurchaseByReceipt(item.Reciept);
            if (purch.StockedValue == null) purch.StockedValue = 0;
            purch.StockedValue = purch.StockedValue - item.priceinc;
            
            np.SaveOrUpdatePurchases(purch);
            return RedirectToAction("NewGrv");
          
        }

        [HttpGet]
        public ActionResult DeleteGRV(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.Purchases.Find(id);
            return PartialView(item);
        }

        [HttpPost]
        public ActionResult DeleteGRVConfirmed(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            var co = np.GetCompanies(partz[2]);
            var purchd = db.Purchases.Find(id);
            var item = db.PurchaseLines.Where(u => u.Reciept == purchd.Reciept).ToList();
            foreach (var pd in item)
            {
                //var itemzz = db.PurchaseLines.Where(u => u.Reciept == purchd.Reciept).Fi;
                var itm = db.Items.Where(u => u.ItemCode == pd.ItemCode).FirstOrDefault();
                if (itm.NewStock == null) itm.NewStock = 0;
                if (itm.Balance == null) itm.Balance = 0;
                if (itm.Quantity == null) itm.Quantity = 0;
                if (itm.Expected == null) itm.Expected = 0;
                itm.NewStock = itm.NewStock - pd.quantity;
                itm.Balance = itm.Balance - pd.quantity;
                itm.Quantity = itm.Quantity - pd.quantity;
                itm.Expected = itm.SellingPrice * itm.Balance;
                np.UpdateItems(itm);

                //np.DeletePurchaseLines(pd);
                db.Entry(pd).State = EntityState.Deleted;
                db.SaveChanges();

                var x = np.GetAccountsByName("STOCK");
                var acc = (from p in x
                           .Where(u => u.CompanyId == co.ID)
                           select p).FirstOrDefault();
                acc.Balance = acc.Balance - (pd.quantity * (itm.SellingPrice));
                np.UpdateAccounts(acc);

                if (itm.category != null && itm.category != "")
                {
                    try
                    {
                        var xx = np.GetAccountsByName(itm.category);
                        var accs = (from p in xx
                                   .Where(u => u.CompanyId == co.ID)
                                    select p).FirstOrDefault();
                        accs.Balance = accs.Balance - (pd.quantity * (itm.SellingPrice));
                        np.UpdateAccounts(accs);
                    }
                    catch
                    {

                    }
                }

                if (itm.SubCategory != null && itm.SubCategory != "")
                {
                    try
                    {
                        var xx = np.GetAccountsByName(itm.SubCategory);
                        var accs = (from p in xx
                                   .Where(u => u.CompanyId == co.ID)
                                    select p).FirstOrDefault();
                        accs.Balance = accs.Balance - (pd.quantity * (itm.SellingPrice));
                        np.UpdateAccounts(accs);
                    }
                    catch
                    {

                    }
                }

                var purch = np.GetPurchaseByReceipt(pd.Reciept);
                if (purch.StockedValue == null) purch.StockedValue = 0;
                purch.StockedValue = purch.StockedValue - pd.priceinc;
                np.SaveOrUpdatePurchases(purch);
            }
            //np.DeletePurchases(purchd);
            db.Entry(purchd).State = EntityState.Deleted;
            db.SaveChanges();
            return RedirectToAction("NewGrv");

        }

        public ActionResult EditGRV (long ID, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            int month = 0;
            var date = DateTime.Now.Date;
            var edate = DateTime.Now.AddDays(1).Date;
            ViewData["Branch"] = param.sColumns;
            ViewData["SearchType"] = param.sEcho;
            if (string.IsNullOrEmpty(param.sStart))
            {
                ViewData["Date"] = DateTime.Now.Date.ToString("dd/MM/yyyy");
                ViewData["sdate"] = date.ToString("dd/MM/yyyy");
                ViewData["edate"] = edate.ToString("dd/MM/yyyy");

            }
            else
            {
                date = DateTime.Parse(param.sStart);
                date = date.Date;
                edate = DateTime.Parse(param.sEnd).Date;
                ViewData["sdate"] = date;
                ViewData["edate"] = edate;
                ViewData["Date"] = date.ToString("MM/dd/yyyy");
                month = date.Month;
            }

            PurchaseLine purch = np.GetPurchaseLinesById(ID);
            return PartialView(purch);
        }

        //
        // POST: /Accounts/Edit/5

        [HttpPost]
        public ActionResult EditGRV(PurchaseLine purch)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            
                PurchaseLine acnt = np.GetPurchaseLinesById(purch.ID);
                acnt.StartDate = purch.StartDate;
                acnt.EndDate = purch.EndDate;
                acnt.DeshelvingDate = purch.DeshelvingDate;

                np.SaveOrUpdatePurchaseLines(acnt);
                return RedirectToAction("GRVList"/*, new { Company = account.Location }*/);
            
        }
            

        [HttpGet]
        public ActionResult newStock(string Qty, string ItemCode, string Company, string Reciept, string ItemName, string Supplier)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            NHibernateDataProvider np = new NHibernateDataProvider();
            var itm = np.GetItemsByCode(ItemCode, partz[2]).FirstOrDefault();
            var purch = np.GetPurchaseByReceipt(Reciept);
            if (purch != null && purch.StockedValue == null) purch.StockedValue = 0;

            ViewData["Money"] = purch.total - purch.StockedValue;
            ViewData["Qty"] = Qty;
            ViewData["ItemCode"] = ItemCode;
            ViewData["Company"] = Company;
            ViewData["Reciept"] = Reciept;
            ViewData["ItemName"] = itm.ItemName;
            ViewData["Supplier"] = purch.supplier;

            return PartialView();
        }

        [HttpPost]
        public ActionResult newStock(string Qty, string ItemCode, string Company, string Reciept, string price)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            var itm = np.GetItemCode(ItemCode, partz[2]);
            
            if (itm.NewStock == null) itm.NewStock = 0;
            if (itm.Balance == null) itm.Balance = 0;
            if (itm.Expected == null) itm.Expected = 0;
            itm.NewStock = itm.NewStock + decimal.Parse(Qty);
            itm.Balance = itm.Balance + decimal.Parse(Qty);
            itm.Quantity = itm.Quantity + decimal.Parse(Qty);
            itm.Expected = itm.SellingPrice * itm.Balance;
            np.UpdateItems(itm);
           var co = np.GetCompanies(Company.Trim());

            // UPDATE STOCK ACCOUNT
           var x = np.GetAccountsByName("STOCK");
           var acc = (from p in x
                      .Where(u => u.CompanyId ==co.ID)
                      select p).FirstOrDefault();
           acc.Balance = acc.Balance + (decimal.Parse(Qty) * (itm.SellingPrice));
           np.UpdateAccounts(acc);

            if (itm.category != null && itm.category != "")
            {
                try
                {
                    var xx = np.GetAccountsByName(itm.category);
                    var accs = (from p in xx
                               .Where(u => u.CompanyId == co.ID)
                                select p).FirstOrDefault();
                    accs.Balance = accs.Balance + (decimal.Parse(Qty) * (itm.SellingPrice));
                    np.UpdateAccounts(accs);
                }catch
                {

                }
            }

            if (itm.SubCategory != null && itm.SubCategory != "")
            {
                try
                {
                    var xx = np.GetAccountsByName(itm.SubCategory);
                    var accs = (from p in xx
                               .Where(u => u.CompanyId == co.ID)
                                select p).FirstOrDefault();
                    accs.Balance = accs.Balance + (decimal.Parse(Qty) * (itm.SellingPrice));
                    np.UpdateAccounts(accs);
                }catch
                {

                }
            }

            PurchaseLine PLines = new PurchaseLine();
            PLines.Reciept = Reciept;
            PLines.item = itm.ItemName.Trim();
            PLines.quantity = decimal.Parse(Qty);
            PLines.Dated = DateTime.Now;
            PLines.Category = itm.category;
            PLines.price = decimal.Parse(price);
            PLines.tax = 0;
            PLines.ItemCode = itm.ItemCode;
            PLines.Status = "O";
            PLines.priceinc = decimal.Parse(price) * decimal.Parse(Qty);
            if (itm.tax == "Taxed") PLines.tax = decimal.Parse("0.15") * decimal.Parse(price) * decimal.Parse(Qty); 
           var pp = np.AddPurchaseLines(PLines);
          
            var purch = np.GetPurchaseByReceipt(Reciept);
           if (purch.StockedValue == null) purch.StockedValue = 0;
            purch.StockedValue = purch.StockedValue + PLines.priceinc;
            if (purch.StockedValue == purch.total)
                purch.Status = "C";
            np.SaveOrUpdatePurchases(purch);
            return RedirectToAction("NewGrv", new { Company = Company, SupplierId = purch.supplier, PurchaseId = purch.ID });
         
        }

        public ActionResult Purchases(string Company, string Supplier)
        {
            Purchase px = new Purchase();
            
            px.supplier = Supplier;
            px.company = Company;
            return PartialView(px);
        }

        [HttpPost]
        public ActionResult Purchases(Purchase purchases)
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                string resp = "";
                char[] delimiter = new char[] { '_' };
                string[] part = purchases.supplier.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (part.Length == 2)
                {
                    if (purchases.total == null || purchases.total == 0)
                    {
                        ModelState.AddModelError("","");
                        return PartialView(purchases);
                    }
                    purchases.company = purchases.company;
                    purchases.Account = part[1];
                    purchases.supplier = part[0]; 
                    purchases.Invoice = purchases.Invoice.ToUpper();
                    purchases.dated = DateTime.Now;
                    purchases.IsManufactured = false;
                    var id = np.AddPurchases(purchases);
                    var CurrentUser = User.Identity.Name;
                    char[] delimita = new char[] { '~' };
                    string[] partz = CurrentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                    var uzar = np.Getlogin(partz[1]);
                   
                    purchases.ID = id;
                    purchases.Status = "O";
                    purchases.Reciept = uzar.prefix.Trim() + "-" + id;
                    np.SaveOrUpdatePurchases(purchases);
                    /*
                    var acc = np.GetSuppliersByCode(part[1]).FirstOrDefault();
                    if (acc.Balance == null) acc.Balance = 0;
                    acc.Balance = acc.Balance + purchases.total;
                    np.SaveOrUpdateSuppliers(acc);*/

                    Transaction trn = new Transaction();
                    trn.Process("STOCK PURCHASE", decimal.Parse(purchases.total.ToString()), purchases.company, purchases.supplier);
                    return RedirectToAction("NewGrv", new { Company = purchases.company, SupplierId = purchases.supplier, PurchaseId = purchases.ID });
                }
                else
                {
                    resp = "Supplier";
                }
                return Json(resp, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json("You Need to login", JsonRequestBehavior.AllowGet); ;
            }
        }

        [HttpPost]
        public ActionResult StockOut(long id = 0)
        {
            Item item = db.Items.Find(id);
            item.Balance = 0;
            item.Quantity = 0;
            db.Entry(item).State = EntityState.Modified;
            db.SaveChanges();
              

            if (item == null)
            {
                return HttpNotFound();
            }
            return Json(new { Balance = "0.0000", NewStock = "0.0000" }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult ClearOut()
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
            var co = partz[2];
            var items = db.Items.Where(u =>u.company == co).ToList();
            foreach (var item in items)
            {
                item.Balance = 0;
                item.Quantity = 0;
                item.Expected = 0;
                item.sold = 0;
                item.NewStock = 0;
                item.transfer = 0;
                item.Returned = 0;
                item.Amount = 0;
                item.Swaps = 0;
                db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            var acc = db.Accounts.Where(u => u.CompanyId == 1).ToList();
            foreach(var a in acc)
            {
                a.Balance = 0;
                a.Opening = 0;
                db.Entry(a).State = EntityState.Modified;
                db.SaveChanges();
            }
           
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Purchases]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [PurchaseLines]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Sales]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Sales_Lines]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [DailyCashierSales]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [DailySales]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [DailyStats]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [SalesLineSammary]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [MonthlySales]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [MonthlyStats]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Orders]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [OrderLines]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Quotations]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [QuotationLines]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Returns]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [ReturnLines]");
            
            return Json(new { Balance = "0.0000", NewStock = "0.0000" }, JsonRequestBehavior.AllowGet);
        }
        #endregion

        #region Manufacturing 
        public ActionResult ManufactureDetails(string Company, string SupplierId, string PurchaseId, JQueryDataTableParamModel param, string DataPost = "Get")
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            GRV px = new Models.GRV();
            var catz = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
            Account acc = new Account();
            acc.AccountName = "--Select A Category--";
            catz.Insert(0, acc);
            ViewBag.categories = catz;
            var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
            Account accs = new Account();
            accs.AccountName = "--Select A SubCategory--";
            subCs.Insert(0, accs);
            ViewBag.SubCategories = subCs;
            //px.supplierDetails= db.Suppliers.Where(u);
            if (string.IsNullOrEmpty(PurchaseId))
            {
                px.grvDetails = db.Purchases.Where(u => u.Status == "O").FirstOrDefault();
                if (px.grvDetails != null)
                {
                    DataPost = "Post";
                    px.supplierDetails = db.Suppliers.Where(u => u.SupplierName == px.grvDetails.supplier).FirstOrDefault();
                }
            }
            else
            {
                px.grvDetails = db.Purchases.Find(long.Parse(PurchaseId));
                if (px.grvDetails != null)
                {
                    if (px.grvDetails.supplier != null)
                    {
                        SupplierId = px.grvDetails.supplier;
                    }
                    DataPost = "Post";
                }
            }
            if (px.grvDetails != null)
                px.supplierDetails = db.Suppliers.Where(u => u.SupplierName == px.grvDetails.supplier).FirstOrDefault();
            if (px.grvDetails != null)
            {
                px.purchaseLines = db.PurchaseLines.Where(u => u.Reciept == px.grvDetails.Reciept).ToList();
            }
               
            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }
            int pages = 0;
            int start = (param.iDisplayStart - 1) * param.iDisplayLength;
            if (px.purchaseLines != null)
            {
                var nn = (from emp in px.purchaseLines.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                          select emp).ToList();

                pages = px.purchaseLines.Count() / param.iDisplayLength;
                if (px.purchaseLines.Count() > 0 && pages == 0)
                {
                    pages = 1;
                }
                else if (px.purchaseLines.Count() > 0 && px.purchaseLines.Count() % param.iDisplayLength > 0)
                {
                    pages += 1;
                }
                ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.purchaseLines.Count();
            }

            int page = param.iDisplayStart;


            if (start == 0) start = 1;
            ViewData["category"] = "--Select A Category--";
            ViewData["SubCategory"] = "--Select A SubCategory--";
            ViewData["DataPost"] = DataPost;
            ViewData["listSize"] = 20;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;

            return PartialView(px);
        }

        public ActionResult ManufacturedProducts(string Company, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUserId = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = currentUserId.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var co = np.GetActivecoByName(part[1]);
            ViewData["category"] = "--Select A Category--";
            ViewData["SubCategory"] = "--Select A SubCategory--";
            if (!string.IsNullOrEmpty(param.sColumns)) ViewData["category"] = param.sColumns;
            if (!string.IsNullOrEmpty(param.sEcho)) ViewData["SubCategory"] = param.sEcho;
            ViewData["DataPost"] = "Get";
            Inventory inv = new Inventory();
           // JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllItems();
            px = (from e in px
                  where e.company == Company
                  && e.HasRecipe == true
                  select e).ToList();
            if(!string.IsNullOrEmpty(param.sSearch))
            {
                px = px.Where(u => u.ItemName.Contains(param.sSearch)).ToList();
            }
            if (!string.IsNullOrEmpty(param.sColumns))
            {
                px = px.Where(u => u.category ==param.sColumns).ToList();
            }
            if (!string.IsNullOrEmpty(param.sEcho))
            {
                px = px.Where(u => u.category == param.sColumns).ToList();
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
            ViewData["listSize"] = param.iDisplayLength;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.Count;
            Account acc = new Account();
            acc.AccountName = "--Select A Category--";
            IList<Account> cate = new List<Account>();
            IList<Account> SubC = new List<Account>();
            cate.Add(acc);
            var catz = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();

            inv.categories = catz;
            acc.AccountName = "--Select A SubCategory--";
            SubC.Add(acc);
            var ct = catz.FirstOrDefault();
            var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();

            inv.SubCategories = subCs;
            inv.items = nn;

            if (Request.IsAjaxRequest())
                return PartialView(inv);
            return View(inv);
        }

        public ActionResult ManList(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
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
            var px = db.Purchases.Where(u => u.IsManufactured == true).ToList();
            /*  px = (from emp in px
                    where (emp.dated >= sdate && emp.dated < eedate)
                    select emp).ToList();
  */
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
            var cat = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
            var SubCategories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            foreach (var item in cat)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.AccountName,
                    Value = item.AccountName.Trim()
                });
            }

            ViewBag.Cat = yr;

            List<SelectListItem> yt = new List<SelectListItem>();
            foreach (var item in cat)
            {
                yt.Add(new SelectListItem
                {
                    Text = item.AccountName,
                    Value = item.AccountName.Trim()
                });
            }

            ViewBag.SCat = yt;

            if (Request.IsAjaxRequest())
                return PartialView(nn);
            return View(nn);
        }

        public ActionResult ManLines(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.PurchaseLines.Where(u => u.Reciept == id).ToList();// np.GetOrderLinesByReciept(id);
            ViewData["Id"] = id;
            return PartialView(item);
        }

        public ActionResult Manufacture(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.Items.Where(u => u.ItemCode == id).FirstOrDefault();// np.GetOrderLinesByReciept(id);
            item.Quantity = 0;
            ViewData["Id"] = id;
            return PartialView(item);
        }

        [HttpPost]
        public ActionResult Manufacture(Item prod)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
            var rec = db.Recipes.Where(u => u.ProductCode == prod.ItemCode).ToList();
            var uzar = np.Getlogin(partz[1]);
            decimal costp = 0;
            foreach (var item in rec)
            {
                // calculate quantity of all things used to manufacture product
                var qitm = np.GetItemCode(item.ItemCode, prod.company);
                if(qitm.Quantity < (prod.Quantity * item.Quantity))
                {
                    return Json("Stock: You do not have enough "+ qitm.ItemName + " for this.You need " + (prod.Quantity * item.Quantity), JsonRequestBehavior.AllowGet);
                }
            }

            Purchase purchase = new Purchase();
            purchase.company = prod.company.Trim();
            purchase.Account = "5-0001-0000000";
            purchase.supplier = prod.company.Trim();
            purchase.Invoice = prod.Description;
            purchase.dated = DateTime.Now;
            purchase.IsManufactured = true;
            var id = np.AddPurchases(purchase);
            #region build item from recipe
            foreach (var item in rec)
            {
                // calculate quantity of all things used to manufacture product

                var itm = np.GetItemCode(item.ItemCode, prod.company);

                if (itm.NewStock == null) itm.NewStock = 0;
                if (itm.Balance == null) itm.Balance = 0;
                if (itm.Quantity == null) itm.Quantity = 0;
                if (itm.Expected == null) itm.Expected = 0;
                itm.NewStock = itm.NewStock - (prod.Quantity * (decimal)(item.Quantity));
                itm.Balance = itm.Balance - (prod.Quantity * (decimal)(item.Quantity));
                itm.Quantity = itm.Quantity - (prod.Quantity * (decimal)(item.Quantity));
                itm.Expected = itm.SellingPrice * itm.Quantity;
                np.UpdateItems(itm);
                var fco = np.GetCompanies(prod.company.Trim());

                // UPDATE STOCK ACCOUNT
                var fx = np.GetAccountsByName("STOCK");
                var facc = (from p in fx
                           .Where(u => u.CompanyId == fco.ID)
                            select p).FirstOrDefault();
                facc.Balance = facc.Balance - ((prod.Quantity * (decimal)(item.Quantity)) * (itm.SellingPrice));
                np.UpdateAccounts(facc);

                if (itm.category != null && itm.category != "")
                {
                    try
                    {
                        var xx = np.GetAccountsByName(itm.category);
                        var accs = (from p in xx
                                   .Where(u => u.CompanyId == fco.ID)
                                    select p).FirstOrDefault();
                        accs.Balance = accs.Balance - ((prod.Quantity * (decimal)(item.Quantity)) * (itm.SellingPrice));
                        np.UpdateAccounts(accs);
                    }
                    catch
                    {

                    }
                }

                if (itm.SubCategory != null && itm.SubCategory != "")
                {
                    try
                    {
                        var xx = np.GetAccountsByName(itm.SubCategory);
                        var accs = (from p in xx
                                   .Where(u => u.CompanyId == fco.ID)
                                    select p).FirstOrDefault();
                        accs.Balance = accs.Balance - ((prod.Quantity * (decimal)(item.Quantity)) * (itm.SellingPrice));
                        np.UpdateAccounts(accs);
                    }
                    catch
                    {

                    }
                }

                #region affect Cost price
                decimal plcost = 0;
                PurchaseLine pl = db.PurchaseLines.Where(u => u.ItemCode.Trim() == item.ItemCode.Trim() && u.Status.Trim() == "O").FirstOrDefault();
                decimal quant = (decimal)(prod.Quantity * (item.Quantity));
                decimal newq = 0;
                if (pl != null)
                {
                    if (pl.quantity >= (prod.Quantity * (decimal)(item.Quantity)))
                    {
                        pl.quantity = pl.quantity - (prod.Quantity * (decimal)(item.Quantity));
                        pl.Sold = pl.Sold + (prod.Quantity * (decimal)(item.Quantity));
                        db.Entry(pl).State = EntityState.Modified;
                        db.SaveChanges();
                        plcost = plcost + (decimal)(pl.price * (prod.Quantity * item.Quantity));
                        costp = costp + (decimal)(pl.price * (prod.Quantity * item.Quantity));
                    }
                    else
                    {
                        newq = quant - (decimal)pl.quantity;
                        pl.quantity = pl.quantity - (decimal)(prod.Quantity * (item.Quantity));
                        pl.Sold = pl.Sold + (decimal)(prod.Quantity * (item.Quantity));
                        pl.Status = "C";
                        pl.DateClosed = DateTime.Now;
                        db.Entry(pl).State = EntityState.Modified;
                        db.SaveChanges();
                        plcost = plcost + (decimal)(pl.price * (prod.Quantity * item.Quantity));
                        costp = costp + (decimal)(pl.price * (prod.Quantity * item.Quantity));
                        while (newq > 0)
                        {
                            quant = newq;
                            PurchaseLine npl = db.PurchaseLines.Where(u => u.ItemCode.Trim() == item.ItemCode.Trim() && u.Status.Trim() == "O").FirstOrDefault();
                            if (npl != null && npl.quantity >= quant)
                            {
                                newq = quant - (decimal)npl.quantity;
                                npl.quantity = npl.quantity - (decimal)(prod.Quantity * (item.Quantity));
                                npl.Sold = npl.Sold + (decimal)(prod.Quantity * (item.Quantity));
                                db.Entry(npl).State = EntityState.Modified;
                                db.SaveChanges();
                                plcost = plcost + (decimal)(npl.price * (prod.Quantity * item.Quantity));
                                costp = costp + (decimal)(npl.price * (prod.Quantity * item.Quantity));
                            }
                            else if (npl != null)
                            {

                                newq = quant - (decimal)npl.quantity;
                                npl.quantity = npl.quantity - (decimal)(prod.Quantity * (item.Quantity));
                                npl.Sold = npl.Sold + (decimal)(prod.Quantity * (item.Quantity));
                                npl.Status = "C";
                                npl.DateClosed = DateTime.Now;
                                db.Entry(npl).State = EntityState.Modified;
                                db.SaveChanges();
                                plcost = plcost + (decimal)(npl.price * (prod.Quantity * item.Quantity));
                                costp = costp + (decimal)(npl.price * (prod.Quantity * item.Quantity));
                            }
                        }
                    }

                    #region Add plines 
                    PurchaseLine PLine = new PurchaseLine();
                    PLine.Reciept = uzar.prefix.Trim() + "-" + id;
                    PLine.item = item.ItemName.Trim();
                    PLine.quantity = (prod.Quantity * (decimal)(item.Quantity));
                    PLine.Dated = DateTime.Now;
                    PLine.Category = itm.category;
                    PLine.price = pl.price * (prod.Quantity * (decimal)(item.Quantity));
                    PLine.tax = 0;
                    PLine.ItemCode = item.ItemCode;
                    PLine.Status = "C";
                    PLine.priceinc = plcost;
                    if (itm.tax == "Taxed") PLine.tax = decimal.Parse("0.15") * costp;
                    var ppc = np.AddPurchaseLines(PLine);
                    #endregion
                }
                #endregion
            }
            #endregion

            #region add product Stock
            var fitm = np.GetItemCode(prod.ItemCode, prod.company);

            if (fitm.NewStock == null) fitm.NewStock = 0;
            if (fitm.Balance == null) fitm.Balance = 0;
            if (fitm.Expected == null) fitm.Expected = 0;
            if (fitm.Quantity == null) fitm.Quantity = 0;
            fitm.NewStock = fitm.NewStock + prod.Quantity;
            fitm.Balance = fitm.Balance + prod.Quantity;
            fitm.Quantity = fitm.Quantity + prod.Quantity;
            
            fitm.Expected = fitm.SellingPrice * fitm.Balance;
            np.UpdateItems(fitm);
            var co = np.GetCompanies(prod.company.Trim());

            // UPDATE STOCK ACCOUNT
            var x = np.GetAccountsByName("STOCK");
            var acc = (from p in x
                       .Where(u => u.CompanyId == co.ID)
                       select p).FirstOrDefault();
            acc.Balance = acc.Balance + (prod.Quantity * (prod.SellingPrice));
            np.UpdateAccounts(acc);

            if (fitm.category != null && fitm.category != "")
            {
                try
                {
                    var xx = np.GetAccountsByName(fitm.category);
                    var accs = (from p in xx
                               .Where(u => u.CompanyId == co.ID)
                                select p).FirstOrDefault();
                    accs.Balance = accs.Balance + (prod.Quantity * (prod.SellingPrice));
                    np.UpdateAccounts(accs);
                }
                catch
                {

                }
            }

            if (fitm.SubCategory != null && fitm.SubCategory != "")
            {
                try
                {
                    var xx = np.GetAccountsByName(fitm.SubCategory);
                    var accs = (from p in xx
                               .Where(u => u.CompanyId == co.ID)
                                select p).FirstOrDefault();
                    accs.Balance = accs.Balance + (prod.Quantity * (prod.SellingPrice));
                    np.UpdateAccounts(accs);
                }
                catch
                {

                }
            }
            

           
            purchase.ID = id;
            purchase.Status = "C";
            purchase.Reciept = uzar.prefix.Trim() + "-" + id;
            np.SaveOrUpdatePurchases(purchase);
            PurchaseLine PLines = new PurchaseLine();
            PLines.Reciept = purchase.Reciept;
            PLines.item = fitm.ItemName.Trim();
            PLines.quantity = prod.Quantity;
            PLines.Dated = DateTime.Now;
            PLines.Category = fitm.category;
            PLines.price = costp / prod.Quantity;
            PLines.tax = 0;
            PLines.ItemCode = fitm.ItemCode;
            PLines.Status = "O";
            PLines.priceinc = costp;
            if (fitm.tax == "Taxed") PLines.tax = decimal.Parse("0.15") * costp;
            var pp = np.AddPurchaseLines(PLines);

            #endregion

            #region update purchase totals 
            var purch = db.Purchases.Find(id);
            purch.total = costp;
            purch.discount = 0;
            purch.UserName = partz[0];
            purch.StockedValue = costp;
            db.Entry(purch).State = EntityState.Modified;
            db.SaveChanges();
            #endregion 

            return RedirectToAction("ManufactureDetails", new { Company = prod.company, SupplierId = purchase.supplier, PurchaseId = purchase.ID });

        }
        #endregion

        #region recipes
        [HttpGet]
        public ActionResult recipes(string Company, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (partz.Length == 4)
            {
                ViewData["Role"] = partz[3];

                var co = np.GetActivecoByName(Company);
                ViewData["Item"] = param.sEcho;
                ViewData["ItemCode"] = param.sSearch;
                Inventory inv = new Inventory();
           
                var px = db.Recipes.Where(u => u.ProductCode == param.sSearch && u.Company == Company).ToList();
                

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
               

                if (Request.IsAjaxRequest()) return PartialView(nn);
                return View("Recipes", "_AdminLayout", nn);
            }
            else
            {
                return RedirectToAction("AccLogout", "Accounts");
            }
        }

        public ActionResult AddRecipeItem(string Id, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUserId = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = currentUserId.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var co = np.GetActivecoByName(part[1]);
            ViewData["category"] = "--Select A Category--";
            ViewData["SubCategory"] = "--Select A SubCategory--";
            ViewData["ItemCode"] = Id;
            Inventory inv = new Inventory();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllItems();
            px = (from e in px
                  where e.company == Company
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
            Account acc = new Account();
            acc.AccountName = "--Select A Category--";
            IList<Account> cate = new List<Account>();
            IList<Account> SubC = new List<Account>();
            cate.Add(acc);
            var catz = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
            foreach (var item in catz)
            {
                cate.Add(item);
            }
            inv.categories = cate;
            acc.AccountName = "--Select A SubCategory--";
            SubC.Add(acc);
            var ct = catz.FirstOrDefault();
            var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
            foreach (var item in subCs)
            {
                SubC.Add(item);
            }
            inv.SubCategories = SubC;
            inv.items = nn;

            if (Request.IsAjaxRequest())
                return PartialView(inv);
            return View(inv);
        }

        [HttpPost]
        public ActionResult AddRecipeItem(string category, string ItemCode,string Reciept, string company, string SubCategory, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUserId = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = currentUserId.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var co = np.GetActivecoByName(part[1]);
            ViewData["ItemCode"] = Reciept;
            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 20;
            }

            ViewData["category"] = category;
            ViewData["SubCategory"] = SubCategory;
            var gx = new List<Item>();
            var px = np.GetAllItems();
            px = (from e in px
                  where e.company == company
                  select e).ToList();
            if (!string.IsNullOrEmpty(ItemCode))
            {
                px = (from ee in px
                      where ItemCode.ToLower() == ItemCode.Trim().ToLower()
                      select ee).ToList();
            }
            if (!category.StartsWith("--") && SubCategory == "--Select A SubCategory--")
            {
                px = (from ee in px
                  .Where(u => u.category.ToLower() == category.Trim().ToLower())
                      select ee).ToList();
            }
            if (!string.IsNullOrEmpty(SubCategory) && !SubCategory.StartsWith("--"))
            {
                px = (from ee in px
                .Where(u => u.SubCategory.ToLower() == SubCategory.Trim().ToLower())
                      select ee).ToList();
            }
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
            Account acc = new Account();
            acc.AccountName = "--Select A Category--";
            IList<Account> cate = new List<Account>();
            IList<Account> SubC = new List<Account>();
            cate.Add(acc);
            var catz = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
            foreach (var item in catz)
            {
                cate.Add(item);
            }
            inv.categories = cate;
          //  acc.AccountName = "--Select A SubCategory--";
            SubC.Add(acc);
            var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
            foreach (var item in subCs)
            {
                SubC.Add(item);
            }
            inv.SubCategories = SubC;
            inv.items = nn;
            return PartialView(inv);
        }

        [HttpGet]
        public ActionResult AddRecipeMeasure(string Qty, string ItemCode, string Company, string Reciept, string ItemName, string Supplier)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            NHibernateDataProvider np = new NHibernateDataProvider();
            var itm = np.GetItemsByCode(ItemCode, Company).FirstOrDefault();
           // var purch = np.GetPurchaseByReceipt(Reciept);
            //if (purch != null && purch.StockedValue == null) purch.StockedValue = 0;

           // ViewData["Money"] = purch.total - purch.StockedValue;
            ViewData["Qty"] = Qty;
            ViewData["ItemCode"] = ItemCode;
            ViewData["Company"] = Company;
            ViewData["Reciept"] = Reciept;
            ViewData["ItemName"] = itm.ItemName;
           
          //  ViewData["Supplier"] = purch.supplier;

            return PartialView();
        }

        [HttpPost]
        public ActionResult AddRecipeMeasure(string Qty, string ItemCode, string Company, string ProductCode, string measure)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
            if (ProductCode != null)
            {
                var itm = np.GetItemCode(ItemCode, Company);
                var px = db.Recipes.Where(u => u.ProductCode == ProductCode && u.ItemCode == ItemCode).FirstOrDefault();
                if (px != null)
                {
                    px.Quantity = decimal.Parse(Qty);
                    px.Unit = measure;
                    db.Entry(px).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else
                {
                    px = new Models.Recipe();
                    px.ItemName = itm.ItemName;
                    px.ItemCode = ItemCode;
                    px.Company = itm.company;
                    px.ProductCode = ProductCode;
                    px.Quantity = decimal.Parse(Qty);
                    px.Unit = measure;
                    db.Recipes.Add(px);
                    db.SaveChanges();
                }

                return RedirectToAction("recipes", new { Company = Company, sSearch = ProductCode });
            }
            return PartialView();

        }

        [HttpGet]
        public ActionResult DeleteRecipe(int id)
        {

            NHibernateDataProvider np = new NHibernateDataProvider();
            Recipe item = db.Recipes.Find(id);
            db.Entry(item).State = EntityState.Deleted;
            db.SaveChanges();
            return PartialView("DeleteConfirmed");
        }

        
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(string id )
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Recipe item = db.Recipes.Find(long.Parse(id));
            db.Entry(item).State = EntityState.Deleted;
            db.SaveChanges();
            return PartialView("DeleteConfirmed");
        }

        #endregion

        public ActionResult Details(long id = 0)
        {
            Item item = db.Items.Find(id);
            var images = db.ItemImages.Where(u => u.ItemCode == item.ItemCode).ToList();
            var features = db.Features.Where(u => u.ItemCode == item.ItemCode).ToList();
            var itemvariations = db.ItemVariations.Where(u => u.ItemCode == item.ItemCode).ToList();
            var related = new List<Item>();
            var spec = new List<Item>();
            var itms = db.Items.ToList();
            if (item.SubCategory == null)
            {
                related = itms.Where(u => u.category == item.category).Take(5).ToList();
                spec = itms.Where(u => u.category == item.category && u.Promotion.Trim() == "YES").ToList();
                if (spec == null) spec = itms.Where(u => u.Promotion.Trim() == "YES").ToList();
            }
            else
            {
                related = itms.Where(u => u.SubCategory == item.SubCategory).Take(5).ToList();
                spec = itms.Where(u => u.SubCategory == item.SubCategory && u.Promotion.Trim() == "YES").ToList();
                if (spec == null) spec = itms.Where(u => u.Promotion.Trim() == "YES").ToList();
            }
           
            var cat = db.Accounts.Where(u => u.AccountCode.StartsWith("3-") && u.CompanyId == 1).ToList();
            item.features = features;
            item.RelatedItems = related;
            item.ItemImages = images;
            item.categories = cat;
            item.SideSpecials = spec;
            item.ItemVariations = itemvariations;

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }
        //
        public ActionResult Create(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var comp = np.GetAllCompanies();
            var countries = db.Countries.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            yr.Add(new SelectListItem
            {
                Text = "Select A Country",
                Value = "Select A Country"
            });
            foreach (var item in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.Name ,
                    Value = item.Id.ToString()
                });
            }

            ViewBag.Country = yr;

            var positions = db.AdPositions.ToList();
            List<SelectListItem> pos = new List<SelectListItem>();
            pos.Add(new SelectListItem
            {
                Text = "Select A Position",
                Value = "Select A Position"
            });
            foreach (var item in positions)
            {
                pos.Add(new SelectListItem
                {
                    Text = item.Name.Trim(),
                    Value = item.Name.Trim()
                });
            }

            ViewBag.Adposition = pos;

            var co = (from e in comp 
                      where e.name == Company
                      select e).FirstOrDefault().ID;
            var px = np.GetAccountsByCode("3");
            var category = (from e in px
                            where e.AccountCode.Length == 4 && e.CompanyId == 1
                            select e).ToList();

            var cc = category.FirstOrDefault();
            var Subcategory = (from e in px
                               where e.AccountCode.StartsWith(cc.AccountCode + "-") && e.CompanyId == 1
                               select e).ToList();
            Item itm = new Item();
            itm.companies = comp;
            itm.categories = category;
            itm.SubCategories = Subcategory;
            ViewData["Compan"] = Company;
            ViewData["Category"] = "";
            ViewData["SubCategory"] = "";
            return PartialView(itm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(Item item,HttpPostedFileBase Images)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid && item != null && item.ItemName != null)
            {
                var cat = db.Accounts.Where(u => u.AccountCode.Trim() == item.category).FirstOrDefault();
                item.category = cat.AccountName.Trim();
                if (Images != null)
                {
                    #region itemimage
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
                        var temp = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/"), fileName);
                        var pathDir = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Products/" + item.category + "/"));
                        var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Products/" + item.category + "/"), fileName);
                        var thumbPath = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Products/" + item.category + "/"), thumbname);
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

                        ResizeImage(temp, path, 500, 500);
                        ResizeImage(temp, thumbPath, 270, 270);
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


                        item.Image = "~/Content/Template/Images/Products/" + item.category + "/" + fileName;
                        item.ThumbImage = "~/Content/Template/Images/Products/" + item.category + "/" + thumbname;
                    }
                    catch (Exception)
                    {
                    }

                    #endregion
                }
               // var ee = np.GetItemCode(item.ItemCode, item.company);
                if (item.Amount == null) item.Amount = 0;
                if (item.Returned  == null) item.Returned  = 0;
                if (item.Quantity  == null) item.Quantity  = 0;
                if (item.Reorder  == null) item.Reorder  = 0;
                if (item.sold  == null) item.sold  = 0;
                if (item.transfer  == null) item.transfer = 0;
                if (item.NewStock  == null) item.NewStock  = 0;
                if (item.Balance  == null) item.Balance  = 0;
                if (item.SellingPrice == null) item.SellingPrice = 0;
                if (item.BuyingPrice  == null) item.BuyingPrice = 0;
                if (item.Measure  == null) item.Measure = "ea";
                if(item.ItemCode == null)
                {
                    var ct = item.category.Substring(0, 2);
                    var val = db.Syskeys.Where(u => u.Name == "ItemCode").FirstOrDefault();
                    string lz = "";
                    int len = 6 - val.Value.Trim().Length;
                    for(int i=1; i <= len; i++ )
                    {
                        lz = lz + "0";
                    }

                    item.ItemCode = ct + lz + val.Value;
                    val.Value = (long.Parse(val.Value) + 1).ToString();
                    db.Entry(val).State = EntityState.Modified;
                    db.SaveChanges();
                }
                    item.ItemName = item.ItemName.ToUpper();
                    var itemCode = item.ItemCode.ToUpper();
                    itemCode = itemCode.Replace(" ", "");

                    item.ItemCode = itemCode;
                    
                    np.SaveOrUpdateItems(item);
                    return RedirectToAction("Stocks", new { Company = item.company });
            }
            else
            {
                ModelState.AddModelError("", "Make Sure All fields are correctly filled in");
            }
            var countries = db.Countries.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            yr.Add(new SelectListItem
            {
                Text = "Select A Country",
                Value = "Select A Country"
            });
            foreach (var nat in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = nat.Name,
                    Value = nat.Id.ToString()
                });
            }

            ViewBag.Country = yr;
            var positions = db.AdPositions.ToList();
            List<SelectListItem> pos = new List<SelectListItem>();
            pos.Add(new SelectListItem
            {
                Text = "Select A Position",
                Value = "Select A Position"
            });
            foreach (var pr in positions)
            {
                pos.Add(new SelectListItem
                {
                    Text = pr.Name.Trim(),
                    Value = pr.Name.Trim()
                });
            }

            ViewBag.Adposition = pos;

            var comp = np.GetAllCompanies();
            var px = np.GetAccountsByCode("3");
            var co = (from e in comp
                      where e.name == item.company 
                      select e).FirstOrDefault().ID;
           
            var category = (from e in px
                            where e.AccountCode.Length == 4 && e.CompanyId == 1
                            select e).ToList();
            var cc = category.FirstOrDefault();
            var Subcategory = (from e in px
                               where e.AccountCode.StartsWith(cc.AccountCode + "-") && e.CompanyId == 1
                               select e).ToList();
            
            item.companies = comp;
            item.categories = category;
            item.SubCategories = Subcategory;
            ViewData["Compan"] = item.company;
            ViewData["Category"] = "";
            ViewData["SubCategory"] = "";
            if (Request.IsAjaxRequest()) return PartialView(item);
            return View(item);
        }

        public ActionResult Search(string json_str)
        {
          NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUserId = User.Identity.Name;
            // string company = np.Getlogin(currentUserId);
            var user = np.Getlogin(currentUserId);
            var item = np.GetItemsByCode(json_str, user.Location.Trim());
            if (item.Count() == 0)
            {
                return RedirectToAction("getMenus", new { json_str = user.Location.Trim() });
            }
            else if (item.Count() == 1)
            {
                var tt = item.FirstOrDefault();
                string returnData = tt.ItemName + "," + tt.ItemCode + "," + tt.SellingPrice + "," + tt.tax;
                return Json(returnData, JsonRequestBehavior.AllowGet);
            }
            return PartialView(item);
         }
        
        [HttpPost]
        public JsonResult GetCategories(string Id,string Company )
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
          
            var currentUserId = User.Identity.Name;
            var xx = np.GetCompanies(Id);
            var px = np.GetAccountsByCode("3");
            var category = (from e in px
                            where e.AccountCode.Length == 4 && e.CompanyId  == xx.ID 
                            select e).ToList();

            var subs = (from x in category
                        select new { Id = x.AccountCode , Name = x.AccountName.Trim() });

            return Json(subs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetSubCategories(string Id, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
           
            var currentUserId = User.Identity.Name;
            var px = db.Accounts.Where(u => u.AccountCode.StartsWith(Id + "-")).ToList();
            var xx = np.GetCompanies(Company);
            
            var subs = (from x in px
                        where x.CompanyId == xx.ID && x.AccountCode.Length > 4
                        select new { Id = x.AccountName  , Name = x.AccountName.Trim() });

            return Json(subs, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult GetItemsByCategory(string json_str)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            //System.Web.Script.Serialization.JavaScriptSerializer j = new System.Web.Script.Serialization.JavaScriptSerializer();

            var currentUserId = User.Identity.Name;
            // Subjectx ax = new Subjectx();
            //  ax = (Subjectx)j.Deserialize(json_str, typeof(Subjectx));
            var px = np.GetSubCategory(json_str);

            var subs = (from x in px
                        select new { Id = x.name, Name = x.name.Trim() });

            return Json(subs, JsonRequestBehavior.AllowGet);
        }
  
        public ActionResult ItemMenu(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
           // Items item = np.GetMenu(id);
            return PartialView();
        }

        [HttpPost]
        public ActionResult ItemMenu(PosMenu menu)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            // Items item = np.GetMenu(id);
            np.SaveOrUpdateMenu(menu);
            return PartialView();
        }

        public ActionResult Edit(int Id, int currentPage)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = np.GetItems(Id);
            var comp = np.GetAllCompanies();
            var countries = db.Countries.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            yr.Add(new SelectListItem
            {
                Text = "Select A Country",
                Value = "Select A Country"
            });
            foreach (var nat in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = nat.Name,
                    Value = nat.Id.ToString()
                });
            }

            ViewBag.Country = yr;

            var positions = db.AdPositions.ToList();
            List<SelectListItem> pos = new List<SelectListItem>();

            pos.Add(new SelectListItem
            {
                Text = "Select A Position",
                Value = "Select A Position"
            });
            foreach (var ad in positions)
            {
                pos.Add(new SelectListItem
                {
                    Text = ad.Name.Trim(),
                    Value = ad.Name.Trim()
                });
            }

            ViewBag.Adposition = pos;

            var co = (from e in comp
                      where e.name == item.company
                      select e).FirstOrDefault().ID;
            var px = np.GetAccountsByCode("3");
            var category = (from e in px
                            where e.AccountCode.Length == 4 && e.CompanyId == co
                            select e).ToList();

            var cc = (from e in category where e.AccountName.Trim() == item.category.Trim() select e).FirstOrDefault();

            var Subcategory = (from e in px
                               where e.AccountCode.StartsWith(cc.AccountCode + "-") //&& e.CompanyId == co
                               select e).ToList();
            ViewData["Category"] = "";
            ViewData["SubCategory"] = "";
            item.companies = comp;
            item.categories = category;
            item.SubCategories = Subcategory;
            ViewData["Compan"] = item.company;
            if (item.category != null) ViewData["Category"] = item.category.Trim();
            if (item.SubCategory != null) ViewData["SubCategory"] =item.SubCategory.Trim();
            ViewData["currentPage"] = currentPage.ToString();
            return PartialView(item);
        }
      
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Item item, int currentPage, HttpPostedFileBase Images, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid && item != null && item.ItemCode != null && item.ItemName != null)
            {
                var cat = db.Accounts.Where(u => u.AccountCode.Trim() == item.category).FirstOrDefault();
                item.category = cat.AccountName.Trim();

                if (Images != null )
                {
                   
                        #region itemimage
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
                            var temp = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/"), fileName);
                            var pathDir = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Products/" + item.category + "/"));
                            var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Products/" + item.category + "/"), fileName);
                            var thumbPath = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Products/" + item.category + "/"), thumbname);
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

                            ResizeImage(temp, thumbPath, 1000, 1000);
                            ResizeImage(temp, thumbPath, 270, 270);
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

                            item.Image = "~/Content/Template/Images/Products/" + item.category + "/" + fileName;
                            item.ThumbImage = "~/Content/Template/Images/Products/" + item.category + "/" + thumbname;
                        }
                        catch (Exception)
                        {
                        }
                                   
                        #endregion
                   
                }
                // var ee = np.GetItemCode(item.ItemCode, item.company);
                if (item.Amount == null) item.Amount = 0;
                if (item.Returned == null) item.Returned = 0;
                if (item.Quantity == null) item.Quantity = 0;
                if (item.Reorder == null) item.Reorder = 0;
                if (item.sold == null) item.sold = 0;
                if (item.transfer == null) item.transfer = 0;
                if (item.NewStock == null) item.NewStock = 0;
                if (item.Balance == null) item.Balance = 0;
                if (item.BuyingPrice == null) item.BuyingPrice = 0;
                item.ItemName = item.ItemName.ToUpper();
                item.ItemCode = item.ItemCode.ToUpper();
                np.UpdateItems(item);
                return RedirectToAction("Stocks", new { Company = item.company, currentPage });
            }
            else
            {
                ModelState.AddModelError("", "Make Sure All fields are correctly filled in");
            }
            var countries = db.Countries.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            yr.Add(new SelectListItem
            {
                Text = "Select A Country",
                Value = "Select A Country"
            });
            foreach (var nat in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = nat.Name,
                    Value = nat.Id.ToString()
                });
            }

            ViewBag.Country = yr;
            var positions = db.AdPositions.ToList();
            List<SelectListItem> pos = new List<SelectListItem>();
            pos.Add(new SelectListItem
            {
                Text = "Select A Position",
                Value = "Select A Position"
            });
            foreach (var pr in positions)
            {
                pos.Add(new SelectListItem
                {
                    Text = pr.Name.Trim(),
                    Value = pr.Name.Trim()
                });
            }

            ViewBag.Adposition = pos;

            var comp = np.GetAllCompanies();
            var px = np.GetAccountsByCode("3");
            var co = (from e in comp
                      where e.name == item.company
                      select e).FirstOrDefault().ID;

            var category = (from e in px
                            where e.AccountCode.Length == 4 && e.CompanyId == co
                            select e).ToList();
            var cc = (from e in category where e.AccountName.Trim() == item.category.Trim() select e).FirstOrDefault();

            var Subcategory = (from e in px
                               where e.AccountCode.StartsWith(cc.AccountCode) && e.CompanyId == co
                               select e).ToList();

            
            item.companies = comp;
            item.categories = category;
            item.SubCategories = Subcategory;
            ViewData["Compan"] = item.company;
            if (Request.IsAjaxRequest()) return PartialView(item);
            return View(item);
        }


        public ActionResult DeleteItem(string Company, int Id, int currentPage)
        {

            

            NHibernateDataProvider np = new NHibernateDataProvider();     
            Item item = db.Items.Where(u => u.ID == Id && u.company == Company).FirstOrDefault();

            ViewData["item"] = item.ItemName;
            ViewData["ID"] = item.ID.ToString();
            ViewData["currentPage"] = currentPage.ToString();
            return PartialView();
        }

        [HttpPost]
        public ActionResult DeleteItem(string ID, int currentPage)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var id = Convert.ToInt32(ID);
            Item item = db.Items.Where(u => u.ID == id).FirstOrDefault();
            var Company = item.company;
            np.DeleteItems(item);
            return RedirectToAction("Stocks", new { Company, currentPage });

        }
        public ActionResult Promotion(int Id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = np.GetItems(Id);
            var comp = np.GetAllCompanies();
            var countries = db.Countries.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            yr.Add(new SelectListItem
            {
                Text = "Select A Country",
                Value = "Select A Country"
            });
            foreach (var nat in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = nat.Name,
                    Value = nat.Id.ToString()
                });
            }

            ViewBag.Country = yr;

            var positions = db.AdPositions.ToList();
            List<SelectListItem> pos = new List<SelectListItem>();

            pos.Add(new SelectListItem
            {
                Text = "Select A Position",
                Value = "Select A Position"
            });
            foreach (var ad in positions)
            {
                pos.Add(new SelectListItem
                {
                    Text = ad.Name.Trim(),
                    Value = ad.Name.Trim()
                });
            }

            ViewBag.Adposition = pos;

            var co = (from e in comp
                      where e.name == item.company
                      select e).FirstOrDefault().ID;
            var px = np.GetAccountsByCode("3");
            var category = (from e in px
                            where e.AccountCode.Length == 4 && e.CompanyId == co
                            select e).ToList();

            var cc = (from e in category where e.AccountName.Trim() == item.category.Trim() select e).FirstOrDefault();

            var Subcategory = (from e in px
                               where e.AccountCode.StartsWith(cc.AccountCode) && e.CompanyId == co
                               select e).ToList();

            item.companies = comp;
            item.categories = category;
            item.SubCategories = Subcategory;
            ViewData["Compan"] = item.company;

            return PartialView(item);
        }

        [HttpPost]
        public ActionResult Promotion(Item item, HttpPostedFileBase Images)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid && item != null && item.ItemCode != null && item.ItemName != null)
            {
                var cat = db.Accounts.Where(u => u.AccountCode.Trim() == item.category).FirstOrDefault();
                item.category = cat.AccountName.Trim();

                if (Images != null)
                {

                    #region Promotion Image
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

                        var pathDir = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Products/" + item.category + "/"));
                        var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Products/" + item.category + "/"), fileName);
                        var image = Images;

                        if (System.IO.Directory.Exists(pathDir))
                        {
                            image.SaveAs(path);

                        }
                        else
                        {
                            Directory.CreateDirectory(pathDir);
                            image.SaveAs(path);

                        }

                        item.PromoImage = "~/Content/Template/Images/Products/" + item.category + "/" + fileName;

                    }
                    catch (Exception)
                    {
                    }

                    #endregion

                }
                // var ee = np.GetItemCode(item.ItemCode, item.company);
                if (item.Amount == null) item.Amount = 0;
                if (item.Returned == null) item.Returned = 0;
                if (item.Quantity == null) item.Quantity = 0;
                if (item.Reorder == null) item.Reorder = 0;
                if (item.sold == null) item.sold = 0;
                if (item.transfer == null) item.transfer = 0;
                if (item.NewStock == null) item.NewStock = 0;
                if (item.Balance == null) item.Balance = 0;
                if (item.BuyingPrice == null) item.BuyingPrice = 0;
                item.ItemName = item.ItemName.ToUpper();
                item.ItemCode = item.ItemCode.ToUpper();
                np.UpdateItems(item);
                return RedirectToAction("Stocks", new { Company = item.company });
            }
            else
            {
                ModelState.AddModelError("", "Make Sure All fields are correctly filled in");
            }
            var countries = db.Countries.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            yr.Add(new SelectListItem
            {
                Text = "Select A Country",
                Value = "Select A Country"
            });
            foreach (var nat in countries)
            {
                yr.Add(new SelectListItem
                {
                    Text = nat.Name,
                    Value = nat.Id.ToString()
                });
            }

            ViewBag.Country = yr;
            var positions = db.AdPositions.ToList();
            List<SelectListItem> pos = new List<SelectListItem>();
            pos.Add(new SelectListItem
            {
                Text = "Select A Position",
                Value = "Select A Position"
            });
            foreach (var pr in positions)
            {
                pos.Add(new SelectListItem
                {
                    Text = pr.Name.Trim(),
                    Value = pr.Name.Trim()
                });
            }

            ViewBag.Adposition = pos;

            var comp = np.GetAllCompanies();
            var px = np.GetAccountsByCode("3");
            var co = (from e in comp
                      where e.name == item.company
                      select e).FirstOrDefault().ID;

            var category = (from e in px
                            where e.AccountCode.Length == 4 && e.CompanyId == co
                            select e).ToList();
            var cc = (from e in category where e.AccountName.Trim() == item.category.Trim() select e).FirstOrDefault();

            var Subcategory = (from e in px
                               where e.AccountCode.StartsWith(cc.AccountCode) && e.CompanyId == co
                               select e).ToList();

            item.companies = comp;
            item.categories = category;
            item.SubCategories = Subcategory;
            ViewData["Compan"] = item.company;
            if (Request.IsAjaxRequest()) return PartialView(item);
            return View(item);
        }
        
        [HttpGet]
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = np.GetItems(id);
            return PartialView(item);
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = np.GetItems(id);
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