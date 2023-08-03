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
    
    public partial class Asset
    {
        public long Id { get; set; }
        public string Type { get; set; }
        public string AssetCode { get; set; }
        public Nullable<System.DateTime> AquisitionDate { get; set; }
        public string AquisitionType { get; set; }
        public Nullable<decimal> lifespan { get; set; }
        public Nullable<System.DateTime> DisposalDate { get; set; }
        public string Location { get; set; }
        public Nullable<decimal> CurrentValue { get; set; }
        public Nullable<decimal> PurchaseValue { get; set; }
        public string Name { get; set; }
        public string CompanyId { get; set; }
        public Nullable<long> FacilityCount { get; set; }
        public Nullable<long> EquipmentCount { get; set; }
        public string Status { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Area { get; set; }
        public string Detail { get; set; }
        public Nullable<long> ZoneId { get; set; }
        public string Customer { get; set; }
        public Nullable<bool> HasValuation { get; set; }
    }
}