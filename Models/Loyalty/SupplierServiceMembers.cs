using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class SupplierServiceMembers
    {
        public String Id { set; get; }
        public IList<CustomerService> members { set; get; }

    }
}