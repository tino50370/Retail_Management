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
    public partial class Customers
    {

        private System.Int64 _Id;
        private System.String _AccountCode;
        private System.String _CustomerName;
        private System.String _ContactPerson;
        private System.String _Dated;
       private System.String _Address1;
        private System.String _Address2;
        private System.String _Phone1;
        private System.String _Phone2;
        private System.String _Username;
        
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

        public virtual System.String CustomerName
        {
            get
            {
                return _CustomerName;
            }
            set
            {
                _CustomerName = value;
            }
        }

        public virtual System.String ContactPerson
        {
            get
            {
                return _ContactPerson;
            }
            set
            {
                _ContactPerson = value;
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

        public virtual System.String Phone1
        {
            get
            {
                return _Phone1;
            }
            set
            {
                _Phone1 = value;
            }
        }

        public virtual System.String Phone2
        {
            get
            {
                return _Phone2;
            }
            set
            {
                _Phone2 = value;
            }
        }

        public virtual System.String Username
        {
            get
            {
                return _Username;
            }
            set
            {
                _Username = value;
            }
        }

    }
}
