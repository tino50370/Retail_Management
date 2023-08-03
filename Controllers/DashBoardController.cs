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
using RetailKing.RavendbClasses;

namespace RetailKing.Controllers
{   
    public class DashboardController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        XmlSupplier sup = new XmlSupplier();
        XmlTransactionLogs trnl = new XmlTransactionLogs();

        //
        // GET: /Accounts/

        public ActionResult Index(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllAccounts();
            var comp = np.GetActivecoByName(Company);
            px= (from e in px 
                     where e.CompanyId == comp.CompanyId
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
        public ActionResult Index(string Company, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            //JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllAccounts();
            var comp = np.GetActivecoByName(Company);
            px = (from e in px
                  where e.CompanyId == comp.CompanyId
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

        public ActionResult WeeklyStatsGraph()
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var id = part[0] + "_" + DateTime.Now.Month;
            List<Stats> stats = new List<Stats>();
            List<GraphData> gdatas = new List<GraphData>();
            List<string> keys = new List<string>();

            var px = trnl.GetStats(id);
            ViewData["Graph"] = "";
            if (px != null)
            {

                stats = px.stats.OrderByDescending(u => u.Date).ToList();
                var gx = stats.GroupBy(u => u.Date).Take(5).ToList();

                foreach (var it in gx)
                {
                    GraphData gd = new GraphData();
                    foreach (var item in it)
                    {
                        if (!keys.Contains(item.ServiceName.ToUpper()))
                        {
                            keys.Add(item.ServiceName.ToUpper());
                        }
                        gd.Period = item.Date.Date.ToString("dd/MM/yyyy");
                       
                    }
                    gdatas.Add(gd);
                }
                // var graph = (from e in stats select new { service = e.ServiceName.Trim(), hits = e.Successful}).ToArray();
                var rx = gdatas.OrderBy(u => u.Period).ToList();

                var x = JsonConvert.SerializeObject(rx);
                ViewBag.data = x;
            }
           return PartialView();
        }

        public ActionResult WeeklySmsGraph()
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var id = part[0] + "_" + DateTime.Now.Month;
            if (part.Length == 3)
            {
                id = "5-0001-0000000" + "_" + DateTime.Now.Month;
            }
           
            List<Stats> stats = new List<Stats>();
            List<SingleStats> gdatas = new List<SingleStats>();
            List<string> keys = new List<string>();

            var px = trnl.GetStats(id);
            ViewData["Graph"] = "";
            if (px != null)
            {

                stats = px.stats.OrderByDescending(u => u.Date).ToList();
                var gx = stats.Where(u => u.ServiceName.ToUpper() == "SMS MESSAGE").Take(5).ToList();

                foreach (var it in gx)
                {
                   
                    SingleStats  gd = new SingleStats();
                    gd.Text = it.Date.Date.ToString("dd/MM/yyyy");
                   // gd.Text = it.ServiceName.Trim();
                    gd.Value = it.Successful;
                    gdatas.Add(gd);
                }
                // var graph = (from e in stats select new { service = e.ServiceName.Trim(), hits = e.Successful}).ToArray();
                var rx = gdatas.OrderBy(u => u.Text).ToList();

                var x = JsonConvert.SerializeObject(rx);
                ViewBag.data = x;
            }
            return PartialView();
        }

        public ActionResult WeeklyServiceGraph( string service)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var id = part[0] + "_" + DateTime.Now.Month;
            List<Stats> stats = new List<Stats>();
            List<SingleStats> gdatas = new List<SingleStats>();
            List<string> keys = new List<string>();

            var px = trnl.GetStats(id);
            ViewData["Graph"] = "";
            if (px != null)
            {
                stats = px.stats.OrderByDescending(u => u.Date).ToList();
                var gx = stats.Where(u => u.ServiceName.ToUpper() == service).Take(5).ToList();

                foreach (var it in gx)
                {
                    SingleStats gd = new SingleStats();
                    gd.Text = it.ServiceName.Trim();
                    gd.Value = it.Successful;
                    gdatas.Add(gd);
                }
                // var graph = (from e in stats select new { service = e.ServiceName.Trim(), hits = e.Successful}).ToArray();
                var rx = gdatas.OrderBy(u => u.Text).ToList();

                var x = JsonConvert.SerializeObject(rx);
                ViewBag.data = x;
            }
            return PartialView();
        }

        public ActionResult DailyStatsGraph()
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var id = part[0] + "_" + DateTime.Now.Month;
            List<Stats> stats = new List<Stats>();
            List<GraphData> gdatas = new List<GraphData>();
            List<string> keys = new List<string>();

            var px = trnl.GetStats(id);
            ViewData["Graph"] = "";
            if (px != null)
            {

                stats = px.stats.OrderByDescending(u => u.Date).ToList();
                var gx = stats.GroupBy(u => u.Date).Take(5).ToList();

                foreach (var it in gx)
                {
                    GraphData gd = new GraphData();
                    foreach (var item in it)
                    {
                        if (!keys.Contains(item.ServiceName.ToUpper()))
                        {
                            keys.Add(item.ServiceName.ToUpper());
                        }
                        gd.Period = item.Date.Date.ToString("dd/MM/yyyy");
                        
                    }
                    gdatas.Add(gd);
                }
                // var graph = (from e in stats select new { service = e.ServiceName.Trim(), hits = e.Successful}).ToArray();
                var rx = gdatas.OrderBy(u => u.Period).ToList();

                var x = JsonConvert.SerializeObject(rx);
                ViewBag.data = x;
            }
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