using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Delivio.Models;
using Vonage.Request;
using Vonage.Messaging;
using MimeKit;
using MimeKit.Text;
using MailKit.Net.Smtp;
using MailKit.Security;

namespace Delivio.Controllers
{
    [ApiController]
    public class NotificationController : Controller
    {
        private readonly IConfiguration _config;

        public NotificationController(IConfiguration config)
        {
            _config = config;
        }

        /// <summary>
        /// Sends a text message
        /// </summary>
        /// <param name="request">JSON Example: "{"Token":"72098839-E0F7-48D6-954B-5F9674CA12E3","TargetNumber":"17031234567","UserId":"user@gmail.com","SessionId":"fbc6d617-c755-4062-84e5-e76d84254b57"}"</param>
        [HttpPost]
        [Route("Notification/SendCustomerNotification")]
        public IActionResult SendCustomerNotification(SendCustomerNotificationModel customerNotification)
        {
            try
            {
                var token = _config.GetValue<string>("VonageToken");

                if (!token.Equals(customerNotification.Token, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest();
                }

                customerNotification.BodyContent = "Thank you for your order. Your order can be tracked by "
                    + _config.GetValue<string>("DelivioUrl") + "/Home/DeliveryStatus?userId="
                    + customerNotification.UserId + "&sessionId=" + customerNotification.SessionId;

                var vonageApiKey = _config.GetValue<string>("VonageApiKey");
                var vonageApiSecret = _config.GetValue<string>("VonageApiSecret");
                var credentials = Credentials.FromApiKeyAndSecret(vonageApiKey, vonageApiSecret);
                var client = new SmsClient(credentials);
                var request = new SendSmsRequest
                {
                    To = customerNotification.TargetNumber,
                    From = _config.GetValue<string>("VonageFromNumber"),
                    Text = customerNotification.BodyContent
                };
                var response = client.SendAnSms(request);

                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Sends an email with a body passed in
        /// </summary>
        /// <param name="request">JSON Example: "{"Token":"72098839-E0F7-48D6-954B-5F9674CA12E3","TargetEmail":"target@gmail.com","Subject":"Delivery Status","Body":"Your delivery is complete. Thanks."}"</param>
        [HttpPost]
        [Route("Notification/SendEmail")]
        public IActionResult SendEmail(SendMailModel sendEmail)
        {
            try
            {
                var token = _config.GetValue<string>("VonageToken");

                if (!token.Equals(sendEmail.Token, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest();
                }

                var smtpUserName = _config.GetValue<string>("SmtpUserName");
                var smtpPassword = _config.GetValue<string>("SmtpPassword");
                var smtpHost = _config.GetValue<string>("SmtpHost");
                var smtpPort = _config.GetValue<string>("SmtpPort");

                var email = new MimeMessage();
                email.From.Add(new MailboxAddress("Delivio", smtpUserName));
                email.To.Add(MailboxAddress.Parse(sendEmail.TargetEmail));
                email.Subject = sendEmail.Subject;
                email.Body = new TextPart(TextFormat.Html) { Text = sendEmail.Body };

                // send email
                using var smtp = new SmtpClient();
                smtp.Connect(smtpHost, int.Parse(smtpPort), SecureSocketOptions.StartTls);
                smtp.Authenticate(smtpUserName, smtpPassword);
                smtp.Send(email);
                smtp.Disconnect(true);

                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }

        /// <summary>
        /// Sends a text message with a message passed in
        /// </summary>
        /// <param name="request">JSON Example: "{"Token":"72098839-E0F7-48D6-954B-5F9674CA12E3","TargetNumber":"17031234567","BodyContent":"Your delivery is here now."}"</param>
        [HttpPost]
        [Route("Notification/SendNotification")]
        public IActionResult SendNotification(SendCustomerNotificationModel customerNotification)
        {
            try
            {
                var token = _config.GetValue<string>("VonageToken");

                if (!token.Equals(customerNotification.Token, StringComparison.OrdinalIgnoreCase))
                {
                    return BadRequest();
                }

                var vonageApiKey = _config.GetValue<string>("VonageApiKey");
                var vonageApiSecret = _config.GetValue<string>("VonageApiSecret");
                var credentials = Credentials.FromApiKeyAndSecret(vonageApiKey, vonageApiSecret);
                var client = new SmsClient(credentials);
                var request = new SendSmsRequest
                {
                    To = customerNotification.TargetNumber,
                    From = _config.GetValue<string>("VonageFromNumber"),
                    Text = customerNotification.BodyContent
                };
                var response = client.SendAnSms(request);

                return Ok();
            }
            catch
            {
                return NotFound();
            }
        }
    }
}
