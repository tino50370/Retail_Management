using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;

namespace RetailKing.Models
{
    public class HomeData
    {
        public IList<Item> items { set; get; }
        public IList<Account> Categories { set; get; }
        public IList<Partner> Partners { set; get; }
        public IList<Storekey> storekeys { set; get; }
       
    }
}