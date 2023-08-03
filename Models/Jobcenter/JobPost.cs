using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class JobPost
    {
        public string Id { set; get; }
        public string CustomerId { set; get; }
        public string CustomerName { set; get; }
        public string Title { set; get; }
        public string Category { set; get; }
        public string Tags { set; get; }
        public string Description { set; get; }
        public Decimal Budget { set; get; }
        public Decimal BidCount { set; get; }
        public string Status { set; get; }
        public DateTime DateCreated  { set; get; }
        public DateTime DateClosed { set; get; }
        public IList<JobBid> Jobbids { set; get; }

    }
}