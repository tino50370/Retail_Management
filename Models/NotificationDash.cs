using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;

namespace RetailKing.Models
{
    public class NotificationDash
    {
        public IList<MailingService> services { set; get; }
        public IList<Stats> stats { set; get; }
   
    }
}