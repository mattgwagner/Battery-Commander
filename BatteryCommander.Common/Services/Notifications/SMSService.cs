using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BatteryCommander.Common.Services.Notifications
{
    public class SMSService : IIdentityMessageService
    {
        // TODO Use Twilio to send the messages as appropriate

        public Task SendAsync(IdentityMessage message)
        {
            throw new System.NotImplementedException();
        }
    }
}
