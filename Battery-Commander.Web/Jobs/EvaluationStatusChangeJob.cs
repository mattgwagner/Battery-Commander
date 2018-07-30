using BatteryCommander.Web.Models;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.IO;
using System.Linq;

namespace BatteryCommander.Web.Jobs
{
    public class EvaluationStatusChangeJob : IJob
    {
        private static TimeSpan EditedTimespan => TimeSpan.FromMinutes(-5);

        private static readonly ILogger Log = Serilog.Log.ForContext<EvaluationStatusChangeJob>();

        private readonly Database db;

        private readonly IFluentEmail emailSvc;

        public EvaluationStatusChangeJob(Database db, IFluentEmail emailSvc)
        {
            this.db = db;
            this.emailSvc = emailSvc;
        }

        public void Execute()
        {
            var since = DateTime.UtcNow.Add(EditedTimespan);

            // Get all evaluations with an event since the configured timespan

            var evaluations =
                db
                .Evaluations
                .Include(evaluation => evaluation.Events)
                .Where(evaluation => evaluation.LastEvent != null && evaluation.LastEvent.Timestamp > since)
                .ToList();

            foreach (var evaluation in evaluations)
            {
                // If it's in a given window, fire off an email to the relevant people

                // TODO Include 1SG?

                //var recipients =
                //        db
                //        .Soldiers
                //        .Include(soldier => soldier.Unit)
                //        .Where(soldier => new[] { evaluation.RaterId, evaluation.SeniorRaterId, evaluation.ReviewerId }.Contains(soldier.Id))
                //        .Where(soldier => !String.IsNullOrWhiteSpace(soldier.CivilianEmail))
                //        .Select(soldier => new Address { EmailAddress = soldier.CivilianEmail, Name = soldier.ToString() })
                //        .ToList();

                var recipients = new[] { new Address { EmailAddress = "mattgwagner@gmail.com" } };

                Log.Information("Sending evaluation updated email to {@recipients}", recipients);

                emailSvc
                    .To(recipients)
                    .Subject($"Evaluation Updated | {evaluation.Ratee}")
                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Reports/EvaluationUpdated.cshtml", evaluation)
                    .Send();
            }
        }
    }
}