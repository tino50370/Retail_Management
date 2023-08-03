using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.DataAcess;
using System.Text.RegularExpressions;
using System.IO;
using System.Data.Sql;

namespace RetailKing.Controllers
{
    public class CompaniesController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Companies/

        public ActionResult Index()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            return PartialView(np.GetAllCompanies());
        }

        //
        // GET: /Companies/Details/5

        public ViewResult Details(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Company company = np.GetCompaniesById(id);
            return View(company);
        }

        //
        // GET: /Companies/Create

        public ActionResult Create()
        {
            return PartialView();
        }

        //
        // POST: /Companies/Create

        [HttpPost]
        public ActionResult Create(Company company, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.ContentLength > 0)
                {
                    try
                    {
                        var fileName = Image.FileName;
                        char[] delimiter = new char[] { '.' };
                        string[] parts = fileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        var ext = parts[1];
                        string regExp = "[^a-zA-Z0-9]";
                        fileName = Regex.Replace(parts[0], regExp, "_");
                        fileName = fileName + "." + ext;
                        var tempP = Path.Combine(HttpContext.Server.MapPath("~/Content//"), "Logos");
                        var temPath = Path.Combine(HttpContext.Server.MapPath("~/Content//Logos//"), fileName);
                        company.Logo = "~/Content//Logos//" + fileName.Substring(0, fileName.Length - ext.Length) + "jpg";


                        if (System.IO.Directory.Exists(tempP))
                        {
                            Image.SaveAs(temPath);
                        }
                        else
                        {
                            Directory.CreateDirectory(tempP);
                            Image.SaveAs(temPath);
                        }
                        //convert upload to mp4
                    }
                    catch (Exception e)
                    {

                    }
                }

                NHibernateDataProvider np = new NHibernateDataProvider();
                company.name = company.name.ToUpper();
                if (company.Address1 != null) company.Address1 = company.Address1.ToUpper();
                if (company.Address2 != null) company.Address2 = company.Address2.ToUpper();
                if (company.Footer1 != null) company.Footer1 = company.Footer1.ToUpper();
                if (company.Footer2 != null) company.Footer2 = company.Footer2.ToUpper();
                if (company.Footer3 != null) company.Footer3 = company.Footer3.ToUpper();
                if (company.footer4 != null) company.footer4 = company.footer4.ToUpper();
                long Id = np.AddCompany(company);
                //   String name = company.name;
                //   String query = "Create Database " + name +";";
                //db.Database.ExecuteSqlCommand(query);

                Activeco co = new Activeco();
                co.company = company.name;
                co.CompanyId = Id;
                co.dailyTarget = 0;
                co.monthlyTarget = 0;
                np.SaveOrUpdateActiveco(co);

                var CurrentUser = User.Identity.Name;
                char[] delimiterr = new char[] { '~' };
                string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

                var user = np.Getlogin(partz[1]);
                var syk = db.Syskeys.Where(u => u.Company == user.Location).ToList();

                foreach (var sy in syk)
                {
                    if (sy.Name != "OnlineStore")
                    {
                        Syskey sys = new Syskey();

                        sys.Name = sy.Name;
                        sys.Value = sy.Value;
                        sys.Module = sy.Module;
                        sys.Description = sy.Description;
                        sys.Company = company.name;

                        db.Syskeys.Add(sys);
                        db.SaveChanges();
                    }
                }

                // creating currencies for new componey

                var currentComp = partz[2];

                var curr = (from e in np.GetAllCurrencies()
                            where e.Company.Trim() == currentComp
                            select e).ToList();

                foreach (var comp in curr)
                {

                    Currency cc = new Currency();
                    cc.Curency = comp.Curency;
                    cc.Description = comp.Description;
                    cc.ExchangeRate = comp.ExchangeRate;
                    cc.IsBase = comp.IsBase;
                    cc.Company = company.name;
                    np.SaveOrUpdateCurrencies(cc);

                }
                
                var car = db.Currencies.Where(u => u.Company == company.name).ToList();
                
                var acc = (from e in np.GetAllAccounts()
                           where e.CompanyId == 1
                           select e).ToList();
                foreach (var item in acc)
                {
                    Currency carr = new Currency();
                    if (item.Currency == null)
                    {
                        carr = car.Where(u => u.IsBase == true).FirstOrDefault();
                    }
                    else
                    {
                        carr = car.Where(u => u.Curency == item.Currency).FirstOrDefault();
                    }
                    Account ac = new Account();
                    ac.ID = 0;
                    ac.Opening = 0;
                    ac.AccountName = item.AccountName;
                    ac.AccountCode = item.AccountCode;
                    ac.CompanyId = Id;
                    ac.CurrencyId = carr.Id;
                    ac.Currency = carr.Curency;
                    ac.Balance = 0;
                    ac.LinkAccount = item.LinkAccount;

                    np.SaveOrUpdateAccounts(ac);
                }

                // creating items for new company
                var itms = db.Items.Where(u => u.company == user.Location).ToList();
                var ps = db.PosKeys.Where(u => u.Company == user.Location).ToList();

                foreach (var poskey in ps)
                {

                    PosKey pos = new PosKey();

                    pos.Company = user.Location;
                    pos.CustomerNameForCash = poskey.CustomerNameForCash;
                    pos.DoDeliveries = poskey.DoDeliveries;
                    pos.FiscalPrinter = poskey.FiscalPrinter;
                    pos.KitchenPrinter = poskey.KitchenPrinter;
                    pos.KitchenPrinting = poskey.KitchenPrinting;
                    pos.OrderBased = poskey.OrderBased;


                    db.PosKeys.Add(pos);

                    db.SaveChanges();


                }
                foreach (var item in itms)
                {
                    Item itm = new Item();
                    itm.ItemName = item.ItemName;
                    itm.Quantity = 0;
                    itm.BuyingPrice = item.BuyingPrice;
                    itm.SellingPrice = item.SellingPrice;
                    itm.Expected = item.Expected;
                    itm.sold = 0;
                    itm.transfer = item.transfer;
                    itm.Balance = 0;
                    itm.Amount = item.Amount;
                    itm.NewStock = 0;
                    itm.Returned = item.Returned;
                    itm.company = company.name;
                    itm.category = item.category;
                    itm.SubCategory = item.SubCategory;
                    itm.Swaps = item.Swaps;
                    itm.Reorder = item.Reorder;
                    itm.status = item.status;
                    itm.ItemCode = item.ItemCode;
                    itm.tax = item.tax;
                    itm.TopSeller = item.TopSeller;
                    itm.SDate = item.SDate;
                    itm.EDate = item.EDate;
                    itm.Measure = item.Measure;
                    itm.Image = item.Image;
                    itm.ThumbImage = item.ThumbImage;
                    itm.DicountType = item.DicountType;
                    itm.Discount = item.Discount;
                    itm.Promotion = item.Promotion;
                    itm.Manufacturer = item.Manufacturer;
                    itm.Title = item.Title;
                    itm.Description = item.Description;
                    itm.AdPosition = item.AdPosition;
                    itm.PromoImage = item.PromoImage;
                    itm.Featured = item.Featured;
                    itm.ItemDescription = item.ItemDescription;
                    itm.ItmDescription = item.ItmDescription;
                    itm.IsBulk = item.IsBulk;
                    itm.Supplier = item.Supplier;
                    itm.HasRecipe = item.HasRecipe;
                    itm.NotForSale = item.NotForSale;

                    db.Items.Add(itm);

                    db.SaveChanges();
                }


                return RedirectToAction("Index", new { Customer = company.name });
            }

            return View(company);
        }

        //
        // GET: /Companies/Edit/5

        public ActionResult Edit(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Company company = np.GetCompaniesById(id);
            return PartialView(company);
        }

        //
        // POST: /Companies/Edit/5

        [HttpPost]
        public ActionResult Edit(Company company, HttpPostedFileBase Image)
        {
            if (ModelState.IsValid)
            {
                if (Image != null && Image.ContentLength > 0)
                {
                    try
                    {
                        var fileName = Image.FileName;
                        char[] delimiter = new char[] { '.' };
                        string[] parts = fileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        var ext = parts[1];
                        string regExp = "[^a-zA-Z0-9]";
                        fileName = Regex.Replace(parts[0], regExp, "_");
                        fileName = fileName + "." + ext;
                        var tempP = Path.Combine(HttpContext.Server.MapPath("~/Content//"), "Logos");
                        var temPath = Path.Combine(HttpContext.Server.MapPath("~/Content//Logos//"), fileName);
                        company.Logo = "~/Content//Logos//" + fileName.Substring(0, fileName.Length - ext.Length) + "jpg";

                        if (System.IO.Directory.Exists(tempP))
                        {
                            Image.SaveAs(temPath);
                        }
                        else
                        {
                            Directory.CreateDirectory(tempP);
                            Image.SaveAs(temPath);
                        }
                        //convert upload to mp4
                    }
                    catch (Exception e)
                    {

                    }
                }

                NHibernateDataProvider np = new NHibernateDataProvider();
                company.name = company.name.ToUpper();
                if (company.Address1 != null) company.Address1 = company.Address1.ToUpper();
                if (company.Address2 != null) company.Address2 = company.Address2.ToUpper();
                if (company.Footer1 != null) company.Footer1 = company.Footer1.ToUpper();
                if (company.Footer2 != null) company.Footer2 = company.Footer2.ToUpper();
                if (company.Footer3 != null) company.Footer3 = company.Footer3.ToUpper();
                if (company.footer4 != null) company.footer4 = company.footer4.ToUpper();
                np.SaveOrUpdateCompanies(company);

                return RedirectToAction("Index", new { Customer = company.name });
            }
            return PartialView(company);
        }

        //
        // GET: /Company/Delete/5

        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();

            Company company = np.GetCompaniesById(id);
            return PartialView(company);
        }

        //
        // POST: /Company/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Company company = np.GetCompaniesById(id);
            np.DeleteCompanies(company);

            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                //context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}