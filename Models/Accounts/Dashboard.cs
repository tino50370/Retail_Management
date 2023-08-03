using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public partial class Dashboard
    {
        
        public Account account { set; get; }
        public int Level1Count { set; get; }
        public int Level2Count{ set; get; }
        public int Level3Count { set; get; } 
        
    }
}