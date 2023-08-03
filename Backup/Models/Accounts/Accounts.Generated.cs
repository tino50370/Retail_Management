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
    public partial class Accounts
    {

        private System.Int64 _Id;
        private System.String _AccountCode;
        private System.String _LinkAccount;
        private System.String _AccountName;
        private Nullable<System.Decimal> _Balance;
        private Nullable<System.Decimal> _Opening;


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

        public virtual Nullable<System.Decimal> Balance
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

        public virtual Nullable<System.Decimal> Opening
        {
            get
            {
                return _Opening;
            }
            set
            {
                _Opening = value;
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

        public virtual System.String LinkAccount
        {
            get
            {
                return _LinkAccount;
            }
            set
            {
                _LinkAccount = value;
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

    }
}
