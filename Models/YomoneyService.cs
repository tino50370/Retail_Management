//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RetailKing.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class YomoneyService
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string ServiceProvider { get; set; }
        public Nullable<long> ServiceProviderId { get; set; }
        public string ServiceType { get; set; }
        public string Image { get; set; }
        public string Thumbnail { get; set; }
        public string ServiceBilling { get; set; }
        public Nullable<decimal> ServiceCharge { get; set; }
        public string ChargeType { get; set; }
        public string ChargeFrequency { get; set; }
        public string ServiceAudience { get; set; }
        public Nullable<decimal> ServicePrice { get; set; }
        public Nullable<long> TransactionType { get; set; }
        public Nullable<decimal> MaxSale { get; set; }
        public Nullable<decimal> MinSale { get; set; }
        public string CommisionChargeType { get; set; }
        public Nullable<decimal> CommisionValue { get; set; }
        public Nullable<decimal> CommisionLimit { get; set; }
        public Nullable<decimal> SuspenseWindow { get; set; }
        public bool HasProduct { get; set; }
        public bool RequireServiceBalance { get; set; }
        public bool IsSupplierService { get; set; }
        public bool CreateMultiple { get; set; }
        public bool IsExternal { get; set; }
        public Nullable<long> ExtServiceId { get; set; }
    }
}