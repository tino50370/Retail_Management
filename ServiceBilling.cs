using RetailKing.Models;
using RetailKing.RavendbClasses;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Web;

namespace RetailKing
{
    public class ServiceBiling
    {
        RetailKingEntities db = new RetailKingEntities();
        XmlTransactionLogs trnl = new XmlTransactionLogs();
        public string Bill(long ServiceId,string SubscriberId, decimal ProviderCharge,string BranchId)
        {
            string response = "";
            var px = db.YomoneyServices.Find(ServiceId);
            if(px != null)
            {
                if(px.ServiceAudience.Trim() == "Suppliers")
                {
                    var x = db.Suppliers.Where(u => u.AccountCode.Trim() == SubscriberId.Trim()).FirstOrDefault();
                    if(x.Balance >= px.ServicePrice)
                    {
                        x.Balance -= px.ServicePrice;
                        db.Entry(x).State = EntityState.Modified;
                        db.SaveChanges();
                        #region stats
                        Stats st = new Stats();
                        st.ServiceProvider = px.ServiceProvider.Trim();
                        st.SupplierId = SubscriberId;
                        st.BranchId = BranchId;
                        st.Date = DateTime.Now.Date;
                        st.ServiceName = px.Name.Trim();
                        st.Successful = 1;
                        st.Failed = 0;
                        st.ServiceId = px.Id;
                        trnl.AddStats(st);
                        #endregion
                        return x.Balance.ToString();
                    }
                    else
                    {
                        return "11102";
                    }

                }
                else
                {
                    var c = db.Customers.Where(u => u.AccountCode == SubscriberId).FirstOrDefault();
                    if(c != null)
                    {
                       if( c.Balance >= px.ServicePrice)
                       {
                           c.Balance -= px.ServicePrice;
                           db.Entry(c).State = EntityState.Modified;
                           db.SaveChanges();
                           #region stats
                           Stats st = new Stats();
                           st.ServiceProvider = px.ServiceProvider;
                           st.SupplierId = SubscriberId;
                           st.BranchId = BranchId;
                           st.Date = DateTime.Now.Date;
                           st.ServiceName = px.Name.Trim();
                           st.Successful = 1;
                           st.Failed = 0;
                           st.ServiceId = px.Id;
                           trnl.AddStats(st);
                           #endregion
                           return c.Balance.ToString();
                       }
                       else
                       {
                           return "11102";
                       }
                    }
                }
            }
            return response;
        }

        public string ReverseBill(long ServiceId, string SubscriberId, decimal ProviderCharge, string BranchId)
        {
            string response = "";
            var px = db.YomoneyServices.Find(ServiceId);
            if (px != null)
            {
                if (px.ServiceAudience.Trim() == "Suppliers")
                {
                    var x = db.Suppliers.Where(u => u.AccountCode == SubscriberId).FirstOrDefault();
                   
                        x.Balance += px.ServicePrice;
                        db.Entry(x).State = EntityState.Modified;
                        db.SaveChanges();
                        #region stats
                        Stats st = new Stats();
                        st.ServiceProvider = px.ServiceProvider;
                        st.SupplierId = SubscriberId;
                        st.BranchId = BranchId;
                        st.Date = DateTime.Now.Date;
                        st.ServiceName = px.Name.Trim();
                        st.Successful = -1;
                        st.Failed = 0;
                        st.ServiceId = px.Id;
                        trnl.AddStats(st);
                        #endregion
                        return x.Balance.ToString();
                    

                }
                else
                {
                    var c = db.Customers.Where(u => u.AccountCode == SubscriberId).FirstOrDefault();
                    if (c != null)
                    {
                        
                            c.Balance += px.ServicePrice;
                            db.Entry(c).State = EntityState.Modified;
                            db.SaveChanges();
                            #region stats
                            Stats st = new Stats();
                            st.ServiceProvider = px.ServiceProvider;
                            st.SupplierId = SubscriberId;
                            st.BranchId = BranchId;
                            st.Date = DateTime.Now.Date;
                            st.ServiceName = px.Name.Trim();
                            st.Successful = -1;
                            st.Failed = 0;
                            st.ServiceId = px.Id;
                            trnl.AddStats(st);
                            #endregion
                            return c.Balance.ToString();
                        
                    }
                }
            }
            return response;
        }
  
    }
}