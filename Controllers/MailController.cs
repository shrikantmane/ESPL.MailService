using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using MailKit.Net.Smtp;
using MailKit;
using ESPL.MailService.Models;
using ESPL.MailService.Services;

namespace ESPL.MailService.Controllers
{

    public class MailController : Controller
    {
        private readonly IEmailSender _emailSender;
        public MailController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("api/[controller]/send")]
        public async Task<IActionResult> send([FromBody]MailWrapper mailWrapper)
        {
            try
            {
                if (mailWrapper != null)
                {
                    #region Validations
                    if (string.IsNullOrWhiteSpace(mailWrapper.smtpOptions.server))
                {
                    return StatusCode(404, "Please specify the email client");
                }

                if (string.IsNullOrWhiteSpace(Convert.ToString(mailWrapper.smtpOptions.port)))
                {
                    return StatusCode(404, "Please specify the SMTP port");
                }
                // else
                // {
                //     if(Convert.ToInt32(mailWrapper.smtpOptions.port) !=25 && Convert.ToInt32(mailWrapper.smtpOptions.port) !=587 )
                //         return StatusCode(500, "Invalid SMTP port");
                // }

                if (string.IsNullOrWhiteSpace(mailWrapper.smtpOptions.user))
                {
                    return StatusCode(404, "Please specify the 'user'");
                }
                else
                {
                    try
                    {
                        var addr = new System.Net.Mail.MailAddress(mailWrapper.smtpOptions.user);
                    }
                    catch (System.Exception ex)
                    {
                        return StatusCode(400, "Invalid SMTP 'user'");
                    }
                }

                if (string.IsNullOrWhiteSpace(mailWrapper.smtpOptions.password))
                {
                    return StatusCode(404, "Please specify the SMTP 'password'");
                }

                if (string.IsNullOrWhiteSpace(mailWrapper.mailOptions.to))
                {
                    return StatusCode(404, "'to' address can not be empty");
                }

                if (string.IsNullOrWhiteSpace(mailWrapper.mailOptions.from))
                {
                    return StatusCode(404, "'from' can not be empty");
                }
                else
                {
                    try
                    {
                        var addr = new System.Net.Mail.MailAddress(mailWrapper.mailOptions.from);
                    }
                    catch (System.Exception ex)
                    {
                        return StatusCode(400, "Invalid 'from' address");
                    }
                }

                // if (string.IsNullOrWhiteSpace(mailWrapper.mailOptions.subject))
                // {
                //     return StatusCode(404, "'subject' can not be empty");
                // }

                if (string.IsNullOrWhiteSpace(mailWrapper.mailOptions.replyTo))
                {
                    return StatusCode(404, "'replyTo' can not be empty");
                }
                else
                {
                    try
                    {
                        var addr = new System.Net.Mail.MailAddress(mailWrapper.mailOptions.replyTo);
                    }
                    catch (System.Exception ex)
                    {
                        return StatusCode(400, "Invalid 'replyTo' address");
                    }
                }
                string strRegex = @"^\s*(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*,\s*|\s*$))*$";
                System.Text.RegularExpressions.Regex regX = new System.Text.RegularExpressions.Regex(strRegex);
                 
                //validate email addresses of 'to' users
                string[] toAdrs = mailWrapper.mailOptions.to.Split(',');
                if (toAdrs.Count() > 1)
                {
                    foreach (var item in toAdrs)
                    {
                        if (!regX.IsMatch(item))
                            return StatusCode(400, "Invalid 'to' address of" + " " + item);
                    }
                }
                else if (toAdrs.Count() == 1)
                {
                    if (!regX.IsMatch(mailWrapper.mailOptions.to))
                            return StatusCode(400, "Invalid 'to' address of" + " " + mailWrapper.mailOptions.to);
                }

                //validate email addresses of 'cc' users
                if(mailWrapper.mailOptions.cc != null)
                    {
                string[] ccAdrs = mailWrapper.mailOptions.cc.Split(',');
                if (ccAdrs.Count() > 1)
                {
                    foreach (var item in ccAdrs)
                    {
                        if (!regX.IsMatch(item))
                            return StatusCode(400, "Invalid 'cc' address of" + " " + item);
                    }
                }
                else if (ccAdrs.Count() == 1)
                {
                    if (!regX.IsMatch(mailWrapper.mailOptions.cc))
                            return StatusCode(400, "Invalid 'cc' address of" + " " + mailWrapper.mailOptions.cc);
                }
                    }

                 //validate email addresses of 'bcc' users   
                if(mailWrapper.mailOptions.bcc != null)
                                {
                            string[] bccAdrs = mailWrapper.mailOptions.bcc.Split(',');
                            if (bccAdrs.Count() > 1)
                            {
                                foreach (var item in bccAdrs)
                                {
                                    if (!regX.IsMatch(item))
                                        return StatusCode(400, "Invalid 'bcc' address of" + " " + item);
                                }
                            }
                            else if (bccAdrs.Count() == 1)
                            {
                                if (!regX.IsMatch(mailWrapper.mailOptions.bcc))
                                        return StatusCode(400, "Invalid 'bcc' address of" + " " + mailWrapper.mailOptions.bcc);
                            }
                                }
                #endregion Validations

                    try
                    {
                    var m = _emailSender.generateMailBody(mailWrapper.mailOptions);
                    // if (m != null)
                    // {
                      await _emailSender.SendEmailAsync(m,mailWrapper.smtpOptions);
                    // }
                    return Ok("Email sent!!");
                    }
                    catch (System.Exception ex)
                    {
                        return StatusCode(500, "Something went wrong");
                    }
                }
                else if (mailWrapper == null)
                    return StatusCode(400, "Invalid parameters");

            }
            catch (System.Exception ex)
            {
                return StatusCode(500, "Something went wrong");
            }
            return StatusCode(500, "Can not send email");
        }

    }
}

