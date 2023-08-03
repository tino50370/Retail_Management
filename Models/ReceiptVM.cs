using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;

namespace RetailKing.Models
{
    public class ReceiptVM
    {
        public string total { get;  set; }
        public string tax { get;  set; }
        public string subTotal { get;  set; }
        public string tender { get;  set; }
        public decimal creditBalance { get; set; }
        public string change { get;  set; }
        public string cashier { get;  set; }
        public string Receipt { get;  set; }
        public string diliveryId { get;  set; }
        public string diliveryAddress { get;  set; }
        public string CustomerName { get;  set; }
        public string CustomerMobile { get;  set; }
        public string ToCollect { get;  set; }
        public string CollectionDate { get; set; }
        public bool isCopy { get; set; }
        public IEnumerable<Printdata> posd { get;  set; }
        public Company company { get;  set; }
      
    }
}