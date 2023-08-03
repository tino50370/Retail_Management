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
    public partial class Shift
    {

        private System.Int64 _Id;
        private System.Int64 _ShiftNumber;
        private System.String _Manager;
        private System.String _State;
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

        public virtual System.Int64  ShiftNumber
        {
            get
            {
                return _ShiftNumber;
            }
            set
            {
                _ShiftNumber = value;
            }
        }

        public virtual System.String Manager
        {
            get
            {
                return _Manager;
            }
            set
            {
                _Manager = value;
            }
        }

        public virtual System.String State
        {
            get
            {
                return _State;
            }
            set
            {
                _State = value;
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
