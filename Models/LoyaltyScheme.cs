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
    
    public partial class LoyaltyScheme
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string SupplierId { get; set; }
        public Nullable<decimal> PointValue { get; set; }
        public Nullable<System.DateTime> ExpireryDate { get; set; }
        public Nullable<System.DateTime> DateCreated { get; set; }
        public Nullable<System.DateTime> SubExpireryDate { get; set; }
        public Nullable<int> Balance { get; set; }
        public Nullable<decimal> PointCost { get; set; }
        public Nullable<int> Tier { get; set; }
        public Nullable<decimal> AccessCost { get; set; }
        public string PaymentPeriod { get; set; }
        public string PaymentType { get; set; }
    }
}