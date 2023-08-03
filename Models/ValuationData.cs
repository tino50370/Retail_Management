using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;

namespace RetailKing.Models
{
    public  class ValuationData
    {
        public IEnumerable<RatingZone> zones { set; get; }
        public IEnumerable<ValuationDetail> valuationdetails { set; get; }
        public Valuation valuation { set; get; }
       
    }
}