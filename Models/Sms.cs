using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;

namespace RetailKing.Models
{
    public class Sms
    {
        public string MSISDN { set; get; }
        public string Message { set; get; }
        public string Sender { set; get; }
        public string Service { set; get; }
        public long ServiceId { set; get; }
        public string Filename { set; get; }
       
    }
}