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
    public partial class Discounts
    {

        private System.Int64 _Id;
        private System.Nullable<System.Decimal> _Amount;
        private System.String _Invoice;
        private System.String _Receipt;
        private System.String _users;
        private System.String _Dated;

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

        public virtual System.String users
        {
            get
            {
                return _users;
            }
            set
            {
                _users = value;
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

   
      

    }
}
