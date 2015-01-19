using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace BatteryCommander.Common.Services.Notifications
{
    public class EmailService : IIdentityMessageService
    {
        private readonly Config _config;

        public EmailService(Config config)
        {
            _config = config;
        }

        public Task SendAsync(IdentityMessage message)
        {
            throw new System.NotImplementedException();
        }

        private SmtpClient GetClient()
        {
            return new SmtpClient
            {
                Host = _config.Host,
                Port = _config.Port,
                Credentials = new System.Net.NetworkCredential(_config.Username, _config.Password)
            };
        }

        public class Config
        {
            public String Host { get; set; }

            public int Port { get; set; }

            public String Username { get; set; }

            public String Password { get; set; }
        }
    }
}
