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
    public partial class Companies
    {

        private System.Int64 _Id;
        private System.String _location;
        private System.String _status;
        private System.String _name;
        private System.String _dated;
        private System.String _Address1;
        private System.String _Address2;
        private System.String _Contact1;
        private System.String _Contact2;
        private System.String _Footer1;
        private System.String _Footer2;
        private System.String _Footer3;
        private System.String _footer4;
        private System.String _Logo;
        private System.String _VatNo;

       
   
   
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

        public virtual System.String location
        {
            get
            {
                return _location;
            }
            set
            {
                _location = value;
            }
        }

        public virtual System.String name
        {
            get
            {
                return _name;
            }
            set
            {
                _name = value;
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

        public virtual System.String Address1
        {
            get
            {
                return _Address1;
            }
            set
            {
                _Address1 = value;
            }
        }
        public virtual System.String Address2
        {
            get
            {
                return _Address2;
            }
            set
            {
                _Address2 = value;
            }
        }
        public virtual System.String Contact1
        {
            get
            {
                return _Contact1;
            }
            set
            {
                _Contact1 = value;
            }
        }
        public virtual System.String Contact2
        {
            get
            {
                return _Contact2;
            }
            set
            {
                _Contact2 = value;
            }
        }
        public virtual System.String Footer1
        {
            get
            {
                return _Footer1;
            }
            set
            {
                _Footer1 = value;
            }
        }
        public virtual System.String Footer2
        {
            get
            {
                return _Footer2;
            }
            set
            {
                _Footer2 = value;
            }
        }
        public virtual System.String Footer3
        {
            get
            {
                return _Footer3;
            }
            set
            {
                _Footer3 = value;
            }
        }
        public virtual System.String footer4
        {
            get
            {
                return _footer4;
            }
            set
            {
                _footer4 = value;
            }
        }
        public virtual System.String Logo
        {
            get
            {
                return _Logo;
            }
            set
            {
                _Logo = value;
            }
        }
        public virtual System.String VatNo
        {
            get
            {
                return _VatNo;
            }
            set
            {
                _VatNo = value;
            }
        }

    }
}
