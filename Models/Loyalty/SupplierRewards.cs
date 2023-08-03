using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class SupplierRewards
    {
        public String Id { set; get; }
        public IList<Reward> rewards { set; get; }

    }
}