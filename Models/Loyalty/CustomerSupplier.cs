using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class CustomerSupplier
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public string logo { set; get; } 
        public IList<CustomerService> services { set; get; }
    }
}