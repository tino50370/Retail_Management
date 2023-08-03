using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class Inventory
    {
        public IEnumerable<Item> items { set; get; }
        public IEnumerable<Account> categories { set; get; }
        public IEnumerable<Account> SubCategories { set; get; }
       
    }
}