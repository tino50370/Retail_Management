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
    
    public partial class InvoiceLine
    {
        public long ID { get; set; }
        public string Reciept { get; set; }
        public string item { get; set; }
        public Nullable<long> quantity { get; set; }
        public Nullable<decimal> price { get; set; }
        public Nullable<decimal> tax { get; set; }
        public Nullable<decimal> priceinc { get; set; }
    }
}
