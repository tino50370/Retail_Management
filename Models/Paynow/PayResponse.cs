using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class PayResponse
    {

        public String reference { set; get; }
        public String Paynowreference { set; get; }
        public String amount { set; get; }
        public String status { set; get; }
        public String pollurl { set; get; }
        public String Hash { set; get; }
      
       
       
    }
}