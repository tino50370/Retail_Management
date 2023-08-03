using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Xml.Linq;
using System.IO;
using RetailKing.Models;
using System.Web.Helpers;
using System.Web.Hosting;
using Raven.Client;
using RetailKing.Controllers;
using Raven.Abstractions.Data;
using Raven.Json.Linq;
using Raven.Client.Indexes;
using Raven.Abstractions.Indexing;
using crypto;
using System.Net;
using RetailKing.DataAcess;


namespace RetailKing.RavendbClasses
{
    public class XmlDataMethods <T,V>: RavenController
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        #region Loyalty
        public T GetCollection(string Id,string typename)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<T>(typename + "/" + Id);
            return gx;
        }

 
        public Loyalty AddCustomerPoints(Loyalty points)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            
            /// add member to supplier/service provider
            #region membership
            var memba = new Loyalty();
            
            var mm = RavenSession.Load<SupplierMembers>("LoyalMembers/" + points.SupplierId);
            if (mm != null)
            {

                memba =mm.members.Where(u => u.Id == points.Id).FirstOrDefault();
                memba.Points = memba.Points + points.Points;  
            }

            #endregion

            #region Customer
            var pnts = new Loyalty();

            var cus = RavenSession.Load<CustomerProfile>("Profile/" + points.PhoneNumber);
            if (cus != null)
            {
                pnts = cus.LoyaltySchemes.Where(u => u.Id == points.Id).FirstOrDefault();
                pnts.Points = pnts.Points + points.Points;
               
                RavenSession.Store(cus);
            }
            else
            {
                DocumentStore.Conventions.RegisterIdConvention<CustomerProfile>((dbname, commands, post) => "loyalty/" + points.PhoneNumber);
                var lpts = new List<Loyalty>();
                var nwx = RavenSession.Load<CustomerProfile>("Profile/" + points.PhoneNumber);
                if (nwx == null) nwx = new CustomerProfile();
                nwx.Name = points.Name;
               // nwx.acc = points.Id;
                //nwx.DOB 
                lpts.Insert(0, points);
                nwx.LoyaltySchemes = lpts;
                RavenSession.Store(nwx);  
            }

            #endregion

            //RavenSession.SaveChanges();
            //RavenSession.Dispose();
            return memba;
        }

        #endregion

    

    }
}
