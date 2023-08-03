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
    public class CompaniesController : Controller
    {
         NHibernateSessionManager ns = new NHibernateSessionManager();

        //
        // GET: /Companies/

        public ViewResult Index()
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            return View(np.GetAllCompanies());
        }

        //
        // GET: /Companies/Details/5

        public ViewResult Details(long id)
        {
             NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
             Companies company = np.GetCompaniesById(id);
            return View(company);
        }

        //
        // GET: /Companies/Create

        public ActionResult Create()
        {
            return View();
        } 

        //
        // POST: /Companies/Create

        [HttpPost]
        public ActionResult Create(Companies company)
        {
            if (ModelState.IsValid)
            {
                NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
                np.SaveOrUpdateCompanies(company);
                return RedirectToAction("Index");  
            }

            return View(company);
        }
        
        //
        // GET: /Companies/Edit/5
 
        public ActionResult Edit(long id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Companies company = np.GetCompaniesById(id);
            return View(company);
        }

        //
        // POST: /Companies/Edit/5

        [HttpPost]
        public ActionResult Edit(Companies company)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            if (ModelState.IsValid)
            {
                np.SaveOrUpdateCompanies(company);
                
                return RedirectToAction("Index");
            }
            return View(company);
        }

        //
        // GET: /Companies/Delete/5
 
        public ActionResult Delete(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());

            Companies company = np.GetCompaniesById(id);
            return View(company);
        }

        //
        // POST: /Companies/Delete/5

        [HttpPost, ActionName("Delete")]
        public ActionResult DeleteConfirmed(int id)
        {
            NHibernateDataProvider np = new NHibernateDataProvider(ns.GetSession());
            Companies company = np.GetCompaniesById(id);
            np.DeleteCompanies(company);
           
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