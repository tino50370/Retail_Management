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
    public partial class Menu
    {

        private System.Int64 _Id;
        private System.String _button;
        private System.String _caption;
        private System.String _code;
        private System.String _color;
        private System.String _company;
        private System.String _tax;
   
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

        public virtual System.String button
        {
            get
            {
                return _button;
            }
            set
            {
                _button = value;
            }
        }

        public virtual System.String caption
        {
            get
            {
                return _caption;
            }
            set
            {
                _caption = value;
            }
        }

        public virtual System.String code
        {
            get
            {
                return _code;
            }
            set
            {
                _code = value;
            }
        }

        public virtual System.String color
        {
            get
            {
                return _color;
            }
            set
            {
                _color = value;
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

        public virtual System.String tax
        {
            get
            {
                return _tax;
            }
            set
            {
                _tax = value;
            }
        }

   

   
      

    }
}
