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
    public partial class MonthlySales
    {

        private System.String _Id;  
        private System.Int64 _Year;
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

        public virtual System.Int64 Year
        {
            get
            {
                return _Year;
            }
            set
            {
                _Year = value;
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
