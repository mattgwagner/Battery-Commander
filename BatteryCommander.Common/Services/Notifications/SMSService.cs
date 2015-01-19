using Microsoft.AspNet.Identity;
using System;
using System.Threading.Tasks;
using Twilio;

namespace BatteryCommander.Common.Services.Notifications
{
    public class SMSService : IIdentityMessageService
    {
        private readonly Config _config;

        public SMSService(Config config)
        {
            _config = config;
        }

        public Task SendAsync(IdentityMessage message)
        {
            // TODO Use Twilio to send the messages as appropriate

            throw new System.NotImplementedException();
        }

        private TwilioRestClient GetClient()
        {
            return new TwilioRestClient(_config.TwilioAccountSid, _config.TwilioAuthToken);
        }

        public class Config
        {
            public String TwilioAccountSid { get; set; }

            public String TwilioAuthToken { get; set; }

            public String TwilioFromNumber { get; set; }
        }
    }
}