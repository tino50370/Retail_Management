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
    
    public partial class Return
    {
        public long ID { get; set; }
        public Nullable<decimal> total { get; set; }
        public Nullable<decimal> discount { get; set; }
        public string company { get; set; }
        public string dated { get; set; }
        public string customer { get; set; }
        public string Reciept { get; set; }
        public string Invoice { get; set; }
        public string shift { get; set; }
        public string Cashier { get; set; }
        public string Authorised { get; set; }
    }
}
