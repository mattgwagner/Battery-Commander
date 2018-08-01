using BatteryCommander.Web.Models;
using FluentEmail.Core;
using FluentScheduler;
using System;

namespace BatteryCommander.Web.Jobs
{
    public class SqliteBackupJob : IJob
    {
        private const String Recipient = "Backups@RedLeg.app";

        private readonly IFluentEmail emailSvc;

        public SqliteBackupJob(IFluentEmail emailSvc)
        {
            this.emailSvc = emailSvc;
        }

        public virtual void Execute()
        {
            emailSvc
                .To(Recipient)
                .Subject("Nightly Db Backup")
                .Body("Please find the nightly database backup attached.")
                .Attach(new FluentEmail.Core.Models.Attachment
                {
                    ContentType = "application/octet-stream",
                    Filename = "Data.db",
                    Data = System.IO.File.OpenRead("Data.db")
                })
                .SendWithErrorCheck();
        }
    }
}