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
    
    public partial class CustomerCode
    {
        public long Id { get; set; }
        public Nullable<long> CustomerId { get; set; }
        public Nullable<long> Size { get; set; }
        public Nullable<System.DateTime> PurchaseDate { get; set; }
        public Nullable<System.DateTime> Expirery { get; set; }
        public string Code { get; set; }
    }
}
