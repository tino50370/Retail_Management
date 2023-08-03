using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;

namespace RetailKing.ViewModels
{
    public class ReceiptVM
    {
        public ReceiptVM(IEnumerable<Printdata> pd, Companies  co)
        {
            this.posd = pd;
            this.company = co;
          
        }

        public IEnumerable<Printdata> posd { get; private set; }
        public Companies company { get; private set; }
      
    }
}