
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using System.Reflection;
using RetailKing.DataAcess;
using RetailKing.ViewModels;
using System.Web.Security;

namespace RetailKing.Controllers
{
    public class RetailKingController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();

        public ActionResult Index(string Company)
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();

                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] partz = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (partz.Length != 4 && partz.Length != 5) return RedirectToAction("Index", "Login");
                var uza = np.Getlogin(partz[1]);
                Company co = new Company();
                Activeco  ccmp = new Activeco();
                if (Company == null || Company == "")
                {
                    if (uza.MultiCompanyAccess == true)
                    {
                        co = np.GetAllCompanies().FirstOrDefault();
                   
                    }
                    else
                    {
                        if(partz.Length == 4)
                        {
                            co = np.GetCompanies(partz[2]);
                        }
                        else
                        {
                            co = np.GetCompanies(partz[1]);
                        }
                        
                    }

                    Company = co.name;
                }
                else
                {
                    if (uza.MultiCompanyAccess == true)
                    {
                        co = np.GetCompanies(Company);
                    }
                    else
                    {
                        if (partz.Length == 4)
                        {
                            co = np.GetCompanies(partz[2]);
                        }
                        else
                        {
                            co = np.GetCompanies(partz[1]);
                        }
                    }
                }
                var Page = db.Syskeys.Where(u => u.Name == "DashBoard" || u.Name == "Dashboard").FirstOrDefault();
                if(!string.IsNullOrEmpty(Page.Value))
                {
                    return Redirect(Page.Value.Trim());
                }
                string ADate = DateTime.Now.Date.ToString("ddMMyyyy") +co.ID;
                string nezuro = DateTime.Now.AddDays(-1).ToString("ddMMyyyy") +co.ID;
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
                var ts =np.GetMonthlySales(DateTime.Now.Year.ToString() + month.ToString() +co.ID.ToString() );
                if (ts != null)
                {
                    ViewData["ThisMonth"] = ts.Sales;
                }
                else
                {
                    ViewData["ThisMonth"] = 0;
                }
                var yy = np.GetMonthlySales(DateTime.Now.Year.ToString() + lastmonth.ToString() +co.ID.ToString());
                var cos = np.GetAllActiveco();

           
                ViewData["listSize"] = 10;
                if (Company == null || Company == "")
                {
                    ViewData["ActiveCo"] = cos.FirstOrDefault().company;
                    ccmp = cos.FirstOrDefault();
                }
                else
                {
                    ViewData["ActiveCo"] = Company;
                    ccmp = (from e in  cos
                            where e.company == Company 
                            select e).FirstOrDefault();
                }
                ViewData["ActiveUser"] = partz[0];

               if (Request.IsAjaxRequest()) return PartialView(new HomeVM(np.GetItemsByCompany(co.name), dd, yy, ccmp, cos, uza));
           
           
               return View("Index", "_AdminLayout", new HomeVM(np.GetItemsByCompany(co.name), dd, yy, ccmp, cos, uza));
               
            }
            else
            {
                return RedirectToAction("Index", "Login");
            }
        }

        [Authorize]
        public ActionResult TopMenu()
        {
            
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            var uza = np.Getlogin(CurrentUser);

            return PartialView(uza);      
        }

        public ActionResult SideMenu(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(); 
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            ViewData["Location"] = partz[1];
            ViewData["Access"] = partz[2];
            Company co = new Company();
            Activeco ccmp = new Activeco();
            IList<Item> itms = null;
            MonthlySale yy = null;
            DailySale dd = null; 

            var cos = np.GetAllActiveco();
            var uza = np.Getlogin(CurrentUser);
            return PartialView(new HomeVM(itms, dd, yy, ccmp, cos, uza));
        }

        public ActionResult ActiveCo(string Company){
            NHibernateDataProvider np = new NHibernateDataProvider(); 
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~'};
            string[] partz = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (Company == null || Company == "")
            {

                Company = partz[1];
               
            }
          
                ViewData["ActiveCo"] = Company;
           

            return Json(Company, JsonRequestBehavior.AllowGet);
        }
  
    }
}
