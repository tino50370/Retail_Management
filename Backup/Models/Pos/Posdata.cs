using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class Printdata
    {
        public string item { set; get; }
        public string qty { set; get; }
        public string amount { set; get; } 
        public string Discount { set; get; }
        public string prize { set; get; }
        public string unit { set; get; }
        public Companies company { set; get; }
       
    }
}