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
    
    public partial class Expens
    {
        public long ID { get; set; }
        public string AccountCode { get; set; }
        public string AccountName { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> value { get; set; }
        public Nullable<decimal> Amount { get; set; }
        public string Payee { get; set; }
        public Nullable<System.DateTime> Dated { get; set; }
        public string Receipt { get; set; }
        public Nullable<decimal> Balance { get; set; }
        public string type { get; set; }
        public string cashier { get; set; }
        public string till { get; set; }
        public string Invoice { get; set; }
    }
}
