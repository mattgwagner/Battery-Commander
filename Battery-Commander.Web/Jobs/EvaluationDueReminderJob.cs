using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using SendGrid.Helpers.Mail;
using System;
using System.Linq;
using System.Text;

namespace BatteryCommander.Web.Jobs
{
    public class EvaluationDueReminderJob : IJob
    {
        private readonly Database db;

        private readonly IEmailService emailSvc;

        public EvaluationDueReminderJob(Database db, IEmailService emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
        }

        public void Execute()
        {
            var soon = DateTime.Today.AddDays(15);

            // TODO Filter this if multiple units in-use

            var evaluations_due_soon =
                db
                .Evaluations
                .Include(evaluation => evaluation.Ratee)
                .Include(evaluation => evaluation.Rater)
                .Include(evaluation => evaluation.SeniorRater)
                .Include(evaluation => evaluation.Reviewer)
                .Where(evaluation => evaluation.IsCompleted == false)
                .Where(evaluation => evaluation.ThruDate < soon)
                .OrderBy(evaluation => evaluation.ThruDate)
                .ToList();

            var sb =
                new StringBuilder()
                .Append("<ul>");

            foreach (var evaluation in evaluations_due_soon)
            {
                sb.AppendLine($"<li>{evaluation.Ratee} due {evaluation.ThruDate:d}</li>");
            }

            sb = sb.AppendLine("</ul>");

            var email = new SendGridMessage
            {
                From = EmailService.FROM_ADDRESS,
                Subject = "Upcoming and Past Due Evaluations",
                HtmlContent = sb.ToString()
            };

            // TODO Pull configured recipients, i.e. E-7+, Officers

            //var recipients =
            //    db
            //    .Soldiers
            //    .ToList();

            email.AddTo(email: "mattgwagner+evals@gmail.com");
            email.AddTo(email: "erich.mccartney@gmail.com ");

            emailSvc.Send(email).Wait();
        }
    }
}