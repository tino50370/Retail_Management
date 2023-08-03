using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class LossQuote
    {
        public IEnumerable<LossLine> quotationlines { set; get; }
        public Loss quote { set; get; }
        public login uzar { set; get; }

    }
}