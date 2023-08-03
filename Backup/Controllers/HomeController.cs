using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using System.Reflection;
using RetailKing.DataAcess;
using RetailKing.ViewModels;

namespace RetailKing.Controllers
{
    public class HomeController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        [Authorize]
        public ActionResult Index()
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            
            var co = np.GetAllCompanies().FirstOrDefault();
            string ADate = DateTime.Now.Date.ToString("ddMMyyyy") + co.Id;
            string nezuro = DateTime.Now.AddDays(-1).ToString("ddMMyyyy") + co.Id;
            var month = DateTime.Now.Month;
            var lastmonth = month - 1;
            var ds = np.GetDailySales(ADate);
            if (ds != null)
            {
                ViewData["Nhasi"] = ds.Sales;
            }
            else
            {
                ViewData["Nhasi"] = 0;
            }
            var dd = np.GetDailySales(nezuro);
            var ts =np.GetMonthlySales(DateTime.Now.Year.ToString() + month.ToString() + co.Id.ToString() );
            if (ts != null)
            {
                ViewData["ThisMonth"] = ts.Sales;
            }
            else
            {
                ViewData["ThisMonth"] = 0;
            }
            var yy = np.GetMonthlySales(DateTime.Now.Year.ToString() + lastmonth.ToString() + co.Id.ToString());
            var cos = np.GetAllActiveco();
            var ccmp = cos.FirstOrDefault();
            ViewData["listSize"] = 10;
            return View(new HomeVM(np.GetAllItems(), dd, yy, ccmp,cos));
        }
  
    }
}
