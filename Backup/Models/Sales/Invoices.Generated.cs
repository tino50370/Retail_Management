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
    public partial class Invoices
    {

        private System.Int64 _Id;
        private System.String _company;
        private System.String _dated;
        private System.String _customer;
        private System.String _Account;
        private System.String _Reciept;
        private System.String _Invoice;
        private System.String _shift;
        private System.String _state;
        private Nullable<System.Decimal> _total;
        private Nullable<System.Decimal> _discount;
          

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

        public virtual Nullable<System.Decimal> total
        {
            get
            {
                return _total;
            }
            set
            {
                _total = value;
            }
        }

        public virtual Nullable<System.Decimal> discount
        {
            get
            {
                return _discount;
            }
            set
            {
                _discount = value;
            }
        }

        public virtual System.String dated
        {
            get
            {
                return _dated;
            }
            set
            {
                _dated = value;
            }
        }

        public virtual System.String customer
        {
            get
            {
                return _customer;
            }
            set
            {
                _customer = value;
            }
        }

        public virtual System.String Account
        {
            get
            {
                return _Account;
            }
            set
            {
                _Account = value;
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

        public virtual System.String Invoice
        {
            get
            {
                return _Invoice;
            }
            set
            {
                _Invoice = value;
            }
        }

        public virtual System.String shift
        {
            get
            {
                return _shift;
            }
            set
            {
                _shift = value;
            }
        }

        public virtual System.String state
        {
            get
            {
                return _state;
            }
            set
            {
                _state = value;
            }
        }
      

    }
}
