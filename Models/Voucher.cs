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
    
    public partial class Voucher
    {
        public long Id { get; set; }
        public string SerialNumber { get; set; }
        public string BatchNumber { get; set; }
        public Nullable<decimal> Denomination { get; set; }
        public string ExpireryDate { get; set; }
        public Nullable<long> VID { get; set; }
        public string Status { get; set; }
        public string Dated { get; set; }
        public string Product { get; set; }
        public string Supplier { get; set; }
        public string Manufacturer { get; set; }
        public string VoucherKey { get; set; }
        public string Destination { get; set; }
    }
}