using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public partial class Account
    {
        
        public string Level1 { set; get; }
        public string Level2 { set; get; }
        public string Level3 { set; get; }
        public bool Selection { set; get; }
        public long CategoryId { set; get; }
        
    }
}