using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class CustomerMessages
    {
        public String Id { set; get; }
        public IList<Message> messages { set; get; }

    }
}