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
using System.Data.Entity.Validation;
using System.Xml.Linq;
using Rotativa;
using System.Collections.Specialized;
using System.Security.Cryptography;
using System.Text;
using System.Net;
using System.IO;
using System.Web.UI;
using crypto;
using System.Threading;
using System.Net.Mail;
using Newtonsoft.Json;
using System.Globalization;

namespace RetailKing.Controllers
{   
    public class CartController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();
         RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Items/
        // [Authorize]
        [HttpGet]
        public ActionResult AddToCart(long qty, long id, string spec, decimal total, string resptyp, string SessionId)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = db.Items.Find(id);
            var cust = new Customer();
            var gues = new Guest();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            if (part.Length > 0)
            {
                cust = db.Customers.Find(long.Parse(part[1]));
            }
            #region Guest
            else
            {

                gues.CustomerName = "Guest";
                gues.Username = SessionId;

                var pz = np.GetAllGuest();
                var pp = (from e in pz select e).LastOrDefault();

                Guest px = new Guest();
                if (pp == null)
                {
                    pp = new Guest();
                    px.AccountCode = "9-0001-0000001";
                }

                else
                {
                    char[] delimita = new char[] { '-' };
                    string[] party = pp.AccountCode.Split(delimita, StringSplitOptions.RemoveEmptyEntries);
                    var Id = long.Parse(party[2]);
                    string acc = "";
                    var x = Id + 1;
                    if (x < 10)
                    {
                        acc = party[0] + "-" + party[1] + "-" + "000000" + x.ToString();
                    }
                    else if (x > 9 && x < 100)
                    {
                        acc = party[0] + "-" + party[1] + "-" + "00000" + x.ToString();
                    }
                    else if (x > 99 && x < 1000)
                    {
                        acc = party[0] + "-" + party[1] + "-" + "0000" + x.ToString();
                    }
                    else if (x > 999 && x < 10000)
                    {
                        acc = party[0] + "-" + party[1] + "-" + "000" + x.ToString();
                    }
                    else if (x > 9999 && x < 100000)
                    {
                        acc = party[0] + "-" + party[1] + "-" + "00" + x.ToString();
                    }
                    else if (x > 99999 && x < 1000000)
                    {
                        acc = party[0] + "-" + party[1] + "-" + "0" + x.ToString();
                    }
                    else if (x > 999999 && x < 10000000)
                    {
                        acc = party[0] + "-" + party[1] + "-" + x.ToString();
                    }
                    else if (x > 999999)
                    {
                        var Idd = long.Parse(part[1]);
                        var xx = Idd + 1;
                        if (xx < 10)
                        {
                            acc = party[0] + "-" + "000" + xx.ToString() + "-" + "0000001";
                        }
                        else if (xx > 9 && xx < 100)
                        {
                            acc = party[0] + "-" + "00" + xx.ToString() + "-" + "0000001";
                        }
                        else if (xx > 99 && xx < 1000)
                        {
                            acc = party[0] + "-" + "0" + xx.ToString() + "-" + "0000001";
                        }
                        else if (xx > 999 && xx < 10000)
                        {
                            acc = party[0] + "-" + xx.ToString() + "-" + "0000001";
                        }
                        else
                        {
                            acc = "Exhausted";
                        }
                    }
                    px.AccountCode = acc;
                    //gues.AccountCode = px.AccountCode;
                    px.Username = SessionId;
                    // Session["Username"] = SessionId;
                    px.CustomerName = "Guest";
                    // px.CompanyId = Company;
                    //cust.AccountCode = TempData["acc"].ToString();

                }


                if (ModelState.IsValid)
                {
                    var CurrentGuest = User.Identity.Name;
                    char[] delimitar = new char[] { '~' };
                    string[] partx = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    /*ST_encrypt en = new ST_encrypt();

                      string newAccount = en.encrypt( px.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
                      Random rr = new Random();
                      var pass = rr.Next(00000, 99999).ToString();
                      string Pin = en.encrypt(pass, "214ea9d5bda5277f6d1b3a3c58fd7034");*/

                    //   var uu = np.GetSyskeysByName("CustomerNetworking", part[1]);
                    //.Value.Trim();
                    string sk = "";
                    //  if (uu != null) sk = uu.Value;
                    var accs = db.Guests.Where(u => u.Username.Trim() == px.Username).ToList();
                    if (accs == null || accs.Count() == 0)
                    {

                        //px.Email = newAccount; 

                        // px.CustomerName =  px.CustomerName.ToUpper();
                        if (sk == "Yes")
                            px.ParentId = long.Parse(part[1]);
                        px.Balance = 0;
                        px.Wallet = 0;
                        px.Purchases = 0;
                        px.PurchasesToDate = 0;
                        //customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                        np.SaveOrUpdateGuest(px);

                        // XmlCustomers cus = new XmlCustomers();
                        // cus.CreateCustomerProfile(customer);

                        return RedirectToAction("Index");

                    }
                    else
                    {
                        ModelState.AddModelError("", "Sorry this customer name is already in use");
                    }
                }


                #endregion

            }
            var result = "";
            //if (Request.IsAuthenticated)
            //{
            if (Request.IsAuthenticated)
            {
                #region Order
                if (item.Balance == 0)
                {
                    return Json("Stock", JsonRequestBehavior.AllowGet);
                }
                //cust.Username = SessionId;
                //// cust.Username = SessionId;

                //var pzr = np.GetAllCustomers();
                //var ppr = pzr.Where(u => u.Username == SessionId).FirstOrDefault();
                //cust.AccountCode = ppr.AccountCode;
                var order = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim()).FirstOrDefault();
                var ordaline = new OrderLine();
                var orda = new Order();

                if (order != null)
                {

                    var ordalin = db.OrderLines.Where(u => u.ItemCode.Trim() == item.ItemCode.Trim() & u.Reciept.Trim() == order.Reciept).FirstOrDefault();

                    if (ordalin != null)// if this item has already been added to the order
                    {
                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordalin.Discount = ordalin.Discount + (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordalin.Discount = ordalin.Discount + (qty * item.Discount);
                            }

                            order.discount = order.discount + ordalin.Discount;
                            total = total - (decimal)ordalin.Discount;

                        }
                        // ordaline = ordalin;
                        ordalin.quantity = ordalin.quantity + qty;
                        ordalin.priceinc = ordalin.priceinc + total;
                        ordalin.price = item.SellingPrice;

                        if (item.tax == "Taxed")
                        {
                            ordalin.tax = ordalin.tax + (total * decimal.Parse("0.15"));
                        }
                        if (!string.IsNullOrEmpty(item.AvailabilityInterval))
                        {
                            if (DateTime.Now > item.AvailabilityDate)
                            {
                                DateTime nextdate = DateTime.Now;
                                int dayspast = (DateTime.Now - item.AvailabilityDate).Value.Days;
                                switch (item.AvailabilityInterval.Trim())
                                {
                                    case "WEEKLY":
                                        int weekspast = dayspast / 7;
                                        if (weekspast == 0) weekspast = 1;
                                        nextdate = item.AvailabilityDate.Value.AddDays(weekspast * 7);
                                        //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                        item.AvailabilityDate = nextdate;
                                        break;
                                    case "FORTNIGHTLY":
                                        int weeks_past = dayspast / 14;
                                        if (weeks_past == 0) weeks_past = 1;
                                        nextdate = item.AvailabilityDate.Value.AddDays(weeks_past * 14);
                                        //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                        item.AvailabilityDate = nextdate;
                                        break;
                                    case "MONTHLY":
                                        int months = dayspast / 30;
                                        if (months == 0) months = 1;
                                        nextdate = item.AvailabilityDate.Value.AddDays(months * 30);
                                        //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                        item.AvailabilityDate = nextdate;
                                        break;
                                    case "QUATERLY":
                                        break;
                                    case "ANNUALY":
                                        int years = dayspast / 365;
                                        if (years == 0) years = 1;
                                        nextdate = item.AvailabilityDate.Value.AddDays(years * 365);
                                        //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                        item.AvailabilityDate = nextdate;
                                        break;
                                }
                            }
                            if (string.IsNullOrEmpty(order.Period))
                            {
                                order.Period = item.AvailabilityDate.ToString();
                            }
                            else
                            {
                                var dt = DateTime.Parse(order.Period);
                                if (dt < item.AvailabilityDate)
                                {
                                    order.Period = item.AvailabilityDate.ToString();
                                }

                            }
                        }

                        order.total = order.total + total;
                        db.Entry(ordalin).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    else// order is null
                    {
                        ordaline.Discount = 0;
                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordaline.Discount = (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordaline.Discount = (qty * item.Discount);
                            }
                            order.discount = order.discount + ordaline.Discount;
                            total = total - (decimal)ordaline.Discount;
                        }
                        ordaline.item = item.ItemName;
                        ordaline.ItemCode = item.ItemCode;
                        ordaline.Category = item.category;
                        ordaline.SubCategory = item.SubCategory;
                        ordaline.quantity = qty;
                        ordaline.price = item.SellingPrice;
                        ordaline.Description = spec;
                        ordaline.priceinc = decimal.Parse(total.ToString());
                        ordaline.tax = 0;

                        ordaline.Dated = DateTime.Now;
                        ordaline.Company = item.company;


                        if (item.tax == "Taxed")
                        {
                            ordaline.tax = (total * decimal.Parse("0.15"));
                            order.Tax = order.Tax + (total * decimal.Parse("0.15"));
                        }
                        if (item.category.Trim() != "AIRTIME" && order.CollectionId == 2)
                        {
                            order.CollectionId = 0;
                            order.DeliveryType = "Delivery";
                        }
                        order.total = order.total + total;
                        ordaline.Reciept = order.Reciept;
                        db.OrderLines.Add(ordaline);
                        db.SaveChanges();
                    }
                    // var pz = np.GetAllCustomers();
                    //var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                    // cust.AccountCode = pp.AccountCode;
                    if (!string.IsNullOrEmpty(item.AvailabilityInterval))
                    {
                        if (DateTime.Now > item.AvailabilityDate)
                        {
                            DateTime nextdate = DateTime.Now;
                            int dayspast = (DateTime.Now - item.AvailabilityDate).Value.Days;
                            switch (item.AvailabilityInterval.Trim())
                            {
                                case "WEEKLY":
                                    int weekspast = dayspast / 7;
                                    if (weekspast == 0) weekspast = 1;
                                    nextdate = item.AvailabilityDate.Value.AddDays(weekspast * 7);
                                    //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                    item.AvailabilityDate = nextdate;
                                    break;
                                case "FORTNIGHTLY":
                                    int weeks_past = dayspast / 14;
                                    if (weeks_past == 0) weeks_past = 1;
                                    nextdate = item.AvailabilityDate.Value.AddDays(weeks_past * 14);
                                    //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                    item.AvailabilityDate = nextdate;
                                    break;
                                case "MONTHLY":
                                    int months = dayspast / 30;
                                    if (months == 0) months = 1;
                                    nextdate = item.AvailabilityDate.Value.AddDays(months * 30);
                                    //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                    item.AvailabilityDate = nextdate;
                                    break;
                                case "QUATERLY":
                                    break;
                                case "ANNUALY":
                                    int years = dayspast / 365;
                                    if (years == 0) years = 1;
                                    nextdate = item.AvailabilityDate.Value.AddDays(years * 365);
                                    //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                    item.AvailabilityDate = nextdate;
                                    break;
                            }
                        }
                        if (string.IsNullOrEmpty(order.Period))
                        {
                            order.Period = item.AvailabilityDate.ToString();
                        }
                        else
                        {
                            var dt = DateTime.Parse(order.Period);
                            if (dt < item.AvailabilityDate)
                            {
                                order.Period = item.AvailabilityDate.ToString();
                            }

                        }
                    }

                    var ordercart = db.OrderLines.Where(u => u.Reciept.Trim() == order.Reciept).Count();
                    ViewData["cartitem"] = ordercart;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    switch (order.state.Trim())
                    {
                        case "O":
                            ViewData["OrderStatus"] = "OPEN";
                            break;
                        case "A":
                            ViewData["OrderStatus"] = "AWAITING COLLECTION";
                            break;
                        case "AS":
                            ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                            break;
                        case "S":
                            ViewData["OrderStatus"] = "SHIPPED";
                            break;
                        case "C":
                            ViewData["OrderStatus"] = "DELIVERED";
                            break;
                    }
                }
                else
                {
                    ordaline.Discount = 0;
                    if (item.Promotion.Trim() == "YES")
                    {
                        if (item.DicountType == "PERCENT                       ")
                        {
                            ordaline.Discount = (total * item.Discount) / 100;
                        }
                        else
                        {
                            ordaline.Discount = (qty * item.Discount);
                        }

                        total = total - (decimal)ordaline.Discount;
                    }
                    ordaline.item = item.ItemName;
                    ordaline.ItemCode = item.ItemCode;
                    ordaline.Category = item.category;
                    ordaline.SubCategory = item.SubCategory;
                    ordaline.quantity = qty;
                    ordaline.price = item.SellingPrice;
                    ordaline.Description = spec;
                    ordaline.priceinc = decimal.Parse(total.ToString());
                    ordaline.tax = 0;

                    ordaline.Dated = DateTime.Now;
                    ordaline.Company = item.company;

                    if (item.tax == "Taxed")
                    {
                        ordaline.tax = (total * decimal.Parse("0.15"));

                    }

                    var rand = new Random();
                    var num = rand.Next(0, cust.CustomerName.Length - 3);
                    orda.Account = cust.AccountCode;
                    orda.customer = cust.CustomerName;
                    orda.dated = DateTime.Now;
                    orda.discount = ordaline.Discount;
                    orda.Tax = ordaline.tax;
                    orda.total = decimal.Parse(total.ToString());
                    orda.state = "O";
                    if (item.category.Trim() == "AIRTIME")
                    {
                        orda.CollectionId = 2;
                        orda.DeliveryType = "Sms";
                    }
                    else
                    {
                        orda.CollectionId = 0;
                        orda.DeliveryType = "Delivery";
                    }
                    db.Orders.Add(orda);
                    db.SaveChanges();
                    orda.Reciept = cust.CustomerName.Substring(num, 3) + orda.ID + "." + DateTime.Now.TimeOfDay.ToString("hhmmss") + "." + DateTime.Now.ToString("ddMMyyyy");
                    db.Entry(orda).State = EntityState.Modified;
                    db.SaveChanges();

                    ordaline.Reciept = orda.Reciept;
                    db.OrderLines.Add(ordaline);
                    db.SaveChanges();
                    ViewData["OrderStatus"] = "OPEN";

                }

                var ols = new List<OrderLine>();
                ols = db.OrderLines.Where(u => u.Reciept == orda.Reciept).ToList();
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                var regionz = db.Regions.Find(cust.RegionId);

                if (Request.IsAuthenticated)
                {
                    var surburb = db.Suburbs.Find(long.Parse(cust.Suburb));
                    var city = db.Cities.Find(long.Parse(cust.City));
                    cust.Suburb = surburb.Name;
                    cust.City = city.Name;

                }

                var resp = new ShoppingCart();
                resp.orderlines = ols;
                resp.categories = categories;
                resp.customer = cust;
                resp.region = regionz;
                if (orda.CollectionId != 0)
                {
                    resp.collectxn = db.CollectionPoints.Find(orda.CollectionId);
                }

                if (resptyp == "Yes")
                {
                    return PartialView(resp);
                }
                else
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
            else
            {
                #region OrderGuest
                if (item.Balance == 0)
                {
                    return Json("Stock", JsonRequestBehavior.AllowGet);
                }
                gues.Username = SessionId;
                // cust.Username = SessionId;

                var pzr = np.GetAllGuest();
                var ppr = pzr.Where(u => u.Username == SessionId).FirstOrDefault();
                gues.AccountCode = ppr.AccountCode;
                var order = db.Orders.Where(u => u.Account.Trim() == gues.AccountCode.Trim()).FirstOrDefault();
                var ordaline = new OrderLine();
                var orda = new Order();

                if (order != null)
                {

                    var ordalin = db.OrderLines.Where(u => u.ItemCode.Trim() == item.ItemCode.Trim() & u.Reciept.Trim() == order.Reciept).FirstOrDefault();

                    if (ordalin != null)// if this item has already been added to the order
                    {
                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordalin.Discount = ordalin.Discount + (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordalin.Discount = ordalin.Discount + (qty * item.Discount);
                            }

                            order.discount = order.discount + ordalin.Discount;
                            total = total - (decimal)ordalin.Discount;

                        }
                        // ordaline = ordalin;
                        ordalin.quantity = ordalin.quantity + qty;
                        ordalin.priceinc = ordalin.priceinc + total;
                        ordalin.price = item.SellingPrice;

                        if (item.tax == "Taxed")
                        {
                            ordalin.tax = ordalin.tax + (total * decimal.Parse("0.15"));

                        }

                        order.total = order.total + total;
                        db.Entry(ordalin).State = EntityState.Modified;
                        db.SaveChanges();



                    }
                    else// order is null
                    {
                        ordaline.Discount = 0;
                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordaline.Discount = (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordaline.Discount = (qty * item.Discount);
                            }
                            order.discount = order.discount + ordaline.Discount;
                            total = total - (decimal)ordaline.Discount;
                        }
                        ordaline.item = item.ItemName;
                        ordaline.ItemCode = item.ItemCode;
                        ordaline.Category = item.category;
                        ordaline.SubCategory = item.SubCategory;
                        ordaline.quantity = qty;
                        ordaline.price = item.SellingPrice;
                        ordaline.Description = spec;
                        ordaline.priceinc = decimal.Parse(total.ToString());
                        ordaline.tax = 0;

                        ordaline.Dated = DateTime.Now;
                        ordaline.Company = item.company;


                        if (item.tax == "Taxed")
                        {
                            ordaline.tax = (total * decimal.Parse("0.15"));
                            order.Tax = order.Tax + (total * decimal.Parse("0.15"));
                        }
                        if (item.category.Trim() != "AIRTIME" && order.CollectionId == 2)
                        {
                            order.CollectionId = 0;
                            order.DeliveryType = "Delivery";
                        }
                        order.total = order.total + total;
                        ordaline.Reciept = order.Reciept;
                        db.OrderLines.Add(ordaline);
                        db.SaveChanges();
                    }
                    // var pz = np.GetAllCustomers();
                    //var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                    // cust.AccountCode = pp.AccountCode;
                    var ordercart = db.OrderLines.Where(u => u.Reciept.Trim() == order.Reciept).Count();
                    ViewData["cartitem"] = ordercart;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    switch (order.state.Trim())
                    {
                        case "O":
                            ViewData["OrderStatus"] = "OPEN";
                            break;
                        case "A":
                            ViewData["OrderStatus"] = "AWAITING COLLECTION";
                            break;
                        case "AS":
                            ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                            break;
                        case "S":
                            ViewData["OrderStatus"] = "SHIPPED";
                            break;
                        case "C":
                            ViewData["OrderStatus"] = "DELIVERED";
                            break;
                    }
                }
                else
                {
                    ordaline.Discount = 0;
                    if (item.Promotion.Trim() == "YES")
                    {
                        if (item.DicountType == "PERCENT                       ")
                        {
                            ordaline.Discount = (total * item.Discount) / 100;
                        }
                        else
                        {
                            ordaline.Discount = (qty * item.Discount);
                        }

                        total = total - (decimal)ordaline.Discount;
                    }
                    ordaline.item = item.ItemName;
                    ordaline.ItemCode = item.ItemCode;
                    ordaline.Category = item.category;
                    ordaline.SubCategory = item.SubCategory;
                    ordaline.quantity = qty;
                    ordaline.price = item.SellingPrice;
                    ordaline.Description = spec;
                    ordaline.priceinc = decimal.Parse(total.ToString());
                    ordaline.tax = 0;

                    ordaline.Dated = DateTime.Now;
                    ordaline.Company = item.company;

                    if (item.tax == "Taxed")
                    {
                        ordaline.tax = (total * decimal.Parse("0.15"));

                    }

                    var rand = new Random();
                    var num = rand.Next(0, gues.CustomerName.Length - 3);
                    orda.Account = gues.AccountCode;
                    orda.customer = gues.CustomerName;
                    orda.dated = DateTime.Now;
                    orda.discount = ordaline.Discount;
                    orda.Tax = ordaline.tax;
                    orda.total = decimal.Parse(total.ToString());
                    orda.state = "O";
                    if (item.category.Trim() == "AIRTIME")
                    {
                        orda.CollectionId = 2;
                        orda.DeliveryType = "Sms";
                    }
                    else
                    {
                        orda.CollectionId = 0;
                        orda.DeliveryType = "Delivery";
                    }
                    db.Orders.Add(orda);
                    db.SaveChanges();
                    orda.Reciept = gues.CustomerName.Substring(num, 3) + orda.ID + "." + DateTime.Now.TimeOfDay.ToString("hhmmss") + "." + DateTime.Now.ToString("ddMMyyyy");
                    db.Entry(orda).State = EntityState.Modified;
                    db.SaveChanges();

                    ordaline.Reciept = orda.Reciept;
                    db.OrderLines.Add(ordaline);
                    db.SaveChanges();
                    ViewData["OrderStatus"] = "OPEN";

                }

                var ols = new List<OrderLine>();
                ols = db.OrderLines.Where(u => u.Reciept == orda.Reciept).ToList();
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                var regionz = db.Regions.Find(cust.RegionId);

                if (Request.IsAuthenticated)
                {

                    var surburb = db.Suburbs.Find(long.Parse(cust.Suburb));
                    var city = db.Cities.Find(long.Parse(cust.City));
                    cust.Suburb = surburb.Name;
                    cust.City = city.Name;

                }

                var resp = new ShoppingCart();
                resp.orderlines = ols;
                resp.categories = categories;
                resp.customer = cust;
                resp.region = regionz;
                if (orda.CollectionId != 0)
                {
                    resp.collectxn = db.CollectionPoints.Find(orda.CollectionId);
                }

                if (resptyp == "Yes")
                {
                    return PartialView(resp);
                }
                else
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
            /*}
            else
            {
                return Json("Login", JsonRequestBehavior.AllowGet);
            }*/
        }

        //[Authorize]
        [HttpGet]
        public ActionResult AddToCartQ(long qty, long id, string spec, decimal total, string resptyp, string SessionId)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = db.Items.Find(id);
            var cust = new Customer();
            var gues = new Guest();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            if (part.Length > 0)
            {
                cust = db.Customers.Find(long.Parse(part[1]));
            }
           #region Guest
            else
            { 
                gues.CustomerName = "Guest";  
                gues.Username=SessionId;
                    
                    var pz = np.GetAllGuest();
                    var pp = (from e in pz  select e).LastOrDefault();
                   
                    Guest px = new Guest();
                    if (pp == null)
                    {
                        pp = new Guest();
                        px.AccountCode = "9-0001-0000001";
                    }
                    else
                    {
                        char[] delimita = new char[] { '-' };
                        string[] party = pp.AccountCode.Split(delimita, StringSplitOptions.RemoveEmptyEntries);
                        var Id = long.Parse(party[2]);
                        string acc = "";
                        var x = Id + 1;
                        if (x < 10)
                        {
                            acc = party[0] + "-" + party[1] + "-" + "000000" + x.ToString();
                        }
                        else if (x > 9 && x < 100)
                        {
                            acc = party[0] + "-" + party[1] + "-" + "00000" + x.ToString();
                        }
                        else if (x > 99 && x < 1000)
                        {
                            acc = party[0] + "-" + party[1] + "-" + "0000" + x.ToString();
                        }
                        else if (x > 999 && x < 10000)
                        {
                            acc = party[0] + "-" + party[1] + "-" + "000" + x.ToString();
                        }
                        else if (x > 9999 && x < 100000)
                        {
                            acc = party[0] + "-" + party[1] + "-" + "00" + x.ToString();
                        }
                        else if (x > 99999 && x < 1000000)
                        {
                            acc = party[0] + "-" + party[1] + "-" + "0" + x.ToString();
                        }
                        else if (x > 999999 && x < 10000000)
                        {
                            acc = party[0] + "-" + party[1] + "-" + x.ToString();
                        }
                        else if (x > 999999)
                        {
                            var Idd = long.Parse(part[1]);
                            var xx = Idd + 1;
                            if (xx < 10)
                            {
                                acc = party[0] + "-" + "000" + xx.ToString() + "-" + "0000001";
                            }
                            else if (xx > 9 && xx < 100)
                            {
                                acc = party[0] + "-" + "00" + xx.ToString() + "-" + "0000001";
                            }
                            else if (xx > 99 && xx < 1000)
                            {
                                acc = party[0] + "-" + "0" + xx.ToString() + "-" + "0000001";
                            }
                            else if (xx > 999 && xx < 10000)
                            {
                                acc = party[0] + "-" + xx.ToString() + "-" + "0000001";
                            }
                            else
                            {
                                acc = "Exhausted";
                            }
                        }
                        px.AccountCode = acc;
                        //gues.AccountCode = px.AccountCode;
                        px.Username = SessionId;
                       // Session["Username"] = SessionId;
                        px.CustomerName = "Guest";
                        // px.CompanyId = Company;
                        //cust.AccountCode = TempData["acc"].ToString();

                    }

                    if (ModelState.IsValid)
                    {
                        var CurrentGuest = User.Identity.Name;
                        char[] delimitar = new char[] { '~' };
                        string[] partx = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                        /*ST_encrypt en = new ST_encrypt();

                          string newAccount = en.encrypt( px.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
                          Random rr = new Random();
                          var pass = rr.Next(00000, 99999).ToString();
                          string Pin = en.encrypt(pass, "214ea9d5bda5277f6d1b3a3c58fd7034");*/

                        //   var uu = np.GetSyskeysByName("CustomerNetworking", part[1]);
                        //.Value.Trim();
                        string sk = "";
                        //  if (uu != null) sk = uu.Value;
                        var accs = db.Guests.Where(u => u.Username.Trim() == px.Username).ToList();
                        if (accs == null || accs.Count() == 0)
                        {

                            //px.Email = newAccount; 

                            // px.CustomerName =  px.CustomerName.ToUpper();
                            if (sk == "Yes")
                                px.ParentId = long.Parse(part[1]);
                            px.Balance = 0;
                            px.Wallet = 0;
                            px.Purchases = 0;
                            px.PurchasesToDate = 0;
                            //customer.Dated = DateTime.Now.ToString("dd-MM-yyyy");
                            np.SaveOrUpdateGuest(px);

                            // XmlCustomers cus = new XmlCustomers();
                            // cus.CreateCustomerProfile(customer);

                            return RedirectToAction("Index");
                            
                        }
                        else
                        {
                            ModelState.AddModelError("", "Sorry this customer name is already in use");
                        }
                    }

            }
           #endregion
            var result = "";
            //if (Request.IsAuthenticated)
            //{
            if (Request.IsAuthenticated)
            {
                #region Order
                if (item.Balance == 0)
                {
                    return Json("Stock", JsonRequestBehavior.AllowGet);
                }
                //cust.Username = SessionId;
                //// cust.Username = SessionId;

                //var pzr = np.GetAllCustomers();
                //var ppr = pzr.Where(u => u.Username == SessionId).FirstOrDefault();
                //cust.AccountCode = ppr.AccountCode;
                var order = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim()).FirstOrDefault();
                var ordaline = new OrderLine();
                var orda = new Order();

                if (order != null)
                {

                    var ordalin = db.OrderLines.Where(u => u.ItemCode.Trim() == item.ItemCode.Trim() & u.Reciept.Trim() == order.Reciept).FirstOrDefault();

                    if (ordalin != null)// if this item has already been added to the order
                    {
                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordalin.Discount = ordalin.Discount + (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordalin.Discount = ordalin.Discount + (qty * item.Discount);
                            }

                            order.discount = order.discount + ordalin.Discount;
                            total = total - (decimal)ordalin.Discount;

                        }
                        // ordaline = ordalin;
                        ordalin.quantity = ordalin.quantity + qty;
                        ordalin.priceinc = ordalin.priceinc + total;
                        ordalin.price = item.SellingPrice;

                        if (item.tax == "Taxed")
                        {
                            ordalin.tax = ordalin.tax + (total * decimal.Parse("0.15"));
                        }
                        if(!string.IsNullOrEmpty(item.AvailabilityInterval))
                        {
                            if (DateTime.Now > item.AvailabilityDate)
                            {
                                DateTime nextdate = DateTime.Now;
                                int dayspast = (DateTime.Now - item.AvailabilityDate).Value.Days;
                                switch (item.AvailabilityInterval.Trim())
                                {
                                    case "WEEKLY":
                                        int weekspast = dayspast / 7;
                                        if (weekspast == 0) weekspast = 1;
                                        nextdate = item.AvailabilityDate.Value.AddDays(weekspast * 7);
                                        //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                        item.AvailabilityDate = nextdate;
                                        break;
                                    case "FORTNIGHTLY":
                                        int weeks_past = dayspast / 14;
                                        if (weeks_past == 0) weeks_past = 1;
                                        nextdate = item.AvailabilityDate.Value.AddDays(weeks_past * 14);
                                        //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                        item.AvailabilityDate = nextdate;
                                        break;
                                    case "MONTHLY":
                                        int months = dayspast / 30;
                                        if (months == 0) months = 1;
                                        nextdate = item.AvailabilityDate.Value.AddDays(months * 30);
                                        //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                        item.AvailabilityDate = nextdate;
                                        break;
                                    case "QUATERLY":
                                        break;
                                    case "ANNUALY":
                                        int years = dayspast / 365;
                                        if (years == 0) years = 1;
                                        nextdate = item.AvailabilityDate.Value.AddDays(years * 365);
                                        //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                        item.AvailabilityDate = nextdate;
                                        break;
                                }
                            }
                            if (string.IsNullOrEmpty(order.Period))
                            {
                                order.Period = item.AvailabilityDate.ToString(); 
                            }
                            else
                            {
                                var dt = DateTime.Parse(order.Period);
                                if(dt < item.AvailabilityDate)
                                {
                                    order.Period = item.AvailabilityDate.ToString();
                                }

                            }
                        }

                        order.total = order.total + total;
                        db.Entry(ordalin).State = EntityState.Modified;
                        db.SaveChanges();

                    }
                    else// order is null
                    {
                        ordaline.Discount = 0;
                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordaline.Discount = (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordaline.Discount = (qty * item.Discount);
                            }
                            order.discount = order.discount + ordaline.Discount;
                            total = total - (decimal)ordaline.Discount;
                        }
                        ordaline.item = item.ItemName;
                        ordaline.ItemCode = item.ItemCode;
                        ordaline.Category = item.category;
                        ordaline.SubCategory = item.SubCategory;
                        ordaline.quantity = qty;
                        ordaline.price = item.SellingPrice;
                        ordaline.Description = spec;
                        ordaline.priceinc = decimal.Parse(total.ToString());
                        ordaline.tax = 0;

                        ordaline.Dated = DateTime.Now;
                        ordaline.Company = item.company;


                        if (item.tax == "Taxed")
                        {
                            ordaline.tax = (total * decimal.Parse("0.15"));
                            order.Tax = order.Tax + (total * decimal.Parse("0.15"));
                        }
                        if (item.category.Trim() != "AIRTIME" && order.CollectionId == 2)
                        {
                            order.CollectionId = 0;
                            order.DeliveryType = "Delivery";
                        }
                        order.total = order.total + total;
                        ordaline.Reciept = order.Reciept;
                        db.OrderLines.Add(ordaline);
                        db.SaveChanges();
                    }
                    // var pz = np.GetAllCustomers();
                    //var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                    // cust.AccountCode = pp.AccountCode;
                    if (!string.IsNullOrEmpty(item.AvailabilityInterval))
                    {
                        if (DateTime.Now > item.AvailabilityDate)
                        {
                            DateTime nextdate = DateTime.Now;
                            int dayspast = (DateTime.Now - item.AvailabilityDate).Value.Days;
                            switch (item.AvailabilityInterval.Trim())
                            {
                                case "WEEKLY":
                                    int weekspast = dayspast / 7;
                                    if (weekspast == 0) weekspast = 1;
                                    nextdate = item.AvailabilityDate.Value.AddDays(weekspast * 7);
                                    //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                    item.AvailabilityDate = nextdate;
                                    break;
                                case "FORTNIGHTLY":
                                    int weeks_past = dayspast / 14;
                                    if (weeks_past == 0) weeks_past = 1;
                                    nextdate = item.AvailabilityDate.Value.AddDays(weeks_past * 14);
                                    //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                    item.AvailabilityDate = nextdate;
                                    break;
                                case "MONTHLY":
                                    int months = dayspast / 30;
                                    if (months == 0) months = 1;
                                    nextdate = item.AvailabilityDate.Value.AddDays(months * 30);
                                    //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                    item.AvailabilityDate = nextdate;
                                    break;
                                case "QUATERLY":
                                    break;
                                case "ANNUALY":
                                    int years = dayspast / 365;
                                    if (years == 0) years = 1;
                                    nextdate = item.AvailabilityDate.Value.AddDays(years * 365);
                                    //int daysToAdd = ((int)nextdate.DayOfWeek - (int)nextdate.DayOfWeek + 7) % 7;
                                    item.AvailabilityDate = nextdate;
                                    break;
                            }
                        }
                        if (string.IsNullOrEmpty(order.Period))
                        {
                            order.Period = item.AvailabilityDate.ToString();
                        }
                        else
                        {
                            var dt = DateTime.Parse(order.Period);
                            if (dt < item.AvailabilityDate)
                            {
                                order.Period = item.AvailabilityDate.ToString();
                            }

                        }
                    }

                    var ordercart = db.OrderLines.Where(u => u.Reciept.Trim() == order.Reciept).Count();
                    ViewData["cartitem"] = ordercart;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    switch (order.state.Trim())
                    {
                        case "O":
                            ViewData["OrderStatus"] = "OPEN";
                            break;
                        case "A":
                            ViewData["OrderStatus"] = "AWAITING COLLECTION";
                            break;
                        case "AS":
                            ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                            break;
                        case "S":
                            ViewData["OrderStatus"] = "SHIPPED";
                            break;
                        case "C":
                            ViewData["OrderStatus"] = "DELIVERED";
                            break;
                    }
                }
                else
                {
                    ordaline.Discount = 0;
                    if (item.Promotion.Trim() == "YES")
                    {
                        if (item.DicountType == "PERCENT                       ")
                        {
                            ordaline.Discount = (total * item.Discount) / 100;
                        }
                        else
                        {
                            ordaline.Discount = (qty * item.Discount);
                        }

                        total = total - (decimal)ordaline.Discount;
                    }
                    ordaline.item = item.ItemName;
                    ordaline.ItemCode = item.ItemCode;
                    ordaline.Category = item.category;
                    ordaline.SubCategory = item.SubCategory;
                    ordaline.quantity = qty;
                    ordaline.price = item.SellingPrice;
                    ordaline.Description = spec;
                    ordaline.priceinc = decimal.Parse(total.ToString());
                    ordaline.tax = 0;

                    ordaline.Dated = DateTime.Now;
                    ordaline.Company = item.company;

                    if (item.tax == "Taxed")
                    {
                        ordaline.tax = (total * decimal.Parse("0.15"));

                    }

                    var rand = new Random();
                    var num = rand.Next(0, cust.CustomerName.Length - 3);
                    orda.Account = cust.AccountCode;
                    orda.customer = cust.CustomerName;
                    orda.dated = DateTime.Now;
                    orda.discount = ordaline.Discount;
                    orda.Tax = ordaline.tax;
                    orda.total = decimal.Parse(total.ToString());
                    orda.state = "O";
                    if (item.category.Trim() == "AIRTIME")
                    {
                        orda.CollectionId = 2;
                        orda.DeliveryType = "Sms";
                    }
                    else
                    {
                        orda.CollectionId = 0;
                        orda.DeliveryType = "Delivery";
                    }
                    db.Orders.Add(orda);
                    db.SaveChanges();
                    orda.Reciept = cust.CustomerName.Substring(num, 3) + orda.ID + "." + DateTime.Now.TimeOfDay.ToString("hhmmss") + "." + DateTime.Now.ToString("ddMMyyyy");
                    db.Entry(orda).State = EntityState.Modified;
                    db.SaveChanges();

                    ordaline.Reciept = orda.Reciept;
                    db.OrderLines.Add(ordaline);
                    db.SaveChanges();
                    ViewData["OrderStatus"] = "OPEN";

                }

                var ols = new List<OrderLine>();
                ols = db.OrderLines.Where(u => u.Reciept == orda.Reciept).ToList();
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                var regionz = db.Regions.Find(cust.RegionId);

                if (Request.IsAuthenticated)
                {
                    var surburb = db.Suburbs.Find(long.Parse(cust.Suburb));
                    var city = db.Cities.Find(long.Parse(cust.City));
                    cust.Suburb = surburb.Name;
                    cust.City = city.Name;

                }

                var resp = new ShoppingCart();
                resp.orderlines = ols;
                resp.categories = categories;
                resp.customer = cust;
                resp.region = regionz;
                if (orda.CollectionId != 0)
                {
                    resp.collectxn = db.CollectionPoints.Find(orda.CollectionId);
                }

                if (resptyp == "Yes")
                {
                    return PartialView(resp);
                }
                else
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
            else
            {
                #region OrderGuest
                if (item.Balance == 0)
                {
                    return Json("Stock", JsonRequestBehavior.AllowGet);
                }
                gues.Username = SessionId;
                // cust.Username = SessionId;

                var pzr = np.GetAllGuest();
                var ppr = pzr.Where(u => u.Username == SessionId).FirstOrDefault();
                gues.AccountCode = ppr.AccountCode;
                var order = db.Orders.Where(u => u.Account.Trim() == gues.AccountCode.Trim()).FirstOrDefault();
                var ordaline = new OrderLine();
                var orda = new Order();

                if (order != null)
                {

                    var ordalin = db.OrderLines.Where(u => u.ItemCode.Trim() == item.ItemCode.Trim() & u.Reciept.Trim() == order.Reciept).FirstOrDefault();

                    if (ordalin != null)// if this item has already been added to the order
                    {
                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordalin.Discount = ordalin.Discount + (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordalin.Discount = ordalin.Discount + (qty * item.Discount);
                            }

                            order.discount = order.discount + ordalin.Discount;
                            total = total - (decimal)ordalin.Discount;

                        }
                        // ordaline = ordalin;
                        ordalin.quantity = ordalin.quantity + qty;
                        ordalin.priceinc = ordalin.priceinc + total;
                        ordalin.price = item.SellingPrice;

                        if (item.tax == "Taxed")
                        {
                            ordalin.tax = ordalin.tax + (total * decimal.Parse("0.15"));

                        }

                        order.total = order.total + total;
                        db.Entry(ordalin).State = EntityState.Modified;
                        db.SaveChanges();



                    }
                    else// order is null
                    {
                        ordaline.Discount = 0;
                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordaline.Discount = (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordaline.Discount = (qty * item.Discount);
                            }
                            order.discount = order.discount + ordaline.Discount;
                            total = total - (decimal)ordaline.Discount;
                        }
                        ordaline.item = item.ItemName;
                        ordaline.ItemCode = item.ItemCode;
                        ordaline.Category = item.category;
                        ordaline.SubCategory = item.SubCategory;
                        ordaline.quantity = qty;
                        ordaline.price = item.SellingPrice;
                        ordaline.Description = spec;
                        ordaline.priceinc = decimal.Parse(total.ToString());
                        ordaline.tax = 0;

                        ordaline.Dated = DateTime.Now;
                        ordaline.Company = item.company;


                        if (item.tax == "Taxed")
                        {
                            ordaline.tax = (total * decimal.Parse("0.15"));
                            order.Tax = order.Tax + (total * decimal.Parse("0.15"));
                        }
                        if (item.category.Trim() != "AIRTIME" && order.CollectionId == 2)
                        {
                            order.CollectionId = 0;
                            order.DeliveryType = "Delivery";
                        }
                        order.total = order.total + total;
                        ordaline.Reciept = order.Reciept;
                        db.OrderLines.Add(ordaline);
                        db.SaveChanges();
                    }
                    // var pz = np.GetAllCustomers();
                    //var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                    // cust.AccountCode = pp.AccountCode;
                    var ordercart = db.OrderLines.Where(u => u.Reciept.Trim() == order.Reciept).Count();
                    ViewData["cartitem"] = ordercart;
                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    switch (order.state.Trim())
                    {
                        case "O":
                            ViewData["OrderStatus"] = "OPEN";
                            break;
                        case "A":
                            ViewData["OrderStatus"] = "AWAITING COLLECTION";
                            break;
                        case "AS":
                            ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                            break;
                        case "S":
                            ViewData["OrderStatus"] = "SHIPPED";
                            break;
                        case "C":
                            ViewData["OrderStatus"] = "DELIVERED";
                            break;
                    }
                }
                else
                {
                    ordaline.Discount = 0;
                    if (item.Promotion.Trim() == "YES")
                    {
                        if (item.DicountType == "PERCENT                       ")
                        {
                            ordaline.Discount = (total * item.Discount) / 100;
                        }
                        else
                        {
                            ordaline.Discount = (qty * item.Discount);
                        }

                        total = total - (decimal)ordaline.Discount;
                    }
                    ordaline.item = item.ItemName;
                    ordaline.ItemCode = item.ItemCode;
                    ordaline.Category = item.category;
                    ordaline.SubCategory = item.SubCategory;
                    ordaline.quantity = qty;
                    ordaline.price = item.SellingPrice;
                    ordaline.Description = spec;
                    ordaline.priceinc = decimal.Parse(total.ToString());
                    ordaline.tax = 0;

                    ordaline.Dated = DateTime.Now;
                    ordaline.Company = item.company;

                    if (item.tax == "Taxed")
                    {
                        ordaline.tax = (total * decimal.Parse("0.15"));

                    }

                    var rand = new Random();
                    var num = rand.Next(0, gues.CustomerName.Length - 3);
                    orda.Account = gues.AccountCode;
                    orda.customer = gues.CustomerName;
                    orda.dated = DateTime.Now;
                    orda.discount = ordaline.Discount;
                    orda.Tax = ordaline.tax;
                    orda.total = decimal.Parse(total.ToString());
                    orda.state = "O";
                    if (item.category.Trim() == "AIRTIME")
                    {
                        orda.CollectionId = 2;
                        orda.DeliveryType = "Sms";
                    }
                    else
                    {
                        orda.CollectionId = 0;
                        orda.DeliveryType = "Delivery";
                    }
                    db.Orders.Add(orda);
                    db.SaveChanges();
                    orda.Reciept = gues.CustomerName.Substring(num, 3) + orda.ID + "." + DateTime.Now.TimeOfDay.ToString("hhmmss") + "." + DateTime.Now.ToString("ddMMyyyy");
                    db.Entry(orda).State = EntityState.Modified;
                    db.SaveChanges();

                    ordaline.Reciept = orda.Reciept;
                    db.OrderLines.Add(ordaline);
                    db.SaveChanges();
                    ViewData["OrderStatus"] = "OPEN";

                }

                var ols = new List<OrderLine>();
                ols = db.OrderLines.Where(u => u.Reciept == orda.Reciept).ToList();
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                var regionz = db.Regions.Find(cust.RegionId);

                if (Request.IsAuthenticated)
                {
                    var surburb = db.Suburbs.Find(long.Parse(cust.Suburb));
                    var city = db.Cities.Find(long.Parse(cust.City));
                    cust.Suburb = surburb.Name;
                    cust.City = city.Name;
                }

                var resp = new ShoppingCart();
                resp.orderlines = ols;
                resp.categories = categories;
                resp.customer = cust;
                resp.region = regionz;
                if (orda.CollectionId != 0)
                {
                    resp.collectxn = db.CollectionPoints.Find(orda.CollectionId);
                }

                if (resptyp == "Yes")
                {
                    return PartialView(resp);
                }
                else
                {
                    return Json("Success", JsonRequestBehavior.AllowGet);
                }
                #endregion
            }
         
            /*}
            else
            {
                return Json("Login", JsonRequestBehavior.AllowGet);
            }*/
        }

        //[Authorize]
        [HttpGet]
        public ActionResult AdjustCart(long qty, string id, string spec, decimal total,string SessionId)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
           
            Item item = db.Items.Where(u => u.ItemCode == id).FirstOrDefault();
            var cust = new Customer();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var add = new DeliveryAddress();
            if (part.Length > 0)
            {
                cust = db.Customers.Find(long.Parse(part[1]));
            }
            else
            {
                cust.CustomerName = "Guest";
                cust.Username = SessionId;
               // cust.AccountCode = Session["accountcode"].ToString();

                //if (oda != null)
                //{
                //    ol = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept.Trim()).ToList();
                //}
                //var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                
                var pz = np.GetAllGuest();
                var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                cust.AccountCode = pp.AccountCode;
                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim()).FirstOrDefault();
                //List<OrderLine> ol = new List<OrderLine>();
                
                if (oda != null && oda.DeliveryType.Trim() == "Address")
                {
                    string studentPath = HttpContext.Server.MapPath("~/Content/AddressLists/");
                    studentPath = studentPath + SessionId + ".xml";
                    var colId = oda.CollectionId.ToString();
                    XDocument Adresses = XDocument.Load(studentPath);
                    var ads = (from e in Adresses.Descendants("Address")
                               where e.Element("Id").Value == colId
                               select e).FirstOrDefault();
                    add.Id = long.Parse(ads.Element("Id").Value);
                    add.RegionId = long.Parse(ads.Element("RegionId").Value);
                    add.SuburbId = ads.Element("SuburbId").Value;
                    add.CityId = ads.Element("CityId").Value;
                    add.City = ads.Element("City").Value;
                    add.Suburb = ads.Element("Suburb").Value;
                    add.Address = ads.Element("Address1").Value;
                    cust.RegionId = add.RegionId;
                    

                }
              //  cust.AccountCode = Session.SessionID;
           }
           
            var result = "";
           // if (Request.IsAuthenticated)
           // {
                #region sale
                if (item.Balance == 0)
                {
                    return Json("Stock", JsonRequestBehavior.AllowGet);
                }
               else if (item.Balance < qty)
                {
                    qty = Convert.ToInt64(item.Balance);
                    //item.Quantity = item.Quantity - qty;
                   // item.Balance = item.Balance - qty;
                   // item.sold = item.sold + qty;
                   // db.Entry(item).State = EntityState.Modified;
                   // db.SaveChanges();
                }
                /*   else if (item.Balance >= qty)
                  {
                      item.Quantity = item.Quantity - qty;
                      item.Balance = item.Balance - qty;
                      item.sold = item.sold + qty;
                      db.Entry(item).State = EntityState.Modified;
                      db.SaveChanges();
                  }*/
              
                var order = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim()).FirstOrDefault();
                var ordaline = new OrderLine();
                var orda = new Order();

                if (order != null)
                {

                    var ordalin = db.OrderLines.Where(u => u.ItemCode.Trim() == item.ItemCode.Trim() & u.Reciept.Trim() == order.Reciept).FirstOrDefault();
                    if (ordalin != null)// if this item has already been added to the order
                    {
                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordalin.Discount = ordalin.Discount + (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordalin.Discount = ordalin.Discount + (qty * item.Discount);
                            }

                            order.discount = order.discount + ordalin.Discount;
                            total = total - (decimal)ordalin.Discount;
                        }
                        // ordaline = ordalin;
                        ordalin.quantity = ordalin.quantity + qty;
                        if(qty < 0)
                        {
                            ordalin.priceinc = ordalin.priceinc - total;
                        }
                        else 
                        {
                            ordalin.priceinc = ordalin.priceinc + total;
                        }
                        ordalin.price = item.SellingPrice;

                        if (item.tax == "Taxed")
                        {
                            ordalin.tax = ordalin.tax + (total * decimal.Parse("0.15"));

                        }
                        if (qty < 0)
                        {
                            order.total = order.total - total;
                        }
                        else
                        {
                            order.total = order.total + total;
                        }

                        db.Entry(ordalin).State = EntityState.Modified;
                        db.SaveChanges();



                    }
                    else// order is null
                    {

                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordaline.Discount = (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordaline.Discount = (qty * item.Discount);
                            }
                            order.discount = order.discount + ordaline.Discount;
                            total = total - (decimal)ordaline.Discount;
                        }
                        ordaline.item = item.ItemName;
                        ordaline.ItemCode = item.ItemCode;
                        ordaline.quantity = qty;
                        ordaline.price = item.SellingPrice;
                        ordaline.Description = spec;
                        ordaline.priceinc = decimal.Parse(total.ToString());
                        ordaline.tax = 0;
                        ordaline.Discount = 0;
                        ordaline.Dated = DateTime.Now;


                        if (item.tax == "Taxed")
                        {
                            ordaline.tax = (total * decimal.Parse("0.15"));
                            order.Tax = order.Tax + (total * decimal.Parse("0.15"));
                        }
                        if(qty < 0)
                        {
                            order.total = order.total - total;
                        }
                        else 
                        { 
                            order.total = order.total + total;
                        }
                        ordaline.Reciept = order.Reciept;
                        db.OrderLines.Add(ordaline);
                        db.SaveChanges();
                    }

                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    switch (order.state.Trim())
                    {
                        case "O":
                            ViewData["OrderStatus"] = "OPEN";
                            break;
                        case "A":
                            ViewData["OrderStatus"] = "AWAITING COLLECTION";
                            break;
                        case "AS":
                            ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                            break;
                        case "S":
                            ViewData["OrderStatus"] = "SHIPPED";
                            break;
                        case "C":
                            ViewData["OrderStatus"] = "DELIVERED";
                            break;
                    }
                }
                else
                {

                    if (item.Promotion.Trim() == "YES")
                    {
                        if (item.DicountType == "PERCENT                       ")
                        {
                            ordaline.Discount = (total * item.Discount) / 100;
                        }
                        else
                        {
                            ordaline.Discount = (qty * item.Discount);
                        }

                        total = total - (decimal)ordaline.Discount;
                    }
                    ordaline.item = item.ItemName;
                    ordaline.ItemCode = item.ItemCode;
                    ordaline.quantity = qty;
                    ordaline.price = item.SellingPrice;
                    ordaline.Description = spec;
                    ordaline.priceinc = decimal.Parse(total.ToString());
                    ordaline.tax = 0;
                    ordaline.Discount = 0;
                    ordaline.Dated = DateTime.Now;

                    if (item.tax == "Taxed")
                    {
                        ordaline.tax = (total * decimal.Parse("0.15"));

                    }

                    var rand = new Random();
                    var num = rand.Next(0, cust.CustomerName.Length - 3);
                    orda.Account = cust.AccountCode;
                    orda.customer = cust.CustomerName;
                    orda.dated = DateTime.Now;
                    orda.discount = ordaline.Discount;
                    orda.Tax = ordaline.tax;
                    orda.total = decimal.Parse(total.ToString());
                    orda.state = "O";
                    orda.CollectionId = 0;
                    if(item.category == "AIRTIME")
                    {
                        orda.DeliveryType = "Sms";
                    }
                    else
                    {
                        orda.DeliveryType = "Delivery";
                    }
                   
                    db.Orders.Add(orda);
                    db.SaveChanges();
                    orda.Reciept = cust.CustomerName.Substring(num, 3) + orda.ID + "." + DateTime.Now.TimeOfDay.ToString("hhmmss") + "." + DateTime.Now.ToString("ddMMyyyy");
                    db.Entry(orda).State = EntityState.Modified;
                    db.SaveChanges();

                    ordaline.Reciept = orda.Reciept;
                    db.OrderLines.Add(ordaline);
                    db.SaveChanges();
                    ViewData["OrderStatus"] = "OPEN";

                }

                var ols = new List<OrderLine>();
                if (orda.Reciept == null) orda = order;
                ols = db.OrderLines.Where(u => u.Reciept.Trim() == orda.Reciept.Trim()).ToList();
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                var regionz = db.Regions.Find(cust.RegionId);
                var resp = new ShoppingCart();
                resp.orderlines = ols;
                resp.categories = categories;
                resp.customer = cust;
                resp.region = regionz;
                resp.order = orda;
                resp.Address = add;
                return PartialView("Cartitems", resp);
                #endregion
           /* }
            else
            {
                ViewData["Error"] = "Login to make a purchase";
                return Json("Login", JsonRequestBehavior.AllowGet);
            }*/
              
        }

      //  [Authorize]
        [HttpGet]
        public ActionResult Cartitems(string GSL)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            // if (Request.IsAuthenticated)
            //{
            #region sale
            var gues = new Guest();
            var cust = new Customer();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            
            if (part.Length > 0)
            {
                cust = db.Customers.Find(long.Parse(part[1]));
            }
            else
            {
                //var Sessionid = Session["Username"].ToString();
                cust.CustomerName = "Guest";
                cust.Username = GSL;
               // cust.Username = SessionId;
                
                var pz = np.GetAllGuest();
                var pp = pz.Where(u => u.Username == GSL).FirstOrDefault();
                if (pp != null)
                {
                    gues.AccountCode = pp.AccountCode;
                }
               
            }
               var oda = db.Orders.Where(u => u.Account.Trim() == gues.AccountCode.Trim()|| u.Account.Trim()==cust.AccountCode.Trim()).FirstOrDefault();
                List<OrderLine> ols = new List<OrderLine>();
                var add = new DeliveryAddress();
                if(oda != null)
                { 
                     ols = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept.Trim()).ToList();
                }
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                
                if (Request.IsAuthenticated)
                {
                    //var regionz = db.Regions.Find(cust.RegionId);
                    var surburb = db.Suburbs.Find(long.Parse(cust.Suburb));
                    var city = db.Cities.Find(long.Parse(cust.City));
                    cust.Suburb = surburb.Name;
                    cust.City = city.Name;

                    if (oda != null && oda.DeliveryType.Trim() == "Address")
                    {
                        string studentPath = HttpContext.Server.MapPath("~/Content/AddressLists/");
                        studentPath = studentPath + part[1] + ".xml";
                        var colId = oda.CollectionId.ToString();
                        XDocument Adresses = XDocument.Load(studentPath);
                        var ads = (from e in Adresses.Descendants("Address")
                                   where e.Element("Id").Value == colId
                                   select e).FirstOrDefault();
                        add.Id = long.Parse(ads.Element("Id").Value);
                        add.RegionId = long.Parse(ads.Element("RegionId").Value);
                        add.SuburbId = ads.Element("SuburbId").Value;
                        add.CityId = ads.Element("CityId").Value;
                        add.City = ads.Element("City").Value;
                        add.Suburb = ads.Element("Suburb").Value;
                        add.Address = ads.Element("Address1").Value;
                    }
                }
                else
                {
                   /* var surburb = db.Suburbs.Find(long.Parse(cust.Suburb));
                    var city = db.Cities.Find(long.Parse(cust.City));
                    cust.Suburb = surburb.Name;
                    cust.City = city.Name;*/

                    if (oda != null && oda.DeliveryType.Trim() == "Address")
                    {
                        string studentPath = HttpContext.Server.MapPath("~/Content/AddressLists/");
                        studentPath = studentPath + GSL + ".xml";
                        var colId = oda.CollectionId.ToString();
                        XDocument Adresses = XDocument.Load(studentPath);
                        var ads = (from e in Adresses.Descendants("Address")
                                   where e.Element("Id").Value == colId
                                   select e).FirstOrDefault();
                        add.Id = long.Parse(ads.Element("Id").Value);
                        add.RegionId = long.Parse(ads.Element("RegionId").Value);
                        add.SuburbId = ads.Element("SuburbId").Value;
                        add.CityId = ads.Element("CityId").Value;
                        add.City = ads.Element("City").Value;
                        add.Suburb = ads.Element("Suburb").Value;
                        add.Address = ads.Element("Address1").Value;
                        cust.RegionId = add.RegionId;
                        
                    }
                }
                var regionz = db.Regions.Find(cust.RegionId);
                var resp = new ShoppingCart();
                resp.orderlines = ols;
                resp.categories = categories;
                resp.customer = cust;
                resp.region = regionz;
                resp.order = oda;
                resp.Address = add;

                ViewData["OrderStatus"] = "NO ORDERS";
                if(oda != null )
                {
                    if(oda.CollectionId != 0) resp.collectxn = db.CollectionPoints.Find(oda.CollectionId);
                  ViewData["OrderNumber"] = oda.Reciept;

                  switch (oda.state.Trim())
                  {
                      case "O":
                          ViewData["OrderStatus"] = "OPEN";
                          break;
                      case "A":
                          ViewData["OrderStatus"] = "AWAITING COLLECTION";
                          break;
                      case "AS":
                          ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                          break;
                      case "S":
                          ViewData["OrderStatus"] = "SHIPPED";
                          break;
                      case "C":
                          ViewData["OrderStatus"] = "DELIVERED";
                          break;
                  }
                }
                #endregion
                if (Request.IsAjaxRequest()) return PartialView(resp);
                return View(resp);
           /*}
            else
            {
                return RedirectToAction("Index", "Home");
            }*/
        }

       // [Authorize]
        [HttpPost]
        public ActionResult Address(long CollectionId, string OrderId)
        {
            if (Request.IsAjaxRequest())
            {
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                
               // var cust = db.Customers.Find(long.Parse(part[1]));
                var order = db.Orders.Where(u => u.Reciept  == OrderId).FirstOrDefault();
                order.CollectionId = CollectionId;
                if (CollectionId == 0 )
                {
                    order.DeliveryType = "Delivery";
                }
                else 
                {
                    order.DeliveryType = "Collection";
                }
                
                order.CollectionId = CollectionId;
                
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return Json("Success", JsonRequestBehavior.AllowGet);
            }
            return Json("", JsonRequestBehavior.AllowGet);
        }

        public ActionResult ItemQuantity(JQueryDataTableParamModel param,long id = 0)
        {

            var today = DateTime.Now;
            var endday = DateTime.Now;
            if (param.sStart != null)
            {
                today = DateTime.Parse(param.sStart);
            }
            else
            {
                DateTime firstDay = new DateTime(today.Year, today.Month, 1);
                //today = firstDay;
                endday = firstDay.AddMonths(1).AddDays(-1);
            }

            if (param.sEcho != null && param.sEcho == "NextQuarter")
            {
                int month = today.Month - 1;
                int year = today.Year;
                int month2 = Math.Abs(month / 3) + 1;
                int nxt = month2 + 1;
                string mn = "";
                if (nxt > 4)
                {
                    year += 1;
                    mn = "01";
                }
                else
                {
                    mn = "0" + ((nxt * 3) - 2);

                }

                var dat = "01/" + mn + "/" + year + " 12:00 AM";
                today = DateTime.Parse(dat);
                endday = today.AddMonths(1).AddDays(-1);
            }
            else if (param.sEcho != null && param.sEcho == "PrevQuarter")
            {
                int month = today.Month - 1;
                int year = today.Year;
                int month2 = Math.Abs(month / 3) + 1;
                int prev = month2 - 1;
                string mn = "";
                if (prev < 1)
                {
                    prev = 4;
                    mn = "0" + ((prev * 3) - 2);
                    year -= 1;
                }
                else
                {
                    mn = "0" + ((prev * 3) - 2);
                }

                var dat = "01/" + mn + "/" + year + " 12:00 AM";
                today = DateTime.Parse(dat);
                endday = today.AddMonths(1).AddDays(-1);
            }
            else if (param.sEcho != null && param.sEcho == "NextMonth")
            {
                today = today.AddMonths(1);
                DateTime firstDay = new DateTime(today.Year, today.Month, 1);
                today = firstDay;
                endday = firstDay.AddMonths(1).AddDays(-1);
            }
            else if (param.sEcho != null && param.sEcho == "PrevMonth")
            {
                today = today.AddMonths(-1);
                DateTime firstDay = new DateTime(today.Year, today.Month, 1);
                today = firstDay;
                endday = firstDay.AddMonths(1).AddDays(-1);
            }

            ViewData["Today"] = today;
            ViewData["Id"] = id;

            var availability = new List<PurchaseLine>();
            Item item = db.Items.Find(id);
            var images = db.ItemImages.Where(u => u.ItemCode == item.ItemCode).ToList();
            var features = db.Features.Where(u => u.ItemCode == item.ItemCode).ToList();
            var itemvariations = db.ItemVariations.Where(u => u.ItemCode == item.ItemCode).ToList();
            if (item.AvailabilityMatrix != null && item.AvailabilityMatrix.Trim() == "TRUE")
            {
                availability = db.PurchaseLines.Where(u => u.ItemCode.Trim() == item.ItemCode.Trim() && u.Status.Trim() == "O" && (u.StartDate <= today && u.EndDate >= today && u.EndDate <= endday)).ToList();
                var regions = db.Regions.ToList();
                foreach(var dd in availability)
                {
                   var d = dd.DeshelvingDate.Value.DayOfWeek;
                   //var dow = dd.DeshelvingDate.Value
                    var r = regions.Where(u => u.DeliveryMatrix != null && u.DeliveryMatrix.Contains(d.ToString())).ToList();
                    List<SelectListItem> sb = new List<SelectListItem>();
                    CultureInfo cul = CultureInfo.CurrentCulture;
                    int weekNum = cul.Calendar.GetWeekOfYear(
                       dd.DeshelvingDate.Value,
                       CalendarWeekRule.FirstDay,
                       DayOfWeek.Sunday);
                    dd.Barcode = weekNum.ToString();
                    foreach (var itm in r)
                    {
                        var cpnts = db.CollectionPoints.Where(u => u.RegionId == itm.Id).ToList();
                        foreach (var cpnt in cpnts)
                        {
                            sb.Add(new SelectListItem
                            {
                                Text = cpnt.City.Trim() + "-" + cpnt.Name.Trim(),
                                Value = itm.Id.ToString()
                            });
                        }
                    }
                    ViewBag["POINTS"] = sb;
                    //dd.collectionPoint = sb;
                }
            }
            var recipeItems = new List<Recipe>();
            if (item.HasRecipe == true)
            {
                recipeItems = db.Recipes.Where(u => u.ProductCode == item.ItemCode && u.Company == item.company).ToList();
            }
            var related = new List<Item>();
            var spec = new List<Item>();
            var itms = db.Items.ToList();
            if (item.SubCategory == null)
            {
                related = itms.Where(u => u.category == item.category).Take(5).ToList();
                spec = itms.Where(u => u.category == item.category && u.Promotion.Trim() == "YES").ToList();
                if (spec == null) spec = itms.Where(u => u.Promotion.Trim() == "YES").ToList();
            }
            else
            {
                related = itms.Where(u => u.SubCategory == item.SubCategory).Take(5).ToList();
                spec = itms.Where(u => u.SubCategory == item.SubCategory && u.Promotion.Trim() == "YES").ToList();
                if (spec == null) spec = itms.Where(u => u.Promotion.Trim() == "YES").ToList();
            }

            var cat = db.Accounts.Where(u => u.AccountCode.StartsWith("3-") && u.CompanyId == 1).ToList();
            item.features = features;
            item.RelatedItems = related;
            item.ItemImages = images;
            item.categories = cat;
            item.SideSpecials = spec;
            item.ItemVariations = itemvariations;
            item.Availability = availability;
            item.RecipeItems = recipeItems;

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        public ActionResult Details(long id = 0)
        {
            Item item = db.Items.Find(id);
            var images = db.ItemImages.Where(u => u.ItemCode == item.ItemCode).ToList();
            var features = db.Features.Where(u => u.ItemCode == item.ItemCode).ToList();
            var itemvariations = db.ItemVariations.Where(u => u.ItemCode == item.ItemCode).ToList();
            var recipeItems = new List<Recipe>();
            var related = new List<Item>();
            var spec = new List<Item>();
            var itms = db.Items.ToList();
            if (item.SubCategory == null)
            {
                related = itms.Where(u => u.category == item.category).Take(5).ToList();
                spec = itms.Where(u => u.category == item.category && u.Promotion.Trim() == "YES").ToList();
                if (spec == null) spec = itms.Where(u => u.Promotion.Trim() == "YES").ToList();
            }
            else
            {
                related = itms.Where(u => u.SubCategory == item.SubCategory).Take(5).ToList();
                spec = itms.Where(u => u.SubCategory == item.SubCategory && u.Promotion.Trim() == "YES").ToList();
                if (spec == null) spec = itms.Where(u => u.Promotion.Trim() == "YES").ToList();
            }

            if(item.HasRecipe == true)
            {
                recipeItems = db.Recipes.Where(u => u.ProductCode == item.ItemCode && u.Company == item.company).ToList();
            }

            var cat = db.Accounts.Where(u => u.AccountCode.StartsWith("3-") && u.CompanyId == 1).ToList();
            item.features = features;
            item.RelatedItems = related;
            item.ItemImages = images;
            item.categories = cat;
            item.SideSpecials = spec;
            item.RecipeItems = recipeItems;
            item.ItemVariations = itemvariations;

            if (item == null)
            {
                return HttpNotFound();
            }
            return View(item);
        }

        [HttpGet]
       // [Authorize]
        public ActionResult ChangeAddress(string SessionId)
        {
             var customers = new Customer();
             Session["SessionId"] = SessionId;
                var nets = db.NetworkTypes.ToList();
                List<SelectListItem> cc = new List<SelectListItem>();

                foreach (var item in nets)
                {
                    cc.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewData["Success"] = "";
                ViewBag.Level = cc;
                if (nets == null)
                {
                    ViewData["NetwokFee"] = nets.FirstOrDefault().Fee.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
                }
                
                
                var cities = db.Cities.ToList();

                List<SelectListItem> ct = new List<SelectListItem>();
                ct.Add(new SelectListItem
                {
                    Text = "-Select City-",
                    Value = "-Select City-"
                });
                foreach (var item in cities)
                {
                    ct.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Cities = ct;
                var cid = cities.FirstOrDefault().Id;
                var surb = db.Suburbs.Where(u => u.CityId == cid).ToList();
                var area = db.Regions.Where(u => u.CityId == cid).ToList();
                List<SelectListItem> sb = new List<SelectListItem>();
                sb.Add(new SelectListItem
                {
                    Text = "-Select Suburb-",
                    Value = "-Select Suburb-"
                });
                foreach (var item in surb)
                {
                    sb.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Suburb = sb;

                List<SelectListItem> rg = new List<SelectListItem>();
                rg.Add(new SelectListItem
                {
                    Text = "-Select Region-",
                    Value = "-Select Region-"
                });
                foreach (var item in area)
                {
                    rg.Add(new SelectListItem
                    {
                        Text = item.Name,
                        Value = item.Id.ToString()
                    });
                }
                ViewBag.Area = rg;

                if (Request.IsAjaxRequest() == true) return PartialView();
            return View();
        }

        //[Authorize]
        [HttpPost]
        public ActionResult ChangeAddress(DeliveryAddress sh, Customer customer, string SessionId)
        {
            
            NHibernateDataProvider np = new NHibernateDataProvider();
            #region intialization
            var customers = new Customer();            
            var nets = db.NetworkTypes.ToList();
            List<SelectListItem> cc = new List<SelectListItem>();
           
            foreach (var item in nets)
            {
                cc.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewData["Success"] = "";
            ViewBag.Level = cc;
            if (nets == null)
            {
                ViewData["NetwokFee"] = nets.FirstOrDefault().Fee.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture);
            }


            var cities = db.Cities.ToList();

            List<SelectListItem> ct = new List<SelectListItem>();
            ct.Add(new SelectListItem
            {
                Text = "-Select City-",
                Value = "-Select City-"
            });
            foreach (var item in cities)
            {
                ct.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Cities = ct;
            var cid = cities.FirstOrDefault().Id;
            var surb = db.Suburbs.Where(u => u.CityId == cid).ToList();
            var area = db.Regions.Where(u => u.CityId == cid).ToList();
            List<SelectListItem> sb = new List<SelectListItem>();
            sb.Add(new SelectListItem
            {
                Text = "-Select Suburb-",
                Value = "-Select Suburb-"
            });
            foreach (var item in surb)
            {
                sb.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Suburb = sb;

            List<SelectListItem> rg = new List<SelectListItem>();
            rg.Add(new SelectListItem
            {
                Text = "-Select Region-",
                Value = "-Select Region-"
            });
            foreach (var item in area)
            {
                rg.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Area = rg;

#endregion
            if (sh.Address == "")
                {
                    ModelState.AddModelError("", "Please add Address");
                    return PartialView(sh);
                }
                if (sh.Phone1  == "")
                {
                    ModelState.AddModelError("", "Please add Phone number");
                    return PartialView(sh);
                }
                if (sh.Suburb == "")
                {
                    ModelState.AddModelError("", "Please select Suburb");
                    return PartialView(sh);
                }
                else
                {
                    var sub = db.Suburbs.Where(u => u.Name == sh.Suburb).FirstOrDefault();
                    if (sub == null)
                    {
                        ModelState.AddModelError("", "Please enter a valid suburb");
                        return PartialView(sh);
                    }
                    else
                    {
                        sh.Suburb = sub.Name.Trim();
                        sh.SuburbId = sub.Id.ToString();
                        sh.RegionId = sub.RegionId;
                        sh.CityId = sub.CityId.ToString();
                        
                    }
                }
                var CurrentUser = User.Identity.Name;
                var cust = new Customer();
                var gue = new Guest();
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                string studentPath = HttpContext.Server.MapPath("~/Content/AddressLists/");
                string Dirpath = studentPath;
                if (part.Length > 0)
                {
                   cust = db.Customers.Find(long.Parse(part[1]));                    
                    studentPath = studentPath + part[1] + ".xml";
                }
                else 
                {
                        
                    gue.CustomerName = "Guest";
                    sh.CustomerName = gue.CustomerName;
                    studentPath = studentPath + SessionId + ".xml";
                    
                    var pz = np.GetAllGuest();                   
                    var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                    gue.AccountCode = pp.AccountCode;
                    //var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                    //cust.AccountCode = pp.AccountCode;
                }
                if (!System.IO.Directory.Exists(Dirpath))
                {
                    Directory.CreateDirectory(Dirpath);                   
                }

                int lastid = 0;
                if (System.IO.File.Exists(studentPath))
                {
                    XDocument Adresses = XDocument.Load(studentPath);
                    var cnt = (from e in Adresses.Descendants("Address")
                                select e).ToList();

                    lastid = cnt.Count() + 1;

                    Adresses.Element("Addresses").Add(new XElement("Address",
                        new XElement("Id", lastid),
                        new XElement("Contact", sh.CustomerName),
                        new XElement("Address1", sh.Address1),
                        new XElement("CityId", sh.CityId),
                        new XElement("City", sh.City),
                        new XElement("SuburbId", sh.SuburbId),
                        new XElement("Suburb", sh.Suburb),
                        new XElement("RegionId", sh.RegionId),
                        new XElement("Phone", sh.Phone1)
                        ));

                    Adresses.Declaration = new XDeclaration("1.0", "utf-8", "true");
                    Adresses.Save(studentPath);

                }
                else
                {
                    lastid = 1;
                    XDocument Adresses = new XDocument(
                      new XComment("Address list"),
                      new XElement("Addresses",
                            new XElement("Address",
                            new XElement("Id",1),
                            new XElement("Contact", sh.CustomerName),
                            new XElement("Address1", sh.Address1),
                            new XElement("CityId", sh.CityId),
                            new XElement("City", sh.City),
                            new XElement("SuburbId", sh.SuburbId),
                            new XElement("Suburb", sh.Suburb),
                            new XElement("RegionId", sh.RegionId),
                            new XElement("Phone", sh.Phone1)
                          )));
                    Adresses.Declaration = new XDeclaration("1.0", "utf-8", "true");
                    Adresses.Save(studentPath);
                }

                var order = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim()|| u.Account.Trim() == gue.AccountCode.Trim()).FirstOrDefault();
                
                order.DeliveryType = "Address";
                order.CollectionId = lastid;
                db.Entry(order).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Cartitems", new { SessionId = SessionId });
            /*}
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet); ;
            }*/
               // return View();
        }

        [HttpGet]
      //[Authorize]
        public ActionResult AddressList()
        {
            var CurrentUser = User.Identity.Name;
            var cust = new Customer();
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (part.Length > 0)
            {
                cust = db.Customers.Find(long.Parse(part[1]));
                //var cust = db.Customers.Find(long.Parse(part[1]));
            }
            else
            {
                return RedirectToAction("ChangeAddress");
            }
            
            string studentPath = HttpContext.Server.MapPath("~/Content/AddressLists/");
            studentPath = studentPath + part[1] + ".xml";
            if (System.IO.File.Exists(studentPath))
            {
                XDocument Adresses = XDocument.Load(studentPath);
                var ads = (from e in Adresses.Descendants("Address")
                           select e).ToList();
                if (ads.Count == 0)
                {
                    return RedirectToAction("ChangeAddress");
                }

                List<DeliveryAddress> Alist = new List<DeliveryAddress>();
                foreach (var item in ads)
                {
                    DeliveryAddress d = new DeliveryAddress();
                    d.Id = long.Parse(item.Element("Id").Value);
                    d.Suburb = item.Element("Suburb").Value;
                    d.SuburbId = item.Element("SuburbId").Value;
                    d.City = item.Element("City").Value;
                    d.CityId = item.Element("CityId").Value;
                    d.RegionId = long.Parse(item.Element("RegionId").Value);
                    d.Address = item.Element("Address1").Value;
                   // d.Phone1 = item.Element("PhoneId").Value;
                    Alist.Add(d);
                }
                if (Request.IsAjaxRequest() == true) PartialView(Alist);
                return View(Alist);
            }
            else
            { 
                return RedirectToAction("ChangeAddress"); 
            }
           
        }

        [HttpGet]
        public ActionResult Orderlist(string SessionId)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var cust = new Customer();
            if(!Request.IsAuthenticated)
            {
                var gue = new Guest();
                var pz = np.GetAllGuest();
                var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                if (pp != null)
                {
                    gue.AccountCode = pp.AccountCode;
                }
                else 
                {
                    return Json(0, JsonRequestBehavior.AllowGet);
                }
                var oda = db.Orders.Where(u => u.Account.Trim() == gue.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                var ols = 0;
                if (oda != null)
                {
                    ols = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept.Trim()).Count();
                }


                return Json(ols, JsonRequestBehavior.AllowGet);
            }

            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;

                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                cust = db.Customers.Find(long.Parse(part[1]));
                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                var ols = 0;
                    if(oda != null)
                    {
                         ols = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept.Trim()).Count();
                    }
               

                return Json(ols, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet); 
            }
        }
       
        //[Authorize]
        [HttpGet]
        public ActionResult SaveList()
        {
            return View();
        }
        
        //[Authorize]
        [HttpPost]
        public ActionResult SaveList(shopingList sh, string SessionId)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var cust = new Customer();
            if (!Request.IsAuthenticated) 
            {
                var gue = new Guest();
                var pz = np.GetAllGuest();
                var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                gue.AccountCode = pp.AccountCode;
                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();

                if (oda != null)
                {
                    var ols = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept.Trim()).ToList();
                    sh.OrderNumber = oda.Reciept;
                    sh.CustomerId = cust.ID;
                    db.shopingLists.Add(sh);
                    db.SaveChanges();
                    string studentPath = HttpContext.Server.MapPath("~/Content/ShoppingLists/");
                    studentPath = studentPath + oda.Reciept.Trim() + ".xml";
                    XDocument shoppinglist = new XDocument(
                    new XComment("shopping list"),
                    new XElement("Items")
                    );

                    foreach (var item in ols)
                    {
                        shoppinglist.Element("Items").Add(new XElement("Item",
                         new XElement("ItemId", item.ItemCode),
                         new XElement("Quantity", item.quantity)
                         ));
                    }
                    shoppinglist.Declaration = new XDeclaration("1.0", "utf-8", "true");
                    shoppinglist.Save(studentPath);

                }

                return RedirectToAction("Cartitems");
            }
            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;

                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                cust = db.Customers.Find(long.Parse(part[1]));
                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
               
                if (oda != null)
                {
                    var ols = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept.Trim()).ToList() ;
                    sh.OrderNumber = oda.Reciept;
                    sh.CustomerId = cust.ID;
                    db.shopingLists.Add(sh);
                    db.SaveChanges();
                    string studentPath = HttpContext.Server.MapPath("~/Content/ShoppingLists/");
                    studentPath = studentPath + oda.Reciept.Trim() + ".xml";
                    XDocument shoppinglist = new XDocument(
                    new XComment("shopping list"),
                    new XElement("Items")
                    );

                      foreach(var item in ols)
                        {
                            shoppinglist.Element("Items").Add(new XElement("Item",
                             new XElement("ItemId", item.ItemCode),
                             new XElement("Quantity", item.quantity)
                             ));
                        }
                      shoppinglist.Declaration = new XDeclaration("1.0", "utf-8", "true");
                      shoppinglist.Save(studentPath);
                    
                }

                return RedirectToAction("Cartitems", new { SessionId = SessionId });
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet); ;
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult MyList()
        {
            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;

                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                var id =long.Parse(part[1]);
                var px = db.shopingLists.Where(u => u.CustomerId == id).ToList();
                return View(px);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpGet]
        public ActionResult LoadShoppingList(long Id)
        {
            if(Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;
                char[] delimita = new char[] { '~' };
                string[] parts = CurrentUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);

                var cust = db.Customers.Find(long.Parse(parts[1]));
                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                if (oda != null)
                {
                    var ols = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept.Trim()).ToList();

                    db.Orders.Remove(oda);
                    db.SaveChanges();
                    foreach (var item in ols)
                    {
                        db.OrderLines.Remove(item);
                        db.SaveChanges();
                    }
                }

                var shl = db.shopingLists.Find(Id);
           
                string studentPath = HttpContext.Server.MapPath("~/Content/ShoppingLists/");
                studentPath = studentPath + shl.OrderNumber.Trim() + ".xml";
                XDocument Itemz = XDocument.Load(studentPath);
                IList<XElement> listdata = (from e in Itemz.Descendants("Item")
                                           select e).ToList();
                foreach (var item in listdata)
                {
                    char[] delimiter = new char[] { '.' };
                    string[] part = item.Element("Quantity").Value.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    var qty = long.Parse(part[0]);

                    createListorder(qty, item.Element("ItemId").Value, "");
                }


                return RedirectToAction("Cartitems");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [Authorize]
        [HttpGet]
        public void createListorder(long qty, string id, string spec)
        {
            
            Item item = db.Items.Where(u=> u.ItemCode == id).FirstOrDefault();
            decimal total = decimal.Parse((qty * item.SellingPrice).ToString());
            var result = "";
            if (Request.IsAuthenticated)
            {
                if (item.Balance == 0)
                {
                    return;
                }
                else if (item.Balance < qty)
                {
                    qty = Convert.ToInt64(item.Balance);
                    item.Quantity = item.Quantity - qty;
                    item.Balance = item.Balance - qty;
                    item.sold = item.sold + qty;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                else if (item.Balance >= qty)
                {
                    item.Quantity = item.Quantity - qty;
                    item.Balance = item.Balance - qty;
                    item.sold = item.sold + qty;
                    db.Entry(item).State = EntityState.Modified;
                    db.SaveChanges();
                }
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);


                var cust = db.Customers.Find(long.Parse(part[1]));
                var order = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim()).FirstOrDefault();
                var ordaline = new OrderLine();
                var orda = new Order();

                if (order != null)
                {

                    var ordalin = db.OrderLines.Where(u => u.ItemCode.Trim() == item.ItemCode.Trim() & u.Reciept.Trim() == order.Reciept).FirstOrDefault();
                    if (ordalin != null)// if this item has already been added to the order
                    {
                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordalin.Discount = ordalin.Discount + (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordalin.Discount = ordalin.Discount + (qty * item.Discount);
                            }

                            order.discount = order.discount + ordalin.Discount;
                            total = total - (decimal)ordalin.Discount;
                        }
                        // ordaline = ordalin;
                        ordalin.quantity = ordalin.quantity + qty;
                        ordalin.priceinc = ordalin.priceinc + total;
                        ordalin.price = item.SellingPrice;

                        if (item.tax == "Taxed")
                        {
                            ordalin.tax = ordalin.tax + (total * decimal.Parse("0.15"));

                        }
                        order.total = order.total + total;
                        db.Entry(ordalin).State = EntityState.Modified;
                        db.SaveChanges();



                    }
                    else// order is null
                    {

                        if (item.Promotion.Trim() == "YES")
                        {
                            if (item.DicountType == "PERCENT                       ")
                            {
                                ordaline.Discount = (total * item.Discount) / 100;
                            }
                            else
                            {
                                ordaline.Discount = (qty * item.Discount);
                            }
                            order.discount = order.discount + ordaline.Discount;
                            total = total - (decimal)ordaline.Discount;
                        }
                        ordaline.item = item.ItemName;
                        ordaline.ItemCode = item.ItemCode;
                        ordaline.quantity = qty;
                        ordaline.price = item.SellingPrice;
                        ordaline.Description = spec;
                        ordaline.priceinc = decimal.Parse(total.ToString());
                        ordaline.tax = 0;
                        ordaline.Discount = 0;
                        ordaline.Dated = DateTime.Now;
                        ordaline.Company = item.company;


                        if (item.tax == "Taxed")
                        {
                            ordaline.tax = (total * decimal.Parse("0.15"));
                            order.Tax = order.Tax + (total * decimal.Parse("0.15"));
                        }
                        order.total = order.total + total;
                        ordaline.Reciept = order.Reciept;
                        db.OrderLines.Add(ordaline);
                        db.SaveChanges();
                    }

                    db.Entry(order).State = EntityState.Modified;
                    db.SaveChanges();
                    switch (order.state.Trim())
                    {
                        case "O":
                            ViewData["OrderStatus"] = "OPEN";
                            break;
                        case "A":
                            ViewData["OrderStatus"] = "AWAITING COLLECTION";
                            break;
                        case "AS":
                            ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                            break;
                        case "S":
                            ViewData["OrderStatus"] = "SHIPPED";
                            break;
                        case "C":
                            ViewData["OrderStatus"] = "DELIVERED";
                            break;
                    }
                }
                else
                {

                    if (item.Promotion.Trim() == "YES")
                    {
                        if (item.DicountType == "PERCENT                       ")
                        {
                            ordaline.Discount = (total * item.Discount) / 100;
                        }
                        else
                        {
                            ordaline.Discount = (qty * item.Discount);
                        }

                        total = total - (decimal)ordaline.Discount;
                    }
                    ordaline.item = item.ItemName;
                    ordaline.ItemCode = item.ItemCode;
                    ordaline.quantity = qty;
                    ordaline.price = item.SellingPrice;
                    ordaline.Description = spec;
                    ordaline.priceinc = decimal.Parse(total.ToString());
                    ordaline.tax = 0;
                    ordaline.Discount = 0;
                    ordaline.Dated = DateTime.Now;
                    ordaline.Company = item.company;

                    if (item.tax == "Taxed")
                    {
                        ordaline.tax = (total * decimal.Parse("0.15"));

                    }

                    var rand = new Random();
                    var num = rand.Next(0, cust.CustomerName.Length - 3);
                    orda.Account = cust.AccountCode;
                    orda.customer = cust.CustomerName;
                    orda.dated = DateTime.Now;
                    orda.discount = ordaline.Discount;
                    orda.Tax = ordaline.tax;
                    orda.total = decimal.Parse(total.ToString());
                    orda.state = "O";
                    orda.CollectionId = 0;
                    orda.DeliveryType = "Delivery";
                    db.Orders.Add(orda);
                    db.SaveChanges();
                    orda.Reciept = cust.CustomerName.Substring(num, 3) + orda.ID + "." + DateTime.Now.TimeOfDay.ToString("hhmmss") + "." + DateTime.Now.ToString("ddMMyyyy");
                    db.Entry(orda).State = EntityState.Modified;
                    db.SaveChanges();

                    ordaline.Reciept = orda.Reciept;
                    db.OrderLines.Add(ordaline);
                    db.SaveChanges();
                    ViewData["OrderStatus"] = "OPEN";

                }

                var ols = new List<OrderLine>();
                ols = db.OrderLines.Where(u => u.Reciept == orda.Reciept).ToList();
                var categories = db.Accounts.Where(u => u.AccountCode.StartsWith("3") && u.CompanyId == 1).ToList();
                var regionz = db.Regions.Find(cust.RegionId);
                var resp = new ShoppingCart();
                resp.orderlines = ols;
                resp.categories = categories;
                resp.customer = cust;
                resp.region = regionz;
                if (orda.CollectionId != 0)
                {
                    resp.collectxn = db.CollectionPoints.Find(orda.CollectionId);
                }

               

            }
            else
            {
                ViewData["Error"] = "Login to make a purchase";
                return ;
            }
        }

        //[Authorize]
        [HttpGet]
        public ActionResult PayNow(string Id, string SessionId)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            try
            {
                var cust = new Customer();
                var add = new DeliveryAddress();
                var gues = new Guest();
                var CurrentUser = User.Identity.Name;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                if (part.Length > 0) 
                {
                    cust = db.Customers.Find(long.Parse(part[1]));
                }
                if (!Request.IsAuthenticated)
                {
                   
                    var pz = np.GetAllCustomers();
                    var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                    gues.AccountCode = pp.AccountCode; 
                    var odar = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim()).FirstOrDefault();
                    if (odar != null && odar.DeliveryType.Trim() == "Address")
                    {
                        string studentPath = HttpContext.Server.MapPath("~/Content/AddressLists/");
                        studentPath = studentPath + SessionId + ".xml";
                        var colId = odar.CollectionId.ToString();
                        XDocument Adresses = XDocument.Load(studentPath);
                        var ads = (from e in Adresses.Descendants("Address")
                                   where e.Element("Id").Value == colId
                                   select e).FirstOrDefault();
                        add.Id = long.Parse(ads.Element("Id").Value);
                        add.RegionId = long.Parse(ads.Element("RegionId").Value);
                        add.SuburbId = ads.Element("SuburbId").Value;
                        add.CityId = ads.Element("CityId").Value;
                        add.City = ads.Element("City").Value;
                        add.Suburb = ads.Element("Suburb").Value;
                        add.Address = ads.Element("Address1").Value;
                        cust.RegionId = add.RegionId;

                    }
                }
                
                   
                

                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                decimal? totl = 0;
                var regionz = db.Regions.Find(cust.RegionId);

                if (oda.DeliveryType.Trim() == "Delivery")
                {
                    totl = oda.total + regionz.DeliveryRate;
                }
                else if(oda.DeliveryType.Trim()=="Address")
                {
                    totl = oda.total + regionz.DeliveryRate;
                }
                else
                {
                    totl = oda.total;
                }
                 ST_decrypt dc = new ST_decrypt();
                 if (!Request.IsAuthenticated)
                 {
                    // var oo = dc.st_decrypt(cust.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
                     string resulturl = "http:/www.baskiti.com/cart/PayStatus?action=status&id=" + Id;
                     string returnurl = "http:/www.baskiti.com/cart/PayResult?action=result&id=" + Id;
                     string reference = Id;
                     string amount = totl.ToString();
                     string id = "1939";
                     string additionalinfo = "baskiti purchase";
                     //string authemail = oo;
                     string status = "Message";
                     string merchant_key = "19192553-83b7-4c5b-84f0-a0c546d5642b";

                     var str = resulturl + returnurl + reference + amount + id + additionalinfo + status;
                     var hash = GenerateTwoWayHash(str, merchant_key).ToUpper();

                     string rBrowseUrl = "";
                     string rPolUrl = "";
                     string rhash = "";
                     using (WebClient client = new WebClient())
                     {
                         NameValueCollection data = new NameValueCollection();

                         data.Add("resulturl", resulturl);
                         data.Add("returnurl", returnurl);
                         data.Add("reference", reference);// Id;
                         data.Add("amount", amount);
                         data.Add("id", id);
                         data.Add("additionalinfo", additionalinfo);
                         //data.Add("authemail", authemail);
                         data.Add("status", status);
                         data.Add("hash", hash);

                         string result = Encoding.UTF8.GetString(client.UploadValues("https://www.paynow.co.zw/interface/initiatetransaction", "POST", data));
                         char[] delimita = new char[] { '&' };
                         string[] partz = result.Split(delimita, StringSplitOptions.RemoveEmptyEntries);
                         char[] delimit = new char[] { '=' };
                         string[] parts = partz[0].Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                         string rstatus = parts[1];

                         if (rstatus == "Ok")
                         {
                             parts = partz[1].Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                             rBrowseUrl = Server.UrlDecode(parts[1]);

                             parts = partz[2].Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                             rPolUrl = Server.UrlDecode(parts[1]);

                             parts = partz[3].Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                             rhash = parts[1];
                             var pp = new Payment();
                             pp.Amount = decimal.Parse(amount);
                             //pp.CustomerId = long.Parse(part[1]);
                             pp.Polls = rPolUrl;
                             pp.ReceiptNo = Id;
                             pp.Source = "Paynow";
                             pp.DateCreated = DateTime.Now;
                             pp.Status = "Payment Request";
                             db.Payments.Add(pp);
                             db.SaveChanges();
                         }
                         else
                         {
                             return RedirectToAction("Payment", new { id = "Error", typ = "PaymentGateway" });
                         }

                     }

                     ViewData["Page"] = rBrowseUrl;
                     ViewData["Poll"] = rPolUrl;

                     return Redirect(rBrowseUrl);
                 }
                 else 
                 {
                     var oo = dc.st_decrypt(cust.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");
                     string resulturl = "http:/www.baskiti.com/cart/PayStatus?action=status&id=" + Id;
                     string returnurl = "http:/www.baskiti.com/cart/PayResult?action=result&id=" + Id;
                     string reference = Id;
                     string amount = totl.ToString();
                     string id = "1939";
                     string additionalinfo = "baskiti purchase";
                     string authemail = oo;
                     string status = "Message";
                     string merchant_key = "19192553-83b7-4c5b-84f0-a0c546d5642b";

                     var str = resulturl + returnurl + reference + amount + id + additionalinfo + authemail + status;
                     var hash = GenerateTwoWayHash(str, merchant_key).ToUpper();

                     string rBrowseUrl = "";
                     string rPolUrl = "";
                     string rhash = "";
                     using (WebClient client = new WebClient())
                     {
                         NameValueCollection data = new NameValueCollection();

                         data.Add("resulturl", resulturl);
                         data.Add("returnurl", returnurl);
                         data.Add("reference", reference);// Id;
                         data.Add("amount", amount);
                         data.Add("id", id);
                         data.Add("additionalinfo", additionalinfo);
                         data.Add("authemail", authemail);
                         data.Add("status", status);
                         data.Add("hash", hash);

                         string result = Encoding.UTF8.GetString(client.UploadValues("https://www.paynow.co.zw/interface/initiatetransaction", "POST", data));
                         char[] delimita = new char[] { '&' };
                         string[] partz = result.Split(delimita, StringSplitOptions.RemoveEmptyEntries);
                         char[] delimit = new char[] { '=' };
                         string[] parts = partz[0].Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                         string rstatus = parts[1];

                         if (rstatus == "Ok")
                         {
                             parts = partz[1].Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                             rBrowseUrl = Server.UrlDecode(parts[1]);

                             parts = partz[2].Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                             rPolUrl = Server.UrlDecode(parts[1]);

                             parts = partz[3].Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                             rhash = parts[1];
                             var pp = new Payment();
                             pp.Amount = decimal.Parse(amount);
                             pp.CustomerId = long.Parse(part[1]);
                             pp.Polls = rPolUrl;
                             pp.ReceiptNo = Id;
                             pp.Source = "Paynow";
                             pp.DateCreated = DateTime.Now;
                             pp.Status = "Payment Request";
                             db.Payments.Add(pp);
                             db.SaveChanges();
                         }
                         else
                         {
                             return RedirectToAction("Payment", new { id = "Error", typ = "PaymentGateway" });
                         }

                     }

                     ViewData["Page"] = rBrowseUrl;
                     ViewData["Poll"] = rPolUrl;

                     return Redirect(rBrowseUrl);
                 }
                   
            }

            catch (Exception g)
            {
            }

            return RedirectToAction("Payment", new { id = "Error" });
        }

    
        [Authorize]
        [HttpGet]
        public ActionResult Wallet(string Id)
        {
            try
            {
                var CurrentUser = User.Identity.Name;

                decimal? totl = 0;
                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                var cust = db.Customers.Find(long.Parse(part[1]));
               
                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                
                var regionz = db.Regions.Find(cust.RegionId);
                if (oda.DeliveryType.Trim() == "Delivery")
                {
                    totl = oda.total + regionz.DeliveryRate;
                }
                else
                {
                    totl = oda.total;
                }
                if (cust.Wallet >= totl)
                {
                    cust.Wallet = cust.Wallet - totl;
                    var pp = new Payment();
                    pp.Amount = totl;
                    pp.CustomerId = long.Parse(part[1]); 
                    pp.ReceiptNo = Id;
                    pp.Source = "Wallet";
                    pp.DateCreated = DateTime.Now;
                    pp.Status = "Paid";
                    db.Payments.Add(pp);
                    db.SaveChanges();

                    db.Entry(cust).State = EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Dispatch", new { id = Id });
                }
                else
                {
                   string respns = "Sorry your wallet balance is not enough for the transaction";
                   return RedirectToAction("Payment", new { id = respns, typ = "PaymentWallet"});
                }

            }

            catch (Exception g)
            {
            }

            return RedirectToAction("Payment", new { id = "Error" });
        }

       
        [HttpGet]
        public ActionResult PayResult(string action, string id,PayResponse resp)
        {
            try
            {
                Thread.Sleep(5000);
                var pp = db.Payments.Where(u => u.ReceiptNo.Trim() == id).FirstOrDefault();
                using (WebClient client = new WebClient())
                {
                    NameValueCollection data = new NameValueCollection();
                    int cnt =0;
                    string result = Encoding.UTF8.GetString(client.UploadValues(pp.Polls.Trim(), "POST", data));
                    char[] delimita = new char[] { '&' };
                    string[] partz = result.Split(delimita, StringSplitOptions.RemoveEmptyEntries);
                    foreach(var item in partz )
                    {
                        char[] delimit = new char[] { '=' };
                        string[] parts =item.Split(delimit, StringSplitOptions.RemoveEmptyEntries);
                        if (cnt == 1) { pp.SourceRefrence = parts[1]; }
                        if (cnt == 3) { pp.Status = parts[1]; }
                        cnt += 1;
                    }
                
                    db.Entry(pp).State = EntityState.Modified;
                    db.SaveChanges();
                    if (pp.Status.Trim() == "Paid" || pp.Status.Trim() == "Created" || pp.Status.Trim() == "Sent")
                    {
                        return RedirectToAction("Dispatch", new { id = id });
                    }
                    if (pp.Status.Trim() == "Cancelled")
                    {
                        return RedirectToAction("Payment", new { id = "Your transaction has been cancelled" });
                    }
                    else
                    {
                        return RedirectToAction("Payment", new { id = "Error" });
                    }

                }
            }
            catch (Exception g)
            {
            }
            return RedirectToAction("Payment", new { id = "Error" });
        }

       [HttpPost]
        public ActionResult PayStatus(string action, string id, PayResponse resp)
        {
            NameValueCollection nvc = Request.Form;

            if (!string.IsNullOrEmpty(nvc["reference"]))
            {
                resp.reference = nvc["reference"];
            }
            if (!string.IsNullOrEmpty(nvc["status"]))
            {
                resp.status = nvc["status"];
            }
            if (!string.IsNullOrEmpty(nvc["Paynowreference"]))
            {
                resp.Paynowreference = nvc["Paynowreference"];
            }
            if (!string.IsNullOrEmpty(nvc["pollurl"]))
            {
                resp.pollurl = nvc["pollurl"];
            }
            if (!string.IsNullOrEmpty(nvc["amount"]))
            {
                resp.amount = nvc["amount"];
            }
            if (!string.IsNullOrEmpty(nvc["Hash"]))
            {
                resp.Hash = nvc["Hash"];
            }

            if (action == "status")
            {
                ViewData["Page"] = resp.reference + "&" + resp.status+ "&" + action;
                return View("PayNow");
            }
            else
            {
                ViewData["Page"] = resp.reference + "&" + resp.status + "&" + action;
                return View("PayNow");
            }
        }

        [HttpGet]
        public ActionResult Payment(string OrderId, string SessionId, string id = "init", string typ = "init")
        {
            //var pz = np.GetAllGuest();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var cust = new Customer();
            var gues = new Guest();
            long regionId = 0;
            string accCode = "";
            ViewData["Sessionid"] = SessionId;
            ViewData["Status"] = id;
            ViewData["Type"] = typ;
            ViewData["OrderNumber"] = OrderId;
            if (!Request.IsAuthenticated)
            {
                var pp = db.Guests.Where(u => u.Username == SessionId).FirstOrDefault();
                accCode = pp.AccountCode;
                
            }else
            {
                var CurrentUser = User.Identity.Name;

                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                cust = db.Customers.Find(long.Parse(part[1]));
                accCode = cust.AccountCode;
                regionId = (long)cust.RegionId;
            }
            var oda = db.Orders.Where(u => u.Reciept == OrderId.Trim() && u.Account == accCode).FirstOrDefault();
            if(oda.DeliveryType.Trim() == "Collection")
            {
                var cp = db.CollectionPoints.Find(oda.CollectionId);
                regionId = (long)cp.RegionId;
            }
            var regionz = db.Regions.Find(regionId);
            if (oda.DeliveryType != null && oda.DeliveryType.Trim() == "Delivery")
            {
                if (regionz.DeliveryRate != null)
                {
                    ViewData["Total"] = oda.total + regionz.DeliveryRate;
                }
                else
                {
                    ViewData["Total"] = oda.total;
                }
            }
            else
            {
                ViewData["Total"] = oda.total;
            }
            ViewData["OrderNumber"] = oda.Reciept;
               YomoneyController dt = new YomoneyController();
                List<YomoneyResponse> tr = new List<YomoneyResponse>();
                switch (oda.state.Trim())
                {
                    case "O":
                        ViewData["OrderStatus"] = "OPEN";
                        break;
                    case "A":
                        ViewData["OrderStatus"] = "AWAITING COLLECTION";
                        break;
                    case "AS":
                        ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                        break;
                    case "S":
                        ViewData["OrderStatus"] = "SHIPPED";
                        break;
                    case "C":
                        ViewData["OrderStatus"] = "DELIVERED";
                        break;
                }

                var Store = db.Syskeys.Where(u => u.Name == "OnlineStore").FirstOrDefault();
                var agent =  db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "YomoneyAgentCode" ).FirstOrDefault().Value;
                var password = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "AgentPassword").FirstOrDefault().Value;
                
                YomoneyRequest yr = new YomoneyRequest();
                yr.MTI = "0300";
                yr.Amount = (decimal)oda.total;
                yr.ProcessingCode = "420000";
                yr.TransactionType = 5;
                yr.AgentCode = agent.Trim() + ":" + password.Trim();
            
                var list = dt.Payment(yr);
                var xx = new List<PaymentType>();
                if (list.Narrative != null)
                {
                   var  mm = JsonConvert.DeserializeObject<List<MobileMenu>>(list.Narrative);
                    
                     foreach(var py in mm)
                     {
                        var itm = new PaymentType();
                        itm.Name = py.Name;
                        itm.trantype = py.TransactionType.ToString();
                        itm.ID = py.ServiceId;
                        itm.ProviderAccount = py.SupplierId;
                        xx.Add(itm);
                     }
                 }
                else
                {
                    var jlist = JsonConvert.SerializeObject(list);
                    return PartialView("EcoMsg", jlist);
                }
            
               // ViewData["Amount"] = amount;
                //ViewData["type"] = type;
               // ViewData["Curr"] = currency;
                return PartialView("Payment", xx);
        }
        //[Authorize]
        /* [HttpGet]
          public ActionResult Payment(string SessionId, string id="init", string typ = "init")
          {
              ViewData["Sessionid"] = SessionId;
              ViewData["Status"] = id;
              ViewData["Type"] = typ;
              NHibernateDataProvider np = new NHibernateDataProvider();
              var cust = new Customer();
              var gues = new Guest();
              if (!Request.IsAuthenticated)
             {
                 var pz = np.GetAllGuest();
                 var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                 gues.AccountCode = pp.AccountCode;
                 var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim()|| u.Account.Trim() == gues.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                 var regionz = db.Regions.Find(cust.RegionId);
                 if (oda.DeliveryType != null && oda.DeliveryType.Trim() == "Delivery")
                 {
                     if (regionz != null)
                     {
                         ViewData["Total"] = oda.total + regionz.DeliveryRate;
                     }
                     else 
                     {
                         ViewData["Total"] = oda.total;
                     }
                 }
                 else
                 {
                     ViewData["Total"] = oda.total;
                 }
                 ViewData["OrderNumber"] = oda.Reciept;
                 switch (oda.state.Trim())
                 {
                     case "O":
                         ViewData["OrderStatus"] = "OPEN";
                         break;
                     case "A":
                         ViewData["OrderStatus"] = "AWAITING COLLECTION";
                         break;
                     case "AS":
                         ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                         break;
                     case "S":
                         ViewData["OrderStatus"] = "SHIPPED";
                         break;
                     case "C":
                         ViewData["OrderStatus"] = "DELIVERED";
                         break;
                 }
                 var xx = db.PaymentTypes.ToList();
                 return View(xx);
             }
              else 
              {

                  var CurrentUser = User.Identity.Name;

                  char[] delimiter = new char[] { '~' };
                  string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                  cust = db.Customers.Find(long.Parse(part[1]));
                  var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                  var regionz = db.Regions.Find(cust.RegionId);
                  if (oda.DeliveryType != null && oda.DeliveryType.Trim() == "Delivery")
                  {
                      if (regionz.DeliveryRate != null)
                      {
                          ViewData["Total"] = oda.total + regionz.DeliveryRate;
                      }
                      else {
                          ViewData["Total"] = oda.total;
                              }
                  }
                  else
                  {
                      ViewData["Total"] = oda.total;
                  }
                  ViewData["OrderNumber"] = oda.Reciept;
                  switch (oda.state.Trim())
                  {
                      case "O":
                          ViewData["OrderStatus"] = "OPEN";
                          break;
                      case "A":
                          ViewData["OrderStatus"] = "AWAITING COLLECTION";
                          break;
                      case "AS":
                          ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                          break;
                      case "S":
                          ViewData["OrderStatus"] = "SHIPPED";
                          break;
                      case "C":
                          ViewData["OrderStatus"] = "DELIVERED";
                          break;
                  }
                  var xx = db.PaymentTypes.ToList();
                  return View(xx);
              }
              else
              {
                  return RedirectToAction("Index", "Home");
              }
          }
          */

        [HttpPost]
        public ActionResult YomoneyPayment(YomoneyRequest yr, string SessionId)
        {
            #region trytoPay
           
            decimal? amnt = 0;
           var oda = db.Orders.Where(u => u.Reciept.Trim() == yr.TransactionRef.Trim()).FirstOrDefault();
            if (Request.IsAuthenticated)
            {
                try
                {
                    
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    var cust = db.Customers.Find(long.Parse(part[1]));
                    var regionz = db.Regions.Find(cust.RegionId);
                    if (oda.DeliveryType.Trim() == "Delivery")
                    {
                        amnt = oda.total + regionz.DeliveryRate;
                    }
                    else
                    {
                        amnt = oda.total;
                    }

                }
                catch
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
                }
            else
            {
                if (!string.IsNullOrEmpty(SessionId))
                {
                   var guest = db.Guests.Where(u => u.Username.Trim() == SessionId).FirstOrDefault();
                    var regionz = db.Regions.Find(guest.RegionId);
                    if (oda.DeliveryType.Trim() == "Delivery")
                    {
                        amnt = oda.total + regionz.DeliveryRate;
                    }
                    else
                    {
                        amnt = oda.total;
                    }
                }
                else
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
            var Store = db.Syskeys.Where(u => u.Name == "OnlineStore").FirstOrDefault();
            var agent = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "YomoneyAgentCode").FirstOrDefault().Value;
            var password = db.Syskeys.Where(u => u.Company == Store.Company && u.Name == "AgentPassword").FirstOrDefault().Value;

            yr.MTI = "0200";
            yr.Amount = (decimal)amnt;
            yr.ProcessingCode = "320000";
            yr.TransactionType = 5;
            yr.AgentCode = agent.Trim() + ":" + password.Trim();

            YomoneyController ym = new YomoneyController();
            var resp = ym.Payment(yr);

            #endregion
            if (resp.ResponseCode == "00000")
            {            
               return RedirectToAction("Dispatch");
            }
            else
            {
                return Json("Pay_Error", JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult WalletPayment(string id, string SessionId)
        {
            var cust = new Customer();
            if(SessionId != null && SessionId != "")
            {
                try
                {
                    decimal? amnt = 0;                    
                    var regionz = db.Regions.Find(cust.RegionId);
                    var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                    var xx = db.Payments.Where(u => u.SourceRefrence.Trim() == id && u.Status.Trim() == "O").FirstOrDefault();
                    if (oda == null) return Json("Order", JsonRequestBehavior.AllowGet);
                    if (xx != null)
                    {
                        if (oda.DeliveryType.Trim() == "Delivery")
                        {
                            amnt = oda.total + regionz.DeliveryRate;
                        }
                        else
                        {
                            amnt = oda.total;
                        }
                        if (xx.Amount >= oda.total)
                        {
                            if (xx.Amount > oda.total) cust.Wallet = xx.Amount - amnt;
                            xx.Status = "C";
                            db.Entry(xx).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Dispatch");
                        }
                        else
                        {
                            if ((cust.Wallet + xx.Amount) >= oda.total)
                            {
                                if ((cust.Wallet + xx.Amount) > oda.total) cust.Wallet = (cust.Wallet + xx.Amount) - amnt;
                                db.Entry(cust).State = EntityState.Modified;
                                db.SaveChanges();
                                xx.Status = "C";
                                db.Entry(xx).State = EntityState.Modified;
                                db.SaveChanges();
                                return RedirectToAction("Dispatch");
                            }
                            else
                            {
                                cust.Wallet = cust.Wallet + xx.Amount;
                                db.Entry(cust).State = EntityState.Modified;
                                db.SaveChanges();
                                xx.Status = "C";

                                db.Entry(xx).State = EntityState.Modified;
                                db.SaveChanges();

                                return Json("Partial", JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                        return Json("No", JsonRequestBehavior.AllowGet);
                    }

                }
                catch (Exception)
                {
                    return Json("Error", JsonRequestBehavior.AllowGet);
                }
            }
            NHibernateDataProvider np = new NHibernateDataProvider();
            var pz = np.GetAllCustomers();
            var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
            cust.AccountCode = pp.AccountCode;
            if(Request.IsAuthenticated)
            {
                try 
                {
                    decimal? amnt = 0;
                    var CurrentUser = User.Identity.Name;
                    char[] delimiter = new char[] { '~' };
                    string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                    cust = db.Customers.Find(long.Parse(part[1]));
                    var regionz = db.Regions.Find(cust.RegionId);
                    var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                    var xx = db.Payments.Where(u=> u.SourceRefrence.Trim() == id && u.Status.Trim() =="O").FirstOrDefault();
                    if (oda == null) return Json("Order", JsonRequestBehavior.AllowGet);
                    if (xx != null)
                    {
                        if (oda.DeliveryType.Trim() == "Delivery")
                        {
                            amnt = oda.total + regionz.DeliveryRate;
                        }else
                        {
                            amnt = oda.total;
                        }
                        if(xx.Amount >= oda.total)
                        {
                            if (xx.Amount > oda.total) cust.Wallet =  xx.Amount - amnt;
                            xx.Status ="C";
                            db.Entry(xx).State = EntityState.Modified;
                            db.SaveChanges();
                            return RedirectToAction("Dispatch");
                        }
                        else 
                        {
                            if ((cust.Wallet + xx.Amount) >= oda.total)
                            {
                                if ((cust.Wallet + xx.Amount) > oda.total) cust.Wallet = (cust.Wallet + xx.Amount) - amnt;
                                db.Entry(cust).State = EntityState.Modified;
                                db.SaveChanges();
                                xx.Status = "C";
                                db.Entry(xx).State = EntityState.Modified;
                                db.SaveChanges();
                                return RedirectToAction("Dispatch");
                            }
                            else
                            {
                                cust.Wallet = cust.Wallet + xx.Amount;
                                db.Entry(cust).State = EntityState.Modified;
                                db.SaveChanges();
                                xx.Status = "C";
                                
                                db.Entry(xx).State = EntityState.Modified;
                                db.SaveChanges();

                                return Json("Partial", JsonRequestBehavior.AllowGet);
                            }
                        }
                    }
                    else
                    {
                       return  Json("No", JsonRequestBehavior.AllowGet);
                    }
                   
                }
                catch(Exception )
                {
                    return Json("Error",JsonRequestBehavior.AllowGet);
                }
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

       [Authorize]
       [HttpGet]
        public ActionResult Dispatch(string id)
        {
            if (Request.IsAuthenticated)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                var CurrentUser = User.Identity.Name;
                ViewData["OrderStatus"] = "AWAITING SHIPMENT";
                ViewData["Document"] = "Receipt";
                char[] delimiters = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

                var cust = db.Customers.Find(long.Parse(part[1]));
                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                var odalines = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept).ToList();
                ViewData["OrderNumber"] = oda.Reciept;
                var mm = DateTime.Now.Month ;
                if(cust.Period != mm )// change of month we start afresh
                {
                    Monthend mend = new Monthend();
                    mend.ChangePeriod(cust);
                }

                var sal = new Sale();
                sal.Account = oda.Account;
                sal.company = oda.company;
                sal.customer = oda.customer;
                sal.dated = DateTime.Now;
                sal.discount = oda.discount;
                sal.total = oda.total;
                sal.DeliveryType = oda.DeliveryType;
                sal.CollectionId = oda.CollectionId;
                sal.Reciept = oda.Reciept;
                sal.Tax = oda.Tax;
                if (oda.DeliveryType.Trim() == "Delivery")
                {
                    sal.state = "AS";
                }
                else
                {
                    sal.state = "A";
                }
               
                db.Sales.Add(sal);
                db.SaveChanges();

                if (oda.DeliveryType.Trim() == "Delivery" || oda.DeliveryType.Trim() == "Collection")
                {
                    #region Delivery
                    // delivary details 
                    var deliva = new Delivery();
                    deliva.CustomerId = cust.ID;
                    deliva.CustomerName = cust.CustomerName;
                    deliva.DateCreated = DateTime.Now;
                    deliva.DeadLine = DateTime.Now.AddDays(1);
                    deliva.Status = "O";
                    deliva.Receipt = oda.Reciept;
                    if (oda.CollectionId == 0)
                    {
                        var region = db.Regions.Find(cust.RegionId);
                        deliva.RegionId = cust.RegionId;
                        deliva.Region = region.Name;
                        deliva.DeliveryType = oda.DeliveryType;
                        deliva.Address = cust.Address1 + " " + cust.Address2;
                        deliva.Amount = region.DeliveryRate;
                        deliva.Saburb = cust.Suburb;
                        deliva.City = cust.City;
                    }
                    else if (oda.CollectionId != 2)
                    {
                        var cpoint = db.CollectionPoints.Find(oda.CollectionId);
                        deliva.RegionId = cpoint.RegionId;
                        deliva.Region = cpoint.RegionName;
                        deliva.DeliveryType = "Collection";
                        deliva.Address = cpoint.Address;
                        deliva.Saburb = cpoint.Suburb;
                        deliva.City = cpoint.City;
                        deliva.Amount = 0;
                    }
                    db.Deliveries.Add(deliva);
                    db.SaveChanges();
                    #endregion
                }

                
                foreach (var itm in odalines)
                {
                    
                    if(itm.Category == "AIRTIME")
                    {
                        TransactionRequest req = new TransactionRequest();
                        //PushController pc = new PushController();
                        req.Amount = decimal.Parse(itm.priceinc.ToString());
                        if (itm.Description == null || itm.Description =="")
                        {
                            req.CustomerMSISDN = cust.Phone1;
                        }
                        else
                        {
                            req.CustomerMSISDN = itm.Description;
                        }
                        req.Action = "SMS";
                        req.ProcessingCode  = "320000";
                        req.CustomerMSISDN = itm.Description;
                        req.Product = itm.SubCategory;
                       //var voucher = pc.getProduct(req, "");
                    }

                    if (itm.Category != null && itm.Category.Trim() != "REGISTRATION")
                    {

                        var inv = db.Items.Where(u => u.ItemCode.Trim() == itm.ItemCode.Trim()).FirstOrDefault();
                        inv.Balance = inv.Balance - 1;
                        inv.Quantity = inv.Quantity - 1;
                        inv.sold = inv.sold + 1;
                        db.Entry(inv).State = EntityState.Modified;
                        db.SaveChanges();
                    }

                    if (itm.Category != null && itm.Category.Trim() == "REGISTRATION")
                    {


                        var code = db.CustomerCodes.Where(u => u.CustomerId == cust.ID).FirstOrDefault();
                        
                        if (code == null)
                        {
                            code = new CustomerCode();
                            code.CustomerId = cust.ID;
                            code.Code = PinGen(cust.ID);
                            code.Size = (long)itm.quantity;
                            code.PurchaseDate = DateTime.Now;
                            code.Expirery = DateTime.Now.AddMonths(1);
                            db.CustomerCodes.Add(code);
                            db.SaveChanges();
                        }
                        else
                        {
                            code.Size = code.Size + (long)itm.quantity;
                            code.PurchaseDate = DateTime.Now;
                            code.Expirery = DateTime.Now.AddMonths(1);
                            db.Entry(code).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                    }
                    var sls = new Sales_Lines();
                    sls.Reciept = sal.Reciept;
                    sls.item = itm.item;
                    sls.ItemCode = itm.ItemCode;
                    sls.Category = itm.Category;
                    sls.SubCategory = itm.SubCategory;
                    sls.price = itm.price;
                    sls.priceinc = itm.priceinc;
                    sls.quantity = itm.quantity;
                    sls.SubCategory = itm.SubCategory;
                    sls.Category = itm.Category;
                    sls.Dated = sal.dated;
                    sls.Discount = itm.Discount;
                    sls.tax = itm.tax;
                    sls.Company = itm.Company;
                    db.Sales_Lines.Add(sls);
                    db.SaveChanges();
                    db.OrderLines.Remove(itm);
                    db.SaveChanges();

                    var todate = DateTime.Now.Date;
                    var slsam = db.SalesLineSammaries.Where(u => u.ItemCode.Trim() == itm.ItemCode.Trim() && u.Dated == todate && u.DeliveryType.Trim() == oda.DeliveryType).FirstOrDefault();
                    var updat = "Yes";
                    if (slsam == null)
                    {
                        updat = "No";
                        slsam = new SalesLineSammary();
                    }

                    slsam.ItemCode = itm.ItemCode;
                    slsam.ItemName = itm.item;
                    if (slsam.Total  == null) slsam.Total  = 0;
                    if (slsam.Quantity == null) slsam.Quantity = 0;
                    if (slsam.Balance == null) slsam.Balance = 0;
                    if (slsam.Received == null) slsam.Received = 0;
                    if(slsam.DeliveredQty == null)slsam.DeliveredQty =0;
                   
                    slsam.Quantity = slsam.Quantity + itm.quantity;
                    slsam.Balance = slsam.Quantity - slsam.DeliveredQty;
                    slsam.Total  = slsam.Quantity * itm.priceinc;
                    slsam.Dated = DateTime.Now.Date;
                    slsam.Company = itm.Company;
                    slsam.DeliveryType = oda.DeliveryType;
                    slsam.Deadline = DateTime.Now.AddDays(1);
                    slsam.DeliveryPicking = false;
                    if(updat == "Yes")
                    {
                        db.Entry(slsam).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    else
                    {
                        db.SalesLineSammaries.Add(slsam);
                        db.SaveChanges();
                    }
                }

                db.Orders.Remove(oda);
                db.SaveChanges();
                cust.Purchases = cust.Purchases + sal.total;
                if ((cust.Purchases) >= 30 && cust.Purchases < 50)
                {
                    cust.NetworkType = 1;
                }
                else if (cust.Purchases >= 50 && cust.Purchases < 80)
                {
                    cust.NetworkType = 2;
                }
                else if (cust.Purchases >= 80)
                {
                    cust.NetworkType = 3;
                } 
                db.Entry(cust).State = EntityState.Modified;
                db.SaveChanges();

                #region SalesStats
                var company = new Company() ;
                if (oda.company != null)
                {
                    company = np.GetCompanies(oda.company.Trim());
                }
                else
                {
                    company.ID = 1;
                }

                DailySale ds = np.GetDailySales(DateTime.Now.ToString("ddMMyyyy") + company.ID);
                if (ds == null)
                {
                    ds = new DailySale();
                    ds.Date = DateTime.Now.ToString("dd/MM/yyyy");
                    if (ds.Sales == null) ds.Sales = 0;
                    ds.Sales = ds.Sales + oda.total;
                    ds.Id = DateTime.Now.ToString("ddMMyyyy") + company.ID;
                    db.DailySales.Add(ds);
                    db.SaveChanges();
                }
                else
                {
                   
                    ds.Date = DateTime.Now.ToString("dd/MM/yyyy");
                    if (ds.Sales == null) ds.Sales = 0;
                    ds.Sales = ds.Sales + oda.total;
                    np.SaveOrUpdateDailySales(ds);
                }

                MonthlySale ms = np.GetMonthlySales(DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + company.ID);
                if (ms == null)
                {
                    ms = new MonthlySale();

                    ms.Id = DateTime.Now.Year.ToString() + DateTime.Now.Month.ToString() + company.ID;
                    if (ms.Sales == null) ms.Sales = 0;
                    ms.Sales = ms.Sales + oda.total;
                    ms.Year = DateTime.Now.Year;
                    db.MonthlySales.Add(ms);
                    db.SaveChanges();
                }
                else
                {
                    if (ms.Sales == null) ms.Sales = 0;
                    ms.Sales = ms.Sales + oda.total;
                    ms.Year = DateTime.Now.Year;
                    np.SaveOrUpdateMonthlySales(ms);
                }
                #endregion

                #region commissions
                // this section the system will look through all this customer's network for commissions
                
                #region parent0 // this is the current user 
                var totalsales = db.NetworkSales.Find(part[1] + "_" + DateTime.Now.Year);// total salesfor current user
                bool update = true;
                if (totalsales == null)
                { 
                    totalsales = new NetworkSale();
                    update = false;
                }
                 #region totals
                    switch (DateTime.Now.Month)
                    {
                        case 1:
                            if (totalsales.Jan0 == null)
                            {
                                totalsales.Jan0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.Jan0 = totalsales.Jan0 + sal.total - sal.Tax;
                            }
                            break;
                        case 2:
                            if (totalsales.Feb0 == null)
                            {
                                totalsales.Feb0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.Feb0 = totalsales.Feb0 + sal.total - sal.Tax;
                            }
                            break;
                        case 3:
                            if (totalsales.Mar0 == null)
                            {
                                totalsales.Mar0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.Mar0 = totalsales.Mar0 + sal.total - sal.Tax;
                            }
                            break;
                        case 4:
                            if (totalsales.Apr0 == null)
                            {
                                totalsales.Apr0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.Apr0 = totalsales.Apr0 + sal.total - sal.Tax;
                            }
                            break;
                        case 5:
                            if (totalsales.May0 == null)
                            {
                                totalsales.May0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.May0 = totalsales.May0 + sal.total - sal.Tax;
                            }
                            break;
                        case 6:
                            if (totalsales.Jun0 == null)
                            {
                                totalsales.Jun0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.Jun0 = totalsales.Jun0 + sal.total - sal.Tax;
                            }
                            break;
                        case 7:
                            if (totalsales.Jul0 == null)
                            {
                                totalsales.Jul0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.Jul0 = totalsales.Jul0 + sal.total - sal.Tax;
                            }
                            break;
                        case 8:
                            if (totalsales.Aug0 == null)
                            {
                                totalsales.Aug0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.Aug0 = totalsales.Aug0 + sal.total - sal.Tax;
                            }
                            break;
                        case 9:
                            if (totalsales.Sep0 == null)
                            {
                                totalsales.Sep0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.Sep0 = totalsales.Sep0 + sal.total - sal.Tax;
                            }
                            break;
                        case 10:
                            if (totalsales.Oct0 == null)
                            {
                                totalsales.Oct0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.Oct0 = totalsales.Oct0 + sal.total - sal.Tax;
                            }
                            break;
                        case 11:
                            if (totalsales.Nov0 == null)
                            {
                                totalsales.Nov0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.Nov0 = totalsales.Nov0 + sal.total - sal.Tax;
                            }
                            break;
                        case 12:
                            if (totalsales.Dec0 == null)
                            {
                                totalsales.Dec0 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales.Dec0 = totalsales.Dec0 + sal.total - sal.Tax;
                            }
                            break;
                    }
                    
                    #endregion
                if(update == false)
                {
                    totalsales.Id = part[1] + "_" + DateTime.Now.Year;
                    totalsales.Year = DateTime.Now.Year;
                    db.NetworkSales.Add(totalsales);
                    db.SaveChanges();
                }
                else
                {
                   db.Entry(totalsales).State = EntityState.Modified;
                   db.SaveChanges();

                }
#endregion
                var parent1 =  db.Customers.Find(cust.ParentId);
                if (parent1 != null)
                {
                    #region Parent1 // this is the parent who registred this user
                    var totalsales1 = db.NetworkSales.Find(parent1.ID + "_" + DateTime.Now.Year);// total salesfor current iser
                    update = true;
                    if (totalsales1 == null)
                    {
                        totalsales1 = new NetworkSale();
                        update = false;
                    }
                    #region totals1
                    switch (DateTime.Now.Month)
                    {
                        case 1:
                            if (totalsales1.Jan1 == null)
                            {
                                totalsales1.Jan1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.Jan1 = totalsales.Jan1 + sal.total - sal.Tax;
                            }
                            break;
                        case 2:
                            if (totalsales1.Feb1 == null)
                            {
                                totalsales1.Feb1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.Feb1 = totalsales1.Feb1 + sal.total - sal.Tax;
                            }
                            break;
                        case 3:
                            if (totalsales1.Mar1 == null)
                            {
                                totalsales1.Mar1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.Mar1 = totalsales1.Mar1 + sal.total - sal.Tax;
                            }
                            break;
                        case 4:
                            if (totalsales1.Apr1 == null)
                            {
                                totalsales1.Apr1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.Apr1 = totalsales1.Apr1 + sal.total - sal.Tax;
                            }
                            break;
                        case 5:
                            if (totalsales1.May1 == null)
                            {
                                totalsales1.May1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.May1 = totalsales1.May1 + sal.total - sal.Tax;
                            }
                            break;
                        case 6:
                            if (totalsales1.Jun1 == null)
                            {
                                totalsales1.Jun1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.Jun1 = totalsales1.Jun1 + sal.total - sal.Tax;
                            }
                            break;
                        case 7:
                            if (totalsales1.Jul1 == null)
                            {
                                totalsales1.Jul1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.Jul1 = totalsales1.Jul1 + sal.total - sal.Tax;
                            }
                            break;
                        case 8:
                            if (totalsales1.Aug1 == null)
                            {
                                totalsales1.Aug1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.Aug1 = totalsales1.Aug1 + sal.total - sal.Tax;
                            }
                            break;
                        case 9:
                            if (totalsales1.Sep1 == null)
                            {
                                totalsales1.Sep1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.Sep1 = totalsales1.Sep1 + sal.total - sal.Tax;
                            }
                            break;
                        case 10:
                            if (totalsales1.Oct1 == null)
                            {
                                totalsales1.Oct1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.Oct1 = totalsales1.Oct1 + sal.total - sal.Tax;
                            }
                            break;
                        case 11:
                            if (totalsales1.Nov1  == null)
                            {
                                totalsales1.Nov1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.Nov1 = totalsales1.Nov1 + sal.total - sal.Tax;
                            }
                            break;
                        case 12:
                            if (totalsales1.Dec1 == null)
                            {
                                totalsales1.Dec1 = sal.total - sal.Tax;
                            }
                            else
                            {
                                totalsales1.Dec1 = totalsales1.Dec1 + sal.total - sal.Tax;
                            }
                            break;
                    }
                    #endregion
                    if (update ==false)
                    {
                        totalsales1.Id = parent1.ID + "_" + DateTime.Now.Year;
                        totalsales1.Year = DateTime.Now.Year; 
                        db.NetworkSales.Add(totalsales1);
                        db.SaveChanges();    
                    }
                    else
                    {
                        db.Entry(totalsales1).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                    if (parent1.Wallet == null) parent1.Wallet = 0;
                    parent1.wallet1 = parent1.wallet1 + (sal.total - sal.Tax) * (decimal)0.03;
                    db.Entry(parent1).State = EntityState.Modified;
                    db.SaveChanges();
                    #endregion
                    var parent2 = db.Customers.Find(parent1.ParentId);
                    if (parent2 != null)
                    {
                        #region parents2 // parent to parent 1
                        var totalsales2 = db.NetworkSales.Find(parent2.ID + "_" + DateTime.Now.Year);// total salesfor current iser
                        update = true;
                        if (totalsales2 == null)
                        {
                            totalsales2 = new NetworkSale();
                            update = false;
                        }
                        #region totals2
                        switch (DateTime.Now.Month)
                        {
                            case 1:
                                if (totalsales2.Jan2 == null)
                                {
                                    totalsales2.Jan2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.Jan2 = totalsales2.Jan2 + sal.total - sal.Tax;
                                }
                                break;
                            case 2:
                                if (totalsales2.Feb2 == null)
                                {
                                    totalsales2.Feb2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.Feb2 = totalsales2.Feb2 + sal.total - sal.Tax;
                                }
                                break;
                            case 3:
                                if (totalsales2.Mar2 == null)
                                {
                                    totalsales2.Mar2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.Mar2 = totalsales2.Mar2 + sal.total - sal.Tax;
                                }
                                break;
                            case 4:
                                if (totalsales2.Apr2 == null)
                                {
                                    totalsales2.Apr2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.Apr2 = totalsales2.Apr2 + sal.total - sal.Tax;
                                }
                                break;
                            case 5:
                                if (totalsales2.May2 == null)
                                {
                                    totalsales2.May2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.May2 = totalsales2.May2 + sal.total - sal.Tax;
                                }
                                break;
                            case 6:
                                if (totalsales2.Jun2 == null)
                                {
                                    totalsales2.Jun2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.Jun2 = totalsales2.Jun2 + sal.total - sal.Tax;
                                }
                                break;
                            case 7:
                                if (totalsales2.Jul2 == null)
                                {
                                    totalsales2.Jul2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.Jul2 = totalsales2.Jul2 + sal.total - sal.Tax;
                                }
                                break;
                            case 8:
                                if (totalsales2.Aug2 == null)
                                {
                                    totalsales2.Aug2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.Aug2 = totalsales2.Aug2 + sal.total - sal.Tax;
                                }
                                break;
                            case 9:
                                if (totalsales2.Sep2 == null)
                                {
                                    totalsales2.Sep2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.Sep2 = totalsales2.Sep2 + sal.total - sal.Tax;
                                }
                                break;
                            case 10:
                                if (totalsales2.Oct2 == null)
                                {
                                    totalsales2.Oct2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.Oct2 = totalsales2.Oct2 + sal.total - sal.Tax;
                                }
                                break;
                            case 11:
                                if (totalsales2.Nov2  == null)
                                {
                                    totalsales2.Nov2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.Nov2 = totalsales2.Nov2 + sal.total - sal.Tax;
                                }
                                break;
                            case 12:
                                if (totalsales2.Dec2 == null)
                                {
                                    totalsales2.Dec2 = sal.total - sal.Tax;
                                }
                                else
                                {
                                    totalsales2.Dec2 = totalsales2.Dec2 + sal.total - sal.Tax;
                                }
                                break;
                        }
                        #endregion
                        if (update == false)
                        {
                            totalsales2 = new NetworkSale();
                            totalsales2.Id = parent2.ID + "_" + DateTime.Now.Year;
                            totalsales2.Year = DateTime.Now.Year;
                            db.NetworkSales.Add(totalsales2);
                            db.SaveChanges();    
                        }
                        else
                        {
                            db.Entry(parent2).State = EntityState.Modified;
                            db.SaveChanges();
                        }
                       
                            if (parent2.wallet2 == null) parent2.wallet2 = 0;
                            if (parent2.Wallet == null) parent2.Wallet = 0;
                            parent2.wallet2 = parent2.wallet2 + (sal.total - sal.Tax) * (decimal)0.02;
                            //parent2.Wallet = parent2.Wallet + (sal.total - sal.Tax) * (decimal)0.02;
                        
                        db.Entry(parent2).State = EntityState.Modified;
                        db.SaveChanges();
#endregion
                        var parent3 = db.Customers.Find(parent2.ParentId);
                        if (parent3 != null)
                        {
                            #region parent3 // parent tot parent 2
                            var totalsales3 = db.NetworkSales.Find(parent3.ID + "_" + DateTime.Now.Year);// total salesfor current iser
                            update = true;
                            if (totalsales3 == null)
                            {
                                totalsales3 = new NetworkSale();
                                update = false;
                            }
                            #region totals3
                            switch (DateTime.Now.Month)
                            {
                                case 1:
                                    if (totalsales3.Jan3 == null)
                                    {
                                        totalsales3.Jan3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.Jan3 = totalsales3.Jan3 + sal.total - sal.Tax;
                                    }
                                    break;
                                case 2:
                                    if (totalsales3.Feb3 == null)
                                    {
                                        totalsales3.Feb3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.Feb3 = totalsales3.Feb3 + sal.total - sal.Tax;
                                    }
                                    break;
                                case 3:
                                    if (totalsales3.Mar3 == null)
                                    {
                                        totalsales3.Mar3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.Mar3 = totalsales3.Mar3 + sal.total - sal.Tax;
                                    }
                                    break;
                                case 4:
                                    if (totalsales3.Apr3 == null)
                                    {
                                        totalsales3.Apr3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.Apr3 = totalsales3.Apr3 + sal.total - sal.Tax;
                                    }
                                    break;
                                case 5:
                                    if (totalsales3.May3 == null)
                                    {
                                        totalsales3.May3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.May3 = totalsales3.May3 + sal.total - sal.Tax;
                                    }
                                    break;
                                case 6:
                                    if (totalsales3.Jun3 == null)
                                    {
                                        totalsales3.Jun3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.Jun3 = totalsales3.Jun3 + sal.total - sal.Tax;
                                    }
                                    break;
                                case 7:
                                    if (totalsales3.Jul3 == null)
                                    {
                                        totalsales3.Jul3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.Jul3 = totalsales3.Jul3 + sal.total - sal.Tax;
                                    }
                                    break;
                                case 8:
                                    if (totalsales3.Aug3 == null)
                                    {
                                        totalsales3.Aug3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.Aug3 = totalsales3.Aug3 + sal.total - sal.Tax;
                                    }
                                    break;
                                case 9:
                                    if (totalsales3.Sep3 == null)
                                    {
                                        totalsales3.Sep3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.Sep3 = totalsales3.Sep3 + sal.total - sal.Tax;
                                    }
                                    break;
                                case 10:
                                    if (totalsales3.Oct3 == null)
                                    {
                                        totalsales3.Oct3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.Oct3 = totalsales3.Oct3 + sal.total - sal.Tax;
                                    }
                                    break;
                                case 11:
                                    if (totalsales3.Nov3  == null)
                                    {
                                        totalsales3.Nov3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.Nov3 = totalsales3.Nov3 + sal.total - sal.Tax;
                                    }
                                    break;
                                case 12:
                                    if (totalsales3.Dec3 == null)
                                    {
                                        totalsales3.Dec3 = sal.total - sal.Tax;
                                    }
                                    else
                                    {
                                        totalsales3.Dec3 = totalsales3.Dec3 + sal.total - sal.Tax;
                                    }
                                    break;
                            }
                            #endregion
                            if (update == false)
                            {
                                totalsales3.Id = parent3.ID + "_" + DateTime.Now.Year;
                                totalsales3.Year = DateTime.Now.Year;
                                db.NetworkSales.Add(totalsales3);
                                db.SaveChanges();  
                            }
                            else
                            {
                                db.Entry(totalsales3).State = EntityState.Modified;
                                db.SaveChanges();
                            }
                          
                                if (parent3.Wallet == null) parent3.Wallet = 0;
                                if (parent3.wallet3 == null) parent3.wallet3 = 0;
                                parent3.wallet3 = parent3.wallet3 + (sal.total - sal.Tax) * (decimal)0.01;
                               // parent3.Wallet = parent3.Wallet + (sal.total - sal.Tax) * (decimal)0.01;
                                db.Entry(parent3).State = EntityState.Modified;
                                db.SaveChanges();
                            
                            #endregion  
                        }
                    }
                } 
#endregion

                #region Sendemail
                // Initialize WebMail helper
                ST_decrypt dec = new ST_decrypt();
                var eml = dec.st_decrypt(cust.Email, "214ea9d5bda5277f6d1b3a3c58fd7034");

                MailMessage mailMessage = new MailMessage(
                                      new MailAddress("support@baskiti.com"), new MailAddress(eml));

                mailMessage.Subject = "Password Recovery";
                mailMessage.IsBodyHtml = true;
                mailMessage.Body =
                "<style>a.buttons {display: inline-block;vertical-align: middle;background: #34495E;font-size: 14px !important;text-transform: initial;padding: 10px 20px;color: #fff !important; margin-left: 2px;cursor:pointer;text-decoration:none;}</style>" +
                "<div style='width: 100%'><div style='width:50%; margin:0 auto '><div style='height:45px; '>" +
                "<a href='http://www.baskiti.com'><img style='width:200px' src='http://www.baskiti.com/Content/Template/img/logo.png' alt='baskiti'></a>" +
                "</div><div style='margin-top:-20px'><div style='margin:10px 15px 10px 10px'><br /><p>Hi "+ cust.ContactPerson + ",</p><p>" +
                "Your purchase on baskiti for $" + sal.total.Value.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture) + " was successful.<br/>" +
                " Your transaction reference number is " + sal.Reciept + ". The details of your purchase are found in your baskiti account purchase history. <br/><br/> " +
                "Thank you for being loyal to your network. Shop again soon." +

                "</p></div></div></div></div>";


                System.Net.NetworkCredential networkCredentials = new
                System.Net.NetworkCredential("support@baskiti.com", "shoppa@2016");

                SmtpClient smtpClient = new SmtpClient();
                smtpClient.EnableSsl = false;
                smtpClient.UseDefaultCredentials = false;
                smtpClient.Credentials = networkCredentials;
                smtpClient.Host = "smtpout.secureserver.net";
                smtpClient.Port = 25;
                smtpClient.Timeout = 10000;
                smtpClient.Send(mailMessage);
                // Send email


                #endregion
                var regionz = db.Regions.Find(cust.RegionId);
                var surburb = db.Suburbs.Find(long.Parse(cust.Suburb));
                var city = db.Cities.Find(long.Parse(cust.City));
                cust.Suburb = surburb.Name;
                cust.City = city.Name;
                var resp = new ShoppingCart();
                resp.orderlines = odalines;
                resp.customer = cust;
                resp.region = regionz;
                resp.order = oda;
                return View("Dispatch", "_PrintLayout", resp);
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        private string PinGen(long  id)
        {
            
            Random rng = new Random();
            long d = rng.Next(1, 99999);
            //modify pin
            //string wwd = DateTime.Now.Date.ToString() + DateTime.Now.Year.ToString();
            ST_encrypt en = new ST_encrypt();
           
            int w = id.ToString().Length;
            int rnd = d.ToString().Length;
            string dum = "";
            string start = "";
            switch (rnd)
            {
                case 1:
                    start = "3";
                    dum = rng.Next(10000, 99999).ToString();
                    break;
                case 2:
                    dum = rng.Next(1345, 9999).ToString();
                    start = "5";
                    break;
                case 3:
                    dum = rng.Next(112, 999).ToString();
                    start = "2";
                    break;
                case 4:
                    dum = rng.Next(17, 99).ToString();
                    start = "1";
                    break;
                case 5:
                    dum = rng.Next(0, 9).ToString();
                    start = "4";
                    break;
            }
            if (dum == "")
            {
                start = "6";
                dum = rng.Next(1000, 9999).ToString();
            }
            var idx = id + int.Parse(start) - rnd;
            string pin = start + rnd + dum + d + idx;

            return pin;
        }
  
        public ActionResult PrintInvoice(string receipt)
        {
          
            return new ActionAsPdf(
                           "Dispatch"
                           ) { FileName = receipt + ".pdf" };
        }
        //
        // GET: /Items/Create

        public ActionResult Create()
        {
            if (Request.IsAuthenticated)
            {
             
                NHibernateDataProvider np = new NHibernateDataProvider();
                var comp = np.GetAllCompanies();
            
                return PartialView();
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }

        [HttpPost]
        public ActionResult Create(Item item)
        {
             if(Request.IsAuthenticated)
             {
                if (ModelState.IsValid)
                {
                    NHibernateDataProvider np = new NHibernateDataProvider();
                    np.SaveOrUpdateItems(item);
                    return RedirectToAction("Index");  
                }

                return PartialView(item);
             }
             else
             {
                 return RedirectToAction("Index", "Home");
             }
        }
        
        //
        // GET: /Items/Edit/5
        public ActionResult RemoveCartItem(int id, string SessionId)
        {
            
            NHibernateDataProvider np = new NHibernateDataProvider();
            var cust = new Customer();
            var gues = new Guest();
            if(!Request.IsAuthenticated)
            {
                gues.CustomerName = "Guest";
                //var pz = np.GetAllCustomers();
                //var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                
                var ols = db.OrderLines.Find(id);
                //var ols = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();

                var oda = db.Orders.Where(u => u.Reciept == ols.Reciept).FirstOrDefault();
                db.OrderLines.Remove(ols);
                db.SaveChanges();

                var olz = db.OrderLines.Where(u => u.Reciept == oda.Reciept).ToList();
                //   var airt = olz.Where(u => u.Category.Equals("AIRTIME")).ToList();
                var pp = db.Payments.Where(u => u.ReceiptNo == oda.Reciept).FirstOrDefault();
                if (olz.Count() == 0)
                {
                    if (pp != null)
                    {
                        db.Payments.Remove(pp);
                        db.SaveChanges();
                    }
                    db.Orders.Remove(oda);
                    db.SaveChanges();

                }
                else
                {
                    if (oda.CollectionId == 2)
                    {
                        oda.CollectionId = 0;
                        oda.DeliveryType = "Delivery";
                    }
                    /* else if (airt.Count > 0 && olz.Count == airt.Count)
                     {
                         oda.CollectionId = 2;
                         oda.DeliveryType = "Sms";
                     }*/
                    oda.total = oda.total - ols.priceinc;
                    oda.discount = oda.discount - ols.Discount;
                    oda.Tax = oda.Tax = ols.tax;
                    db.Entry(oda).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Cartitems", new { SessionId = SessionId });
            }

            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;

                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                cust = db.Customers.Find(long.Parse(part[1]));
               
                var ols = db.OrderLines.Find(id);
                var oda = db.Orders.Where(u => u.Reciept == ols.Reciept).FirstOrDefault();
                db.OrderLines.Remove(ols);
                db.SaveChanges();

                var olz = db.OrderLines.Where(u => u.Reciept == oda.Reciept).ToList();
             //   var airt = olz.Where(u => u.Category.Equals("AIRTIME")).ToList();
                var pp = db.Payments.Where(u => u.ReceiptNo == oda.Reciept).FirstOrDefault();
                if (olz.Count() == 0)
                {
                    if (pp != null)
                    {
                        db.Payments.Remove(pp);
                        db.SaveChanges();
                    }
                    db.Orders.Remove(oda);
                    db.SaveChanges();

                }
                else
                {
                    if ( oda.CollectionId == 2)
                    {
                        oda.CollectionId = 0;
                        oda.DeliveryType = "Delivery";
                    }
                   /* else if (airt.Count > 0 && olz.Count == airt.Count)
                    {
                        oda.CollectionId = 2;
                        oda.DeliveryType = "Sms";
                    }*/
                    oda.total = oda.total - ols.priceinc;
                    oda.discount = oda.discount - ols.Discount;
                    oda.Tax = oda.Tax = ols.tax;
                    db.Entry(oda).State = EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Cartitems");
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet); ;
            }
        }
 
        public ActionResult EmptyCart(string SessionId)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var cust = new Customer();
            var gues = new Guest();
            if (!Request.IsAuthenticated) 
            {
                var pz = np.GetAllGuest();
                var pp = pz.Where(u => u.Username == SessionId).FirstOrDefault();
                gues.AccountCode = pp.AccountCode;
               // var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                cust.CustomerName = "Guest";
                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim()|| u.Account.Trim() == gues.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();

                var ols = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept.Trim()).ToList();

                db.Orders.Remove(oda);
                db.SaveChanges();
                foreach (var item in ols)
                {
                    db.OrderLines.Remove(item);
                    db.SaveChanges();
                }

                return RedirectToAction("Cartitems");
            }
            if (Request.IsAuthenticated)
            {
                var CurrentUser = User.Identity.Name;

                char[] delimiter = new char[] { '~' };
                string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                cust = db.Customers.Find(long.Parse(part[1]));
                var oda = db.Orders.Where(u => u.Account.Trim() == cust.AccountCode.Trim() & u.state.Trim() == "O").FirstOrDefault();
                
                var ols = db.OrderLines.Where(u => u.Reciept.Trim() == oda.Reciept.Trim()).ToList();

                db.Orders.Remove(oda);
                db.SaveChanges();
                foreach(var item  in ols)
                {
                    db.OrderLines.Remove(item);
                    db.SaveChanges();
                }

                return RedirectToAction("Cartitems", new { SessionId = SessionId });
            }
            else
            {
                return Json(0, JsonRequestBehavior.AllowGet); ;
            }
        }

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Item item = np.GetItems(id);
            np.DeleteItems(item); 
            return RedirectToAction("Index");
        }

        [HttpPost]
        public string EcocashPayment()
        {
            try
            {
               
                NameValueCollection nvc = Request.Form;
                string abonent = "";
                string refno = "";
                decimal Amount = 0;
                string pincode = "";
                string username = "";
                string password = "";
                string Sender = "";
                string bal = "";
                decimal dd = 0;

                if (!string.IsNullOrEmpty(nvc["abonent"]))
                {
                    abonent = nvc["abonent"];
                }
                if (!string.IsNullOrEmpty(nvc["username"]))
                {
                    username = nvc["username"];
                }
                if (!string.IsNullOrEmpty(nvc["password"]))
                {
                    password = nvc["password"];
                }
                if (!string.IsNullOrEmpty(nvc["refno"]))
                {
                    refno = nvc["refno"];
                }
                if (!string.IsNullOrEmpty(nvc["Sender"]))
                {
                    Sender = nvc["Sender"];
                }
                if (!string.IsNullOrEmpty(nvc["Amount"]))
                {
                    Amount = decimal.Parse(nvc["Amount"]);
                }
                LoginController lg = new LoginController();
               
              
                Payment  py = new Payment();
                string message = "";

                py.Amount = Amount;
                if (pincode != null && pincode != "")
                {
                    py.Description  = "M-Wallet payment Pin generated";
                    message = "Your account top up code for $" + Amount.ToString("#0.##") + " is " + pincode + ". To recharge log into Brainstorm, use Credit manager menu or Redeem Voucher on the low balance screen.";
                }
                else
                {
                    py.Description = "M-Wallet payment";
                    message = "You have topped up your account with $" + Amount.ToString("#0.##") + " Your new balance is $" + dd.ToString("#0.##");
                }

                py.SourceRefrence = refno;
                py.Location = "Ecocash-Applevine";
                py.Source = Sender;
                py.SourceMobileNumber = abonent;
                py.DateCreated = DateTime.Now;
                py.PaymentType = "Payment";
                db.Payments.Add(py);
                db.SaveChanges();
               // SendSms(abonent, message);
                return "true";
            }
            catch
            {
                return "Failed";
            }
        }

        private static string GenerateTwoWayHash(string uri, string guid)
        {            
            SHA512 check = SHA512.Create();
            byte[] resultArr = check.ComputeHash(Encoding.UTF8.GetBytes(uri + guid));             
            return ByteArrayToString (resultArr); 
        }  
        
        public static string ByteArrayToString(byte[] ba) 
        {  
            StringBuilder hex = new StringBuilder(ba.Length * 2);   
            foreach (byte b in ba)       
                hex.AppendFormat("{0:x2}", b);    
            return hex.ToString();
        }   

        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               // context.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}