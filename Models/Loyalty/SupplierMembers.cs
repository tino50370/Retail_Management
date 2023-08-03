using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class SupplierMembers
    {
        public String Id { set; get; }
        public IList<Loyalty> members { set; get; }

    }
}