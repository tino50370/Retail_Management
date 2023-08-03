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
using System.Text.RegularExpressions;

namespace RetailKing.Controllers
{
    public class HomeController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        public ActionResult Index(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUser = User.Identity.Name;
            var key = db.Syskeys.Where(u => u.Name == "HomePage").FirstOrDefault();
            if (!Request.IsAuthenticated)
            {
                ViewData["Sessionid"] = Session.SessionID;
            }
            if(Request.IsAuthenticated)
            {
                char[] delimiter = new char[] { '~' };
                string[] partz = currentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (partz.Length == 4 || partz.Length == 5) 
                { 
                    return RedirectToAction("Index","RetailKing", new {Company = Company}); 
                }
                else if(partz.Length == 2)
                {
                    if (key.Value == "STORE")
                    {
                        return RedirectToAction("AccLogout", "Accounts"); 
                    }
                    else if (key.Value == "POS")
                    {
                        return RedirectToAction("Index", "Pos");
                    }

                }
                else
                {
                    return RedirectToAction("RetailLogOff", "Login");
                }
            }

            var co = np.GetAllCompanies().FirstOrDefault();
            ViewData["Company"] = co.name.Trim();
            if (currentUser == null || currentUser == "") // visit Counter
            {
                DailySale ds = np.GetDailySales(DateTime.Now.ToString("ddMMyyyy") +co.ID);
                MonthlySale ms = np.GetMonthlySales(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + 1);
                if (ds == null)
                {
                    ds = new DailySale();
                    ds.Id = DateTime.Now.ToString("ddMMyyyy") + co.ID;

                    if (ds.UniqueVisits == null) ds.UniqueVisits = 0;
                    if (ds.Hits == null) ds.Hits = 0;
                    ds.Date = DateTime.Now.ToString("dd/MM/yyyy");
                    ds.UniqueVisits = ds.UniqueVisits + 1;
                    ds.Hits = ds.Hits + 1;
                    np.AddDailySales(ds);
                   
                }
                else
                {
                    if (ds.UniqueVisits == null) ds.UniqueVisits = 0;
                    if (ds.Hits == null) ds.Hits = 0;
                    ds.Date = DateTime.Now.ToString("dd/MM/yyyy");
                    ds.UniqueVisits = ds.UniqueVisits + 1;
                    ds.Hits = ds.Hits + 1;
                    np.SaveOrUpdateDailySales(ds); 
                }
                if (ms == null)
                {
                    ms = new MonthlySale();
                    ms.Id = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + 1;
                    if (ms.UniqueVisits == null) ms.UniqueVisits = 0;
                    if (ms.Hits == null) ms.Hits = 0;
                    ms.UniqueVisits = ms.UniqueVisits + 1;
                    ms.Hits = ms.Hits + 1;
                    ms.Year = DateTime.Now.Year;
                    np.AddMonthlySales(ms);     
                }
                else
                {
                    if (ms.UniqueVisits == null) ms.UniqueVisits = 0;
                    if (ms.Hits == null) ms.Hits = 0;
                    ms.UniqueVisits = ms.UniqueVisits + 1;
                    ms.Hits = ms.Hits + 1;
                    ms.Year = DateTime.Now.Year;
                    np.SaveOrUpdateMonthlySales(ms);
                }
                var id = Session.SessionID;
                bool isMobile = false;
                string um = Request.ServerVariables["HTTP_USER_AGENT"];
                Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
                if ((b.IsMatch(um) || v.IsMatch(um.Substring(0, 4)) || Request.Browser.IsMobileDevice))
                {
                    isMobile = true;
                }

                var ana = new Analytics();
                ana.logViews("Home", isMobile);
           
               
              //  FormsAuthentication.SetAuthCookie("Guest", false);
            }
            if (key.Value.Trim() == "ADMIN")
            {
                return RedirectToAction("Index","RetailKing");
            }
            else if (key.Value.Trim() == "POS")
            {
                return RedirectToAction("Index", "Pos");
            }
            else
            {
                var sk = db.Syskeys.Where(u => u.Name == "OnlineStore").FirstOrDefault();
                var itm = db.Items.Where(u => u.company == sk.Value.Trim() && (u.NotForSale == null || u.NotForSale != true)).OrderByDescending(u=>u.Balance).ToList();
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                var partnerz = db.Partners.Take(10).ToList();
                HomeData hd = new HomeData();
                hd.items = itm;
                hd.Categories = categories;
                hd.Partners = partnerz;
                hd.storekeys = db.Storekeys.Where(u => u.Company == sk.Value).ToList();
                //if (Request.IsAjaxRequest()) return PartialView(hd);
                return View("Index","_Layout",hd);
            }
        }
        [HttpGet]
        public ActionResult CategoryCorusel(JQueryDataTableParamModel param, int currpage = 0)
        {
            var px = new List<Item>();
            var main_px = new List<Item>();
            string Id = "";
            var categories = new List<Account>();
            var sk = db.Syskeys.Where(u => u.Name == "OnlineStore").FirstOrDefault();
            var comp = db.Companies.Where(u => u.name.Trim() == sk.Value.Trim()).FirstOrDefault();
           
            while (main_px.Count() == 0)
            {

                categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3-") && u.AccountCode.Length == 4 && u.CompanyId == comp.ID).ToList();
                var current = categories.Skip(currpage).FirstOrDefault();
                if (current == null) return null;
                Id = current.AccountName.Trim();
                var c = categories.Where(u => u.AccountName.Trim() == Id).FirstOrDefault();

                main_px = db.Items.Where(u => u.category == Id && u.company.Trim() == sk.Value.Trim() && (u.NotForSale == null || u.NotForSale != true)).ToList();
                currpage += 1;
            }

            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 6;
            }
            Random r = new Random();
            int xskip = 0;

            if(main_px.Count() > 6)
            {
              xskip = r.Next(0, main_px.Count() - 6);
            }
            var nn = (from emp in main_px.Skip(xskip).Take(6)
                      select emp).ToList();

            int pages = main_px.Count / param.iDisplayLength;
            if (main_px.Count > 0 && pages == 0)
            {
                pages = 1;
            }
            else if (main_px.Count > 0 && main_px.Count % param.iDisplayLength > 0)
            {
                pages += 1;
            }
            int page = param.iDisplayStart;

            int start = (param.iDisplayStart - 1) * param.iDisplayLength;
            int end = (param.iDisplayStart) * param.iDisplayLength;
            if (start == 0) start = 1;
            int column = param.iDisplayStart / 5;
            ViewData["Category"] = Id;
            ViewData["listSize"] = categories.Count();
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = currpage;
            ViewData["Columns"] = column + 1;
            ViewData["RecordData"] = "Showing " + start + " to " + end + " of " + main_px.Count;
            ViewData["last"] = "false";
            if (page == pages) ViewData["last"] = "true";
            var partnerz = db.Partners.Take(10).ToList();
            HomeData hd = new HomeData();
            hd.items = nn;
            hd.Categories = categories;
            hd.Partners = partnerz;
            hd.storekeys = db.Storekeys.Where(u => u.Company.Trim() == sk.Value.Trim()).ToList();
            if (Request.IsAjaxRequest()) return PartialView(hd);
            return View(hd);
        }

        [HttpGet]
        public ActionResult PromotionCorusel(JQueryDataTableParamModel param, int currpage = 0)
        {

            var px = new List<Advert>();
            var main_px = new List<Advert>();
            string Id = "";
            if (param.sColumns == null)
            {
                main_px = (from e in db.Adverts.Where(u => u.AdPosition.Trim() == "Banner") select e).Take(5).ToList();
            }
            else
            {
                main_px = (from e in db.Adverts.Where(u => u.AdPosition.Trim() == param.sColumns) select e).Take(5).ToList();
            }
            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 6;
            }
            Random r = new Random();
            int xskip = 0;

            if (main_px.Count() > 6)
            {
                xskip = r.Next(0, main_px.Count() - 6);
            }
            var nn = (from emp in main_px.Skip(xskip).Take(6)
                      select emp).ToList();

            int pages = main_px.Count / param.iDisplayLength;
            if (main_px.Count > 0 && pages == 0)
            {
                pages = 1;
            }
            else if (main_px.Count > 0 && main_px.Count % param.iDisplayLength > 0)
            {
                pages += 1;
            }
            int page = param.iDisplayStart;

            int start = (param.iDisplayStart - 1) * param.iDisplayLength;
            int end = (param.iDisplayStart) * param.iDisplayLength;
            if (start == 0) start = 1;
            int column = param.iDisplayStart / 5;
            ViewData["Category"] = Id;
            ViewData["listSize"] = main_px.Count();
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = currpage;
            ViewData["Columns"] = column + 1;
            ViewData["RecordData"] = "Showing " + start + " to " + end + " of " + main_px.Count;
            ViewData["last"] = "false";
            if (page == pages) ViewData["last"] = "true";
            var partnerz = db.Partners.Take(10).ToList();
            HomeData hd = new HomeData();
            if (param.sColumns == "SIDE")
            {
                
                if (Request.IsAjaxRequest()) return PartialView("SidePromotion", nn);
                return View("SidePromotion", nn);
            }
            else
            {
                if (Request.IsAjaxRequest()) return PartialView(nn);
                return View(nn);
            }
        }

        public ActionResult eShopMenu(string SessionId)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3-") && u.CompanyId == 1).ToList();
            ViewData["Session"] = SessionId;
            return PartialView(categories);
        }
        [HttpPost]
        public ActionResult Indexsearch(string company, string ItemCode, string category)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
          
            var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3-") && u.CompanyId == 1).ToList();
            var sk = db.Syskeys.Where(u => u.Name == "OnlineStore").FirstOrDefault();

            var px = db.Items.Where(u => u.company.Trim() == sk.Value.Trim()).ToList();
            if (!string.IsNullOrEmpty(ItemCode))
            {
                px = px.Where(u =>u.ItemCode.ToLower().Contains(ItemCode.Trim().ToLower()) || u.ItemName.ToLower().Contains(ItemCode.Trim().ToLower()))
                      .ToList();
            }
            return PartialView(px);
        }

        public ActionResult About()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            bool isMobile = false;
            string um = Request.ServerVariables["HTTP_USER_AGENT"];
            Regex b = new Regex(@"(android|bb\d+|meego).+mobile|avantgo|bada\/|blackberry|blazer|compal|elaine|fennec|hiptop|iemobile|ip(hone|od)|iris|kindle|lge |maemo|midp|mmp|mobile.+firefox|netfront|opera m(ob|in)i|palm( os)?|phone|p(ixi|re)\/|plucker|pocket|psp|series(4|6)0|symbian|treo|up\.(browser|link)|vodafone|wap|windows (ce|phone)|xda|xiino", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            Regex v = new Regex(@"1207|6310|6590|3gso|4thp|50[1-6]i|770s|802s|a wa|abac|ac(er|oo|s\-)|ai(ko|rn)|al(av|ca|co)|amoi|an(ex|ny|yw)|aptu|ar(ch|go)|as(te|us)|attw|au(di|\-m|r |s )|avan|be(ck|ll|nq)|bi(lb|rd)|bl(ac|az)|br(e|v)w|bumb|bw\-(n|u)|c55\/|capi|ccwa|cdm\-|cell|chtm|cldc|cmd\-|co(mp|nd)|craw|da(it|ll|ng)|dbte|dc\-s|devi|dica|dmob|do(c|p)o|ds(12|\-d)|el(49|ai)|em(l2|ul)|er(ic|k0)|esl8|ez([4-7]0|os|wa|ze)|fetc|fly(\-|_)|g1 u|g560|gene|gf\-5|g\-mo|go(\.w|od)|gr(ad|un)|haie|hcit|hd\-(m|p|t)|hei\-|hi(pt|ta)|hp( i|ip)|hs\-c|ht(c(\-| |_|a|g|p|s|t)|tp)|hu(aw|tc)|i\-(20|go|ma)|i230|iac( |\-|\/)|ibro|idea|ig01|ikom|im1k|inno|ipaq|iris|ja(t|v)a|jbro|jemu|jigs|kddi|keji|kgt( |\/)|klon|kpt |kwc\-|kyo(c|k)|le(no|xi)|lg( g|\/(k|l|u)|50|54|\-[a-w])|libw|lynx|m1\-w|m3ga|m50\/|ma(te|ui|xo)|mc(01|21|ca)|m\-cr|me(rc|ri)|mi(o8|oa|ts)|mmef|mo(01|02|bi|de|do|t(\-| |o|v)|zz)|mt(50|p1|v )|mwbp|mywa|n10[0-2]|n20[2-3]|n30(0|2)|n50(0|2|5)|n7(0(0|1)|10)|ne((c|m)\-|on|tf|wf|wg|wt)|nok(6|i)|nzph|o2im|op(ti|wv)|oran|owg1|p800|pan(a|d|t)|pdxg|pg(13|\-([1-8]|c))|phil|pire|pl(ay|uc)|pn\-2|po(ck|rt|se)|prox|psio|pt\-g|qa\-a|qc(07|12|21|32|60|\-[2-7]|i\-)|qtek|r380|r600|raks|rim9|ro(ve|zo)|s55\/|sa(ge|ma|mm|ms|ny|va)|sc(01|h\-|oo|p\-)|sdk\/|se(c(\-|0|1)|47|mc|nd|ri)|sgh\-|shar|sie(\-|m)|sk\-0|sl(45|id)|sm(al|ar|b3|it|t5)|so(ft|ny)|sp(01|h\-|v\-|v )|sy(01|mb)|t2(18|50)|t6(00|10|18)|ta(gt|lk)|tcl\-|tdg\-|tel(i|m)|tim\-|t\-mo|to(pl|sh)|ts(70|m\-|m3|m5)|tx\-9|up(\.b|g1|si)|utst|v400|v750|veri|vi(rg|te)|vk(40|5[0-3]|\-v)|vm40|voda|vulc|vx(52|53|60|61|70|80|81|83|85|98)|w3c(\-| )|webc|whit|wi(g |nc|nw)|wmlb|wonu|x700|yas\-|your|zeto|zte\-", RegexOptions.IgnoreCase | RegexOptions.Multiline);
            if ((b.IsMatch(um) || v.IsMatch(um.Substring(0, 4)) || Request.Browser.IsMobileDevice))
            {
                isMobile = true;
            }

            var ana = new Analytics();
            ana.logViews("_About", isMobile);
            var categories = db.Infographics.ToList();

            return View(categories);
        }
        
        public ActionResult Faq()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            var categories = db.Faqs.Where(u => u.FaqType.Trim() != "Privacy").ToList();

            return View(categories);
        }

        public ActionResult Privacy()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            var categories = db.Faqs.Where(u => u.FaqType.Trim() == "Privacy").ToList();

            return View(categories);
        }

        public ActionResult LevelActivation()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3-") && u.CompanyId == 1).ToList();

            return View(categories);
        }

        public ActionResult Recruiting()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3-") && u.CompanyId == 1).ToList();

            return View(categories);
        }

        [Authorize]
        public ActionResult RetailKing(string Company)
        {
            // company change for mulicompany use
            NHibernateDataProvider np = new NHibernateDataProvider();

            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if(partz.Length !=4) return RedirectToAction("Index","Home");
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
                    co = np.GetCompanies(partz[1]); 
                }
                Company = co.name;
            }
            else
            {
               if (uza.MultiCompanyAccess == true)
                {
                    var authTicket = new FormsAuthenticationTicket(
                                        1,
                                        "5-0001-0000000" + "~" + uza.username + "~" + Company + "~" + uza.accesslevel,  //user id
                                        DateTime.Now,
                                        DateTime.Now.AddMinutes(120),  // expiry
                                        true,  //true to rememberme
                                        uza.accesslevel, //roles 
                                        "/"
                                        );
                    HttpCookie cookies = new HttpCookie(FormsAuthentication.FormsCookieName, FormsAuthentication.Encrypt(authTicket));
                    Response.Cookies.Add(cookies);
                    co = np.GetCompanies(Company);
                }
                else
                {
                    co = np.GetCompanies(partz[1]);
                }
            }
            var Page = db.Syskeys.Where(u => u.Name == "DashBoard").FirstOrDefault();
            if (!string.IsNullOrEmpty(Page.Value))
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
           
            return View("RetailKing","_AdminLayout",new HomeVM(np.GetItemsByCompany(co.name), dd, yy, ccmp, cos,uza));
        }

        public ActionResult TopMenu()
        {
            
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            ViewData["Location"] = partz[1];
            ViewData["Access"] = partz[2];
            var uza = np.Getlogin(partz[0]);

            return PartialView(uza);      
        }

        public ActionResult SideMenu(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(); 
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if(partz.Length != 4)
            {
                return RedirectToAction("LogOff", "Login");
            }
            ViewData["Location"] = partz[2];
            ViewData["Access"] = partz[3];
            Company co = new Company();
            Activeco ccmp = new Activeco();
            IList<Item> itms = null;
            MonthlySale yy = null;
            DailySale dd = null; 

            var cos = np.GetAllActiveco();
            var uza = np.Getlogin(partz[1]);
            if (partz.Length == 5)
            {
                return PartialView(new HomeVM(itms, dd, yy, ccmp, cos, uza));
            }
            else
            {
                return PartialView("AdminMenu",new HomeVM(itms, dd, yy, ccmp, cos, uza));
            }
        }

        public ActionResult ActiveCo(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(); 
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~'};
            string[] partz = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (Company == null || Company == "")
            {
                if (partz.Length == 4)
                {
                    Company = partz[2];
                }else
                {
                    string acc = partz[0];
                    var s =db.Suppliers.Where(u => u.AccountCode == acc).FirstOrDefault();
                    if(s!= null && s.Balance > 0)
                    {
                        Company = partz[1] + " Balance: $" + String.Format("{0:n}", s.Balance)  ;
                    }
                    else
                    {
                        Company = partz[1];
                    }
                }
               
            }
          
                ViewData["ActiveCo"] = Company;
           

            return Json(Company, JsonRequestBehavior.AllowGet);
        }
  
    }
}
