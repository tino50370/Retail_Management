using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class SupplierProfile
    {
        public string Id { set; get; }
        public int ServiceCount { set; get; }
        public string Name { set; get; }
        public string ServiceType { set; get; }
        public string Description { set; get; }
        public string Website { set; get; }
        public string Email { set; get; }
        public string Phone { set; get; }
        public string Logo { set; get; }
        public string DateCreated { set; get; }
        public IList<SupplierService>  Services { set; get; }    
    }
}