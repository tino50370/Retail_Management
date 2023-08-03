using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using RetailKing.Models;

namespace RetailKing.Models
{
    public class Mail
    {
        public string Subject { set; get; }
        public string recipient { set; get; }
        public string Sender { set; get; }
        public string SenderName { set; get; }
        public string Cc { set; get; }
        public string Company { set; get; }
        public string body { set; get; }
        public string Bcc { set; get; }
        public string Password { set; get; }
        public string Attachments { set; get; }
        public string SeviceName { set; get; }
        public long ServiceId { set; get; }
        public string Filename { set; get; }
    }
}