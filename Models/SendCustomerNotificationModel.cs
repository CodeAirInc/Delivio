using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Delivio.Models
{
    public class SendCustomerNotificationModel
    {
        public string Token { get; set; }
        public string TargetNumber { get; set; }
        public string UserId { get; set; }
        public string SessionId { get; set; }
        public string BodyContent { get; set; }
    }
}
