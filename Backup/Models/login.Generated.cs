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
    public partial class login
    {

        private System.Int64 _Id;
        private System.String _username;
        private System.String _password;
        private System.String _accesslevel;
        private System.String _Firstname;
        private System.String _Surname;
        private System.String _prefix;
        private System.String _Location;
        private System.Boolean _PosUser;
        private System.Boolean _RetailUser;
        private System.Boolean _UsesBoth;
        private System.Boolean _Shifts;
        private System.Boolean _ManagerMenu;
        private System.Boolean _Tables;
        private System.Boolean _Payments;
        private System.Boolean _Reporting;
        private System.Boolean _Sales;
        private System.Boolean _Pricing;
        private System.Boolean _Barcoding;
        private System.Boolean _AddsProducts;
        private System.Boolean _Purchases;
        private System.Boolean _ManageUsers;
        private System.Boolean _ManagePos;
        private System.Boolean _Transfers;
        private System.Boolean _Accounts;
        private System.Boolean _OnShift;
        private System.Boolean _xreport;
        private System.Boolean _zreport;
        private System.Boolean _voidsale;
        

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

        public virtual System.String username
        {
            get
            {
                return _username;
            }
            set
            {
                _username = value;
            }
        }

        public virtual System.String password
        {
            get
            {
                return _password;
            }
            set
            {
                _password = value;
            }
        }

        public virtual System.String accesslevel
        {
            get
            {
                return _accesslevel;
            }
            set
            {
                _accesslevel = value;
            }
        }

        public virtual System.String Firstname
        {
            get
            {
                return _Firstname;
            }
            set
            {
                _Firstname = value;
            }
        }

        public virtual System.String Surname
        {
            get
            {
                return _Surname;
            }
            set
            {
                _Surname = value;
            }
        }

        public virtual System.String prefix
        {
            get
            {
                return _prefix;
            }
            set
            {
                _prefix = value;
            }
        }

        public virtual System.String Location
        {
            get
            {
                return _Location;
            }
            set
            {
                _Location = value;
            }
        }

        public virtual System.Boolean PosUser
        {
            get
            {
                return _PosUser;
            }
            set
            {
                _PosUser = value;
            }
        }

        public virtual System.Boolean RetailUser
        {
            get
            {
                return _RetailUser;
            }
            set
            {
                _RetailUser = value;
            }
        }

        public virtual System.Boolean UsesBoth
        {
            get
            {
                return _UsesBoth;
            }
            set
            {
                _UsesBoth = value;
            }
        }

        public virtual System.Boolean Shifts
        {
            get
            {
                return _Shifts;
            }
            set
            {
                _Shifts = value;
            }
        }

        public virtual System.Boolean ManagerMenu
        {
            get
            {
                return _ManagerMenu;
            }
            set
            {
                _ManagerMenu = value;
            }
        }

        public virtual System.Boolean Tables
        {
            get
            {
                return _Tables;
            }
            set
            {
                _Tables = value;
            }
        }

        public virtual System.Boolean Payments
        {
            get
            {
                return _Payments;
            }
            set
            {
                _Payments = value;
            }
        }

        public virtual System.Boolean Reporting
        {
            get
            {
                return _Reporting;
            }
            set
            {
                _Reporting = value;
            }
        }

        public virtual System.Boolean Sales
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

        public virtual System.Boolean Pricing
        {
            get
            {
                return _Pricing;
            }
            set
            {
                _Pricing = value;
            }
        }

        public virtual System.Boolean Barcoding
        {
            get
            {
                return _Barcoding;
            }
            set
            {
                _Barcoding = value;
            }
        }

        public virtual System.Boolean AddsProducts
        {
            get
            {
                return _AddsProducts;
            }
            set
            {
                _AddsProducts = value;
            }
        }

        public virtual System.Boolean Purchases
        {
            get
            {
                return _Purchases;
            }
            set
            {
                _Purchases = value;
            }
        }

        public virtual System.Boolean ManageUsers
        {
            get
            {
                return _ManageUsers;
            }
            set
            {
                _ManageUsers = value;
            }
        }

        public virtual System.Boolean ManagePos
        {
            get
            {
                return _ManagePos;
            }
            set
            {
                _ManagePos = value;
            }
        }

        public virtual System.Boolean Transfers
        {
            get
            {
                return _Transfers;
            }
            set
            {
                _Transfers = value;
            }
        }

        public virtual System.Boolean Accounts
        {
            get
            {
                return _Accounts;
            }
            set
            {
                _Accounts = value;
            }
        }

        public virtual System.Boolean OnShift
        {
            get
            {
                return _OnShift;
            }
            set
            {
                _OnShift = value;
            }
        }

        public virtual System.Boolean xreport
        {
            get
            {
                return _xreport;
            }
            set
            {
                _xreport = value;
            }
        }

        public virtual System.Boolean zreport
        {
            get
            {
                return _zreport;
            }
            set
            {
                _zreport = value;
            }
        }

        public virtual System.Boolean voidsale
        {
            get
            {
                return _voidsale;
            }
            set
            {
                _voidsale = value;
            }
        }
        
    }
}
