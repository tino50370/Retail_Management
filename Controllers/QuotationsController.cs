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
using crypto;
using System.Web.Script.Serialization;
using RetailKing.ChatServer;
using System.Net;
using System.Web.Routing;
using System.IO;

namespace RetailKing.Controllers
{   [Authorize]
    public class QuotationsController : Controller
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
            var px = np.GetAllOrders().Where(u => u.state.Trim() == "OQ").ToList();
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
           /// if (Request.IsAjaxRequest())
                return PartialView(inv);
           // return PartialView("Index", inv);
        }

        #region Quotations
        public ActionResult NewQuotation(string Company, string CustomerId, string QuotationId, JQueryDataTableParamModel param, string DataPost = "Get")
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Quote px = new Models.Quote();
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
            if (string.IsNullOrEmpty(QuotationId))
            {
               /* var quot = db.Orders.Where(u => u.state == "OQ").FirstOrDefault();
                if (quot != null)
                {
                    DataPost = "Post";
                    px.customer  = db.Customers.Where(u => u.AccountCode == quot.Account).FirstOrDefault();
                    px.quote = quot;
                    px.quotationlines = db.QuotationLines.Where(u => u.Reciept == px.quote.Reciept).ToList();
                }*/
            }
            else
            {
                px.quote = db.Orders.Find(long.Parse(QuotationId));
                if (px.quote != null)
                {
                    CustomerId = px.quote.Account;
                    px.quotationlines = db.OrderLines.Where(u => u.Reciept == px.quote.Reciept).ToList();
                }
                    DataPost = "Post";// to show the system that we are using existing quotation
                
            }
            ST_decrypt  dec = new ST_decrypt();
            if (CustomerId != null)
            {
                px.customer = db.Customers.Where(u => u.AccountCode == CustomerId).FirstOrDefault();
                px.customer.Email = dec.st_decrypt(px.customer.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
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
            if (px.quote != null)
            {
                var nn = (from emp in px.quotationlines.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
                          select emp).ToList();

                pages = px.quotationlines.Count() / param.iDisplayLength;
                if (px.quotationlines.Count() > 0 && pages == 0)
                {
                    pages = 1;
                }
                else if (px.quotationlines.Count() > 0 && px.quotationlines.Count() % param.iDisplayLength > 0)
                {
                    pages += 1;
                }
                ViewData["RecordData"] = "Showing " + start + " to " + param.iDisplayLength + " of " + px.quotationlines.Count();
            }

            int page = param.iDisplayStart;


            if (start == 0) start = 1;
            ViewData["category"] = "--Select A Category--";
            ViewData["SubCategory"] = "--Select A SubCategory--";
            ViewData["DataPost"] = DataPost;
            ViewData["listSize"] = 20;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            @ViewData["organisation"] = Company;
             return PartialView(px);
        }

        [HttpGet]
        public ActionResult VoidGRVLine(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.OrderLines.Find(id);
            return PartialView(item);
        }

        [HttpPost]
        public ActionResult VoidGRVLineConfirmed(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = db.OrderLines.Find(id);
            db.Entry(item).State = EntityState.Deleted;
            db.SaveChanges();
            var purch = db.Orders.Where(u=>u.Reciept==item.Reciept).FirstOrDefault();
            if (purch.total == null) purch.total = 0;
            purch.total = purch.total - item.priceinc;

            db.Entry(purch).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("NewQuotation");

        }

        [HttpGet]
        public ActionResult newStock(string Qty, string ItemCode, string Company, string Reciept, string ItemName, string Supplier)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            NHibernateDataProvider np = new NHibernateDataProvider();
            var itm = np.GetItemsByCode(ItemCode, Company).FirstOrDefault();
            var purch = db.Orders.Where(u => u.Reciept.Trim() == Reciept.Trim()).FirstOrDefault();
            if (purch != null && purch.total== null) purch.total = 0;

            ViewData["Money"] = String.Format("{0:F}", itm.SellingPrice);
            ViewData["Qty"] = Qty;
            ViewData["ItemCode"] = ItemCode;
            ViewData["Company"] = Company;
            ViewData["Reciept"] = Reciept;
            ViewData["ItemName"] = itm.ItemName;
            ViewData["Supplier"] = purch.customer;

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
            //np.UpdateItems(itm);
            var co = np.GetCompanies(Company.Trim());

            OrderLine PLines = new OrderLine();
            PLines.Reciept = Reciept;
            PLines.item = itm.ItemName.Trim();
            PLines.quantity = decimal.Parse(Qty);
            PLines.Dated = DateTime.Now;
            PLines.Category = itm.category;
            PLines.price = decimal.Parse(price);
            PLines.tax = 0;
            PLines.ItemCode = itm.ItemCode;
           // PLines. = "O";
            PLines.priceinc = decimal.Parse(price) * decimal.Parse(Qty);
            if (itm.tax == "Taxed") PLines.tax = decimal.Parse("0.15") * decimal.Parse(price) * decimal.Parse(Qty);
            db.OrderLines.Add(PLines);
            db.SaveChanges();

            var purch = db.Orders.Where(u => u.Reciept.Trim() == Reciept.Trim()).FirstOrDefault();
            if (purch.total == null) purch.total = 0;
            if (purch.discount  == null) purch.discount = 0;
            if (purch.Tax  == null) purch.Tax = 0;
            if (purch.Balance  == null) purch.Balance = 0;
            purch.total = purch.total + PLines.price;
            purch.Balance  = purch.Balance + PLines.priceinc ;
            if(itm.SellingPrice > PLines.priceinc)
            purch.discount = itm.SellingPrice - PLines.priceinc;
            db.Entry(purch).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("NewQuotation", new { Company = Company, CustomerId  = purch.customer, QuotationId = purch.ID });

        }

        public ActionResult Purchases(string Company, string Supplier)
        {
            Order px = new Order();
            px.customer = Supplier;
            px.company = Company;
            return PartialView(px);
        }

        [HttpPost]
        public ActionResult Purchases(Order quotation )
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                string resp = "";
                char[] delimiter = new char[] { '_' };
                string[] part = quotation.customer.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (part.Length == 2)
                {

                    quotation.Account = part[1];
                    quotation.customer = part[0];
                    quotation.dated = DateTime.Now;
                    db.Orders.Add(quotation);
                    db.SaveChanges();
                    var CurrentUser = User.Identity.Name;
                    char[] delimita = new char[] { '~' };
                    string[] partz = CurrentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                    var uzar = np.Getlogin(partz[1]);
                   // quotation.ID = id;
                    quotation.state = "OQ";
                    quotation.Reciept = uzar.prefix.Trim() + "-" + quotation.ID;
                    db.Entry(quotation).State = EntityState.Modified;
                    db.SaveChanges();
                    //np.SaveOrUpdatePurchases(quotation);
                    /*
                    var acc = np.GetSuppliersByCode(part[1]).FirstOrDefault();
                    if (acc.Balance == null) acc.Balance = 0;
                    acc.Balance = acc.Balance + purchases.total;
                    np.SaveOrUpdateSuppliers(acc);*/

                    Transaction trn = new Transaction();
                   // trn.Process("STOCK PURCHASE", decimal.Parse(purchases.total.ToString()), purchases.company, purchases.supplier);
                    return RedirectToAction("NewQuotation", new { Company = quotation.company, CustomerId = quotation.Account , QuotationId = quotation.ID });
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

        public ActionResult Details(string id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = np.GetOrderLinesByReciept(id);
            ViewData["Id"] = id;
            return PartialView(item);
        }

        [HttpPost]
        public ActionResult Notes(string Id, string item, string company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            char[] delimiters = new char[] { '/' };
            string[] parts = item.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
            List<Printdata> pds = new List<Printdata>();
            foreach (var itm in parts)
            {
                char[] delimite = new char[] { ',' };
                string[] par = itm.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                var sal = np.GetOrderLines(long.Parse(par[1]));
                var sls = np.GetSalesLinesByReciept(sal.Reciept);
                var sl = (from e in sls
                          .Where(e => e.ItemCode == sal.ItemCode)
                          select e).FirstOrDefault();
                sl.Description = par[0];
                np.SaveOrUpdateSalesLines(sl);
                np.DeleteOrderLines(sal);

                Printdata pd = new Printdata();
                pd.item = sl.item;
                pd.qty = sl.quantity.ToString();
                pd.amount = sl.priceinc.ToString();
                pd.prize = sl.Description;
                pds.Add(pd);
            }

            var ord = np.GetOrderByReciept(Id).FirstOrDefault();
            np.DeleteOrders(ord);
            var toprint = np.GetSalesLinesByReciept(Id);
            var comp = np.GetCompanies(company.Trim());
            ReceiptVM receipt = new  ReceiptVM();
            receipt.posd = pds;
            receipt.company = comp;
            return PartialView(receipt);
        }
        #endregion 
#region Invoice
        [Authorize]
        [HttpPost]
        public ActionResult Print(Quote px, string item, string tender, string change, string customer, string creditPeriod, string IsCredit, string tr, string Discount, string OrderReceipt, string collectionPoint, string Currency, string BaseTotal, string Qid)
        {
            int quaotationid = Convert.ToInt32(Qid);
            var qut = db.Orders.Find(quaotationid);
            var quotationline = db.OrderLines.Where(u => u.Reciept == qut.Reciept).ToList();
            if (item != "" && change != "NaN")
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                ReceiptVM receiptModel = new ReceiptVM();
                string account = "";
                receiptModel.ToCollect = "No";
                var todate = DateTime.Now.Date;
                var ColectionDate = DateTime.Now.Date;

                var CurrentUser = User.Identity.Name;
                char[] delimiterr = new char[] { '~' };
                string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
                string location = partz[2];

                var user = np.Getlogin(partz[1]);
                char[] delimiters = new char[] { '/' };
                string[] parts = item.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                var cust = new Customer();
                var keys = np.GetPosKeys(partz[2]);
                int cnt = parts.Count();
                List<Printdata> pds = new List<Printdata>();

                Sale sl = new Sale();

                sl.customer = qut.customer;
                sl.Account = qut.Account;
                sl.company = partz[1];
                sl.dated = DateTime.Now;
                sl.discount = decimal.Parse(Discount);
                sl.Cashier = partz[1];
                sl.PaymentModes = tr;
                sl.TransactionCurrency = Currency;
                sl.CurrencyTotal = decimal.Parse(tender);
                qut.state = "C";
                db.Sales.Add(sl);
                db.SaveChanges();
                long id = sl.ID;
              
              
                decimal CostOfSale = 0;
                decimal amt = 0;
                decimal tax = 0;
                decimal subTotal = 0;

                Transaction trn = new Transaction();

                #region  update itemdata
                foreach (var itm in quotationline)
                {
                    var priceinc = (itm.priceinc).ToString();
                    var quantity = (itm.quantity).ToString();
                    var itemcode = itm.ItemCode;
                    decimal unitprice = 0;
                    decimal baseunitprice = 0;
                    char[] delimite = new char[] { ',' };
                    //string[] par = itm.Split(delimite, StringSplitOptions.RemoveEmptyEntries);
                    var itmcode = itemcode;
                    var it = db.Items.Where(u => u.company == user.Location && u.ItemCode.Trim() == itmcode).FirstOrDefault();//np.GetItemCode(par[3],partz[1]);
                    it.Balance = it.Balance - decimal.Parse(quantity);
                    it.Quantity = it.Quantity - decimal.Parse(quantity);
                    it.sold = it.sold + decimal.Parse(quantity);
                    it.Amount = it.Amount + ((decimal)it.SellingPrice * decimal.Parse(quantity));
                    it.Expected = it.Expected - ((decimal)it.SellingPrice * decimal.Parse(quantity));
                    baseunitprice = (decimal)it.SellingPrice;
                    unitprice = Math.Round(decimal.Parse(priceinc) / decimal.Parse(quantity), 2);
                    db.Entry(it).State = EntityState.Modified;
                    db.SaveChanges();
                    //np.UpdateItems(it);

                    Printdata pd = new Printdata();
                    decimal unitNoTax = 0;
                    decimal unitTax = 0;
                    decimal cost = 0;
                    decimal money = 0;
                    decimal taxamount = 0;
                    decimal basetaxamount = 0;
                    decimal basemoney = 0;
                    decimal baseunitNoTax = 0;
                    decimal baseunitTax = 0;
                    if (it.tax == "1" || it.tax == "Taxed")
                    {
                        var taxrate = db.Syskeys.Where(u => u.Name.Trim().ToUpper() == "TAX" && u.Company == location).FirstOrDefault();
                        basemoney = Math.Round(decimal.Parse(priceinc) - decimal.Parse(priceinc) * decimal.Parse(taxrate.Value), 2);
                        money = Math.Round(((decimal)it.SellingPrice * decimal.Parse(quantity)) - decimal.Parse(priceinc) * decimal.Parse(taxrate.Value), 2);
                        baseunitNoTax = Math.Round(basemoney / decimal.Parse(quantity), 2);
                        unitNoTax = Math.Round(money / decimal.Parse(quantity), 2);
                        taxamount = Math.Round(decimal.Parse(priceinc) * decimal.Parse(taxrate.Value), 2);
                        basetaxamount = Math.Round(((decimal)it.SellingPrice * decimal.Parse(quantity)) * decimal.Parse(taxrate.Value), 2);

                        unitTax = Math.Round(unitNoTax / decimal.Parse(quantity), 2);
                        baseunitTax = Math.Round(unitNoTax / decimal.Parse(quantity), 2);
                    }
                    else
                    {
                        money = Math.Round(((decimal)it.SellingPrice * decimal.Parse(quantity)), 2);
                        unitNoTax = Math.Round(money / decimal.Parse(quantity), 2);
                        basemoney = Math.Round(decimal.Parse(priceinc), 2);
                        baseunitNoTax = Math.Round(basemoney / decimal.Parse(quantity), 2);
                        taxamount = 0;
                        basetaxamount = 0;
                        unitTax = 0;
                        baseunitTax = 0;
                    }
                    #region Sale

                    Sales_Lines bs = new Sales_Lines();
                    bs.item = it.ItemName.Trim();
                    bs.ItemCode = it.ItemCode.Trim();
                    var qunt = Convert.ToDecimal(quantity);
                    bs.quantity = qunt;
                    bs.price = basemoney;
                    bs.tax = basetaxamount;
                    bs.priceinc = Math.Round(((decimal)it.SellingPrice * decimal.Parse(quantity)), 2); //decimal.Parse(par[2]);
                    bs.TransactionPrice = money;
                    bs.Reciept = user.prefix.Trim() + "-" + id;
                    bs.Category = np.GetItemsByCode(itemcode.ToString(), user.Location.Trim()).FirstOrDefault().category;
                    bs.Dated = DateTime.Now;
                    bs.ItemCode = it.ItemCode.Trim();
                    bs.Company = partz[2];

                    amt = amt + decimal.Parse(priceinc);
                    tax = tax + taxamount;
                    subTotal = subTotal + money;

                    np.SaveOrUpdateSalesLines(bs);

                    //process sales transaction
                    #region GP
                    var slqty = decimal.Parse(quantity);
                    decimal tmpqty = 0;

                    var pl = db.PurchaseLines.Where(u => u.Company == user.Location.Trim() && u.ItemCode.Trim() == it.ItemCode.Trim() && u.Status == "O").FirstOrDefault();
                    if (pl != null)
                    {
                        if (pl.quantity >= slqty)
                        {
                            pl.quantity = pl.quantity - slqty;
                            if (pl.Sold == null) pl.Sold = 0;
                            pl.Sold = pl.Sold + slqty;
                            if (pl.quantity == 0)
                            {
                                pl.Status = "C";
                                pl.DateClosed = DateTime.Now;
                            }
                            cost = (decimal)pl.price;
                            db.Entry(pl).State = EntityState.Modified;
                            db.SaveChanges();

                            Sales_Lines ss = new Sales_Lines();
                            ss.item = it.ItemName.Trim();
                            ss.ItemCode = it.ItemCode.Trim();
                            ss.quantity = slqty;
                            ss.price = slqty * baseunitNoTax;// money; 
                            ss.tax = slqty * baseunitTax;  //taxamount;
                            ss.priceinc = slqty * baseunitprice; //decimal.Parse(par[2]); //;
                            ss.Reciept = user.prefix.Trim() + "-" + id;
                            ss.Category = np.GetItemsByCode(itemcode.ToString(), user.Location.Trim()).FirstOrDefault().category;
                            ss.Dated = DateTime.Now;
                            ss.CostPrice = cost;
                            ss.Company = partz[1];
                            ss.TransactionPrice = slqty * unitprice;
                            //amt = amt  +(decimal)ss.priceinc;// decimal.Parse(par[2]);

                            // subTotal = subTotal  + (decimal)ss.price;// decimal.Parse(par[2]);
                            //ss.Reciept 
                            // np.SaveOrUpdateSalesLines(ss);
                            CostOfSale += cost;
                        }
                        else
                        {
                            // not enough inventory in current grv
                            while (slqty > 0)
                            {
                                pl = db.PurchaseLines.Where(u => u.ItemCode.Trim() == it.ItemCode.Trim() && u.Status == "O").FirstOrDefault();
                                if (pl != null)
                                {
                                    if (pl.quantity >= slqty)
                                    {
                                        pl.quantity = pl.quantity - slqty;
                                        pl.Sold = pl.Sold + slqty;
                                        if (pl.quantity == 0)
                                        {
                                            pl.Status = "C";
                                            pl.DateClosed = DateTime.Now;
                                        }
                                        cost = slqty * (decimal)pl.price;
                                        db.Entry(pl).State = EntityState.Modified;
                                        db.SaveChanges();

                                        Sales_Lines ss = new Sales_Lines();
                                        ss.item = it.ItemName.Trim();
                                        ss.ItemCode = it.ItemCode.Trim();
                                        ss.quantity = slqty;
                                        ss.price = slqty * baseunitNoTax;// money; 
                                        ss.tax = slqty * baseunitTax;  //taxamount;
                                        ss.priceinc = slqty * baseunitprice; //decimal.Parse(par[2]); //;
                                        ss.Reciept = user.prefix.Trim() + "-" + id;
                                        ss.Category = np.GetItemsByCode(itemcode.ToString(), user.Location.Trim()).FirstOrDefault().category;
                                        ss.Dated = DateTime.Now;
                                        ss.CostPrice = cost;
                                        ss.Company = partz[1];
                                        ss.TransactionPrice = slqty * unitprice;
                                        // amt = amt  +(decimal)ss.priceinc;

                                        //subTotal = subTotal  +(decimal)ss.price; //decimal.Parse(par[2]);
                                        //ss.Reciept 
                                        //np.SaveOrUpdateSalesLines(ss);
                                        CostOfSale += cost;
                                        slqty = 0;
                                    }
                                    else
                                    {
                                        slqty = slqty - (decimal)pl.quantity;
                                        tmpqty = (decimal)pl.quantity;
                                        pl.quantity = 0;
                                        pl.Sold = pl.Sold + (decimal)pl.quantity;
                                        if (pl.quantity == 0)
                                        {
                                            pl.Status = "C";
                                            pl.DateClosed = DateTime.Now;
                                        }
                                        cost = slqty * (decimal)pl.price;
                                        db.Entry(pl).State = EntityState.Modified;
                                        db.SaveChanges();

                                        Sales_Lines ss = new Sales_Lines();
                                        ss.item = it.ItemName.Trim();
                                        ss.ItemCode = it.ItemCode.Trim();
                                        ss.quantity = tmpqty;
                                        ss.price = tmpqty * baseunitNoTax;// money; 
                                        ss.tax = tmpqty * baseunitTax;  //taxamount;
                                        ss.priceinc = tmpqty * baseunitprice; //decimal.Parse(par[2]); //;
                                        ss.Reciept = user.prefix.Trim() + "-" + id;
                                        ss.Category = np.GetItemsByCode(itemcode.ToString(), user.Location.Trim()).FirstOrDefault().category;
                                        ss.Dated = DateTime.Now;
                                        ss.CostPrice = cost;
                                        ss.Company = partz[1];
                                        ss.TransactionPrice = tmpqty * unitprice;
                                        // amt = amt + (decimal)ss.priceinc;//decimal.Parse(par[2]);

                                        // subTotal = subTotal  +(decimal)ss.price; 
                                        //ss.Reciept 
                                        ///np.SaveOrUpdateSalesLines(ss);
                                        CostOfSale += cost;
                                        // pl = db.PurchaseLines.Where(u => u.item.Trim() == it.ItemCode.Trim() && u.Status.Trim() == "O").FirstOrDefault();
                                    }
                                }
                                else
                                {
                                    Sales_Lines ss = new Sales_Lines();
                                    ss.item = it.ItemName.Trim();
                                    ss.ItemCode = it.ItemCode.Trim();
                                    ss.quantity = slqty;
                                    ss.price = slqty * baseunitNoTax;// money; 
                                    ss.tax = slqty * baseunitTax;  //taxamount;
                                    ss.priceinc = slqty * baseunitprice; //decimal.Parse(par[2]); //;
                                    ss.Reciept = user.prefix.Trim() + "-" + id;
                                    ss.Category = np.GetItemsByCode(itemcode.ToString(), user.Location.Trim()).FirstOrDefault().category;
                                    ss.Dated = DateTime.Now;
                                    ss.CostPrice = cost;
                                    ss.Description = "Item has no latest GRV record";
                                    ss.Company = partz[1];
                                    ss.TransactionPrice = slqty * unitprice;
                                    // amt = amt + (decimal)ss.priceinc;//decimal.Parse(par[2]);

                                    //subTotal = subTotal + (decimal)ss.price;
                                    //ss.Reciept 
                                    //np.SaveOrUpdateSalesLines(ss);
                                }
                            }
                        }
                    }
                    else
                    {
                        Sales_Lines ss = new Sales_Lines();
                        ss.item = it.ItemName.Trim();
                        ss.ItemCode = it.ItemCode.Trim();
                        ss.quantity = slqty;
                        ss.price = slqty * baseunitNoTax;// money; 
                        ss.tax = slqty * baseunitTax;  //taxamount;
                        ss.priceinc = slqty * baseunitprice; //decimal.Parse(par[2]); //;
                        ss.Reciept = user.prefix.Trim() + "-" + id;
                        ss.Category = np.GetItemsByCode(itemcode.ToString(), user.Location.Trim()).FirstOrDefault().category;
                        ss.Dated = DateTime.Now;
                        ss.CostPrice = cost;
                        ss.Description = "Item has no GRV record";
                        ss.Company = partz[1];
                        ss.TransactionPrice = slqty * unitprice;
                        //amt = amt + (decimal)ss.priceinc;//decimal.Parse(par[2]);

                        // subTotal = subTotal + (decimal)ss.price;
                        //ss.Reciept 
                        // np.SaveOrUpdateSalesLines(ss);
                    }
                    #endregion

                    trn.ProcessSale("", decimal.Parse(priceinc), user.Location.Trim(), it.category, it.SubCategory, customer);

                    pd.item = bs.item;
                    pd.qty = bs.quantity.ToString();
                    pd.amount = priceinc;
                    pd.prize = Math.Round((decimal)(decimal.Parse(priceinc) / bs.quantity), 2).ToString();
                    if (it.Measure == null)
                    {
                        pd.unit = "ea";
                    }
                    else
                    {
                        pd.unit = it.Measure.Trim();
                    }

                    pds.Add(pd);

                    if (keys != null && keys.OrderBased != null && keys.OrderBased.Trim() == "Y")
                    {
                        OrderLine oo = new OrderLine();
                        oo.item = it.ItemName.Trim();
                        oo.ItemCode = it.ItemCode.Trim();
                        oo.quantity = money;
                        oo.price = taxamount;
                        oo.tax = tax + taxamount;
                        oo.priceinc = decimal.Parse(priceinc);
                        oo.Reciept = user.prefix.Trim() + "-" + id;
                        oo.Dated = DateTime.Now;
                        // amt = amt + decimal.Parse(par[2]);
                        //ss.Reciept 
                        np.SaveOrUpdateOrderLines(oo);
                    }
                    #endregion

                    #region sales sammary

                    var slsam = db.SalesLineSammaries.Where(u => u.ItemCode.Trim() == it.ItemCode.Trim() && u.Dated == todate && u.DeliveryType.Trim() == "Collection").FirstOrDefault();
                    var updat = "Yes";
                    if (slsam == null)
                    {
                        updat = "No";
                        slsam = new SalesLineSammary();
                    }

                    slsam.ItemCode = it.ItemCode;
                    slsam.ItemName = it.ItemName;
                    if (slsam.Total == null) slsam.Total = 0;
                    if (slsam.Quantity == null) slsam.Quantity = 0;
                    if (slsam.Balance == null) slsam.Balance = 0;
                    if (slsam.Received == null) slsam.Received = 0;
                    if (slsam.DeliveredQty == null) slsam.DeliveredQty = 0;

                    slsam.Quantity = slsam.Quantity + qunt;
                    slsam.Balance = slsam.Quantity - slsam.DeliveredQty;
                    slsam.Total = slsam.Total + decimal.Parse(priceinc);
                    slsam.Dated = DateTime.Now.Date;
                    slsam.Company = partz[2];
                    slsam.DeliveryType = "Collection";
                    slsam.Deadline = DateTime.Now.AddDays(1);
                    slsam.DeliveryPicking = false;
                    if (slsam.GPAmount == null) slsam.GPAmount = 0;
                    slsam.GPAmount = slsam.GPAmount + (decimal.Parse(priceinc) - (cost * qunt));
                    if (updat == "Yes")
                    {
                        db.Entry(slsam).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.SalesLineSammaries.Add(slsam);
                        db.SaveChanges();
                    }
                    #endregion
                }
                #endregion

                var company = np.GetCompanies(user.Location.Trim());

                var trns = tr.Split(',');

                foreach (var ptrn in trns)
                {
                    var trd = ptrn.Split('-');
                    if (trd.Length >= 2)
                    {
                        customer = qut.customer;
                        trn.ProcessSale(trd[0], decimal.Parse(trd[1]), user.Location.Trim(), "", "", customer);
                        #region cashier sales
                        var dasid = DateTime.Now.ToString("ddMMyyyy") + user.ID + "_" + trd[0];
                        DailyCashierSale das = db.DailyCashierSales.Find(dasid);
                        if (das == null)
                        {
                            das = new DailyCashierSale();
                            das.Date = DateTime.Now.ToString("dd/MM/yyyy");
                            if (das.Sales == null) das.Sales = 0;
                            das.Sales = das.Sales + amt;
                            das.Id = dasid;
                            das.CompanyId = company.ID;
                            das.Cashier = user.username;
                            das.TransactionDate = DateTime.Now;
                            das.AccountName = trd[0];
                            db.DailyCashierSales.Add(das);
                            db.SaveChanges();

                            //np.SaveOrUpdateDailySales(ds);
                        }
                        else
                        {
                            das.Date = DateTime.Now.ToString("dd/MM/yyyy");
                            if (das.Sales == null) das.Sales = 0;
                            das.Sales = das.Sales + amt;
                            das.Hits += 1;
                            // das.Id = dasid;
                            das.Cashier = user.username;
                            das.CompanyId = company.ID;
                            das.TransactionDate = DateTime.Now;
                            das.AccountName = trd[0];
                            db.Entry(das).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion

                        if (trd[2] == "Y")
                        {
                            if (cust.Balance == null) cust.Balance = 0;
                            cust.Balance = cust.Balance + decimal.Parse(trd[1]);
                           // db.Entry(cust).State = EntityState.Modified;
                            db.SaveChanges();
                            receiptModel.creditBalance = receiptModel.creditBalance + decimal.Parse(trd[1]);
                        }

                        #region Sales By Account
                        DailySale dsa = np.GetDailySales(DateTime.Now.ToString("ddMMyyyy") + company.ID + "_" + trd[0]);
                        if (dsa == null)
                        {
                            dsa = new DailySale();
                            dsa.Date = DateTime.Now.ToString("dd/MM/yyyy");
                            if (dsa.Sales == null) dsa.Sales = 0;
                            dsa.Sales = dsa.Sales + decimal.Parse(trd[1]);
                            dsa.Id = DateTime.Now.ToString("ddMMyyyy") + company.ID + "_" + trd[0];
                            dsa.CompanyId = company.ID;
                            dsa.TransactionDate = DateTime.Now;
                            dsa.AccountName = trd[0];
                            db.DailySales.Add(dsa);
                            db.SaveChanges();

                            //np.SaveOrUpdateDailySales(ds);
                        }
                        else
                        {
                            dsa.Date = DateTime.Now.ToString("dd/MM/yyyy");
                            if (dsa.Sales == null) dsa.Sales = 0;
                            dsa.Sales = dsa.Sales + decimal.Parse(trd[1]);
                            dsa.Hits += 1;
                            // dsa.Id = DateTime.Now.ToString("ddMMyyyy") + company.ID + "_" + trd[0];
                            dsa.CompanyId = company.ID;
                            dsa.TransactionDate = DateTime.Now;
                            dsa.AccountName = trd[0];
                            db.Entry(dsa).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                        #endregion
                    }
                }

                #region Complete Sale
                var sal = np.GetSales(id);
                sal.total = decimal.Parse(BaseTotal); ;
                if (tender == "0") tender = amt.ToString();
                if (IsCredit == "Y")
                {
                    sal.Invoice = user.prefix.Trim() + "-" + sl.ID;
                    sal.Reciept = user.prefix.Trim() + "-" + sl.ID;
                    sal.Period = creditPeriod;
                    sal.Balance = receiptModel.creditBalance;
                    sal.state = "O";
                    sal.CostPrice = CostOfSale;
                    sal.GPAmount = sal.total - sal.CostPrice;
                   // sal.CollectionId = long.Parse(collectionPoint);
                    sal.Tender = decimal.Parse(BaseTotal);
                    sal.company = partz[2];
                    receiptModel.Receipt = sal.Invoice;
                }
                else
                {
                    sal.Reciept = user.prefix.Trim() + "-" + sl.ID;
                    sal.Period = "0";
                    sal.state = "C";
                    sal.CostPrice = CostOfSale;
                    //sal.CollectionId = long.Parse(collectionPoint);
                    sal.company = partz[2];
                    sal.GPAmount = sal.total - sal.CostPrice;
                    sal.Tender = decimal.Parse(BaseTotal);
                    receiptModel.Receipt = sal.Reciept;
                }
                receiptModel.total = amt.ToString();
                receiptModel.tax = tax.ToString();
                receiptModel.subTotal = subTotal.ToString();
                receiptModel.tender = tender;
                receiptModel.change = change;
                receiptModel.cashier = partz[1];
                receiptModel.posd = pds;
                receiptModel.company = company;
                sal.Miscellenious = new JavaScriptSerializer().Serialize(receiptModel);
                np.SaveOrUpdateSales(sal);
                #endregion

                try
                {
                    #region Delivery
                    // delivary details 
                    if (keys.DoDeliveries == true)
                    {

                        var deliva = new Delivery();
                        deliva.CustomerId = cust.ID;
                        deliva.CustomerName = customer;
                        deliva.DateCreated = DateTime.Now;
                        deliva.DeadLine = ColectionDate;
                        deliva.CustomerMobile = cust.Phone1;
                        deliva.CustomerNationalId = cust.Phone2;

                        deliva.Status = "O";
                        if (creditPeriod == "Collected")
                            deliva.Status = "C";
                        deliva.Receipt = user.prefix.Trim() + "-" + sl.ID;
                        if (creditPeriod == "ToDeliver")
                        {
                            var region = db.Regions.Find(cust.RegionId);
                            deliva.RegionId = cust.RegionId;
                            deliva.Region = region.Name.Trim();
                            deliva.DeliveryType = sl.DeliveryType;
                            deliva.Address = cust.Address1 + " " + cust.Address2;
                            deliva.Amount = region.DeliveryRate;
                            deliva.Saburb = cust.Suburb;

                            deliva.City = cust.City;
                        }
                        else
                        {
                            if (IsCredit == "Y")
                            {
                                var cpoint = db.CollectionPoints.Find(long.Parse(collectionPoint));
                                deliva.RegionId = cpoint.RegionId;
                                deliva.Region = cpoint.RegionName.Trim();
                                deliva.DeliveryType = "Collection";
                                deliva.Address = cpoint.Address;
                                deliva.Saburb = cpoint.Suburb.Trim();
                                deliva.City = cpoint.City.Trim();
                                deliva.Amount = 0;
                            }
                            else
                            {
                                // var cpoint = db.CollectionPoints.Find(long.Parse(collectionPoint));
                                deliva.RegionId = 1;
                                deliva.Region = "N/A";
                                deliva.DeliveryType = "Collection";
                                deliva.Address = "N/A";
                                deliva.Saburb = "N/A";
                                deliva.City = "N/A";
                                deliva.Amount = 0;
                            }

                        }

                        db.Deliveries.Add(deliva);
                        db.SaveChanges();
                        receiptModel.diliveryId = deliva.Id.ToString();
                        receiptModel.diliveryAddress = deliva.Address;
                        receiptModel.CustomerName = deliva.CustomerName;
                        receiptModel.CustomerMobile = deliva.CustomerMobile;

                    }
                    #endregion
                }
                catch { }


                #region Remove order
                if (!string.IsNullOrEmpty(OrderReceipt))
                {
                    var oda = db.Orders.Where(u => u.Invoice == OrderReceipt).FirstOrDefault();
                    db.Orders.Remove(oda);
                    db.SaveChanges();

                    var odal = db.OrderLines.Where(u => u.Reciept == OrderReceipt).ToList();
                    foreach (var it in odal)
                    {
                        db.OrderLines.Remove(it);
                        db.SaveChanges();
                    }
                    // return Json("Done", JsonRequestBehavior.AllowGet);
                }
                #endregion

                #region stats
                DailySale ds = np.GetDailySales(DateTime.Now.ToString("ddMMyyyy") + company.ID + "_SALE");
                if (ds == null)
                {
                    ds = new DailySale();
                    ds.Date = DateTime.Now.ToString("dd/MM/yyyy");
                    if (ds.Sales == null) ds.Sales = 0;
                    ds.Sales = ds.Sales + amt;
                    ds.Id = DateTime.Now.ToString("ddMMyyyy") + company.ID + "_SALE";
                    ds.CompanyId = company.ID;
                    ds.TransactionDate = DateTime.Now;
                    ds.AccountName = "SALE";
                    db.DailySales.Add(ds);
                    db.SaveChanges();

                    //np.SaveOrUpdateDailySales(ds);
                }
                else
                {
                    ds.Date = DateTime.Now.ToString("dd/MM/yyyy");
                    if (ds.Sales == null) ds.Sales = 0;
                    ds.Sales = ds.Sales + amt;
                    ds.Hits += 1;
                    // ds.Id = DateTime.Now.ToString("ddMMyyyy") + company.ID + "_SALE";
                    ds.CompanyId = company.ID;
                    ds.TransactionDate = DateTime.Now;
                    ds.AccountName = "SALE";
                    db.Entry(ds).State = EntityState.Modified;
                    db.SaveChanges();
                }

                MonthlySale ms = np.GetMonthlySales(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + company.ID);
                if (ms == null)
                {
                    ms = new MonthlySale();
                    ms.Id = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + company.ID;
                    ms.Sales = amt;
                    ms.Hits = 1;
                    ms.AccountName = "SALE";
                    ms.Year = DateTime.Now.Year;
                    ms.CompanyId = company.ID;
                    db.MonthlySales.Add(ms);
                    db.SaveChanges();

                }
                else
                {
                    // ms.Id = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + company.ID;
                    ms.Sales = amt;
                    ms.Hits += 1;
                    ms.AccountName = "SALE";
                    ms.Year = DateTime.Now.Year;
                    ms.CompanyId = company.ID;
                    db.Entry(ms).State = EntityState.Modified;
                    db.SaveChanges();
                }
                // sales by cashier


                #endregion

                try
                {
                    // print to dispatch

                    string s = RenderPartialView("Pos", "~/Views/Pos/Print.cshtml", receiptModel);
                    var context = Microsoft.AspNet.SignalR.GlobalHost.ConnectionManager.GetHubContext<ChatHub>();

                    string Ip = GetIP();
                    var toTill = db.PosPrinters.Where(u => u.Type == "Till" && u.Location == company.name && u.IpAddress == Ip).FirstOrDefault();
                    if (toTill != null)
                    {
                        context.Clients.Client(toTill.ConnectionId.Trim()).sendPrintMessage(s);
                    }
                    var toDispatch = db.PosPrinters.Where(u => u.Type == "Dispatch" && u.Location == company.name).FirstOrDefault();
                    if (toDispatch != null)
                    {
                        context.Clients.Client(toDispatch.ConnectionId.Trim()).sendPrintMessage(s);
                    }

                }
                catch (Exception E)
                {

                }

                return RedirectToAction("Index"); 

            }

            return PartialView("Index");
        }

        String GetIP()
        {
            string strHostName = Dns.GetHostName();

            // Find host by name
            IPHostEntry iphostentry = Dns.GetHostEntry(strHostName);

            // Grab the first IP addresses
            String IPStr = "";
            foreach (IPAddress ipaddress in iphostentry.AddressList)
            {
                IPStr = ipaddress.ToString();
                return IPStr;
            }
            return IPStr;
        }
        public static string RenderPartialView(string controllerName, string partialView, object model)
        {
            //var context = new HttpContextWrapper(System.Web.HttpContext.Current) as HttpContextBase;
            var contxt = MockHelper.FakeHttpContext();
            var context = new HttpContextWrapper(contxt) as HttpContextBase;
            var routes = new System.Web.Routing.RouteData();
            routes.Values.Add("controller", controllerName);

            var requestContext = new RequestContext(context, routes);

            string requiredString = requestContext.RouteData.GetRequiredString("controller");
            var controllerFactory = ControllerBuilder.Current.GetControllerFactory();
            var controller = controllerFactory.CreateController(requestContext, requiredString) as ControllerBase;

            controller.ControllerContext = new ControllerContext(context, routes, controller);

            var ViewData = new ViewDataDictionary();

            var TempData = new TempDataDictionary();

            ViewData.Model = model;

            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialView);
                var viewContext = new ViewContext(controller.ControllerContext, viewResult.View, ViewData, TempData, sw);

                viewResult.View.Render(viewContext, sw);
                return sw.GetStringBuilder().ToString();
            }
        }
        [Authorize]
        public ActionResult Tender(int QuotationId,string PaymentType, string Currency, decimal Baseamount = 0, decimal PartTotal = 0, decimal amount = 0)
        {

            NHibernateDataProvider np = new NHibernateDataProvider();
           // var qu = db.OrderLines.Find(QuotationId)
            
           var quot = db.Orders.Where(u => u.ID == QuotationId).FirstOrDefault();
            amount = Convert.ToDecimal(quot.Balance);
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            var location = partz[2];
            var comp = db.Companies.Where(u => u.name == location).FirstOrDefault();
            var px = new List<Account>();
            Acc pp = new Acc();
            pp.quot = QuotationId;
            if (string.IsNullOrEmpty(PaymentType))
            {
                PaymentType = "SALE";
                var dd = db.Currencies.Where(u => u.Company == location).ToList();
                if (!string.IsNullOrEmpty(Currency))
                {

                    pp.Accnt = db.Accounts.Where(u => u.CompanyId == comp.ID && u.AccountCode.StartsWith("2") && u.AccountCode.Length > 4 && u.Currency == Currency).ToList();
                }
                else
                {
                    pp.Accnt = db.Accounts.Where(u => u.CompanyId == comp.ID && u.AccountCode.StartsWith("2") && u.AccountCode.Length > 4).ToList();

                }
                foreach (var itm in pp.Accnt)
                {
                    var rate = dd.Where(u => u.Id == itm.CurrencyId).FirstOrDefault();
                    decimal topay = 0;
                    if (Baseamount > 0)
                    {
                        topay = Baseamount - (PartTotal);
                    }
                    else
                    {
                        topay = amount;
                    }
                    itm.Balance = topay * rate.ExchangeRate;
                    itm.Opening = rate.ExchangeRate;
                }
            }
            else
            {

                pp.Accnt = db.Accounts.Where(u => u.CompanyId == comp.ID && u.AccountCode.StartsWith("6") && u.AccountCode.Length > 4).ToList();
            }

            

            return PartialView("_Tender", pp);
            // return PartialView("Tender");
        }
        [Authorize]
        public ActionResult TenderData(string type, string currency, decimal amount, int Quotationidd)
        {

            NHibernateDataProvider np = new NHibernateDataProvider();
            var quot = db.Orders.Where(u => u.ID == Quotationidd).FirstOrDefault();
            
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
            var r = type.Split('_');
            var keys = np.GetPosKeys(partz[1]);
            var company = partz[2];
            var cu = partz[1];
            var user = db.logins.Where(u => u.username == cu).FirstOrDefault();
            var cc = db.Companies.Where(u => u.name == company).FirstOrDefault();
            ViewData["ActiveCity"] = cc.location.Trim();
            ViewData["Paid"] = amount;
            ViewData["Qid"] = Quotationidd.ToString();
            
            if (r.Length == 1 && keys != null && user.PosCustomers == true)
            {
                ViewData["Date"] = DateTime.Now;
                List<SelectListItem> mixr = new List<SelectListItem>();

                mixr.Add(new SelectListItem
                {
                    Text = "Collect Now",
                    Value = "Collected"
                });
                mixr.Add(new SelectListItem
                {
                    Text = "Collect Later",
                    Value = "ToCollect"
                });
                mixr.Add(new SelectListItem
                {
                    Text = "Deliver",
                    Value = "Delivery"
                });

                ViewBag.ResortArea = mixr;
                ViewData["Type"] = type;

                var cty = db.Cities.OrderBy(u => u.Name).ToList();
                var fc = cty.Where(u => u.Name.ToLower().Trim() == cc.location.ToLower().Trim()).FirstOrDefault();
                List<SelectListItem> ct = new List<SelectListItem>();
                ct.Add(new SelectListItem
                {
                    Text = "--Select City--",
                    Value = "--Select City--"
                });
                foreach (var item in cty)
                {
                    ct.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Name.Trim()
                    });
                }
                ViewBag.Cities = ct;

                var points = db.CollectionPoints.Where(u => u.City.Trim() == fc.Name.Trim()).ToList();
                List<SelectListItem> cp = new List<SelectListItem>();
                cp.Add(new SelectListItem
                {
                    Text = "--Select Collection point--",
                    Value = "--Select Collection point--"
                });
                foreach (var item in points)
                {
                    cp.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.CollectionPoint = cp;

                return PartialView("CustomerData");
            }
            else
            {
                ViewData["Type"] = type;
                // ViewData["Amount"] = amount;
                return PartialView("Tender");
            }

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