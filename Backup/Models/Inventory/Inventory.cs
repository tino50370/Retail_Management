using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class Inventory
    {
        public IEnumerable<Items> items { set; get; }
        public IEnumerable<Category> categories { set; get; }
       
    }
}