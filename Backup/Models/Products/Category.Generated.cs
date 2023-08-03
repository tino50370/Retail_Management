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
    public partial class Category
    {

        private System.Int64 _Id;
        private System.String _location;
        private System.String _status;
        private System.String _name;
        private System.String _dated;
       
   
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

   
      

    }
}
