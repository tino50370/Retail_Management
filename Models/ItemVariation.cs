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
    
    public partial class ItemVariation
    {
        public long Id { get; set; }
        public string ItemCode { get; set; }
        public string Description { get; set; }
        public Nullable<decimal> Quantity { get; set; }
        public string VariationType { get; set; }
    }
}