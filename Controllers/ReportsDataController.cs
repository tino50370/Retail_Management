using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.DataAcess;
using Newtonsoft.Json;

namespace RetailKing.Controllers
{   
    public class ReportsDataController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        public ActionResult CashPosition(string Company, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            //JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllAccounts();
            var comp = np.GetActivecoByName(Company);
            px = (from e in px
                  where e.CompanyId == comp.CompanyId
                  orderby e.AccountCode ascending 
                  select e).ToList();

            var debts = np.GetAllSalesOpenCredit(Company);
            var creds = np.GetAllPurchasesOpenCredit(Company);

            var uniqDebts = (from e in debts select new { e.customer }).Distinct();
            var uniqCreds = (from e in creds select new { e.supplier }).Distinct();

            IList<Customer> cust = new List<Customer>();
            IList<Supplier> sup = new List<Supplier>();
            foreach (var item in uniqDebts)
             {
                 var dd = np.GetCustomersByName(item.customer).FirstOrDefault();
                 cust.Add(dd);
             }
            foreach (var item in uniqCreds)
            {
                var ss = np.GetSuppliersByName(item.supplier).FirstOrDefault();
                sup.Add(ss);
            }
            if (param.sStart == null)
            {
                param.sStart = DateTime.Now.ToString() ;
            }
            if (param.sEnd == null)
            {
                param.sEnd = DateTime.Now.ToString();
            }
            var exp = np.GetExpensesBydateRange(Company, DateTime.Parse(param.sStart),DateTime.Parse(param.sEnd));

            
            CashPosition cshp = new CashPosition();
            cshp.accounts = px;
            cshp.customers = cust;
            cshp.suppliers = sup;
            cshp.expenses = exp;

            return PartialView(cshp);
        }

        public ActionResult Purchases(string Company, JQueryDataTableParamModel param)
        {
            int month = 0;
            var date = DateTime.Now.Date;
            var edate = DateTime.Now.AddDays(1).Date;
            ViewData["Branch"] = param.sColumns;
            ViewData["SearchType"] = param.sEcho;
            if (string.IsNullOrEmpty(param.sStart))
            {
                ViewData["Date"] = DateTime.Now.Date.ToString("MM/dd/yyyy");
               
            }
            else
            {
                date = DateTime.Parse(param.sStart);
                date = date.Date;
                edate = date.AddDays(1).Date;
                ViewData["Date"] = date.ToString("MM/dd/yyyy"); ;
                month = date.Month;
            }
            var px = db.Purchases.ToList();

                if (param.sEcho == "Date")
                {
                    px = px.Where(u =>  u.dated >= date && u.dated <= edate).ToList();
                }

            if (!string.IsNullOrEmpty(param.sColumns) && param.sColumns != "ALL")
            {
                px = px.Where(u => u.supplier == param.sColumns).ToList();
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
            return PartialView();
        }

        public ActionResult SalesByUser(string Company, JQueryDataTableParamModel param)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            Company = part[2];
            int month = 0;
            var date = DateTime.Now.Date;
            var edate = DateTime.Now.AddDays(1).Date;
            ViewData["Branch"] = param.sColumns;
            ViewData["SearchType"] = param.sEcho;
            if (string.IsNullOrEmpty(param.sStart))
            {
                ViewData["Date"] = DateTime.Now.Date.ToString("MM/dd/yyyy");
                ViewData["sdate"] = date;
                ViewData["edate"] = edate;

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
            
             var logins  = db.logins.Where(u => u.Location == Company && u.PosUser == true).ToList();

            List<SelectListItem> yr = new List<SelectListItem>();
           
            foreach (var item in logins)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.username.Trim(),
                    Value = item.username.Trim()
                });
            }
            ViewBag.Cashiers = yr;
           
            if (string.IsNullOrEmpty(param.sSearch))
            {
                ViewData["Cashier"] = logins.FirstOrDefault().username;
                param.sSearch = logins.FirstOrDefault().username;
            }
            else
            {
                ViewData["Cashier"] = param.sSearch;
            }
            ViewBag.Salez = db.DailyCashierSales.Where(u => u.Cashier == param.sSearch && (u.TransactionDate >= date && u.TransactionDate <= edate )).ToList();
            var px = db.Sales.Where(u => u.Cashier == param.sSearch && (u.dated >= date && u.dated  <= edate)).ToList();
            if (param.sEcho == "Date")
            {
                px = px.Where(u => u.dated  >= date && u.dated <= edate).ToList();
            }


            if (!string.IsNullOrEmpty(param.sColumns) && param.sColumns != "ALL")
            {
              //  px = px.Where(u => u.supplier == param.sColumns).ToList();
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

        public ActionResult SalesSammary(string Company, JQueryDataTableParamModel param)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            Company = part[2];
            int month = 0;
            var date = DateTime.Now.Date;
            var edate = DateTime.Now.AddDays(1).Date;
            ViewData["Branch"] = param.sColumns;
            ViewData["SearchType"] = param.sEcho;
            if (string.IsNullOrEmpty(param.sStart))
            {
                ViewData["Date"] = DateTime.Now.Date.ToString("MM/dd/yyyy");
                ViewData["sdate"] = date;
                ViewData["edate"] = edate;

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

            var px = db.SalesLineSammaries.Where(u =>  (u.Dated >= date && u.Dated <= edate)).ToList();
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                px = px.Where(u => u.ItemCode.Contains(param.sSearch) || u.ItemName.Contains(param.sSearch)).ToList();
            }
            


            if (!string.IsNullOrEmpty(param.sColumns) && param.sColumns != "ALL")
            {
                //  px = px.Where(u => u.supplier == param.sColumns).ToList();
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

        public ActionResult SalesByProductCustomer(string Company, JQueryDataTableParamModel param, string ProductID, string cc)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            Company = part[2];
            int month = 0;
            var date = DateTime.Now.Date;
            var edate = DateTime.Now.AddDays(1).Date;
            ViewData["Branch"] = param.sColumns;
            ViewData["SearchType"] = param.sEcho;
            ViewData["searchcustomer"] = param.sSearch;
            if (string.IsNullOrEmpty(param.sStart))
            {
                ViewData["Date"] = DateTime.Now.Date.ToString("MM/dd/yyyy");
                ViewData["sdate"] = date;
                ViewData["edate"] = edate;

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
            //var z = (from x in db.Sales_Lines
            //            where x.ItemCode == ProductID
            //         join sale in db.Sales on x.Reciept equals sale.Reciept
            //         select sale).ToList();

            var x = (from sl in db.Sales_Lines
                     join s in db.Sales
                     on sl.Reciept equals s.Reciept
                     where sl.ItemCode == ProductID && (sl.Dated >= date && sl.Dated <= edate)
                     select new { Receipt = sl.Reciept, item = sl.item, priceinc = sl.priceinc, quantity = sl.quantity, SubCategory = s.Cashier, Description = s.customer, Company = s.company, Dated = s.dated }).ToList();

            var trn = JsonConvert.SerializeObject(x);
            var px = JsonConvert.DeserializeObject<List<Sales_Lines>>(trn);
            //var px = x.Where(u=> u.Dated >= date && u.Dated <= edate).ToList();
            
              
                     
            if (!string.IsNullOrEmpty(param.sSearch))
            {
                px = px.Where(u => u.Description.Contains(param.sSearch) || u.SubCategory.Contains(param.sSearch)).ToList();
            }
            if (!string.IsNullOrEmpty(cc))
            {
                px = px.Where(u => u.Description.Contains(cc) || u.SubCategory.Contains(cc)).ToList();
            }


            if (!string.IsNullOrEmpty(param.sColumns) && param.sColumns != "ALL")
            {
                 px = px.Where(u => u.Company== param.sColumns).ToList();
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
            ViewData["itm"] = ProductID;
            return PartialView(nn);

        }

        public ActionResult Collections(string Company, JQueryDataTableParamModel param)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            Company = part[2];
            int month = 0;
            var date = DateTime.Now.Date;
            var edate = DateTime.Now.AddDays(1).Date;
            ViewData["Branch"] = param.sColumns;
            ViewData["SearchType"] = param.sEcho;
            if (string.IsNullOrEmpty(param.sStart))
            {
                ViewData["Date"] = DateTime.Now.Date.ToString("MM/dd/yyyy");
                ViewData["sdate"] = date;
                ViewData["edate"] = edate;

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

            var px = db.Deliveries.Where(u => u.DeliveryType.Trim() == "Collection" && u.Status == "O").ToList();
            px = (from emp in px
                  where (emp.DeadLine.Value.Date >= date.Date && emp.DeadLine.Value.Date <= edate.Date)
                  select emp).ToList();

            if (!string.IsNullOrEmpty(param.sSearch))
            {
                px = px.Where(u => u.CustomerName.Contains(param.sSearch) || u.Receipt.Contains(param.sSearch)).ToList();
            }



            if (!string.IsNullOrEmpty(param.sColumns) && param.sColumns != "ALL")
            {
                //  px = px.Where(u => u.supplier == param.sColumns).ToList();
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

        public ActionResult DailyCollectionReport(string Company, JQueryDataTableParamModel param)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            Company = part[2];
            int month = 0;
            var date = DateTime.Now.Date;
            var edate = DateTime.Now.AddDays(1).Date;
            ViewData["Branch"] = param.sColumns;
            ViewData["SearchType"] = param.sEcho;
            if (string.IsNullOrEmpty(param.sStart))
            {
                ViewData["Date"] = DateTime.Now.Date.ToString("MM/dd/yyyy");
                ViewData["sdate"] = date;
                ViewData["edate"] = edate;

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

            var cashreport = db.CashCollections.Where(u => u.Type == "Dayend" || u.Type == "Cashup").ToList();

            List<SelectListItem> yr = new List<SelectListItem>();

            foreach (var item in cashreport)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.Type.Trim(),
                    Value = item.Type.Trim()
                });
            }
            ViewBag.Cashiers = yr;

            if (string.IsNullOrEmpty(param.sSearch))
            {
                ViewData["CashCollections"] = cashreport.FirstOrDefault().Type;
                param.sSearch = cashreport.FirstOrDefault().Type;
            }
            else
            {
                ViewData["CashCollections"] = param.sSearch;
            }
            ViewBag.DailyCollectionReport = db.CashCollections.Where(u => u.Type == param.sSearch && (u.Dated >= date && u.Dated <= edate)).ToList();
            var px = db.CashCollections.Where(u => u.Type == param.sSearch && (u.Dated >= date && u.Dated <= edate)).ToList();
            if (param.sEcho == "Date")
            {
                px = px.Where(u => u.Dated >= date && u.Dated <= edate).ToList();
            }


            if (!string.IsNullOrEmpty(param.sColumns) && param.sColumns != "ALL")
            {
                //  px = px.Where(u => u.supplier == param.sColumns).ToList();
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

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               // np.dis();
            }
            base.Dispose(disposing);
        }
    }
}