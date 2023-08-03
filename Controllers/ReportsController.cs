using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.DataAcess;

namespace RetailKing.Controllers
{   
    public class ReportsController : Controller
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

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               // np.dis();
            }
            base.Dispose(disposing);
        }
    }
}