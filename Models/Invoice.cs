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
    
    public partial class Invoice
    {
        public long ID { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> discount { get; set; }
        public string company { get; set; }
        public Nullable<System.DateTime> dated { get; set; }
        public string customer { get; set; }
        public string Account { get; set; }
        public string Reciept { get; set; }
        public string Invoice1 { get; set; }
        public string shift { get; set; }
        public string state { get; set; }
        public string Period { get; set; }
    }
}
