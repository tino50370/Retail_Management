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
    
    public partial class shift
    {
        public long ID { get; set; }
        public Nullable<System.DateTime> dated { get; set; }
        public Nullable<long> ShiftNumber { get; set; }
        public string Manager { get; set; }
        public string State { get; set; }
    }
}