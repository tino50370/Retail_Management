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

namespace RetailKing.Controllers
{
    [Authorize]
    public class LossController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Items/

        public ActionResult Index(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
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
            var px = np.GetAllLosses().Where(u => u.state.Trim() == "C" && u.company == company).ToList();
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

        [HttpPost]
        public ActionResult Index(string category, string ItemCode, JQueryDataTableParamModel param)
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
                inv.SubCategories = (from e in np.GetAccountsByCode("3-") where e.AccountCode.Length == 8 select e).ToList();
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
                return PartialView("IndexSearches",inv);
            return View(inv);
        }

        [HttpPost]
        public ActionResult IndexSearches(string category, string ItemCode, string company, string SubCategory, JQueryDataTableParamModel param, string json_str)
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
            if (!category.StartsWith("--")  && SubCategory == "--Select A SubCategory--")
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
            //var items = db.Items.Where(u => u.ItemName.Contains(json_str) || u.ItemCode.Contains(json_str)).ToList();
            return PartialView("IndexSearches", inv);
        }

        public ActionResult Authorizeloss(string date, string enddate, string category, string ItemCode, string company, JQueryDataTableParamModel param)
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
            var px = np.GetAllLosses().Where(u => u.state.Trim() == "A").ToList();
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
        public ActionResult CompleteAuthorizeloss(string Reciept)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
           var purch = db.Losses.Where(u => u.Reciept.Trim() == Reciept).FirstOrDefault(); ;
           // if (purch.StockedValue == null) purch.StockedValue = 0;
//purch.StockedValue = purch.StockedValue + PLines.priceinc;
          //  if (purch.StockedValue == purch.total)
           var CurrentUser = User.Identity.Name;
           char[] delimiterr = new char[] { '~' };
           string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

           var user = np.Getlogin(partz[1]);
                purch.state = "C";
                purch.Authorizer = user.username;
            //np.SaveOrUpdateLossLines(purch);
                db.SaveChanges();
            return RedirectToAction("NewLoss", new {LossId = purch.ID });
         
        }
        public ActionResult CompleteShrinkage(string Reciept)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var purch = db.Losses.Where(u => u.Reciept.Trim() == Reciept).FirstOrDefault(); ;
            // if (purch.StockedValue == null) purch.StockedValue = 0;
            //purch.StockedValue = purch.StockedValue + PLines.priceinc;
            //  if (purch.StockedValue == purch.total)
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            var user = np.Getlogin(partz[1]);
            purch.state = "C";
            purch.Authorizer = user.username;
            //np.SaveOrUpdateLossLines(purch);
            db.SaveChanges();
            return RedirectToAction("Index", new { LossId = purch.ID , company =user.Location });

        }
        #region Quotations
        public ActionResult NewLoss(string Company, string LossId, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            RetailKingEntities db = new RetailKingEntities();
            LossQuote px = new Models.LossQuote();
            List<LossLine> lines = new List<LossLine>();
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

            //Get user authorize 
            var CurrentUser = User.Identity.Name;
           char[] delimiterr = new char[] { '~' };
           string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

           var user = np.Getlogin(partz[1]);
           long userid = user.ID;
            
               var xs= db.logins.Where(x => x.ID == userid).FirstOrDefault();
               px.uzar = xs;
          //  px.uzar
            //px.supplierDetails= db.Suppliers.Where(u);
            if (string.IsNullOrEmpty(LossId))
            {
                px.quote = db.Losses.Where(u => u.state == "O").FirstOrDefault();
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
                px.quote = db.Losses.Find(long.Parse(LossId));
                if (px.quote != null)
                {
                    px.quotationlines = db.LossLines.Where(u => u.Reciept == px.quote.Reciept).ToList();
                }
                else
                {
                    px.quotationlines = lines;
                }
                //if (px.quote != null)
                //{
                //    CustomerId = px.quote.Account;
                //    px.quotationlines = db.OrderLines.Where(u => u.Reciept == px.quote.Reciept).ToList();
                //}
                //DataPost = "Post";// to show the system that we are using existing quotation

            }
            //ST_decrypt dec = new ST_decrypt();
            //if (CustomerId != null)
            //{
            //    px.customer = db.Customers.Where(u => u.AccountCode == CustomerId).FirstOrDefault();
            //    px.customer.Email = dec.st_decrypt(px.customer.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
            //}
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
                if (px.quotationlines == null) px.quotationlines = new List<LossLine>();
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
            // ViewData["DataPost"] = DataPost;
            ViewData["listSize"] = 20;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;

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
            var purch = db.Orders.Where(u => u.Reciept == item.Reciept).FirstOrDefault();
            if (purch.total == null) purch.total = 0;
            purch.total = purch.total - item.priceinc;

            db.Entry(purch).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("NewLoss");

        }

        [HttpGet]
        public ActionResult newStock(string Qty, string ItemCode, string Company, string Reciept, string ItemName, string Supplier)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            NHibernateDataProvider np = new NHibernateDataProvider();
            var itm = np.GetItemsByCode(ItemCode, Company).FirstOrDefault();
            var purchz = db.PurchaseLines.Where(u => u.ItemCode.Replace(" ","") == ItemCode.Trim()).FirstOrDefault();
            var purch = db.Losses.Where(u => u.Reciept.Trim() == Reciept.Trim()).FirstOrDefault();
            if (purch != null && purch.total == null) purch.total = 0;

            ViewData["Money"] = String.Format("{0:F}", purchz.price);
            ViewData["Qty"] = Qty;
            ViewData["ItemCode"] = ItemCode;
            ViewData["Company"] = Company;
            ViewData["Reciept"] = Reciept;
            ViewData["ItemName"] = itm.ItemName;
            //ViewData["Supplier"] = purch.customer;
            var item = db.Items.Where(u => u.ItemName.Contains(ItemCode) || u.ItemCode.Contains(ItemCode)).ToList();
            return PartialView(new { items = item });
        }

        [HttpPost]
        public ActionResult newStock(string Qty, string ItemCode, string Company, string Reciept, string price, string item, string x)
        {
            if (item != "")
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

                var user = np.Getlogin(partz[1]);
                char[] delimiters = new char[] { '/' };
                string[] parts = item.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                var cust = new Customer();
                var keys = np.GetPosKeys(partz[1]);
                
                int cnt = parts.Count();
                List<Printdata> pds = new List<Printdata>();
                Loss sl = new Loss();

              
                sl = db.Losses.Where(u => u.Reciept == Reciept).FirstOrDefault() ;
                long id = sl.ID;
                decimal CostOfSale = 0;
                decimal amt = 0;
                decimal tax = 0;
                decimal subTotal = 0;

                Transaction trn = new Transaction();

                #region  update itemdata
                foreach (var itm in parts)
                {
                    decimal unitprice = 0;
                    char[] delimite = new char[] { ',' };
                    string[] par = itm.Split(delimite, StringSplitOptions.RemoveEmptyEntries);
                    var itmcode = par[3];
                    var it = db.Items.Where(u => u.ItemCode.Trim() == itmcode).FirstOrDefault();//np.GetItemCode(par[3],partz[1]);
                    it.Balance = it.Balance - decimal.Parse(par[1]);
                    it.Quantity = it.Quantity - decimal.Parse(par[1]);
                    it.sold = it.sold + decimal.Parse(par[1]);
                    it.Amount = it.Amount + decimal.Parse(par[2]);
                    it.Expected = it.Expected - decimal.Parse(par[2]);
                    unitprice = (decimal)it.SellingPrice;
                    db.Entry(it).State = EntityState.Modified;
                    db.SaveChanges();
                    amt = decimal.Parse(par[1]) * unitprice;

                    Printdata pd = new Printdata();
                    decimal unitNoTax = 0;
                    decimal unitTax = 0;
                    decimal cost = 0;
                    decimal money = 0;
                    decimal taxamount = 0;
                    if (it.tax == "1" || it.tax == "Taxed")
                    {
                        money = decimal.Parse(par[2]) - Math.Round(decimal.Parse(par[2]) * decimal.Parse("0.130435"), 2);
                        unitNoTax = money / decimal.Parse(par[1]);
                        taxamount = Math.Round(decimal.Parse(par[2]) * decimal.Parse("0.130435"), 2);
                        unitTax = unitNoTax / decimal.Parse(par[1]);
                    }
                    else
                    {
                        money = Math.Round(decimal.Parse(par[2]), 2);
                        unitNoTax = money / decimal.Parse(par[1]);
                        taxamount = 0;
                        unitTax = 0;
                    }
                    #region Sale
                    
                    LossLine  bs = new LossLine ();
                    bs.item = it.ItemName.Trim();
                    bs.ItemCode = it.ItemCode.Trim();
                    bs.quantity = long.Parse(par[1]);
                    bs.price = unitprice;
                    bs.tax = taxamount;
                    bs.priceinc = decimal.Parse(par[2]);
                    bs.Reciept = user.prefix.Trim() + "-" + id;
                    bs.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
                    bs.Dated = DateTime.Now;
                    bs.ItemCode = it.ItemCode.Trim();
                    bs.Company = partz[1];
                    amt = amt + decimal.Parse(par[2]);
                    tax = tax + taxamount;
                    subTotal = subTotal + unitprice;

                    np.SaveOrUpdateLossLines(bs);

                    //process sales transaction
                    #region GP
                    var slqty = decimal.Parse(par[1]);
                    decimal tmpqty = 0;

                    var pl = db.PurchaseLines.Where(u => u.ItemCode.Trim() == it.ItemCode.Trim() && u.Status == "O").FirstOrDefault();
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

                            LossLine  ss = new LossLine ();
                            ss.item = it.ItemName.Trim();
                            ss.ItemCode = it.ItemCode.Trim();
                            ss.quantity = slqty;
                            ss.price = slqty * unitNoTax;// money; 
                            ss.tax = slqty * unitTax;  //taxamount;
                            ss.priceinc = slqty * unitprice; //decimal.Parse(par[2]); //;
                            ss.Reciept = user.prefix.Trim() + "-" + id;
                            ss.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
                            ss.Dated = DateTime.Now;
                            ss.CostPrice = cost;
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

                                        LossLine  ss = new LossLine ();
                                        ss.item = it.ItemName.Trim();
                                        ss.ItemCode = it.ItemCode.Trim();
                                        ss.quantity = slqty;
                                        ss.price = slqty * unitNoTax;// money; 
                                        ss.tax = slqty * unitTax;  //taxamount;
                                        ss.priceinc = slqty * unitprice; //decimal.Parse(par[2]); //;
                                        ss.Reciept = user.prefix.Trim() + "-" + id;
                                        ss.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
                                        ss.Dated = DateTime.Now;
                                        ss.CostPrice = cost;
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

                                        LossLine  ss = new LossLine ();
                                        ss.item = it.ItemName.Trim();
                                        ss.ItemCode = it.ItemCode.Trim();
                                        ss.quantity = tmpqty;
                                        ss.price = tmpqty * unitNoTax;// money; 
                                        ss.tax = tmpqty * unitTax;  //taxamount;
                                        ss.priceinc = tmpqty * unitprice; //decimal.Parse(par[2]); //;
                                        ss.Reciept = user.prefix.Trim() + "-" + id;
                                        ss.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
                                        ss.Dated = DateTime.Now;
                                        ss.CostPrice = cost;
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
                                    LossLine  ss = new LossLine ();
                                    ss.item = it.ItemName.Trim();
                                    ss.ItemCode = it.ItemCode.Trim();
                                    ss.quantity = slqty;
                                    ss.price = slqty * unitNoTax;// money; 
                                    ss.tax = slqty * unitTax;  //taxamount;
                                    ss.priceinc = slqty * unitprice; //decimal.Parse(par[2]); //;
                                    ss.Reciept = user.prefix.Trim() + "-" + id;
                                    ss.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
                                    ss.Dated = DateTime.Now;
                                    ss.CostPrice = cost;
                                    ss.Description = "Item has no latest GRV record";
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
                        LossLine  ss = new LossLine ();
                        ss.item = it.ItemName.Trim();
                        ss.ItemCode = it.ItemCode.Trim();
                        ss.quantity = slqty;
                        ss.price = slqty * unitNoTax;// money; 
                        ss.tax = slqty * unitTax;  //taxamount;
                        ss.priceinc = slqty * unitprice; //decimal.Parse(par[2]); //;
                        ss.Reciept = user.prefix.Trim() + "-" + id;
                        ss.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
                        ss.Dated = DateTime.Now;
                        ss.priceinc  = cost;
                        ss.Description = "Item has no GRV record";
                        //amt = amt + (decimal)ss.priceinc;//decimal.Parse(par[2]);

                        // subTotal = subTotal + (decimal)ss.price;
                        //ss.Reciept 
                        // np.SaveOrUpdateSalesLines(ss);
                    }
                    #endregion

                    trn.ProcessSale("Loss", decimal.Parse(par[2]), user.Location.Trim(), it.category, it.SubCategory, "");

                    pd.item = bs.item;
                    pd.qty = bs.quantity.ToString();
                    pd.amount = bs.priceinc.ToString();
                    pd.prize = Math.Round((decimal)(decimal.Parse(par[2]) / bs.quantity), 2).ToString();
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
     
                    #endregion

                  
                }
                #endregion

                var company = np.GetCompanies(user.Location.Trim());
          
                }
               
                   #region Complete Sale
                    var sal = db.Losses.Find(id);
                    if (sal.total == null) sal.total = 0;
                    sal.total += amt;
                    sal.Reciept = user.prefix.Trim() + "-" + sl.ID;
                    sal.Period = "0";
                    sal.state = "O";
                    sal.cashier = user.username;
                    if (sal.CostPrice == null) sal.CostPrice = 0;
                    sal.CostPrice += CostOfSale;
                    
                    //sal.CollectionId = long.Parse(collectionPoint);
                    //.GPAmount = sal.total - sal.CostPrice;
                    //sal.Tender = decimal.Parse(tender);
                    db.Entry(sal).State = EntityState.Modified;
                    db.SaveChanges();
                #endregion
                   return RedirectToAction("NewLoss", new { Company = Company, LossId = sl.ID });
            }


            return PartialView();
        }

        public ActionResult Purchases(string Company)
        {
            Loss px = new Loss();
            // px.customer = Supplier;
            px.company = Company;
            return PartialView(px);
        }

        [HttpPost]
        public ActionResult Purchases(Loss quotation, string Company)
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                var CurrentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] partz = CurrentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                string resp = "";
                // char[] delimiter = new char[] { '_' };
                //tring[] part = quotation.customer.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);


                //quotation.Account = part[1];
                quotation.company = partz[2];
                quotation.state  = "O";
                quotation.dated = DateTime.Now;
                db.Losses.Add(quotation);
                db.SaveChanges();

                var uzar = np.Getlogin(partz[1]);
                // quotation.ID = id;
                quotation.state = "O";
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
                return RedirectToAction("NewLoss", new { Company = quotation.company, CustomerId = quotation.Account, LossId = quotation.ID });

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
            var item = db.LossLines.Where(u => u.Reciept == id).ToList();
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
            ReceiptVM receipt = new ReceiptVM();
            receipt.posd = pds;
            receipt.company = comp;
            return PartialView(receipt);
        }



        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                // context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
                #endregion