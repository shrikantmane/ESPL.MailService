 using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace ESPL.MailService.Models
{
 public class EventOptions : BasicMailOptions
    {
        public string eventName { get; set; }
        public DateTime startTime { get; set; }
        public DateTime endTime { get; set; }
        public string eventDescription { get; set; }
        public string location { get; set; }
    }
}