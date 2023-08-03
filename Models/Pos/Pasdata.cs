using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public class Posdata
    {
        public Item item { set; get; }
        public IEnumerable<PosMenu> menu { set; get; }
        public login uzar { set; get; }
        public IEnumerable<Currency> curr { set; get; }
    }
}