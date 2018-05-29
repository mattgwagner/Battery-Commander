using BatteryCommander.Web.Services;
using FluentScheduler;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;

namespace BatteryCommander.Web.Jobs
{
    public class SqliteBackupJob : IJob
    {
        private const String Recipient = "mattgwagner+backup@gmail.com"; // TODO Make this configurable

        private readonly IEmailService emailSvc;

        public SqliteBackupJob(IEmailService emailSvc)
        {
            this.emailSvc = emailSvc;
        }

        public virtual void Execute()
        {
            var data = System.IO.File.ReadAllBytes("Data.db");

            var encoded = System.Convert.ToBase64String(data);

            var message = new SendGridMessage
            {
                From = EmailService.FROM_ADDRESS,
                Subject = "Nightly Db Backup",
                Contents = new List<Content>()
                {
                    new Content
                    {
                        Type = "text/plain",
                        Value = "Please find the nightly database backup attached."
                    }
                },
                Attachments = new List<Attachment>()
                {
                    new Attachment
                    {
                        Filename = "Data.db",
                        Type = "application/octet-stream",
                        Content = encoded
                    }
                }
            };

            message.AddTo(Recipient);

            emailSvc.Send(message).Wait();
        }
    }
}