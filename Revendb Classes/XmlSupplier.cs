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
    public class XmlSupplier : RavenController
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        #region Profile
        public void CreateCustomerProfile(Supplier supplier)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var lpts = new List<Loyalty>();

            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + supplier.AccountCode);
            if (gx == null)
            {
                DocumentStore.Conventions.RegisterIdConvention<SupplierProfile>((dbname, commands, post) => "SupplierProfile/" + supplier.AccountCode);

                var nwx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + supplier.AccountCode);
                if (nwx == null) nwx = new SupplierProfile();
                nwx.Id = "SupplierProfile/" + supplier.AccountCode;
                
                RavenSession.Store(nwx);
                RavenSession.SaveChanges();
                RavenSession.Dispose();

            }

        }

        public void updateProfileString(string Id, string parameter, string value)
        {
            DocumentStoreHolder.DocumentStore.DatabaseCommands.Patch(
            Id,
            new[]
                {
                    new PatchRequest
                        {
                            Type = PatchCommandType.Set,
                            Name = parameter,
                            Value = value
                        }
                });
        }

        public SupplierProfile GetSupplierProfileId(string Id)
        {

            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
          
            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + Id);
            return gx;
        }

       
        #endregion

        #region Loyalty
        public SupplierMembers GetAllLoyalCustomers(string SupplierId)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<SupplierMembers>("LoyalMembers/" + SupplierId);
            RavenSession.Dispose();
            return gx;
        }

        public Loyalty GetLoyalCustomers(string SupplierId,  string Id)
        {
            if(Id.StartsWith("8") && Id.Length == 12)
            {
               Id = Id.Substring(0, 1) + "-" + Id.Substring(1, 4) + "-" + Id.Substring(5, 7);
            }
            else if (Id != null && Id.Length == 10)
            {
                Id = "263" + Id.Substring(1, Id.Length - 1);
            }
            else if (Id != null && Id.Length == 9)
            {
                Id = "263" + Id;
            }
            else if (Id != null && Id.Length == 12)
            {
            }

            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<SupplierMembers>("LoyalMembers/" + SupplierId);
            var lm = new Loyalty();
            if (gx != null)
            {
                lm = (from e in gx.members where(e.Id == Id || e.CardNumber == Id || e.PhoneNumber == Id) select e).FirstOrDefault();
            }
            RavenSession.Dispose();
            return lm;
        }

        public Loyalty AddLoyaltyPoints(string SupplierId, string Id, int points, decimal monetary)
        {

            if (Id.StartsWith("8") && Id.Length == 12)
            {
                Id = Id.Substring(0, 1) + "-" + Id.Substring(1, 4) + "-" + Id.Substring(5, 7);
            }
            else if (Id != null && Id.Length == 10)
            {
                Id = "263" + Id.Substring(1, Id.Length - 1);
            }
            else if (Id != null && Id.Length == 9)
            {
                Id = "263" + Id;
            }
            else if (Id != null && Id.Length == 12)
            {
            }
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<SupplierMembers>("LoyalMembers/" + SupplierId);
            var lm = new Loyalty();
            if (gx != null)
            {
                
                lm = gx.members.Where(u => u.Id == Id || u.CardNumber == Id || u.PhoneNumber == Id).FirstOrDefault();
                //var monetary = lm.MonetaryValue / lm.Points;
                if(points < 0)
                {
                    points = points * (-1);
                }
                lm.Points = lm.Points + points;
                lm.MonetaryValue = lm.Points * monetary;
                lm.LastAccess = DateTime.Now;
                RavenSession.Store(gx);

                #region customer
                
                var cx = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == lm.PhoneNumber).FirstOrDefault();
                var pp = cx.LoyaltySchemes.Where(u => u.SupplierId == SupplierId).FirstOrDefault();
               
                pp.Points = lm.Points;
                pp.MonetaryValue = lm.MonetaryValue;
                cx.DateLastAccessed = DateTime.Now;
                RavenSession.Store(cx);
                #endregion

                RavenSession.SaveChanges();
                RavenSession.Dispose();
            }

            return lm;
        }

        public Loyalty RedeemLoyaltyPoints(string SupplierId, string Id, int points)
        {

            if (Id.StartsWith("8") && Id.Length == 12)
            {
                Id = Id.Substring(0, 1) + "-" + Id.Substring(1, 4) + "-" + Id.Substring(5, 7);
            }
            else if (Id != null && Id.Length == 10)
            {
                Id = "263" + Id.Substring(1, Id.Length - 1);
            }
            else if (Id != null && Id.Length == 9)
            {
                Id = "263" + Id;
            }
            else if (Id != null && Id.Length == 12)
            {
            }
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<SupplierMembers>("LoyalMembers/" + SupplierId);
            var lm = new Loyalty();
            if (gx != null)
            {
                lm = gx.members.Where(u => u.Id == Id || u.CardNumber == Id || u.PhoneNumber == Id).FirstOrDefault();
                lm.Points = lm.Points - points;
                if (lm.Points < 0)
                {
                    lm.Points = lm.Points * -1;
                }
                RavenSession.Store(lm);

                #region customer
                //var cx = RavenSession.Load<CustomerProfile>("Profile/" + lm.PhoneNumber);
                var cx = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == lm.PhoneNumber).FirstOrDefault();
                var pp = cx.LoyaltySchemes.Where(u => u.SupplierId == SupplierId.Trim()).FirstOrDefault();
                pp.Points = pp.Points - points;
                RavenSession.Store(cx);
                #endregion

              
                RavenSession.SaveChanges();
                
            }
            RavenSession.Dispose();
            return lm;
        }

        public CustomerProfile CreateCustomerLoyalty(Loyalty loyalty)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var lpts = new List<Loyalty>();
  
            var gx = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == loyalty.PhoneNumber).FirstOrDefault();
            if (gx == null)
            {
                DocumentStore.Conventions.RegisterIdConvention<CustomerProfile>((dbname, commands, post) => "Profile/" + loyalty.Id);

                var nwx = RavenSession.Load<CustomerProfile>("Profile/" + loyalty.PhoneNumber);
                if (nwx == null) nwx = new CustomerProfile();

                lpts.Insert(0, loyalty);
                nwx.LoyaltySchemes = lpts;
                RavenSession.Store(nwx);  
            }
            else
            {
                gx.LoyaltySchemes.Insert(0, loyalty);
                RavenSession.Store(gx);  
            }

            /// add member to supplier/service provider
            #region membership
            var mlist = new List<Loyalty>();
           
            var mm = RavenSession.Load<SupplierMembers>("LoyalMembers/" + loyalty.SupplierId);
            if(mm == null)
            {
                DocumentStore.Conventions.RegisterIdConvention<SupplierMembers>((dbname, commands, post) => "LoyalMembers/" + loyalty.SupplierId);
                var nwx = RavenSession.Load<SupplierMembers>("LoyalMembers/" + loyalty.SupplierId);
                if (nwx == null) nwx = new SupplierMembers();

                mlist.Insert(0, loyalty);
                nwx.members = mlist;
                RavenSession.Store(nwx);
            }
            else
            {
                mm.members.Insert(0, loyalty);
                RavenSession.Store(mm);
            }

            #endregion

            RavenSession.SaveChanges();
            RavenSession.Dispose();
            return gx;
        }

        public SupplierRewards AddRewards(Reward reward)
        {

            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<SupplierRewards>("Rewards/" + reward.SupplierId);
            var lpts = new List<Reward>();
            if (gx != null)
            {
                gx.rewards.Add(reward);
                RavenSession.Store(gx);
               
            }
            else
            {
                DocumentStore.Conventions.RegisterIdConvention<SupplierRewards>((dbname, commands, post) => "Rewards/" + reward.SupplierId);

                var nwx = RavenSession.Load<SupplierRewards>("Rewards/" + reward.SupplierId);
                if (nwx == null) nwx = new SupplierRewards();

                lpts.Insert(0, reward);
                nwx.rewards = lpts;
                RavenSession.Store(nwx);
                gx = nwx;
            }
            RavenSession.SaveChanges();
            RavenSession.Dispose();
            return gx;
        }

        public SupplierRewards EditRewards(Reward reward)
        {

            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<SupplierRewards>("Rewards/" + reward.SupplierId);
            var lpts = new List<Reward>();
            if (gx != null)
            {
                var rr =gx.rewards.Where(u => u.Id == reward.Id).FirstOrDefault();
                gx.rewards.Remove(rr);
                gx.rewards.Add(reward);
                RavenSession.Store(gx);
                RavenSession.SaveChanges();
             
            }
            RavenSession.Dispose();
            return gx;
        }

        public SupplierRewards GetAllRewards(string SupplierId)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<SupplierRewards>("Rewards/" + SupplierId);
            RavenSession.Dispose();
            return gx;
        }

        public IList<Reward> GetRewardsBySchemeId(string SupplierId, long schemeId)
        {

            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<SupplierRewards>("Rewards/" + SupplierId);
            var px = new List<Reward>();
            if(gx != null)
            {
                 px = gx.rewards.Where(u => u.SchemeId == schemeId).ToList();
            }

            RavenSession.Dispose();
            return px;
        }

        public Reward GetRewardsById(string SupplierId, long Id)
        {

            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Load<SupplierRewards>("Rewards/" + SupplierId);
            var px = gx.rewards.Where(u => u.Id == Id).FirstOrDefault();
            return px;
        }

        #endregion
        
    }
}
