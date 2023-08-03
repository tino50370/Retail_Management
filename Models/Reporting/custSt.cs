using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class custSt
    {
        public IEnumerable<Expens> Expenses { set; get; }
        public Customer cust { set; get; }
    }
}