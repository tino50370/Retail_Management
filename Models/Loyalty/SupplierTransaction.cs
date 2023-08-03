using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class SupplierTransaction
    {
        public String Id { set; get; }
        public IList<Transaction> transactions { set; get; }

    }
}