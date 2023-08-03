using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class ServiceAction
    {
        public long Id	{ set; get; }
        public long ServiceId { set; get; }
        public string SupplierId { set; get; }
        public string Name { set; get; }
        public string Description	{ set; get; }
        public long  TransactionType { set; get; }
        public long ServiceActionId { set; get; }
        public string ChargeFrequency { set; get; }
        public decimal SuspenseWindow	{ set; get; }
        public string ChargeType { set; get; }
        public decimal ChargeValue	{ set; get; }
        public string CommisionChargeType { set; get; }
        public decimal CommisionValue	{ set; get; }
        public decimal ChargeLimit	{ set; get; }
        public decimal CommisionLimit	{ set; get; }
        public string  ServiceType	{ set; get; }
        public string  AccountSufix	{ set; get; }
        public string  ServiceProvider	{ set; get; }
        public string  ProductName	{ set; get; }
        public string  Image { set; get; }
        public string  VerifyAccount { set; get; }
        public string  AmountRequiredIn	{ set; get; }
        public decimal MaxSale	{ set; get; }
        public decimal MinSale { set; get; }
        public string Status { set; get; }
        public bool  HasProducts { set; get; }
        public bool IsExternal { set; get; }
        public string ExternalServiceId { set; get; }

    }
}