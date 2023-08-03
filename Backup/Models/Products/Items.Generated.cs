using System;
using System.Collections.Generic;
using System.ComponentModel;
using Iesi.Collections;
using Iesi.Collections.Generic;
using RetailKing.Models.Components;
using RetailKing.Models.Exceptions;
using RetailKing.Models.Enumerations;
using RetailKing.Models;



namespace RetailKing.Models
{
    /// <summary>
    /// An object representation of the Account table
    /// </summary>
    [Serializable]
    public partial class Items
    {

        private System.Int64 _Id;
        private System.String _Item;
        private System.String _company;
        private System.String _category;
        private System.String _SubCategory;
        private System.String _status;
        private System.String _ItemCode;
        private System.String _tax;
        private System.String _TopSeller;
        private System.String _SDate;
        private System.String _EDate;
        private Nullable<System.Decimal> _BuyingPrice;
        private Nullable<System.Decimal> _SellingPrice;
        private Nullable<System.Decimal> _Amount;
        private Nullable<System.Decimal> _Expected;
        private System.Int64 _Quantity;
        private System.Int64 _sold;
        private System.Int64 _transfer;
        private System.Int64 _Balance;
        private System.Int64 _NewStock;
        private System.Int64 _Returned;
        private System.Int64 _Swaps;
        private System.Int64 _Reorder;
        private IEnumerable<Companies> _companies;
        private IEnumerable<Category> _categories;

        public virtual IEnumerable<Companies> companies
        {
            get
            {
                return _companies;
            }
            set
            {
                _companies = value;
            }
        }

        public virtual IEnumerable<Category> categories
        {
            get
            {
                return _categories;
            }
            set
            {
                _categories = value;
            }
        }

        public virtual System.Int64 Id
        {
            get
            {
                return _Id;
            }
            set
            {
                _Id = value;
            }
        }

        public virtual System.Int64 Quantity
        {
            get
            {
                return _Quantity;
            }
            set
            {
                _Quantity = value;
            }
        }

        public virtual System.Int64 sold
        {
            get
            {
                return _sold;
            }
            set
            {
                _sold = value;
            }
        }
        public virtual System.Int64 transfer
        {
            get
            {
                return _transfer;
            }
            set
            {
                _transfer = value;
            }
        }
        public virtual System.Int64 Balance
        {
            get
            {
                return _Balance;
            }
            set
            {
                _Balance = value;
            }
        }
        public virtual System.Int64 NewStock
        {
            get
            {
                return _NewStock;
            }
            set
            {
                _NewStock = value;
            }
        }
        public virtual System.Int64 Returned
        {
            get
            {
                return _Returned;
            }
            set
            {
                _Returned = value;
            }
        }
        public virtual System.Int64 Swaps
        {
            get
            {
                return _Swaps;
            }
            set
            {
                _Swaps = value;
            }
        }
        public virtual System.Int64 Reorder
        {
            get
            {
                return _Reorder;
            }
            set
            {
                _Reorder = value;
            }
        }

        public virtual System.String company
        {
            get
            {
                return _company;
            }
            set
            {
                _company = value;
            }
        }
        public virtual System.String SubCategory
        {
            get
            {
                return _SubCategory;
            }
            set
            {
                _SubCategory = value;
            }
        }

        public virtual Nullable<System.Decimal> BuyingPrice
        {
            get
            {
                return _BuyingPrice;
            }
            set
            {
                _BuyingPrice = value;
            }
        }

        public virtual Nullable<System.Decimal> SellingPrice
        {
            get
            {
                return _SellingPrice;
            }
            set
            {
                _SellingPrice = value;
            }
        }

        public virtual Nullable<System.Decimal> Amount
        {
            get
            {
                return _Amount;
            }
            set
            {
                _Amount = value;
            }
        }

        public virtual Nullable<System.Decimal> Expected
        {
            get
            {
                return _Expected;
            }
            set
            {
                _Expected = value;
            }
        }

        public virtual System.String Item
        {
            get
            {
                return _Item;
            }
            set
            {
                _Item = value;
            }
        }

        public virtual System.String category
        {
            get
            {
                return _category;
            }
            set
            {
                _category = value;
            }
        }

        public virtual System.String status
        {
            get
            {
                return _status;
            }
            set
            {
                _status = value;
            }
        }

        public virtual System.String ItemCode
        {
            get
            {
                return _ItemCode;
            }
            set
            {
                _ItemCode = value;
            }
        }

        public virtual System.String tax
        {
            get
            {
                return _tax;
            }
            set
            {
                _tax = value;
            }
        }

        public virtual System.String TopSeller
        {
            get
            {
                return _TopSeller;
            }
            set
            {
                _TopSeller = value;
            }
        }

        public virtual System.String SDate
        {
            get
            {
                return _SDate;
            }
            set
            {
                _SDate = value;
            }
        }

        public virtual System.String EDate
        {
            get
            {
                return _EDate;
            }
            set
            {
                _EDate = value;
            }
        }    

    }
}
