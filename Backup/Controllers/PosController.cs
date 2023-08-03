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
    public class PosController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();

        //
        // GET: /Items/
        [Authorize]
        public ViewResult Index()
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            var currentUserId = User.Identity.Name;
           // string company = np.Getlogin(currentUserId);
            var user = np.Getlogin(currentUserId);
            Posdata pd = new Posdata();
            pd.menu = np.GetMenu(user.Location.Trim()).Take(12);
            return View("Index","_PosLayout",pd);
        }

        [HttpPost]
        public ActionResult Print(string  item,string tender,string change )
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
                var currentUserId = User.Identity.Name;
                var user = np.Getlogin(currentUserId);
                char[] delimiters = new char[] { '/' };
                string[] parts = item.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                int cnt = parts.Count();
                Sales sl = new Sales();
               
                List<Printdata> pds = new List<Printdata>();
                sl.company = user.Location.Trim();
                sl.dated = DateTime.Now.ToString();
                long id = np.AddSales(sl);
                decimal amt = 0;
                decimal tax = 0;
                decimal subTotal = 0;
                foreach (var itm in parts)
                {
                    char[] delimite = new char[] { ',' };
                    string[] par = itm.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

                    var it = np.GetItemCode(par[3],user.Location.Trim());
                    it.Balance = it.Balance - long.Parse(par[1]);
                    it.sold = it.sold + long.Parse(par[1]);
                    it.Amount = it.Amount + decimal.Parse(par[2]);
                    it.Expected = it.Expected - decimal.Parse(par[2]);
                    np.SaveOrUpdateItems(it);

                    Printdata pd = new Printdata();
                    SalesLines ss = new SalesLines();
                    ss.item = par[0];
                    ss.quantity = long.Parse(par[1]);
                    ss.price = decimal.Parse(par[2]) - Math.Round(decimal.Parse(par[2]) * decimal.Parse("0.130435"), 2);
                    ss.tax = Math.Round(decimal.Parse(par[2]) * decimal.Parse("0.130435"), 2);
                    ss.priceinc = decimal.Parse(par[2]);
                    ss.Reciept = user.prefix.Trim() + "-" + id;
                    amt = amt + decimal.Parse(par[2]);
                    tax = tax + Math.Round(decimal.Parse(par[2]) * decimal.Parse("0.130435"), 2);
                    subTotal = subTotal + decimal.Parse(par[2]) - Math.Round(decimal.Parse(par[2]) * decimal.Parse("0.130435"), 2);
                    //ss.Reciept 
                    np.SaveOrUpdateSalesLines(ss);

                    pd.item = ss.item;
                    pd.qty = ss.quantity.ToString();
                    pd.amount = ss.priceinc.ToString();
                    pd.prize = Math.Round((decimal)(decimal.Parse(par[2]) / ss.quantity), 2).ToString();
                    pd.unit = "ea";
                    pds.Add(pd);
                }

                var sal =np.GetSales(id);
                sal.Reciept = user.prefix.Trim() + "-" + id;
                sal.total = amt;
                sal.state = "C";
                np.SaveOrUpdateSales(sal);
                var company = np.GetCompanies(user.Location.Trim());
                ViewData["total"] = amt;
                ViewData["tax"] = tax;
                ViewData["subTotal"] = subTotal;
                ViewData["tender"] = tender;
                ViewData["change"] = change;
                
                DailySales ds = np.GetDailySales(DateTime.Now.ToString("ddMMyyyy") + company.Id);
                if (ds == null) ds = new DailySales();
                ds.Date = DateTime.Now.ToString("dd/MM/yyyy");
                ds.Sales = ds.Sales + amt;
                ds.Id = DateTime.Now.ToString("ddMMyyyy") + company.Id;
                np.SaveOrUpdateDailySales(ds);

                MonthlySales ms = np.GetMonthlySales(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + company.Id);
                if (ms == null) ms = new MonthlySales();

                ms.Id = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + company.Id;
                ms.Sales = ms.Sales + amt;
                ms.Year = DateTime.Now.Year;
                np.SaveOrUpdateMonthlySales(ms);
                return PartialView(new ReceiptVM(pds, company));
            }

            return PartialView("Index");
        }

        public ActionResult Tender()
        {
            if (Request.IsAjaxRequest())
                return PartialView("_Tender");
            return PartialView("Tender");
        }

        //
        // GET: /Items/Details/5

        public ActionResult getItems(string json_str)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            var item = np.GetItemsByCode(json_str, "PRECIOUS SUPERMARKET");
            if (item.Count() == 0)
            {
                return RedirectToAction("getMenus", new { json_str = "PRECIOUS SUPERMARKET" });
            }
            else if (item.Count() == 1)
            {
                var tt = item.FirstOrDefault();
                string returnData =  tt.Item + "," + tt.ItemCode + "," + tt.SellingPrice + "," + tt.tax;
                return Json(returnData, JsonRequestBehavior.AllowGet);
            }
            return PartialView(item);
        }

        public ActionResult getMenus(string json_str)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            var item = np.GetMenu(json_str);
            return PartialView(item);
        }

        public ActionResult Authenticate(string json_str)
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult Authenticate(string json_str, string action)
        {
                //int cnt;
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
                ST_encrypt en = new ST_encrypt();
                login px = new login();
                string resp = "";
                char[] delim = new char[] { '-' };
                string[] pr = json_str.Split(delim, StringSplitOptions.RemoveEmptyEntries);
                action = pr[1];
                // Account = en.encrypt(Account, "214ea9d5bda5277f6d1b3a3c58fd7034");
                // Pin = en.encrypt(Pin, "214ea9d5bda5277f6d1b3a3c58fd7034");
                login acc = np.GetloginPW(pr[0]);
                if (acc == null)
                {
                    resp = "Sorry Wrong Password";
                    // return px;
                }
                else
                {
                    switch (action)
                    {
                        case "voidsale":
                            if (acc.voidsale == true)
                            {
                                resp = "success";
                            }
                            else
                            {
                                resp = "You are not authorised for this";
                            }

                            break;

                        case "voidline":
                            if (acc.voidsale == true)
                            {
                                resp = "success";
                            }
                            else
                            {
                                resp = "You are not authorised for this";
                            }

                            break;

                        case "cashup":
                            if (acc.Payments == true)
                            {
                                resp = "success";
                            }
                            else
                            {
                                resp = "You are not authorised for this";
                            }

                            break;
                        default:
                            resp = "You are not authorised for this";
                            break;

                    }
                }
                
           return Json(resp, JsonRequestBehavior.AllowGet);
        }

        public ActionResult voidSale()
        {
            return PartialView();
        }
        //
        // GET: /Items/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Items/Create

        [HttpPost]
        public ActionResult Create(Items item)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
                np.SaveOrUpdateItems(item);
                return RedirectToAction("Index");  
            }

            return RedirectToAction("Index");
        }
        
        //
        // GET: /Items/Edit/5
 
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Items item = np.GetItems(id);
            return View(item);
        }

        //
        // POST: /Items/Edit/5

        [HttpPost]
        public ActionResult Edit(Items item)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
                np.SaveOrUpdateItems(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        //
        // GET: /Items/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Items item = np.GetItems(id);
            return View(item);
        }

        //
        // POST: /Items/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Items item = np.GetItems(id);
            np.DeleteItems(item); 
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               //context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}