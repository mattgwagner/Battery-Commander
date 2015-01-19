using Microsoft.AspNet.Identity;
using System.Net.Mail;
using System.Threading.Tasks;

namespace BatteryCommander.Common.Services.Notifications
{
    public class EmailService : IIdentityMessageService
    {
        public Task SendAsync(IdentityMessage message)
        {
            throw new System.NotImplementedException();
        }

        private SmtpClient GetClient()
        {
            return new SmtpClient();
        }
    }
}