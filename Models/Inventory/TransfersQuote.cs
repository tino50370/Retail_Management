using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class TransfersQuote
    {
        public IEnumerable<TransferingLine> quotationlines { set; get; }
        public Transfer quote { set; get; }
        public login uzar { get;  set; }
       /* public Customer customer { set; get; }
        public Region  region { set; get; }
        public Suburb  suburb { set; get; }
        public CollectionPoint  collectxn { set; get; }
        public DeliveryAddress Address { set; get; }
       */
       
    }
}