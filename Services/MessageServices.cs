using MailKit.Net.Smtp;
using MimeKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ESPL.MailService.Models;
using MailKit.Net.Smtp;
using MailKit;
using MimeKit;

namespace ESPL.MailService.Services
{
    // This class is used by the application to send Email and SMS
    // when you turn on two-factor authentication in ASP.NET Identity.
    // For more details see this link https://go.microsoft.com/fwlink/?LinkID=532713
    public class MessageSender : IEmailSender
    {
        public async Task SendEmailAsync(MimeMessage mimeMessage,SMTPOptions smtpOpt)
        {
            try
            {
                using (var client = new SmtpClient())
                {
                    client.Connect(smtpOpt.server, smtpOpt.port, smtpOpt.useSSL);
                    client.Authenticate(
                        smtpOpt.user,
                        smtpOpt.password
                        );
                    await client.SendAsync(mimeMessage);
                    await client.DisconnectAsync(true);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public MimeMessage generateEventBody(EventOptions eventOptions)
            {
                var m = new MimeMessage();
            try
            {
 m.From.Add(new MailboxAddress("", eventOptions.from));
                if (!string.IsNullOrWhiteSpace(eventOptions.replyTo))
                {
                    m.ReplyTo.Add(new MailboxAddress("", eventOptions.replyTo));
                }

//'to' users addition
                string[] adrs = eventOptions.to.Split(',');
                if (adrs.Count() > 1)
                {
                    foreach (string item in adrs)
                    {
                        if (!string.IsNullOrEmpty(item)) { m.To.Add(new MailboxAddress("", item)); ; }
                    }
                }
                else if (adrs.Count() == 1)
                {
                    m.To.Add(new MailboxAddress("", eventOptions.to));
                }

                //'cc' users addition
 string[] ccAdrs = eventOptions.cc.Split(',');
                if (ccAdrs.Count() > 1)
                {
                    foreach (string item in ccAdrs)
                    {
                        if (!string.IsNullOrEmpty(item)) { m.Cc.Add(new MailboxAddress("", item)); ; }
                    }
                }
                else if (ccAdrs.Count() == 1)
                {
                    m.Cc.Add(new MailboxAddress("", eventOptions.cc));
                }

                //'bcc' users addition
 string[] bccAdrs = eventOptions.bcc.Split(',');
                if (bccAdrs.Count() > 1)
                {
                    foreach (string item in bccAdrs)
                    {
                        if (!string.IsNullOrEmpty(item)) { m.Bcc.Add(new MailboxAddress("", item)); ; }
                    }
                }
                else if (bccAdrs.Count() == 1)
                {
                    m.Bcc.Add(new MailboxAddress("", eventOptions.bcc));
                }


                m.Subject = eventOptions.subject;

               m.Importance = MessageImportance.Normal;

                BodyBuilder bodyBuilder = new BodyBuilder();

                //create ics file for event
                System.Text.StringBuilder sb = new System.Text.StringBuilder();
                string DateFormat = "yyyyMMddTHHmmssZ";
                string now = DateTime.Now.ToUniversalTime().ToString(DateFormat);
                sb.AppendLine("BEGIN:VCALENDAR");
                sb.AppendLine("PRODID:-//Compnay Inc//Product Application//EN");
                sb.AppendLine("VERSION:2.0");
                sb.AppendLine("METHOD:PUBLISH");

                sb.AppendLine("BEGIN:VEVENT");
                sb.AppendLine("DTSTART:" + eventOptions.startTime.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTEND:" + eventOptions.endTime.ToUniversalTime().ToString(DateFormat));
                sb.AppendLine("DTSTAMP:" + now);
                sb.AppendLine("UID:" + Guid.NewGuid());
                sb.AppendLine("ORGANIZER;CN= " + "Bharati S" + ":MAILTO:" + "bhartishinde01@gmail.com");
                sb.AppendLine("CREATED:" + now);
                sb.AppendLine("X-ALT-DESC;FMTTYPE=text/html:" + eventOptions.eventDescription);
                sb.AppendLine("LAST-MODIFIED:" + now);
                sb.AppendLine("LOCATION:" + eventOptions.location);
                sb.AppendLine("SEQUENCE:0");
                sb.AppendLine("STATUS:CONFIRMED");
                sb.AppendLine("SUMMARY:" + eventOptions.eventName);
                sb.AppendLine("TRANSP:OPAQUE");
                sb.AppendLine("END:VEVENT");

                sb.AppendLine("END:VCALENDAR");

                var calendarBytes = System.Text.Encoding.UTF8.GetBytes(sb.ToString());
                System.IO.MemoryStream ms = new System.IO.MemoryStream(calendarBytes);
                bodyBuilder.Attachments.Add("event.ics", ms);
                m.Body = bodyBuilder.ToMessageBody();

                return m;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            }

            public MimeMessage generateMailBody(MailOptions mailOptions)
            {
                var m = new MimeMessage();
            try
            {
var hasPlainText = !string.IsNullOrWhiteSpace(mailOptions.plainTextMessage);
                var hasHtml = !string.IsNullOrWhiteSpace(mailOptions.htmlMessage);


                m.From.Add(new MailboxAddress("", mailOptions.from));
                if (!string.IsNullOrWhiteSpace(mailOptions.replyTo))
                {
                    m.ReplyTo.Add(new MailboxAddress("", mailOptions.replyTo));
                }

//'to' users addition
                string[] toAdrs = mailOptions.to.Split(',');
                if (toAdrs.Count() > 1)
                {
                    foreach (string item in toAdrs)
                    {
                        if (!string.IsNullOrEmpty(item)) { m.To.Add(new MailboxAddress("", item)); ; }
                    }
                }
                else if (toAdrs.Count() == 1)
                {
                    m.To.Add(new MailboxAddress("", mailOptions.to));
                }
//'cc' users addition
 string[] ccAdrs = mailOptions.cc.Split(',');
                if (ccAdrs.Count() > 1)
                {
                    foreach (string item in ccAdrs)
                    {
                        if (!string.IsNullOrEmpty(item)) { m.Cc.Add(new MailboxAddress("", item)); ; }
                    }
                }
                else if (ccAdrs.Count() == 1)
                {
                    m.Cc.Add(new MailboxAddress("", mailOptions.cc));
                }

                //'bcc' users addition
 string[] bccAdrs = mailOptions.bcc.Split(',');
                if (bccAdrs.Count() > 1)
                {
                    foreach (string item in bccAdrs)
                    {
                        if (!string.IsNullOrEmpty(item)) { m.Bcc.Add(new MailboxAddress("", item)); ; }
                    }
                }
                else if (bccAdrs.Count() == 1)
                {
                    m.Bcc.Add(new MailboxAddress("", mailOptions.bcc));
                }

                m.Subject = mailOptions.subject;

                m.Importance = MessageImportance.Normal;

                BodyBuilder bodyBuilder = new BodyBuilder();
                if (hasPlainText)
                {
                    bodyBuilder.TextBody = mailOptions.plainTextMessage;
                }

                if (hasHtml)
                {
                    bodyBuilder.HtmlBody = mailOptions.htmlMessage;
                }

                m.Body = bodyBuilder.ToMessageBody();

                return m;
            }
            catch (Exception ex)
            {
                throw ex;
            }
            }


    }
}
