using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;

namespace RetailKing.ViewModels
{
    public class HomeVM
    {
        public HomeVM(IEnumerable<Items> Jack, DailySales dsales,MonthlySales msales,Activeco comp,IEnumerable<Activeco> c)
        {
            this.items = Jack;
            this.dailysales = dsales;
            this.monthlysale = msales;
            this.activeco = comp;
            this.companies = c;
        }

        public IEnumerable<Items> items { get; private set; }
        public DailySales dailysales { get; private set; }
        public MonthlySales monthlysale { get; private set; }
        public Activeco activeco { get; private set; }
        public IEnumerable<Activeco> companies { get; private set; }
    }
}