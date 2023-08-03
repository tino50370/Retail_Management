using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class ShoppingCart
    {
        public IEnumerable<OrderLine > orderlines { set; get; }
        public IEnumerable<Account> categories { set; get; }
        public Order order { set; get; }
        public Customer customer { set; get; }
        public Region  region { set; get; }
        public Suburb  suburb { set; get; }
        public CollectionPoint  collectxn { set; get; }
        public DeliveryAddress Address { set; get; }
       


    }
}