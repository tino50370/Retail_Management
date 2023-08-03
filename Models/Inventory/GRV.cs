using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class GRV
    {
        
        public IEnumerable<PurchaseLine> purchaseLines { set; get; }
        public Purchase grvDetails { set; get; }
        public Supplier supplierDetails { set; get; }
        public string  Company { set; get; }
        public string  Receiver { set; get; }
        public DateTime Date { set; get; }
        public Transfer grvTransfer { set; get; }
        public Company companyDetails { set; get; }

 
    }
}