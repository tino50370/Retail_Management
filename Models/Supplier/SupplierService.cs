using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class SupplierService
    {
        public long Id { set; get; }
        public string SupplierId { set; get; }
        public string SupplierName { set; get; }
        public string Name { set; get; }
        public bool HasProduct { set; get; }
        public string ServiceType { set; get; }
        public long YomoneyServiceId { set; get; }
        public string Description { set; get; }
        public string DateCreated { set; get; }
        public decimal AvailableBalance { set; get; }
        public decimal SuspenseBalance { set; get; }
        public decimal ActualBalance { set; get; }
        public decimal CommissionBalance { set; get; }
        public List<ServiceAction> ServiceActions { set; get; }
    }
}