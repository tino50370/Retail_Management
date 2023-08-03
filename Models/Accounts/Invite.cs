using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public partial class Invite
    {
        public string sender { set; get; }
        public string senderEmail { set; get; }
        public IEnumerable<Account> categories { set; get; }

    }
}