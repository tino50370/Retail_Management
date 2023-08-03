using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class Loyalty
    {
        public String Id { set; get; }
        public String Name { set; get; }
        public String ContactPerson { set; get; }
        public String SupplierId { set; get; }
        public String SupplierName { set; get; }
        public String Branch { set; get; }
        public Int64 SchemeId { set; get; }
        public int Points { set; get; }
        public Decimal MonetaryValue { set; get; }
        public String DOB { set; get; }
        public String Email { set; get; }
        public String CardNumber { set; get; }
        public String PhoneNumber { set; get; }
        public DateTime LastAccess { set; get; }

    }
}