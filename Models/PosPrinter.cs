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
    
    public partial class PosPrinter
    {
        public long Id { get; set; }
        public string Name { get; set; }
        public string IpAddress { get; set; }
        public string ConnectionId { get; set; }
        public string Type { get; set; }
        public string Location { get; set; }
        public Nullable<System.DateTime> DateLastAccess { get; set; }
    }
}
