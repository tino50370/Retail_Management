using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class PurchaseOrderz
    {
        
        public IEnumerable<PurchaseOrderLine> purchaseLines { set; get; }
        public PurchaseOrder grvDetails { set; get; }
        public Supplier supplierDetails { set; get; }
        public string  Company { set; get; }
        public string  Receiver { set; get; }
        public DateTime Date { set; get; }
 
    }
}