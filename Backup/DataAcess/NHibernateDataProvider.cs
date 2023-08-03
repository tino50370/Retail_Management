using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NHibernate.Cfg;
using NHibernate;
using NHibernate.Criterion;
using RetailKing.Models;

namespace RetailKing.DataAcess
{
 public class NHibernateDataProvider
 {

 #region Fields

    private ISession _session;

    #endregion

 #region Constructors

    /// <summary>
    /// Initializes a new instance of the NHibernateDataProvider class.
    /// </summary>
    public NHibernateDataProvider(ISession session)
    {
        _session = session;
    }


    #endregion

 #region  Category Methods

    public long AddBankDeposits(Category BankDeposits, string IP)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {

                Category  x = new Category();
                long newId = (long)_session.Save(BankDeposits);
                _session.Save(x);

                _session.Flush();


                _session.Save(x);
                _session.Flush();
                tx.Commit();
                return newId;
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public IList<Category> GetAllCategory()
    {
        return _session.CreateCriteria(typeof(Category)).

            List<Category>();
    }

    public long listBankDeposits(Category BankDeposits, string IP)
    {
        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                long newId = (long)_session.Save(BankDeposits);
                _session.Flush();
                return newId;
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }
    }
  
    public void DeleteCategory(Category BankDeposits)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Delete(BankDeposits);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public Category GetCategory(int Id)
    {
        return _session.Get<Category>(Id);
    }

    public IList<Category> GetSubCategory(string name)
    {
        return _session.CreateCriteria(typeof(Category))
             .Add(Expression.Like("status", name.Trim(),MatchMode.Exact))
                     .List<Category>();
    }

    public IList<Category> GetCategoryByLocation(string location)
    {
        return _session.CreateCriteria(typeof(Category))
             .Add(Expression.Like("location", location.Trim(), MatchMode.Exact))
                     .List<Category>();
    }

    public IList<Category> GetBankDepositsByExample(Category BankDepositsSample)
    {
        return _session.CreateCriteria(typeof(Category)).Add(Example.Create(BankDepositsSample)).List<Category>();
    }


    public void UpdateCategory(Category BankDeposits)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Update(BankDeposits);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public void SaveOrUpdateCategory(Category BankDeposits)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.SaveOrUpdate(BankDeposits);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }


    #endregion

 #region  Items Methods

    public Items GetItems(long Id)
    {
        return _session.Get<Items>(Id);
    }

    public Items GetItemCode(string Id, string Company)
    {
        return _session.CreateCriteria(typeof(Items))
             .Add(Expression.Like("ItemCode", Id, MatchMode.Exact))
             .Add(Expression.Like("company", Company.Trim(), MatchMode.Exact))
                     .List<Items>().FirstOrDefault();
    }

    public IList<Items> GetDepositsByExample(Items DepositsSample)
    {
        return _session.CreateCriteria(typeof(Items)).Add(Example.Create(DepositsSample)).List<Items>();
    }

    public IList<Items> GetAllItems()
    {
        return _session.CreateCriteria(typeof(Items)).List<Items>();
    }


    public IList<Items> GetItemsByCode(string Id, string Company )
    {
        return _session.CreateCriteria(typeof(Items))
            .Add(Expression.Like("ItemCode",Id, MatchMode.Start))
            .Add(Expression.Like("company", Company.Trim(), MatchMode.Exact))
                    .List<Items>();
    }

    public IList<Items> GetItemsByCategory(string Category, string Company)
    {
        return _session.CreateCriteria(typeof(Items))
            .Add(Expression.Like("category", Category.Trim(), MatchMode.Exact))
            .Add(Expression.Like("company", Company.Trim(), MatchMode.Exact))
                    .List<Items>();
    }

    public IList<Items> GetItemsByCompany(string Company)
    {
        return _session.CreateCriteria(typeof(Items))
            .Add(Expression.Like("company", Company.Trim(), MatchMode.Exact))
                    .List<Items>();
    }

    public void UpdateItems(Items Deposits)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Update(Deposits);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public void SaveOrUpdateItems(Items Deposits)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.SaveOrUpdate(Deposits);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public void DeleteItems(Items BankDeposits)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Delete(BankDeposits);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    #endregion

 #region  InvoiceLines Methods

    public void DeleteInvoiceLines(InvoiceLines InvoiceLines)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Delete(InvoiceLines);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public InvoiceLines GetInvoiceLines(long Id)
    {
        return _session.Get<InvoiceLines>(Id);
    }

    public IList<InvoiceLines> GetInvoiceLinesByExample(InvoiceLines InvoiceLinesSample)
    {
        return _session.CreateCriteria(typeof(InvoiceLines)).Add(Example.Create(InvoiceLinesSample)).List<InvoiceLines>();
    }

    public IList<InvoiceLines> GetAllInvoiceLines()
    {
        return _session.CreateCriteria(typeof(InvoiceLines)).List<InvoiceLines>();
    }


    public IList<InvoiceLines> GetInvoiceLinesById(long Id,long MenuId, long SubjectId)
    {
        return _session.CreateCriteria(typeof(InvoiceLines))
            .Add(Expression.Like("MenuId", MenuId))
            .Add(Expression.Like("SubjectId", SubjectId))
           
                    .List<InvoiceLines>();
    }


    public void UpdateInvoiceLines(InvoiceLines InvoiceLines)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Update(InvoiceLines);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public void SaveOrUpdateInvoiceLines(InvoiceLines InvoiceLines)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.SaveOrUpdate(InvoiceLines);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }


    #endregion

 #region  Invoices Methods

   public IList<Invoices> GetAllInvoices()
    {
        return _session.CreateCriteria(typeof(Invoices)).

            List<Invoices>();
    }

    public IList<Invoices> GetAllInvoicesBySylabus(long MenuId)
    {
        return _session.CreateCriteria(typeof(Invoices))
             .Add(Expression.Like("MenuId", MenuId))
             .List<Invoices>();
    }

    public long listBankDeposits(Invoices BankDeposits, string IP)
    {
        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                long newId = (long)_session.Save(BankDeposits);
                _session.Flush();
                return newId;
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }
    }

    public void DeleteBankDeposits(Invoices BankDeposits)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Delete(BankDeposits);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public Invoices GetInvoices(long Id)
    {
        return _session.Get<Invoices>(Id);
    }

    public Invoices GetInvoicesById(long Id)
    {
        return _session.Get<Invoices>(Id);
    }
    
    public void UpdateInvoices(Invoices BankDeposits)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Update(BankDeposits);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public void SaveOrUpdateInvoices(Invoices BankDeposits)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.SaveOrUpdate(BankDeposits);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }


    #endregion

 #region OrderLines Methods

    public long AddOrderLines(OrderLines Account)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                long newId = (long)_session.Save(Account);
                _session.Flush();
                tx.Commit();
                return newId;
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public void DeleteOrderLines(OrderLines Account)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Delete(Account);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public OrderLines GetOrderLines(long Account)
    {
        return _session.Get<OrderLines>(Account);
    }

    public IList<OrderLines> GetOrderLinesByExample(OrderLines AccountSample)
    {
        return _session.CreateCriteria(typeof(OrderLines)).Add(Example.Create(AccountSample)).List<OrderLines>();
    }

    public IList<OrderLines> GetAllOrderLines()
    {
        return _session.CreateCriteria(typeof(OrderLines)).

            List<OrderLines>();
    }


    public IList<OrderLines> GetOrderLinesByQuestionName(long SubCategory, long MenuId, long TopicId, long SubjectId, string ExamYear, string ExamSitting, string PaperNumber, string Description)
    {
        if (ExamYear.Trim() == "No Value" || ExamYear == null || ExamYear == "")
        {
            if (Description == "Prev")
            {
                if (SubCategory == 4)
                {
                    return _session.CreateCriteria(typeof(OrderLines))
                    .Add(Expression.Like("SubCategoryId", SubCategory))
                    .Add(Expression.Like("SylubusId", MenuId))
                    .Add(Expression.Like("TopicId", TopicId))
                    .Add(Expression.Like("SubjectId", SubjectId))
                    .AddOrder(Order.Desc("Id"))
                             .List<OrderLines>();
                }
                else
                {

                    return _session.CreateCriteria(typeof(OrderLines))
                     .Add(Expression.Like("SubCategoryId", SubCategory))
                     .Add(Expression.Like("SylubusId", MenuId))
                     .Add(Expression.Like("TopicId", TopicId))
                     .Add(Expression.Like("SubjectId", SubjectId))
                     .Add(Expression.Like("ExamYear", "No Value", MatchMode.Anywhere))
                     .AddOrder(Order.Desc("Id"))
                              .List<OrderLines>();
                }
            }
            else
            {
                if (SubCategory == 4)
                {
                    return _session.CreateCriteria(typeof(OrderLines))
                        .Add(Expression.Like("SubCategoryId", SubCategory))
                        .Add(Expression.Like("SylubusId", MenuId))
                        .Add(Expression.Like("TopicId", TopicId))
                        .Add(Expression.Like("SubjectId", SubjectId))
                        .AddOrder(Order.Asc("Id"))
                                 .List<OrderLines>();
                }
                else
                {
                    return _session.CreateCriteria(typeof(OrderLines))
                        .Add(Expression.Like("SubCategoryId", SubCategory))
                        .Add(Expression.Like("SylubusId", MenuId))
                        .Add(Expression.Like("TopicId", TopicId))
                        .Add(Expression.Like("SubjectId", SubjectId))
                        .Add(Expression.Like("ExamYear", "No Value", MatchMode.Anywhere))
                        .AddOrder(Order.Asc("Id"))
                                 .List<OrderLines>();
                }
            }
        }
        else
        {
            if (Description == "Prev")
            {
                return _session.CreateCriteria(typeof(OrderLines))
                .Add(Expression.Like("SubCategoryId", SubCategory))
                .Add(Expression.Like("SylubusId", MenuId))
                .Add(Expression.Like("SubjectId", SubjectId))
                .Add(Expression.Like("ExamYear", ExamYear, MatchMode.Anywhere))
                .Add(Expression.Like("ExamSitting", ExamSitting, MatchMode.Anywhere))
                .Add(Expression.Like("PaperNumber", PaperNumber, MatchMode.Anywhere))
                .AddOrder(Order.Desc("Id"))
                         .List<OrderLines>();
            }
            else
            {
                return _session.CreateCriteria(typeof(OrderLines))
                .Add(Expression.Like("SubCategoryId", SubCategory))
                .Add(Expression.Like("SylubusId", MenuId))
                .Add(Expression.Like("SubjectId", SubjectId))
                .Add(Expression.Like("ExamYear", ExamYear, MatchMode.Anywhere))
                .Add(Expression.Like("ExamSitting", ExamSitting, MatchMode.Anywhere))
                .Add(Expression.Like("PaperNumber", PaperNumber, MatchMode.Anywhere))
                .AddOrder(Order.Asc("Id"))
                         .List<OrderLines>();
            }
        }
    }

    public IList<OrderLines> GetAllOrderLinesWhere(long SubCategory, long SylubusId, long SubjectId)
    {
        var x = _session.CreateCriteria(typeof(OrderLines))
                .Add(Expression.Like("SubCategoryId", SubCategory))
                .Add(Expression.Like("SylubusId", SylubusId))
                .Add(Expression.Like("SubjectId", SubjectId))
                .AddOrder(Order.Asc("ExamYear"))
                .List<OrderLines>();
        return x;
    }

    public IList<OrderLines> GetAllOrderLinesPaper(string Sitting, string Year, string Paper, long SubjectId, long SylubusId, long Subcategory)
    {
        return _session.CreateCriteria(typeof(OrderLines))
                .Add(Expression.Like("ExamSitting", Sitting, MatchMode.Anywhere))
                .Add(Expression.Like("SylubusId", SylubusId))
                .Add(Expression.Like("SubjectId", SubjectId))
                .Add(Expression.Like("ExamYear", Year, MatchMode.Anywhere))
                .Add(Expression.Like("PaperNumber", Paper, MatchMode.Anywhere))
                .Add(Expression.Like("SubCategoryId", Subcategory))
                .AddOrder(Order.Asc("Id"))
                .List<OrderLines>();
    }


    public void UpdateOrderLines(OrderLines OrderLines)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Update(OrderLines);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public void SaveOrUpdateOrderLines(OrderLines Account)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.SaveOrUpdate(Account);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }


    #endregion

 #region Orders Methods

    public long AddOrders(Orders Account)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                long newId = (long)_session.Save(Account);
                _session.Flush();
                tx.Commit();
                return newId;
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public void DeleteOrders(Orders Account)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Delete(Account);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public Orders GetOrders(long Account)
    {
        return _session.Get<Orders>(Account);
    }

    public IList<Orders> GetOrdersByExample(Orders AccountSample)
    {
        return _session.CreateCriteria(typeof(Orders)).Add(Example.Create(AccountSample)).List<Orders>();
    }

    public IList<Orders> GetAllOrders()
    {
        return _session.CreateCriteria(typeof(Orders)).

            List<Orders>();
    }


    public IList<Orders> GetOrdersByQuestionName(long SubCategory, long MenuId, long TopicId, long SubjectId, string ExamYear, string ExamSitting, string PaperNumber, string Description)
    {
        if (ExamYear.Trim() == "No Value" || ExamYear == null || ExamYear == "")
        {
            if (Description == "Prev")
            {
                if (SubCategory == 4)
                {
                    return _session.CreateCriteria(typeof(Orders))
                    .Add(Expression.Like("SubCategoryId", SubCategory))
                    .Add(Expression.Like("SylubusId", MenuId))
                    .Add(Expression.Like("TopicId", TopicId))
                    .Add(Expression.Like("SubjectId", SubjectId))
                    .AddOrder(Order.Desc("Id"))
                             .List<Orders>();
                }
                else
                {

                    return _session.CreateCriteria(typeof(Orders))
                     .Add(Expression.Like("SubCategoryId", SubCategory))
                     .Add(Expression.Like("SylubusId", MenuId))
                     .Add(Expression.Like("TopicId", TopicId))
                     .Add(Expression.Like("SubjectId", SubjectId))
                     .Add(Expression.Like("ExamYear", "No Value", MatchMode.Anywhere))
                     .AddOrder(Order.Desc("Id"))
                              .List<Orders>();
                }
            }
            else
            {
                if (SubCategory == 4)
                {
                    return _session.CreateCriteria(typeof(Orders))
                        .Add(Expression.Like("SubCategoryId", SubCategory))
                        .Add(Expression.Like("SylubusId", MenuId))
                        .Add(Expression.Like("TopicId", TopicId))
                        .Add(Expression.Like("SubjectId", SubjectId))
                        .AddOrder(Order.Asc("Id"))
                                 .List<Orders>();
                }
                else
                {
                    return _session.CreateCriteria(typeof(Orders))
                        .Add(Expression.Like("SubCategoryId", SubCategory))
                        .Add(Expression.Like("SylubusId", MenuId))
                        .Add(Expression.Like("TopicId", TopicId))
                        .Add(Expression.Like("SubjectId", SubjectId))
                        .Add(Expression.Like("ExamYear", "No Value", MatchMode.Anywhere))
                        .AddOrder(Order.Asc("Id"))
                                 .List<Orders>();
                }
            }
        }
        else
        {
            if (Description == "Prev")
            {
                return _session.CreateCriteria(typeof(Orders))
                .Add(Expression.Like("SubCategoryId", SubCategory))
                .Add(Expression.Like("SylubusId", MenuId))
                .Add(Expression.Like("SubjectId", SubjectId))
                .Add(Expression.Like("ExamYear", ExamYear, MatchMode.Anywhere))
                .Add(Expression.Like("ExamSitting", ExamSitting, MatchMode.Anywhere))
                .Add(Expression.Like("PaperNumber", PaperNumber, MatchMode.Anywhere))
                .AddOrder(Order.Desc("Id"))
                         .List<Orders>();
            }
            else
            {

                return _session.CreateCriteria(typeof(Orders))
                    .Add(Expression.Like("SubCategoryId", SubCategory))
                    .Add(Expression.Like("SylubusId", MenuId))
                    .Add(Expression.Like("SubjectId", SubjectId))
                    .Add(Expression.Like("ExamYear", ExamYear, MatchMode.Anywhere))
                    .Add(Expression.Like("ExamSitting", ExamSitting, MatchMode.Anywhere))
                    .Add(Expression.Like("PaperNumber", PaperNumber, MatchMode.Anywhere))
                    .AddOrder(Order.Asc("Id"))
                             .List<Orders>();
            }
        }
    }

    public IList<Orders> GetAllOrdersWhere(long SubCategory, long SylubusId, long SubjectId)
    {
        var x = _session.CreateCriteria(typeof(Orders))
                .Add(Expression.Like("SubCategoryId", SubCategory))
                .Add(Expression.Like("SylubusId", SylubusId))
                .Add(Expression.Like("SubjectId", SubjectId))
                .AddOrder(Order.Asc("ExamYear"))
                .List<Orders>();
        return x;
    }

    public IList<Orders> GetAllOrdersPaper(string Sitting, string Year, string Paper, long SubjectId, long SylubusId, long Subcategory)
    {
        return _session.CreateCriteria(typeof(Orders))
                .Add(Expression.Like("ExamSitting", Sitting, MatchMode.Anywhere))
                .Add(Expression.Like("SylubusId", SylubusId))
                .Add(Expression.Like("SubjectId", SubjectId))
                .Add(Expression.Like("ExamYear", Year, MatchMode.Anywhere))
                .Add(Expression.Like("PaperNumber", Paper, MatchMode.Anywhere))
                .Add(Expression.Like("SubCategoryId", Subcategory))
                .AddOrder(Order.Asc("Id"))
                .List<Orders>();
    }

    public void UpdateOrders(Orders Orders)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Update(Orders);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public void SaveOrUpdateOrders(Orders Account)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.SaveOrUpdate(Account);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }
    #endregion

 #region PurchaseLines Methods

    public long AddPurchaseLines(PurchaseLines Account)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                long newId = (long)_session.Save(Account);
                _session.Flush();
                tx.Commit();
                return newId;
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public void DeletePurchaseLines(PurchaseLines Account)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Delete(Account);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public PurchaseLines GetPurchaseLines(long Account)
    {
        return _session.Get<PurchaseLines>(Account);
    }

    public IList<PurchaseLines> GetPurchaseLinesByExample(PurchaseLines AccountSample)
    {
        return _session.CreateCriteria(typeof(PurchaseLines)).Add(Example.Create(AccountSample)).List<PurchaseLines>();
    }

    public IList<PurchaseLines> GetAllPurchaseLines()
    {
        return _session.CreateCriteria(typeof(PurchaseLines)).

            List<PurchaseLines>();
    }

    public IList<PurchaseLines> GetPurchaseLinesByQuestionName(long SubCategory, long MenuId, long TopicId, long SubjectId, string ExamYear, string ExamSitting, string PaperNumber, string Description)
    {
        if (ExamYear.Trim() == "No Value" || ExamYear == null || ExamYear == "")
        {
            if (Description == "Prev")
            {
                if (SubCategory == 4)
                {
                    return _session.CreateCriteria(typeof(PurchaseLines))
                    .Add(Expression.Like("SubCategoryId", SubCategory))
                    .Add(Expression.Like("SylubusId", MenuId))
                    .Add(Expression.Like("TopicId", TopicId))
                    .Add(Expression.Like("SubjectId", SubjectId))
                    .AddOrder(Order.Desc("Id"))
                             .List<PurchaseLines>();
                }
                else if (SubCategory == 6)
                {
                    return _session.CreateCriteria(typeof(PurchaseLines))
                    .Add(Expression.Like("SubCategoryId", SubCategory))
                    .Add(Expression.Like("SylubusId", MenuId))
                    .Add(Expression.Like("SubjectId", SubjectId))
                    .AddOrder(Order.Desc("Id"))
                             .List<PurchaseLines>();
                }
                else
                {

                    return _session.CreateCriteria(typeof(PurchaseLines))
                     .Add(Expression.Like("SubCategoryId", SubCategory))
                     .Add(Expression.Like("SylubusId", MenuId))
                     .Add(Expression.Like("TopicId", TopicId))
                     .Add(Expression.Like("SubjectId", SubjectId))
                     .Add(Expression.Like("ExamYear", "No Value", MatchMode.Anywhere))
                     .AddOrder(Order.Desc("Id"))
                              .List<PurchaseLines>();
                }
            }
            else
            {
                if (SubCategory == 4)
                {
                    return _session.CreateCriteria(typeof(PurchaseLines))
                        .Add(Expression.Like("SubCategoryId", SubCategory))
                        .Add(Expression.Like("SylubusId", MenuId))
                        .Add(Expression.Like("TopicId", TopicId))
                        .Add(Expression.Like("SubjectId", SubjectId))
                        .AddOrder(Order.Asc("Id"))
                                 .List<PurchaseLines>();
                }
                else if (SubCategory == 6)
                {
                    return _session.CreateCriteria(typeof(PurchaseLines))
                        .Add(Expression.Like("SubCategoryId", SubCategory))
                        .Add(Expression.Like("SylubusId", MenuId))
                        .Add(Expression.Like("SubjectId", SubjectId))
                        .AddOrder(Order.Asc("Id"))
                                 .List<PurchaseLines>();
                }
                else
                {
                    return _session.CreateCriteria(typeof(PurchaseLines))
                        .Add(Expression.Like("SubCategoryId", SubCategory))
                        .Add(Expression.Like("SylubusId", MenuId))
                        .Add(Expression.Like("TopicId", TopicId))
                        .Add(Expression.Like("SubjectId", SubjectId))
                        .Add(Expression.Like("ExamYear", "No Value", MatchMode.Anywhere))
                        .AddOrder(Order.Asc("Id"))
                                 .List<PurchaseLines>();
                }
            }
        }
        else
        {
            if (Description == "Prev")
            {
                return _session.CreateCriteria(typeof(PurchaseLines))
                .Add(Expression.Like("SubCategoryId", SubCategory))
                .Add(Expression.Like("SylubusId", MenuId))
                .Add(Expression.Like("SubjectId", SubjectId))
                .Add(Expression.Like("ExamYear", ExamYear, MatchMode.Anywhere))
                .Add(Expression.Like("ExamSitting", ExamSitting, MatchMode.Anywhere))
                .Add(Expression.Like("PaperNumber", PaperNumber, MatchMode.Anywhere))
                .AddOrder(Order.Desc("Id"))
                         .List<PurchaseLines>();
            }
            else
            {

                return _session.CreateCriteria(typeof(PurchaseLines))
                    .Add(Expression.Like("SubCategoryId", SubCategory))
                    .Add(Expression.Like("SylubusId", MenuId))
                    .Add(Expression.Like("SubjectId", SubjectId))
                    .Add(Expression.Like("ExamYear", ExamYear, MatchMode.Anywhere))
                    .Add(Expression.Like("ExamSitting", ExamSitting, MatchMode.Anywhere))
                    .Add(Expression.Like("PaperNumber", PaperNumber, MatchMode.Anywhere))
                    .AddOrder(Order.Asc("Id"))
                             .List<PurchaseLines>();
            }
        }
    }

    public IList<PurchaseLines> GetAllPurchaseLinesWhere(long SubCategory, long SylubusId, long SubjectId)
    {
        if (SubjectId == 5)
        {

            var x = _session.CreateCriteria(typeof(PurchaseLines))
                    .Add(Expression.Like("SubCategoryId", SubCategory))
                    .Add(Expression.Like("SylubusId", SylubusId))
                    .Add(Expression.Like("SubjectId", SubjectId))
                    .AddOrder(Order.Asc("ExamSitting"))
                    .List<PurchaseLines>();
        
               return x;
        }
        else 
        {
            var x = _session.CreateCriteria(typeof(PurchaseLines))
                .Add(Expression.Like("SubCategoryId", SubCategory))
                .Add(Expression.Like("SylubusId", SylubusId))
                .Add(Expression.Like("SubjectId", SubjectId))
                .AddOrder(Order.Asc("ExamYear"))
                .List<PurchaseLines>();
                return x;
        }
    }

    public IList<PurchaseLines> GetAllPurchaseLinesPaper(string Sitting, string Year, string Paper, long SubjectId, long SylubusId, long Subcategory)
    {
        return _session.CreateCriteria(typeof(PurchaseLines))
                .Add(Expression.Like("ExamSitting", Sitting, MatchMode.Anywhere))
                .Add(Expression.Like("SylubusId", SylubusId))
                .Add(Expression.Like("SubjectId", SubjectId))
                .Add(Expression.Like("ExamYear", Year, MatchMode.Anywhere))
                .Add(Expression.Like("PaperNumber", Paper, MatchMode.Anywhere))
                .Add(Expression.Like("SubCategoryId", Subcategory))
                .AddOrder(Order.Asc("Id"))
                .List<PurchaseLines>();
    }

    public void UpdatePurchaseLines(PurchaseLines PurchaseLines)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.Update(PurchaseLines);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    public void SaveOrUpdatePurchaseLines(PurchaseLines Account)
    {

        using (ITransaction tx = _session.BeginTransaction())
        {
            try
            {
                _session.SaveOrUpdate(Account);
                _session.Flush();
                tx.Commit();
            }
            catch (NHibernate.HibernateException)
            {
                tx.Rollback();
                throw;
            }
        }

    }

    #endregion
  
 #region  Purchases Methods

        public long AddPurchases(Purchases Purchases)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(Purchases);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeletePurchases(Purchases Purchases)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(Purchases);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public Purchases GetPurchases(int Purchases)
        {
            return _session.Get<Purchases>(Purchases);
        }

        public IList<Purchases> GetPurchasesByExample(Purchases PurchasesSample)
        {
            return _session.CreateCriteria(typeof(Purchases)).Add(Example.Create(PurchasesSample)).List<Purchases>();
        }

        public IList<Purchases> GetAllPurchases()
        {
            return _session.CreateCriteria(typeof(Purchases)).List<Purchases>();
        }


  public IList<Purchases> GetPurchasesByDescription(string Description)
        {
            return _session.CreateCriteria(typeof(Purchases))
             .Add(Expression.Like("Description",Description.Trim()+"%"))
                      .List<Purchases>();
        }


        public void UpdatePurchases(Purchases Purchases)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(Purchases);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdatePurchases(Purchases Purchases)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Save(Purchases);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  ReturnLines Methods

        public long AddReturnLines(ReturnLines ReturnLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(ReturnLines);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteReturnLines(ReturnLines ReturnLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(ReturnLines);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public ReturnLines GetReturnLines(int ReturnLinesId)
        {
            return _session.Get<ReturnLines>(ReturnLinesId);
        }

        public IList<ReturnLines> GetReturnLinesByExample(ReturnLines ReturnLinesSample)
        {
            return _session.CreateCriteria(typeof(ReturnLines)).Add(Example.Create(ReturnLinesSample)).List<ReturnLines>();
        }

        public IList<ReturnLines> GetAllReturnLines(long ShiftId, long SubjectId)
        {
            return _session.CreateCriteria(typeof(ReturnLines))
               .Add(Expression.Like("ShiftId", ShiftId))
               .Add(Expression.Like("SubjectId", SubjectId))
               .AddOrder(Order.Asc("TopicId"))
               .List<ReturnLines>();
        }


        public IList<ReturnLines> GetReturnLinesByDescription(string Description)
        {
            return _session.CreateCriteria(typeof(ReturnLines))
             .Add(Expression.Like("Description", Description.Trim() + "%"))
                      .List<ReturnLines>();
        }


        public void UpdateReturnLines(ReturnLines ReturnLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(ReturnLines);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdateReturnLines(ReturnLines ReturnLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Save(ReturnLines);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


   #endregion

 #region  Returns Methods

        public long AddReturns(Returns ShiftReturns)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(ShiftReturns);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteReturns(Returns Returns)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(Returns);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public Returns GetReturns(int ReturnsId)
        {
            return _session.Get<Returns>(ReturnsId);
        }

        public IList<Returns> GetReturnsByExample(Returns ReturnsSample)
        {
            return _session.CreateCriteria(typeof(Returns)).Add(Example.Create(ReturnsSample)).List<Returns>();
        }

        public IList<Returns> GetAllReturns()
        {
            return _session.CreateCriteria(typeof(Returns))
               .List<Returns>();
        }


        public IList<Returns> GetReturnsByDescription(string Description)
        {
            return _session.CreateCriteria(typeof(Returns))
             .Add(Expression.Like("Description", Description.Trim() + "%"))
                      .List<Returns>();
        }


        public void UpdateReturns(Returns Returns)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(Returns);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdateReturns(Returns Returns)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Save(Returns);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  Sales Methods

        public long AddSales(Sales ShiftSales)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(ShiftSales);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteSales(Sales Sales)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(Sales);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public Sales GetSales(long  SalesId)
        {
            return _session.Get<Sales>(SalesId);
        }

        public IList<Sales> GetSalesByExample(Sales SalesSample)
        {
            return _session.CreateCriteria(typeof(Sales)).Add(Example.Create(SalesSample)).List<Sales>();
        }

        public IList<Sales> GetAllSales(long ShiftId, long SubjectId)
        {
            return _session.CreateCriteria(typeof(Sales))
               .Add(Expression.Like("ShiftId", ShiftId))
               .Add(Expression.Like("SubjectId", SubjectId))
               .List<Sales>();
        }


        public IList<Sales> GetSalesByDescription(string Description)
        {
            return _session.CreateCriteria(typeof(Sales))
             .Add(Expression.Like("Description", Description.Trim() + "%"))
                      .List<Sales>();
        }


        public void UpdateSales(Sales Sales)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(Sales);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdateSales(Sales Sales)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(Sales);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }
    

        #endregion

 #region  SalesLines Methods

        public long AddSalesLines(SalesLines ShiftSalesLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(ShiftSalesLines);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteSalesLines(SalesLines SalesLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(SalesLines);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public SalesLines GetSalesLines(string SalesLinesId)
        {
            return _session.Get<SalesLines>(SalesLinesId);
        }

        public IList<SalesLines> GetSalesLinesByExample(SalesLines SalesLinesSample)
        {
            return _session.CreateCriteria(typeof(SalesLines)).Add(Example.Create(SalesLinesSample)).List<SalesLines>();
        }

        public IList<SalesLines> GetAllSalesLines( long QId)
        {
            return _session.CreateCriteria(typeof(SalesLines))
               .Add(Expression.Like("QId", QId))
               .AddOrder(Order.Asc("Tries"))
               .List<SalesLines>();
        }

        public IList<SalesLines> GetAllSalesLinesByShift(string ShiftId, long QId)
        {
            return _session.CreateCriteria(typeof(SalesLines))
               .Add(Expression.Like("Id", ShiftId, MatchMode.Anywhere))
               .Add(Expression.Like("QId", QId))
                .AddOrder(Order.Asc("Tries"))
               .List<SalesLines>();
        }


        public IList<SalesLines> GetSalesLinesByDescription(string Description)
        {
            return _session.CreateCriteria(typeof(SalesLines))
             .Add(Expression.Like("Description", Description.Trim() + "%"))
                      .List<SalesLines>();
        }


        public void UpdateSalesLines(SalesLines SalesLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(SalesLines);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdateSalesLines(SalesLines SalesLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(SalesLines);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  TransferLines Methods

        public long AddTransferLines(TransferLines ShiftTransferLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(ShiftTransferLines);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteTransferLines(TransferLines TransferLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(TransferLines);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public TransferLines GetTransferLines(string TransferLinesId)
        {
            return _session.Get<TransferLines>(TransferLinesId);
        }

        public IList<TransferLines> GetTransferLinesByExample(TransferLines TransferLinesSample)
        {
            return _session.CreateCriteria(typeof(TransferLines)).Add(Example.Create(TransferLinesSample)).List<TransferLines>();
        }

        public IList<TransferLines> GetAllTransferLinesByShiftId(string ShiftId, long QId)
        {
            return _session.CreateCriteria(typeof(TransferLines))
               .Add(Expression.Like("Id", ShiftId, MatchMode.Anywhere))
               .Add(Expression.Like("QId", QId))
               .List<TransferLines>();
        }

        public IList<TransferLines> GetAllTransferLinesByShift(string ShiftId)
        {
            return _session.CreateCriteria(typeof(TransferLines))
               .Add(Expression.Like("Id", ShiftId, MatchMode.Anywhere))
               .List<TransferLines>();
        }


        public IList<TransferLines> GetTransferLinesByDescription(string Description)
        {
            return _session.CreateCriteria(typeof(TransferLines))
             .Add(Expression.Like("Description", Description.Trim() + "%"))
                      .List<TransferLines>();
        }


        public void UpdateTransferLines(TransferLines TransferLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(TransferLines);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdateTransferLines(TransferLines TransferLines)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(TransferLines);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  Transfers Methods

        public long AddTransfers(Transfers Transfers)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {

                    long newId = (long)_session.Save(Transfers);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteTransfers(Transfers Transfers)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(Transfers);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public Transfers GetTransfers(long Id)
        {
            return _session.Get<Transfers>(Id);
        }

        public IList<Transfers> GetTransfersByExample(Transfers TransfersSample)
        {
            return _session.CreateCriteria(typeof(Transfers)).Add(Example.Create(TransfersSample)).List<Transfers>();
        }

        public IList<Transfers> GetAllTransfers()
        {
            return _session.CreateCriteria(typeof(Transfers)).List<Transfers>();
        }


  public IList<Transfers> GetTransfersByDescription(string Description)
        {
            return _session.CreateCriteria(typeof(Transfers))
             .Add(Expression.Like("Description",Description.Trim()+"%"))
                      .List<Transfers>();
        }


        public void UpdateTransfers(Transfers Transfers)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(Transfers);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdateTransfers(Transfers Transfers)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(Transfers);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  Expenses Methods

        public long AddExpenses(Expenses Expenses)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(Expenses);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteExpenses(Expenses Expenses)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(Expenses);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public Expenses GetExpenses(int Expenses)
        {
            return _session.Get<Expenses>(Expenses);
        }

        public IList<Expenses> GetExpensesByExample(Expenses ExpensesSample)
        {
            return _session.CreateCriteria(typeof(Expenses)).Add(Example.Create(ExpensesSample)).List<Expenses>();
        }

        public IList<Expenses> GetAllExpenses()
        {
            return _session.CreateCriteria(typeof(Expenses)).List<Expenses>();
        }


  public IList<Expenses> GetExpensesBydate(DateTime date,string AccountNumber)
        {
            return _session.CreateCriteria(typeof(Expenses))
             .Add(Expression.Like("Date",date))
             .Add(Expression.Like("SourceAccountNumber", AccountNumber))
                      .List<Expenses>();
        }


        public void UpdateExpenses(Expenses Expenses)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(Expenses);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdateExpenses(Expenses Expenses)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(Expenses);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region Customers
        public Customers GetCustomers(long Account)
        {
            return _session.Get<Customers>(Account);
        }

        public IList<Customers> GetAllCustomers()
        {
            return _session.CreateCriteria(typeof(Customers)).List<Customers>();
        }

        public Customers GetAccountQuestion(string Question)
        {
            return _session.Get<Customers>(Question);
        }

        public void SaveOrUpdateCustomers(Customers Account)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {

                    _session.Save(Account);
                    _session.Flush();
                    tx.Commit();

                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteCompanies(Customers customers)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(customers);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }
        #endregion

 #region Companies

        public Companies GetCompanies(string Account)
        {
            return _session.CreateCriteria(typeof(Companies))
              .Add(Expression.Like("name", Account, MatchMode.Exact))
              .List<Companies>().FirstOrDefault();
           // return _session.Get<Companies>(Account);
        }

        public Companies GetCompaniesById(long Account)
        {
           
             return _session.Get<Companies>(Account);
        }

        public IList<Companies> GetAllCompanies()
        {
            return _session.CreateCriteria(typeof(Companies)).List<Companies>();
        }

       

        public void SaveOrUpdateCompanies(Companies Account)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {

                    _session.Save(Account);
                    _session.Flush();
                    tx.Commit();

                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteCompanies(Companies companies)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(companies);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }
        #endregion

 #region  Shift Methods

        public long AddShift(Shift Shift)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(Shift);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteShift(Shift Shift)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(Shift);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public Shift GetShift(string Shift)
        {
            return _session.Get<Shift>(Shift);
            //  return _session.QueryOver<Shift>()
            // .Where(u => u.ShiftNumber == Shift)

        }

        public IList<Shift> GetShiftByExample(Shift ShiftSample)
        {
            return _session.CreateCriteria(typeof(Shift)).Add(Example.Create(ShiftSample)).List<Shift>();
        }

        public IList<Shift> GetAllShift()
        {
            return _session.CreateCriteria(typeof(Shift)).
                List<Shift>();
        }


       

        public void UpdateShift(Shift Shift)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(Shift);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdateShift(Shift Shift)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(Shift);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  Menu Methods

        public long AddMenu(Menu Menu)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(Menu);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteMenu(Menu Menu)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(Menu);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public IList<Menu> GetMenu(string Menu)
        {
            return _session.CreateCriteria(typeof(Menu))
              .Add(Expression.Like("company", Menu.Trim()))
                       .List<Menu>();
        }

        public IList<Menu> GetMenuByExample(Menu MenuSample)
        {
            return _session.CreateCriteria(typeof(Menu)).Add(Example.Create(MenuSample)).List<Menu>();
        }

        public IList<Menu> GetAllMenu()
        {
            return _session.CreateCriteria(typeof(Menu)).
                List<Menu>();
        }


        public IList<Menu> GetMenuByMenuName(string MenuName)
        {
            return _session.CreateCriteria(typeof(Menu))
             .Add(Expression.Like("MenuName", MenuName.Trim() + "%"))
                      .List<Menu>();
        }


        public void UpdateMenu(Menu Menu)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(Menu);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdateMenu(Menu Menu)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(Menu);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  CreditNotes Methods

      
        public void DeleteCreditNotes (CreditNotes  CreditNotes )
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(CreditNotes );
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public CreditNotes  GetCreditNotes (long Id)
        {
            return _session.Get<CreditNotes >(Id);
        }

        public IList<CreditNotes > GetCreditNotesByExample(CreditNotes  CreditNotesSample)
        {
            return _session.CreateCriteria(typeof(CreditNotes)).Add(Example.Create(CreditNotesSample)).List<CreditNotes >();
        }

        public IList<CreditNotes > GetAllCreditNotes ()
        {
            return _session.CreateCriteria(typeof(CreditNotes )).List<CreditNotes >();
        }


        public IList<CreditNotes > GetCreditNotesById(long Id, long MenuId, long SubjectId)
        {
            return _session.CreateCriteria(typeof(CreditNotes ))
                .Add(Expression.Like("MenuId", MenuId))
                .Add(Expression.Like("SubjectId", SubjectId))

                        .List<CreditNotes >();
        }


        public void UpdateCreditNotes (CreditNotes  CreditNotes )
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(CreditNotes);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public long SaveOrUpdateCreditNotes (CreditNotes  CreditNotes )
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long ref_number = (long)_session.Save(CreditNotes );
                    _session.Flush();
                    tx.Commit();
                    return ref_number;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  Discounts Methods

        public long AddDiscounts(Discounts ShiftDiscounts)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(ShiftDiscounts);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteDiscounts(Discounts Discounts)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(Discounts);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public Discounts GetDiscounts(long DiscountsId)
        {
            return _session.Get<Discounts>(DiscountsId);
        }

        public IList<Discounts> GetDiscountsByExample(Discounts DiscountsSample)
        {
            return _session.CreateCriteria(typeof(Discounts)).Add(Example.Create(DiscountsSample)).List<Discounts>();
        }

        public IList<Discounts> GetAllDiscounts(long SubjectId)
        {
            return _session.CreateCriteria(typeof(Discounts))
               .Add(Expression.Like("SubjectId", SubjectId))
              // .Add(Expression.Like("SubjectId", SubjectId))
               .AddOrder(Order.Asc("GroupName"))
               .List<Discounts>();
        }
        public IList<Discounts> GetAllGrps()
        {
            return _session.CreateCriteria(typeof(Discounts)).
                List<Discounts>();
        }

        public IList<Discounts> GetAllDiscountsBySubject(string GroupName, long SubjectId)
        {
            return _session.CreateCriteria(typeof(Discounts))
               .Add(Expression.Like("SubjectId", SubjectId))
               //.Add(Expression.Like("GroupName", GroupName, MatchMode.Anywhere))
                //.AddOrder(Order.Asc("GroupName"))
               .List<Discounts>();
        }


        public IList<Discounts> GetDiscountsByDescription(string Description)
        {
            return _session.CreateCriteria(typeof(Discounts))
             .Add(Expression.Like("GroupName", Description.Trim() + "%"))
                      .List<Discounts>();
        }


        public void UpdateDiscounts(Discounts Discounts)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(Discounts);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public long SaveOrUpdateDiscounts(Discounts Discounts)
        {
            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(Discounts);
                    _session.Flush();
                    tx.Commit();
                    return Discounts.Id;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  Accounts Methods

        public long AddAccounts(Accounts ShiftAccounts)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(ShiftAccounts);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteAccounts(Accounts Accounts)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(Accounts);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public Accounts GetAccounts(long AccountsId)
        {
            return _session.Get<Accounts>(AccountsId);
        }

        public IList<Accounts> GetAccountsByExample(Accounts AccountsSample)
        {
            return _session.CreateCriteria(typeof(Accounts)).Add(Example.Create(AccountsSample)).List<Accounts>();
        }

        public IList<Accounts> GetAllAccounts()
        {
            return _session.CreateCriteria(typeof(Accounts)).
                List<Accounts>();
        }
     
        public IList<Accounts> GetAllAccountsBySubject(string GroupName, long SubjectId)
        {
            return _session.CreateCriteria(typeof(Accounts))
               .Add(Expression.Like("SubjectId", SubjectId))
               .List<Accounts>();
        }


        public IList<Accounts> GetAccountsByDescription(string Description)
        {
            return _session.CreateCriteria(typeof(Accounts))
             .Add(Expression.Like("GroupName", Description.Trim() + "%"))
                      .List<Accounts>();
        }


        public void UpdateAccounts(Accounts Accounts)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(Accounts);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public long SaveOrUpdateAccounts(Accounts Accounts)
        {
            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(Accounts);
                    _session.Flush();
                    tx.Commit();
                    return Accounts.Id;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region login Methods

        public long Addlogin(login login)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(login);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void Deletelogin(login login)
        {
            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(login);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public login Getlogin(string login)
        {
            return _session.CreateCriteria(typeof(login))
                .Add(Expression.Like("username", login, MatchMode.Exact))
                .List<login>().FirstOrDefault() ;
           
        }

        public login GetloginPW(string password)
        {
            return _session.CreateCriteria(typeof(login))
                .Add(Expression.Like("password", password, MatchMode.Exact))
                .List<login>().FirstOrDefault();

        }

        public IList<login> GetloginByExample(login loginSample)
        {
            return _session.CreateCriteria(typeof(login)).Add(Example.Create(loginSample)).List<login>();
        }

        public IList<login> GetAlllogin(long MenuId1, long SubjectId1, int Position, string Country)
        {
            return _session.CreateCriteria(typeof(login))
                 .Add(Expression.Like("MenuId1", MenuId1))
                 .Add(Expression.Like("SubjectId1", SubjectId1))
                 .List<login>();
        }


        public IList<login> GetAllloginTop10(long SylabussId, long TopTen)
        {
            return _session.CreateCriteria(typeof(login))
                 .Add(Expression.Like("Date", SylabussId))
                 .Add(Expression.Like("SourceAccountNumber", SylabussId))
                 .List<login>();
        }


        public IList<login> GetloginByloginName(string AssetName)
        {
            return _session.CreateCriteria(typeof(login))
             .Add(Expression.Like("AssetName", AssetName.Trim() + "%"))
                      .List<login>();
        }


        public void Updatelogin(login login)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(login);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdatelogin(login login)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(login);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  Activeco Methods

        public long AddActiveco(Activeco Activeco)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(Activeco);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteActiveco(Activeco Activeco)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(Activeco);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public Activeco GetActiveco(string Activeco)
        {
            return _session.Get<Activeco>(Activeco);
            //  return _session.QueryOver<Activeco>()
            // .Where(u => u.ActivecoNumber == Activeco)

        }

        public IList<Activeco> GetActivecoByExample(Activeco ActivecoSample)
        {
            return _session.CreateCriteria(typeof(Activeco)).Add(Example.Create(ActivecoSample)).List<Activeco>();
        }

        public IList<Activeco> GetAllActiveco()
        {
            return _session.CreateCriteria(typeof(Activeco)).
                List<Activeco>();
        }




        public void UpdateActiveco(Activeco Shift)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(Shift);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdateShift(Activeco Shift)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(Shift);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  DailySales Methods

  public long AddDailySales(DailySales DailySales)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(DailySales);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

  public void DeleteDailySales(DailySales DailySales)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(DailySales);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

  public DailySales GetDailySales(string DailySales)
        {
            return _session.Get<DailySales>(DailySales);
            //  return _session.QueryOver<DailySales>()
            // .Where(u => u.DailySalesNumber == DailySales)

        }

   public IList<DailySales> GetDailySalesByExample(DailySales DailySalesSample)
        {
            return _session.CreateCriteria(typeof(DailySales)).Add(Example.Create(DailySalesSample)).List<DailySales>();
        }

   public IList<DailySales> GetAllDailySales()
        {
            return _session.CreateCriteria(typeof(DailySales)).
                List<DailySales>();
        }


   public void UpdateDailySales(DailySales DailySale)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(DailySale);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

   public void SaveOrUpdateDailySales(DailySales DailySale)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(DailySale);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

 #region  MonthlySales Methods

        public long AddMonthlySales(MonthlySales MonthlySales)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    long newId = (long)_session.Save(MonthlySales);
                    _session.Flush();
                    tx.Commit();
                    return newId;
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void DeleteMonthlySales(MonthlySales MonthlySales)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Delete(MonthlySales);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public MonthlySales GetMonthlySales(string MonthlySales)
        {
            return _session.Get<MonthlySales>(MonthlySales);
            //  return _session.QueryOver<MonthlySales>()
            // .Where(u => u.MonthlySalesNumber == MonthlySales)

        }

        public IList<MonthlySales> GetMonthlySalesByExample(MonthlySales MonthlySalesSample)
        {
            return _session.CreateCriteria(typeof(MonthlySales)).Add(Example.Create(MonthlySalesSample)).List<MonthlySales>();
        }

        public IList<MonthlySales> GetAllMonthlySales()
        {
            return _session.CreateCriteria(typeof(MonthlySales)).
                List<MonthlySales>();
        }




        public void UpdateMonthlySales(MonthlySales MonthlySale)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.Update(MonthlySale);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }

        public void SaveOrUpdateMonthlySales(MonthlySales MonthlySale)
        {

            using (ITransaction tx = _session.BeginTransaction())
            {
                try
                {
                    _session.SaveOrUpdate(MonthlySale);
                    _session.Flush();
                    tx.Commit();
                }
                catch (NHibernate.HibernateException)
                {
                    tx.Rollback();
                    throw;
                }
            }

        }


        #endregion

        internal void ConfirmTransaction(string message)
        {
            throw new NotImplementedException();
        }
    }
}



    




 
        
        

        














