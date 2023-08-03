using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class Reward
    {
        public long Id { set; get; }
        public long SchemeId { set; get; }
        public string Name { set; get; }
        public int Points { set; get; }
        public decimal MonetaryValue { set; get; }
        public string Branch { set; get; }
        public string SupplierId { set; get; }
        public string SupplerName { set; get; }
        public string RewardType { set; get; }
      
    }
}