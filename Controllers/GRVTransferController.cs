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
    public class GRVTransferController : Controller
    {

        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();

        #region GRVs
        public ActionResult GrvList(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
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
            var px = db.Purchases.Where(u => u.IsManufactured != true).OrderBy(u => u.dated).ToList();

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


        public ActionResult Browse(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
           // var comp = np.GetAllCompanies();
           var pp = np.GetAllTransfers();
            var px = (from e in pp.Where(u => u.destination == Company && u.State == "C" ) select e).ToList();

            return PartialView(px);
        }

        public ActionResult GrvLines(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.PurchaseLines.Where(u => u.Reciept == id).ToList();// np.GetOrderLinesByReciept(id);
            ViewData["Id"] = id;
            return PartialView(item);
        }

        public ActionResult NewGrv(string Company, string SupplierId, string TransferId, string PurchasesId, JQueryDataTableParamModel param, string DataPost = "Get")
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            GRV px = new Models.GRV();
            Transfer trnf = new Transfer();
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
            if (TransferId != null)
            {
            // get tranfer by id
            var idtran = db.Transfers.Find(long.Parse(TransferId));
            var tranRec = idtran.Reciept;
            //declare a Purchasesline list itm
            //create new purchase
            var purchId = db.Purchases.Find(long.Parse(PurchasesId));

            //gt transfer lines by receipt
            var trnl = db.TransferingLines.Where(u=> u.Reciept == tranRec).ToList();
            //loop thrugh the transfer 
            foreach (var item in trnl)
            {
                PurchaseLine pu = new PurchaseLine();
                pu.Reciept = purchId.Reciept;
                pu.ItemCode = item.ItemCode;
                pu.item = item.items;
                pu.quantity = item.quantity;
                pu.Category = item.Category;
                pu.price = item.price;
                pu.Status = "C";
                pu.DateClosed = DateTime.Now;
                var itm = np.GetItemCode(item.ItemCode, Company);

                if (itm.NewStock == null) itm.NewStock = 0;
                if (itm.Balance == null) itm.Balance = 0;
                if (itm.Expected == null) itm.Expected = 0;
                itm.NewStock = itm.NewStock + item.quantity;
                itm.Balance = itm.Balance + item.quantity;
                itm.Quantity = itm.Quantity + item.quantity;
                itm.Expected = itm.SellingPrice * itm.Balance;
                np.UpdateItems(itm);
                db.PurchaseLines.Add(pu);

                db.SaveChanges();
            }
            idtran.State = "CC";
           purchId.Status = "C";
           purchId.shift = "Transfer";
            db.SaveChanges();
            px.grvDetails = db.Purchases.Find(long.Parse(PurchasesId));
            if (px.grvDetails != null)
                px.companyDetails = db.Companies.Where(u => u.name== px.grvDetails.company).FirstOrDefault();
            if (px.grvDetails != null)
                px.purchaseLines = db.PurchaseLines.Where(u => u.Reciept == px.grvDetails.Reciept).ToList();
                px.grvTransfer = db.Transfers.Find(long.Parse(TransferId));
        }
           
            //    if (px.grvDetails != null)
            //    {
            //        if (px.grvDetails.supplier != null)
            //        {
            //            SupplierId = px.grvDetails.supplier;
            //        }
            //        DataPost = "Post";
            //    }
            //}
            //if (px.grvDetails != null)
            //    px.supplierDetails = db.Suppliers.Where(u => u.SupplierName == px.grvDetails.supplier).FirstOrDefault();
           
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

            // UPDATE STOCK ACCOUNT
            var x = np.GetAccountsByName("STOCK");
            var acc = (from p in x
                       .Where(u => u.CompanyId == co.ID)
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
                    accs.Balance = accs.Balance + (decimal.Parse(Qty) * (itm.SellingPrice));
                    np.UpdateAccounts(accs);
                }
                catch
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

        public ActionResult Purchases(Purchase Purchases, string Company, string Supplier,string Recieptno)
        {
             
            Transfer px = new Transfer();
            Purchases.supplier = Supplier;
            px.Reciept = Supplier;
            px.company = Company;
            return PartialView(px);
        }

        [HttpPost]
        public ActionResult Purchases(Purchase purchases, string Recieptno, string TransferId)
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                string resp = "";
                char[] delimiter = new char[] { '_' };
                string[] part = purchases.Reciept.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
               Transfer cust = new Transfer();
                cust.Reciept = part[1];
                var pz = np.GetAllTransfers();
                var pp = db.Transfers.Where(v=>v.Reciept == cust.Reciept).FirstOrDefault();
                var tranid = pp.ID;
                if (part.Length == 2)
                {
                    purchases.company = purchases.company;
                    purchases.Account = part[1];
                    purchases.supplier = part[0];
                    purchases.Invoice = purchases.Invoice.ToUpper();
                    purchases.dated = DateTime.Now;
                    purchases.IsManufactured = false;
                   var id = np.AddPurchases(purchases);
                    //var id = pp.ID;
                    var CurrentUser = User.Identity.Name;
                    char[] delimita = new char[] { '~' };
                    string[] partz = CurrentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);
                    purchases.total = pp.total;
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
                    return RedirectToAction("NewGrv", new { Company = purchases.company, SupplierId = purchases.supplier, PurchasesId = purchases.ID,TransferId =tranid });
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
            var items = db.Items.Where(u => u.company == co).ToList();
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
            foreach (var a in acc)
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
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [Returns]");
            db.Database.ExecuteSqlCommand("TRUNCATE TABLE [ReturnLines]");

            return Json(new { Balance = "0.0000", NewStock = "0.0000" }, JsonRequestBehavior.AllowGet);
        }
        #endregion


    }
}
