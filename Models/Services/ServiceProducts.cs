using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class ServiceProducts
    {
        public long Id	{ set; get; }
        public string Name { set; get; }
        public long ServiceId { set; get; }
        public long Points { set; get; }
        public long Quantity { set; get; }
        public decimal Price { set; get; }
        public bool Promotion { set; get; }
        public decimal SuspenseWindow { set; get; }
        public string ChargeType { set; get; }
        public decimal ChargeValue { set; get; }
        public string CommisionChargeType { set; get; }
        public decimal CommisionValue { set; get; }
        public decimal ChargeLimit { set; get; }
        public long CommisionLimit { set; get; }
        public long ServiceType { set; get; }
        public string ServiceProvider { set; get; }
        public string Image { set; get; }
        public decimal Tax { set; get; }
        public decimal Taxed { set; get; }
        public decimal PromotionValue { get; set; }
        public long PromotionType { get; set; }

    }
}