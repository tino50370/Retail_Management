using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;

namespace RetailKing.ViewModels
{
    public class ReceiptVMold
    {
        public ReceiptVMold(IEnumerable<Printdata> pd, Company  co)
        {
            this.posd = pd;
            this.company = co;
          
        }

        public IEnumerable<Printdata> posd { get; private set; }
        public Company company { get; private set; }
      
    }
}