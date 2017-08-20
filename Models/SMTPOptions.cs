using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESPL.MailService.Models
{
    public class SMTPOptions
    {
        public string server { get; set; }
        public int port { get; set; }
        public bool enableTLS { get; set; }
        public string user { get; set; }
        public string password { get; set; }
        public bool useSSL { get; set; } = false;
    }
}