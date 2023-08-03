using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RetailKing.Models;
using System.Data;


namespace RetailKing.DataAcess
{
 public class NHibernateDataProvider
 {
 private RetailKingEntities db = new RetailKingEntities();
 
 #region  Category Methods

  

    public IList<Category> GetAllCategory()
    {
        return db.Categories.ToList();
    }

    public void DeleteCategory(Category category)
    {
            try
            {
                db.Categories.Remove(category);
                 db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

    }

    public Category GetCategory(int Id)
    {
        return db.Categories.Find(Id);
    }

    public IList<Category> GetSubCategory(string name)
    {
        return db.Categories.Where(u=> u.status == name.Trim()).ToList();         
    }

    public IList<Category> GetCategoryByLocation(string location)
    {
        return db.Categories.Where(u=> u.location  == location.Trim()).ToList();      
           
    }

    public void UpdateCategory(Category category)
    {
            try
            {
                db.Entry(category).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                
                throw;
            }
    }

    public void SaveOrUpdateCategory(Category category)
    {
            try
            {
              
               db.Categories.Add(category);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                
                throw;
            }
    }


    #endregion

 #region  Items Methods

    public Item GetItems(long Id)
    {
        return db.Items.Find(Id);
    }

    public Item GetItemCode(string Id, string Company)
    {
            //return db.Items.Where(u => u.ItemCode == Id).FirstOrDefault();
           return db.Items.Where(u=> u.ItemCode == Id && u.company == Company.Trim()).FirstOrDefault();
             
    }

    public IList<Item> GetAllItems()
    {
        return db.Items.ToList();
    }

    public IList<Item> GetItemsByCode(string Id, string Company )
    {
        return db.Items.Where(u => (u.ItemName.StartsWith(Id) || u.ItemCode.StartsWith(Id)) && u.company == Company.Trim()).ToList();
    }
   
    public IList<Item> GetItemsByCategory(string Category, string Company)
    {
        return db.Items.Where(u=> u.category == Category && u.company == Company.Trim()).ToList();
        
    }

    public IList<Item> GetItemsByCompany(string Company)
    {
         return db.Items.Where(u => u.company == Company.Trim()).ToList();
        
    }
    public void SaveOrUpdateItems(Item item)
    {

        try
        {
            db.Items.Add(item);
            db.SaveChanges();
        }
        catch (Exception e)
        {
            throw;
        }

    }
    public void UpdateItems(Item item)
    {

        try
            {
               db.Entry(item).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            { 
                throw;
            }

    }

    public void DeleteItems(Item item)
    {
            try
            {
                //db.Items.Remove(item);
                //db.SaveChanges();
                db.Entry(item).State = EntityState.Deleted;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

    }

    #endregion
      
 #region  InvoiceLines Methods

    public void DeleteInvoiceLines(InvoiceLine InvoiceLines)
    {

            try
            {
                db.InvoiceLines.Remove(InvoiceLines);
                db.SaveChanges();
            }
            catch (Exception e )
            {
                throw;
            }

    }

    public InvoiceLine GetInvoiceLines(long Id)
    {
        return db.InvoiceLines.Find(Id);
    }

   

    public IList<InvoiceLine> GetAllInvoiceLines()
    {
        return db.InvoiceLines.ToList();
    }


    public void UpdateInvoiceLines(InvoiceLine InvoiceLines)
    {

           try
            {
               db.Entry(InvoiceLines).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            { 
                throw;
            }

    }

    public void SaveOrUpdateInvoiceLines(InvoiceLine InvoiceLines)
    {

           try
            {
                db.InvoiceLines.Add(InvoiceLines);
                db.SaveChanges();
            }
            catch (Exception e)
            { 
                throw;
            }

    }


    #endregion

 #region  Invoices Methods

   public IList<Invoice> GetAllInvoices()
    {
       return  db.Invoices.ToList();
    }

    public IList<Invoice> GetAllInvoicesByCompany(string  Company)
    {
        return db.Invoices.Where(u => u.company == Company).ToList();
    }

    public Invoice GetInvoices(long Id)
    {
        return db.Invoices.Find(Id);
    }

   
    
    public void UpdateInvoices(Invoice invoice)
    {

        try
        {
            db.Entry(invoice).State = EntityState.Modified;
            db.SaveChanges();
        }
        catch (Exception e)
        {

            throw;
        }

    }

    public void SaveOrUpdateInvoices(Invoice invoice)
    {

        try
        {
            db.Invoices.Add(invoice);
            db.SaveChanges();
        }
        catch (Exception e)
        {

            throw;
        }

    }


    #endregion

 #region OrderLines Methods

    public long AddOrderLines(OrderLine Account)
    {
        try
        {
            db.Entry(Account).State = EntityState.Modified;
            db.SaveChanges();
            return Account.ID;
        }
        catch (Exception e)
        {

            throw;
        }
            

    }

    public void DeleteOrderLines(OrderLine Account)
    {

            try
            {
                db.OrderLines.Remove(Account);
                db.SaveChanges();
               
            }
            catch (Exception )
            {
              
                throw;
            }
        

    }

    public OrderLine GetOrderLines(long Account)
    {
        return db.OrderLines.Find(Account);
    }


    public IList<OrderLine> GetAllOrderLines()
    {
        return db.OrderLines.ToList();
    }

    public IList<OrderLine> GetOrderLinesByReciept(string Reciept)
    {
        return db.OrderLines.Where(u => u.Reciept  == Reciept ).ToList(); 
       
    }

    public IList<OrderLine> GetOrderLinesByItemCode(string ItemCode)
    {
        return db.OrderLines.Where(u => u.ItemCode == ItemCode).ToList(); 
    }

   
    public void UpdateOrderLines(OrderLine OrderLines)
    {
        try
        {
            db.Entry(OrderLines).State = EntityState.Modified;
            db.SaveChanges();
          
        }
        catch (Exception e)
        {

            throw;
        }
      

    }

    public void SaveOrUpdateOrderLines(OrderLine Account)
    {
        try
        {
            db.OrderLines.Add(Account);
            db.SaveChanges();

        }
        catch (Exception e)
        {

            throw;
        }
    }


    #endregion

 #region Orders Methods

    public long AddOrders(Order Account)
    {

        try
        {
            db.Orders.Add(Account);
            db.SaveChanges();
            return Account.ID;
        }
        catch (Exception e)
        {

            throw;
        }

    }

    public void DeleteOrders(Order Account)
    {

        try
        {
            db.Orders.Remove(Account);
            db.SaveChanges();

        }
        catch (Exception)
        {

            throw;
        }

    }

    public Order GetOrders(long Account)
    {
         return db.Orders.Find(Account);
    }

    public IList<Order> GetAllOrders()
    {
          return db.Orders.ToList();
    }

    public IList<Order> GetOrderByReciept(string Reciept)
    {
        return db.Orders.Where(u=>u.Reciept.Trim() == "Reciept" ).ToList();
       
    }
   

      public void SaveOrUpdateOrders(Order Account)
    {
        try
        {
            db.Entry(Account).State = EntityState.Modified;
            db.SaveChanges();
            

        }
        catch (Exception e)
        {

            throw;
        }

    }
    #endregion

 #region PurchaseLines Methods

    public long AddPurchaseLines(PurchaseLine Account)
    {

        try
        {
            db.PurchaseLines.Add(Account);
            db.SaveChanges();
            return Account.ID;
        }
        catch (Exception e)
        {

            throw;
        }

    }

    public void DeletePurchaseLines(PurchaseLine Account)
    {

        try
        {
            db.PurchaseLines.Remove(Account);
            db.SaveChanges();

        }
        catch (Exception)
        {

            throw;
        }
    }

    public PurchaseLine GetPurchaseLines(long Account)
    {
        return db.PurchaseLines.Find(Account);
    }
public PurchaseLine GetPurchaseLinesById(long ID)
        {
            return db.PurchaseLines.Find(ID);
        }
    public IList<PurchaseLine> GetAllPurchaseLines()
    {
        return db.PurchaseLines.ToList();
    }



    public void SaveOrUpdatePurchaseLines(PurchaseLine Account)
    {

        try
        {
            db.Entry(Account).State = EntityState.Modified;
            db.SaveChanges();

        }
        catch (Exception e)
        {

            throw;
        }

    }

    #endregion
  
 #region  Purchases Methods

        public long AddPurchases(Purchase Purchases)
        {

            try
            {
                db.Purchases.Add(Purchases);
                db.SaveChanges();
                return Purchases.ID;
            }
            catch (Exception e)
            {
                
                throw;
            }

        }

        public void DeletePurchases(Purchase Purchases)
        {

            try
            {
                db.Purchases.Remove(Purchases);
                db.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public Purchase GetPurchases(int Purchases)
        {
            return db.Purchases.Find(Purchases);
        }

        public Purchase GetPurchaseByReceipt(string reciept)
        {
            return db.Purchases.Where(u => u.Reciept.Trim() == reciept).FirstOrDefault();
        }
        public IList<Purchase> GetAllPurchases()
        {
            return db.Purchases.ToList();
        }

        public IList<Purchase> GetPurchasesOpenCredit(string Supplier, string Company)
        {
            return db.Purchases
                .Where(u => u.supplier == Supplier
                && u.Status.Trim() == "O"
                && u.company.Trim() == Company
                ).ToList();
           
        }

        public IList<Purchase> GetAllPurchasesOpenCredit(string Company)
        {
            return db.Purchases
               .Where(u => u.Status.Trim() == "O"
               && u.company.Trim() == Company
               ).ToList();
            
        }


        public void SaveOrUpdatePurchases(Purchase Purchases)
        {

            try
            {
                db.Entry(Purchases).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception e)
            {

                throw;
            }

        }


        #endregion

 #region  ReturnLines Methods

        public long AddReturnLines(ReturnLine ReturnLines)
        {

            try
            {
                db.ReturnLines.Add(ReturnLines);
                db.SaveChanges();
                return ReturnLines.ID;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public void DeleteReturnLines(ReturnLine ReturnLines)
        {

            try
            {
                db.ReturnLines.Remove(ReturnLines);
                db.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }

        }

        public ReturnLine GetReturnLines(int ReturnLinesId)
        {
            return db.ReturnLines.Find(ReturnLinesId);
        }
       

        public void SaveOrUpdateReturnLines(ReturnLine ReturnLines)
        {

            try
            {
                db.Entry(ReturnLines).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception e)
            {

                throw;
            }


        }


   #endregion

 #region  Returns Methods

        public long AddReturns(Return ShiftReturns)
        {

            try
            {
                db.Returns.Add(ShiftReturns);
                db.SaveChanges();
                return ShiftReturns.ID;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public void DeleteReturns(Return Returns)
        {

            try
            {
                db.Returns.Remove(Returns);
                db.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }

        }

        public Return GetReturns(int ReturnsId)
        {
            return db.Returns.Find(ReturnsId);
        }


        public IList<Return> GetAllReturns()
        {
            return db.Returns.ToList();
        }


       
        public void SaveOrUpdateReturns(Return Returns)
        {


            try
            {
                db.Entry(Returns).State = EntityState.Modified;
                db.SaveChanges();

            }
            catch (Exception e)
            {

                throw;
            }

        }


        #endregion

 #region  Sales Methods

        public long AddSales(Sale ShiftSales)
        {

                try
                {
                    db.Sales.Add(ShiftSales);
                   var newId = ShiftSales.ID;
                    return newId;
                }
                catch (Exception e)
                {
                    
                    throw;
                }
            

        }

        public void DeleteSales(Sale Sales)
        {
                try
                {
                  
                   db.Sales.Remove(Sales);
                   db.SaveChanges();
                }
                catch (Exception e)
                {
                   
                    throw;
                }

        }

        public Sale GetSales(long SalesId)
        {
            return db.Sales.Find(SalesId);
        }

        public IList<Sale> GetAllSalesRange(DateTime  date, DateTime  endDate)
        {  
            var  rex = db.Sales
           .Where(u => u.dated>=date && u.dated <= endDate)
           .ToList();
            return rex;
        }

        public IList<Sale> GetAllSales()
        {
            return db.Sales.ToList();
        }

        public IList<Sale> GetSalesOpenCredit(string Customer, string Company)
        {
            return db.Sales.Where(u => u.customer == Customer.Trim() && u.PaymentModes.Contains("CREDIT LINE") && ( u.Balance == null || u.Balance > 0 )&& u.company == Company.Trim()).ToList();
            
        }

        public IList<Sale> GetAllSalesOpenCredit( string Company)
        {
            return db.Sales.Where(u => u.state == "O" && u.company == Company.Trim()).ToList();
 
        }

        public void SaveOrUpdateSales(Sale Sales)
        {
           try
            {
               db.Entry(Sales).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            { 
                throw;
            }

        }
    

        #endregion

 #region  SalesLines Methods

        public long AddSalesLines(Sales_Lines ShiftSalesLines)
        {

            db.Sales_Lines.Add(ShiftSalesLines);
            db.SaveChanges();
            return ShiftSalesLines.ID;

        }

        public void DeleteSalesLines(Sales_Lines SalesLines)
        {
                try
                {
                    db.Sales_Lines.Remove(SalesLines);
                    db.SaveChanges();
                }
                catch (Exception e )
                {
                  
                    throw;
                }

        }

        public Sales_Lines GetSalesLines(string SalesLinesId)
        {
            return db.Sales_Lines.Find(SalesLinesId);
        }



        public IList<Sales_Lines> GetSalesLinesByReciept(string Reciept)
        {
            return db.Sales_Lines.Where(u => u.Reciept == Reciept.Trim()).ToList();
            
        }


        public void UpdateSalesLines(Sales_Lines SalesLines)
        {

            try
            {
                db.Entry(SalesLines).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void SaveOrUpdateSalesLines(Sales_Lines SalesLines)
        {

            try
            {
                db.Sales_Lines.Add(SalesLines);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 //#region  TransferLines Methods

 //       public long AddTransferLines(TransferLine TransferLines)
 //       {

 //               try
 //               {
 //                   db.TransferLines.Add(TransferLines);
 //                   db.SaveChanges();
 //                   return TransferLines.ID;
 //               }
 //               catch (Exception )
 //               {
                   
 //                   throw;
 //               }
            

 //       }

 //       public void DeleteTransferLines(TransferLine TransferLines)
 //       {

 //           try
 //           {
 //               db.TransferLines.Remove(TransferLines);
 //               db.SaveChanges();
 //           }
 //           catch (Exception e)
 //           {

 //               throw;
 //           }

 //       }

 //       public TransferLine GetTransferLines(string TransferLinesId)
 //       {
 //           return db.TransferLines.Find(TransferLinesId);
 //       }


 //       public void SaveOrUpdateTransferLines(TransferLine TransferLines)
 //       {

 //           try
 //           {
 //               db.Entry(TransferLines).State = EntityState.Modified;
 //               db.SaveChanges();
 //           }
 //           catch (Exception e)
 //           {
 //               throw;
 //           }

 //       }


 //       #endregion

 //#region  Transfers Methods

 //       public long AddTransfers(Transfer Transfers)
 //       {
 //           try
 //           {
 //               db.Entry(Transfers).State = EntityState.Modified;
 //               var newId = Transfers.ID;
 //               return newId;
 //           }
 //           catch (Exception e)
 //           {

 //               throw;
 //           }
            
 //       }

 //       public void DeleteTransfers(Transfer transfers)
 //       {

 //           db.Transfers.Remove(transfers);
 //           db.SaveChanges();

 //       }

 //       public Transfer GetTransfers(long Id)
 //       {
 //           return db.Transfers.Find(Id);
 //       }

       
 //       public IList<Transfer> GetAllTransfers()
 //       {
 //           return db.Transfers.ToList();
 //       }

 //       public void SaveOrUpdateTransfers(Transfer Transfers)
 //       {
 //           try
 //           {
 //               db.Entry(Transfers).State = EntityState.Modified;
 //               db.SaveChanges();
 //           }
 //           catch (Exception e)
 //           {
 //               throw;
 //           }

 //       }


 //       #endregion

 #region  Expenses Methods

        public long AddExpenses(Expens Expenses)
        {
            try
            {
                db.Entry(Expenses).State = EntityState.Added ;
                db.SaveChanges();
                var newId = Expenses.ID;
                return newId;
            }
            catch (Exception e)
            {

                throw;
            }
            

        }

        public void DeleteExpenses(Expens Expenses)
        {
            db.Expenses.Remove(Expenses);
            db.SaveChanges();
        }

        public Expens GetExpenses(long Expenses)
        {
            return db.Expenses.Find(Expenses);
        }

      
        public IList<Expens> GetAllExpenses()
        {
            return db.Expenses.ToList();
        }


      public IList<Expens> GetExpensesBydate(DateTime date,string AccountNumber)
        {
           
            return db.Expenses.Where(u => u.Dated == date && u.AccountCode == AccountNumber).ToList();
            
        }

      public IList<Expens> GetExpensesBydateRange(string Company,DateTime   date, DateTime  endDate)
      {
          var dd = date.ToString();
          return db.Expenses.Where(u => u.till == Company && u.Dated >= date && u.Dated <= endDate).ToList();
        
      }

        public void SaveOrUpdateExpenses(Expens Expenses)
        {

            try
            {
                db.Entry(Expenses).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 #region Customers
        public Customer GetCustomers(long Account)
        {
            return db.Customers.Find(Account);
        }

        public IList<Customer> GetAllCustomers()
        {
            return db.Customers.ToList();
        }

        public IList<Customer> GetCustomersByName(string Name)
        {
            return db.Customers.Where(u => u.CustomerName == Name).ToList();
           
        }

        public IList<Customer> GetCustomersSearch(string Description)
        {
              return db.Customers
              .Where(u => u.CustomerName.Contains(Description.Trim().ToLower()) || u.AccountCode.Contains(Description.Trim().ToLower())|| u.Phone1.Contains(Description))
              .ToList();
        }

        public void SaveOrUpdateCustomers(Customer Account)
        {
            try
            {
                db.Customers.Add(Account);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void UpdateCustomers(Customer Account)
        {
            try
            {
                db.Entry(Account).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void DeleteCompanies(Customer customers)
        {

            db.Customers.Remove(customers);
            db.SaveChanges();

        }
        #endregion

        #region Guest

        public Guest GetGuest(long Account)
        {
            return db.Guests.Find(Account);
        }

        public IList<Guest> GetAllGuest()
        {
            return db.Guests.ToList();
        }

        public IList<Guest> GetGuestByName(string Name)
        {
            return db.Guests.Where(u => u.CustomerName == Name).ToList();

        }

        public IList<Guest> GetGuestSearch(string Description)
        {
            return db.Guests
            .Where(u => u.CustomerName.Contains(Description.Trim().ToLower()) || u.AccountCode.Contains(Description.Trim().ToLower()) || u.Phone1.Contains(Description))
            .ToList();
        }

        public void SaveOrUpdateGuest(Guest Account)
        {
            try
            {
                db.Guests.Add(Account);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void UpdateGuest(Guest Account)
        {
            try
            {
                db.Entry(Account).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void DeleteCompanies(Guest guest)
        {

            db.Guests.Remove(guest);
            db.SaveChanges();

        }
        #endregion
 #region System Key
        public Syskey GetSyskeys(long Account)
        {
            return db.Syskeys.Find(Account);
        }

        public IList<Syskey> GetAllSyskeys()
        {
            return db.Syskeys.ToList();
        }

        public Syskey GetSyskeysByName(string Name, string Company)
        {
            return db.Syskeys.Where(u => u.Name.Trim() == Name && u.Company.Trim() == Company).FirstOrDefault();

        }
        public IList<Syskey> GetSyskeysSearch(string Description, string Company)
        {
            return db.Syskeys
              .Where(u => (u.Description.Contains(Description.Trim().ToLower()) || u.Name.Contains(Description.Trim().ToLower())) && u.Company.Trim() == Company)
              .ToList();
        }

        public void SaveOrUpdateSyskeys(Syskey Account)
        {
            try
            {
                db.Syskeys.Add(Account);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }
        public void UpdateSyskeys(Syskey Account)
        {
            try
            {
                db.Entry(Account).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public void DeleteCompanies(Syskey Syskeys)
        {

            db.Syskeys.Remove(Syskeys);
            db.SaveChanges();

        }
        #endregion

 #region Suppliers
        public Supplier GetSuppliers(long Account)
        {
            return db.Suppliers.Find(Account);
        }

        public IList<Supplier> GetAllSuppliers()
        {
            return db.Suppliers.ToList();
        }

        public IList<Supplier> GetSuppliersByName(string Name)
        {
           return db.Suppliers.Where(u => u.SupplierName == Name).ToList();
            
        }

        public IList<Supplier> GetSuppliersByCode(string Name)
        {
            return db.Suppliers.Where(u => u.AccountCode  == Name).ToList();

        }

        public IList<Supplier> GetSuppliersSearch(string Description)
        {
            return db.Suppliers
             .Where(u => u.SupplierName.Contains(Description.Trim().ToLower()) || u.AccountCode.Contains(Description.Trim().ToLower()))
             .ToList();
          
        }

        public long AddSupplier(Supplier  company)
        {

            try
            {
                db.Suppliers.Add(company);
                db.SaveChanges();
                var newId = company.ID;
                return newId;
            }
            catch (Exception e)
            {

                throw;
            }

        }


        public void SaveOrUpdateSuppliers(Supplier Account)
        {
            try
            {
                db.Entry(Account).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void UpdateSuppliers(Supplier Account)
        {
            try
            {
                db.Entry(Account).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void DeleteCompanies(Supplier Suppliers)
        {

              try
                {
                    db.Suppliers.Remove(Suppliers);
                    db.SaveChanges();
                    
                }
                catch (Exception )
                {
                   
                    throw;
                }
            

        }
        #endregion

 #region Companies

        public Company GetCompanies(string Account)
        {
           return  db.Companies.Where(u => u.name == Account).FirstOrDefault();
            
        }

        public Company GetCompaniesById(long Account)
        {
             return db.Companies.Find(Account);
        }

        public IList<Company> GetAllCompanies()
        {
            return db.Companies.ToList(); 
        }

        public long AddCompany(Company company)
        {

            try
            {
                db.Companies.Add(company);
                db.SaveChanges();
                var newId = company.ID;
                return newId;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public void SaveOrUpdateCompanies(Company Account)
        {
            try
            {
                db.Entry(Account).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void DeleteCompanies(Company companies)
        {

                try
                {
                    db.Companies.Remove(companies);
                    db.SaveChanges();
                    
                }
                catch (NHibernate.HibernateException)
                {
                    
                    throw;
                }

        }
        #endregion

 #region  Shift Methods

        public long AddShift(shift shift)
        {
            try
            {
                db.Entry(shift).State = EntityState.Modified;
                var newId = shift.ID;
                return newId;
            }
            catch (Exception e)
            {

                throw;
            }

        }

        public void DeleteShift(shift Shift)
        {

                try
                {
                    db.shifts.Remove(Shift);
                    db.SaveChanges();
                    
                }
                catch (Exception e)
                {
                    
                    throw;
                }

        }

        public shift GetShift(string Shift)
        {
            return db.shifts.Find(Shift);         
        }


        public IList<shift> GetAllShift()
        {
            return db.shifts.ToList();
           
        }


        public void UpdateShift(shift Shift)
        {

            try
            {
                db.Entry(Shift).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void SaveOrUpdateShift(shift Shift)
        {


            try
            {
                db.shifts.Add(Shift);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 #region  PosMenu Methods

        public long AddMenu(PosMenu PosMenu)
        {
  
            try
            {
                db.Entry(PosMenu).State = EntityState.Modified;
                db.SaveChanges();
                var newId = PosMenu.ID;
                return newId;
            }
            catch (Exception e)
            {

                throw;
            }
     

        }

        public void DeleteMenu(PosMenu PosMenu)
        {

                try
                {
                    db.PosMenus.Remove(PosMenu);
                    db.SaveChanges();
                    
                }
                catch (Exception e)
                {
                    
                    throw;
                }
 
        }

        public IList<PosMenu> GetMenu(string PosMenu)
        {
            return db.PosMenus.Where(u => u.company == PosMenu.Trim()).ToList();    
        }

        public IList<PosMenu> GetAllMenu()
        {
            return db.PosMenus.ToList();
        }

        public IList<PosMenu> GetMenuByMenuName(long  id,string  Company )
        {
            return db.PosMenus.Where(u => u.ID == id && u.company == Company.Trim()).ToList();
            
        }

        public void UpdateMenu(PosMenu PosMenu)
        {
            try
            {
                db.Entry(PosMenu).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void SaveOrUpdateMenu(PosMenu PosMenu)
        {
            try
            {
                db.PosMenus.Add(PosMenu);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        #endregion

 #region  CreditNotes Methods

        public void DeleteCreditNotes (CreditNote  CreditNotes )
        {
                try
                {
                    db.CreditNotes.Remove(CreditNotes);
                    db.SaveChanges();
                }
                catch (Exception e)
                {
                   
                    throw;
                }
        }

        public CreditNote  GetCreditNotes (long Id)
        {
            return db.CreditNotes.Find(Id);
        }
   
        public IList<CreditNote> GetAllCreditNotes ()
        {
            return db.CreditNotes.ToList();
        }

        public IList<CreditNote > GetCreditNotesById(long Id)
        {
           return  db.CreditNotes.Where(u => u.ID == Id).ToList();
           
        }

        public void UpdateCreditNotes (CreditNote  CreditNotes )
        {

            try
            {
                db.Entry(CreditNotes).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }

        public long SaveOrUpdateCreditNotes (CreditNote  CreditNotes )
        {

            try
            {
                db.CreditNotes.Add(CreditNotes);
                db.SaveChanges();
                return CreditNotes.ID;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        #endregion

 #region  Discounts Methods

        public long AddDiscounts(Discount discounts)
        {

            try
            {
                db.Entry(discounts).State = EntityState.Modified;
                db.SaveChanges();
                return discounts.ID;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void DeleteDiscounts(Discount Discounts)
        {

                try
                {
                    db.Discounts.Remove(Discounts);
                  db.SaveChanges();
                }
                catch (Exception )
                {
                   
                    throw;
                }

        }

        public Discount GetDiscounts(long DiscountsId)
        {
            return db.Discounts.Find(DiscountsId);
        }

        public IList<Discount> GetAllDiscounts()
        {
            return db.Discounts.ToArray();
        }

        public void UpdateDiscounts(Discount Discounts)
        {
            try
            {
                db.Entry(Discounts).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public long SaveOrUpdateDiscounts(Discount Discounts)
        {

            try
            {
                db.Discounts.Add(Discounts);
                db.SaveChanges();
                return Discounts.ID;
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 #region  Accounts Methods

        public long AddAccounts(Account accounts)
        {

            try
            {
                db.Entry(accounts).State = EntityState.Modified;
                db.SaveChanges();
                return accounts.ID;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void DeleteAccounts(Account Accounts)
        {

                try
                {
                    db.Accounts.Remove(Accounts);
                    db.SaveChanges();
                }
                catch (Exception)
                {
                   
                    throw;
                }
        }

        public Account GetAccounts(long AccountsId)
        {
            return db.Accounts.Find(AccountsId);
        }
     
        public IList<Account> GetAllAccounts()
        {
            return db.Accounts.ToList();
        }
     
        public IList<Account> GetAccountsByCode(string Code)
        {
            return db.Accounts.Where(u => u.AccountCode.StartsWith(Code)).ToList();
        }

        public IList<Account> GetAccountsByName(string Name)
        {
            return db.Accounts.Where(u => u.AccountName == Name).ToList();
           
        }

        public IList<Account> GetAccountSearch(string Description)
        {
            return db.Accounts
             .Where(u => u.AccountName.Contains(Description.Trim().ToLower()) || u.AccountCode.Contains(Description.Trim().ToLower()))
             .ToList();
            
        }

        public void UpdateAccounts(Account accounts)
        {

            try
            {
                db.Entry(accounts).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public long SaveOrUpdateAccounts(Account Accounts)
        {

            try
            {
                db.Accounts.Add(Accounts);
                db.SaveChanges();
                return Accounts.ID;
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

        #region  Currency Methods

        public long AddCurrencies(Currency currencies)
        {

            try
            {
                db.Entry(currencies).State = EntityState.Modified;
                db.SaveChanges();
                return currencies.Id;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        

        public Currency GetCurrencies(long Id)
        {
            return db.Currencies.Find(Id);
        }

        public IList<Currency> GetAllCurrencies()
        {
            return db.Currencies.ToList();
        }

        public IList<Currency> GetCurrenciesByName(string Name)
        {
            return db.Currencies.Where(u => u.Curency == Name).ToList();

        }


        public void UpdateCurrencies(Currency currencies)
        {

            try
            {
                db.Entry(currencies).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public long SaveOrUpdateCurrencies(Currency currencies)
        {

            try
            {
                db.Currencies.Add(currencies);
                db.SaveChanges();
                return currencies.Id;
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 #region Transactions Methods

        public long AddTransactions(TransactionData tran)
        {

            try
            {
                db.TransactionDatas.Add(tran);
                db.SaveChanges();
                return tran.ID;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void DeleteTransactions(TransactionData Transactions)
        {

             try
                {
                    db.TransactionDatas.Remove(Transactions);
                    db.SaveChanges();
                    
                }
                catch (Exception )
                {
                  
                    throw;
                }
            

        }

        public TransactionData GetTransactions(long TransactionsId)
        {
            return db.TransactionDatas.Find(TransactionsId);
        }

        public IList<TransactionData> GetAllTransactions()
        {
            return db.TransactionDatas.ToList();
        }

        public IList<TransactionData> GetTransactionsByName(string Name)
        {
            return db.TransactionDatas.Where(u => u.TransactionName == Name).ToList(); 
        }

        public IList<TransactionData> GetTransactionsByType(string type)
        {
            return db.TransactionDatas.Where(u => u.Type == type).ToList(); 
            
        }

        public void SaveOrUpdateTransactions(TransactionData Transactions)
        {

            try
            {
               // db.TransactionDatas.Add(Transactions);
                db.Entry(Transactions).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 #region Asset Methods

        public long AddAsset(Asset tran)
        {

            try
            {
                db.Assets.Add(tran);
                db.SaveChanges();
                return tran.Id;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void DeleteAssets(Asset Assets)
        {

            try
            {
                db.Assets.Remove(Assets);
                db.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }


        }

        public Asset GetAsset(long AssetId)
        {
            return db.Assets.Find(AssetId);
        }

        public IList<Asset> GetAllAssets()
        {
            return db.Assets.ToList();
        }

        public IList<Asset> GetAssetByName(string Name)
        {
            return db.Assets.Where(u => u.Name == Name).ToList();
        }

        public IList<Asset> GetAssetByType(string type)
        {
            return db.Assets.Where(u => u.Type == type).ToList();
        }

        public void SaveOrUpdateTransactions(Asset Assets)
        {

            try
            {
                // db.TransactionDatas.Add(Transactions);
                db.Entry(Assets).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 #region Facility Methods

        public long AddFacility(Facility tran)
        {

            try
            {
                db.Facilities.Add(tran);
                db.SaveChanges();
                return tran.Id;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void DeleteFacilitys(Facility Facility)
        {
            try
            {
                db.Facilities.Remove(Facility);
                db.SaveChanges();

            }
            catch (Exception)
            {
                throw;
            }

        }

        public Facility GetFacility(long FacilityId)
        {
            return db.Facilities.Find(FacilityId);
        }

        public IList<Facility> GetAllFacilitys()
        {
            return db.Facilities.ToList();
        }

        public IList<Facility> GetFacilityByName(string Name)
        {
            return db.Facilities.Where(u => u.Name == Name).ToList();
        }

        public IList<Facility> GetFacilityChildren(long FacilityId)
        {
            return db.Facilities.Where(u => u.ParentId == FacilityId).ToList();
        }

        public void SaveOrUpdateTransactions(Facility Facilities)
        {

            try
            {
                // db.TransactionDatas.Add(Transactions);
                db.Entry(Facilities).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 #region Equipment Methods

        public long AddEquipment(Equipment tran)
        {

            try
            {
                db.Equipments.Add(tran);
                db.SaveChanges();
                return tran.Id;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void DeleteEquipments(Equipment Equipments)
        {
            try
            {
                db.Equipments.Remove(Equipments);
                db.SaveChanges();
            }
            catch (Exception)
            {

                throw;
            }
        }

        public Equipment GetEquipment(long EquipmentId)
        {
            return db.Equipments.Find(EquipmentId);
        }

        public IList<Equipment> GetAllEquipments()
        {
            return db.Equipments.ToList();
        }

        public IList<Equipment> GetEquipmentByName(string Name)
        {
            return db.Equipments.Where(u => u.Name == Name).ToList();
        }

        public IList<Equipment> GetEquipmentByFacility(long FacilityId)
        {
            return db.Equipments.Where(u => u.FacilityId == FacilityId).ToList();
        }

        public IList<Equipment> GetEquipmentByAsset(long AssetId)
        {
            return db.Equipments.Where(u => u.EntityId == AssetId).ToList();
        }

        public void SaveOrUpdateTransactions(Equipment Equipments)
        {

            try
            {
                // db.TransactionDatas.Add(Transactions);
                db.Entry(Equipments).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        #endregion

 #region Tenant Methods

        public long AddTenant(Tenant tran)
        {

            try
            {
                db.Tenants.Add(tran);
                db.SaveChanges();
                return tran.Id;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void DeleteTenants(Tenant Tenants)
        {

            try
            {
                db.Tenants.Remove(Tenants);
                db.SaveChanges();
            }
            catch (Exception)
            {
                throw;
            }

        }

        public Tenant GetTenant(long TenantId)
        {
            return db.Tenants.Find(TenantId);
        }

        public IList<Tenant> GetAllTenants()
        {
            return db.Tenants.ToList();
        }

        public IList<Tenant> GetTenantByName(string Name)
        {
            return db.Tenants.Where(u => u.Name == Name).ToList();
        }

        public IList<Tenant> GetTenantByLeaseType(string type)
        {
            return db.Tenants.Where(u => u.lease == type).ToList();
        }

        public void SaveOrUpdateTransactions(Tenant Tenants)
        {
            try
            {
                // db.TransactionDatas.Add(Transactions);
                db.Entry(Tenants).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        #endregion

 #region login Methods

        public long Addlogin(login login)
        {

            try
            {
                db.Entry(login).State = EntityState.Modified;
                db.SaveChanges();
                return login.ID;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void Deletelogin(login login)
        {
          
                try
                {
                    db.logins.Remove(login);
                    db.SaveChanges();
                    
                }
                catch (Exception )
                {
                    
                    throw;
                }
            

        }

        public login Getlogin(string login)
        {
            return db.logins.Where(u => u.username.Trim() == login.Trim() || u.Firstname.Trim() == login.Trim()).FirstOrDefault();
            
        }

        public login GetloginById(long Id)
        {
            return db.logins.Find(Id);

        }

        public login GetloginPW(string password)
        {
            return db.logins.Where(u => u.password.Trim() == password.Trim()).FirstOrDefault();
           
        }


        public IList<login> GetAlllogin( string Location)
        {
            return db.logins.Where(u => u.Location.Trim() == Location.Trim()).ToList();
           
        }



        public void SaveOrUpdatelogin(login login)
        {

            try
            {
                db.logins.Add(login);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void UpdateLogin(login login)
        {

            try
            {
                db.Entry(login).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 #region  Activeco Methods

        public long AddActiveco(Activeco Activeco)
        {
            try
            {
                db.Entry(Activeco).State = EntityState.Modified;
                db.SaveChanges();
                return Activeco.ID;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void DeleteActiveco(Activeco Activeco)
        {
            try
            {
                db.Activecoes.Remove(Activeco);
                db.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }
            

        }

        public Activeco GetActiveco(string Activeco)
        {
            return db.Activecoes.Find(Activeco);
          
        }

        public Activeco GetActivecoByName(string company)
        {
            return db.Activecoes.Where(u => u.company == company).FirstOrDefault();
            
        }


        public IList<Activeco> GetAllActiveco()
        {
            return db.Activecoes.ToList();
        }





        public void SaveOrUpdateActiveco(Activeco Shift)
        {


            try
            {
                db.Activecoes.Add(Shift);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 #region  PosKeys Methods

        public long AddPosKeys(PosKey Activeco)
        {

            try
            {
                db.Entry(Activeco).State = EntityState.Modified;
                db.SaveChanges();
                return Activeco.Id;
            }
            catch (Exception e)
            {
                throw;
            }

        }

        public void DeletePosKeys(PosKey Activeco)
        {

            try
            {
                db.PosKeys.Remove(Activeco);
                db.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }
            

        }

        public PosKey GetPosKeys(string company)
        {
            return db.PosKeys.Where(u => u.Company == company).FirstOrDefault();
           

        }

        public IList<PosKey> GetAllPosKeys()
        {
            return db.PosKeys.ToList();
            
        }


        public void SaveOrUpdateShift(PosKey Shift)
        {

            try
            {
                db.PosKeys.Add(Shift);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 #region  DailySales Methods

  public string AddDailySales(DailySale DailySales)
        {

            try
            {
               
                db.DailySales.Add(DailySales);
                db.SaveChanges();
                return DailySales.Id;
            }
            catch (Exception e)
            {
                throw;
            }

        }

  public void DeleteDailySales(DailySale DailySales)
        {

            try
            {
                db.DailySales.Remove(DailySales);
                db.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }

        }

  public DailySale GetDailySales(string DailySales)
        {
            return db.DailySales.Where(u => u.Id == DailySales).FirstOrDefault();
            
        }

   

   public IList<DailySale> GetAllDailySales()
        {
            return db.DailySales.ToList();
        }


  

   public void SaveOrUpdateDailySales(DailySale DailySale)
   {

            try
            {
                var dd = db.DailySales.Find(DailySale.Id);
                if (dd != null)
                {
                    db.Entry(DailySale).State = EntityState.Modified;
                }
                else
                {
                    db.DailySales.Add(DailySale);
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }


        #endregion

 #region  MonthlySales Methods

        public string AddMonthlySales(MonthlySale MonthlySales)
        {

            try
            {
                db.MonthlySales.Add(MonthlySales);
                db.SaveChanges();
                return MonthlySales.Id;
            }
            catch (Exception e)
            {
                throw;
            }


        }

        public void DeleteMonthlySales(MonthlySale MonthlySales)
        {

            try
            {
                db.MonthlySales.Remove(MonthlySales);
                db.SaveChanges();

            }
            catch (Exception)
            {

                throw;
            }
        }

        public MonthlySale GetMonthlySales(string MonthlySales)
        {
            return db.MonthlySales.Where(u => u.Id == MonthlySales).FirstOrDefault();

        }

       

        public IList<MonthlySale> GetAllMonthlySales()
        {
            return db.MonthlySales.ToList();   
            
        }



        public void SaveOrUpdateMonthlySales(MonthlySale MonthlySale)
        {

            try
            {
                db.Entry(MonthlySale).State = EntityState.Modified;
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }
        }


        #endregion
        #region Loss
        public IList<Loss> GetAllLosses()
        {
            return db.Losses.ToList();
        }
        public void SaveOrUpdateLossLines(LossLine LossLines)
        {

            try
            {
                db.LossLines.Add(LossLines);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }
#endregion
        #region Transfers
        public IList<Transfer> GetAllTransfers()
        {
            return db.Transfers.ToList();
        }
        public void SaveOrUpdateTransferingLines(TransferingLine TransferingLine)
        {

            try
            {
                db.TransferingLines.Add(TransferingLine);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                throw;
            }

        }
    
        #endregion

        internal void ConfirmTransaction(string message)
        {
            throw new NotImplementedException();
        }
    }
}



    




 
        
        

        














