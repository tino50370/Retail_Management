using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class TransactionLog
    {
        public string Id { set; get; }
        public IList<Tranlog> logs { set; get; }
    }
}