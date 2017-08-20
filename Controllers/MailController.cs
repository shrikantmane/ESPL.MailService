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

                if (string.IsNullOrWhiteSpace(mailWrapper.mailOptions.subject))
                {
                    return StatusCode(404, "'subject' can not be empty");
                }

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

                //valid email addresses of 'to' users
                string[] adrs = mailWrapper.mailOptions.to.Split(',');
                if (adrs.Count() > 1)
                {
                    //      int index = adrs.Length - 1;
                    // System.Net.Mail.MailAddress parsedAddress = MailAddressParser.ParseAddress(adrs, false, ref index);
                    // //Debug.Assert(index == -1, "The index indicates that part of the address was not parsed: " + index);
                    // if(index == -1)
                    //     return StatusCode(500,parsedAddress);
                    // else
                }
                else if (adrs.Count() == 1)
                {
                    try
                    {
                        var addr = new System.Net.Mail.MailAddress(mailWrapper.mailOptions.to);
                    }
                    catch (System.Exception ex)
                    {
                        return StatusCode(400, "Invalid 'to' address");
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

