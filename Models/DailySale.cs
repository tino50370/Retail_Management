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
    
    public partial class DailySale
    {
        public string Id { get; set; }
        public string Date { get; set; }
        public Nullable<decimal> Sales { get; set; }
        public Nullable<long> UniqueVisits { get; set; }
        public Nullable<long> Hits { get; set; }
        public Nullable<long> CompanyId { get; set; }
        public string AccountName { get; set; }
        public Nullable<System.DateTime> TransactionDate { get; set; }
    }
}