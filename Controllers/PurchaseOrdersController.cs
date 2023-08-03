using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.ViewModels;
using RetailKing.DataAcess;

namespace RetailKing.Controllers
{   
    public class PurchaseOrdersController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Items/

       public ActionResult Index(string date, string enddate,string category ,string ItemCode, string company, JQueryDataTableParamModel param)
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
              var px = db.PurchaseOrders.ToList();
              px = (from emp in px
                   where (emp.dated >= sdate && emp.dated < eedate)
                   select emp).ToList();
                         
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
                    Text = item.AccountName  ,
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

        [HttpPost]
        public ActionResult Index(string category ,string ItemCode,JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            
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
                inv.categories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
                inv.SubCategories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
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
                inv.categories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
                inv.SubCategories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
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
                inv.categories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 4 select e).ToList();
                inv.SubCategories =(from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
                inv.items = nn;
                return PartialView(inv);
            }
            return PartialView();
                
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
            var subCs = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
            foreach (var item in subCs)
            {
                SubC.Add(item);
            }
            inv.SubCategories = SubC;
            inv.items = nn;
            return PartialView("Index", inv);
        }

        public ActionResult Hamper(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.PurchaseOrderLines.Where(u => u.Reciept.Trim() ==id).ToList();
            return PartialView(item);
        }

        #region purchase oders
        public ActionResult NewPO(string Company, string SupplierId, string PurchaseId, JQueryDataTableParamModel param, string DataPost = "Get")
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            PurchaseOrderz px = new Models.PurchaseOrderz();
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
                px.grvDetails = db.PurchaseOrders.Where(u => u.Status == "O").FirstOrDefault();
                if (px.grvDetails != null)
                {
                    DataPost = "Post";
                    px.supplierDetails = db.Suppliers.Where(u => u.SupplierName == px.grvDetails.supplier).FirstOrDefault();
                }
            }
            else
            {
                px.grvDetails = db.PurchaseOrders.Find(long.Parse(PurchaseId));
                if (px.grvDetails != null)
                {
                    DataPost = "Post";
                }
            }
            if (SupplierId != null)
                px.supplierDetails = db.Suppliers.Where(u => u.SupplierName == px.grvDetails.supplier).FirstOrDefault();
            if (px.grvDetails != null)
                px.purchaseLines = db.PurchaseOrderLines.Where(u => u.Reciept == px.grvDetails.Reciept).ToList();
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
            var item = db.PurchaseOrderLines.Find(id);
            return PartialView(item);
        }

        [HttpPost]
        public ActionResult VoidGRVLineConfirmed(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.PurchaseOrderLines.Find(id);
            db.Entry(item).State = EntityState.Deleted;
            db.SaveChanges();
            var purch = db.PurchaseOrders.Where(u=>u.Reciept==item.Reciept).FirstOrDefault();
            if (purch.StockedValue == null) purch.StockedValue = 0;
            purch.StockedValue = purch.StockedValue - item.priceinc;

            db.Entry(purch).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("NewPO");

        }

        [HttpGet]
        public ActionResult newStock(string Qty, string ItemCode, string Company, string Reciept, string ItemName, string Supplier)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            NHibernateDataProvider np = new NHibernateDataProvider();
            var itm = np.GetItemsByCode(ItemCode, Company).FirstOrDefault();
            var purch = db.PurchaseOrders.Where(u => u.Reciept.Trim() == Reciept.Trim()).FirstOrDefault();
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

            PurchaseOrderLine PLines = new PurchaseOrderLine();
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
            db.PurchaseOrderLines.Add(PLines);
            db.SaveChanges();

            var purch = db.PurchaseOrders.Where(u => u.Reciept.Trim() == Reciept.Trim()).FirstOrDefault();
            if (purch.StockedValue == null) purch.StockedValue = 0;
            purch.StockedValue = purch.StockedValue + PLines.priceinc;
            if (purch.StockedValue == purch.total)
                purch.Status = "C";
            db.Entry(purch).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("NewPO", new { Company = Company, SupplierId = purch.supplier, PurchaseId = purch.ID });

        }

        public ActionResult Purchases(string Company, string Supplier)
        {
            PurchaseOrder px = new PurchaseOrder();
            px.supplier = Supplier;
            px.company = Company;
            return PartialView(px);
        }

        [HttpPost]
        public ActionResult Purchases(PurchaseOrder purchases)
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
                    var id = db.PurchaseOrders.Add(purchases);
                    db.SaveChanges();
                    var CurrentUser = User.Identity.Name;
                    char[] delimita = new char[] { '~' };
                    string[] partz = CurrentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                    var uzar = np.Getlogin(partz[1]);
                   // purchases.ID = id;
                    purchases.Status = "O";
                    purchases.Reciept = uzar.prefix.Trim() + "-" + purchases.ID;
                    db.Entry(purchases).State = EntityState.Modified;
                    db.SaveChanges();
                    /*
                    var acc = np.GetSuppliersByCode(part[1]).FirstOrDefault();
                    if (acc.Balance == null) acc.Balance = 0;
                    acc.Balance = acc.Balance + purchases.total;
                    np.SaveOrUpdateSuppliers(acc);*/

                    Transaction trn = new Transaction();
                    trn.Process("STOCK PURCHASE", decimal.Parse(purchases.total.ToString()), purchases.company, purchases.supplier);
                    return RedirectToAction("NewPO", new { Company = purchases.company, SupplierId = purchases.supplier, PurchaseId = purchases.ID });
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
        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               // context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}