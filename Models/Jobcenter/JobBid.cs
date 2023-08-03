using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class JobBid
    {
        public long Id { set; get; }
        public String CustomerId { set; get; }
        public String JobPostId { set; get; }
        public String RequesterId { set; get; }
        public String Description { set; get; }
        public Decimal Amount { set; get; }

    }
}