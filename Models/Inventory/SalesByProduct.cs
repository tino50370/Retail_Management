using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class SalesByProduct
    {
        public IEnumerable<Sales_Lines> Saleslines { set; get; }
        public Sale sale { set; get; }
       // public login uzar { set; get; }

    }
}