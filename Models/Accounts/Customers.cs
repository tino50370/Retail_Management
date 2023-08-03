using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace RetailKing.Models
{
    public partial class Customer
    {
        public IEnumerable<Account> categories { set; get; }
        public IEnumerable<Customer> level1 { set; get; }
        public IEnumerable<Customer> level2 { set; get; }
        public IEnumerable<Customer> level3 { set; get; }
        public IEnumerable<NetworkType> networktyp { set; get; }
        public IEnumerable<Item> slides { set; get; }
        public IEnumerable<DeliveryAddress> deliveries { set; get; }
        public NetworkSale  totals { set; get; }
        public string code { set; get; }
        [Required]
        [Display(Name = "Date")]
        public int Date { get; set; }
        [Required]
        [Display(Name = "Month")]
        public int month { get; set; }
        [Required]
        [Display(Name = "Year")]
        public int year { get; set; }
    }
}