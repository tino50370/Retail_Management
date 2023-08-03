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
    public class TransactionsController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        private RetailKingEntities db = new RetailKingEntities();
        //
        // GET: /Accounts/

        public ActionResult Index()
        {
            //NHibernateDataProvider np = new NHibernateDataProvider();
            var px = db.TransactionDatas.ToList();// np.GetAllTransactions();
            return PartialView(px);
        }

        //
        // GET: /Accounts/Details/5

        public ActionResult Details(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Account account = db.Accounts.Find(id);// np.GetAccounts(id);
            return PartialView(account);
        }

        //
        // GET: /Accounts/Create

        public ActionResult Create( string Company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var comp = np.GetCompanies(Company);
            var accx = np.GetAllAccounts();
            var acc = (from e in accx
                       where e.CompanyId == comp.ID
                       select e).ToList();
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
                if (item.AccountCode.Length == 1)
                {
                    typ.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountName
                    });

                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountName
                    });
                }
                else
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountName
                    });
                }
            }

            ViewBag.Accountz = yr;
            ViewBag.Type = typ;
            return PartialView();
        } 

        //
        // POST: /Accounts/Create

        [HttpPost]
        public ActionResult Create(TransactionData trn)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                trn.TransactionName = trn.TransactionName.ToUpper();
                np.AddTransactions(trn);
                return RedirectToAction("Index"); 
            }

            return PartialView(trn);
        }
        
        //
        // GET: /Accounts/Edit/5
 
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var account = np.GetTransactions(id);
            var acc = np.GetAllAccounts();
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
                if (item.AccountCode.Length == 1)
                {
                    typ.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountName
                    });

                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountName
                    });
                }
                else
                {
                    yr.Add(new SelectListItem
                    {
                        Text = item.AccountName,
                        Value = item.AccountName
                    });
                }
            }

            ViewBag.Accountz = yr;
            ViewBag.Type = typ;
            return PartialView(account);
        }

        //
        // POST: /Accounts/Edit/5

        [HttpPost]
        public ActionResult Edit(TransactionData trn)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider();
                np.SaveOrUpdateTransactions(trn);
                return RedirectToAction("Index");
            }
            return PartialView(trn);
        }

        //
        // GET: /Accounts/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            Account account = np.GetAccounts(id);
            return View(account);
        }

        //
        // POST: /Accounts/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Account account)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            np.DeleteAccounts(account);
            return RedirectToAction("Index");
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