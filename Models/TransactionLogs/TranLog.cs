using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class Tranlog
    {
        public String Id { set; get; }
        public Decimal Amount { set; get; }
        public Decimal TransactionCharge { set; get; } 
        public Decimal TransactionTotal	{ set; get; }
        public DateTime Date { set; get; }
        public string SourceAccountNumber { set; get; }
        public string ParentId { set; get; }
        public string DestinationAccountNumber	{ set; get; }
        public long TransactionReferenceNumber	{ set; get; }
        public string Description	{ set; get; }
        public long TransactionType { set; get; }
        public string ReceiptNumber	{ set; get; }
        public DateTime DateCreated	{ set; get; }
        public string ServiceId	{ set; get; }
        public decimal Commision	{ set; get; }
        public string Status	{ set; get; }
        public string Miscellaneous { set; get; }
        public string TerminalId { set; get; }
        public string Username { set; get; }
        public string BranchName { set; get; }

    }
}