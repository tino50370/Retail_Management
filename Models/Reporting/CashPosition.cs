using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class CashPosition
    {
        public IEnumerable<Sale> salez { set; get; }
        public IEnumerable<Supplier> suppliers { set; get; }
        public IEnumerable<Customer> customers { set; get; }
        public IEnumerable<Account> accounts { set; get; }
        public IEnumerable<Expens> expenses { set; get; } 
    }
}