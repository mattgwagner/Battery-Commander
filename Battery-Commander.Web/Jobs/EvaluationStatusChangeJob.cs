using BatteryCommander.Web.Models;
using BatteryCommander.Web.Services;
using FluentEmail.Core;
using FluentEmail.Core.Models;
using FluentScheduler;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace BatteryCommander.Web.Jobs
{
    public class EvaluationStatusChangeJob : IJob
    {
        private static TimeSpan EditedTimespan => TimeSpan.FromMinutes(-5);

        private static readonly ILogger Log = Serilog.Log.ForContext<EvaluationStatusChangeJob>();

        private readonly Database db;

        private readonly IFluentEmailFactory emailSvc;

        public EvaluationStatusChangeJob(Database db, IFluentEmailFactory emailSvc)
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
                .Include(evaluation => evaluation.Reviewer)
                .AsEnumerable()
                .Where(evaluation => evaluation.LastUpdated > since);

            foreach (var evaluation in evaluations)
            {
                // If it's in a given window, fire off an email to the relevant people

                var recipients = 
                    Get_Recipients(evaluation)
                    .Distinct(new EmailComparer { })
                    .ToList();

                emailSvc
                    .Create()
                    .To(recipients)
                    .Subject($"Evaluation Updated | {evaluation.Ratee}")
                    .UsingTemplateFromFile($"{Directory.GetCurrentDirectory()}/Jobs/EvaluationUpdated.html", evaluation)
                    .SendWithErrorCheck()
                    .Wait();
            }
        }

        private class EmailComparer : IEqualityComparer<Address>
        {
            public bool Equals(Address x, Address y)
            {
                return String.Equals(x.EmailAddress, y.EmailAddress, StringComparison.CurrentCultureIgnoreCase);
            }

            public int GetHashCode(Address obj) => obj.GetHashCode();
        }

        public IEnumerable<Address> Get_Recipients(Evaluation evaluation)
        {
            yield return new Address { EmailAddress = "Evaluations@RedLeg.app" };

            foreach (var email in evaluation.Rater.GetEmails()) yield return email;

            foreach (var email in evaluation.SeniorRater.GetEmails()) yield return email;

            foreach (var email in evaluation.Reviewer.GetEmails()) yield return email;

            // Include the unit 1SG on event notifications

            foreach (var soldier in SoldierService.Filter(db, new SoldierService.Query { Ranks = new[] { Rank.E8, Rank.E9 }, Unit = evaluation.Ratee.UnitId }).GetAwaiter().GetResult())
            {
                foreach (var email in soldier.GetEmails()) yield return email;
            }
        }
    }
}
