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

namespace RetailKing.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
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
            var co = np.GetActivecoByName(part[1]);
            
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
        public ActionResult IndexSearches(string category, string ItemCode, string company, string SubCategory, JQueryDataTableParamModel param)
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
        public ActionResult Stocks(string Company)
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
        public ActionResult Stocks(string category, string ItemCode, string company, string SubCategory,JQueryDataTableParamModel param)
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
            if (!string.IsNullOrEmpty(ItemCode))
            {
                px =  px.Where(u => u.ItemCode.ToLower().Contains(ItemCode.Trim().ToLower()) || u.ItemName.ToLower().Contains(ItemCode.Trim().ToLower()) ) 
                      .ToList();
            }
            if ((category != "--Select A Category--" && category != "--Select A SubCategory--") && (SubCategory == "--Select A SubCategory--" || string.IsNullOrEmpty(SubCategory)))
            {
                px =  px.Where(u => u.category.Trim() == category.Trim())
                     .ToList();
            }
            if (!string.IsNullOrEmpty(SubCategory) && SubCategory != "--Select A SubCategory--")
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
        public ActionResult NewGrv(string Company, string SupplierId, string PurchaseId,  JQueryDataTableParamModel param, string DataPost = "Get")
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
            if(string.IsNullOrEmpty(PurchaseId))
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
                    DataPost = "Post";
                }
            }
            if(SupplierId != null)
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
            var item = db.PurchaseLines.Find(id);
            db.Entry(item).State = EntityState.Deleted;
            db.SaveChanges();
            var purch = np.GetPurchaseByReceipt(item.Reciept);
            if (purch.StockedValue == null) purch.StockedValue = 0;
            purch.StockedValue = purch.StockedValue - item.priceinc;
            
            np.SaveOrUpdatePurchases(purch);
            return RedirectToAction("NewGrv");
          
        }

        [HttpGet]
        public ActionResult newStock(string Qty, string ItemCode, string Company, string Reciept, string ItemName, string Supplier)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            NHibernateDataProvider np = new NHibernateDataProvider();
            var itm = np.GetItemsByCode(ItemCode, Company).FirstOrDefault();
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

            var itm = np.GetItemCode(ItemCode, Company);
            
            if (itm.NewStock == null) itm.NewStock = 0;
            if (itm.Balance == null) itm.Balance = 0;
            if (itm.Expected == null) itm.Expected = 0;
            itm.NewStock = itm.NewStock + decimal.Parse(Qty);
            itm.Balance = itm.Balance + decimal.Parse(Qty);
            itm.Quantity = itm.Quantity + decimal.Parse(Qty);
            itm.Expected = itm.SellingPrice * itm.Balance;
            np.UpdateItems(itm);
           var co = np.GetCompanies(Company.Trim());

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
                    purchases.company = purchases.company;
                    purchases.Account = part[1];
                    purchases.supplier = part[0]; 
                    purchases.Invoice = purchases.Invoice.ToUpper();
                    purchases.dated = DateTime.Now;
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
                    item.ItemCode = item.ItemCode.ToUpper();
                    
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

        public ActionResult Edit(int Id)
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
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(Item item, HttpPostedFileBase Images)
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
            return View(item);
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