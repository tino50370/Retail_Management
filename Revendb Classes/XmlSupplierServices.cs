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
    public class XmlSupplierServices : RavenController
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        
        XmlCustomers cus = new XmlCustomers();

        #region Services
        public SupplierProfile CreateSupplierService(SupplierService  service)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var lpts = new List<SupplierService>();

            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + service.SupplierId);
            if (gx == null)
            {
                DocumentStore.Conventions.RegisterIdConvention<SupplierProfile>((dbname, commands, post) => "SupplierProfile/" + service.SupplierId);

                var nwx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + service.SupplierId);
                if (nwx == null) nwx = new SupplierProfile();
                nwx.Id = "SupplierProfile/"+ service.SupplierId;
                nwx.ServiceCount = 1;
                service.Id = 1;
                lpts.Insert(0, service);
                nwx.Services = lpts;
                RavenSession.Store(nwx);
            }
            else
            {
                if (gx.Services  != null)
                {
                    var yy = gx.Services.Where(u => u.Name.ToLower().Trim() == service.Name.ToLower().Trim()).ToList();
                    if (yy.Count == 0)
                    {
                        service.Id = gx.Services.Count() + 1;
                        gx.Services.Insert(0, service);
                        RavenSession.Store(gx);
                    }
                    else
                    {
                        return gx;
                    }
                }
                else
                {
                    service.Id = 1;

                    lpts.Insert(0, service);
                    gx.Services = lpts;
                    RavenSession.Store(gx);
                }
            }

           
            RavenSession.SaveChanges();
            RavenSession.Dispose();
            return gx;
        }

        public List<ServiceAction> CreateServiceAction(ServiceAction service)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var lpts = new List<ServiceAction>();

            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + service.SupplierId);
            if (gx != null)
            {
                var sv = gx.Services.Where(u => u.Id == service.ServiceId ).FirstOrDefault();

                if (sv.ServiceActions != null)
                {
                    service.Id = sv.ServiceActions.Count + 1;
                    sv.ServiceActions.Insert(0, service);
                    lpts = sv.ServiceActions;
                    RavenSession.Store(gx);
                }
                else
                {
                   // sv = new Models.SupplierService();
                    service.Id = 1;
                    lpts.Insert(0, service);
                    sv.ServiceActions = lpts;
                    RavenSession.Store(gx);
                }
            }

            RavenSession.SaveChanges();
            RavenSession.Dispose();
            return lpts;
        }

        public List<SupplierService> GetSupplierServices(string Id)
        {   
            var lpts = new List<SupplierService>();
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + Id);
            if(gx!= null)
            {
                lpts = gx.Services.ToList();
            }
            RavenSession.Dispose();
            return lpts ; 
        }

        public SupplierService GetSupplierServicesById(string ServiceId,long Id)
        {

            var lpts = new SupplierService();
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + ServiceId);
            if (gx != null)
            {
                lpts = gx.Services.Where(u => u.Id == Id).FirstOrDefault();
            }
            RavenSession.Dispose();
            return lpts;
        }

        public SupplierService GetSupplierServicesByName(string ServiceId, string  ServiceName)
        {

            var lpts = new SupplierService();
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + ServiceId);
            if (gx != null)
            {
                lpts = gx.Services.Where(u => u.Name == ServiceName).FirstOrDefault();
            }
            RavenSession.Dispose();
            return lpts;
        }

        public List<ServiceAction> GetServicesActions(string Id, long ServiceId)
        {
            var svList = new  List<ServiceAction>();
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            string proid = "SupplierProfile/" + Id;
            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + Id);
            var sv = gx.Services.Where(u=> u.Id == ServiceId).FirstOrDefault();
            RavenSession.Dispose();
            if(sv.ServiceActions != null)
            {
                svList = sv.ServiceActions.ToList();
            }
            return svList;
        }

        public ServiceAction GetServicesActionsById(string SupId, long ServiceId, long Id)
        {
            ServiceAction sa = new ServiceAction();
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + SupId);
            var sv = gx.Services.Where(u => u.Id == ServiceId).FirstOrDefault();
            if(sv != null)
            {
                sa = sv.ServiceActions.Where(u => u.Id == ServiceId).FirstOrDefault();
            }
            RavenSession.Dispose();
            return sa;
        }

        public ServiceAction GetServicesActionsByYomoneyId(string SupId, long YomoneyId, long Trantype)
        {
            ServiceAction sa = new ServiceAction();
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + SupId.Trim());
            var sv = gx.Services.Where(u => u.YomoneyServiceId  == YomoneyId).FirstOrDefault();
            if (sv != null)
            {
                sa = sv.ServiceActions.Where(u => u.TransactionType == Trantype).FirstOrDefault();
            }
            RavenSession.Dispose();
            return sa;
        }

        public List<ServiceAction> GetServicesActionsByTrantype(string SupId, long Trantype)
        {
            List<ServiceAction> sa = new List<ServiceAction>();
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + SupId.Trim());
            var sv = gx.Services;
            if (sv != null)
            {
                foreach (var itm in sv)
                {
                    if (itm.ServiceActions != null)
                    {
                        var lst = itm.ServiceActions.Where(u => u.TransactionType == Trantype).ToList();
                        sa.AddRange(lst);
                    }
                }
            }
            RavenSession.Dispose();
            return sa;
        }

        public List<ServiceAction> GetServicesActionsByServiceActionId(string SupId, long ServiceActionId)
        {
            List<ServiceAction> sa = new List<ServiceAction>();
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + SupId.Trim());
            var sv = gx.Services;
            if (sv != null)
            {
                foreach (var itm in sv)
                {
                    if (itm.ServiceActions != null)
                    {
                        var lst = itm.ServiceActions.Where(u => u.ServiceActionId == ServiceActionId).ToList();
                        sa.AddRange(lst);
                    }
                }
            }
            RavenSession.Dispose();
            return sa;
        }

        public void UpdateServicesActions(ServiceAction acxn)
        {
            ServiceAction sa = new ServiceAction();
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + acxn.SupplierId);
            var sv = gx.Services.Where(u => u.Id == acxn.ServiceId).FirstOrDefault();
            if (sv != null)
            {
                sa = sv.ServiceActions.Where(u => u.Id == acxn.Id).FirstOrDefault();
                sa = acxn;
                RavenSession.Store(gx);
                RavenSession.SaveChanges();
                
            }
            RavenSession.Dispose();
        }

        public void DeleteServicesActions(ServiceAction acxn)
        {
            ServiceAction sa = new ServiceAction();
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            var gx = RavenSession.Load<SupplierProfile>("SupplierProfile/" + acxn.SupplierId);
            var sv = gx.Services.Where(u => u.Id == acxn.ServiceId).FirstOrDefault();
            if (sv != null)
            {
                sa = sv.ServiceActions.Where(u => u.Id == acxn.Id).FirstOrDefault();
                sv.ServiceActions.Remove(acxn);
                RavenSession.Store(gx);
                RavenSession.SaveChanges();

            }
            RavenSession.Dispose();
        }
        #endregion

        #region service Customers
        
        public SupplierMembers GetAllServiceCustomers(string SupplierId, string ServiceName)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            ServiceName = ServiceName.Replace(" ", "");
            var gx = RavenSession.Load<SupplierMembers>(ServiceName + "/" + SupplierId);
            RavenSession.Dispose();
            return gx;
        }

        public Loyalty GetServiceCustomers(string SupplierId, string Id, string ServiceName)
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
            ServiceName = ServiceName.Replace(" ", "");
            var gx = RavenSession.Load<SupplierMembers>(ServiceName + "/" + SupplierId);
            var lm = new Loyalty();
            if (gx != null)
            {
                lm = gx.members.Where(u => u.Id == Id || u.CardNumber == Id || u.PhoneNumber == Id).FirstOrDefault();
            }
            RavenSession.Dispose();
            return lm;
        }

        public CustomerService AddServiceBalance(string SupplierId, string Id, decimal Amount, string ServiceName, string TransactionCode,long ActionId,string ReceiverMobile)
        {
            var lm = new CustomerService();
            #region format Account
            if (Id.StartsWith("8") && Id.Length == 12)
            {
                Id = Id.Substring(0, 1) + "-" + Id.Substring(1, 4) + "-" + Id.Substring(5, 7);
                lm.CustomerCardNumber = Id;
            }
            else if (Id != null && Id.Length == 10)
            {
                Id = "263" + Id.Substring(1, Id.Length - 1);
                lm.CustomerMobileNumber = Id;
            }
            else if (Id != null && Id.Length == 9)
            {
                Id = "263" + Id;
                lm.CustomerMobileNumber = Id;
            }
            else if (Id != null && Id.Length == 12)
            {
                lm.CustomerMobileNumber = Id;
            }
            #endregion

            #region format receiver
            if (ReceiverMobile != null && ReceiverMobile.Length == 10)
            {
                ReceiverMobile = "263" + ReceiverMobile.Substring(1, ReceiverMobile.Length - 1);
               
            }
            else if (ReceiverMobile != null && ReceiverMobile.Length == 9)
            {
                ReceiverMobile = "263" + ReceiverMobile;
                
            }
            else if (ReceiverMobile != null && ReceiverMobile.Length == 12)
            {
               
            }
            #endregion

            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            var jointServiceName = ServiceName.Replace(" ", "");
            var gx = RavenSession.Load<SupplierServiceMembers>(jointServiceName + "/" + SupplierId.Trim()); 
            if (gx != null)
            {
                lm = gx.members.Where(u => (u.CustomerId == Id || u.CustomerCardNumber == Id || u.CustomerMobileNumber == Id) && u.TransactionCode == TransactionCode).FirstOrDefault();
                if (lm != null)
                {
                    lm.Balance = lm.Balance + Amount;
                    RavenSession.Store(gx);

                    #region customer
                    //var cx = RavenSession.Load<CustomerProfile>("Profile/" + lm.PhoneNumber);
                    var cx = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == lm.CustomerMobileNumber).FirstOrDefault();
                    var pp = cx.suppliers.Where(u => u.Id == SupplierId).FirstOrDefault();
                    if (pp != null)
                    {
                        lm = pp.services.Where(u => u.ServiceName == ServiceName && u.TransactionCode == TransactionCode ).FirstOrDefault();
                        if (lm != null)
                        {
                            lm.Balance += Amount;
                            lm.DatelastAccess = DateTime.Now;
                            RavenSession.Store(cx);
                            lm.ResponseCode = "Success";
                        }
                        else
                        {
                            var cp = BuildCustomerService(SupplierId, Id, ServiceName, ActionId, Amount, TransactionCode, ReceiverMobile);
                            var s = cp.suppliers.Where(u => u.Id == SupplierId.Trim()).FirstOrDefault();
                            lm = s.services.Where(u => u.ServiceName == ServiceName).FirstOrDefault();
                            lm.ResponseCode = "Success";
                        }

                    }
                    else
                    {
                        lm.ResponseCode = "Error";
                        lm.ResponseDescription = "Service Account not Found ";
                    }

                    #endregion

                    RavenSession.SaveChanges();
                }
                else
                {
                   var cp = BuildCustomerService(SupplierId, Id, ServiceName, ActionId, Amount,TransactionCode, ReceiverMobile);
                   var s= cp.suppliers.Where(u => u.Id == SupplierId.Trim()).FirstOrDefault();
                   lm = s.services.Where(u => u.ServiceName == ServiceName).FirstOrDefault();
                   lm.ResponseCode = "Success";
                    //lm.ResponseDescription = "Service Account not Found ";
                }

            }
            else
            {
                
                var cp = BuildCustomerService(SupplierId, Id, ServiceName, ActionId, Amount, TransactionCode,ReceiverMobile);
                var s = cp.suppliers.Where(u => u.Id == SupplierId.Trim()).FirstOrDefault();
                lm = s.services.Where(u => u.ServiceName == ServiceName).FirstOrDefault();
                lm.ResponseCode = "Success";
            }
            RavenSession.Dispose();
            return lm;
        }

        public CustomerService RedeemServiceBalance(string SupplierId, string Id, decimal Amount, string ServiceName, string TransactionCode, long ActionId, string ReceiverMobile)
        {
            #region formate accout
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
            #endregion

            #region format receiver
            if (ReceiverMobile != null && ReceiverMobile.Length == 10)
            {
                ReceiverMobile = "263" + ReceiverMobile.Substring(1, ReceiverMobile.Length - 1);

            }
            else if (ReceiverMobile != null && ReceiverMobile.Length == 9)
            {
                ReceiverMobile = "263" + ReceiverMobile;

            }
            else if (ReceiverMobile != null && ReceiverMobile.Length == 12)
            {

            }
            #endregion

            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            var jointServiceName = ServiceName.Replace(" ", "");
            var gx = RavenSession.Load<SupplierServiceMembers>(jointServiceName + "/" + SupplierId);
            var lm = new CustomerService();
            if (gx != null)
            {
                lm = gx.members.Where(u => u.ReceiverMobile == ReceiverMobile && u.TransactionCode == TransactionCode ).FirstOrDefault();
                if (lm.Balance >= Amount)
                {
                    lm.Balance = lm.Balance - Amount;
                    lm.DatelastAccess = DateTime.Now;
                    RavenSession.Store(gx);
                }
                else
                {
                    lm.ResponseCode = "11102";
                    lm.Description = "you voucher value is too law";
                    return lm;
                }
                #region customer
                //var cx = RavenSession.Load<CustomerProfile>("Profile/" + lm.PhoneNumber);
                var cx = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == lm.CustomerMobileNumber).FirstOrDefault();
                var pp = cx.suppliers.Where(u => u.Id == SupplierId).FirstOrDefault();
                if(pp != null)
                {
                    lm =  pp.services.Where(u => u.ServiceName == ServiceName && u.TransactionCode == TransactionCode).FirstOrDefault();
                    if (lm.Balance >= Amount)
                    {
                        lm.Balance -= Amount;
                        lm.DatelastAccess = DateTime.Now;
                        lm.ResponseCode = "Claimed";
                        RavenSession.Store(cx);
                    }
                    else
                    {
                        lm.ResponseCode = "Error";
                        lm.ResponseDescription = "Service Balance too low";
                    }
                }
                else
                {
                    lm.ResponseCode = "Error";
                    lm.ResponseDescription = "Service Account not Found ";
                }
              
                #endregion

                RavenSession.SaveChanges();
            }
            RavenSession.Dispose();
            return lm;
        }

        public CustomerService VerifyServiceAccount(string SupplierId, string code, string ServiceName, string TransactionCode, string ReceiverMobile)
        {
            
            #region format receiver
            if (ReceiverMobile != null && ReceiverMobile.Length == 10)
            {
                ReceiverMobile = "263" + ReceiverMobile.Substring(1, ReceiverMobile.Length - 1);

            }
            else if (ReceiverMobile != null && ReceiverMobile.Length == 9)
            {
                ReceiverMobile = "263" + ReceiverMobile;

            }
            else if (ReceiverMobile != null && ReceiverMobile.Length == 12)
            {

            }
            #endregion

            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            ServiceName = ServiceName.Replace(" ", "");
            var gx = RavenSession.Load<SupplierServiceMembers>(ServiceName + "/" + SupplierId);
            var lm = new CustomerService();
            if (gx != null)
            {
                lm = gx.members.Where(u =>  u.ReceiverMobile == ReceiverMobile && u.TransactionCode == TransactionCode).FirstOrDefault();
                if(lm != null)
                {
                    lm.ResponseCode = code;
                    lm.DatelastAccess = DateTime.Now;
                    RavenSession.Store(gx);
                    RavenSession.SaveChanges();
                }
            }
            RavenSession.Dispose();
            return lm;
        }

        public string AuthoriseServiceAccount(string SupplierId, string code, string ServiceName, string TransactionCode, string ReceiverMobile)
        {
            #region format receiver
            if (ReceiverMobile != null && ReceiverMobile.Length == 10)
            {
                ReceiverMobile = "263" + ReceiverMobile.Substring(1, ReceiverMobile.Length - 1);

            }
            else if (ReceiverMobile != null && ReceiverMobile.Length == 9)
            {
                ReceiverMobile = "263" + ReceiverMobile;

            }
            else if (ReceiverMobile != null && ReceiverMobile.Length == 12)
            {

            }
            #endregion

            string response = "Error";
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            ServiceName = ServiceName.Replace(" ", "");
            var gx = RavenSession.Load<SupplierServiceMembers>(ServiceName + "/" + SupplierId);
            var lm = new CustomerService();
            if (gx != null)
            {
                lm = gx.members.Where(u => u.ReceiverMobile == ReceiverMobile && u.TransactionCode == TransactionCode).FirstOrDefault();
                if (lm != null)
                {
                    var nw = DateTime.Now.Subtract(lm.DatelastAccess).Minutes;
                    
                    if (lm.ResponseCode == code && nw < 5)
                    {
                        lm.ResponseCode = "";
                        //lm.DatelastAccess = DateTime.Now;
                        RavenSession.Store(gx);
                        RavenSession.SaveChanges();
                        response = "Success";
                    }
                    else
                    {
                        response = "Error";
                    }
                }
            }
            RavenSession.Dispose();
            return response;
        }

        public CustomerProfile BuildCustomerService(string SupplierId, string Id, string ServiceName,long ActionId,decimal Amount, string TransactionCode, string ReceiverMobile)
        {
            //  create customer service for an existing customer
            var lm = new CustomerService();
            var gx = new CustomerProfile();
            var customer = cus.GetCustomerProfile(Id);
            if (customer != null)// Person is a member
            {
                var serv = GetSupplierServicesByName(SupplierId, ServiceName);
                if (serv != null)
                {
                    var act = serv.ServiceActions.Where(u => u.Id == ActionId).FirstOrDefault();
                    lm.ServiceId = serv.Id;
                    lm.Id = serv.Id;
                    lm.ServiceName = serv.Name;
                    lm.SupplierName = serv.SupplierName;
                    lm.HasProduct = serv.HasProduct;
                    lm.Description = serv.Description;
                    lm.SupplierId = serv.SupplierId;
                    lm.ServiceType = serv.ServiceType;
                    lm.ServiceProvider = act.ServiceProvider;
                    lm.ProductName = act.ProductName;
                    lm.CustomerName = customer.Name;
                    lm.CustomerId = customer.Id;
                    lm.TransactionCode = TransactionCode;
                    lm.Balance = Amount;
                    lm.ReceiverMobile = ReceiverMobile;
                    lm.SuspenseBalance = 0;
                    lm.CustomerMobileNumber = customer.MobileNumber;
                    gx = CreateCustomerService(lm);
                }
            }
            return gx;
        }

        public CustomerProfile CreateCustomerService(CustomerService service)
        {
            RavenSession = DocumentStoreHolder.DocumentStore.OpenSession();
            NHibernateDataProvider np = new NHibernateDataProvider();
            var lpts = new List<CustomerService>();
            var suList = new List<CustomerSupplier>();
            var ids = 0;
            CustomerSupplier su = new CustomerSupplier();
            service.DateCreated = DateTime.Now;
            var gx = RavenSession.Query<CustomerProfile>().Where(u => u.MobileNumber == service.CustomerMobileNumber).FirstOrDefault();
            if (gx == null)
            {
                DocumentStore.Conventions.RegisterIdConvention<CustomerProfile>((dbname, commands, post) => "Profile/" + service.CustomerId);

                var nwx = RavenSession.Load<CustomerProfile>("Profile/" + service.CustomerId);
                if (nwx == null) nwx = new CustomerProfile();
                ids =  1;
                service.Id = ids;
                lpts.Insert(0, service);
                su.Name = service.SupplierName;
                su.Id = service.SupplierId;
                su.services = lpts;
                suList.Insert(0, su);
                nwx.suppliers = suList;
                RavenSession.Store(nwx);
            }
            else
            {
                if (gx.suppliers != null)
                {
                    su = gx.suppliers.Where(u => u.Id == service.SupplierId).FirstOrDefault();
                    var x = su.services.Where(u => u.ServiceId == service.ServiceId && u.TransactionCode == service.TransactionCode);
                    if (x.Count() == 0)
                    {
                        ids = su.services.Count() + 1;
                        service.Id = ids;
                        su.services.Add(service);
                        RavenSession.Store(gx);
                    }
                }else
                {
                    service.Id = 1;
                    lpts.Insert(0, service);
                    su.Name = service.SupplierName;
                    su.Id = service.SupplierId;
                    su.services = lpts;
                    suList.Insert(0, su);
                    gx.suppliers = suList;
                    RavenSession.Store(gx);
                }
            }

            /// add member to supplier/service provider
            #region membership
            var mlist = new List<CustomerService>();
            string nam =service.ServiceName.Replace(" ", "");
            var mm = RavenSession.Load<SupplierServiceMembers>(nam + "/" + service.SupplierId);
            if (mm == null)
            {
                DocumentStore.Conventions.RegisterIdConvention<SupplierServiceMembers>((dbname, commands, post) => nam + "/" + service.SupplierId);
                var nwx = RavenSession.Load<SupplierServiceMembers>(nam + "/" + service.SupplierId);
                if (nwx == null) nwx = new SupplierServiceMembers();
                service.Id = 1;
                mlist.Insert(0, service);
                nwx.Id = nam + "/" + service.SupplierId;
                nwx.members = mlist;
                RavenSession.Store(nwx);
            }
            else
            {
                ids =mm.members.Count() + 1;
                service.Id = ids;
                mm.members.Insert(0, service);
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
                var rr = gx.rewards.Where(u => u.Id == reward.Id).FirstOrDefault();
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
            if (gx != null)
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
