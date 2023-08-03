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
using System.IO;
using System.Reflection;
using System.Web.SessionState;
using System.Web.Routing;
using Newtonsoft.Json;
using RetailKing.ChatServer;
using System.Net;

namespace RetailKing.Controllers
{   
   
    public class PosController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Items/

         public ActionResult Index()
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var usar = np.Getlogin(part[0]);
                var currency = db.Currencies.Where(u => u.Company == usar.Location).ToList();
                List<SelectListItem> sb = new List<SelectListItem>();
                sb.Add(new SelectListItem
                {
                    Text = "-Select Currency-",
                    Value = "-Select Currency-"
                });
                foreach (var item in currency)
                {
                    sb.Add(new SelectListItem
                    {
                        Value = item.Curency +"~"+ item.ExchangeRate.ToString(),
                        Text = item.Curency + "~" + item.ExchangeRate.ToString()
                    });
                }

                ViewBag.CURR = sb;


                Posdata pd = new Posdata();
                pd.menu = np.GetMenu(part[1]).Take(12);
                pd.uzar = usar;
               // pd.curr = currency;
                return View("Index", "_PosLayout", pd);
            }
            else
            
{
                return RedirectToAction("PosLogin","Login");
            }

        }
        public ActionResult Voucher(decimal amount =0)
        {
            YomoneyController dt = new YomoneyController();
            List<YomoneyResponse> tr = new List<YomoneyResponse>();
            
            YomoneyRequest yr = new YomoneyRequest();
            var Store = db.Syskeys.Where(u => u.Name == "OnlineStore").FirstOrDefault();
            var agent = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "YomoneyAgentCode").FirstOrDefault().Value;
            var password = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "AgentPassword").FirstOrDefault().Value;

            yr.MTI = "0300";
            yr.Amount = (decimal)amount;
            yr.ProcessingCode = "420000";
            yr.TransactionType = 3;
            yr.AgentCode = agent.Trim() + ":" + password.Trim();
            //yr.CustomerMSISDN = resp.CustomerMSISDN;
            var list = dt.Payment(yr);
            if (list.Narrative != null)
            {
                tr = JsonConvert.DeserializeObject<List<YomoneyResponse>>(list.Narrative);
            }
            else
            {
                var jlist = JsonConvert.SerializeObject(list);
                return PartialView("EcoMsg", jlist);
            }

            //ViewData["Amount"] = amount;
            //ViewData["type"] = type;
            ViewData["TransactionType"] = yr.TransactionType;
            return PartialView("PaymentMethods",tr);

            //return PartialView();
        }
        public ActionResult ZESA(decimal amount = 0)
        {
            YomoneyController dt = new YomoneyController();
            List<YomoneyResponse> tr = new List<YomoneyResponse>();
            var MTI = "0300";
            var agentcode = "5-0001-0000091:Accu123()";
            string CustomerMSISDN = "";
            var Processsingcode = "420000";
            long TransactionType = 3;
            YomoneyRequest yr = new YomoneyRequest();
            var Store = db.Syskeys.Where(u => u.Name == "OnlineStore").FirstOrDefault();
            var agent = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "YomoneyAgentCode").FirstOrDefault().Value;
            var password = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "AgentPassword").FirstOrDefault().Value;

            yr.MTI = "0300";
            yr.Amount = (decimal)amount;
            yr.ProcessingCode = "420000";
            yr.TransactionType = 3;
            yr.AgentCode = agent.Trim() + ":" + password.Trim();
            //yr.CustomerMSISDN = resp.CustomerMSISDN;
            var list = dt.Payment(yr);
            if (list.Narrative != null)
            {
                tr = JsonConvert.DeserializeObject<List<YomoneyResponse>>(list.Narrative);
            }
            else
            {
                var jlist = JsonConvert.SerializeObject(list);
                return PartialView("EcoMsg", jlist);
            }

            //ViewData["Amount"] = amount;
            //ViewData["type"] = type;
            ViewData["TransactionType"] = TransactionType;
            return PartialView("PaymentMethods", tr);

            //return PartialView();
        }
        [Authorize]
        [HttpPost]
        public ActionResult Print(string item, string tender, string change, string customer, string creditPeriod, string IsCredit, string tr, string Discount,string OrderReceipt,string collectionPoint, string Currency, string BaseTotal)
        {
            if (item !="" && change != "NaN")
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                ReceiptVM  receiptModel= new ReceiptVM();
                string account = "";
                receiptModel.ToCollect = "No";
                var todate = DateTime.Now.Date;
                var ColectionDate = DateTime.Now.Date;

                var CurrentUser = User.Identity.Name;
                char[] delimiterr = new char[] { '~' };
                string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
                string location = partz[1];

                var user = np.Getlogin(partz[0]);
                char[] delimiters = new char[] { '/' };
                string[] parts = item.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
                var cust = new Customer();
                var keys = np.GetPosKeys(partz[1]);
                if (customer == null && IsCredit == "N")
                {
                    customer = "CASH SALE";
                    account = "8-01-0000000";
                }
                else if(keys.CustomerNameForCash.Trim() == "Yes")
                {
                    
                    char[] delimiter = new char[] { '_' };
                    string[] part = customer.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    if (part.Length > 1)
                    {
                        customer = part[0];
                        account = part[1];
                        if (part.Length >= 3 && creditPeriod == "ToCollect")
                        {
                            receiptModel.ToCollect = "Yes";
                            receiptModel.CollectionDate = part[2];
                            ColectionDate = DateTime.Parse(part[2]);
                        }
                        cust = db.Customers.Where(u => u.AccountCode == account).FirstOrDefault();
                    }
                    else
                    {
                        return Json("Customer Required", JsonRequestBehavior.AllowGet);
                    }
                }
                int cnt = parts.Count();
                List<Printdata> pds = new List<Printdata>();
                Sale sl = new Sale();

                sl.customer = customer;
                sl.Account = account;
                sl.company = partz[1];
                sl.dated = DateTime.Now;
                sl.discount = decimal.Parse(Discount);
                sl.Cashier = partz[0];
                sl.PaymentModes = tr;
                sl.TransactionCurrency = Currency;
                sl.CurrencyTotal = decimal.Parse(tender);
                db.Sales.Add(sl);
                db.SaveChanges();
                long id =sl.ID;
                
                decimal CostOfSale = 0;
                decimal amt = 0;
                decimal tax = 0;
                decimal subTotal = 0;
                
                Transaction trn = new Transaction();

                #region  update itemdata
                foreach (var itm in parts)
                {
                    decimal unitprice = 0;
                    decimal baseunitprice = 0;
                    char[] delimite = new char[] { ',' };
                    string[] par = itm.Split(delimite, StringSplitOptions.RemoveEmptyEntries);
                    var itmcode = par[3];
                    var it = db.Items.Where(u => u.company == user.Location && u.ItemCode.Trim() == itmcode).FirstOrDefault();//np.GetItemCode(par[3],partz[1]);
                    it.Balance = it.Balance - decimal.Parse(par[1]);
                    it.Quantity = it.Quantity - decimal.Parse(par[1]);
                    it.sold = it.sold + decimal.Parse(par[1]);
                    it.Amount = it.Amount + ((decimal)it.SellingPrice * decimal.Parse(par[1]));
                    it.Expected = it.Expected - ((decimal)it.SellingPrice * decimal.Parse(par[1]));
                    baseunitprice = (decimal)it.SellingPrice;
                    unitprice = Math.Round(decimal.Parse(par[2])/ decimal.Parse(par[1]),2);
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
                        basemoney = Math.Round(decimal.Parse(par[2]) - decimal.Parse(par[2]) * decimal.Parse(taxrate.Value), 2);
                        money = Math.Round(((decimal)it.SellingPrice * decimal.Parse(par[1])) - decimal.Parse(par[2]) * decimal.Parse(taxrate.Value), 2);
                        baseunitNoTax = Math.Round(basemoney / decimal.Parse(par[1]),2);
                        unitNoTax = Math.Round(money / decimal.Parse(par[1]),2);
                        taxamount = Math.Round(decimal.Parse(par[2]) * decimal.Parse(taxrate.Value), 2);
                        basetaxamount = Math.Round(((decimal)it.SellingPrice * decimal.Parse(par[1])) * decimal.Parse(taxrate.Value), 2);
                        
                        unitTax = Math.Round(unitNoTax / decimal.Parse(par[1]),2);
                        baseunitTax = Math.Round(unitNoTax / decimal.Parse(par[1]),2);
                    }
                    else
                    { 
                        money = Math.Round(((decimal)it.SellingPrice * decimal.Parse(par[1])), 2);
                        unitNoTax = Math.Round(money / decimal.Parse(par[1]),2);
                        basemoney = Math.Round(decimal.Parse(par[2]), 2);
                        baseunitNoTax = Math.Round(basemoney / decimal.Parse(par[1]),2);
                        taxamount = 0;
                        basetaxamount = 0;
                        unitTax = 0;
                        baseunitTax = 0;
                    }
                    #region Sale 
                       
                        Sales_Lines bs = new Sales_Lines();
                        bs.item = it.ItemName.Trim();
                        bs.ItemCode = it.ItemCode.Trim();
                        bs.quantity = long.Parse(par[1]);
                        bs.price = basemoney;
                        bs.tax = basetaxamount;
                        bs.priceinc = Math.Round(((decimal)it.SellingPrice * decimal.Parse(par[1])),2); //decimal.Parse(par[2]);
                        bs.TransactionPrice = money;
                        bs.Reciept = user.prefix.Trim() + "-" +id;
                        bs.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
                        bs.Dated = DateTime.Now;
                        bs.ItemCode = it.ItemCode.Trim();
                        bs.Company = partz[1];
                        
                        amt = amt + decimal.Parse(par[2]);
                        tax = tax + taxamount;
                        subTotal = subTotal + money;
 
                        np.SaveOrUpdateSalesLines(bs);
                   
                        //process sales transaction
                        #region GP
                        var slqty = decimal.Parse(par[1]);
                        decimal tmpqty = 0;

                        var pl = db.PurchaseLines.Where(u =>u.Company == user.Location.Trim() &&  u.ItemCode.Trim() == it.ItemCode.Trim() && u.Status == "O").FirstOrDefault();
                        if (pl != null)
                        {
                            if (pl.quantity >= slqty)
                            {
                                pl.quantity = pl.quantity - slqty;
                                if (pl.Sold == null) pl.Sold = 0;
                                pl.Sold  = pl.Sold  + slqty;
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
                                ss.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
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
                                            ss.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
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
                                            ss.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
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
                                        ss.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
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
                            ss.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
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

                        trn.ProcessSale("", decimal.Parse(par[2]), user.Location.Trim(), it.category, it.SubCategory, customer);
                         
                        pd.item = bs.item;
                        pd.qty = bs.quantity.ToString();
                        pd.amount = par[2];
                        pd.prize = Math.Round((decimal)(decimal.Parse(par[2]) / bs.quantity), 2).ToString();
                        if (it.Measure == null)
                        {
                            pd.unit = "ea";
                        }else
                        {
                            pd.unit = it.Measure.Trim();
                        }

                        pds.Add(pd);

                        if ( keys!= null && keys.OrderBased != null && keys.OrderBased.Trim() == "Y")
                        {
                            OrderLine oo = new OrderLine();
                            oo.item = it.ItemName.Trim();
                            oo.ItemCode = it.ItemCode.Trim();
                            oo.quantity = money;
                            oo.price = taxamount;
                            oo.tax = tax + taxamount;
                            oo.priceinc = decimal.Parse(par[2]);
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

                    slsam.Quantity = slsam.Quantity + long.Parse(par[1]);
                    slsam.Balance = slsam.Quantity - slsam.DeliveredQty;
                    slsam.Total = slsam.Total + decimal.Parse(par[2]);
                    slsam.Dated = DateTime.Now.Date;
                    slsam.Company = partz[1];
                    slsam.DeliveryType = "Collection";
                    slsam.Deadline = DateTime.Now.AddDays(1);
                    slsam.DeliveryPicking = false;
                    if (slsam.GPAmount == null) slsam.GPAmount = 0;
                    slsam.GPAmount = slsam.GPAmount + (decimal.Parse(par[2]) - (cost * long.Parse(par[1])));
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

                        if(trd[2] == "Y")
                        {
                            if (cust.Balance == null) cust.Balance = 0;
                            cust.Balance = cust.Balance + decimal.Parse(trd[1]);
                            db.Entry(cust).State = EntityState.Modified;
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
                    sal.Period = creditPeriod;
                    sal.Balance = receiptModel.creditBalance;
                    sal.state = "O";
                    sal.CostPrice = CostOfSale;
                    sal.GPAmount = sal.total - sal.CostPrice;
                    sal.CollectionId = long.Parse(collectionPoint);
                    sal.Tender = decimal.Parse(BaseTotal);
                    sal.company = partz[1];
                    receiptModel.Receipt = sal.Invoice;
                }
                else
                {
                    sal.Reciept = user.prefix.Trim() + "-" + sl.ID;
                    sal.Period = "0";
                    sal.state = "C";
                    sal.CostPrice = CostOfSale;
                    //sal.CollectionId = long.Parse(collectionPoint);
                    sal.company = partz[1];
                    sal.GPAmount = sal.total - sal.CostPrice;
                    sal.Tender = decimal.Parse(BaseTotal);
                    receiptModel.Receipt = sal.Reciept;
                }
                receiptModel.total = amt.ToString();
                receiptModel.tax = tax.ToString();
                receiptModel.subTotal = subTotal.ToString();
                receiptModel.tender = tender;
                receiptModel.change = change;
                receiptModel.cashier = partz[0];
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
                    ds.Id = DateTime.Now.ToString("ddMMyyyy") + company.ID  + "_SALE";
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
                    ds.AccountName ="SALE";
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
                catch(Exception E) 
                {

                }

                return PartialView(receiptModel);
                
            }

            return PartialView("Index");
        }
        
        [Authorize]
        public ActionResult Tender(string PaymentType, string Currency, int? QuotationId, decimal Baseamount = 0, decimal PartTotal = 0, decimal amount = 0)
        {


            NHibernateDataProvider np = new NHibernateDataProvider();
            string location;
            
            if (QuotationId != null)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimiterr = new char[] { '~' };
                string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
                var quot = db.Orders.Where(u => u.ID == QuotationId).FirstOrDefault();
                amount = Convert.ToDecimal(quot.total);
                location = partz[2];
            }

            else
            { 


            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            location = partz[1];

            }
            var comp = db.Companies.Where(u => u.name == location).FirstOrDefault();
            var px = new List<Account>();
            if (string.IsNullOrEmpty(PaymentType))
            {
                PaymentType = "SALE";
                var dd = db.Currencies.Where(u => u.Company == location).ToList();
                if (!string.IsNullOrEmpty(Currency))
                {
                    
                    px = db.Accounts.Where(u => u.CompanyId == comp.ID && u.AccountCode.StartsWith("2") && u.AccountCode.Length > 4 && u.Currency == Currency ).ToList();
                }
                else
                {
                    px = db.Accounts.Where(u => u.CompanyId == comp.ID && u.AccountCode.StartsWith("2") && u.AccountCode.Length > 4).ToList();

                }
                foreach (var itm in px)
                {
                    var rate = dd.Where(u => u.Id == itm.CurrencyId).FirstOrDefault();
                    decimal topaye = 0;
                    decimal topay= 0;
                    if(Baseamount > 0)
                    {
                        topaye = Baseamount - (PartTotal);
                        topay = Convert.ToDecimal(topaye / (rate.ExchangeRate));
                        
                    }else
                    {
                        topay = amount;
                    }
                    itm.Balance = topay * rate.ExchangeRate;
                    itm.Opening = rate.ExchangeRate;
                }
            }
            else
            {
                
                px = db.Accounts.Where(u => u.CompanyId == comp.ID && u.AccountCode.StartsWith("6") && u.AccountCode.Length > 4).ToList();
            }
            

            return PartialView("_Tender",px);
           // return PartialView("Tender");
        }

        [Authorize]
        public ActionResult ActionMenu()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            return PartialView();
            // return PartialView("Tender");
        }

        [Authorize]
        public ActionResult TenderData(string type, string currency, decimal amount)
        {

            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
            var r = type.Split('_');
            //var keys = np.GetPosKeys(partz[1]);
            var company = partz[1];
            var cu = partz[0];
            var user = db.logins.Where(u => u.username == cu).FirstOrDefault();
            var cc = db.Companies.Where(u => u.name == company).FirstOrDefault();
            ViewData["ActiveCity"] = cc.location.Trim();
            ViewData["Paid"] = amount;
            var discount = db.Accounts.Where(u => u.AccountName == type && u.GiveDiscount == "YES").FirstOrDefault();
            if (discount != null)
            {
                ViewData["Disc"] = discount.GiveDiscount;
                ViewData["percent"] = discount.Discount;
            }
            if (r.Length == 1 && user.PosCustomers == true)
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
                ViewData["Amnt"] = amount;

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
                List<CollectionPoint> points = new List<CollectionPoint>();
                if (fc != null)
                {
                    points = db.CollectionPoints.Where(u => u.City != null && u.City.Trim() == fc.Name.Trim()).ToList();

                }
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
                if (type == "YOMONEY")
                {
                    // AccountsController dt = new AccountsController();
                    YomoneyController dt = new YomoneyController();
                    List<YomoneyResponse> tr = new List<YomoneyResponse>();
                    YomoneyRequest yr = new YomoneyRequest();
                    //var Store = db.Syskeys.Where(u => u.Name == "OnlineStore").FirstOrDefault();
                    var agent = db.Syskeys.Where(u => u.Company == company && u.Name == "YomoneyAgentCode").FirstOrDefault().Value;
                    var password = db.Syskeys.Where(u => u.Company == company && u.Name == "AgentPassword").FirstOrDefault().Value;

                    yr.MTI = "0300";
                    yr.Amount = (decimal)amount;
                    yr.ProcessingCode = "420000";
                    yr.TransactionType = 5;
                    yr.AgentCode = agent.Trim() + ":" + password.Trim();
                    // yr.CustomerMSISDN = resp.CustomerMSISDN;
                    var list = dt.Payment(yr);
                    if (list.Narrative != null)
                    {
                        tr = JsonConvert.DeserializeObject<List<YomoneyResponse>>(list.Narrative);
                    }
                    else
                    {
                        var jlist = JsonConvert.SerializeObject(list);
                        return PartialView("EcoMsg", jlist);
                    }

                    ViewData["Amount"] = amount;
                    ViewData["type"] = type;
                    ViewData["Curr"] = currency;
                    return PartialView("PaymentMethods", tr);

                }
                ViewData["Type"] = type;
                ViewData["Amnt"] = amount;
               
                // ViewData["Amount"] = amount;
                return PartialView("Tender");
            }

        }


        [Authorize]
        public ActionResult CreditPayment()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var acc = np.GetTransactionsByType("RECEIPT");
            List<SelectListItem> yr = new List<SelectListItem>();
           
            yr.Add(new SelectListItem
            {
                Text = "--Select Type--",
                Value = ""
            });

            foreach (var item in acc)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.TransactionName,
                    Value = item.TransactionName
                });
            }

            ViewBag.Type = yr;
            if (Request.IsAjaxRequest())
                return PartialView("CreditPayment");
            return PartialView("CreditPayment");
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreditPayment(string item, string tender, string change, string customer, string creditPeriod, string IsCredit, string tr, string Discount, string OrderReceipt, string paymentType)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
            var user = np.Getlogin(partz[0]);
            if (partz.Length > 2)
            {
                if (!string.IsNullOrEmpty(customer) && !string.IsNullOrEmpty(tender) && !string.IsNullOrEmpty(OrderReceipt))
                {
                    char[] delimiter = new char[] { '_' };
                    string[] part = customer.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    customer = part[0];
                    string account = part[1];
                    switch (paymentType)
                    {
                        case "SALES TOPUP":
                            #region topup
                            break;
                        #endregion
                        case "CREDIT PAYMENT":
                            #region Credit payment
                            var cust = db.Customers.Where(U => U.AccountCode == account).FirstOrDefault();
                            cust.Balance = cust.Balance - decimal.Parse(tender);
                            db.Entry(cust).State = EntityState.Modified;
                            db.SaveChanges();
                            break;
                            #endregion
                    }
                    //var company = np.GetCompanies(partz[2]);
                    #region Knock off receipts
                    var orders = OrderReceipt.Split(',');
                    string paymentBreakdown = "";
                    if (orders.Length > 0)
                    {
                        decimal payment = decimal.Parse(tender);
                        foreach (var oda in orders)
                        {
                            decimal topay = 0;

                            Sale sl = db.Sales.Where(u => u.Reciept == oda).FirstOrDefault();
                            if (sl != null)
                            {
                                if (payment >= (decimal)sl.Balance.Value)
                                {
                                    topay = (decimal)sl.Balance.Value;
                                    payment = payment - (decimal)sl.Balance.Value;
                                }
                                else
                                {
                                    topay = payment;
                                }
                                sl.Balance = sl.Balance + topay;
                                db.Entry(sl).State = EntityState.Modified;
                                db.SaveChanges();
                                paymentBreakdown = paymentBreakdown + "," + (oda + "$ " + topay);
                            }else
                            {
                                paymentBreakdown = paymentBreakdown + "," + (oda + "$ " + payment);
                            }
                        }
                    }
                    else
                    {
                        paymentBreakdown = OrderReceipt;
                    }
                    #endregion
                    Transaction trn = new Transaction();
                    trn.ProcessSale(paymentType, decimal.Parse(tender), partz[2], "", "", customer);
                    #region Create Receipt
                    Receipt rp = new Receipt();
                    rp.Account = account;
                    rp.customer = customer;
                    rp.dated = DateTime.Now;
                    rp.total = decimal.Parse(tender);
                    rp.Cashier = partz[1];
                    rp.PaymentReferences = paymentBreakdown;
                   // rp.PaymentReferences = tr;
                    rp.state = "C";
                    db.Receipts.Add(rp);
                    db.SaveChanges();
                    rp.Receipt1 = user.prefix + "-" + rp.ID;
                    db.Entry(rp).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion
                    // hanlde customer's account
                }
                var acc = np.GetTransactionsByType("RECEIPT");
                List<SelectListItem> yr = new List<SelectListItem>();

                yr.Add(new SelectListItem
                {
                    Text = "--Select Type--",
                    Value = ""
                });

                foreach (var itm in acc)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = itm.TransactionName,
                        Value = itm.TransactionName
                    });
                }

                ViewBag.Type = yr;

                if (Request.IsAjaxRequest())
                    return PartialView("Credit");
                return PartialView("Credit");
            }
            else
            {
               return  RedirectToAction("Login");
            }
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateCustomer(Customer customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (Request.IsAuthenticated)
            {
                var resp = "";
                if (ModelState.IsValid)
                {
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    ST_encrypt en = new ST_encrypt();

                    string newAccount = en.encrypt(customer.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
                    Random rr = new Random();
                    var pass = rr.Next(00000, 99999).ToString();
                    string Pin = en.encrypt(pass, "214ea9d5bda5277f6d1b3a3c58fd7034");

                    var uu = np.GetSyskeysByName("CustomerNetworking", part[1]);
                    //.Value.Trim();
                   
                    string sk = "";
                    if (uu != null) sk = uu.Value;
                    if (customer.Phone1 != null && customer.Phone1.Length == 10)
                    {
                        customer.Phone1 = "263" + customer.Phone1.Substring(1, customer.Phone1.Length - 1);
                    }
                    else if (customer.Phone1 != null && customer.Phone1.Length == 9)
                    {
                        customer.Phone1 = "263" + customer.Phone1;
                    }
                    else if (customer.Phone1 != null && customer.Phone1.Length == 12)
                    {
                    }
                    else
                    {
                        resp = "Valid";
                        return Json(resp, JsonRequestBehavior.AllowGet);
                    }
                    var accs = db.Customers.Where(u => u.Phone1.Trim() == customer.Phone1 || u.CustomerName == customer.CustomerName.ToUpper()).ToList();
                    if (accs == null || accs.Count == 0)
                    {
                        #region accountcode 
                        var pz = np.GetAllCustomers().OrderBy(u =>u.ID).ToList();
                        var pp = (from e in pz select e).LastOrDefault();
                        if (pp == null)
                        {
                            pp = new Customer();
                            customer.AccountCode = "8-0001-0000001";
                        }
                        else
                        {
                            char[] delimitor = new char[] { '-' };
                            string[] partx = pp.AccountCode.Split(delimitor, StringSplitOptions.RemoveEmptyEntries);
                            var Id = long.Parse(partx[2]);
                            string acc = "";
                            var x = Id + 1;
                            if (x < 10)
                            {
                                acc = partx[0] + "-" + partx[1] + "-" + "000000" + x.ToString();
                            }
                            else if (x > 9 && x < 100)
                            {
                                acc = partx[0] + "-" + partx[1] + "-" + "00000" + x.ToString();
                            }
                            else if (x > 99 && x < 1000)
                            {
                                acc = partx[0] + "-" + partx[1] + "-" + "0000" + x.ToString();
                            }
                            else if (x > 999 && x < 10000)
                            {
                                acc = partx[0] + "-" + partx[1] + "-" + "000" + x.ToString();
                            }
                            else if (x > 9999 && x < 100000)
                            {
                                acc = partx[0] + "-" + partx[1] + "-" + "00" + x.ToString();
                            }
                            else if (x > 99999 && x < 1000000)
                            {
                                acc = partx[0] + "-" + partx[1] + "-" + "0" + x.ToString();
                            }
                            else if (x > 999999 && x < 10000000)
                            {
                                acc = partx[0] + "-" + partx[1] + "-" + x.ToString();
                            }
                            else if (x > 999999)
                            {
                                var Idd = long.Parse(partx[1]);
                                var xx = Idd + 1;
                                if (xx < 10)
                                {
                                    acc = partx[0] + "-" + "000" + xx.ToString() + "-" + "0000001";
                                }
                                else if (xx > 9 && xx < 100)
                                {
                                    acc = partx[0] + "-" + "00" + xx.ToString() + "-" + "0000001";
                                }
                                else if (xx > 99 && xx < 1000)
                                {
                                    acc = partx[0] + "-" + "0" + xx.ToString() + "-" + "0000001";
                                }
                                else if (xx > 999 && xx < 10000)
                                {
                                    acc = partx[0] + "-" + xx.ToString() + "-" + "0000001";
                                }
                                else
                                {
                                    acc = "Exhausted";
                                }
                            }
                            customer.AccountCode = acc;
                        }
                            #endregion
                        customer.Email = newAccount;
                        customer.Password = pass;
                        customer.CustomerName = customer.CustomerName.ToUpper();
                        if (sk == "Yes")
                        customer.ParentId = long.Parse(part[1]);
                        customer.Balance = 0;
                        customer.Wallet = 0;
                        customer.Purchases = 0;
                        customer.PurchasesToDate = 0;
                        //customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                        np.SaveOrUpdateCustomers(customer);
                        resp = customer.CustomerName + "_" + customer.AccountCode;
                        return Json(resp, JsonRequestBehavior.AllowGet);
                       // return RedirectToAction("Index");
                    }
                    else
                    {
                        resp = "Created";
                        return Json(resp, JsonRequestBehavior.AllowGet);
                    }
                }
                resp = "Error";
                return Json(resp, JsonRequestBehavior.AllowGet);
                //return PartialView(customer);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult Search(string searchText)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (searchText.StartsWith("0"))
            {
                searchText = "263" + searchText.Substring(1, searchText.Length - 1);
            }

            var px = np.GetCustomersSearch(searchText);
            return PartialView(px);
        }

        [Authorize]
        public ActionResult getItems(string json_str)
        {
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            string company = part[1];
            NHibernateDataProvider np = new NHibernateDataProvider();
            var currentUserId = User.Identity.Name;
            // string company = np.Getlogin(currentUserId);
            var item = db.Items.Where(u => u.ItemName.Contains(json_str) && u.company == company).ToList();
           
            return PartialView(item);
        }

        [Authorize]
        public ActionResult getPosItems(string json_str)
        
{
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                var currentUserId = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = currentUserId.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var location = part[1];
                var currentUser = part[0];
                var logins = np.GetAlllogin(location);
                var user = logins.Where(u => u.Firstname.ToUpper().Trim() == currentUser.ToUpper()).FirstOrDefault();
                var givesDiscounts = user.GivesDiscounts.ToString();
                var item = db.Items.Where(u => u.company == location && (u.ItemName.Contains(json_str) || u.ItemCode.Contains(json_str))).ToList();

                
                var tt = item.FirstOrDefault();
                //if (givesDiscounts == false || givesDiscounts == null)
                //{
                //    tt.Discount = null;
                //}


                if (item.Count() == 0)
                {
                    return RedirectToAction("getMenus", new { json_str = part[0] });
                }
                else if (item.Count() == 1)
                {

                    string returnData = tt.ItemName + "," + tt.ItemCode + "," + tt.SellingPrice + "," + tt.tax + "," + tt.Quantity + "," + tt.Discount + "," + tt.DicountType + "," + tt.tax + "," + givesDiscounts;
                    return Json(returnData, JsonRequestBehavior.AllowGet);
                  
                   
                }
                ViewData["givesDisc"] = givesDiscounts;
                return PartialView(item);
            }
            else
            {
                return RedirectToAction("PosLogin", "Login");
            }

        }

        [Authorize]
        public ActionResult getMenus(string json_str)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var item = np.GetMenu(json_str);
            return PartialView(item);
        }

        public ActionResult Authenticate(string json_str)
        {
            login lg = new login();
            lg.Account  = json_str;
            if(Request.IsAjaxRequest())
            return PartialView(lg);
            return View(lg);
        }

        [HttpPost]
        public JsonResult Authenticate(login login)
        {
                //int cnt;
                NHibernateDataProvider np = new NHibernateDataProvider();
                ST_encrypt en = new ST_encrypt();
                login px = new login();
                string resp = "";
                char[] delim = new char[] { '-' };
              ////  string[] pr = suppass.Split(delim, StringSplitOptions.RemoveEmptyEntries);
               var action = login.Account;
                // Account = en.encrypt(Account, "214ea9d5bda5277f6d1b3a3c58fd7034");
               var  Pin = en.encrypt(login.password, "214ea9d5bda5277f6d1b3a3c58fd7034");
                login acc = np.GetloginPW(Pin);
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
                            if (acc.CashCollection == true)
                            {
                            var npw = login.password + "-" + DateTime.Now;
                            var ccount = en.encrypt(npw, "214ea9d5bda5277f6d1b3a3c58fd7034");
                            resp = "success-" + ccount;
                            }
                            else
                            {
                                resp = "You are not authorised for this";
                            }

                            break;
                    case "cashnote":
                        if (acc.CashCollection == true)
                        {
                            var npw = login.password + "-" + DateTime.Now;
                            var ccount = en.encrypt(npw, "214ea9d5bda5277f6d1b3a3c58fd7034");
                            resp = "success-" + ccount;
                        }
                        else
                        {
                            resp = "You are not authorised for this";
                        }

                        break;
                    case "zreport":
                        if (acc.CashCollection == true)
                        {
                            var npw = login.password + "-" + DateTime.Now;
                            var ccount = en.encrypt(npw, "214ea9d5bda5277f6d1b3a3c58fd7034");
                            resp = "success-" + ccount;
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

        [Authorize]
        public ActionResult voidSale()
         {
            return PartialView();
        }

        public ActionResult OutOfStock(string Id)
        {
            ViewData["item"] = Id;
            return PartialView();
        }
        public ActionResult TransDiscount(string tran, string discount,double Amount)
        {
            ViewData["id"] = discount;
            ViewData["TransType"] = tran;
            ViewData["amount"] = Amount;

            var dis = db.Accounts.Where(u => u.AccountName == tran.Trim()).FirstOrDefault();

            if (dis.DiscountType.Trim() == "PERCENT")
            {
                tran = discount + "%";
            }

            if (dis.DiscountType.Trim() == "AMOUNT")
            {
                tran = "$" + discount;
            }

            ViewData["disType"] = discount;
            ViewData["percent"] = dis.Discount;
            ViewData["DisType"] = dis.DiscountType;
            return PartialView();
        }

        public ActionResult Discount(string id,string Name, double Qty, string type, double prize, double dcount, string tax, string disType)
        {
            ViewData["id"] = id;
            ViewData["item"] = Name;
            ViewData["Qty"] = Qty;
            ViewData["type"] = type.Trim();
            ViewData["prize"] = prize;
            ViewData["dcount"] = dcount;
            ViewData["tax"] = tax;

            if (type.Trim() == "PERCENT")
            {
                disType = dcount + "%";
            }

            if (type.Trim() == "AMOUNT")
            {
                disType = "$" + dcount;
            }

            ViewData["disType"] = disType;

            return PartialView();
        }
        #region YomoneyPayments
        public ActionResult AccountNumber(string Id)
        {
            YomoneyRequest resp = new YomoneyRequest();
            char[] delimiterr = new char[] { '~' };
            string[] partz = Id.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

            if (partz[0] == "Yomoney")
            {
                ViewData["TransactionType"] = "Yomoney";
                string amount = partz[0];
                decimal Amount = Convert.ToDecimal(partz[0]);
                resp.Amount = Amount;
            }
            else if (partz[0] == "2")
            {

                ViewData["TransactionType"] = "2";
            }
            else if (partz[0] == "3")
            {
                ViewData["TransactionType"] = "3";
             
                string amount = partz[0];
                decimal Amount = Convert.ToDecimal(partz[0]);
                resp.Amount = Amount;
            }
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult AccountNumber( YomoneyResponse resp, string Id)
        {
            //char[] delimiterr = new char[] { '~' };
            //string[] partz = Id.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
            //decimal Amount = Convert.ToDecimal(partz[0]);
            //string Currency = partz[1];
            // List<YomoneyResponse> List = new List<YomoneyResponse>();
           
            YomoneyController req = new YomoneyController();
            //decimal Amount = resp.Amount;
            if (resp.CustomerMSISDN.Length <= 13)
            {
                decimal Amount = decimal.Parse(resp.Amount);

                YomoneyRequest yr = new YomoneyRequest();
                var Store = db.Syskeys.Where(u => u.Name == "OnlineStore").FirstOrDefault();
                var agent = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "YomoneyAgentCode").FirstOrDefault().Value;
                var password = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "AgentPassword").FirstOrDefault().Value;

                yr.MTI = "0200";
                yr.Amount = (decimal)Amount;
                yr.ProcessingCode = "320000";
                yr.TransactionType = 5;
                yr.AgentCode = agent.Trim() + ":" + password.Trim();
                yr.CustomerMSISDN = resp.CustomerMSISDN;
                resp = req.Payment(yr);
            }
            else
            {
                decimal Amount = decimal.Parse(resp.Amount);
                
                YomoneyRequest yr = new YomoneyRequest();
                var Store = db.Syskeys.Where(u => u.Name == "OnlineStore").FirstOrDefault();
                var agent = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "YomoneyAgentCode").FirstOrDefault().Value;
                var password = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "AgentPassword").FirstOrDefault().Value;

                yr.MTI = "0200";
                yr.Amount = (decimal)Amount;
                yr.ProcessingCode = "330000";
                yr.TransactionType = 3;
                yr.AgentCode = agent.Trim() + ":" + password.Trim();
                yr.CustomerMSISDN = resp.CustomerMSISDN;
                resp = req.Payment(yr);
            }
            var jlist = JsonConvert.SerializeObject(resp);
            
            if (resp.ResponseCode == "00000")
            {
                //if (resp.Amount < Amount)
                //{
                   //var Balance = Amount - resp.Amount;
                   //var amount = Balance;
                   //var jlist = JavaScriptSerializer().Serialize(list);
                    //return Json(jlist, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                    //return Json(list, JsonRequestBehavior.AllowGet);
                //}
                return Json(jlist, JsonRequestBehavior.AllowGet);
            }
            //var lis = req.GetExternal(resp);
            return Json(jlist, JsonRequestBehavior.AllowGet);
        }
        public ActionResult ReceiverPhoneNumber(string Id)
        {
            YomoneyRequest resp = new YomoneyRequest();

            char[] delimiterr = new char[] { '~' };
            string[] partz = Id.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
            string amount = partz[0];
            decimal Amount = Convert.ToDecimal(partz[0]);
            resp.Amount = Amount;
            return PartialView();
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public JsonResult ReceiverPhoneNumber(YomoneyResponse resp, string Id)
        {
            //char[] delimiterr = new char[] { '~' };
            //string[] partz = Id.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
            //decimal Amount = Convert.ToDecimal(partz[0]);
            //string Currency = partz[1];

            YomoneyController req = new YomoneyController();
            //decimal Amount = resp.Amount;
            decimal Amount = 0;
            var MTI = "0200";
            var agentcode = "5-0001-0000091:Accu123()";
            var Processsingcode = "320000";
            long TransactionType = 5;
            string CustomerMSISDN = resp.CustomerMSISDN;
            YomoneyRequest yr = new YomoneyRequest();
            var Store = db.Syskeys.Where(u => u.Name == "OnlineStore").FirstOrDefault();
            var agent = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "YomoneyAgentCode").FirstOrDefault().Value;
            var password = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "AgentPassword").FirstOrDefault().Value;

            yr.MTI = "0200";
            yr.Amount = (decimal)Amount;
            yr.ProcessingCode = "320000";
            yr.TransactionType = 5;
            yr.AgentCode = agent.Trim() + ":" + password.Trim();
            yr.CustomerMSISDN = resp.CustomerMSISDN;
            var list = req.Payment(yr);
            var jlist = JsonConvert.SerializeObject(list);

            if (list.ResponseCode == "00000")
            {
                //if (resp.Amount < Amount)
                //{
                //var Balance = Amount - resp.Amount;
                //var amount = Balance;
                //var jlist = JavaScriptSerializer().Serialize(list);
                //return Json(jlist, JsonRequestBehavior.AllowGet);
                //}
                //else
                //{
                //return Json(list, JsonRequestBehavior.AllowGet);
                //}
                return Json(jlist, JsonRequestBehavior.AllowGet);
            }
            //var lis = req.GetExternal(resp);
            return Json(jlist, JsonRequestBehavior.AllowGet);
        }
        #endregion
        [Authorize]
        public ActionResult Create()
        {
            return View();
        } 
  
        [HttpPost]
        [Authorize]
        public ActionResult Create(Item item)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                np.SaveOrUpdateItems(item);
                return RedirectToAction("Index");  
            }

            return RedirectToAction("Index");
        }

        [Authorize]
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = np.GetItems(id);
            return View(item);
        }
     
        [Authorize]
        [HttpPost]
        public ActionResult Edit(Item item)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                np.SaveOrUpdateItems(item);
                return RedirectToAction("Index");
            }
            return View(item);
        }

        //
        // GET: /Items/Delete/5
        [Authorize]
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = np.GetItems(id);
            return View(item);
        }

        //
        // POST: /Items/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = np.GetItems(id);
            np.DeleteItems(item); 
            return RedirectToAction("Index");
        }

        #region OrderCompletion 
        [Authorize]
        [HttpPost]
        public ActionResult SaveOrder(string item, string tender, string change, string customer, string creditPeriod, string IsCredit, string tr, string Discount, string receipt)
        {
            if (item != "" && change != "NaN")
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                string account = "";

                var CurrentUser = User.Identity.Name;
                char[] delimiterr = new char[] { '~' };
                string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);

                var user = np.Getlogin(partz[0]);
                char[] delimiters = new char[] { '/' };
                string[] parts = item.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);
         
                int cnt = parts.Count();
                List<Printdata> pds = new List<Printdata>();
               
                Order sl = new Order();
                if (string.IsNullOrEmpty(receipt))
                {
                    sl.customer = customer;
                    sl.Account = account;
                    sl.company = partz[1];
                    sl.dated = DateTime.Now;
                    sl.discount = decimal.Parse(Discount);
                    sl.total = decimal.Parse(tender);
                    sl.cashier = partz[0];
                    db.Orders.Add(sl);
                    db.SaveChanges();
                }else
                {
                    sl = db.Orders.Where(u => u.Invoice == receipt).FirstOrDefault();
                    sl.customer = customer;
                    sl.Account = account;
                    sl.company = partz[1];
                    sl.dated = DateTime.Now;
                    sl.discount = decimal.Parse(Discount);
                    sl.total = decimal.Parse(tender);
                    sl.cashier = partz[0];
                    db.Entry(sl).State = EntityState.Modified;
                    db.SaveChanges();
                    var odl = db.OrderLines.Where(u => u.Reciept == receipt).ToList();
                    foreach(var i in odl)
                    {
                        db.Entry(i).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                }
                long id = sl.ID;

                decimal CostOfSale = 0;
                decimal amt = 0;
                decimal tax = 0;
                decimal subTotal = 0;
                var keys = np.GetPosKeys(partz[1]); // gets the pos settings
                Transaction trn = new Transaction();

                foreach (var itm in parts)
                {
                    char[] delimite = new char[] { ',' };
                    string[] par = itm.Split(delimite, StringSplitOptions.RemoveEmptyEntries);
                    var itmcode = par[3];
                    var it = db.Items.Where(u => u.ItemCode.Trim() == itmcode).FirstOrDefault();//np.GetItemCode(par[3],partz[1]);
                    it.Balance = it.Balance - decimal.Parse(par[1]);
                    it.Quantity = it.Quantity - decimal.Parse(par[1]);
                    it.sold = it.sold + decimal.Parse(par[1]);
                    it.Amount = it.Amount + decimal.Parse(par[2]);
                    it.Expected = it.Expected - decimal.Parse(par[2]);
                    //db.Entry(it).State = EntityState.Modified;
                    //db.SaveChanges();
                    //np.UpdateItems(it);

                    Printdata pd = new Printdata();
                    #region CashSale 
                    decimal money = 0;
                    decimal taxamount = 0;
                    if (it.tax == "1" || it.tax == "Taxed")
                    {
                        money = decimal.Parse(par[2]) - Math.Round(decimal.Parse(par[2]) * decimal.Parse("0.130435"), 2);
                        taxamount = Math.Round(decimal.Parse(par[2]) * decimal.Parse("0.130435"), 2);
                    }
                    else
                    {
                        money = Math.Round(decimal.Parse(par[2]), 2);
                        taxamount = 0;
                    }
                   
                        OrderLine ss = new OrderLine();
                        ss.item = it.ItemName.Trim();
                        ss.ItemCode = it.ItemCode.Trim();
                        ss.quantity = long.Parse(par[1]);
                        ss.price = money;
                        ss.tax =taxamount;
                        ss.priceinc = decimal.Parse(par[2]);
                        ss.Reciept = user.prefix.Trim() + "-" + id;
                        ss.Category = np.GetItemsByCode(par[3].ToString(), user.Location.Trim()).FirstOrDefault().category;
                        ss.Dated = DateTime.Now;
                        ss.ItemCode = it.ItemCode.Trim();
                        amt = amt + decimal.Parse(par[2]);
                        tax = tax +taxamount;
                        subTotal = subTotal + money;

                        db.OrderLines.Add(ss);
                        db.SaveChanges();
                        var quant = decimal.Parse(par[1]);
                        decimal newq = 0;
                       
                        //process sales transaction
                       // trn.ProcessSale(tr, decimal.Parse(par[2]), user.Location.Trim(), it.category, it.SubCategory, customer);

                        pd.item = ss.item;
                        pd.qty = ss.quantity.ToString();
                        pd.amount = ss.priceinc.ToString();
                        pd.prize = Math.Round((decimal)(decimal.Parse(par[2]) / ss.quantity), 2).ToString();
                        if (it.Measure == null)
                        {
                            pd.unit = "ea";
                        }
                        else
                        {
                            pd.unit = it.Measure.Trim();
                        }
                        pds.Add(pd);

                       
                    
                   #endregion

                }

                var sal = db.Orders.Find(id);
              
                    sal.Invoice = user.prefix.Trim() + "-" + sl.ID;
                    sal.Reciept  = user.prefix.Trim() + "-" + sl.ID;
                    sal.Period = creditPeriod;
                    sal.state = "O";
                    
                    ViewData["Receipt"] = sal.Invoice;
                
                db.Entry(sal).State  = EntityState.Modified;
                db.SaveChanges();

                var company = np.GetCompanies(user.Location.Trim());
                ViewData["total"] = amt;
                ViewData["tax"] = tax;
                ViewData["subTotal"] = subTotal;
                ViewData["tender"] = tender;
                ViewData["change"] = change;
                ViewData["cashier"] = partz[0];

                return PartialView();

            }

            return PartialView("Index");
        }

        [Authorize]
        public ActionResult getOrders(string date)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
            DateTime dd = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(date))
            {
                dd = DateTime.Parse(date).Date;
            }
            var item = db.Orders.Where(u => u.cashier  == usar && u.dated > dd &&  u.state.Trim() == "O" ).ToList();
            ViewData["Date"] = DateTime.Now;
            return PartialView(item);
        }

        public JsonResult  getOrderDetails(string receipt)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];

            var item = db.OrderLines.Where(u => u.Reciept == receipt).ToList();

           return Json(item, JsonRequestBehavior.AllowGet);
        }

        public JsonResult DailySalesSammary()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
            var todate = DateTime.Now.Date;
            var eDate = DateTime.Now.Date.AddDays(1);
            var item = db.DailyCashierSales.Where(u => u.Cashier == usar && (u.TransactionDate >= todate && u.TransactionDate < eDate)).ToList();


            //var item = db.OrderLines.Where(u => u.Reciept == receipt).ToList();

            return Json(item, JsonRequestBehavior.AllowGet);
        }


        [Authorize]
        [HttpPost]
        public ActionResult SaveCreditnote(string item, string tender, string change, string customer, string creditPeriod, string IsCredit, string tr, string Discount, string receipt, string Authorised)
        {
            if (item != "" && change != "NaN")
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                string account = "";
                ReceiptVM receiptModel = new ReceiptVM();
                
                var CurrentUser = User.Identity.Name;
                char[] delimiterr = new char[] { '~' };
                string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
                receiptModel.cashier = partz[0];
                receiptModel.CollectionDate = DateTime.Now.Date.ToString("dd MMM yyyy");
                receiptModel.ToCollect = "No";
                receiptModel.creditBalance = 0;
                var user = np.Getlogin(partz[0]);
                char[] delimiters = new char[] { '/' };
                string[] parts = item.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                int cnt = parts.Count();
                List<Printdata> pds = new List<Printdata>();

                Return sl = new RetailKing.Models.Return();
                if (string.IsNullOrEmpty(receipt))
                {
                    sl.customer = customer;
                    sl.company = partz[1];
                    sl.dated = DateTime.Now.ToString();
                    sl.discount = decimal.Parse(Discount);
                    sl.total = decimal.Parse(tender);
                    sl.Cashier = partz[0];
                    sl.Authorised = Authorised;
                    db.Returns.Add(sl);
                    db.SaveChanges();
                }
                else
                {
                    sl = db.Returns.Where(u => u.Invoice == receipt).FirstOrDefault();
                    sl.customer = customer;
                    sl.Cashier = partz[0];
                    sl.company = partz[1];
                    sl.dated = DateTime.Now.ToString();
                    sl.discount = decimal.Parse(Discount);
                    sl.total = decimal.Parse(tender);
                   
                    db.Entry(sl).State = EntityState.Modified;
                    db.SaveChanges();
                    var odl = db.OrderLines.Where(u => u.Reciept == receipt).ToList();
                    foreach (var i in odl)
                    {
                        db.Entry(i).State = EntityState.Deleted;
                        db.SaveChanges();
                    }
                }
                long id = sl.ID;

                decimal CostOfSale = 0;
                decimal amt = 0;
                decimal tax = 0;
                decimal subTotal = 0;
                var keys = np.GetPosKeys(partz[1]); // gets the pos settings
                Transaction trn = new Transaction();

                foreach (var itm in parts)
                {
                    char[] delimite = new char[] { ',' };
                    string[] par = itm.Split(delimite, StringSplitOptions.RemoveEmptyEntries);
                    var itmcode = par[3];
                    var it = db.Items.Where(u => u.ItemCode.Trim() == itmcode).FirstOrDefault();//np.GetItemCode(par[3],partz[1]);
                    it.Balance = it.Balance + decimal.Parse(par[1]);
                    it.Quantity = it.Quantity + decimal.Parse(par[1]);
                    it.sold = it.sold - decimal.Parse(par[1]);
                    it.Amount = it.Amount - decimal.Parse(par[2]);
                    it.Expected = it.Expected + decimal.Parse(par[2]);
                    db.Entry(it).State = EntityState.Modified;
                    db.SaveChanges();
                    //np.UpdateItems(it);

                    Printdata pd = new Printdata();
                    #region ReturnData 
                    decimal money = 0;
                    decimal taxamount = 0;
                    if (it.tax == "1" || it.tax == "Taxed")
                    {
                        money = decimal.Parse(par[2]) - Math.Round(decimal.Parse(par[2]) * decimal.Parse("0.130435"), 2);
                        taxamount = Math.Round(decimal.Parse(par[2]) * decimal.Parse("0.130435"), 2);
                    }
                    else
                    {
                        money = Math.Round(decimal.Parse(par[2]), 2);
                        taxamount = 0;
                    }
                   
                        ReturnLine ss = new ReturnLine();
                        ss.item = it.ItemName.Trim();
                        ss.ItemCode  = it.ItemCode.Trim();
                        ss.quantity = long.Parse(par[1]);
                        ss.price = money;
                        ss.tax = taxamount;
                        ss.priceinc = decimal.Parse(par[2]);
                        ss.Reciept = user.prefix.Trim() + "-" + id;
                        ss.ItemCode = it.ItemCode.Trim();
                        amt = amt + decimal.Parse(par[2]);
                        tax = tax +taxamount;
                        subTotal = subTotal + money;

                        db.ReturnLines.Add(ss);
                        db.SaveChanges();
                        var quant = decimal.Parse(par[1]);
                        decimal newq = 0;

                        //process sales transaction
                        // trn.ProcessSale(tr, decimal.Parse(par[2]), user.Location.Trim(), it.category, it.SubCategory, customer);

                        pd.item = ss.item;
                        pd.qty = ss.quantity.ToString();
                        pd.amount = ss.priceinc.ToString();
                        pd.prize = Math.Round((decimal)(decimal.Parse(par[2]) / ss.quantity), 2).ToString();
                        if (it.Measure == null)
                        {
                            pd.unit = "ea";
                        }
                        else
                        {
                            pd.unit = it.Measure.Trim();
                        }
                        pds.Add(pd);

                        PurchaseLine pl = db.PurchaseLines.Where(u => u.ItemCode.Trim() == it.ItemCode.Trim() && u.Status.Trim() == "O").FirstOrDefault();
                        if (pl != null)
                        {
                            if (pl.quantity >= quant)
                            {
                                pl.quantity = pl.quantity + long.Parse(par[1]);
                                pl.Sold = pl.Sold - long.Parse(par[1]);
                                db.Entry(pl).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                            else
                            {
                                newq = quant +(decimal)pl.quantity;
                                pl.quantity = pl.quantity + long.Parse(par[1]);
                                pl.Sold = pl.Sold - long.Parse(par[1]);
                                //pl.Status = "C";
                                pl.DateClosed = DateTime.Now;
                                db.Entry(pl).State = EntityState.Modified;
                                db.SaveChanges();
                                while (newq > 0)
                                {
                                    quant = newq;
                                    PurchaseLine npl = db.PurchaseLines.Where(u => u.ItemCode.Trim() == it.ItemCode.Trim() && u.Status.Trim() == "O").FirstOrDefault();
                                    if (npl != null && npl.quantity >= quant)
                                    {
                                        newq = quant + (decimal)npl.quantity;
                                        npl.quantity = npl.quantity + long.Parse(par[1]);
                                        npl.Sold = npl.Sold - long.Parse(par[1]);
                                        db.Entry(npl).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                    else
                                    {
                                        newq = quant + (decimal)npl.quantity;
                                        npl.quantity = npl.quantity + long.Parse(par[1]);
                                        npl.Sold = npl.Sold - long.Parse(par[1]);
                                        npl.Status = "C";
                                        npl.DateClosed = DateTime.Now;
                                        db.Entry(npl).State = EntityState.Modified;
                                        db.SaveChanges();
                                    }
                                }
                            }
                        }
                    
                    #endregion

                }
                receiptModel.Receipt = sl.Reciept;
               var company = np.GetCompanies(user.Location.Trim());
                receiptModel.total = amt.ToString();
                receiptModel.tax = tax.ToString();
                receiptModel.subTotal = subTotal.ToString();
                receiptModel.tender = tender;
                receiptModel.change = change;
                receiptModel.cashier = partz[0];
                receiptModel.posd = pds;
                receiptModel.company = company;
                ViewData["total"] = amt;
                ViewData["tax"] = tax;
                ViewData["subTotal"] = subTotal;
                ViewData["tender"] = tender;
                ViewData["change"] = change;
                ViewData["cashier"] = partz[0];

                return PartialView("CreditNote", receiptModel);

            }

            return Json("Error",JsonRequestBehavior.AllowGet);
        }

        #endregion

        #region Sales 
        public ActionResult getSales(string date)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar =part[0];
            DateTime dd = DateTime.Now.Date;
            if(!string.IsNullOrEmpty(date))
            {
                dd = DateTime.Parse(date);
            }
            var item = db.Sales.Where(u => u.Cashier == usar && u.dated > dd).OrderBy(u => u.ID).ToList();
            ViewData["Date"] = dd;
            return PartialView(item);
        }
        [HttpPost]
        public ActionResult getSales(string search, DateTime date)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
            var item = db.Sales.Where(u => u.Cashier == usar && u.dated == date).OrderBy(u => u.ID).ToList();

            return PartialView(item);
        }

        public ActionResult getSalesDetails(string receipt)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
            var sale = db.Sales.Where(u => u.Reciept == receipt).FirstOrDefault();
            var receiptModel = JsonConvert.DeserializeObject<ReceiptVM>(sale.Miscellenious);
            receiptModel.isCopy = true;
            return PartialView("Print", receiptModel);
        }

        public ActionResult getCustomerSales(string date, string customer,string TranType)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
            var account="";
            if (!string.IsNullOrEmpty(customer))
            {
                var cust = customer.Split('_');
                account = cust[1];
                customer = cust[0];
            }
            DateTime dd = DateTime.Now.Date.AddDays(-30);
            if (!string.IsNullOrEmpty(date))
            {
                dd = DateTime.Parse(date);
            }
            string state = "C";
            if(TranType == "CREDIT PAYMENT") state = "O";
            var item = db.Sales.Where(u => u.Account.Trim() == account && u.state.Trim() == state && u.dated >= dd).OrderBy(u => u.ID).ToList();
            ViewData["Date"] = dd;
            return PartialView(item);
        }
        #endregion

        #region Cash And reports
        [Authorize]
        public ActionResult CashCollection()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var px = db.Accounts.Where(u => u.AccountCode.StartsWith("2-01") && u.AccountCode.Length > 4).ToList();

            List<SelectListItem> ct = new List<SelectListItem>();
            
            foreach (var item in px)
            {
                ct.Add(new SelectListItem
                {
                    Text = item.AccountName.Trim(),
                    Value = item.AccountName.Trim()
                });
            }
            ViewBag.currency = ct;
            login lg = new login();
            lg.Account = "cashup";
            return PartialView(lg);
        }
        [Authorize]
        [HttpPost]
        public ActionResult CashCollection(string lines, string total, string supervisor)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
            ST_encrypt en = new ST_encrypt();
            ST_decrypt dec = new ST_decrypt();
            var npx = dec.st_decrypt(supervisor, "214ea9d5bda5277f6d1b3a3c58fd7034");
            var s = npx.Split('-');
            var Pin = en.encrypt(s[0], "214ea9d5bda5277f6d1b3a3c58fd7034");
            login acc = np.GetloginPW(Pin);
            if (acc != null)
            {
             if(acc.CashCollection == true)
               {
                    CashCollection cc = new Models.CashCollection();
                    cc.Cashier = usar;
                    cc.CollectedBy = acc.username;
                    cc.Dated = DateTime.Now;
                    cc.Breakdown = lines;
                    cc.Type = "Cashup";
                    cc.Total = decimal.Parse(total);
                    db.CashCollections.Add(cc);
                    db.SaveChanges();

                    return Json("Success: You are not Authorised for this", JsonRequestBehavior.AllowGet);
                }
                else
                {
                   return Json("Error: You are not Authorised for this", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Error: You are not Authorised for this", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult XCollection()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
            var px = db.Accounts.Where(u => u.AccountCode.StartsWith("2") && u.AccountCode.Length > 4).ToList();
            var todate = DateTime.Now.Date;
            var eDate = DateTime.Now.Date.AddDays(1);
          //  var ss = db.DailyCashierSales.Where(u => u.Cashier == usar && (u.TransactionDate >= todate && u.TransactionDate < eDate)).ToList();
            

            List<SelectListItem> ct = new List<SelectListItem>();

            foreach (var item in px)
            {
                ct.Add(new SelectListItem
                {
                    Text = item.AccountName.Trim(),
                    Value = item.AccountName.Trim()
                });
            }
            ViewBag.currency = ct;
            //var dat = (from e in ss select new { Id = e.Id.Trim(), Sales = e.Sales, AccountName = e.AccountName.Trim() }).ToList();
            login lg = new login();
            lg.Account = "zreport";
         //   ViewBag.AccountType = Json(dat, JsonRequestBehavior.AllowGet); ;
            return PartialView(lg);
        }
        [Authorize]
        [HttpPost] 
        public ActionResult XCollection(string lines, string total, string supervisor, string password)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
            ST_encrypt en = new ST_encrypt();
            ST_decrypt dec = new ST_decrypt();
            var npx = dec.st_decrypt(supervisor, "214ea9d5bda5277f6d1b3a3c58fd7034");
            var s = npx.Split('-');
            var Pin = en.encrypt(s[0], "214ea9d5bda5277f6d1b3a3c58fd7034");
            login acc = np.GetloginPW(Pin);
            if (acc != null)
            {
                if (acc.xreport == true)
                {
                    CashCollection cc = new Models.CashCollection();
                    cc.Cashier = usar;
                    cc.CollectedBy = acc.username;
                    cc.Dated = DateTime.Now;
                    cc.Breakdown = lines;
                    cc.Type = "Dayend";
                    cc.Total = decimal.Parse(total);
                    db.CashCollections.Add(cc);
                    db.SaveChanges();

                    return Json("Success: You are not Authorised for this", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json("Error: You are not Authorised for this", JsonRequestBehavior.AllowGet);
                }
            }
            return Json("Error: You are not Authorised for this", JsonRequestBehavior.AllowGet);
        }

        [Authorize]
        public ActionResult TillReports()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
            var trndate = DateTime.Now.Date;
            var px = db.Sales.Where(u => u.Cashier.Trim()== usar && u.dated >= trndate).ToList();
            ViewData["Company"] = part[2];
            ViewData["Cashier"] = usar;
            return PartialView(px);
        }
        #endregion

        #region Pos Printers
        public ActionResult Printers(string date)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
            DateTime dd = DateTime.Now.Date;
           
            var item = db.PosPrinters.ToList();
            ViewData["Date"] = dd;
            return PartialView(item);
        }

        public ActionResult CreatePrinter()
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
         
            var companies = db.Companies.ToList();
            List<SelectListItem> yr = new List<SelectListItem>();
            foreach (var item in companies)
            {
                yr.Add(new SelectListItem
                {
                    Text = item.name ,
                    Value = item.name.ToString()
                });
            }

            ViewBag.Location = yr;

           
            return PartialView();
        }

        [HttpPost]
        public ActionResult CreatePrinter(PosPrinter printer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var usar = part[0];

                db.PosPrinters.Add(printer);
                db.SaveChanges();
                return RedirectToAction("Printers");
            }
            return PartialView(printer);
        }
        public ActionResult EditPrinter(string date)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var usar = part[0];
            DateTime dd = DateTime.Now.Date;
            if (!string.IsNullOrEmpty(date))
            {
                dd = DateTime.Parse(date);
            }
            var item = db.Sales.Where(u => u.Cashier == usar && u.dated > dd).OrderBy(u => u.ID).ToList();
            ViewData["Date"] = dd;
            return PartialView(item);
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
        #endregion 

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

        public class FakeController : Controller
        {
        }

        public class MockHelper
        {
            public static HttpContext FakeHttpContext()
            {
                var httpRequest = new HttpRequest(string.Empty, "http://novomatic/", string.Empty);
                var stringWriter = new StringWriter();
                var httpResponce = new HttpResponse(stringWriter);
                var httpContext = new HttpContext(httpRequest, httpResponce);

                var sessionContainer = new HttpSessionStateContainer(
                    "id",
                    new SessionStateItemCollection(),
                    new HttpStaticObjectsCollection(),
                    10,
                    true,
                    HttpCookieMode.AutoDetect,
                    SessionStateMode.InProc,
                    false);

                httpContext.Items["AspSession"] =
                    typeof(HttpSessionState).GetConstructor(
                        BindingFlags.NonPublic | BindingFlags.Instance,
                        null,
                        CallingConventions.Standard,
                        new[] { typeof(HttpSessionStateContainer) },
                        null).Invoke(new object[] { sessionContainer });

                return httpContext;
            }
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