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
        public class Wallet
        {
            public string Id { get; set; }
            public string  Account { get; set; }
            public string PostFix { get; set; }
            public decimal Balance { get; set; }
            public decimal Commission { get; set; }
            public decimal Suspended { get; set; }

           
        }
   
    }

  
