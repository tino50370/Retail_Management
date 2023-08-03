using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;
using RetailKing.DataAcess;

namespace RetailKing
{
    public class Monthend
    {
        NHibernateSessionManager ns = new NHibernateSessionManager();
        RetailKingEntities db = new RetailKingEntities();
        public void ChangePeriod(Customer acc)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            decimal? total = 0;
            if (acc.wallet1 == null) acc.wallet1 = 0;
            if (acc.wallet2 == null) acc.wallet2 = 0;
            if (acc.wallet3 == null) acc.wallet3 = 0;
            switch (acc.NetworkType)
            {
                case 1:
                    acc.Wallet = acc.Wallet + acc.wallet1;
                    total = acc.wallet1;
                    break;
                case 2:
                    acc.Wallet = acc.Wallet + acc.wallet1 + acc.wallet2;
                    total = acc.wallet1 + acc.wallet2;
                    break;
                case 3:
                    acc.Wallet = acc.Wallet + acc.wallet1 + acc.wallet2 + acc.wallet3;
                    total = acc.wallet1 + acc.wallet2 + acc.wallet3;
                    break;
            }
            if (acc.NetworkType > 0)
            {
                int th = int.Parse(acc.Period.ToString()) - DateTime.Now.Month;
                string month = DateTime.Now.AddMonths(th).ToString("MMM");
                Payment pp = new Payment();
                pp.Amount = total;
                pp.CustomerId = acc.ID;
                pp.Description = month + " commission";
                pp.DateCreated = DateTime.Now;
                pp.Status = "Paid";
                pp.Source = "MobiShoppa";
                pp.SourceRefrence = "P" + acc.ID + "." + acc.Period + "." + DateTime.Now.ToString("ddmmyyyy");
                pp.ReceiptNo = "P" + acc.ID + "." + acc.Period + "." + DateTime.Now.ToString("ddmmyyyy");
                db.Payments.Add(pp);
                db.SaveChanges();
            }
            acc.Period = DateTime.Now.Month;
            acc.NetworkType = 0;
            acc.Purchases = 0;
            acc.wallet1 = 0;
            acc.wallet2 = 0;
            acc.wallet3 = 0;
            np.UpdateCustomers(acc);

           
        }

   

       
    }
}