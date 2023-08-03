using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class CustomerService
    {
        public long Id { set; get; }
        public string CustomerId { set; get; }
        public string CustomerName { set; get; }
        public string CustomerMobileNumber { set; get; }
        public string CustomerCardNumber { set; get; }
        public long ServiceId { set; get; }
        public long TransactionType { set; get; }
        public string ServiceName { set; get; }
        public string ServiceType { set; get; }
        public bool HasProduct { set; get; }
        public string ProductName { set; get; }
        public string ServiceProvider { set; get; }
        public string SupplierId { set; get; }
        public string SupplierName { set; get; }
        public string Description { set; get; }
        public decimal Balance { set; get; }
        public decimal SuspenseBalance { set; get; }
        public string TransactionCode { set; get; }
        public DateTime DateCreated { set; get; }
        public DateTime DatelastAccess { set; get; }
        public DateTime SubDue { set; get; }
        public string BillingCycle { set; get; }
        public string ReceiverMobile { set; get; }
        public string ResponseCode { set; get; }
        public string ResponseDescription { set; get; }
        public string logo { set; get; }     
    }
}