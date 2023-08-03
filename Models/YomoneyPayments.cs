using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RetailKing.Models;
using RetailKing.DataAcess;
using System.IO;
using System.Text.RegularExpressions;
using System.Web.Helpers;
using System.Windows.Media.Imaging;
using RetailKing.Controllers;

namespace RetailKing.Models
{
    public class YomoneyPayments
    {
        public YomoneyResponse YomoneyRequest(string name, string Amount,string MTI)
        {
            YomoneyRequest req = new YomoneyRequest();
            YomoneyController pc = new YomoneyController();
           // req.CustomerMSISDN = CustomerMSISDN;
            //req.ServiceId = Convert.ToDouble(ServiceId);
            req.Amount = Convert.ToDecimal(Amount);
            req.MTI = MTI;
            //req.TransactionType = TransactionType;
            //req.CustomerAccount = CustomerAccount;
            //req.Quantity = Quantity;
            //req.ProcessingCode = ProcessingCode;
           // var voucher;
            return  pc.GetExternal(req);
        }
    }
}