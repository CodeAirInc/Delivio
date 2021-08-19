using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delivio.Models
{
    public class SendMailModel
    {
        public string Token { get; set; }
        public string TargetEmail { get; set; }
        public string Subject { get; set; }
        public string Body { get; set; }
    }
}
