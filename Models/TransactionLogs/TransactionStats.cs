using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class TransactionStats
    {
        public string Id { set; get; }
        public IList<Stats> stats { set; get; }
    }
}