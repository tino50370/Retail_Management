using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class Posdata
    {
        public Items item { set; get; }
        public IEnumerable<Menu> menu { set; get; }  
    }
}