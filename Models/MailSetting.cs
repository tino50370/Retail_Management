//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace RetailKing.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class MailSetting
    {
        public long Id { get; set; }
        public string Domain { get; set; }
        public string SmtpHost { get; set; }
        public Nullable<bool> EnableSSl { get; set; }
        public Nullable<bool> UseDefaultCredentials { get; set; }
        public Nullable<int> Port { get; set; }
        public string Company { get; set; }
        public string EmailAddress { get; set; }
        public string Password { get; set; }
        public Nullable<bool> UseExchange { get; set; }
        public Nullable<bool> UseLocalSmtp { get; set; }
    }
}
