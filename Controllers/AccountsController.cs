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
using System.Web.Helpers;
using System.Windows.Media.Imaging;
using System.Web.Security;
using crypto;
using System.Net.Mail;
using Newtonsoft.Json;


namespace RetailKing.Controllers
{   
    public class AccountsController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        private void ResizeImage(string inputPath, string outputPath, int width, int height)
        {
            BitmapImage bitmap = new BitmapImage();

            using (var stream = new FileStream(inputPath, FileMode.Open))
            {
                bitmap.BeginInit();
                bitmap.DecodePixelWidth = width;
                bitmap.DecodePixelHeight = height;
                bitmap.CacheOption = BitmapCacheOption.OnLoad;
                bitmap.StreamSource = stream;
                bitmap.EndInit();
            }

            PngBitmapEncoder encoder = new PngBitmapEncoder();
            encoder.Frames.Add(BitmapFrame.Create(bitmap));

            using (var stream = new FileStream(outputPath, FileMode.Create))
            {
                encoder.Save(stream);
            }
        }
        //
        // GET: /Accounts/

        public ActionResult Payment(decimal amount, string MTI, string Processsingcode, long TransactionType, string agentcode)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            YomoneyResponse resp = new YomoneyResponse();
            YomoneyRequest req = new YomoneyRequest();
            //var getlist;
            req.AgentCode = agentcode;
            req.ServiceId = 20;
            req.Amount = amount;
            req.MTI = MTI;
            req.ProcessingCode = Processsingcode;
            req.TransactionType = TransactionType;
            req.MaxSale = 0;
            req.MinSale = 0;
            IList<YomoneyResponse> pd = new List<YomoneyResponse>();
            YomoneyController pc = new YomoneyController();
            switch (TransactionType)
            {
                case 1:
                    break;
                case 2:
                    break;
                case 3:
                    break;
                case 4:
                    break;
                case 5:
                    switch (MTI)
                    {
                        case "0300":
                            switch (Processsingcode)
                            {
                                case "420000":
                                    var getlist = pc.GetExternal(req);

                                    var tr = JsonConvert.DeserializeObject<List<YomoneyResponse>>(getlist.Narrative);
                                    foreach (var res in tr)
                                    {
                                        resp.Name = res.Name;
                                        pd.Add(resp);
                                    }


                                    // RedirectToAction("PaymentMethods",pd);
                                   // return pd;
                                    break;

                                case "400000":
                                    break;

                                case "430000":
                                    break;

                                case "440000":
                                    break;
                            }
                            break;
                        case "0200":
                            switch (Processsingcode)
                            {
                                case "300000":
                                    break;

                                case "310000":
                                    break;

                                case "340000":
                                    break;

                                case "320000":
                                    break;

                                case "330000":
                                    break;
                            }
                            break;
                        case "0100":

                            break;
                    }
                    break;

            }



            return View("PaymentMethods",pd); 
        }
        [HttpGet]
        public ActionResult GetSuburbs(string Id, long City)
        {
            if (Id == null) Id = "";
            Id = Id.Trim();
            var level = db.Suburbs.Where(u=>u.Name.Contains(Id) && u.CityId == City ).ToList();
            List<SelectListItem> sb = new List<SelectListItem>();
            
            foreach (var item in level)
            {
                sb.Add(new SelectListItem
                {
                    Text = item.Name.Trim(),
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Suburb = sb;

            return Json(sb, JsonRequestBehavior.AllowGet);
            //return PartialView(level);
        }
     
        [HttpGet]
        public ActionResult GetRegions(long Id)
        {
            var level = db.Regions.Where(u => u.CityId == Id).ToList();
            List<SelectListItem> sb = new List<SelectListItem>();
            sb.Add(new SelectListItem
            {
                Text = "-Select Region-",
                Value = "-Select Region-"
            });
            foreach (var item in level)
            {
                sb.Add(new SelectListItem
                {
                    Text = item.Name,
                    Value = item.Id.ToString()
                });
            }

            ViewBag.Regions = sb;

            return PartialView();
        }
 
        [Authorize]
        public ActionResult Index(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if(string.IsNullOrEmpty(Company))
            {
                Company = part[1];
            }
            var px = np.GetAllAccounts();
            var comp = np.GetActivecoByName(Company);
            px= (from e in px
                     .Where(u => u.CompanyId == comp.CompanyId && !u.AccountCode.StartsWith("3-") && !u.AccountCode.StartsWith("8"))
                     select e).OrderBy(u=> u.AccountCode).ToList();
            
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
                       .Where(u => u.CompanyId == comp.CompanyId && !u.AccountCode.StartsWith("3-") && !u.AccountCode.StartsWith("8"))
                  select e).OrderBy(u => u.AccountCode).ToList();

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

        public ActionResult listExpenses(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (string.IsNullOrEmpty(Company))
            {
                Company = part[1];
            }
            var px = np.GetAllExpenses();
            var comp = np.GetActivecoByName(Company);
            px = (from e in px
                      .Where(u => u.till == comp.company && u.AccountCode.StartsWith("8") && u.type == "RECEIPT")
                  select e).OrderBy(u => u.AccountCode).ToList();

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

            return PartialView(nn);
        }

        [HttpPost]
        public ActionResult listExpenses(string Company, JQueryDataTableParamModel param)
        {
             NHibernateDataProvider np = new NHibernateDataProvider();
            //JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var px = np.GetAllExpenses();
            var comp = np.GetActivecoByName(Company);
            px = (from e in px
                      .Where(u => u.till == comp.company && u.AccountCode.StartsWith("8") && u.type == "RECEIPT")
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

            return PartialView(nn);
        }

        //
        // GET: /Accounts/Details/5
        public ActionResult Payments(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var acc= np.GetTransactionsByType("PAYMENT");
           // var comp = np.GetCompanies(Company);
            Expens ex = new Expens();

            List<SelectListItem> yr = new List<SelectListItem>();
            List<SelectListItem> typ = new List<SelectListItem>();
            yr.Add(new SelectListItem
            {
                Text = "--Select Account--",
                Value = ""
            });

            typ.Add(new SelectListItem
            {
                Text = "--Select Type--",
                Value = ""
            });

            foreach (var item in acc)
            {       
                    yr.Add(new SelectListItem
                    {
                        Text = item.TransactionName ,
                        Value = item.TransactionName 
                    });  
            }

        
            ViewBag.Type = yr;
            return PartialView (ex);
        }

        [HttpPost]
        public ActionResult Payments(Expens exp)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (exp.Invoice != null && exp.Balance != null && exp.Payee != null && exp.type != "" )
            {
           
            char[] delimiter = new char[] { '_' };
            string[] part = exp.Payee.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

            char[] delimite = new char[] { ',' };
            string[] invoices = exp.Invoice.Split(delimite, StringSplitOptions.RemoveEmptyEntries);
            string[] balances = exp.type.Split(delimite, StringSplitOptions.RemoveEmptyEntries);
            var currenUser = User.Identity.Name;
            char[] delimita = new char[] { '~' };
            string[] ppp = currenUser.Split(delimita, StringSplitOptions.RemoveEmptyEntries);
            var uzar = np.Getlogin(ppp[0]);
            exp.AccountName = part[0];
            exp.AccountCode = part[1];
            exp.Dated = DateTime.Now;
            exp.cashier = currenUser;
            exp.Payee = exp.type;
            exp.type = "PAYMENT";
            long id = np.AddExpenses(exp);
            var expns = np.GetExpenses(id);
            expns.Receipt = uzar.prefix.Trim() + "-" + id;
            np.SaveOrUpdateExpenses(expns);

           // var px = np.GetSalesOpenCredit(exp.Payee, exp.till);
            var px = np.GetPurchasesOpenCredit(exp.AccountName, exp.till);
            foreach (var item in px)
            {
                for (int i = 0; i < invoices.Length; i++)
                {
                    if (item.Invoice.Trim() == invoices[i])
                    {
                        item.Balance = decimal.Parse(balances[i]);
                        if (item.Balance == 0)
                        {
                            item.Status = "C";
                        }
                        np.SaveOrUpdatePurchases(item);
                    }
                }
            }
            
            Transaction trn = new Transaction();

            trn.Process(exp.Payee, decimal.Parse(exp.Amount.ToString()), exp.till, exp.AccountName);
            Expens ex = new Expens();
            var acc = np.GetTransactionsByType("PAYMENT");

            List<SelectListItem> yr = new List<SelectListItem>();
            List<SelectListItem> typ = new List<SelectListItem>();
            yr.Add(new SelectListItem
            {
                Text = "--Select Account--",
                Value = ""
            });

            typ.Add(new SelectListItem
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
            return PartialView(ex);
            }
            else{
                exp.cashier = "Please make sure the invoices are knocked off";
                var acc = np.GetTransactionsByType("PAYMENT");
               // var comp = np.GetCompanies(exp.till);
                Expens ex = new Expens();

                List<SelectListItem> yr = new List<SelectListItem>();
                List<SelectListItem> typ = new List<SelectListItem>();
                yr.Add(new SelectListItem
                {
                    Text = "--Select Account--",
                    Value = ""
                });

                typ.Add(new SelectListItem
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
                return PartialView(exp);
            }
            //return PartialView();
        }

        public ActionResult PaymentLines(string type, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var main_px = np.GetAllExpenses();
            if (param.iDisplayStart == 0)
            {
                param.iDisplayStart = 1;
            }
            if (param.iDisplayLength == 0)
            {
                param.iDisplayLength = 21;
            }
            var nn = (from emp in main_px.Skip((param.iDisplayStart - 1) * param.iDisplayLength).Take(param.iDisplayLength)
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
            ViewData["listSize"] = param.iDisplayLength;
            ViewData["Pages"] = pages;
            ViewData["ThisPage"] = page;
            ViewData["Columns"] = column + 1;
            ViewData["RecordData"] = "Showing " + start + " to " + end + " of " + main_px.Count;

            return PartialView(nn);
        }

        public ActionResult GetPayee(string Transaction, string searchtext, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var trn = np.GetTransactionsByName(Transaction).FirstOrDefault();
            string Payee;
             IList<Account> puu = new List<Account>();
             if (trn != null)
             {
                 #region debits
                 if (trn.Debit1 != null && trn.Debit1 != "")
                 {
                     if (trn.Debit1.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                        foreach(var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.CustomerName;
                            aa.AccountCode = item.AccountCode;
                            aa.Balance = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            puu.Add(aa);
                        }

                     }
                     else if (trn.Debit1.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }

                 }
                 if (trn.Debit2 != null && trn.Debit2 != "")
                 {
                     if (trn.Debit2.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.CustomerName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }

                     }
                     else if (trn.Debit2.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }
                 }

                 if (trn.Debit3 != null && trn.Debit3 != "")
                 {
                     if (trn.Debit3.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.CustomerName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }

                     }
                     else if (trn.Debit3.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }
                 }

                 if (trn.Debit4 != null && trn.Debit4 != "")
                 {
                     if (trn.Debit4.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.CustomerName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }

                     }
                     else if (trn.Debit4.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance; 
                             if (aa.Balance == null) aa.Balance = 0;

                             puu.Add(aa);
                         }
                     }
                 }

                 if (trn.Debit5 != null && trn.Debit5 != "")
                 {
                     if (trn.Debit5.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.CustomerName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }

                     }
                     else if (trn.Debit5.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }

                 }
                 if (trn.Debit6 != null && trn.Debit6 != "")
                 {
                     if (trn.Debit6.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.CustomerName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }

                     }
                     else if (trn.Debit6.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }
                 }
                 #endregion

                 #region credits
                 if (trn.Credit1 != null && trn.Credit1 != "")
                 {
                     if (trn.Credit1.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.CustomerName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }

                     }
                     else if (trn.Credit1.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }
                 }

                 if (trn.Credit2 != null && trn.Credit2 != "")
                 {

                     if (trn.Credit2.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.CustomerName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }

                     }
                     else if (trn.Credit2.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }

                 }

                 if (trn.Credit3 != null && trn.Credit3 != "")
                 {

                     if (trn.Credit3.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.CustomerName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }
                     else if (trn.Credit3.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }
                 }

                 if (trn.Credit4 != null && trn.Credit4 != "")
                 {

                     if (trn.Credit4.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.CustomerName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }
                     else if (trn.Credit4.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }
                 }

                 if (trn.Credit5 != null && trn.Credit5 != "")
                 {

                     if (trn.Credit5.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.CustomerName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }

                     }
                     else if (trn.Credit5.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }
                     }
                 }
                 if (trn.Credit6 != null && trn.Credit6 != "")
                 {
                     if (trn.Credit6.Trim() == "DEBTORS")
                     {
                         var px = np.GetCustomersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.CustomerName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);
                         }

                     }
                     else if (trn.Credit6.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersSearch(searchtext);
                         foreach (var item in px)
                         {
                              Account aa = new Account();
                             aa.AccountName = item.SupplierName;
                             aa.AccountCode = item.AccountCode;
                             aa.Balance = item.Balance;
                             if (aa.Balance == null) aa.Balance = 0;
                             puu.Add(aa);

                         }
                     }
                 }
                 #endregion
             }
             if (puu.Count  == 0 && searchtext.Length >= 5)
             {
                 var px = np.GetSuppliersSearch(searchtext);
                 foreach (var item in px)
                 {
                     Account aa = new Account();
                     aa.AccountName = item.SupplierName;
                     aa.AccountCode = item.AccountCode;
                     aa.Balance = item.Balance;
                     if (aa.Balance == null) aa.Balance = 0;
                     puu.Add(aa);
                 }
             }
             if (puu.Count == 0 && searchtext.Length >= 5)
             {
                 var px = np.GetCustomersSearch(searchtext);
                 foreach (var item in px)
                 {
                     Account aa = new Account();
                     aa.AccountName = item.CustomerName;
                     aa.AccountCode = item.AccountCode;
                     aa.Balance = item.Balance;
                     if (aa.Balance == null) aa.Balance = 0;
                     puu.Add(aa);
                 }

             }
             if (puu.Count == 0 && searchtext.Length >= 5)
            {
             var pux = np.GetAccountSearch(searchtext);
             if (pux != null && pux.Count > 0)
             {
                 var comp = np.GetActivecoByName(Company);
                 puu = (from e in pux
                        where e.CompanyId == comp.CompanyId
                        select e).ToList();
             }
            }

            return PartialView(puu);
        }

        public ActionResult GetInvoices(string Transaction, string Payee, string Company, string amount)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var trn = np.GetTransactionsByName(Transaction).FirstOrDefault();
             char[] delimite = new char[] { '_' };
            string[] part = Payee.Split(delimite, StringSplitOptions.RemoveEmptyEntries);

             char[] delimiter = new char[] { '-' };
            string[] parts = part[1].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            var co = np.GetCompanies(Company);

            if(parts[0] == "5")
            {
                var payi = np.GetSuppliersByName(part[0]).FirstOrDefault();
                  ViewData["AccountName"] = payi.SupplierName;
                  if (payi.Balance == null) payi.Balance = 0;
                  ViewData["Balance"]=payi.Balance;
                  ViewData["Amount"]= amount;
    
            }
            else if(parts[0] == "8")
            {
                var payi = np.GetCustomersByName(part[0]).FirstOrDefault();
                  ViewData["AccountName"] = payi.CustomerName ;
                  if (payi.Balance == null) payi.Balance = 0;
                  ViewData["Balance"] = payi.Balance;
                  ViewData["Amount"]= amount;
   
            }
            else
            {
                var pa = np.GetAccountsByName(part[0]);
                var payi = (from e in pa
                            where e.CompanyId ==co.ID
                            select e).FirstOrDefault();
               ViewData["AccountName"] = payi.AccountName ;
               if (payi.Balance == null) payi.Balance = 0;
               ViewData["Balance"] = payi.Balance;
               ViewData["Amount"]=amount ;
    
            }

            string typ = trn.Type;
            IList<Account> puu = new List<Account>();
            Payee = part[0];
            if (trn != null)
            {
                #region debits
                if (trn.Debit1 != null && trn.Debit1 != "")
                {
                    if (trn.Debit1.Trim() == "CASH")
                    {
                        var px = np.GetSalesOpenCredit(Payee, Company);

                        foreach (var item in px)
                        {
                            Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            if (item.Balance == null) item.Balance = item.total;
                            aa.Opening = item.Balance;
                            aa.LinkAccount = item.Reciept;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }

                    }
                    else if (trn.Debit1.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee,Company);

                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            if (item.Balance == null) item.Balance = item.total;
                            aa.Opening = item.Balance;
                            aa.LinkAccount = item.Invoice;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }

                    }
                    else if (trn.Debit1.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee,Company);
                        foreach (var item in px)
                        {
                            Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.Opening = item.Balance;
                            aa.LinkAccount = item.Invoice;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }

                }
                if (trn.Debit2 != null && trn.Debit2 != "")
                {
                    if (trn.Debit2.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee,Company);

                        foreach (var item in px)
                        {
                            Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            if (item.Balance == null) item.Balance = item.total;
                            aa.Opening = item.Balance;
                            aa.LinkAccount = item.Reciept ;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }

                    }
                    else if (trn.Debit2.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee, Company);
                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                }

                if (trn.Debit3 != null && trn.Debit3 != "")
                {
                    if (trn.Debit3.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee, Company);

                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            if (item.Balance == null) item.Balance = item.total;
                            aa.Opening = item.Balance;
                            aa.LinkAccount = item.Reciept;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }

                    }
                    else if (trn.Debit3.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee, Company);
                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            if (item.Balance == null) item.Balance = item.total;
                            aa.Opening = item.Balance;
                            aa.LinkAccount = item.Reciept;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                }

                if (trn.Debit4 != null && trn.Debit4 != "")
                {
                    if (trn.Debit4.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee, Company);

                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            if (item.Balance == null) item.Balance = item.total;
                            aa.Opening = item.Balance;
                            aa.LinkAccount = item.Reciept;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }

                    }
                    else if (trn.Debit4.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee, Company);
                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                }

                if (trn.Debit5 != null && trn.Debit5 != "")
                {
                    if (trn.Debit5.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee, Company);

                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            if (item.Balance == null) item.Balance = item.total;
                            aa.Opening = item.Balance;
                            aa.LinkAccount = item.Reciept;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }

                    }
                    else if (trn.Debit5.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee, Company);
                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }

                }
                if (trn.Debit6 != null && trn.Debit6 != "")
                {
                    if (trn.Debit6.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee, Company);

                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            if (item.Balance == null) item.Balance = item.total;
                            aa.Opening = item.Balance;
                            aa.LinkAccount = item.Reciept;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }

                    }
                    else if (trn.Debit6.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee, Company);
                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                }

                #endregion

                #region credits
                if (trn.Credit1 != null && trn.Credit1 != "")
                {
                    if (trn.Credit1.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee, Company);

                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }

                    }
                    else if (trn.Credit1.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee, Company);
                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                }

                if (trn.Credit2 != null && trn.Credit2 != "")
                {

                    if (trn.Credit2.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee, Company);

                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }

                    }
                    else if (trn.Credit2.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee, Company);
                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }

                }

                if (trn.Credit3 != null && trn.Credit3 != "")
                {

                    if (trn.Credit3.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee, Company);

                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                    else if (trn.Credit3.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee, Company);
                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                }

                if (trn.Credit4 != null && trn.Credit4 != "")
                {

                    if (trn.Credit4.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee, Company);

                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                    else if (trn.Credit4.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee, Company);
                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                }

                if (trn.Credit5 != null && trn.Credit5 != "")
                {

                    if (trn.Credit5.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee, Company);

                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                    else if (trn.Credit5.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee, Company);
                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                }
                if (trn.Credit6 != null && trn.Credit6 != "")
                {
                    if (trn.Credit6.Trim() == "DEBTORS")
                    {
                        var px = np.GetSalesOpenCredit(Payee, Company);

                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.customer;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                    else if (trn.Credit6.Trim() == "CREDITORS")
                    {
                        var px = np.GetPurchasesOpenCredit(Payee, Company);
                        foreach (var item in px)
                        {
                             Account aa = new Account();
                            aa.AccountName = item.supplier;
                            aa.AccountCode = item.ID.ToString();
                            aa.Balance = item.total;
                            aa.LinkAccount = item.Invoice;
                            aa.Opening = item.Balance;
                            if (aa.Balance == null) aa.Balance = 0;
                            if (aa.Opening == null) aa.Opening = aa.Balance;
                            puu.Add(aa);
                        }
                    }
                }
                #endregion
            }
          
            if (puu == null)
            {
                var pux = np.GetAccountSearch(Payee);
                if (pux != null && pux.Count > 0)
                {
                    var comp = np.GetActivecoByName(Company);
                    puu = (from e in pux
                           where e.CompanyId == comp.CompanyId
                           select e).ToList();
                }
            }

            return PartialView(puu);
        }

        [Authorize]
        public ActionResult Receipts(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var acc = np.GetTransactionsByType("RECEIPT");
            var comp = np.GetCompanies(Company);

            Expens ex = new Expens();
            List<SelectListItem> yr = new List<SelectListItem>();
            List<SelectListItem> typ = new List<SelectListItem>();
            yr.Add(new SelectListItem
            {
                Text = "--Select Account--",
                Value = ""
            });

            typ.Add(new SelectListItem
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
            return PartialView();
        }

        [Authorize]
        [HttpPost]
        public ActionResult Receipts(Expens exp)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (exp.Invoice != null && exp.Amount != null && exp.Payee != null && exp.type != "")
            {
               // NHibernateDataProvider np = new NHibernateDataProvider();
                //exp.Balance = exp.value - exp.Amount;
                char[] delimiter = new char[] { '_' };
                string[] part = exp.Payee.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);

                char[] delimite = new char[] { ',' };
                string[] lines = exp.Invoice.Split(delimite, StringSplitOptions.RemoveEmptyEntries);
                List<string> invoices = new List<string>();
                List<string> balances = new List<string>();
                for (var i = 0; i < lines.Length; i++)
                {
                    string[] invoice = lines[i].Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                    invoices.Add(invoice[0]);
                    balances.Add(invoice[1]);
                }
                var CurrentUser = User.Identity.Name;
                char[] delimitar = new char[] { '~' };
                string[] partz = CurrentUser.Split(delimitar, StringSplitOptions.RemoveEmptyEntries);
                var uzar = np.Getlogin(partz[1]);
                exp.AccountName = part[0];
                exp.AccountCode = part[1];
                exp.Dated = DateTime.Now;
                exp.cashier = partz[1];
                exp.Payee = exp.type;
                exp.type = "RECEIPT";
                long id = np.AddExpenses(exp);
                var expns = np.GetExpenses(id);
                expns.Receipt = uzar.prefix.Trim() + "-" + id;
                np.SaveOrUpdateExpenses(expns);

                var px = np.GetSalesOpenCredit(exp.AccountName, exp.till);
                //var px = np.GetPurchasesOpenCredit(exp.Payee, exp.till);
                foreach (var item in px)
                {
                    for (int i = 0; i < invoices.Count; i++)
                    {
                        if (item.Reciept.Trim() == invoices[i])
                        {
                            if (item.Balance == null) item.Balance = item.total;
                            item.Balance = item.Balance - decimal.Parse(balances[i])    ;
                            if (item.Balance == 0)
                            {
                                item.state  = "C";
                            }
                            np.SaveOrUpdateSales(item);
                        }
                    }
                }

                Transaction trn = new Transaction();

                trn.Process(exp.Payee, decimal.Parse(exp.Amount.ToString()), exp.till, exp.AccountName);
                var acc = np.GetTransactionsByType("RECEIPT");
                var comp = np.GetCompanies(exp.till);

                Expens ex = new Expens();
                List<SelectListItem> yr = new List<SelectListItem>();
                List<SelectListItem> typ = new List<SelectListItem>();
                yr.Add(new SelectListItem
                {
                    Text = "--Select Account--",
                    Value = ""
                });

                typ.Add(new SelectListItem
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
                return RedirectToAction("listExpenses", new { Company = exp.till});
            }
            else
            {
                exp.cashier = "Please make sure the invoices are knocked off";
                var acc = np.GetTransactionsByType("RECEIPT");
                var comp = np.GetCompanies(exp.till);

                Expens ex = new Expens();
                List<SelectListItem> yr = new List<SelectListItem>();
                List<SelectListItem> typ = new List<SelectListItem>();
                yr.Add(new SelectListItem
                {
                    Text = "--Select Account--",
                    Value = ""
                });

                typ.Add(new SelectListItem
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
                return PartialView(exp);
            }
      
        }

        public ActionResult Details(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Account account = np.GetAccounts(id);
            return View(account);
        }

        //
        // GET: /Accounts/Create
        public ActionResult secondLevel(string json_str)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var acc = np.GetAccountsByCode(json_str);
            
            List<SelectListItem> typ = new List<SelectListItem>();
           

            typ.Add(new SelectListItem
            {
                Text = "Select Account",
                Value = "Select Account"
            });

            foreach (var item in acc)
            {
            if (item.AccountCode.Length == 4)
                {
                    typ.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }
            }

            return Json(typ, JsonRequestBehavior.AllowGet);
        }

        public ActionResult AccountNumber(string json_str, string level)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var usa = User.Identity.Name;
            var acc = db.Accounts.Where(u => u.AccountCode.StartsWith(json_str)).ToList();
            var code = new Account();
            string typ = "";
            int numb = 0;
            if (level == "SECOND")
            {
                code = acc.OrderByDescending(u => u.AccountCode).Where(u => u.AccountCode.Length == 4).FirstOrDefault();
                if (code == null)
                {
                    numb = 1;
                }
                else { 
                numb = int.Parse(code.AccountCode.Substring(2, code.AccountCode.Length - 2)) + 1;
                }
                if(numb < 10)
                {
                    typ = "0" + numb;
                }
                else
                {
                    typ = numb.ToString();
                }
            }
            else if (level == "THIRD")
            {
                code = acc.OrderByDescending(u => u.AccountCode).Where(u => u.AccountCode.Length > 4).FirstOrDefault();
                if (code == null)
                {
                    numb = 1;
                }
                else
                {
                    numb = int.Parse(code.AccountCode.Substring(5, code.AccountCode.Length - 5)) + 1;
                }
                if (numb < 10)
                {
                    typ = "000" + numb;
                }
                else if (numb > 9 && numb < 100)
                {
                    typ = "00" + numb;
                }
                else if (numb > 99 && numb < 1000)
                {
                    typ = "0" + numb;
                }
                else
                {
                    typ = numb.ToString();
                }
                    
            }
            return Json(typ, JsonRequestBehavior.AllowGet);
        } 
        //
        // POST: /Accounts/Create
        [Authorize]
        [HttpGet]
        public ActionResult Create(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var acc = np.GetAllAccounts();
            List<SelectListItem> yr = new List<SelectListItem>();
            List<SelectListItem> typ = new List<SelectListItem>();
            List<SelectListItem> comp = new List<SelectListItem>();
            List<SelectListItem> cur = new List<SelectListItem>();
            var co = np.GetAllActiveco();
            yr.Add(new SelectListItem
            {
                Text = "Select An Account",
                Value = "Select An Account"
            });


            typ.Add(new SelectListItem
            {
                Text = "Select Account type",
                Value = "Select Account type"
            });

            foreach (var item in acc)
            {
                if (item.AccountCode.Length == 1)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }
            }

            foreach (var item in co)
            {
                comp.Add(new SelectListItem
                {
                    Text = item.company,
                    Value = item.CompanyId.ToString()
                });
            }

           var curr = db.Currencies.Where(u => u.Company == Company).ToList();
            var bas = curr.Where(u => u.IsBase == true).FirstOrDefault();
            foreach (var item in curr)
            {
                cur.Add(new SelectListItem
                {
                    Text = item.Curency,
                    Value = item.Id.ToString()
                });
            }

            

            var coid = (from e in co where e.company == Company select e).FirstOrDefault().ID;
            Account ac = new Account();
            
            ac.Currency = bas.Curency;
            ac.CurrencyId = bas.Id;
            ViewBag.Accountz = yr;
            ViewBag.Type = typ;
            ViewBag.Company = comp;
            ViewBag.Currency = cur;
            ViewData["Comp"] = coid;
            return PartialView(ac);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Create(Account account, HttpPostedFileBase Image)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Company conp = new Company();
            var accnt = db.Accounts.Where(u => u.AccountCode.StartsWith(account.Level1)).ToList();
            var code = new Account();
            if(string.IsNullOrEmpty(account.AccountCode))
            { 
                int numb = 0;
                if (account.Level2 == null)
                {
                    code = accnt.OrderByDescending(u => u.AccountCode).Where(u => u.AccountCode.Length == 4).FirstOrDefault();
                    if (code != null)
                    {
                        numb = int.Parse(code.AccountCode.Substring(2, code.AccountCode.Length - 2)) + 1;
                        if (numb < 10)
                        {
                            account.Level2 = "0" + numb;
                        }
                        else
                        {
                            account.Level2 = numb.ToString();
                        }
                    }
                    else
                    {
                        account.Level2 = "01";
                    }
                }
                else if (account.Level2 != null && account.Level3 == null)
                {
                    code = accnt.OrderByDescending(u => u.AccountCode).Where(u => u.AccountCode.Length > 4).FirstOrDefault();
                    numb = int.Parse(code.AccountCode.Substring(5, code.AccountCode.Length - 5)) + 1;
                    if (numb < 10)
                    {
                        account.Level3 = "000" + numb;
                    }
                    else if (numb > 9 && numb < 100)
                    {
                        account.Level3 = "00" + numb;
                    }
                    else if (numb > 99 && numb < 1000)
                    {
                        account.Level3 = "0" + numb;
                    }
                    else
                    {
                        account.Level3 = numb.ToString();
                    }
                }
            }
            
            if (ModelState.IsValid  && account.AccountName != null)
            {
                if (Image != null)
                {
                    #region itemimage
                    try
                    {

                        var fileName = Image.FileName;
                        char[] delimiter = new char[] { '.' };
                        string[] parts = fileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        var ext = parts[1];
                        string regExp = "[^a-zA-Z0-9]";
                        fileName = Regex.Replace(parts[0], regExp, "_");

                        var thumbname = fileName + "_Thumb" + "." + ext;
                        fileName = fileName + "." + ext;
                        var temp = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/"), fileName);
                        var pathDir = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"));
                        var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), fileName);
                        var thumbPath = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), thumbname);
                        var image = Image;
                        var Thimage = image;
                        if (System.IO.Directory.Exists(pathDir))
                        {
                            image.SaveAs(temp);
                        }
                        else
                        {
                            Directory.CreateDirectory(pathDir);
                            image.SaveAs(temp);

                        }

                        ResizeImage(temp, path, 166, 100);

                        if (System.IO.File.Exists(temp))
                        {
                            // Use a try block to catch IOExceptions, to 
                            // handle the case of the file already being 
                            // opened by another process. 
                            try
                            {
                                System.IO.File.Delete(temp);
                            }
                            catch (System.IO.IOException e)
                            {

                            }
                        }

                        account.Icon  = "~/Content/Template/Images/Partners/" + fileName;
                        account.Icon = "~/Content/Template/Images/Partners/" + fileName;

                    }
                    catch (Exception)
                    {
                    }

                    #endregion
                }

                if ((account.Level2 == null || account.Level2 == "") && (account.Level3 == null || account.Level3 == "") && string.IsNullOrEmpty(account.AccountCode))
                {
                    account.AccountCode = account.Level1;
                }
                else if (string.IsNullOrEmpty(account.Level3) && !string.IsNullOrEmpty(account.Level2))
                {
                    account.AccountCode = account.Level1 + "-" + account.Level2;
                }
                else if(string.IsNullOrEmpty(account.AccountCode))
                {
                    account.AccountCode = account.Level2 + "-" + account.Level3;
                }
                account.LinkAccount = db.Accounts.Where(u => u.AccountCode.Trim() == account.Level1).FirstOrDefault().LinkAccount;
                account.AccountName = account.AccountName.ToUpper();
                var ck = np.GetAccountsByCode(account.AccountCode);
                ck = (from e in ck where e.CompanyId == account.CompanyId select e).ToList();
                if (ck == null || ck.Count == 0)
                {
                    np.SaveOrUpdateAccounts(account);
                    conp = db.Companies.Find(account.CompanyId);
                    if(Request.IsAjaxRequest()) return RedirectToAction("Index", new { company = conp.name});
                    return RedirectToAction("Index", new { company = conp.name });
                }
                ModelState.AddModelError("", "This account code is already in use");
            }
            else
            {
                ModelState.AddModelError("", "Make Sure all fields are entered");
            }
            var acc = np.GetAllAccounts();
            List<SelectListItem> yr = new List<SelectListItem>();
            List<SelectListItem> typ = new List<SelectListItem>();
            List<SelectListItem> comp = new List<SelectListItem>();
            List<SelectListItem> cur = new List<SelectListItem>();
            var co = np.GetAllActiveco();
            yr.Add(new SelectListItem
            {
                Text = "Select An Account",
                Value = "Select An Account"
            });


            typ.Add(new SelectListItem
            {
                Text = "Select Account type",
                Value = "Select Account type"
            });

            foreach (var item in acc)
            {
                if (item.AccountCode.Length == 1)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }
            }

            foreach (var item in co)
            {
                comp.Add(new SelectListItem
                {
                    Text = item.company,
                    Value = item.CompanyId.ToString()
                });
            }

            var curr = db.Currencies.Where(u => u.Company == conp.name).ToList();
            var bas = curr.Where(u => u.IsBase == true).FirstOrDefault();
            foreach (var item in curr)
            {
                cur.Add(new SelectListItem
                {
                    Text = item.Curency,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Currency = cur;

            ViewBag.Accountz = yr;
            ViewBag.Type = typ;
            ViewBag.Company = comp;
            ViewData["Comp"] = account.CompanyId;
           // ViewData["Company"] = Company;
            if(Request.IsAjaxRequest())return PartialView();
           return View();
        }
        
        //
        // GET: /Accounts/Edit/5
        [Authorize]
        [HttpGet]
        public ActionResult Edit(int id, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Account account = np.GetAccounts(id);
            var acc = np.GetAllAccounts();
            List<SelectListItem> yr = new List<SelectListItem>();
            List<SelectListItem> typ = new List<SelectListItem>();
            List<SelectListItem> comp = new List<SelectListItem>();
            List<SelectListItem> cur = new List<SelectListItem>();
            var co = np.GetAllActiveco();
            yr.Add(new SelectListItem
            {
                Text = "Select An Account",
                Value = "Select An Account"
            });


            typ.Add(new SelectListItem
            {
                Text = "Select Account type",
                Value = "Select Account type"
            });

            foreach (var item in acc)
            {
                if (item.AccountCode.Length == 1)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }

            }

            foreach (var item in co)
            {
                comp.Add(new SelectListItem
                {
                    Text = item.company,
                    Value = item.CompanyId.ToString()
                });
            }

            var curr = db.Currencies.Where(u => u.Company == Company).ToList();
            var bas = curr.Where(u => u.IsBase == true).FirstOrDefault();
            foreach (var item in curr)
            {
                cur.Add(new SelectListItem
                {
                    Text = item.Curency,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Currency = cur;

            var coid = (from e in co where e.company == Company select e).FirstOrDefault().ID;
            char[] delimiter = new char[] { '-' };
            string[] part = account.AccountCode.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            account.Level1 = part[0];
            if (string.IsNullOrEmpty(account.Currency) && curr.Count >0)
            { 
                account.Currency = curr.FirstOrDefault().Curency;
            }
            if (part.Length >= 2) account.Level2 = part[1];
            if(part.Length ==3 ) account.Level3 = part[2];
            ViewBag.Accountz = yr;
            ViewBag.Type = typ;
            ViewBag.Company = comp;
            ViewData["Comp"] = coid;
            return PartialView(account);
        }

        [Authorize]
        [HttpPost]
        public ActionResult Edit(Account account, HttpPostedFileBase Image)
        {
            string conam ="";
            NHibernateDataProvider np = new NHibernateDataProvider();
            if (ModelState.IsValid)
            {
                if (Image != null)
                {
                    #region itemimage
                    try
                    {

                        var fileName = Image.FileName;
                        char[] delimiter = new char[] { '.' };
                        string[] parts = fileName.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
                        var ext = parts[1];
                        string regExp = "[^a-zA-Z0-9]";
                        fileName = Regex.Replace(parts[0], regExp, "_");

                        var thumbname = fileName + "_Thumb" + "." + ext;
                        fileName = fileName + "." + ext;
                        var temp = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/"), fileName);
                        var pathDir = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"));
                        var path = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), fileName);
                        var thumbPath = Path.Combine(HttpContext.Server.MapPath("~/Content/Template/Images/Partners/"), thumbname);
                        var image = Image;
                        var Thimage = image;
                        if (System.IO.Directory.Exists(pathDir))
                        {
                            image.SaveAs(temp);
                        }
                        else
                        {
                            Directory.CreateDirectory(pathDir);
                            image.SaveAs(temp);

                        }

                        ResizeImage(temp, path, 166, 100);

                        if (System.IO.File.Exists(temp))
                        {
                            // Use a try block to catch IOExceptions, to 
                            // handle the case of the file already being 
                            // opened by another process. 
                            try
                            {
                                System.IO.File.Delete(temp);
                            }
                            catch (System.IO.IOException e)
                            {

                            }
                        }

                        account.Icon = "~/Content/Template/Images/Partners/" + fileName;
                        account.Icon = "~/Content/Template/Images/Partners/" + fileName;

                    }
                    catch (Exception)
                    {
                    }

                    #endregion
                }

                conam = np.GetCompaniesById((long)account.CompanyId).name;
                np.UpdateAccounts(account);
                return RedirectToAction("Index",new { company =conam});
            }
            else
            {
                ModelState.AddModelError("", "Make Sure all fields are entered");
            }
            var acc = np.GetAllAccounts();
            List<SelectListItem> yr = new List<SelectListItem>();
            List<SelectListItem> typ = new List<SelectListItem>();
            List<SelectListItem> comp = new List<SelectListItem>();
            List<SelectListItem> cur = new List<SelectListItem>();
            var co = np.GetAllActiveco();
            yr.Add(new SelectListItem
            {
                Text = "Select An Account",
                Value = "Select An Account"
            });


            typ.Add(new SelectListItem
            {
                Text = "Select Account type",
                Value = "Select Account type"
            });

            foreach (var item in acc)
            {
                if (item.AccountCode.Length == 1)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }
            }

            foreach (var item in co)
            {
                comp.Add(new SelectListItem
                {
                    Text = item.company,
                    Value = item.CompanyId.ToString()
                });
            }

            var curr = db.Currencies.Where(u => u.Company == conam).ToList();
            var bas = curr.Where(u => u.IsBase == true).FirstOrDefault();
            foreach (var item in curr)
            {
                cur.Add(new SelectListItem
                {
                    Text = item.Curency,
                    Value = item.Id.ToString()
                });
            }
            ViewBag.Currency = cur;

            ViewBag.Accountz = yr;
            ViewBag.Type = typ;
            ViewBag.Company = comp;
            
            // ViewData["Company"] = Company;
            if (Request.IsAjaxRequest()) return PartialView(account);
            return View(account);
            //return PartialView(account);
        }

        //
        // GET: /Accounts/Delete/5
       [Authorize]
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Account account = np.GetAccounts(id);
            return View(account);
        }

        //
        // POST: /Accounts/Delete/5
        [Authorize]
        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Account account)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            np.DeleteAccounts(account);
            return RedirectToAction("Index");
        }


        #region Currency
        [Authorize]
        public ActionResult CurrencyList(string Company, JQueryDataTableParamModel param)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            // JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (string.IsNullOrEmpty(Company))
            {
                Company = part[1];
            }
            var px = db.Currencies.Where(u => u.Company == Company).ToList();

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

        [Authorize]
        [HttpGet]
        public ActionResult CreateCurrency(string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var acc = np.GetAllAccounts();
            List<SelectListItem> yr = new List<SelectListItem>();
            List<SelectListItem> typ = new List<SelectListItem>();
            List<SelectListItem> comp = new List<SelectListItem>();
            List<SelectListItem> cur = new List<SelectListItem>();
            var co = np.GetAllActiveco();
            yr.Add(new SelectListItem
            {
                Text = "Select An Account",
                Value = "Select An Account"
            });


            typ.Add(new SelectListItem
            {
                Text = "Select Account type",
                Value = "Select Account type"
            });

            foreach (var item in acc)
            {
                if (item.AccountCode.Length == 1)
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountCode
                    });
                }
            }

            foreach (var item in co)
            {
                comp.Add(new SelectListItem
                {
                    Text = item.company,
                    Value = item.company.ToString()
                });
            }


            var coid = (from e in co where e.company == Company select e).FirstOrDefault().ID;
            Currency cr = new Currency();
            cr.Company = Company;
            ViewBag.Accountz = yr;
            ViewBag.Type = typ;
            ViewBag.Company = comp;
            ViewBag.Currency = cur;
            ViewData["Comp"] = coid;
            return PartialView(cr);
        }

        [Authorize]
        [HttpPost]
        public ActionResult CreateCurrency(Currency currency, HttpPostedFileBase Image)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Company conp = new Company();
            var accnt = db.Currencies.Where(u => u.Curency == currency.Curency && u.Company == currency.Company).ToList();
            if (accnt.Count() == 0)
            {
                db.Entry(currency).State = EntityState.Added;
                db.SaveChanges();
                return RedirectToAction("CurrencyList", new { Company = currency.Company });
            }

            ModelState.AddModelError("", "This account code is already in use");

            if (Request.IsAjaxRequest()) return PartialView();
            return View();
        }


        [Authorize]
        [HttpGet]
        public ActionResult changeBaseCurrency(string Company, JQueryDataTableParamModel param)
        {

            NHibernateDataProvider np = new NHibernateDataProvider();
            // JQueryDataTableParamModel param = new JQueryDataTableParamModel();
            var CurrentUser = User.Identity.Name;
            char[] delimiter = new char[] { '~' };
            string[] part = CurrentUser.Split(delimiter, StringSplitOptions.RemoveEmptyEntries);
            if (string.IsNullOrEmpty(Company))
            {
                Company = part[2];
            }



            var px = db.Currencies.Where(u => u.Company.Trim() == Company).ToList();



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


        [Authorize]
        [HttpPost]
        public ActionResult changeBaseCurrency(string itemz)
        {
            //NHibernateDataProvider np = new NHibernateDataProvider();
            //db.Entry(px).State = EntityState.Modified;
            //np.SaveOrUpdateCurrencies(px);
            NHibernateDataProvider np = new NHibernateDataProvider();
            var CurrentUser = User.Identity.Name;
            char[] delimiterr = new char[] { '~' };
            string[] partz = CurrentUser.Split(delimiterr, StringSplitOptions.RemoveEmptyEntries);
            var Company = partz[2];
            var user = np.Getlogin(partz[1]);

            

            var basecurr = db.Currencies.Where(u => u.IsBase == true && u.Company.Trim() == Company).FirstOrDefault();
            var stock = db.Items.Where(u => u.company.Trim() == user.Location).ToList();

            char[] delimiters = new char[] { '/' };
            string[] parts = itemz.Split(delimiters, StringSplitOptions.RemoveEmptyEntries);

            foreach (var item in parts)
            {
                char[] delimite = new char[] { ',' };
                string[] cur = item.Split(delimite, StringSplitOptions.RemoveEmptyEntries);
                int id = Convert.ToInt16(cur[0]);
                Currency updcurr = new Currency();
                var currency = db.Currencies.Find(id);
                if (cur[1] == "true")
                {
                    currency.IsBase = true;
                }
                else
                {
                    currency.IsBase = false;
                }
                currency.ExchangeRate = Convert.ToDecimal(cur[2]);

                db.Currencies.Add(updcurr);

                db.SaveChanges();
                //NHibernateDataProvider np = new NHibernateDataProvider();
                //db.Entry(cur).State = EntityState.Modified;
                //np.SaveOrUpdateCurrencies(cur);
            }



            var exchangeRateReciprocal = 1 / (basecurr.ExchangeRate);
            
            

            
            foreach (var itm in stock)
            {
                //Item stoc = new Item();
                itm.SellingPrice = exchangeRateReciprocal * itm.SellingPrice;
               // db.Items.Add(itm);
               // db.SaveChanges();
                np.UpdateItems(itm);
            }
            var comp = db.Companies.Where(u => u.name == user.Location).FirstOrDefault();
            var acc = db.Accounts.Where(u => u.CompanyId == comp.ID && u.AccountCode.StartsWith("2") && u.AccountCode.Length > 4).ToList();
            foreach (var acclist in acc)
            {
                //Account acnts = new Account();
                acclist.Currency = basecurr.Curency;
                acclist.CurrencyId = basecurr.Id;
                acclist.Balance = acclist.Balance * exchangeRateReciprocal;
                //db.Accounts.Add(acclist);
                np.UpdateAccounts(acclist);
            }

            return RedirectToAction ("CurrencyList",new { Company });

        }

      
        [Authorize]
        [HttpGet]
        public ActionResult EditCurrency(long Id, string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var px = db.Currencies.Find(Id);
            List<SelectListItem> typ = new List<SelectListItem>();
            List<SelectListItem> comp = new List<SelectListItem>();
            List<SelectListItem> cur = new List<SelectListItem>();
            var co = np.GetAllActiveco();

            typ.Add(new SelectListItem
            {
                Text = "Select Account type",
                Value = "Select Account type"
            });


            foreach (var item in co)
            {
                comp.Add(new SelectListItem
                {
                    Text = item.company,
                    Value = item.company.ToString()
                });
            }

            var coid = (from e in co where e.company == Company select e).FirstOrDefault().ID;

            ViewBag.Type = typ;
            ViewBag.Company = comp;
            ViewBag.Currency = cur;
            ViewData["Comp"] = coid;
            return PartialView(px);
        }

        [Authorize]
        [HttpPost]
        public ActionResult EditCurrency(Currency currency, HttpPostedFileBase Image)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Company conp = new Company();
            // var accnt = db.Currencies.Where(u => u.Curency == currency.Curency && u.Company == currency.Company).ToList();
            if (ModelState.IsValid)
            {
                db.Entry(currency).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("CurrencyList", new { Company = currency.Company });
            }

            ModelState.AddModelError("", "This account code is already in use");

            if (Request.IsAjaxRequest()) return PartialView();
            return View();
        }

        #endregion


        protected override void Dispose(bool disposing)
        {
            if (disposing) {
               // np.dis();
            }
            base.Dispose(disposing);
        }
    }
}