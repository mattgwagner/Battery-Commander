using Microsoft.Extensions.Options;
using SendGrid.Helpers.Mail;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Services
{
    public interface IEmailService
    {
        Task Send(SendGridMessage message);
    }

    public class EmailService : IEmailService
    {
        public static readonly EmailAddress FROM_ADDRESS = new EmailAddress("BatteryCommander@red-leg-dev.com");

        private readonly IOptions<SendGridSettings> settings;

        private SendGrid.ISendGridClient client => new SendGrid.SendGridClient(apiKey: settings.Value.APIKey);

        public EmailService(IOptions<SendGridSettings> settings)
        {
            this.settings = settings;
        }

        public virtual async Task Send(SendGridMessage message)
        {
            // TODO Add logging
            // TODO Handle if it doesn't send successfully

            var response = await client.SendEmailAsync(message);
        }
    }
}