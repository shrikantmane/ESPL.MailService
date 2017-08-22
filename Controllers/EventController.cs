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

    public class EventController : Controller
    {
        private readonly IEmailSender _emailSender;
        public EventController(IEmailSender emailSender)
        {
            _emailSender = emailSender;
        }

        [HttpPost]
        [Route("api/[controller]/send")]
        public async Task<IActionResult> send([FromBody]EventWrapper eventWrapper)
        {
            try
            {
                if (eventWrapper != null)
                {
                    #region Validations
                    if (string.IsNullOrWhiteSpace(eventWrapper.smtpOptions.server))
                    {
                        return StatusCode(404, "Please specify the email client");
                    }

                    if (string.IsNullOrWhiteSpace(Convert.ToString(eventWrapper.smtpOptions.port)))
                    {
                        return StatusCode(404, "Please specify the SMTP port");
                    }
                    // else
                    // {
                    //     if(Convert.ToInt32(mailWrapper.smtpOptions.port) !=25 && Convert.ToInt32(mailWrapper.smtpOptions.port) !=587 )
                    //         return StatusCode(500, "Invalid SMTP port");
                    // }

                    if (string.IsNullOrWhiteSpace(eventWrapper.smtpOptions.user))
                    {
                        return StatusCode(404, "Please specify the 'user'");
                    }
                    else
                    {
                        try
                        {
                            var addr = new System.Net.Mail.MailAddress(eventWrapper.smtpOptions.user);
                        }
                        catch (System.Exception ex)
                        {
                            return StatusCode(400, "Invalid SMTP 'user'");
                        }
                    }

                    if (string.IsNullOrWhiteSpace(eventWrapper.smtpOptions.password))
                    {
                        return StatusCode(404, "Please specify the SMTP 'password'");
                    }

                    if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.to))
                    {
                        return StatusCode(404, "'to' address can not be empty");
                    }

                    if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.from))
                    {
                        return StatusCode(404, "'from' can not be empty");
                    }
                    else
                    {
                        try
                        {
                            var addr = new System.Net.Mail.MailAddress(eventWrapper.eventOptions.from);
                        }
                        catch (System.Exception ex)
                        {
                            return StatusCode(400, "Invalid 'from' address");
                        }
                    }

                    // if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.subject))
                    // {
                    //     return StatusCode(404, "'subject' can not be empty");
                    // }

                    if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.replyTo))
                    {
                        return StatusCode(404, "'replyTo' can not be empty");
                    }
                    else
                    {
                        try
                        {
                            var addr = new System.Net.Mail.MailAddress(eventWrapper.eventOptions.replyTo);
                        }
                        catch (System.Exception ex)
                        {
                            return StatusCode(400, "Invalid 'replyTo' address");
                        }
                    }

                    if (string.IsNullOrWhiteSpace(eventWrapper.eventOptions.eventName))
                    {
                        return StatusCode(404, "Please specify 'eventName'");
                    }

                    if (eventWrapper.eventOptions.startTime == DateTime.MinValue)
                    {
                        return StatusCode(404, "Please specify event 'startTime'");
                    }
                    else
                    {
                        if (eventWrapper.eventOptions.startTime == null)
                            return StatusCode(400, "Invalid event 'startTime'");
                    }

                    if (eventWrapper.eventOptions.endTime == DateTime.MinValue)
                    {
                        return StatusCode(404, "Please specify event 'endTime'");
                    }
                    else
                    {
                        if (eventWrapper.eventOptions.endTime == null)
                            return StatusCode(400, "Invalid event 'endTime'");
                    }

                    if (eventWrapper.eventOptions.startTime > eventWrapper.eventOptions.endTime)
                    {
                        return StatusCode(400, "Event 'endTime' should be greater than 'startTime'");
                    }

                    string strRegex = @"^\s*(([a-zA-Z0-9_\-\.]+)@((\[[0-9]{1,3}\.[0-9]{1,3}\.[0-9]{1,3}\.)|(([a-zA-Z0-9\-]+\.)+))([a-zA-Z]{2,4}|[0-9]{1,3})(\]?)(\s*,\s*|\s*$))*$";
                System.Text.RegularExpressions.Regex regX = new System.Text.RegularExpressions.Regex(strRegex);
                 
                //validate email addresses of 'to' users
                string[] toAdrs = eventWrapper.eventOptions.to.Split(',');
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
                    if (!regX.IsMatch(eventWrapper.eventOptions.to))
                            return StatusCode(400, "Invalid 'to' address of" + " " + eventWrapper.eventOptions.to);
                }

                //validate email addresses of 'cc' users
                if(eventWrapper.eventOptions.cc != null)
                    {
                string[] ccAdrs = eventWrapper.eventOptions.cc.Split(',');
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
                    if (!regX.IsMatch(eventWrapper.eventOptions.cc))
                            return StatusCode(400, "Invalid 'cc' address of" + " " + eventWrapper.eventOptions.cc);
                }
                    }

                 //validate email addresses of 'bcc' users   
                if(eventWrapper.eventOptions.bcc != null)
                                {
                            string[] bccAdrs = eventWrapper.eventOptions.bcc.Split(',');
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
                                if (!regX.IsMatch(eventWrapper.eventOptions.bcc))
                                        return StatusCode(400, "Invalid 'bcc' address of" + " " + eventWrapper.eventOptions.bcc);
                            }
                                }
                    #endregion Validations

                    try
                    {
                    var m = _emailSender.generateEventBody(eventWrapper.eventOptions);
                    // if (m != null)
                    // {
                      await _emailSender.SendEmailAsync(m,eventWrapper.smtpOptions);
                    // }
                    return Ok("Email sent!!");
                    }
                    catch (System.Exception ex)
                    {
                        return StatusCode(500, "Something went wrong");
                    }
                }
                else if (eventWrapper == null)
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