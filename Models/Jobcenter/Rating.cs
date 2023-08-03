using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class Rating
    {
        public String CustomerId { set; get; }
        public String Name { set; get; }
        public Int32 RateScore { set; get; }
        public String Description { set; get; }

    }
}