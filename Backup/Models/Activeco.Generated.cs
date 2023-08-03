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
    public partial class Activeco
    {

        private System.Int64 _Id;
       
        private System.String _company;
        private System.Decimal _monthlyTarget;
        private System.Decimal _dailyTarget;
        
       
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

        public virtual System.Decimal monthlyTarget
        {
            get
            {
                return _monthlyTarget;
            }
            set
            {
                _monthlyTarget = value;
            }
        }

        public virtual System.Decimal dailyTarget
        {
            get
            {
                return _dailyTarget;
            }
            set
            {
                _dailyTarget = value;
            }
        }

    }
}
