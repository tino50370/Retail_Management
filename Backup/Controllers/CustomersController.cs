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
    public class CustomersController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();

        //
        // GET: /Customers/

        public ViewResult Index()
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            return View(np.GetAllCustomers());
        }

        //
        // GET: /Customers/Details/5

        public ViewResult Details(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Customers customer = np.GetCustomers(id);
            return View(customer);
        }

        //
        // GET: /Customers/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Customers/Create

        [HttpPost]
        public ActionResult Create(Customers customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            if (ModelState.IsValid)
            {
                np.SaveOrUpdateCustomers(customer);
               
                return RedirectToAction("Index");  
            }

            return View(customer);
        }
        
        //
        // GET: /Customers/Edit/5
 
        public ActionResult Edit(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Customers customer = np.GetCustomers(id);
            return View(customer);
        }

        //
        // POST: /Customers/Edit/5

        [HttpPost]
        public ActionResult Edit(Customers customer)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
                np.SaveOrUpdateCustomers(customer);
                return RedirectToAction("Index");
            }
            return View(customer);
        }

        //
        // GET: /Customers/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Customers customer = np.GetCustomers(id);
            return View(customer);
        }

        //
        // POST: /Customers/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Customers customer = np.GetCustomers(id);
            np.SaveOrUpdateCustomers(customer);
            
            return RedirectToAction("Index");
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