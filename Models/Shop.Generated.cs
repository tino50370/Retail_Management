using System;
using System.Collections.Generic;
using System.ComponentModel;
using Iesi.Collections;
using Iesi.Collections.Generic;
using Yomoney.Models.Components;
using Yomoney.Models.Exceptions;
using Yomoney.Models.Enumerations;
using Yomoney.Models;

namespace Yomoney.Models
{
    /// <summary>
    /// An object representation of the TransactionType table
    /// </summary>
    /// 
    [Serializable]
    public partial class Shop
    {
        protected System.String _Id;
        private System.String _TerminalID;
        private System.String _Mac;
        private System.String _Activation;
        private System.String _ActKey;

        public virtual System.String TerminalID
        {
            get
            {
                return _TerminalID;
            }
            set
            {
                _TerminalID = value;
            }
        }

        public virtual System.String Mac
        {
            get
            {
                return _Mac;
            }
            set
            {
                _Mac = value;
            }
        }
        public virtual System.String ActKey
        {
            get
            {
                return _ActKey;
            }
            set
            {
                _ActKey = value;
            }
        }

        public virtual System.String Activation
        {
            get
            {
                return _Activation;
            }
            set
            {
                _Activation = value;
            }
        }


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

       

    }
}
