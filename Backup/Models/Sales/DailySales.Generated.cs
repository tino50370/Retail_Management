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
    public partial class DailySales
    {

        private System.String _Id;
        private System.String _Date;
        private System.Decimal _Sales;
 
       
        public virtual System.String Id
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

        public virtual System.String Date
        {
            get
            {
                return _Date;
            }
            set
            {
                _Date = value;
            }
        }

        public virtual System.Decimal Sales
        {
            get
            {
                return _Sales;
            }
            set
            {
                _Sales = value;
            }
        }


    }
}
