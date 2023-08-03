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
    public class AccountsController : Controller
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();

        //
        // GET: /Accounts/

        public ViewResult Index()
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            var px = np.GetAllAccounts();
            return View(px);
        }

        //
        // GET: /Accounts/Details/5

        public ViewResult Details(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Accounts account = np.GetAccounts(id);
            return View(account);
        }

        //
        // GET: /Accounts/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Accounts/Create

        [HttpPost]
        public ActionResult Create(Accounts account)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());  
                np.SaveOrUpdateAccounts(account);
                return RedirectToAction("Index");  
            }

            return View(account);
        }
        
        //
        // GET: /Accounts/Edit/5
 
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Accounts account = np.GetAccounts(id);
            return View(account);
        }

        //
        // POST: /Accounts/Edit/5

        [HttpPost]
        public ActionResult Edit(Accounts account)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
                np.SaveOrUpdateAccounts(account);
                return RedirectToAction("Index");
            }
            return View(account);
        }

        //
        // GET: /Accounts/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Accounts account = np.GetAccounts(id);
            return View(account);
        }

        //
        // POST: /Accounts/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(Accounts account)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
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