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
        public class Upload
        {
            [Required]
            [Display(Name = "Supplier")]
            public string Supplier { get; set; }
            [Required]
            [Display(Name = "Product")]
            public string Product { get; set; }
            [Display(Name = "Manufacturer")]
            public string Manufacturer { get; set; }
            public IList<Voucher> duplicates { get; set; }

        }

     
    }

  
