using BatteryCommander.Web.Models;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
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
                .Include(evaluation => evaluation.Ratee)
                .Include(evaluation => evaluation.Rater)
                .Include(evaluation => evaluation.SeniorRater)
                .Include(evaluation => evaluation.SeniorRater)
                .AsEnumerable()
                .Where(evaluation => evaluation.LastUpdated > since);

            foreach (var evaluation in evaluations)
            {
                // If it's in a given window, fire off an email to the relevant people

                var recipients = Get_Recipients(evaluation).ToList();

                Log.Information("Sending evaluation updated email to {@recipients}", recipients);

                emailSvc
                    .To(recipients)
                    .Subject($"Evaluation Updated | {evaluation.Ratee}")
                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Views/Reports/EvaluationUpdated.cshtml", evaluation)
                    .Send();
            }
        }

        public IEnumerable<Address> Get_Recipients(Evaluation evaluation)
        {
            // TODO Include the unit 1SG on events

            // TODO Remove after testing

            yield return new Address { EmailAddress = "MattGWagner@Gmail.com" };

            if (!String.IsNullOrWhiteSpace(evaluation.Rater?.CivilianEmail))
            {
                yield return new Address { EmailAddress = evaluation.Rater.CivilianEmail };
            }

            if (!String.IsNullOrWhiteSpace(evaluation.SeniorRater?.CivilianEmail))
            {
                yield return new Address { EmailAddress = evaluation.SeniorRater.CivilianEmail };
            }

            if (!String.IsNullOrWhiteSpace(evaluation.Reviewer?.CivilianEmail))
            {
                yield return new Address { EmailAddress = evaluation.Reviewer.CivilianEmail };
            }
        }
    }
}