using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;

namespace RetailKing.ViewModels
{
    public class HomeVM
    {
        public HomeVM(IEnumerable<Item> Jack, DailySale dsales,MonthlySale msales,Activeco comp,IEnumerable<Activeco> c, login uzza)
        {
            this.items = Jack;
            this.dailysales = dsales;
            this.monthlysale = msales;
            this.activeco = comp;
            this.companies = c;
            this.uzar = uzza;
        }

        public IEnumerable<Item> items { get; private set; }
        public DailySale dailysales { get; private set; }
        public MonthlySale monthlysale { get; private set; }
        public Activeco activeco { get; private set; }
        public login uzar { get; private set; }
        public IEnumerable<Activeco> companies { get; private set; }
    }
}