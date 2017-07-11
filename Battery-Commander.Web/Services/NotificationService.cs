using BatteryCommander.Web.Models;
using System.Collections.Generic;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;
using Twilio.Types;

namespace BatteryCommander.Web.Services
{
    public class NotificationService
    {
        // Send Email

        // Send SMS

        private readonly PhoneNumber fromNumber;

        public NotificationService(TwilioSettings settings)
        {
            TwilioClient.Init(settings.AccountSid, settings.AuthToken);
            fromNumber = new PhoneNumber(settings.PhoneNumber);
        }

        public async Task Send(SMS message)
        {
            foreach (var recipient in message.Recipients)
            {
                var response = await MessageResource.CreateAsync(
                    to: new PhoneNumber(recipient),
                    from: fromNumber,
                    body: message.Message);

                // TODO Log response for tracking of costs, success, etc.
            }
        }

        public class SMS
        {
            public ICollection<string> Recipients { get; set; } = new List<string>();

            public string Message { get; set; }
        }
    }
}