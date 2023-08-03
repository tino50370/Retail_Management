using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public partial class Item
    {
        public IEnumerable<Company> companies { set; get; }
        public IEnumerable<Item> RelatedItems { set; get; }
        public IEnumerable<Item> SideSpecials { set; get; }
        public IEnumerable<Feature> features { set; get; }
        public IEnumerable<Account> categories { set; get; }
        public IEnumerable<Account> SubCategories { set; get; }
        public IEnumerable<ItemImage> ItemImages { set; get; }
        public IEnumerable<ItemVariation> ItemVariations { set; get; }
        public IEnumerable<PurchaseLine> Availability { set; get; }
        public IEnumerable<Recipe> RecipeItems { set; get; }
    }
}