using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESPL.MailService.Models;
using MailKit;
using MimeKit;

namespace ESPL.MailService.Services
{
    public interface IEmailSender
    {
        Task SendEmailAsync(MimeMessage mimeMessage,SMTPOptions smtpOpt);

        MimeMessage generateMailBody(MailOptions mailOptions);

        MimeMessage generateEventBody(EventOptions eventOptions);
    }
}
