using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;


namespace RetailKing.Models
    {
        [Serializable]
        public class YomoneyAccount
        {
           
            public string Id { get; set; }
            public long AccountType { get; set; }
            public long ServiceType { get; set; }
		    public string AccountName { get; set; }
		    public string FirstName { get; set; }
		    public string Surname { get; set; }
		    public DateTime DateCreated { get; set; }
		    public DateTime DateLastAccess { get; set; }
		    public string LastAccessFrom { get; set; }
		    public string Email { get; set; }
            public string IsService { get; set; }
		    public decimal ActualBalance { get; set; }
		    public decimal SuspenseBalance { get; set; }
		    public decimal AvailableBalance { get; set; }
            public decimal CommissionWallet { get; set; }
            public string Agent1 { get; set; }
            public string ConnectionId { get; set; }
            public string Address { get; set; }
            public string Country { get; set; }
            public string City { get; set; }
            public string Std { get; set; }
            public string MerchantPoints { get; set; }
            public long ServiceLoyalty { get; set; }
            public long TerminalId { get; set; }
       

           
        }
   
    }

  
