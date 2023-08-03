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
using System.Net.Mail;


namespace RetailKing.RavendbClasses
{
    public class XmlCustomers : RavenController
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        #region Profile
        public void CreateCustomerProfile(Customer customer)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var lpts = new List<Loyalty>();

            var gx = RavenSession.Load<CustomerProfile>("Profile/" + customer.AccountCode);
            if (gx == null)
            {
                DocumentStore.Conventions.RegisterIdConvention<CustomerProfile>((dbname, commands, post) => "Profile/" + customer.AccountCode);

                var nwx = RavenSession.Load<CustomerProfile>("Profile/" + customer.AccountCode);
                if (nwx == null) nwx = new CustomerProfile();
                nwx.Id = "Profile/" + customer.AccountCode;
                //nwx.DOB = DateTime.Parse(customer.Dated);
                nwx.MobileNumber = customer.Phone1;
                nwx.Name = customer.CustomerName;
                
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

        public CustomerProfile GetCustomerProfileByConnectionId(string Id)
        {
           
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var gx = RavenSession.Query<CustomerProfile>().Where(u => u.ConnectionId == Id ).FirstOrDefault();
            return gx;
        }

        public void AddCustomerMessage(Message points)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();

            /// add member to supplier/service provider
            #region membership
            var msgs = new List<Message>();

            var mm = RavenSession.Load<CustomerMessages>("Messages/" + points.ReceiverId);
            if (mm != null)
            {
                mm.messages.Add(points);
                RavenSession.Store(mm);
            }
            else
            {
                DocumentStore.Conventions.RegisterIdConvention<CustomerMessages>((dbname, commands, post) => "Messages/" + points.ReceiverId);
                var nwx = RavenSession.Load<CustomerMessages>("Messages/" + points.ReceiverId);
                if (nwx == null) nwx = new CustomerMessages();
                msgs.Add(points);
                nwx.messages = msgs;
                RavenSession.Store(mm);

            }
            #endregion

            RavenSession.SaveChanges();
            RavenSession.Dispose();

        }

        public Loyalty editCustomer(Loyalty points)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();

            /// add member to supplier/service provider
            #region membership
            var memba = new Loyalty();

            var mm = RavenSession.Load<SupplierMembers>("LoyalMembers/" + points.SupplierId);
            if (mm != null)
            {

                memba = mm.members.Where(u => u.Id == points.Id).FirstOrDefault();
                memba.Points = memba.Points + points.Points;
            }

            #endregion

            #region Customer
            var pnts = new Loyalty();

            // var cus = RavenSession.Load<CustomerProfile>("Profile/" + points.PhoneNumber);
            var cus = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == points.PhoneNumber).FirstOrDefault();
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
                //nwx.Id  = points.Id;
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

        #region supplier
        public List<CustomerSupplier> GetCustomerSuppliers(string CustomerMobile)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();

            #region Customer
            var pnts = new List<CustomerSupplier>();
            var cus = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == CustomerMobile).FirstOrDefault();
            if (cus != null)
            {
                pnts = cus.suppliers.ToList();    
            }
            RavenSession.Dispose();
            #endregion

            return pnts;
        }

        public List<CustomerService> GetAllCustomerSupplierServices(string CustomerMobile, string SupplierId)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();

            #region Customer
            var pnts = new List<CustomerService>();
            var cus = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == CustomerMobile).FirstOrDefault();
            if (cus != null)
            {
              var  sups = cus.suppliers.Where(u=> u.Id == SupplierId ).FirstOrDefault();
                if(sups != null)
                   pnts = sups.services.ToList();
            }
            RavenSession.Dispose();
            #endregion

            return pnts;
        }

        public List<CustomerService> GetAllCustomerServices(string CustomerMobile)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();

            #region Customer
            var pnts = new List<CustomerService>();
            var cus = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == CustomerMobile).FirstOrDefault();
            if (cus != null)
            {
                var sups = cus.suppliers.ToList();
                if (sups != null)
                {
                    foreach(var it in sups)
                    {
                        pnts.AddRange(it.services);
                    }
                }
                    
            }
            RavenSession.Dispose();
            #endregion

            return pnts;
        }

        #endregion

        #region Loyalty
        public CustomerProfile GetCustomerProfile(string Id)
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
            string proid = "Profile/" + Id;
            var gx = RavenSession.Query<CustomerProfile>().Where(u => u.Id == proid || u.MobileNumber == Id ).FirstOrDefault();
            return gx;
        }

        public IList<Loyalty> GetAllCustomerLoyalty(string Id)
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
            var gx = RavenSession.Query<CustomerProfile>().Where(u => u.Id == Id || u.MobileNumber == Id).FirstOrDefault();
            return gx.LoyaltySchemes;
        }

        public IList<Loyalty> GetCustomerLoyaltyBySupplier(string Id, string SupplierId)
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
            var gx = RavenSession.Query<CustomerProfile>().Where(u => u.Id == Id || u.MobileNumber == Id).FirstOrDefault();
            var px = gx.LoyaltySchemes.Where(u => u.SupplierId == SupplierId).ToList();
            return px;
        }

        public CustomerProfile CreateCustomerLoyalty(Loyalty loyalty)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var lpts = new List<Loyalty>();

            var gx = RavenSession.Load<CustomerProfile>("Profile/" + loyalty.Id );
            if (gx == null)
            {
                DocumentStore.Conventions.RegisterIdConvention<CustomerProfile>((dbname, commands, post) => "Profile/" + loyalty.Id);

                var nwx = RavenSession.Load<CustomerProfile>("Profile/" + loyalty.Id);
                if (nwx == null) nwx = new CustomerProfile();

                lpts.Insert(0, loyalty);
                nwx.LoyaltySchemes  = lpts;
                RavenSession.Store(nwx);  
            }
            else
            {
                if (gx.LoyaltySchemes != null)
                {
                    var yy = gx.LoyaltySchemes.Where(u => u.SupplierId == loyalty.SupplierId && u.SchemeId == loyalty.SchemeId).ToList();
                    if (yy.Count == 0)
                    {
                        gx.LoyaltySchemes.Insert(0, loyalty);
                        RavenSession.Store(gx);
                    }
                    else
                    {
                        return gx;
                    }
                }
                else
                {
                    lpts.Insert(0, loyalty);
                    gx.LoyaltySchemes = lpts;
                    RavenSession.Store(gx);  
                }
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

           // var cus = RavenSession.Load<CustomerProfile>("Profile/" + points.PhoneNumber);
            var cus = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == points.PhoneNumber).FirstOrDefault();
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
                //nwx.Id  = points.Id;
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

        public Loyalty CustomerRedeemRewards(string SupplierId, string Id, int points)
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
                RavenSession.Store(lm);

                #region customer
                //var cx = RavenSession.Load<CustomerProfile>("Profile/" + lm.PhoneNumber);
                var cx = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == lm.PhoneNumber).FirstOrDefault();
                var pp = cx.LoyaltySchemes.Where(u => u.SupplierId == SupplierId).FirstOrDefault();
                pp.Points = pp.Points - points;
                RavenSession.Store(cx);
                #endregion

                RavenSession.SaveChanges();
                RavenSession.Dispose();
            }
            return lm;
        }

        #endregion

        #region Notifications


        #endregion

        #region Jobs
        public void CreateJobProfile(JobProfile customer)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var lpts = new List<Loyalty>();

            var gx = RavenSession.Load<JobProfile>("JobProfile/" + customer.CustomerId);
            if (gx == null)
            {
                DocumentStore.Conventions.RegisterIdConvention<JobProfile>((dbname, commands, post) => "Profile/" + customer.CustomerId);

                var nwx = RavenSession.Load<JobProfile>("JobProfile/" + customer.CustomerId);
                if (nwx == null) nwx = new JobProfile();
               
                RavenSession.Store(customer);
                RavenSession.SaveChanges();
                RavenSession.Dispose();

            }

        }

        public void CreateJobPost(JobPost job)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            long identity = DocumentStore.DatabaseCommands.NextIdentityFor("JobPosts");

            DocumentStore.Conventions.RegisterIdConvention<JobRequests>((dbname, commands, post) => "JobPosts/" + identity);

            var nwx = RavenSession.Load<JobRequests>("JobPosts/" + identity);
            if (nwx == null) nwx = new JobRequests();
            job.Id = "JobPosts/" + identity;
            nwx.jobs.Add(job);
            RavenSession.Store(nwx);
            RavenSession.SaveChanges();
            RavenSession.Dispose();
            
        }

        public void CreateJobBid(JobBid Bid)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var lpts = new List<Loyalty>();
            long cnt = 0;
            var gx = RavenSession.Load<JobPost>("JobPost/" + Bid.JobPostId);
           
                cnt = gx.Jobbids.Count();
                cnt += 1;
                Bid.Id = cnt;
                gx.Jobbids.Add(Bid);
                RavenSession.Store(gx);
                RavenSession.SaveChanges();
                RavenSession.Dispose();

        }

        public List<JobPost> GetJobPostByCategory(string Category)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var wx = RavenSession.Query<JobPost>().Where(u => u.Category  == Category).ToList();
            RavenSession.Dispose();
            return wx;

        }

        public JobProfile GetJobProfileById(string CustomerId)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var wx = RavenSession.Query<JobProfile>().Where(u => u.CustomerId == CustomerId || u.MobileNumber == CustomerId).FirstOrDefault();
            RavenSession.Dispose();
            return wx;
        }

        public List<JobPost> GetJobOppotunities(string CustomerId)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            List<JobPost> wx = new List<JobPost>();
            var x = GetJobProfileById(CustomerId);
            DateTime clos = DateTime.MinValue;
            if (x != null)
            {
                var px = RavenSession.Query<JobPost>().Where(u => u.DateClosed == clos && u.Category == x.JobCategory).ToList();
                var e = x.OtherSkills.Select(u => u.Name).ToArray();
                wx = (from p in px
                     where e.Any(val => p.Tags.Contains(val))
                     select p).Take(30).ToList();

                RavenSession.Dispose();
               
            }
            else
            {
                wx = RavenSession.Query<JobPost>().Where(u => u.DateClosed == clos).OrderBy(u => u.DateCreated).Take(30).ToList();
                RavenSession.Dispose();
            }
            return wx;

        }
        #endregion

    }
}
