using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using ESPL.MailService.Models;

namespace ESPL.MailService.Models
{
    public class MailWrapper
    {
        public SMTPOptions smtpOptions { get; set; }
        public MailOptions mailOptions { get; set; }
    }
}