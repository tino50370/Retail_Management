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
        public class MobileMenu
        {
            public string Id { set; get; }
            public string Image { set; get; }
            public string Title { set; get; }
            public string Description { set; get; }
            public string Section { set; get; }
            public long ServiceId { set; get; }
            public long TransactionType { set; get; }
            public string SupplierId { set; get; }
            public int Count { set; get; }
            public DateTime date { set; get; }
            public bool HasProducts { set; get; }
        public string Name { get; set; }
        }
   
    }

  
