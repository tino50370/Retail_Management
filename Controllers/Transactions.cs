using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;
using RetailKing.DataAcess;

namespace RetailKing
{
    public class Transaction
    {
        // this class deals with coresbonding accounts for a transaction
        NHibernateSessionManager ns = new NHibernateSessionManager();
        public void Process(string tran, decimal amount, string company , string DebtorOrCreditor)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            var trn = np.GetTransactionsByName(tran).FirstOrDefault();
           var co = np.GetCompanies(company);
            if (trn != null)
            {
                #region debits
                if (trn.Debit1 != null && trn.Debit1 != "")
                {
                    if (trn.Debit1.Trim() == "DEBTORS" || trn.Debit1.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance + amount;
                        np.UpdateCustomers(px);
                    }
                    else if (trn.Debit1.Trim() == "CREDITORS")
                    {
                        var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateSuppliers(px);
                    }
                    else
                    {
                        var px = np.GetAccountsByName(trn.Debit1);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId ==co.ID)
                                   select p).FirstOrDefault();
                        if (acc.Balance == null) acc.Balance = 0;
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                           
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }
                }

                if (trn.Debit2 != null && trn.Debit2 != "")
                {
                    if (trn.Debit2.Trim() == "DEBTORS" || trn.Debit2.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance + amount;
                        np.UpdateCustomers(px);
                    }
                    else if (trn.Debit2.Trim() == "CREDITORS")
                    {
                        var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateSuppliers(px);
                    }
                    else
                    {
                        var px = np.GetAccountsByName(trn.Debit2);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId ==co.ID)
                                   select p).FirstOrDefault();
                        if (acc.Balance == null) acc.Balance = 0;
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }
                }

                if (trn.Debit3 != null && trn.Debit3 != "")
                {
                    if (trn.Debit3.Trim() == "DEBTORS" || trn.Debit3.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance + amount;
                        np.UpdateCustomers(px);
                    }
                    else if (trn.Debit3.Trim() == "CREDITORS")
                    {
                        
                        var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateSuppliers(px);
                    }
                    else
                    {
                        var px = np.GetAccountsByName(trn.Debit3);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId ==co.ID)
                                   select p).FirstOrDefault();
                        if (acc.Balance == null) acc.Balance = 0;
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }
                }

                if (trn.Debit4 != null && trn.Debit4 != "")
                {
                    if (trn.Debit4.Trim() == "DEBTORS" || trn.Debit4.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance + amount;
                        np.UpdateCustomers(px);
                    }
                    else if (trn.Debit4.Trim() == "CREDITORS")
                    {
                        var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateSuppliers(px);
                    }
                    else
                    {
                        var px = np.GetAccountsByName(trn.Debit4);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId ==co.ID)
                                   select p).FirstOrDefault();
                        if (acc.Balance == null) acc.Balance = 0;
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }
                }

                if (trn.Debit5 != null && trn.Debit5 != "")
                {
                    if (trn.Debit5.Trim() == "DEBTORS" || trn.Debit5.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance + amount;
                        np.UpdateCustomers(px);
                    }
                    else if (trn.Debit5.Trim() == "CREDITORS")
                    {
                        var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateSuppliers(px);
                    }
                    else
                    {
                        var px = np.GetAccountsByName(trn.Debit5);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId ==co.ID)
                                   select p).FirstOrDefault();
                        if (acc.Balance == null) acc.Balance = 0;
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }
                }
                if (trn.Debit6 != null && trn.Debit6 != "")
                {
                    if (trn.Debit6.Trim() == "DEBTORS" || trn.Debit6.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance + amount;
                        np.UpdateCustomers(px);
                    }
                    else if (trn.Debit6.Trim() == "CREDITORS")
                    {
                        var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateSuppliers(px);
                    }
                    else
                    {
                        var px = np.GetAccountsByName(trn.Debit6);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId ==co.ID)
                                   select p).FirstOrDefault();
                        if (acc.Balance == null) acc.Balance = 0;
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }
                }
                #endregion

                #region credits
                if (trn.Credit1 != null && trn.Credit1 != "")
                {
                    if (trn.Credit1.Trim() == "DEBTORS"  || trn.Credit1.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateCustomers(px);
                    }
                    else if (trn.Credit1.Trim() == "CREDITORS")
                    {
                        var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance + amount;
                        np.UpdateSuppliers(px);
                    }
                    else
                    {
                        var px = np.GetAccountsByName(trn.Credit1);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId ==co.ID)
                                   select p).FirstOrDefault();
                        if (acc.Balance == null) acc.Balance = 0;
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }
                }

                if (trn.Credit2 != null && trn.Credit2 != "")
                {
                     if (trn.Credit2.Trim() == "DEBTORS" || trn.Credit2.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateCustomers(px);
                    }
                     else if (trn.Credit2.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                         if (px.Balance == null) px.Balance = 0;
                         px.Balance = px.Balance + amount;
                         np.UpdateSuppliers(px);
                     }
                     else
                     {
                         var px = np.GetAccountsByName(trn.Credit2);
                         var acc = (from p in px
                                    .Where(u => u.CompanyId ==co.ID)
                                    select p).FirstOrDefault();
                         if (acc.Balance == null) acc.Balance = 0;
                         if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                         {
                             acc.Balance = acc.Balance - amount;
                         }
                         else
                         {
                             acc.Balance = acc.Balance + amount;
                         }
                         np.UpdateAccounts(acc);
                     }
                }

                if (trn.Credit3 != null && trn.Credit3 != "")
                {
                     if (trn.Credit3.Trim() == "DEBTORS" || trn.Credit3.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateCustomers(px);
                    }
                     else if (trn.Credit3.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                         if (px.Balance == null) px.Balance = 0;
                         px.Balance = px.Balance + amount;
                         np.UpdateSuppliers(px);
                     }
                     else
                     {
                         var px = np.GetAccountsByName(trn.Credit3);
                         var acc = (from p in px
                                    .Where(u => u.CompanyId ==co.ID)
                                    select p).FirstOrDefault();
                         if (acc.Balance == null) acc.Balance = 0;
                         if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                         {
                             acc.Balance = acc.Balance - amount;
                         }
                         else
                         {
                             acc.Balance = acc.Balance + amount;
                         }
                         np.UpdateAccounts(acc);
                     }
                }

                if (trn.Credit4 != null && trn.Credit4 != "")
                {
                     if (trn.Credit4.Trim() == "DEBTORS" || trn.Credit4.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateCustomers(px);
                    }
                     else if (trn.Credit4.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                         if (px.Balance == null) px.Balance = 0;
                         px.Balance = px.Balance + amount;
                         np.UpdateSuppliers(px);
                     }
                     else
                     {
                         var px = np.GetAccountsByName(trn.Credit4);
                         var acc = (from p in px
                                    .Where(u => u.CompanyId ==co.ID)
                                    select p).FirstOrDefault();
                         if (acc.Balance == null) acc.Balance = 0;
                         if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                         {
                             acc.Balance = acc.Balance - amount;
                         }
                         else
                         {
                             acc.Balance = acc.Balance + amount;
                         }
                         np.UpdateAccounts(acc);
                     }
                }

                if (trn.Credit5 != null && trn.Credit5 != "")
                {
                     if (trn.Credit5.Trim() == "DEBTORS" || trn.Credit5.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateCustomers(px);
                    }
                     else if (trn.Credit5.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                         if (px.Balance == null) px.Balance = 0;
                         px.Balance = px.Balance + amount;
                         np.UpdateSuppliers(px);
                     }
                     else
                     {
                         var px = np.GetAccountsByName(trn.Credit5);
                         var acc = (from p in px
                                    .Where(u => u.CompanyId ==co.ID)
                                    select p).FirstOrDefault();
                         if (acc.Balance == null) acc.Balance = 0;
                         if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                         {
                             acc.Balance = acc.Balance - amount;
                         }
                         else
                         {
                             acc.Balance = acc.Balance + amount;
                         }
                         np.UpdateAccounts(acc);
                     }
                }
                if (trn.Credit6 != null && trn.Credit6 != "")
                {
                     if (trn.Credit6.Trim() == "DEBTORS" || trn.Credit6.Trim() == "CUSTOMER")
                    {
                        var px = np.GetCustomersByName(DebtorOrCreditor).FirstOrDefault();
                        if (px.Balance == null) px.Balance = 0;
                        px.Balance = px.Balance - amount;
                        np.UpdateCustomers(px);
                    }
                     else if (trn.Credit6.Trim() == "CREDITORS")
                     {
                         var px = np.GetSuppliersByName(DebtorOrCreditor).FirstOrDefault();
                         if (px.Balance == null) px.Balance = 0;
                         px.Balance = px.Balance + amount;
                         np.UpdateSuppliers(px);
                     }
                     else
                     {
                         var px = np.GetAccountsByName(trn.Credit6);
                         var acc = (from p in px
                                    .Where(u => u.CompanyId ==co.ID)
                                    select p).FirstOrDefault();
                         if (acc.Balance == null) acc.Balance = 0;
                         if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                         {
                             acc.Balance = acc.Balance - amount;
                         }
                         else
                         {
                             acc.Balance = acc.Balance + amount;

                         }
                         np.UpdateAccounts(acc);
                     }
                }
                #endregion
            }
        }

        public void ProcessSale(string tran, decimal amount, string company, string Category, string Subcategory, string customer)
        {
            NHibernateDataProvider np = new NHibernateDataProvider();
            string PaymentType = "";
            var pacc = new Account();
            var co = np.GetCompanies(company);
            var accnt = np.GetAccountsByName(tran.ToUpper()).FirstOrDefault();
            if (accnt != null)
            {
                if(accnt.AccountCode.Length > 4)
                {
                    PaymentType = tran.ToUpper();
                    pacc = accnt;
                    accnt = np.GetAccountsByCode(accnt.AccountCode.Substring(0, 4)).FirstOrDefault();
                    tran = accnt.AccountName;
                }
                var trn = np.GetTransactionsByName(tran.ToUpper()).FirstOrDefault();
               
                if (trn != null)
                {
                    #region debits
                    if (trn.Debit1 != null && trn.Debit1 != "")
                    {
                        var px = np.GetAccountsByName(trn.Debit1);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }

                    if (trn.Debit2 != null && trn.Debit2 != "")
                    {
                        var px = np.GetAccountsByName(trn.Debit2);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }

                    if (trn.Debit3 != null && trn.Debit3 != "")
                    {
                        var px = np.GetAccountsByName(trn.Debit3);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }

                    if (trn.Debit4 != null && trn.Debit4 != "")
                    {
                        var px = np.GetAccountsByName(trn.Debit4);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }

                    if (trn.Debit5 != null && trn.Debit5 != "")
                    {
                        var px = np.GetAccountsByName(trn.Debit5);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }
                    if (trn.Debit6 != null && trn.Debit6 != "")
                    {
                        var px = np.GetAccountsByName(trn.Debit6);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }
                    #endregion

                    #region credits
                    if (trn.Credit1 != null && trn.Credit1 != "")
                    {
                        var px = np.GetAccountsByName(trn.Credit1);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }

                    if (trn.Credit2 != null && trn.Credit2 != "")
                    {
                        var px = np.GetAccountsByName(trn.Credit2);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }

                    if (trn.Credit3 != null && trn.Credit3 != "")
                    {
                        var px = np.GetAccountsByName(trn.Credit3);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }

                    if (trn.Credit4 != null && trn.Credit4 != "")
                    {
                        var px = np.GetAccountsByName(trn.Credit4);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }

                    if (trn.Credit5 != null && trn.Credit5 != "")
                    {
                        var px = np.GetAccountsByName(trn.Credit5);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;
                        }
                        np.UpdateAccounts(acc);
                    }
                    if (trn.Credit6 != null && trn.Credit6 != "")
                    {
                        var px = np.GetAccountsByName(trn.Credit6);
                        var acc = (from p in px
                                   .Where(u => u.CompanyId == co.ID)
                                   select p).FirstOrDefault();
                        if (acc.LinkAccount == "EXPENSE" || acc.LinkAccount == "ASSET")
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance - amount;
                        }
                        else
                        {
                            if (acc.Balance == null) acc.Balance = 0;
                            acc.Balance = acc.Balance + amount;

                        }
                        np.UpdateAccounts(acc);
                    }
                    #endregion

                    #region PaymentAccount
                    /// pmx = np.GetAccountsByName(PaymentType);
                    if(!string.IsNullOrEmpty(PaymentType))
                    {
                        if (pacc.LinkAccount == "EXPENSE" || pacc.LinkAccount == "ASSET")
                        {
                            if (pacc.Balance == null) pacc.Balance = 0;
                            pacc.Balance = pacc.Balance - amount;
                        }
                        else
                        {
                            if (pacc.Balance == null) pacc.Balance = 0;
                            pacc.Balance = pacc.Balance + amount;
                        }
                        np.UpdateAccounts(pacc);
                    }
                    #endregion

                    
                    if (tran.ToUpper() == "ACCOUNT SALES")
                    {
                        var accs = np.GetCustomersByName(customer).FirstOrDefault();
                        if (accs.Balance == null) accs.Balance = 0;
                        accs.Balance = accs.Balance + amount;
                        np.UpdateCustomers(accs);
                    }

                    if (tran.ToUpper() == "ACCOUNT PAYMENT")
                    {
                        var accs = np.GetCustomersByName(customer).FirstOrDefault();
                        if (accs.Balance == null) accs.Balance = 0;
                        accs.Balance = accs.Balance - amount;
                        np.UpdateCustomers(accs);
                    }
                }
            }
            if (Category != null && Category != "")
            {
                var xx = np.GetAccountsByName(Category);
                var accs = (from p in xx
                           .Where(u => u.CompanyId == co.ID)
                            select p).FirstOrDefault();
                if (accs != null)
                {
                    if (accs.Balance == null) accs.Balance = 0;
                    accs.Balance = accs.Balance - amount;
                    np.UpdateAccounts(accs);
                }
            }

            if (Subcategory != null && Subcategory != "")
            {
                var xx = np.GetAccountsByName(Subcategory);
                var accs = (from p in xx
                           .Where(u => u.CompanyId == co.ID)
                            select p).FirstOrDefault();
                if (accs != null)
                {
                    if (accs.Balance == null) accs.Balance = 0;
                    accs.Balance = accs.Balance - amount;
                    np.UpdateAccounts(accs);
                }
            }

        }


    }
}