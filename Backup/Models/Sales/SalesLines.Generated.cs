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
    public partial class SalesLines
    {

        private System.Int64 _Id;
        private System.String _Reciept;
        private System.String _item;
        private System.Int64 _quantity;
        private Nullable<System.Decimal> _price;
        private Nullable<System.Decimal> _tax;
        private Nullable<System.Decimal> _priceinc;
        
  

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

        public virtual System.Int64 quantity
        {
            get
            {
                return _quantity;
            }
            set
            {
                _quantity = value;
            }
        }

        public virtual System.String Reciept
        {
            get
            {
                return _Reciept;
            }
            set
            {
                _Reciept = value;
            }
        }

        public virtual System.String item
        {
            get
            {
                return _item;
            }
            set
            {
                _item = value;
            }
        }

        public virtual Nullable<System.Decimal> price
        {
            get
            {
                return _price;
            }
            set
            {
                _price = value;
            }
        }

        public virtual Nullable<System.Decimal> tax
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

        public virtual Nullable<System.Decimal> priceinc
        {
            get
            {
                return _priceinc;
            }
            set
            {
                _priceinc = value;
            }
        }

  

    }
}
