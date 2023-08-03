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
    public partial class Expenses
    {

        private System.Int64 _Id;
        private System.Nullable<System.Decimal> _Amount;
        private System.Nullable<System.Decimal> _value;
        private System.Nullable<System.Decimal> _Balance;
        private System.String _AccountCode;
        private System.String _AccountName;
        private System.String _Description;
        private System.String _Dated;
          private System.String _Payee;
        private System.String _Receipt;
        private System.String _type;
         private System.String _cashier;
         private System.String _till;
       
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

        public virtual System.Nullable<System.Decimal> Amount
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
        public virtual System.Nullable<System.Decimal> value
        {
            get
            {
                return _value;
            }
            set
            {
                _value = value;
            }
        }
        public virtual System.Nullable<System.Decimal> Balance
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
        public virtual System.String AccountCode
        {
            get
            {
                return _AccountCode;
            }
            set
            {
                _AccountCode = value;
            }
        }

        public virtual System.String Description
        {
            get
            {
                return _Description;
            }
            set
            {
                _Description = value;
            }
        }

        public virtual System.String AccountName
        {
            get
            {
                return _AccountName;
            }
            set
            {
                _AccountName = value;
            }
        }

        public virtual System.String Dated
        {
            get
            {
                return _Dated;
            }
            set
            {
                _Dated = value;
            }
        }

        public virtual System.String Payee
        {
            get
            {
                return _Payee;
            }
            set
            {
                _Payee = value;
            }
        }

        public virtual System.String Receipt
        {
            get
            {
                return _Receipt;
            }
            set
            {
                _Receipt = value;
            }
        }

        public virtual System.String cashier
        {
            get
            {
                return _cashier;
            }
            set
            {
                _cashier = value;
            }
        }

        public virtual System.String type
        {
            get
            {
                return _type;
            }
            set
            {
                _type = value;
            }
        }

        public virtual System.String till
        {
            get
            {
                return _till;
            }
            set
            {
                _till = value;
            }
        }

      

    }
}
