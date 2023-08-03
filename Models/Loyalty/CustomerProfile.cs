using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class CustomerProfile
    {
        public string Id { set; get; }
        public string MobileNumber { set; get; }
        public string Name { set; get; }
        public string Image { set; get; }
        public DateTime DOB { set; get; }
        public string  ConnectionId { set; get; }
        public DateTime DateLastAccessed { set; get; }
        public IList<Loyalty> LoyaltySchemes { set; get; }
        public IList<CustomerSupplier> suppliers { set; get; }
        
    }
}