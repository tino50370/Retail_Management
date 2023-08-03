using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

using System.Web.Security;
using System.ComponentModel.DataAnnotations;
using System.Data.Entity;


    namespace Yomoney.Models
    {
        public class Topup
        {
            public string Id { get; set; }
            public string  Account { get; set; }
            public string PostFix { get; set; }
            public decimal Amount { get; set; }
            public string  Password { get; set; }

        }
   
    }

  
