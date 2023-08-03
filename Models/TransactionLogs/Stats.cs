using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class Stats
    {
        public String Id { set; get; }
        public String SupplierId { set; get; }
        public String ServiceProvider { set; get; }
        public String BranchId { set; get; }
        public String ServiceName { set; get; }
        public long Successful { set; get; } 
        public long Failed { set; get; }
        public DateTime Date { set; get; }
        public long ServiceId { set; get; }
        public decimal Amount { set; get; }

    }
}