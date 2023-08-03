using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class Quote
    {
        public IEnumerable<OrderLine> quotationlines { set; get; }
        public Order quote { set; get; }
        public Customer customer { set; get; }
        public Region  region { set; get; }
        public Suburb  suburb { set; get; }
        public CollectionPoint  collectxn { set; get; }
        public DeliveryAddress Address { set; get; }
       
       
    }
}